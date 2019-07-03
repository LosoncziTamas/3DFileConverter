using System.IO;
using Converter;
using NUnit.Framework;

namespace IntegrationTest
{
    [TestFixture]
    public class ConverterTest
    {
        [Test]
        public void TestCow()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..", "..", ".."));
            var outputDirPath = $@"{projectRoot}\TestResults";
            var dstPath = $@"{outputDirPath}\cow.stl";

            Directory.CreateDirectory(outputDirPath);
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }
            
            ConverterEntry.Main(new[]
            {
                "-i",
                $@"{projectRoot}\SampleFiles\cow.obj",
                "-o",
                dstPath
            });
            
            Assert.True(File.Exists(dstPath));
        }
        
        [Test]
        public void TestAirboat()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..", "..", ".."));
            var outputDirPath = $@"{projectRoot}\TestResults";
            var dstPath = $@"{outputDirPath}\airboat.stl";

            Directory.CreateDirectory(outputDirPath);
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }
            
            ConverterEntry.Main(new[]
            {
                "-i",
                $@"{projectRoot}\SampleFiles\airboat.obj",
                "-o",
                dstPath
            });
            
            Assert.True(File.Exists(dstPath));
        }
        
        [Test]
        public void TestPumpkin()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..", "..", ".."));
            var outputDirPath = $@"{projectRoot}\TestResults";
            var dstPath = $@"{outputDirPath}\pumpkin.stl";

            Directory.CreateDirectory(outputDirPath);
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }
            
            ConverterEntry.Main(new[]
            {
                "-i",
                $@"{projectRoot}\SampleFiles\pumpkin.obj",
                "-o",
                dstPath
            });
            
            Assert.True(File.Exists(dstPath));
        }
        
        [Test]
        public void TestNegativeReferenceModel()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(System.AppContext.BaseDirectory, "..", "..", ".."));
            var outputDirPath = $@"{projectRoot}\TestResults";
            var dstPath = $@"{outputDirPath}\negative.stl";

            Directory.CreateDirectory(outputDirPath);
            if (File.Exists(dstPath))
            {
                File.Delete(dstPath);
            }
            
            ConverterEntry.Main(new[]
            {
                "-i",
                $@"{projectRoot}\SampleFiles\negative.obj",
                "-o",
                dstPath
            });
            
            Assert.True(File.Exists(dstPath));
        }
    }
}