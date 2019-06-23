using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using Converter.Documents;

namespace Converter
{
    public class ObjReader
    {
        public static ObjDocument ReadObjFile(Stream stream)
        {
            var geometricVertices = new List<Vector4>();
            var textureVertices = new List<Vector3>();
            var vertexNormals = new List<Vector3>();
            var faces = new List<Face>();

            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() > -1)
                {
                    var trimmedLine = reader.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(trimmedLine))
                    {
                        continue;
                    }

                    var firstChar = trimmedLine[0];
                    if (firstChar == '#' || firstChar == 'g')
                    {
                        continue;
                    }

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
            private static readonly Regex OnlyVertices = new Regex(@"^\d+$");
            private static readonly Regex VerticesAndNormals = new Regex(@"^\d+\/\/\d+$");
            private static readonly Regex VerticesAndTexture = new Regex(@"^\d+\/\d+$");
            private static readonly Regex Complete = new Regex(@"^\d+\/\d+\/\d+$");

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
                var faceLayout = DetermineFaceLayout(str);
                var faceElementsPerLine = str.Split(' ');
                var result = new Face();
                foreach (var faceStr in faceElementsPerLine)
                {
                    if (faceLayout != null && faceLayout.IsMatch(faceStr))
                    {
                        if (faceLayout == OnlyVertices)
                        {
                            var vertex = int.Parse(faceStr);
                            result.GeometricVertexReferences.Add(vertex);
                        }
                        else if (faceLayout == Complete)
                        {
                            var elements = faceStr.Split('/');
                            result.GeometricVertexReferences.Add(int.Parse(elements[0]));
                            result.TextureVertexReferences.Add(int.Parse(elements[1]));
                            result.NormalVertexReferences.Add(int.Parse(elements[2]));
                        }
                        else if (faceLayout == VerticesAndNormals)
                        {
                            var elements = faceStr.Split('/');
                            result.GeometricVertexReferences.Add(int.Parse(elements[0]));
                            result.NormalVertexReferences.Add(int.Parse(elements[2]));
                        }
                        else if (faceLayout == VerticesAndTexture)
                        {
                            var elements = faceStr.Split('/');
                            result.GeometricVertexReferences.Add(int.Parse(elements[0]));
                            result.TextureVertexReferences.Add(int.Parse(elements[1]));
                        }
                    }
                    else
                    {
                        throw new FormatException($"Not recognizable .obj face element layout: {faceStr}");
                    }
                }

                return result;
            }


            private static Regex DetermineFaceLayout(string str)
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
                    else if (Complete.IsMatch(firstFace))
                    {
                        usedPattern = Complete;
                    }
                    else if (VerticesAndNormals.IsMatch(firstFace))
                    {
                        usedPattern = VerticesAndNormals;
                    }
                    else if (VerticesAndTexture.IsMatch(firstFace))
                    {
                        usedPattern = VerticesAndTexture;
                    }
                }
                else
                {
                    throw new FormatException($"Not recognizable .obj face element layout: {str}");
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