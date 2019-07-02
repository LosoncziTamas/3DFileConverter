using System.IO;
using Converter.Documents;

namespace Converter.Conversion
{
    public interface IMeshWriter
    {
        void WriteToStream(Mesh mesh, Stream outputStream);
    }
}