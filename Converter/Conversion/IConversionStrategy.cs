using System.IO;
using Converter.Documents;

namespace Converter.Conversion
{
    public interface IConversionStrategy    
    {
        void ApplyConversion(Stream inputStream, Stream outputStream);
    }
}