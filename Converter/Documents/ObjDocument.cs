using System.Collections.Generic;
using System.Numerics;

namespace Converter.Documents
{
    public class ObjDocument : IDocument
    {
        public List<Vector4> geometricVertices;
        public List<Vector3> textureVertices;
        public List<Vector3> vertexNormals;
        public List<Face> faces;

        public ObjDocument(List<Vector4> geometricVertices, List<Vector3> textureVertices, List<Vector3> vertexNormals, List<Face> faces)
        {
            this.geometricVertices = geometricVertices;
            this.textureVertices = textureVertices;
            this.vertexNormals = vertexNormals;
            this.faces = faces;
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