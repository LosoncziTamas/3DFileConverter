using System.IO;

namespace Converter.MeshFormat.Reader
{
    public interface IMeshFormatReader
    {
        Mesh ReadFromStream(Stream inputStream);

        string Tag { get; }
    }
}