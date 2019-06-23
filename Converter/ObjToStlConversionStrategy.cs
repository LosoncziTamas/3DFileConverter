using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Converter.Documents;

namespace Converter
{
    public class ObjToStlConversionStrategy : IConversionStrategy<ObjDocument, StlDocument>
    {
        public ObjDocument ReadFromStream(Stream stream)
        {
            return ObjReader.ReadObjFile(stream);
        }

        public void WriteToStream(StlDocument d, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                var header = new byte[80];
                writer.Write(header);
                writer.Write((uint) d.Triangles.Count);
                d.Triangles.ForEach(triangle => triangle.Write(writer));
            }
        }

        public StlDocument ApplyConversion(ObjDocument source)
        {
            return Triangulate(source);
        }
        
         private static StlDocument Triangulate(ObjDocument objDocument)
        {
            var result = new List<Triangle>();
            
            foreach (var face in objDocument.faces)
            {
                if (IsClockWiseOrder(face))
                {
                    ReverseVertexOrder(face);
                }

                if (face.GeometricVertexReferences.Count > 3)
                {
                    var clippedTriangles = EarClip(objDocument.geometricVertices, face);
                    foreach (var clippedTriangle in clippedTriangles)
                    {
                        result.Add(clippedTriangle);
                    }
                }
                else
                {           
                    //TODO: calculate norm if not provided
                    // Reference numbers start from 1
                    var v1 = objDocument.geometricVertices[face.GeometricVertexReferences[0] - 1];
                    var v2 = objDocument.geometricVertices[face.GeometricVertexReferences[1] - 1];
                    var v3 = objDocument.geometricVertices[face.GeometricVertexReferences[2] - 1];
                    
                    result.Add(new Triangle(Vector3.Zero, new Vector3[3]
                    {
                        new Vector3(v1.X, v1.Y, v1.Z), 
                        new Vector3(v2.X, v2.Y, v2.Z), 
                        new Vector3(v3.X, v3.Y, v3.Z) 
                    }));
                }
            }
            
            return new StlDocument(result);
        }

        private static List<Triangle> EarClip(List<Vector4> geometricVertices, ObjReader.Face face)
        {
            var faceVertexCount = face.GeometricVertexReferences.Count;
            Vector3[] faceVertices = new Vector3[faceVertexCount];
            for (var i = 0; i < faceVertexCount; i++)
            {
                var geoVertex = geometricVertices[face.GeometricVertexReferences[i] - 1];
                faceVertices[i] =  new Vector3(geoVertex.X, geoVertex.Y, geoVertex.Z);
            }

            var result = new List<Triangle>();

            for (var i = 1; i < faceVertices.Length - 1; ++i)
            {
                result.Add(new Triangle(Vector3.Zero, new Vector3[3]
                {
                   faceVertices[0], 
                   faceVertices[i], 
                   faceVertices[i + 1]
                }));
            }

            return result;
        }

        private static bool IsClockWiseOrder(ObjReader.Face face)
        {
            return false;
        }

        private static void ReverseVertexOrder(ObjReader.Face face)
        {
            face.GeometricVertexReferences.Reverse();
            face.NormalVertexReferences.Reverse();
            face.TextureVertexReferences.Reverse();
        }
    }
    
    
}