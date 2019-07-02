using System.IO;
using Converter.Documents;

namespace Converter.Conversion
{
    public interface IMeshReader
    {
        Mesh ReadFromStream(Stream inputStream);
    }
}