using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Converter.Documents
{
    public class StlDocument : Converter.IDocument
    {
        public readonly List<Triangle> Triangles;

        public StlDocument(List<Triangle> triangles)
        {
            Triangles = triangles;
        }
    }
    
    public class Triangle
    {
        private Vector3[] vertices;
        private Vector3 norm;
        private UInt16 attributeByteCount;

        public Triangle(Vector3 norm, Vector3[] vertices)
        {
            this.vertices = vertices;
            this.norm = norm;
            attributeByteCount = 0;
        }
            
        public void Write(BinaryWriter writer)
        {
            norm.Write(writer);
            for (var i = 0; i < 3; ++i)
            {
                vertices[i].Write(writer);
            }
            writer.Write(attributeByteCount);
        }
    }
}