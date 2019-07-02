using System.Collections.Generic;
using System.Numerics;

namespace Converter.Documents
{
    public class Mesh
    {
        public class Triangle
        {
            public readonly Vector3[] Vertices;
            public readonly Vector3 Norm;

            public Triangle(Vector3[] vertices, Vector3 norm)
            {
                Vertices = vertices;
                Norm = norm;
            }
        }
        
        public readonly List<Triangle> Triangles;

        public Mesh(List<Triangle> triangles)
        {
            Triangles = triangles;
        }
    }
}