using System.Collections.Generic;
using System.Numerics;

namespace Converter.Documents
{
    public class StlDocument
    {
        public readonly List<Triangle> Triangles;
        public StlDocument(List<Triangle> triangles)
        {
            Triangles = triangles;
        }
    }
    
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
}