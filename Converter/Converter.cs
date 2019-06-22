using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Converter
{
    internal class Converter
    {
        public static void Main(string[] args)
        {
            var obj = OBJReader.ReadOBJFile();
            var triangles = Triangulate(obj);
            STLWriter.WriteBinary(triangles, File.Open("obj_test.stl", FileMode.Create));
        }

        private static List<STLWriter.Triangle> Triangulate(OBJReader.OBJDocument obj)
        {
            var result = new List<STLWriter.Triangle>();

            foreach (var face in obj.faces)
            {
                if (IsClockWiseOrder(face))
                {
                    ReverseVertexOrder(face);
                }

                if (face.geometricVertexReferences.Count > 3)
                {
                    EarClip(face);
                    // TODO
                }
                else
                {           
                    //TODO: calculate norm if not provided
                    // Reference numbers start from 1
                    var v1 = obj.vertices[face.geometricVertexReferences[0] - 1];
                    var v2 = obj.vertices[face.geometricVertexReferences[1] - 1];
                    var v3 = obj.vertices[face.geometricVertexReferences[2] - 1];
                    
                    result.Add(new STLWriter.Triangle(Vector3.Zero, new Vector3[3]
                    {
                        new Vector3(v1.x, v1.y, v1.z), 
                        new Vector3(v2.x, v2.y, v2.z), 
                        new Vector3(v3.x, v3.y, v3.z) 
                    }));
                }
            }
            
            return result;
        }

        private static void EarClip(OBJReader.Face face)
        {
            
        }

        private static bool IsClockWiseOrder(OBJReader.Face face)
        {
            return false;
        }

        private static void ReverseVertexOrder(OBJReader.Face face)
        {
            face.geometricVertexReferences.Reverse();
            face.normalVertexReferences.Reverse();
            face.textureVertexReferences.Reverse();
        }
    }
}