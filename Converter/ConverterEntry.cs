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
            private readonly string _srcFormat;
            private readonly string _dstFormat;
            private readonly IConversionStrategy _conversionStrategy;

            public ConversionType(string srcFormat, string dstFormat, IConversionStrategy conversionStrategy)
            {
                this._srcFormat = srcFormat;
                this._dstFormat = dstFormat;
                _conversionStrategy = conversionStrategy;
            }

            public bool MatchesArguments(string argSrcFormat, string argDstFormat)
            {
                return argSrcFormat.StartsWith(_srcFormat) && argDstFormat.StartsWith(_dstFormat);
            }

            public IConversionStrategy GetConversionStrategy()
            {
                return _conversionStrategy;
            }
        }
        class Options
        {
            [Option('i', "input", Required = true, HelpText = "Input filename")]
            public string InputFile { get; set; }
        
            [Option('o', "output", Required = false, HelpText = "Name of the output file. Input filename is used if not specified.")]
            public string OutputFile { get; set; }
        
            [Option('s', "srcFormat", Required = false, HelpText = "Format of the source file. Set to .obj by default.")]
            public string SourceFormat { get; set; }
        
            [Option('d', "dstFormat", Required = false, HelpText = "Destination file format. Set to .stl by default.")]
            public string DestinationFormat { get; set; }
        }

        private static readonly List<ConversionType> supportedConversions = new List<ConversionType>
        {
            new ConversionType(".obj", ".stl", new ObjToStlConversionStrategy()),
        };

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed((errs) => HandleParseError(errs));
        }
        
        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            var conversionType = supportedConversions.FirstOrDefault(type =>
                type.MatchesArguments(opts.SourceFormat, opts.DestinationFormat));
            if (conversionType != null)
            {
                var inputPath = opts.InputFile;
                var outputPath = string.IsNullOrEmpty(opts.OutputFile) ? "argtest.stl" : opts.OutputFile;
                
                conversionType.GetConversionStrategy().ApplyConversion(
                    File.Open(inputPath, FileMode.Open), 
                    File.Open(outputPath, FileMode.Create));
            }
        }
        
        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("handle parse error" + errs);
        }
    }
}