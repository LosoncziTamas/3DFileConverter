using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Converter.Documents;

namespace Converter.Conversion
{
    public class ObjToStlConversionStrategy : IConversionStrategy<ObjDocument, StlDocument>
    {
        private readonly ObjReader _reader;
        private readonly StlWriter _writer;
        
        public ObjToStlConversionStrategy()
        {
            _reader = new ObjReader();
            _writer = new StlWriter();
        }

        public ObjDocument ReadFromStream(Stream stream)
        {
            return _reader.ReadFromStream(stream);
        }

        public void WriteToStream(StlDocument document, Stream stream)
        {
            _writer.WriteToStream(document, stream);
        }

        public StlDocument ApplyConversion(ObjDocument source)
        {
            var result = new List<Triangle>();

            foreach (var face in source.Faces)
            {
                if (IsClockWiseOrder(face))
                {
                    ReverseVertexOrder(face);
                }

                if (face.GeometricVertexReferences.Count > 3)
                {
                    var clippedTriangles = EarClip(source.GeometricVertices, face);
                    foreach (var clippedTriangle in clippedTriangles)
                    {
                        result.Add(clippedTriangle);
                    }
                }
                else
                {
                    // TODO: calculate norm if not provided
                    // Reference numbers start from 1
                    var v1 = source.GeometricVertices[face.GeometricVertexReferences[0] - 1];
                    var v2 = source.GeometricVertices[face.GeometricVertexReferences[1] - 1];
                    var v3 = source.GeometricVertices[face.GeometricVertexReferences[2] - 1];

                    result.Add(new Triangle(Vector3.Zero, new Vector3[3]
                    {
                        new Vector3(v1.X, v1.Y, v1.Z),
                        new Vector3(v2.X, v2.Y, v2.Z),
                        new Vector3(v3.X, v3.Y, v3.Z)
                    }));
                }
            }

            return new StlDocument(result);
        }


        private List<Triangle> EarClip(List<Vector4> geometricVertices, Face face)
        {
            var faceVertexCount = face.GeometricVertexReferences.Count;
            var faceVertices = new Vector3[faceVertexCount];
            for (var i = 0; i < faceVertexCount; i++)
            {
                var geoVertex = geometricVertices[face.GeometricVertexReferences[i] - 1];
                faceVertices[i] = new Vector3(geoVertex.X, geoVertex.Y, geoVertex.Z);
            }

            var result = new List<Triangle>();

            for (var i = 1; i < faceVertices.Length - 1; ++i)
            {
                result.Add(new Triangle(Vector3.Zero, new Vector3[3]
                {
                    faceVertices[0],
                    faceVertices[i],
                    faceVertices[i + 1]
                }));
            }

            return result;
        }

        private bool IsClockWiseOrder(Face face)
        {
            return false;
        }

        private void ReverseVertexOrder(Face face)
        {
            face.GeometricVertexReferences.Reverse();
            face.NormalVertexReferences.Reverse();
            face.TextureVertexReferences.Reverse();
        }
    }
}