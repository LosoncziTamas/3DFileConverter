using System.Collections.Generic;
using System.Numerics;

namespace Converter.Documents
{
    public class ObjDocument : IDocument
    {
        public readonly List<Vector4> GeometricVertices;
        public readonly List<Vector3> TextureVertices;
        public readonly List<Vector3> VertexNormals;
        public readonly List<Face> Faces;

        public ObjDocument(List<Vector4> geometricVertices, List<Vector3> textureVertices, List<Vector3> vertexNormals, List<Face> faces)
        {
            this.GeometricVertices = geometricVertices;
            this.TextureVertices = textureVertices;
            this.VertexNormals = vertexNormals;
            this.Faces = faces;
        }
    }
    
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
}