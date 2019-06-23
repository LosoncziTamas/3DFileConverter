using System.IO;
using System.Numerics;
using System.Text;
using Converter.Documents;
using Converter.Utils;

namespace Converter.Conversion
{
    public class StlWriter
    {
        private const int HeaderSize = 80;
        
        public void WriteToStream(StlDocument stlDocument, Stream stream)
        {
            // TODO: leave open?
            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                var header = new byte[HeaderSize];
                writer.Write(header);
                writer.Write((uint) stlDocument.Triangles.Count);
                // TODO perf check
                stlDocument.Triangles.ForEach(triangle => WriteTriangle(triangle, writer));
            }
        }
             
        private void WriteTriangle(Triangle triangle, BinaryWriter writer)
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
    }
}