using NTemplates.EventArgs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace NTemplates.DocumentStructure
{

    internal class ScanBlock : ConditionalBaseBlock, IControlBlock
    {
        
        #region Instance Variables.

        private int start;
        private int end;
        private int lenght;
        private IControlBlock parent;
        private DataTable table;
        private string tableName;

        #endregion

        public Match OpenRegEx { get; set; }

        public Match CloseRegEx { get; set; }

        public List<IControlBlock> Children
        {
            get;
            set;
        }

        public IControlBlock Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public int Lenght
        {
            get { return lenght; }
            set { lenght = value; }
        }

        public int End
        {
            get { return end; }
            set { end = value; }
        }

        public int Start
        {
            get { return start; }
            set { start = value; }
        }


        public int MatchStart
        {
            get;
            set;
        }

        public int MatchEnd { get; set; }

        public bool IsRoot
        {
            get;
            set;
        }

        public BlockType Type
        {
            get
            {
                return BlockType.SCAN;
            }
        }


        public ScanBlock(Parser parser)
        {
            Children = new List<IControlBlock>();
            DocumentParser = parser;
        }

        public ScanBlock(Match start, bool hasCondition, Parser parser)
        {
            Children = new List<IControlBlock>();
            DocumentParser = parser;
            InitializeFromMatch(start, hasCondition);            
        }

        private void InitializeFromMatch(Match start, bool hasCondition)
        {
            string regex = start.ToString().Replace(DocumentParser._d, "");

            string nameSubstring;
            if (hasCondition)
            {
                //Get the table's name
                string[] scanfor = regex.Split(new string[] { "FOR" }, StringSplitOptions.None);
                nameSubstring = scanfor[0].Trim();
                int ob = nameSubstring.IndexOf("(");
                int cb = nameSubstring.IndexOf(")");
                int le = nameSubstring.Length - ob - 2;
                TableName = nameSubstring.Substring(ob + 1, le);

                //...and the loop condition
                string condSubstring = scanfor[1].Trim();
                ob = condSubstring.IndexOf("(");
                cb = condSubstring.IndexOf(")");
                le = condSubstring.Length - ob - 2;
                Condition = condSubstring.Substring(ob + 1, le);
            }
            else
            {
                //Condition = "";
                int ob = regex.IndexOf("(");
                int le = regex.Length - ob - 2;
                TableName = regex.Substring(ob + 1, le);
            }
        }

        public string TableName
        {
            get
            {
                return tableName;
            }
            internal set
            {
                tableName = value;
            }
        }

        public DataTable Table
        {
            get { return table; }
            internal set
            {
                table = value;
            }
        }


        public void Expand()
        {
            int pos = 0;

            DocumentParser.RaiseScanStartEvent(new ScanStartEventArgs(tableName, DocumentParser.DataManager));
            foreach (DataRow row in this.Table.Rows)
            {
                #region BeforeScanRecord event
 
                bool isMatch = MatchesCondition();
                BeforeScanRecordEventArgs beforeScanEvArgs = new BeforeScanRecordEventArgs(row, pos, DocumentParser.DataManager, isMatch);
                DocumentParser.RaiseBeforeScanRecordEvent(beforeScanEvArgs);

                if (beforeScanEvArgs.Cancel)
                    break;

                if (beforeScanEvArgs.Skip)
                {
                    pos++;
                    DocumentParser.DataManager.IncrementCurrentRecord(this.TableName);
                    continue;
                }

                #endregion

                if (isMatch)
                {
                    foreach (IControlBlock cb in Children)
                        cb.Expand();
                }

                #region AfterScanRecord event

                AfterScanRecordEventArgs afterScanEvArgs = new AfterScanRecordEventArgs(row, pos, DocumentParser.DataManager, isMatch);
                DocumentParser.RaiseAfterScanRecordEvent(afterScanEvArgs);
                if (afterScanEvArgs.Cancel)
                    break;

                #endregion
                
                pos++;
                DocumentParser.DataManager.IncrementCurrentRecord(TableName);
            }

            DocumentParser.DataManager.ResetCurrentRecord(tableName);
            DocumentParser.RaiseScanEndedEvent(new ScanEndedEventArgs(tableName, DocumentParser.DataManager));

        }


        public string InnerText { get; set; }
    }
}