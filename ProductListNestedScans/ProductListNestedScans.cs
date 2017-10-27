using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NTemplates;
using NTemplates.EventArgs;
using System.Diagnostics;
using System.IO;

namespace ProductListNestedScans
{
    public partial class ProductListNestedScans : Form
    {
        List<Vendor> vendors = new List<Vendor>();
        List<Product> products = new List<Product>();

        private string _inputPath = @"..\..\Templates\ProductListWithGroupingsSample.rtf";
        private string _outputPath = @"..\..\Templates\ProductList.rtf";
        private string _descriptionPath = @"..\..\ExampleDescription.txt";

        public bool _unitTest = false;

        public ProductListNestedScans(string inputPath, string outputPath, string descriptionPath) : this()
        {
            _inputPath = Path.GetFullPath(inputPath);
            _outputPath = Path.GetFullPath(outputPath);
            _descriptionPath = Path.GetFullPath(descriptionPath);
            _unitTest = true;
        }


        public ProductListNestedScans()
        {
            InitializeComponent();

            //Load Vendors and Products. This data could come from a database or a webservice.
            //It doesn't need to be sorted to any particular order.
            vendors.Add(new Vendor() { ID = 1, Description = "Vendor A" });
            vendors.Add(new Vendor() { ID = 2, Description = "Vendor B" });
            vendors.Add(new Vendor() { ID = 3, Description = "Vendor C" });
            vendors.Add(new Vendor() { ID = 4, Description = "Vendor D" });

            products.Add(new Product() { Code = "pr000", Vendor = 1, Desc = "Product 000", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr001", Vendor = 1, Desc = "Product 001", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr002", Vendor = 1, Desc = "Product 002", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr003", Vendor = 1, Desc = "Product 003", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr004", Vendor = 2, Desc = "Product 004", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr005", Vendor = 2, Desc = "Product 005", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr006", Vendor = 2, Desc = "Product 006", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr007", Vendor = 2, Desc = "Product 007", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr008", Vendor = 3, Desc = "Product 008", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr009", Vendor = 3, Desc = "Product 009", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr010", Vendor = 3, Desc = "Product 010", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr011", Vendor = 3, Desc = "Product 011", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr012", Vendor = 4, Desc = "Product 012", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr013", Vendor = 4, Desc = "Product 013", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr014", Vendor = 4, Desc = "Product 014", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr015", Vendor = 4, Desc = "Product 015", IsAvailable = "Yes" });


        }

        public class Product
        {
            public int ID;
            public string Code { get; set; }
            public int Vendor { get; set; }
            public string Desc { get; set; }
            public string IsAvailable
            {
                get;
                set;
            }

        }

        public class Vendor
        {
            public int ID { get; set; }
            public string Description { get; set; }

        }

        public void BtnProductList_Click(object sender, EventArgs e)
        {            
            DocumentCreator dc = new DocumentCreator();
            dc.AddList<Vendor>(vendors, "vendor");
            dc.AddList<Product>(products, "prod");

            //Totals per vendor
            dc.AddInt32("avail", 0);
            dc.AddInt32("unavail", 0);
            dc.AddInt32("tot", 0);

            //Final totals
            dc.AddInt32("available", 0);
            dc.AddInt32("unavailable", 0);
            dc.AddInt32("total", 0);


            dc.ScanStart +=new ScanStartEventHandler(Dc_ScanStart);
            dc.AfterScanRecord += new AfterScanRecordEventHandler(Dc_AfterScanRecord1);
            dc.ScanEnded += new ScanEndedEventHandler(Dc_ScanEnded1);
            dc.CreateDocument(_inputPath, _outputPath);

            if (!_unitTest)
            Process.Start(_outputPath);
        }

        void Dc_ScanStart(object sender, ScanStartEventArgs e)
        {
            if (e.TableName == "prod")
            {
                //Set totals per vendor back to 0
                e.DataManager.AddInt32("avail", 0);
                e.DataManager.AddInt32("unavail", 0);
                e.DataManager.AddInt32("tot", 0);
            }
        }

        void Dc_ScanEnded1(object sender, ScanEndedEventArgs e)
        {
            if (e.TableName == "prod")
            {
                int av = e.DataManager.GetInt32("available");
                int unav = e.DataManager.GetInt32("unavailable");

                e.DataManager.AddInt32("total", av + unav);
            }
        }

        private void Dc_AfterScanRecord1(object sender, AfterScanRecordEventArgs e)
        {

            if (e.TableName == "prod" && e.MatchesScanCondition)
            {
                int avail = e.DataManager.GetInt32("avail");
                int unavail = e.DataManager.GetInt32("unavail");
                if (e.Record[3].ToString() == "Yes")
                    avail++;
                else
                    unavail++;

                //These will be set to 0 for the next vendor.
                e.DataManager.AddInt32("avail", avail);
                e.DataManager.AddInt32("unavail", unavail);
                e.DataManager.AddInt32("tot", avail+unavail);
               
                //Count available and unavailable products (Totals)
                int available = e.DataManager.GetInt32("available");
                int unavailable = e.DataManager.GetInt32("unavailable");
                if (e.Record[3].ToString() == "Yes")
                    available++;
                else
                    unavailable++;

                e.DataManager.AddInt32("available", available);
                e.DataManager.AddInt32("unavailable", unavailable);
                e.DataManager.AddInt32("total", available + unavailable);

            }
        }

        public void ProductList_Load(object sender, EventArgs e)
        {
            string descFile = File.ReadAllText(_descriptionPath);
            txtDescription.Text = descFile;
        }

    }
}