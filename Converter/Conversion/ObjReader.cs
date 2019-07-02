using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;
using Converter.Data;
using Converter.Documents;

namespace Converter.Conversion
{
    public class ObjReader : IMeshReader
    {
        private static readonly Regex OnlyVertices = new Regex(@"^\d+$");
        private static readonly Regex VerticesAndNormals = new Regex(@"^\d+\/\/\d+$");
        private static readonly Regex VerticesAndTexture = new Regex(@"^\d+\/\d+$");
        private static readonly Regex Complete = new Regex(@"^\d+\/\d+\/\d+$");
        
        public Mesh ReadFromStream(Stream inputStream)
        {
            var geometricVertices = new List<Vector4>();
            var textureVertices = new List<Vector3>();
            var vertexNormals = new List<Vector3>();
            var faces = new List<ObjDocument.Face>();

            using (var reader = new StreamReader(inputStream))
            {
                while (reader.Peek() > -1)
                {
                    var trimmedLine = reader.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(trimmedLine))
                    {
                        continue;
                    }

                    var isComment = trimmedLine[0]  == '#';
                    if (isComment)
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
                                geometricVertices.Add(ParseGeometricVertex(remainder));
                                break;
                            case "f":
                                faces.Add(ParseFace(remainder));
                                break;
                            case "vt":
                                textureVertices.Add(ParseTextureVertex(remainder));
                                break;
                            case "vn":
                                vertexNormals.Add(ParseVertexNormal(remainder));
                                break;
                        }
                    }
                }
            }
            
            var obj = new ObjDocument(
            geometricVertices,
            textureVertices,
            vertexNormals,
            faces);
            return ObjDocument.ToMesh(obj); 
        }

        private Vector3 ParseVertexNormal(string str)
        {
            var vertices = str.Split(' ');
            if (vertices.Length != 3)
            {
                throw new FormatException($"Unexpected vertex normal count {vertices.Length}");
            }

            if (float.TryParse(vertices[0], out var x) &&
                float.TryParse(vertices[1], out var y) &&
                float.TryParse(vertices[2], out var z))
            {
                return new Vector3(x, y, z);
            }

            throw new FormatException("Vertex normal parsing failed.");
        }

        private Vector3 ParseTextureVertex(string str)
        {
            var vertices = str.Split(' ');
            if (vertices.Length < 2)
            {
                throw new FormatException($"Unexpected texture vertex count {vertices.Length}");
            }

            if (float.TryParse(vertices[0], out var x) &&
                float.TryParse(vertices[1], out var y))
            {
                var result = new Vector3(x, y, 1.0f);
                if (vertices.Length == 3 && float.TryParse(vertices[2], out var z))
                {
                    result.Z = z;
                }

                return result;
            }
            throw new FormatException("Texture Vertex parsing failed.");
        }

        private Vector4 ParseGeometricVertex(string str)
        {
            var vertices = str.Split(' ');
            if (vertices.Length != 3)
            {
                throw new FormatException($"Unexpected vertex count {vertices.Length}");
            }

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
            throw new FormatException("Vertex parsing failed.");
        }

        public static ObjDocument.Face ParseFace(string str)
        {
            var faceLayout = DetermineFaceLayout(str);
            var faceElementsPerLine = str.Split(' ');
            var result = new ObjDocument.Face();
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
}