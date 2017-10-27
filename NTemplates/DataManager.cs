using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NTemplates.DocumentStructure;
using System.Drawing;

namespace NTemplates
{
    /// <summary>
    /// Responsible for handling data. Provides methods for adding and accessing data.
    /// </summary>
    public class DataManager
    {
        Dictionary<string, TableManager> tables = new Dictionary<string, TableManager>();
        Dictionary<string, string> variableMap = new Dictionary<string, string>();
        Dictionary<string, Int32> int32Variables = new Dictionary<string, Int32>();
        Dictionary<string, Double> doubleVariables = new Dictionary<string, Double>();
        Dictionary<string, Boolean> booleanVariables = new Dictionary<string, Boolean>();
        Dictionary<string, String> stringVariables = new Dictionary<string, String>();
        Dictionary<string, DateTime> dateTimeVariables = new Dictionary<string, DateTime>();
        Dictionary<string, Image> imageVariables = new Dictionary<string, Image>();
        public Parser Parser { get; private set; }
        public DataManager(Parser parser)
        {
            Parser = parser;
        }
        internal Dictionary<string, string> VariablesMap
        {
            get
            {
                return variableMap;
            }
        }
        
        public void AddDataTable(DataTable table)
        {
            if (table.TableName != "")
                tables.Add(table.TableName, new TableManager(table));
            else
                throw new Exception("DataTable.TableName property must be set before adding the DataTable object");
        }

        internal void AddDataTable(DataTable table, string alias)
        {
            table.TableName = alias;
            if (table.TableName != "")
                tables.Add(table.TableName, new TableManager(table));
            else
                throw new Exception("DataTable.TableName property must be set before adding the DataTable object");
        }

        public void ResetTables()
        {
            tables = new Dictionary<string, TableManager>();
        }

        internal void ResetRecordPositions()
        {
            foreach (string tableName in Tables.Keys)
                ResetCurrentRecord(tableName);
        }

        internal Dictionary<string, TableManager> Tables
        {
            get { return tables; }
        }

        public List<ErrorInRecord> ErrorsInRecords { get; private set; }

        /// <summary>
        /// Returns a list of all possible placeholders (data fields).
        /// </summary>
        /// <returns></returns>
        internal List<String> GetAllPlaceHolders(bool enclose)
        {
            List<String> result = new List<string>();
            foreach (TableManager tbmgr in Tables.Values)
            {
                string prefix = tbmgr.Table.TableName + ".";
                foreach (DataColumn column in tbmgr.Table.Columns)
                {
                    string sufix = column.ColumnName;
                    if (enclose)
                    {
                        result.Add(new CommonMethods(Parser).Prepare(prefix + sufix));
                    }
                    else
                        result.Add(prefix + sufix);
                }
            }

            result.AddRange(GetPlaceholdersForVariables());
            result.Add(Parser._pgbrk);

            return result;
        }

        internal List<String> GetAllPlaceHolders()
        {
            return GetAllPlaceHolders(true);
        }

        internal List<string> GetPlaceholdersForVariables()
        {
            return variableMap.Keys.Select(var => new CommonMethods(Parser).Prepare(var)).ToList<string>();
        }

        internal string GetCurrentValueForPlaceHolder(string field)
        {
            return GetCurrentValueForPlaceHolderAsString(field, false);
        }

        /// <summary>
        /// Returns the value for a field from the current row
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal string GetCurrentValueForPlaceHolderAsString(string field, bool quotesIfString)
        {
            try
            {
                object theValue = GetCurrentValueForPlaceHolderObject(field, quotesIfString);
                if (theValue != null)
                {
                    if (theValue.GetType() == typeof(Image) ||
                        theValue.GetType() == typeof(Bitmap))
                    {
                        //Don't want' the RTF Text in this case, but the RTF Code itself

                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        Image img = (Image)theValue;
                        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                        byte[] bytes = stream.ToArray();

                        string str = BitConverter.ToString(bytes, 0).Replace("-", string.Empty).Trim();
                        string mpic = @"{\pict\pngblip\picscalex100\picscaley100\picw" +
                            (img.Width).ToString().Trim() + @"\pich" + (img.Height).ToString().Trim() +
                            @"\picwgoal" + (img.Width * 15).ToString().Trim() + @"\pichgoal" + (img.Height * 15).ToString().Trim() +
                            @"\hex " + str + "}";
                        return mpic.Trim();
                    }
                    return GetRtfText(theValue.ToString().Trim(), Encoding.UTF32);
                }
            }
            catch (Exception ex)
            {
                //Get the table manager for grabbing further info to build the error object
                string[] parts = field.Trim().Split('.');
                string fieldName = parts[1].Replace(Parser._d, "");
                TableManager tbmgr = Tables[parts[0].Replace(Parser._d, "")];
                ErrorInRecord err = new ErrorInRecord()
                {
                    FieldName = field,
                    RecordPosition = tbmgr.CurrentRecord,
                    TypeName = tbmgr.Table.TableName,
                    ExceptionThrown = ex
                };

                if (ErrorsInRecords == null)
                    ErrorsInRecords = new List<ErrorInRecord>();

                ErrorsInRecords.Add(err);

                return "";

            }
            return field.Trim();
        }
        
        

        /// <summary>
        /// Returns the value for a field from the current row
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        internal object GetCurrentValueForPlaceHolderObject(string field, bool quotesIfString)
        {

            if (field == Parser._pgbrk)
                return @"\page";

            object theValue = field;

            //It's a field from a table / list
            if (field.Contains("."))
            {
                string[] parts = field.Trim().Split('.');
                string fieldname = parts[1].Replace(Parser._d, "");
                TableManager tbmgr = Tables[parts[0].Replace(Parser._d, "")];
                DataRow currentRow = tbmgr.Table.Rows[tbmgr.CurrentRecord];
                if (currentRow.Table.Columns[fieldname].DataType == typeof(string))
                {
                    if (quotesIfString)
                        theValue = "\"" + currentRow[fieldname] + "\"";
                    else
                        theValue = currentRow[fieldname];
                }
                else
                {
                    theValue = currentRow[fieldname];
                }
            }
            else
            {
                //It's a variable
                string variableName = field.Replace(Parser._d, "");
                if (variableMap.ContainsKey(variableName))
                {
                    switch (variableMap[variableName])
                    {
                        case "System.Int32":
                            theValue = int32Variables[variableName];
                            break;
                        case "System.Double":
                            theValue = doubleVariables[variableName];
                            break;
                        case "System.DateTime":
                            theValue = dateTimeVariables[variableName];
                            break;
                        case "System.String":
                            if (quotesIfString)
                                theValue = "\"" + stringVariables[variableName] + "\"";
                            else
                                theValue = stringVariables[variableName];
                            break;
                        case "System.Boolean":
                            theValue = booleanVariables[variableName];
                            break;
                        case "System.Drawing.Image":
                        case "System.Drawing.Bitmap":
                            theValue = imageVariables[variableName];
                            break;
                    }
                }
            }

            return theValue;
        }
        

        /// <summary>
        /// Concatenates 2 strings using the template encoding
        /// </summary>
        internal static string GetRtfText(string s, Encoding enc)
        {
            //Contributed by Jan Stuchlík
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                int code = Char.ConvertToUtf32(s, i);

                if (code >= 128 || code < 32)
                {
                    res.Append(@"\'");
                    byte[] bytes = enc.GetBytes(new char[] { s[i] });
                    res.Append(GetHexa(bytes[0]));
                }
                else
                {
                    res.Append(s[i]);
                }
            }
            return res.ToString();
        }

        /// <summary>
        /// Gets the hexa code for an integer number
        /// </summary>
        private static string GetHexa(int code)
        {
            //Contributed by Jan Stuchlík
            string hexa = Convert.ToString(code, 16);

            if (hexa.Length == 1)
            {
                hexa = "0" + hexa;
            }

            return hexa;
        }

        internal void IncrementCurrentRecord(string tableName)
        {
            TableManager mgr = Tables[tableName];
            if (mgr.CurrentRecord < mgr.Table.Rows.Count-1)
                mgr.CurrentRecord++;
        }

        internal void ResetCurrentRecord(string tableName)
        {
            TableManager mgr = Tables[tableName];
            mgr.CurrentRecord = 0;
        }

        #region Variable handling
        
        private void UpdateMap(string variableName, object value)
        {
            if (!variableMap.ContainsKey(variableName))
                variableMap.Add(variableName, value.GetType().ToString());
        }
        
        public void AddInt32(string variableName, Int32 value)
        {
            int32Variables.AddOrUpdate<string, Int32>(variableName, value);
            UpdateMap(variableName, value);
        }
        
        public void AddDateTime(string variableName, DateTime value)
        {
            dateTimeVariables.AddOrUpdate<string, DateTime>(variableName, value);
            UpdateMap(variableName, value);
        }

        public void AddString(string variableName, String value)
        {
            stringVariables.AddOrUpdate<string, string>(variableName, value);
            UpdateMap(variableName, value);
        }

        public void AddDouble(string variableName, Double value)
        {
            doubleVariables.AddOrUpdate<string, Double>(variableName, value);
            UpdateMap(variableName, value);
        }

        public void AddBoolean(string variableName, bool value)
        {
            booleanVariables.AddOrUpdate<string, Boolean>(variableName, value);
            UpdateMap(variableName, value);
        }

        public void AddImage(string variableName, Image value)
        {
            imageVariables.AddOrUpdate<string, Image>(variableName, value);
            UpdateMap(variableName, value);
        }

        public Int32 GetInt32(string variableName)
        {
            return int32Variables[variableName];
        }

        public DateTime GetDateTime(string variableName)
        {
            return dateTimeVariables[variableName];
        }

        public String GetString(string variableName)
        {
            return stringVariables[variableName];
        }

        public Double GetDouble(string variableName)
        {
            return doubleVariables[variableName];
        }

        public Boolean GetBoolean(string variableName)
        {
            return booleanVariables[variableName];
        }

        public Image GetImage(string variableName)
        {
            return imageVariables[variableName];
        }        

        internal void RemoveVariable(string variableName)
        {           
            if (variableMap.ContainsKey(variableName))
            {
                switch (variableMap[variableName])
                {
                    case "System.Int32":
                        int32Variables.Remove(variableName);
                        break;
                    case "System.Double":
                        doubleVariables.Remove(variableName);
                        break;
                    case "System.DateTime":
                        dateTimeVariables.Remove(variableName);
                        break;
                    case "System.String":
                        stringVariables.Remove(variableName);
                        break;
                    case "System.Boolean":
                        booleanVariables.Remove(variableName);
                        break;
                }
            }
            
        }
               
        internal void ResetVariables()
        {
            int32Variables.Clear();
            stringVariables.Clear();
            doubleVariables.Clear();
            dateTimeVariables.Clear();
            booleanVariables.Clear();
        }
       
        #endregion

        /// <summary>
        /// Returns the record in the next position without moving the pointer.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataRow GetNextRecord(string tableName)
        {
            TableManager mgr = Tables[tableName];
            int next = mgr.CurrentRecord + 1;
            if (mgr != null)
            {
                if (next < mgr.Table.Rows.Count)
                    return mgr.Table.Rows[next];
            }
            return null;
        }
        
    }
}
