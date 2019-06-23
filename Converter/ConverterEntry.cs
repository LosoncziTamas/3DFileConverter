using System.IO;
using Converter.Conversion;

namespace Converter
{
    public static class ConverterEntry
    {   
        public static void Main(string[] args)
        {
            var inputPath = "mini.obj";
            var outputPath = "obj_test.stl";

            var strategy = new ObjToStlConversionStrategy();
            var obj = strategy.ReadFromStream(File.Open(inputPath, FileMode.Open));
            var stl = strategy.ApplyConversion(obj);
            strategy.WriteToStream(stl, File.Open(outputPath, FileMode.Create));
        }       
    }
}