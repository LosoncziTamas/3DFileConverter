using System.IO;
using Converter.Documents;

namespace Converter
{
    public interface IConversionStrategy<Source, Destination> 
        where Source : IDocument 
        where Destination : IDocument
    {
        Source ReadFromStream(Stream stream);
        void WriteToStream(Destination d, Stream stream);
        Destination ApplyConversion(Source source);
    }
}