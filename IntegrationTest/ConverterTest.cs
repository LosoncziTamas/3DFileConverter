using System.IO;
using Converter;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture]
    public class ConverterTest
    {
        private static void CheckSampleConversion(string sampleName)
        {
            var projectRoot = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..", "..", ".."));
            var outputDirPath = $@"{projectRoot}\TestResults";
            var dstPath = $@"{outputDirPath}\{sampleName}.stl";

            Directory.CreateDirectory(outputDirPath);
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }
            
            ConverterEntry.Main(new[]
            {
                "-i",
                $@"{projectRoot}\SampleFiles\{sampleName}.obj",
                "-o",
                dstPath
            });
            
            Assert.True(File.Exists(dstPath));
        }
        
        [Test]
        public void TestSampleFiles()
        {
            CheckSampleConversion("cow");
            CheckSampleConversion("airboat");
            CheckSampleConversion("pumpkin");
            CheckSampleConversion("negative");
            CheckSampleConversion("teddy");
            CheckSampleConversion("teapot");
            CheckSampleConversion("mini");
            CheckSampleConversion("box");
        }
    }
}