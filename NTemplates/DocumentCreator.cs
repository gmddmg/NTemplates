using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Text;
using NTemplates.EventArgs;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;

namespace NTemplates
{
    public delegate void ScanStartEventHandler(Object sender, ScanStartEventArgs e);
    public delegate void BeforeScanRecordEventHandler(Object sender, BeforeScanRecordEventArgs e);
    public delegate void AfterScanRecordEventHandler(Object sender, AfterScanRecordEventArgs e);
    public delegate void ScanEndedEventHandler(Object sender, ScanEndedEventArgs e);


    /// <summary>
    /// Represents the whole reporting API
    /// </summary>
    public class DocumentCreator
    {
        Parser parser;

        //Events
        public event ScanStartEventHandler ScanStart;
        public event AfterScanRecordEventHandler AfterScanRecord;
        public event BeforeScanRecordEventHandler BeforeScanRecord;
        public event ScanEndedEventHandler ScanEnded;


        public DocumentCreator(string delimiter, bool preprocessText)
        {
            parser = new Parser(ETextFormat.RTF,delimiter,preprocessText)
            {
                Creator = this
            };
        }

        public DocumentCreator()
        {
            parser = new Parser(ETextFormat.RTF)
            {
                Creator = this
            };
        }

        #region Table and variables handling

        /// <summary>
        /// Adds a DataTable instance to the Data manager. Table must have it's TableName property set, and it must be unique.
        /// </summary>
        public void AddDataTable(DataTable table)
        {
            parser.DataManager.AddDataTable(table);
        }

        /// <summary>
        /// Adds a DataTable instance to the Data manager. Table must have it's TableName property set, and it must be unique.
        /// </summary>
        internal void AddDataTable(DataTable table, string alias)
        {
            parser.DataManager.AddDataTable(table, alias);
        }

        public void AddList<T>(IList<T> list, string alias)
        {
            DataTable dt = new TableList<T>(list).GetDataTable();
            dt.TableName = alias;
            this.AddDataTable(dt);
        }

        /// <summary>
        /// Adds or updates an Int32 variable to the Data Manager. It's name must be unique.
        /// </summary>
        public void AddInt32(string variableName, Int32 value)
        {
            parser.DataManager.AddInt32(variableName, value);
        }

        /// <summary>
        /// Adds or updates a DateTime variable to the Data Manager. It's name must be unique.
        /// </summary>
        public void AddDateTime(string variableName, DateTime value)
        {
            parser.DataManager.AddDateTime(variableName, value);
        }

        /// <summary>
        /// Adds or updates a String variable to the Data Manager. It's name must be unique.
        /// </summary>
        public void AddString(string variableName, string value)
        {
            parser.DataManager.AddString(variableName, value);
        }

        /// <summary>
        /// Adds or updates a Double variable to the Data Manager. It's name must be unique.
        /// </summary>
        public void AddDouble(string variableName, Double value)
        {
            parser.DataManager.AddDouble(variableName, value);
        }

        public void AddBoolean(string variableName, Boolean value)
        {
            parser.DataManager.AddBoolean(variableName, value);
        }
        
        public void AddImage(string variableName, Image value)
        {
            parser.DataManager.AddImage(variableName, value);
        }

        /// <summary>
        /// Retrives the value of an Int32 variable from the Data Manager by it's unique name.
        /// </summary>
        public Int32 GetInt32(string variableName)
        {
            return parser.DataManager.GetInt32(variableName);
        }

        /// <summary>
        /// Retrives the value of a DateTime variable from the Data Manager by it's unique name.
        /// </summary>
        public DateTime GetDateTime(string variableName)
        {
            return parser.DataManager.GetDateTime(variableName);
        }

        /// <summary>
        /// Retrives the value of a String variable from the Data Manager by it's unique name.
        /// </summary>
        public string GetString(string variableName)
        {
            return parser.DataManager.GetString(variableName);
        }

        /// <summary>
        /// Retrives the value of a Double variable from the Data Manager by it's unique name.
        /// </summary>
        public Double GetDouble(string variableName)
        {
            return parser.DataManager.GetDouble(variableName);
        }

        public Boolean GetBoolean(string variableName)
        {
            return parser.DataManager.GetBoolean(variableName);
        }

        /// <summary>
        /// Clears all variables from the Data Manager.
        /// </summary>
        public void ClearVariables()
        {
            parser.DataManager.ResetVariables();
        }

        #endregion

        #region Report Creation

        #region Input from file, output to file

        /// <summary>
        /// Creates a report based on a template, and saves it to disk.
        /// </summary>
        /// <param name="inputFile">The complete input file path, imcluding file name</param>
        /// <param name="outputFile">The complete output file path, imcluding file name</param>
        public void CreateDocument(string inputFile, string outputFile)
        {

            if (inputFile != null && inputFile.Trim() != String.Empty)
            {
                string RTF = File.ReadAllText(inputFile);
                parser.Parse(RTF);
                File.WriteAllText(outputFile, parser.RTFOutput);
            }
            else
                throw new ArgumentException("inputFile cannot be null or empty string");
        }

        /// <summary>
        /// Creates a report based on a template, and saves it to disk.
        /// </summary>
        /// <param name="inputFile">The complete input file path, imcluding file name</param>
        /// <param name="outputFile">The complete output file path, imcluding file name</param>
        //public async void CreateDocumentAsync(string inputFile, string outputFile)
        //{
        //    await CreateDocumentInternalAsync(inputFile, outputFile);     
        //}

        //private Task CreateDocumentInternalAsync(string inputFile, string outputFile)
        //{
        //   return Task.Factory.StartNew(() => CreateDocument(inputFile, outputFile));        
        //}

        #endregion

        #region Input from memory stream, output to file
        

        /// <summary>
        /// Creates a report based on a template, and saves it to disk.
        /// (Doesn't close the input stream)
        /// </summary>
        /// <param name="inputStream">An instance of MemoryStream representing the template.</param>
        /// <param name="outputFile">The complete output file path, imcluding file name</param>
        public void CreateDocument(Stream inputStream, string outputFile)
        {
            string RTF = StreamToText(inputStream);
            parser.Parse(RTF);
            File.WriteAllText(outputFile, parser.RTFOutput);
        }

        /// <summary>
        /// Creates a report based on a template, and saves it to disk.
        /// (Doesn't close the input stream)
        /// </summary>
        /// <param name="inputStream">An instance of MemoryStream representing the template.</param>
        /// <param name="outputFile">The complete output file path, imcluding file name</param>
        //public async void CreateDocumentAsync(Stream inputStream, string outputFile)
        //{
        //    await CreateDocumentInternalAsync(inputStream, outputFile);
        //}

        private Task CreateDocumentInternalAsync(Stream inputStream, string outputFile)
        {
            return Task.Factory.StartNew(() => CreateDocument(inputStream, outputFile));
        }

        #endregion
        
        #region Input from file, output to memory stream

        /// <summary>
        /// Creates a report based on a template, and returns it as a MemoryStream instance
        /// </summary>
        /// <param name="inputFile">The complete input file path, imcluding file name</param>
        /// <returns>MemoryStream</returns>
        public MemoryStream CreateDocumentToMemoryStream(string inputFile)
        {
            if (inputFile != null && inputFile.Trim() != String.Empty)
            {
                string RTF = File.ReadAllText(inputFile);
                parser.Parse(RTF);
                return TextToMemoryStream(parser.RTFOutput);
            }
            else
                throw new ArgumentException("inputFile cannot be null or empty string");
        }

        //public async void CreateDocumentToMemoryStreamAsync(string inputFile)
        //{
        //    await CreateDocumentToMemoryStreamInteralAsync(inputFile);
        //}

        private Task CreateDocumentToMemoryStreamInteralAsync(string inputFile)
        {
            return Task.Factory.StartNew(() => CreateDocumentToMemoryStream(inputFile));
        }


        #endregion

        #region Input from Stream, output to Memorytream

        /// <summary>
        /// Creates a report based on a template, and returns it as a MemoryStream instance
        /// (Doesn't close the input stream)
        /// </summary>
        /// <param name="inputStream">An Stream representing the template.</param>
        /// <returns>MemoryStream</returns>
        public MemoryStream CreateDocumentToMemoryStream(Stream inputStream)
        {
            string RTF = StreamToText(inputStream);
            parser.Parse(RTF);
            return TextToMemoryStream(parser.RTFOutput);
        }

        /// <summary>
        /// Creates a report based on a template, and returns it as a MemoryStream instance
        /// (Doesn't close the input stream)
        /// </summary>
        /// <param name="inputStream">An Stream representing the template.</param>
        /// <returns>MemoryStream</returns>
        //public async void CreateDocumentToMemoryStreamAsync(Stream inputStream)
        //{
        //    await CreateDocumentToMemoryStreamInternalAsync(inputStream);
        //}

        private Task CreateDocumentToMemoryStreamInternalAsync(Stream inputStream)
        {
            return Task.Factory.StartNew(() => CreateDocumentToMemoryStream(inputStream));
        }

        #endregion

        #endregion

        #region Event raising methods


        internal void RaiseScanStartEvent(ScanStartEventArgs e)
        {
            ScanStart?.Invoke(this, e);
        }


        internal void RaiseBeforeScanRecordEvent(BeforeScanRecordEventArgs e)
        {
            BeforeScanRecord?.Invoke(this, e);
        }

        internal void RaiseAfterScanRecordEvent(AfterScanRecordEventArgs e)
        {
            AfterScanRecord?.Invoke(this, e);
        }



        internal void RaiseScanEndedEvent(ScanEndedEventArgs e)
        {
            ScanEnded?.Invoke(this, e);
        }

        #endregion

        #region Utility Functions

        private string StreamToText(Stream stream)
        {
            StreamReader reader;
            reader = new StreamReader(stream, Encoding.ASCII, true);
            string RTF = String.Empty;
            string line;
            do
            {
                line = reader.ReadLine();
                RTF += line;
            } while (line != null);
            return RTF;
        }

        private MemoryStream TextToMemoryStream(string text)
        {
            return new MemoryStream(Encoding.Default.GetBytes(text));
        }

        #endregion


    }
}
