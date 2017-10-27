using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NTemplates;
using System.Diagnostics;

namespace ParentChild
{
    public partial class Form1 : Form
    {
        private bool _unitTest = false;
        private string _inputPath = @"..\..\Templates\ParentChildSample.rtf";
        private string _outputPath = @"..\..\Templates\ParentChild.rtf";

        public Form1()
        {
            InitializeComponent();
        }
        
        public Form1(string inputPath, string outputPath) : this ()
        {
            _inputPath = inputPath;
            _outputPath = outputPath;
            _unitTest = true;
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            DataTable Parent = new DataTable("P"); 
            Parent.Columns.Add(new DataColumn("id", typeof(int))); 
            Parent.Columns.Add(new DataColumn("description", typeof(string)));
            
            DataTable Child = new DataTable("C"); //TableName is mandatory
            Child.Columns.Add(new DataColumn("id", typeof(int)));
            Child.Columns.Add(new DataColumn("parentID", typeof(int)));
            Child.Columns.Add(new DataColumn("description", typeof(string)));

            DataTable GrandChild = new DataTable("GRCH"); //TableName is mandatory
            GrandChild.Columns.Add(new DataColumn("id", typeof(int)));
            GrandChild.Columns.Add(new DataColumn("parentID", typeof(int)));
            GrandChild.Columns.Add(new DataColumn("description", typeof(string)));

            Parent.Rows.Add(new object[] { "1", "Parent 1"});
            Parent.Rows.Add(new object[] { "2", "Parent 2" });
            Parent.Rows.Add(new object[] { "3", "Parent 3" });

            Child.Rows.Add(new object[] { "1", "1", "Parent 1 Child 1" });
            Child.Rows.Add(new object[] { "2", "1", "Parent 1 Child 2" });
            Child.Rows.Add(new object[] { "3", "1", "Parent 1 Child 3" });

            Child.Rows.Add(new object[] { "4", "2", "Parent 2 Child 1" });
            Child.Rows.Add(new object[] { "5", "2", "Parent 2 Child 2" });
            Child.Rows.Add(new object[] { "6", "2", "Parent 2 Child 3" });

            GrandChild.Rows.Add(new object[] { "1", "1", "Parent 1 Child 1 Grand Child 1" });
            GrandChild.Rows.Add(new object[] { "2", "1", "Parent 1 Child 1 Grand Child 2" });
            GrandChild.Rows.Add(new object[] { "3", "1", "Parent 1 Child 1 Grand Child 3" });

            DocumentCreator dc = new DocumentCreator();
            dc.ScanStart += new ScanStartEventHandler(Dc_ScanStart);
            dc.AddDataTable(Parent);
            dc.AddDataTable(Child);
            dc.AddDataTable(GrandChild);
            dc.CreateDocument(_inputPath, _outputPath);

            if (!_unitTest)
            Process.Start(_outputPath);

        }

        void Dc_ScanStart(object sender, NTemplates.EventArgs.ScanStartEventArgs e)
        {
            
        }
    }
}
