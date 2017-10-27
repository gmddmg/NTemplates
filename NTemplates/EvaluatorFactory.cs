using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using System.Text.RegularExpressions;

namespace NTemplates
{
    public class EvaluatorFactory
    {
        static string newExpression = "";
        public static Assembly CreateEvaluator(string expression, DataManager mgr)
        {
            //Get a list with the needed information of the parameters involved in the expression.
            List<MethodArgument> parameters = GetParameters(expression, mgr);
            //Instantiate a code compiler and set some attributes on it
            CodeDomProvider comp = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("system.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            //Start generating the class that will define the method used for evaluating the expression.
            StringBuilder code = new StringBuilder();
            code.Append("using System; \n");
            code.Append("namespace NTemplates { \n");
            code.Append("  public class _Evaluator { \n");
            string argsStr = "";
            foreach (MethodArgument par in parameters)
            {
                argsStr += par.TypeName + " " + par.Name + ", ";
            }
            if (argsStr.EndsWith(", ")) //enhace this later.. one thay perhaps... in the next century :D
                argsStr = argsStr.Remove(argsStr.LastIndexOf(','));

            code.AppendFormat("    public bool Evaluate({0})\n ", argsStr);
            code.Append("{\n");
            code.AppendFormat("      return ({0});\n", newExpression);
            code.Append("}\n");
            code.Append("}\n }");

            //Compile the generated code and check for errors.
            CompilerResults cr = comp.CompileAssemblyFromSource(cp, code.ToString());
            if (cr.Errors.HasErrors)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Error Compiling Expression: ");
                foreach (CompilerError err in cr.Errors)
                {
                    error.AppendFormat("{0}\n", err.ErrorText);
                }
                throw new Exception("Error Compiling Expression: " + error.ToString());
            }

            return cr.CompiledAssembly;
        }

        private static List<MethodArgument> GetParameters(string expression, DataManager mgr)
        {
            newExpression = expression;
            List<MethodArgument> parameters = new List<MethodArgument>();
            foreach (KeyValuePair<string, string> varInfo in mgr.VariablesMap)
            {
                if (expression.Contains(varInfo.Key))
                {
                    parameters.Add(
                            new MethodArgument()
                            {
                                Name = varInfo.Key,
                                TypeName = varInfo.Value
                            });
                }
            }

            foreach (TableManager tbmgr in mgr.Tables.Values)
            {
                foreach (DataColumn column in tbmgr.Table.Columns)
                {
                    string originalFieldNotation = tbmgr.Table.TableName + "." + column.ColumnName;
                    if (expression.Contains(originalFieldNotation))
                    {
                        MethodArgument arg = new MethodArgument()
                             {
                                 Name = tbmgr.Table.TableName + column.ColumnName,
                                 TypeName = column.DataType.ToString()
                             };
                       parameters.Add(arg);
                       expression = expression.Replace(originalFieldNotation, arg.Name);
                       newExpression = expression;
                    }
                }
            }
            return parameters;
        }
       
        private class MethodArgument
        {
            public string Name { get; set; }
            public string TypeName { get; set; }
        }
    }


}
