using System;
using System.Collections.Generic;
using System.Reflection;

namespace NTemplates.DocumentStructure
{
    internal class ConditionalBaseBlock
    {
        private Parser _documentParser;
        protected CommonMethods _commons;
        private Assembly _evaluator;
        private string _condition;


        internal string Condition
        {
            get { return _condition; }
            set
            {
                _condition = value;
                if (_evaluator == null)
                {
                    //Compile just the first time. Yeah !! ;)
                    _evaluator = EvaluatorFactory.CreateEvaluator(_condition, this.DocumentParser.DataManager);
                }
            }
        }
        
        public Parser DocumentParser
        {
            get { return _documentParser; }
            set
            {
                _documentParser = value;
                //Instantiate a "common methods" instance
                _commons = new CommonMethods(value);
            }
        }

        protected bool MatchesCondition()
        {
            
            if (String.IsNullOrEmpty(Condition))
                return true; //No condition
            else
            {
                List<string> placeHolders = DocumentParser.DataManager.GetAllPlaceHolders(false);
                List<object> parameters = new List<object>();
                foreach (string ph in placeHolders)
                {
                    string placeholder = ph.Replace(_documentParser._d, "");
                    if (Condition.Contains(placeholder))
                    {
                        object parameter = DocumentParser.DataManager.GetCurrentValueForPlaceHolderObject(placeholder, false);
                        parameters.Add(parameter);
                    }
                }
                object instance = _evaluator.CreateInstance("NTemplates._Evaluator");
                MethodInfo mi = instance.GetType().GetMethod("Evaluate");
                
                return (bool)mi.Invoke(instance, parameters.ToArray());
            }
        }
    }
}
