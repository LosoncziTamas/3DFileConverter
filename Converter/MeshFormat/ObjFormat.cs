using System.Collections.Generic;
using System.Numerics;
using Converter.Utils;

namespace Converter.MeshFormat
{
    public class ObjFormat
    {
        public class Face
        {
            public readonly List<int> GeometricVertexReferences;
            public readonly List<int> TextureVertexReferences;
            public readonly List<int> NormalVertexReferences;

            public Face()
            {
                GeometricVertexReferences = new List<int>();
                TextureVertexReferences = new List<int>();
                NormalVertexReferences = new List<int>();
            }
        }

        private readonly List<Vector4> _geometricVertices;
        private readonly List<Vector3> _textureVertices;
        private readonly List<Vector3> _vertexNormals;
        private readonly List<Face> _faces;

        public ObjFormat(List<Vector4> geometricVertices, List<Vector3> textureVertices, List<Vector3> vertexNormals,
            List<Face> faces)
        {
            _geometricVertices = geometricVertices;
            _textureVertices = textureVertices;
            _vertexNormals = vertexNormals;
            _faces = faces;
        }

        public static Mesh ToMesh(ObjFormat source)
        {
            var triangles = new List<Mesh.Triangle>();
            foreach (var face in source._faces)
            {
                if (face.GeometricVertexReferences.Count > 3)
                {
                    var clippedTriangles = PerformEarClipping(source._geometricVertices, face);
                    foreach (var clippedTriangle in clippedTriangles)
                    {
                        triangles.Add(clippedTriangle);
                    }
                }
                else
                {
                    // Reference numbers start from 1
                    var v1 = source._geometricVertices[face.GeometricVertexReferences[0] - 1].ToVector3();
                    var v2 = source._geometricVertices[face.GeometricVertexReferences[1] - 1].ToVector3();
                    var v3 = source._geometricVertices[face.GeometricVertexReferences[2] - 1].ToVector3();
                    var normal = CalculateTriangleNormal(v1, v2, v3);

                    triangles.Add(new Mesh.Triangle(new Vector3[3] {v1, v2, v3}, normal));
                }
            }

            return new Mesh(triangles);
        }

        private static Vector3 CalculateTriangleNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var u = v2 - v1;
            var v = v3 - v1;
            var norm = Vector3.Cross(u, v);
            return Vector3.Normalize(norm);
        }

        private static List<Mesh.Triangle> PerformEarClipping(List<Vector4> geometricVertices, Face face)
        {
            var faceVertexCount = face.GeometricVertexReferences.Count;
            var faceVertices = new Vector3[faceVertexCount];
            for (var i = 0; i < faceVertexCount; i++)
            {
                var geoVertex = geometricVertices[face.GeometricVertexReferences[i] - 1];
                faceVertices[i] = new Vector3(geoVertex.X, geoVertex.Y, geoVertex.Z);
            }

            var result = new List<Mesh.Triangle>();
            for (var i = 1; i < faceVertices.Length - 1; ++i)
            {
                var normal = CalculateTriangleNormal(faceVertices[0], faceVertices[i], faceVertices[i + 1]);
                result.Add(new Mesh.Triangle(new Vector3[3]
                {
                    faceVertices[0],
                    faceVertices[i],
                    faceVertices[i + 1]
                }, normal));
            }

            return result;
        }
    }
}