using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Converter.MeshFormat
{
    public class StlFormat
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

        public StlFormat(List<Triangle> triangles)
        {
            Triangles = triangles;
        }

        public static StlFormat FromMesh(Mesh mesh)
        {
            var result = mesh.Triangles.Select(meshTriangle => new Triangle(meshTriangle.Norm, meshTriangle.Vertices));
            return new StlFormat(result.ToList());
        }
    }
}