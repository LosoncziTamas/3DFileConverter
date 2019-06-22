using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Converter
{
    public static class Converter
    {
        public static void Main(string[] args)
        {
            var obj = ObjReader.ReadObjFile();
            var triangles = Triangulate(obj);
            STLWriter.WriteBinary(triangles, File.Open("obj_test.stl", FileMode.Create));
        }

        private static List<STLWriter.Triangle> Triangulate(ObjReader.ObjDocument obj)
        {
            var result = new List<STLWriter.Triangle>();

            foreach (var face in obj.faces)
            {
                if (IsClockWiseOrder(face))
                {
                    ReverseVertexOrder(face);
                }

                if (face.GeometricVertexReferences.Count > 3)
                {
                    EarClip(face);
                    // TODO
                }
                else
                {           
                    //TODO: calculate norm if not provided
                    // Reference numbers start from 1
                    var v1 = obj.geometricVertices[face.GeometricVertexReferences[0] - 1];
                    var v2 = obj.geometricVertices[face.GeometricVertexReferences[1] - 1];
                    var v3 = obj.geometricVertices[face.GeometricVertexReferences[2] - 1];
                    
                    result.Add(new STLWriter.Triangle(Vector3.Zero, new Vector3[3]
                    {
                        new Vector3(v1.X, v1.Y, v1.Z), 
                        new Vector3(v2.X, v2.Y, v2.Z), 
                        new Vector3(v3.X, v3.Y, v3.Z) 
                    }));
                }
            }
            
            return result;
        }

        private static void EarClip(ObjReader.Face face)
        {
            
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