using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Converter.Documents;
using Converter.Utils;

namespace Converter.Conversion
{
    public class ObjToStlConversionStrategy : IConversionStrategy
    {
        private readonly ObjReader _reader;
        private readonly StlWriter _writer;

        public ObjToStlConversionStrategy()
        {
            _reader = new ObjReader();
            _writer = new StlWriter();
        }
        
        public void ApplyConversion(Stream inputStream, Stream outputStream)
        {
            var source = _reader.ReadFromStream(inputStream);
            var triangles = new List<Triangle>();
            foreach (var face in source.Faces)
            {
                if (face.GeometricVertexReferences.Count > 3)
                {
                    var clippedTriangles = EarClip(source.GeometricVertices, face);
                    foreach (var clippedTriangle in clippedTriangles)
                    {
                        triangles.Add(clippedTriangle);
                    }
                }
                else
                {
                    // Reference numbers start from 1
                    var v1 = source.GeometricVertices[face.GeometricVertexReferences[0] - 1].ToVector3();
                    var v2 = source.GeometricVertices[face.GeometricVertexReferences[1] - 1].ToVector3();
                    var v3 = source.GeometricVertices[face.GeometricVertexReferences[2] - 1].ToVector3();
                    var normal = CalculateTriangleNormal(v1, v2, v3);

                    triangles.Add(new Triangle(normal, new Vector3[3] {v1, v2, v3}));
                }
            }
            _writer.WriteToStream(new StlDocument(triangles), outputStream);
        }

        private Vector3 CalculateTriangleNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var u = v2 - v1;
            var v = v3 - v1;
            var norm = Vector3.Cross(u, v);
            return Vector3.Normalize(norm);
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
                var normal = CalculateTriangleNormal(faceVertices[0], faceVertices[i], faceVertices[i + 1]);
                result.Add(new Triangle(normal, new Vector3[3]
                {
                    faceVertices[0],
                    faceVertices[i],
                    faceVertices[i + 1]
                }));
            }
            return result;
        }
    }
}