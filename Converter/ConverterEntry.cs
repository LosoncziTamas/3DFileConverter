using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Converter.MeshFormat.Reader;
using Converter.MeshFormat.Writer;

namespace Converter
{
    public static class ConverterEntry
    {
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

        private static readonly List<IMeshFormatReader> FormatReaders = new List<IMeshFormatReader>
        {
            new ObjFormatReader()
        };

        private static readonly List<IMeshFormatWriter> FormatWriters = new List<IMeshFormatWriter>
        {
            new StlFormatWriter()
        };

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CommandLineOptions>(args).WithParsed(ProcessOptions);
        }

        private static void ProcessOptions(CommandLineOptions opts)
        {
            var defaultSrcFormat = FormatReaders.First().Tag;
            var defaultDstFormat = FormatWriters.First().Tag;
            opts.SourceFormat = string.IsNullOrEmpty(opts.SourceFormat) ? defaultSrcFormat : opts.SourceFormat;
            opts.DestinationFormat = string.IsNullOrEmpty(opts.DestinationFormat) ? defaultDstFormat : opts.DestinationFormat;

            var reader = FormatReaders.FirstOrDefault(r => r.Tag.StartsWith(opts.SourceFormat));
            var writer = FormatWriters.FirstOrDefault(r => r.Tag.StartsWith(opts.DestinationFormat));
            if (reader != null && writer != null)
            {
                var inputPath = opts.InputFile;
                var outputPath = string.IsNullOrEmpty(opts.OutputFile)
                    ? Path.GetFileNameWithoutExtension(inputPath) + opts.DestinationFormat
                    : opts.OutputFile;
                if (!File.Exists(inputPath))
                {
                    Console.WriteLine("Provided input file does not exist.");
                    return;
                }

                try
                {
                    var mesh = reader.ReadFromStream(File.Open(inputPath, FileMode.Open));
                    writer.WriteToStream(mesh, File.Open(outputPath, FileMode.Create));
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                PrintSupportedConversions();
            }
        }

        private static void PrintSupportedConversions()
        {
            Console.WriteLine("Currently, the following conversions are supported.");
            Console.WriteLine("Reading: ");
            foreach (var reader in FormatReaders)
            {
                Console.WriteLine(reader.Tag);
            }

            Console.WriteLine("Writing: ");
            foreach (var writer in FormatWriters)
            {
                Console.WriteLine(writer.Tag);
            }
        }
    }
}