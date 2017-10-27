using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NTemplates;
using NTemplates.EventArgs;
using System.IO;
using System.Threading;

namespace Invoices
{
    public partial class Invoices : Form
    {

        DocumentCreator dc;
        private const string _taxes = "taxes";
        private const string _subtotal = "subtotal";
        private const string _total = "total";
        private const string _extprice = "extPrice";

        private string _inputPath = @"..\..\Templates\InvoiceSample.rtf";
        private string _outputPath = @"..\..\Templates\Invoice.rtf";

        private string _exampleDescriptionPath = @"..\..\ExampleDescription.txt";
        private bool _unitTest = false;

        public Invoices()
        {
            InitializeComponent();
        }

        public Invoices(string inputPath, string outputPath, string exampleDescriptionPath) : this()
        {
            _inputPath = Path.GetFullPath(inputPath);
            _outputPath = Path.GetFullPath(outputPath);
            _exampleDescriptionPath = Path.GetFullPath(exampleDescriptionPath);
            _unitTest = true;
        }

        /// <summary>
        /// Loads data in memory and loads data into it. 
        /// In-memory datatables not necesary need to reflect database tables.
        /// </summary>
        /// <returns></returns>
        private DataSet LoadData()
        {
            DataSet dataset = new DataSet();

            #region Create the invoice header

            DataTable invoice = new DataTable("I"); //TableName is mandatory
            invoice.Columns.Add(new DataColumn("invNum", typeof(string))); //Invoice number
            invoice.Columns.Add(new DataColumn("customer", typeof(string))); //Customer's name
            invoice.Columns.Add(new DataColumn("custAddr", typeof(string))); //Customer's address
            
            #endregion

            #region Create the invoice detail

            DataTable invoiceLine = new DataTable("L"); //TableName is mandatory
            invoiceLine.Columns.Add(new DataColumn("invNum", typeof(string))); //Invoice (header) number
            invoiceLine.Columns.Add(new DataColumn("product", typeof(string))); //Product name
            invoiceLine.Columns.Add(new DataColumn("units", typeof(int))); //Units purchased
            invoiceLine.Columns.Add(new DataColumn("unitPrice", typeof(double))); //Price per unit
            
            #endregion

            //Add an invoice with 4 products
            invoice.Rows.Add(new object[] { "1", "John Doe", "John Doe's Address. Montevideo, Uruguay" });
            invoiceLine.Rows.Add(new object[] { "1", "Striped T-Shirt", 1, 29.75 });
            invoiceLine.Rows.Add(new object[] { "1", "Leather coat", 2, 199.90 });
            invoiceLine.Rows.Add(new object[] { "1", "Sport Shoes", 3, 65.00 });
            invoiceLine.Rows.Add(new object[] { "1", "Green hat", 4, 15.35 });

            //Add tables to the dataset. Not required, but a way to return all tables together.
            dataset.Tables.Add(invoice);
            dataset.Tables.Add(invoiceLine);

            return dataset;
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Create a Dataset containing the required tables.
                DataSet MyData = LoadData();

                //DocumentCreator represents the API to the NTemplates engine, so we need to instantiate it                
                dc = new DocumentCreator();

                //We need to make the DocumentCreator instance aware of the tables by ccalling the AddDataTable method
                //for each need table.
                foreach (DataTable dt in MyData.Tables)
                    dc.AddDataTable(dt);

                //We'are handling some events, so we need to define some event handlers
                dc.BeforeScanRecord += new BeforeScanRecordEventHandler(Dc_BeforeScanRecord);
                dc.ScanEnded += new ScanEndedEventHandler(Dc_ScanEnded);

                //By calling the Add<Type> methods of DocumentCreator, we add variables to the 
                //document creator memory space. We can use those for showing values in the report
                //itself or, we can use them for internal calculations.
                dc.AddDateTime("invDate", DateTime.Today); //Invoice date
                dc.AddDouble(_extprice, 0); //Line total (price * units)  
                dc.AddDouble(_subtotal, 0); //Sum of all extPrices
                dc.AddDouble(_taxes, 0); //Asume this is a calculated result
                dc.AddDouble(_total, 0); // subtotal + taxes

                dc.AddString("title", "Powered by NTemplates");
                dc.AddString("address", "http://ntemplates.codeplex.com");

                //Finally, we command to create the document using one of the CreateDocument method overloads.
                //In this case, we are picking a template from a physical file on disc and generating the report
                //to a physical file too.

                dc.CreateDocument(_inputPath, _outputPath);
                if (!_unitTest)
                System.Diagnostics.Process.Start(_outputPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Dc_BeforeScanRecord(object sender, BeforeScanRecordEventArgs e)
        {
            if (e.TableName == "L" && //The invoice lines are being scanned
                e.MatchesScanCondition) //and the current record matches the scan condition
            {
                DataRow row = ((DataRow)e.Record);
                double extendedPrice = Convert.ToDouble(row["units"]) * Convert.ToDouble(row["unitPrice"]);

                //First time adds the variable. Following times, automatically updates it.
                e.DataManager.AddDouble(_extprice, extendedPrice);
                double subTotal = e.DataManager.GetDouble(_subtotal);
                subTotal += extendedPrice;
                e.DataManager.AddDouble(_subtotal, subTotal);
            }
        }

        /// <summary>
        /// Performs some operations after the SCAN loop is finished.
        /// In this case, we pick the value of the "subtotal" variable and use it to calculate
        /// the (final) total and taxes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Dc_ScanEnded(object sender, ScanEndedEventArgs e)
        {
            //Make sure the loop corresponds to the "Invoice Lines" table.
            //NTemplates raises an event for each scan loop and we need to be sure we're working 
            //with the right one.
            if (e.TableName == "L") 
            {
                double subTotal = e.DataManager.GetDouble(_subtotal);
                double taxes = subTotal * 22 / 100;  //22% taxes
                double total = subTotal + taxes;
                e.DataManager.AddDouble(_taxes, taxes);
                e.DataManager.AddDouble(_total, total);
            }
        }

        public void Invoices_Load(object sender, EventArgs e)
        {
            string descFile = File.ReadAllText(_exampleDescriptionPath);
            txtDescription.Text = descFile;
        }


    }
}
