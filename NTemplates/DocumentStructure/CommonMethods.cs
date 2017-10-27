using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

namespace NTemplates.DocumentStructure
{
    internal class CommonMethods
    {
        Parser _parser;

        internal CommonMethods(Parser p)
        {
            _parser = p;
        }

        internal string Prepare(string placeholder)
        {
            return _parser._d + placeholder + _parser._d;
        }

        /// <summary>
        /// Should move this to the "Control" classes hierarchy.
        /// </summary>
        /// <param name="OpenRegex"></param>
        /// <param name="textAux"></param>
        /// <returns></returns>
        internal string GetReplacementsForAllPlaceHolders(string textAux)
        {
            try
            {
                List<string> placeHolders = _parser.DataManager.GetAllPlaceHolders();
                foreach (string ph in placeHolders)
                {
                    object value = _parser.DataManager.GetCurrentValueForPlaceHolder(ph);
                    if (value != null)
                        textAux = textAux.Replace(ph, value.ToString()).Trim();
                }
                return GetReplacementsForFunctions(textAux);
            }
            catch
            {
                return null;
            }
        }
        
        internal string GetReplacementsForFunctions(string text)
        {
            Regex funcs = GetRegex(_parser.FunctionsRegexString);
            MatchCollection matches = funcs.Matches(text);
            string matchString;
            FunctionsEvaluator FEvaluator = new FunctionsEvaluator();

            foreach (Match m in matches)
            {
                matchString = m.ToString();

                if (Regex.Matches(matchString, _parser._dtfmt).Count > 0) 
                {
                    //DateTimeFormat
                    text = text.Replace(m.ToString(), FEvaluator.Format(matchString, this._parser.DataManager, TemplateFunctions.DateFormat));
                }
                else
                {
                    if (Regex.Matches(matchString, _parser._dbfmt).Count > 0) 
                    {
                        //DoubleFormat
                        text = text.Replace(m.ToString(), FEvaluator.Format(matchString, _parser.DataManager, TemplateFunctions.DoubleFormat));
                    }
                    else
                    {
                        if (Regex.Matches(matchString, _parser._hlnk).Count > 0) 
                        {
                            //Hyperlink
                            text = text.Replace(m.ToString(), FEvaluator.Hyperlink(matchString, _parser.DataManager));
                        }
                    }
                }
            }
            return text.Trim();
        }


        internal static Regex GetRegex(string text)
        {
            Regex regex = new Regex(text, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Singleline);
            return regex;
        }
        
        internal static KeyWords GetKeyWord(Match match)
        {
            //TODO: Reemplazar los valores hard-coded por las constantes para cada sentencia

            string sMatch = match.ToString();
            if (sMatch.Contains("SCAN(") && sMatch.Contains("FOR"))
            {
                return KeyWords.SCAN_FOR;
            }
            else
            {
                if (sMatch.Contains("SCAN("))
                {
                    return KeyWords.SCAN;
                }
                else
                {
                    if (sMatch.Contains("ENDSCAN"))
                    {
                        return KeyWords.ENDSCAN;
                    }
                    else
                    {
                        if (sMatch.Contains("ELSE"))
                        {
                            return KeyWords.ELSE;
                        }
                        else
                        {
                            if (sMatch.Contains("ENDIF"))
                            {
                                return KeyWords.ENDIF;
                            }
                            else
                            {
                                if (sMatch.Contains("IF"))
                                {
                                    return KeyWords.IF;
                                }
                                else
                                {
                                    return KeyWords.NONE;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
