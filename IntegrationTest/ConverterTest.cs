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
            var expectedResultPath = $@"{outputDirPath}\Expected\{sampleName}.stl";

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
            
            Assert.True(FileContentAreTheSame(dstPath, expectedResultPath));
        }

        private static bool FileContentAreTheSame(string file1Path, string file2Path)
        {
            Assert.True(File.Exists(file1Path));
            Assert.True(File.Exists(file2Path));

            var file1Content = File.ReadAllBytes(file1Path);
            var file2Content = File.ReadAllBytes(file2Path);
            
            Assert.AreEqual(file1Content.Length, file2Content.Length);

            for(var i = 0; i < file1Content.Length; ++i)
            {
                if (file1Content[i] != file2Content[i])
                {
                    return false;
                }
            }

            return true;
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