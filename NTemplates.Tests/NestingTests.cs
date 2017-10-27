using System.Collections.Generic;
using Xunit;

namespace NTemplates.Tests.Tests
{
    public class NestingTests
    {
        public List<string> inputTemplatePath = new List<string>()
        {
            @"../../Templates/Nesting1.rtf" ,
            @"../../Templates/Nesting2.rtf",
            @"../../Templates/Nesting3.rtf",
            @"../../Templates/Nesting4.rtf",
            @"../../Templates/Nesting5.rtf",
            @"../../Templates/Nesting6.rtf",
        };

        public List<string> outputPath = new List<string>()
        {
            "@../../../../Generated/Nesting1.Rtf",
            "@../../../../Generated/Nesting2.Rtf",
            "@../../../../Generated/Nesting3.Rtf",
            "@../../../../Generated/Nesting4.Rtf",
            "@../../../../Generated/Nesting5.Rtf",
        };

        public List<string> comparisonPath = new List<string>()
        {
            @"../../ComparisonData/Nesting1.rtf",
            @"../../ComparisonData/Nesting2.rtf",
            @"../../ComparisonData/Nesting3.rtf",
            @"../../ComparisonData/Nesting4.rtf",
            @"../../ComparisonData/Nesting5.rtf"
        };

        [Fact]
        public void NestedTest1()
        {
            new DocumentCreator(delimiter:"~",preprocessText: false).CreateDocument(inputTemplatePath[0], outputPath[0]);
            FileAssert.AreEqual(outputPath[0], comparisonPath[0]);
        }

        [Fact]
        public void NestedTest2()
        {
            new DocumentCreator(delimiter: "~", preprocessText: false).CreateDocument(inputTemplatePath[1], outputPath[1]);
            FileAssert.AreEqual(outputPath[1], comparisonPath[1]);
        }

        [Fact]
        public void NestedTest3()
        {
            new DocumentCreator(delimiter: "~", preprocessText: false).CreateDocument(inputTemplatePath[2], outputPath[2]);
            FileAssert.AreEqual(outputPath[2], comparisonPath[2]);
        }

        [Fact]
        public void NestedTest4()
        {
            new DocumentCreator(delimiter: "~", preprocessText: false).CreateDocument(inputTemplatePath[3], outputPath[3]);
            FileAssert.AreEqual(outputPath[3], comparisonPath[3]);
        }

        [Fact]
        public void NestedTest5()
        {
            new DocumentCreator(delimiter: "~", preprocessText: false).CreateDocument(inputTemplatePath[4], outputPath[4]);
            FileAssert.AreEqual(outputPath[4], comparisonPath[4]);
        }

    }
}
