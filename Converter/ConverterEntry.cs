using System.Collections.Generic;
using System.IO;
using System.Linq;
using Converter.Conversion;

namespace Converter
{
    public static class ConverterEntry
    {
        private static readonly Dictionary<string, IConversionStrategy> SupportedConversions =
            new Dictionary<string, IConversionStrategy>()
            {
                {"OBJ to STL conversion.", new ObjToStlConversionStrategy()}
            };

        public static void Main(string[] args)
        {
            var inputPath = "airboat.obj";
            var outputPath = "obj_test.stl";

            var strategy = SupportedConversions.First().Value;
            strategy.ApplyConversion(File.Open(inputPath, FileMode.Open), File.Open(outputPath, FileMode.Create));
        }
    }
}