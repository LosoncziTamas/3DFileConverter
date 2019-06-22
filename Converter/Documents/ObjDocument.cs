using System.Collections.Generic;
using System.Numerics;

namespace Converter.Documents
{
    public class ObjDocument : Converter.IDocument
    {
        public List<Vector4> geometricVertices;
        public List<Vector3> textureVertices;
        public List<Vector3> vertexNormals;
        public List<ObjReader.Face> faces;

        public ObjDocument(List<Vector4> geometricVertices, List<Vector3> textureVertices, List<Vector3> vertexNormals, List<ObjReader.Face> faces)
        {
            this.geometricVertices = geometricVertices;
            this.textureVertices = textureVertices;
            this.vertexNormals = vertexNormals;
            this.faces = faces;
        }
    }
}