using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using NTemplates;
using NTemplates.EventArgs;
using System.Diagnostics;
using System.IO;

namespace ProductListWithGroupings
{
    public partial class ProductListWithGroupings : Form
    {

        List<Product> products = new List<Product>();
        private string _inputPath = @"..\..\Templates\ProductListWithGroupingsSample.rtf";
        private string _outputPath = @"..\..\Templates\ProductList.rtf";
        private string _descriptionPath = @"..\..\ExampleDescription.txt";
        private bool _isUnitTest = false;

        public ProductListWithGroupings(string inputPath, string outputPath, string descriptionPath) : this()
        {
            _inputPath = inputPath;
            _outputPath = outputPath;
            _descriptionPath = descriptionPath;
            _isUnitTest = true;
        }

        public ProductListWithGroupings()
        {
            InitializeComponent();

            //Load a list of products. This data could come from a database or a webservice.
            //For grouping purposes, we asume the informatin is sorted by Vendor
            products.Add(new Product() { Code = "pr000", Vendor = "Vendor A", Desc = "Product 000", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr001", Vendor = "Vendor A", Desc = "Product 001", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr002", Vendor = "Vendor A", Desc = "Product 002", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr003", Vendor = "Vendor A", Desc = "Product 003", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr004", Vendor = "Vendor B", Desc = "Product 004", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr005", Vendor = "Vendor B", Desc = "Product 005", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr006", Vendor = "Vendor B", Desc = "Product 006", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr007", Vendor = "Vendor B", Desc = "Product 007", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr008", Vendor = "Vendor C", Desc = "Product 008", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr009", Vendor = "Vendor C", Desc = "Product 009", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr010", Vendor = "Vendor C", Desc = "Product 010", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr011", Vendor = "Vendor C", Desc = "Product 011", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr012", Vendor = "Vendor D", Desc = "Product 012", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr013", Vendor = "Vendor D", Desc = "Product 013", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr014", Vendor = "Vendor D", Desc = "Product 014", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr015", Vendor = "Vendor D", Desc = "Product 015", IsAvailable = "Yes" });

            bindingSource1.DataSource = products;

            
            dgvProducts.DataSource = bindingSource1;
            dgvProducts.AutoGenerateColumns = false;
            
        }

        public class Product
        {            
            public string Code { get; set; }
            public string Vendor { get; set; }
            public string Desc { get; set; }
            public string IsAvailable
            {
                get;
                set;
            }
            
        }

        string currentVendor;
        public void BtnProductList_Click(object sender, EventArgs e)
        {            
            DocumentCreator dc = new DocumentCreator();
            dc.AddList<Product>(products, "P");

            //Group Totals
            dc.AddInt32("avail", 0);
            dc.AddInt32("unavail", 0);
            dc.AddInt32("tot", 0);

            //Final totals
            dc.AddInt32("available", 0);
            dc.AddInt32("unavailable", 0);
            dc.AddInt32("total", 0);

            dc.AddInt32("showVendor", 0);
            dc.AddBoolean("groupEnd", false);

            currentVendor = "-none-";

            dc.BeforeScanRecord += new BeforeScanRecordEventHandler(Dc_BeforeScanRecord);
            dc.AfterScanRecord += new AfterScanRecordEventHandler(Dc_AfterScanRecord1);
            dc.ScanEnded += new ScanEndedEventHandler(Dc_ScanEnded1);
            dc.CreateDocument(_inputPath, _outputPath);
            if (!_isUnitTest)
            Process.Start(_outputPath);
        }

        
        void Dc_BeforeScanRecord(object sender, BeforeScanRecordEventArgs e)
        {

            //Decide if must show group header
            if (e.Record[1].ToString() != currentVendor)
            {
                e.DataManager.AddInt32("showVendor", 1);
                currentVendor = e.Record[1].ToString();
            }
            else
            {
                e.DataManager.AddInt32("showVendor", 0);
            }

            int avail = e.DataManager.GetInt32("avail");
            int unavail = e.DataManager.GetInt32("unavail");
            if (e.Record[3].ToString() == "Yes")
                avail++;
            else
                unavail++;
            e.DataManager.AddInt32("avail", avail);
            e.DataManager.AddInt32("unavail", unavail);
            e.DataManager.AddInt32("tot", avail + unavail);
            
            //Decide if must shows totals per group
            DataRow row = e.DataManager.GetNextRecord(e.TableName);
            if (row != null)
            {
                if (e.Record[1].ToString() != row[1].ToString())
                {
                    e.DataManager.AddBoolean("groupEnd", true);
                }
                else
                {
                    e.DataManager.AddBoolean("groupEnd", false);                   
                }
            }
            else
            {
                e.DataManager.AddBoolean("groupEnd", true);
            }

        }

        void Dc_ScanEnded1(object sender, ScanEndedEventArgs e)
        {
            if (e.TableName == "P")
            {
                int av = e.DataManager.GetInt32("available");
                int unav = e.DataManager.GetInt32("unavailable");

                e.DataManager.AddInt32("total", av + unav);
            }
        }

        private void Dc_AfterScanRecord1(object sender, AfterScanRecordEventArgs e)
        {
            if (e.TableName == "P")
            {
                //Count available and unavailable products (Totals)
                int available = e.DataManager.GetInt32("available");
                int unavailable = e.DataManager.GetInt32("unavailable");
                if (e.Record[3].ToString() == "Yes")
                    available++;
                else
                    unavailable++;
                e.DataManager.AddInt32("available", available);
                e.DataManager.AddInt32("unavailable", unavailable);

                if (e.DataManager.GetBoolean("groupEnd"))
                {
                    e.DataManager.AddInt32("avail", 0);
                    e.DataManager.AddInt32("unavail", 0);
                    e.DataManager.AddInt32("tot", 0);
                }
                
            }
        }

        public void ProductList_Load(object sender, EventArgs e)
        {
            string descFile = File.ReadAllText(_descriptionPath);
            txtDescription.Text = descFile;
        }
        
    }
}
