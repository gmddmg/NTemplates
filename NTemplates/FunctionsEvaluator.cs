using System;
using System.Text.RegularExpressions;
using NTemplates.DocumentStructure;

namespace NTemplates
{
    public enum TemplateFunctions { DateFormat, DoubleFormat, String, Hyperlink };


    internal class FunctionsEvaluator
    {
        private const string invalidarguments = "Invalid arguments supplied for function {0}";
        private const string quoted = "\"([^\"])*\"";


        public string Format(string data, DataManager manager, TemplateFunctions function)
        {
            Match m = Regex.Match(data, quoted);
            string format = m.ToString().Replace("\"", string.Empty);
            if (m != null)
            {
                try
                {

                    if (m.Index >= data.IndexOf(",")) //Make sure parameters came in the correct order.
                        throw new Exception(String.Format(invalidarguments, GetFunctionName(function)));

                    switch (function)
                    {
                        case TemplateFunctions.DateFormat:
                            {
                                return DateTime.Parse(GetStringData(data, manager, m, function)).ToString(format);
                            }
                        case TemplateFunctions.DoubleFormat:
                            {
                                return Double.Parse(GetStringData(data, manager, m, function)).ToString(format);
                            }
                    }
                }
                catch
                {
                    //Just a patch. If a variable wasn't added to the datamanager it can't be formatted!!
                }
            }
            else
            {
                throw new Exception(String.Format(invalidarguments, GetFunctionName(function)));
            }
            return data; //Invalid case, not expected to happen.
        }

        public string Hyperlink(string data,  DataManager manager)
        {
            //Asume data comes in the following format, where D is the datasource name:
            //#Hlnk(D.TextToShow, D.Hyperlink)#

            int paramStart = data.IndexOf("(")+1;
            int paramEnd = data.IndexOf(")");
            string[] parameters = data.Substring(paramStart, paramEnd - paramStart).Split(',');

            string text = manager.GetCurrentValueForPlaceHolder(new CommonMethods(manager.Parser).Prepare(parameters[0].Trim())).ToString();
            string hplnk = manager.GetCurrentValueForPlaceHolder(new CommonMethods(manager.Parser).Prepare(parameters[1].Trim())).ToString();

            //original
            string ret = "{\\field\\flddirty{\\*\\fldinst {HYPERLINK \"" + hplnk + "\" }}{\\fldrslt{" + text + "}}}";

            return ret;
         
        }
        
        private static string GetFunctionName(TemplateFunctions function)
        {
            switch (function)
            {
                case TemplateFunctions.DateFormat:
                    {
                        return "Dtfmt";
                    }
                case TemplateFunctions.DoubleFormat:
                    {
                        return "Dbfmt";
                    }
                case TemplateFunctions.Hyperlink:
                    {
                        return "Hlnk";
                    }
                default:
                    return "";
            }
        }

        private static string GetStringData(string data, DataManager manager, Match m, TemplateFunctions func)
        {
            string varName = data.Replace(m.ToString(), string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace(",", string.Empty).Replace(GetFunctionName(func), string.Empty).Replace(manager.Parser._d, string.Empty);
            string varValue = manager.GetCurrentValueForPlaceHolder(new CommonMethods(manager.Parser).Prepare(varName)).ToString();
            return varValue;
        }
    }
}
