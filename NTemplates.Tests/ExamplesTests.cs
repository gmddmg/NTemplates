using System;
using Xunit;
/// <summary>
/// Note if the code or options are changed inside of the Invoice example, the tests will fail. 
/// </summary>
namespace NTemplates.Tests
{
    public class ExampleTests
    {
        [Fact]
        public void InvoiceTest()
        {
            var inputTemplatePath = @"../../../Invoices/Templates/InvoiceSample.rtf";
            var outputPath = "@../../../../Generated/Invoice.Rtf";
            var exampleDescriptionPath = @"../../../Invoices/ExampleDescription.txt";

            var invoice = new Invoices.Invoices(inputTemplatePath,outputPath, exampleDescriptionPath);
            invoice.Invoices_Load(null,null);
            invoice.Button1_Click(null,null);
            invoice.Close();

            FileAssert.AreEqual(outputPath, @"../../ComparisonData/Invoice.rtf");
        }

        [Fact]
        public void LetterTest()
        {
            var inputTemplatePath = @"../../../LetterExample/LetterExample/Documents/LetterTemplate.rtf";
            var outputPath = "@../../../../Generated/Letter.Rtf";
            var logoPath = @"../../../LetterExample/LetterExample/Documents/rocksolidLogo.jpg";

            var letterExample = new LetterExample.LetterExample(inputTemplatePath, outputPath, logoPath, "27/10/2017");
            letterExample.BtnGenerateLetter_Click(null, null);
            letterExample.Close();

            FileAssert.AreEqual(outputPath, @"../../ComparisonData/Letter.rtf");
        }

        [Fact]
        public void ParentChildTest()
        {
            var inputTemplatePath = @"../../../ParentChild/Templates/ParentChildSample.rtf";
            var outputPath = "@../../../../Generated/ParentChild.Rtf";

            var parentChild = new ParentChild.Form1(inputTemplatePath, outputPath);
            parentChild.Button1_Click(null, null);
            parentChild.Close();

            FileAssert.AreEqual(outputPath, @"../../ComparisonData/ParentChild.rtf");
        }

        [Fact]
        public void ProductListTest()
        {
            var inputTemplatePath = @"../../../ProductList/Templates/ProductListSample.rtf";
            var outputPath = "@../../../../Generated/ProductList.Rtf";
            var exampleDescriptionPath = @"../../../ProductList/ExampleDescription.txt";

            var productList = new ProductList.ProductList(inputTemplatePath, outputPath, exampleDescriptionPath);
            productList.ProductList_Load(null, null);
            productList.BtnProductList_Click(null, null);
            productList.Close();

            FileAssert.AreEqual(outputPath, @"../../ComparisonData/ProductList.rtf");
        }

        [Fact]
        public void ProductListNestedScansTest()
        {
            var inputTemplatePath = @"../../../ProductListNestedScans/Templates/ProductListWithGroupingsSample.rtf";
            var outputPath = "@../../../../Generated/ProductListNestedScan.Rtf";
            var exampleDescriptionPath = @"../../../ProductListNestedScans/ExampleDescription.txt";

            var productListNestedScans = new ProductListNestedScans.ProductListNestedScans(inputTemplatePath, outputPath, exampleDescriptionPath);
            productListNestedScans.ProductList_Load(null, null);
            productListNestedScans.BtnProductList_Click(null, null);
            productListNestedScans.Close();


            FileAssert.AreEqual(outputPath, @"../../ComparisonData/ProductListNestedScans.rtf");
        }

        [Fact]
        public void ProductListWithGroupingsTest()
        {
            var inputTemplatePath = @"../../../ProductListWithGroupings/Templates/ProductListWithGroupingsSample.rtf";
            var outputPath = "@../../../../Generated/ProductListWithGroupings.Rtf";
            var exampleDescriptionPath = @"../../../ProductListWithGroupings/ExampleDescription.txt";

            var productListWithGroupings = new ProductListWithGroupings.ProductListWithGroupings(inputTemplatePath, outputPath, exampleDescriptionPath);
            productListWithGroupings.ProductList_Load(null, null);
            productListWithGroupings.BtnProductList_Click(null, null);
            productListWithGroupings.Close();

            FileAssert.AreEqual(outputPath, @"../../ComparisonData/ProductListWithGroupings.rtf");
        }
    }
}
