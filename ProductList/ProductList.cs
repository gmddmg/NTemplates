using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NTemplates;
using NTemplates.EventArgs;
using System.Diagnostics;
using System.IO;

namespace ProductList
{
    public partial class ProductList : Form
    {

        List<Product> products = new List<Product>();
        private string _inputPath = @"..\..\Templates\ProductListSample.rtf";
        private string _outputPath = @"..\..\Templates\ProductList.rtf";
        private string _descriptionPath = @"..\..\ExampleDescription.txt";

        private bool _isUnitTest = false;
        public ProductList(string inputPath, string outputPath, string descriptionPath) : this()
        {
            _isUnitTest = true;
            _inputPath = inputPath;
            _outputPath = outputPath;
            _descriptionPath = descriptionPath;
        }

        public ProductList()
        {
            InitializeComponent();

            //Load a list of products. This data could come from a database or a webservice.
            products.Add(new Product() { Code = "pr001", Desc = "Product 001", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr001", Desc = "Product 001", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr002", Desc = "Product 002", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr003", Desc = "Product 003", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr004", Desc = "Product 004", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr005", Desc = "Product 005", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr006", Desc = "Product 006", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr007", Desc = "Product 007", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr008", Desc = "Product 008", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr009", Desc = "Product 009", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr010", Desc = "Product 009", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr011", Desc = "Product 010", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr012", Desc = "Product 011", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr013", Desc = "Product 012", IsAvailable = "No" });
            products.Add(new Product() { Code = "pr014", Desc = "Product 013", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr015", Desc = "Product 014", IsAvailable = "Yes" });
            products.Add(new Product() { Code = "pr016", Desc = "Product 015", IsAvailable = "Yes" });

            bindingSource1.DataSource = products;

            
            dgvProducts.DataSource = bindingSource1;
            dgvProducts.AutoGenerateColumns = false;
            
        }

        public class Product
        {            
            public string Code { get; set; }
            public string Desc { get; set; }
            public string IsAvailable
            {
                get;
                set;
            }
            
        }

        public void BtnProductList_Click(object sender, EventArgs e)
        {            
            DocumentCreator dc = new DocumentCreator();
            dc.AddList<Product>(products, "P");
            dc.AddInt32("available", 0);
            dc.AddInt32("unavailable", 0);
            dc.AddInt32("total", 0);

            dc.AfterScanRecord += new AfterScanRecordEventHandler(Dc_AfterScanRecord1);
            dc.ScanEnded += new ScanEndedEventHandler(Dc_ScanEnded1);

            dc.CreateDocument(_inputPath, _outputPath);

            if(!_isUnitTest)
            Process.Start(_outputPath);
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
                int av = e.DataManager.GetInt32("available");
                int unav = e.DataManager.GetInt32("unavailable");

                if (e.Record[2].ToString() == "Yes")
                    av++;
                else
                    unav++;

                e.DataManager.AddInt32("available", av);
                e.DataManager.AddInt32("unavailable", unav);
            }
        }

        public void ProductList_Load(object sender, EventArgs e)
        {
            string descFile = File.ReadAllText(_descriptionPath);
            txtDescription.Text = descFile;
        }
        
    }
}
