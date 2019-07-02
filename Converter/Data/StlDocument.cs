using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Converter.Documents;

namespace Converter.Data
{
    public class StlDocument
    {
        public class Triangle
        {
            public readonly Vector3[] Vertices;
            public readonly Vector3 Norm;
            public readonly ushort AttributeByteCount;

            public Triangle(Vector3 norm, Vector3[] vertices)
            {
                Vertices = vertices;
                Norm = norm;
                AttributeByteCount = 0;
            }
        }

        public readonly List<Triangle> Triangles;

        public StlDocument(List<Triangle> triangles)
        {
            Triangles = triangles;
        }

        public static StlDocument FromMesh(Mesh mesh)
        {
            var result = mesh.Triangles.Select(meshTriangle => new StlDocument.Triangle(meshTriangle.Norm, meshTriangle.Vertices));
            return new StlDocument(result.ToList());
        }
    }
}