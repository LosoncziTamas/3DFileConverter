using System.IO;
using System.Numerics;

namespace Converter.MeshFormat.Writer
{
    public class StlFormatWriter : IMeshFormatWriter
    {
        private const int HeaderSize = 80;

        public string Tag => ".stl";

        private void WriteTriangle(StlDocument.Triangle triangle, BinaryWriter writer)
        {
            WriteVector3(triangle.Norm, writer);
            for (var i = 0; i < 3; ++i)
            {
                WriteVector3(triangle.Vertices[i], writer);
            }

            writer.Write(triangle.AttributeByteCount);
        }

        private void WriteVector3(Vector3 vec, BinaryWriter writer)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);
        }

        public void WriteToStream(Mesh mesh, Stream outputStream)
        {
            var stl = StlDocument.FromMesh(mesh);
            using (var writer = new BinaryWriter(outputStream))
            {
                var header = new byte[HeaderSize];
                writer.Write(header);
                writer.Write((uint) stl.Triangles.Count);
                stl.Triangles.ForEach(triangle => WriteTriangle(triangle, writer));
            }
        }
    }
}