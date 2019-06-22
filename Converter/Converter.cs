using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Converter
{
    internal class Converter
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

            struct GeometricVertex
            {
                private float x;
                private float y;
                private float z;
                private float w;
                // TODO: reference number?
                
                public GeometricVertex(float x, float y, float z, float w)
                {
                    this.x = x;
                    this.y = y;
                    this.z = z;
                    this.w = w;
                }

                public static GeometricVertex Parse(string str)
                {
                    var vertices = str.Split(' ');
                    if (vertices.Length < 3)
                    {
                        Console.WriteLine("Invalid vertex count");
                        //TODO: throw
                        return new GeometricVertex(0, 0, 0, 0);
                    }
                    //TODO: check number format
                    var x = float.Parse(vertices[0]);
                    var y = float.Parse(vertices[1]);
                    var z = float.Parse(vertices[2]);
                    var w = vertices.Length == 4 ? float.Parse(vertices[3]) : 1.0f;
                    return new GeometricVertex(x, y, z, w);
                }

                public override string ToString()
                {
                    return $"{nameof(x)}: {x}, {nameof(y)}: {y}, {nameof(z)}: {z}, {nameof(w)}: {w}";
                }
            }

            struct TextureVertex
            {
                private float u;
                private float v;
                private float w;

                public TextureVertex(float u, float v = 1.0f, float w = 1.0f)
                {
                    this.u = u;
                    this.v = v;
                    this.w = w;
                }
            }

            struct VertexNormal
            {
                private float i;
                private float j;
                private float k;

                public VertexNormal(float i, float j, float k)
                {
                    this.i = i;
                    this.j = j;
                    this.k = k;
                }
            }

            struct Face
            {
                
                private const string NumberPattern = @"\d$";
                private static readonly Regex OnlyVertices = new Regex($"^{NumberPattern}");
                
                private List<int> geometricVertexReferences;
                private List<int> textureVertexReferences;
                private List<int> normalVertexReferences;
                
                public static Face Parse(string str)
                {
                    var usedPattern = DetermineMatchingPattern(str);
                    var faceElementsPerLine = str.Split(' ');

                    var face = new Face
                    {
                        geometricVertexReferences = new List<int>(),
                        textureVertexReferences = new List<int>(),
                        normalVertexReferences = new List<int>()
                    };

                    //1//1 in 1//1 2//2 3//3 4//4
                    foreach (var faceStr in faceElementsPerLine)
                    {
                        if (usedPattern != null && usedPattern.IsMatch(faceStr))
                        {
                            //1
                            if (usedPattern == OnlyVertices)
                            {
                                var vertex = int.Parse(faceStr);
                                face.geometricVertexReferences.Add(vertex);
                            }
                        }
                        else
                        {
                            return new Face();
                            //TODO: throw   
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
            
            
        public static void Main(string[] args)
        {
            var lines = TestObjFile.Split('\n');

            var geometricVertices = new List<GeometricVertex>();
            var faces = new List<Face>();
            // TODO: add 'global' face pattern variable
            
            foreach (var line in lines)
            {
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
                            var vertex = GeometricVertex.Parse(remainder);
                            geometricVertices.Add(vertex);
                            break;
                        case "f":
                            var face = Face.Parse(remainder);
                            faces.Add(face);
                            break;
                        case "vt":
                        case "vn":
                            Console.WriteLine("{0} processed", trimmedLine);
                            break;
                        default:
                            Console.WriteLine("{0} ignored", trimmedLine);
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid obj format");
                    //TODO: throw
                }
            }
            Console.WriteLine("{0}{1}", geometricVertices, faces);
        }  
    }
}