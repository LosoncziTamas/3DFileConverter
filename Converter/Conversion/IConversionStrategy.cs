using System.IO;

namespace Converter.Conversion
{
    public interface IConversionStrategy    
    {
        void ApplyConversion(Stream inputStream, Stream outputStream);
    }
}