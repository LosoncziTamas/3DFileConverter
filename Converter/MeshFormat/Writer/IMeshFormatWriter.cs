using System.IO;

namespace Converter.MeshFormat.Writer
{
    public interface IMeshFormatWriter
    {
        void WriteToStream(Mesh mesh, Stream outputStream);

        string Tag { get; }
    }
}