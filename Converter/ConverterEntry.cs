using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Converter.Conversion;

namespace Converter
{
    public static class ConverterEntry
    {
        class ConversionType
        {
            public string SrcFormat { get; }
            public string DstFormat { get; }
            public IConversionStrategy ConversionStrategy { get; }

            public ConversionType(string srcFormat, string dstFormat, IConversionStrategy conversionStrategy)
            {
                SrcFormat = srcFormat;
                DstFormat = dstFormat;
                ConversionStrategy = conversionStrategy;
            }

            public bool MatchesArguments(string argSrcFormat, string argDstFormat)
            {
                return argSrcFormat.StartsWith(SrcFormat) && argDstFormat.StartsWith(DstFormat);
            }
        }

        class CommandLineOptions
        {
            [Option('i', "input", Required = true, HelpText = "Input filename")]
            public string InputFile { get; set; }

            [Option('o', "output", Required = false,
                HelpText = "Name of the output file. Input filename is used if not specified.")]
            public string OutputFile { get; set; }

            [Option('s', "srcFormat", Required = false,
                HelpText = "Format of the source file. Set to .obj by default.")]
            public string SourceFormat { get; set; }

            [Option('d', "dstFormat", Required = false, HelpText = "Destination file format. Set to .stl by default.")]
            public string DestinationFormat { get; set; }
        }

        private static readonly List<ConversionType> SupportedConversions = new List<ConversionType>
        {
            new ConversionType(".obj", ".stl", new ObjToStlConversionStrategy()),
        };

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(ProcessOptions);
        }

        private static void ProcessOptions(CommandLineOptions opts)
        {
            var defaultSupportedConversion = SupportedConversions.First();
            opts.SourceFormat = string.IsNullOrEmpty(opts.SourceFormat) ? defaultSupportedConversion.SrcFormat : opts.SourceFormat;
            opts.DestinationFormat = string.IsNullOrEmpty(opts.DestinationFormat) ? defaultSupportedConversion.DstFormat : opts.DestinationFormat;
            
            var conversionType = SupportedConversions.FirstOrDefault(c => c.MatchesArguments(opts.SourceFormat, opts.DestinationFormat));
            if (conversionType != null)
            {
                var inputPath = opts.InputFile;
                var outputPath = string.IsNullOrEmpty(opts.OutputFile) ? Path.GetFileNameWithoutExtension(inputPath) + opts.DestinationFormat : opts.OutputFile;
                if (!File.Exists(inputPath))
                {
                    Console.WriteLine("Provided input file does not exist.");
                    return;
                }
                conversionType.ConversionStrategy.ApplyConversion(File.Open(inputPath, FileMode.Open),File.Open(outputPath, FileMode.Create));
            }
            else
            {
                Console.WriteLine("Currently, the following conversions are supported.");
                foreach (var conversion in SupportedConversions)
                {
                    Console.WriteLine("{0} to {1}", conversion.SrcFormat, conversion.DstFormat);
                }
            }
        }
    }
}