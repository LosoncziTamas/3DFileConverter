using System.Collections.Generic;
using System.Numerics;

namespace Converter.MeshFormat
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

        public float CalculateArea()
        {
            var area = 0f;
            
            foreach(var triangle in Triangles)
            {
                var u = triangle.Vertices[1] - triangle.Vertices[0];
                var v = triangle.Vertices[2] - triangle.Vertices[0];
                area += Vector3.Cross(u, v).Length();
            }

            return area;
        }

        public float CalculateVolume()
        {
            var volume = 0f;

            foreach (var triangle in Triangles)
            {
                // based on https://stackoverflow.com/a/1568551
                var tetrahedronVolume = Vector3.Dot(triangle.Vertices[0], Vector3.Cross(triangle.Vertices[1], triangle.Vertices[3])) * 1.0f / 6.0f;
                volume += tetrahedronVolume;
            }
            return volume;
        }  
    }
}