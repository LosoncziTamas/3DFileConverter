using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using Converter.Documents;

namespace Converter
{
    public class ObjReader : Converter.IDocumentReader<ObjDocument>
    {
        private const string TestObjFile =
              @"#	                Vertices: 8
                #	                  Points: 0
                #	                   Lines: 0
                #	                   Faces: 6
                #	               Materials: 1
                
                        o 1
                
                # Vertex list
                
                        v -0.5 -0.5 0.5
                        v -0.5 -0.5 -0.5
                        v -0.5 0.5 -0.5
                        v -0.5 0.5 0.5
                        v 0.5 -0.5 0.5
                        v 0.5 -0.5 -0.5
                        v 0.5 0.5 -0.5
                        v 0.5 0.5 0.5
                
                # Point/Line/Face list
                
                        usemtl Default
                        f 4 3 2 1
                        f 2 6 5 1
                        f 3 7 6 2
                        f 8 7 3 4
                        f 5 8 4 1
                        f 6 7 8 5
                
                # End of file";
        
        //TestObjFile.Split('\n');



        public static ObjDocument ReadObjFile(Stream stream)
        {
            // TODO: add 'global' face pattern variable

            var geometricVertices = new List<Vector4>();
            var textureVertices = new List<Vector3>();
            var vertexNormals = new List<Vector3>();
            var faces = new List<Face>();

            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();

                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine))
                    {
                        continue;
                    }

                    var firstChar = trimmedLine[0];
                    if (firstChar == '#')
                    {
                        continue;
                    }

                    // Other whitespaces as separators?
                    var wordEnd = trimmedLine.IndexOf(' ');
                    if (wordEnd > -1)
                    {
                        var firstWord = trimmedLine.Substring(0, wordEnd);
                        var remainderLen = trimmedLine.Length - firstWord.Length;
                        var remainder = trimmedLine.Substring(wordEnd, remainderLen).Trim();
                        switch (firstWord)
                        {
                            case "v":
                                var vertex = ParseGeometricVertex(remainder);
                                geometricVertices.Add(vertex);
                                break;
                            case "f":
                                var face = Face.Parse(remainder);
                                faces.Add(face);
                                break;
                            case "vt":
                                var textureVertex = ParseTextureVertex(remainder);
                                textureVertices.Add(textureVertex);
                                break;
                            case "vn":
                                var vertexNormal = ParseVertexNormal(remainder);
                                vertexNormals.Add(vertexNormal);
                                break;
                            default:
                                Console.WriteLine("{0} ignored", trimmedLine);
                                break;
                        }
                    }
                    else
                    {
                        Debug.Fail("Invalid obj format");
                    }
                }
            }

            return new ObjDocument(
                geometricVertices,
                textureVertices,
                vertexNormals,
                faces);
        }

        private static Vector3 ParseVertexNormal(string str)
        {
            var vertices = str.Split(' ');
            Debug.Assert(vertices.Length == 3, "Invalid vertex normal count");
            
            if (float.TryParse(vertices[0], out var x) &&
                float.TryParse(vertices[1], out var y) &&
                float.TryParse(vertices[2], out var z))
            {
                return new Vector3(x, y, z);
            }

            Debug.Fail("Invalid vertex format");
            return Vector3.Zero;            
        }

        private static Vector3 ParseTextureVertex(string str)
        {
            var vertices = str.Split(' ');
            Debug.Assert(vertices.Length >= 2, "Invalid vertex count");

            if (float.TryParse(vertices[0], out var x)&&
                float.TryParse(vertices[1], out var y))
            {
                var result = new Vector3(x, y, 1.0f);
                if (vertices.Length == 3 && float.TryParse(vertices[2], out var z))
                {
                    result.Z = z;
                }

                return result;
            }
            
            Debug.Fail("Invalid vertex format");
            return Vector3.Zero;  
        }

        private static Vector4 ParseGeometricVertex(string str)
        {
            var vertices = str.Split(' ');
            Debug.Assert(vertices.Length >= 3, "Invalid vertex count");

            if (float.TryParse(vertices[0], out var x) &&
                float.TryParse(vertices[1], out var y) &&
                float.TryParse(vertices[2], out var z))
            {
                var result = new Vector4(x, y, z, 1.0f);
                if (vertices.Length == 4 && float.TryParse(vertices[3], out var w))
                {
                    result.W = w;
                }

                return result;
            }
            
            Debug.Fail("Invalid vertex format");
            return Vector4.Zero;  
        }

        public class Face
        {
            private const string NumberPattern = @"\d+$";
            private static readonly Regex OnlyVertices = new Regex($"^{NumberPattern}");

            public readonly List<int> GeometricVertexReferences;
            public readonly List<int> TextureVertexReferences;
            public readonly List<int> NormalVertexReferences;

            public Face()
            {
                GeometricVertexReferences = new List<int>();
                TextureVertexReferences = new List<int>();
                NormalVertexReferences = new List<int>();
            }

            public static Face Parse(string str)
            {
                var usedPattern = DetermineMatchingPattern(str);
                var faceElementsPerLine = str.Split(' ');
                var face = new Face();

                foreach (var faceStr in faceElementsPerLine)
                {
                    if (usedPattern != null && usedPattern.IsMatch(faceStr))
                    {
                        if (usedPattern == OnlyVertices)
                        {
                            var vertex = int.Parse(faceStr);
                            face.GeometricVertexReferences.Add(vertex);
                        }
                        // TODO: add other layouts
                    }
                    else
                    {
                       Debug.Fail("No matching pattern for face element layout");
                    }
                }

                return face;
            }


            private static Regex DetermineMatchingPattern(string str)
            {
                var firstFaceLen = str.IndexOf(' ');
                Regex usedPattern = null;
                if (firstFaceLen > 0)
                {
                    var firstFace = str.Substring(0, firstFaceLen);
                    if (OnlyVertices.IsMatch(firstFace))
                    {
                        usedPattern = OnlyVertices;
                    }
                }
                else
                {
                    //TODO throw
                    Console.WriteLine("{0} invalid face format", str);
                }

                return usedPattern;
            }
        }

        public ObjDocument Read(Stream stream)
        {
            return ReadObjFile(stream);
        }
    }
}