using System;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Converter.MeshFormat.Reader
{
    public class ObjFormatReader : IMeshFormatReader
    {        
        private static readonly Regex OnlyVertices = new Regex(@"^[\-]*\d+$");
        private static readonly Regex VerticesAndNormals = new Regex(@"^[\-]*\d+\/\/[\-]*\d+$");
        private static readonly Regex VerticesAndTexture = new Regex(@"^[\-]*\d+\/[\-]*\d+$");
        private static readonly Regex Complete = new Regex(@"^[\-]*\d+\/[\-]*\d+\/[\-]*\d+$");

        public string Tag => ".obj";

        public Mesh ReadFromStream(Stream inputStream)
        {            
            var obj = new ObjFormat();
            using (var reader = new StreamReader(inputStream))
            {
                while (reader.Peek() > -1)
                {
                    var trimmedLine = reader.ReadLine()?.Trim();
                    if (string.IsNullOrEmpty(trimmedLine))
                    {
                        continue;
                    }

                    var isComment = trimmedLine[0] == '#';
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
                                obj.GeometricVertices.Add(ParseGeometricVertex(remainder));
                                break;
                            case "f":
                                obj.Faces.Add(ParseFace(remainder, obj));
                                break;
                            case "vt":
                                obj.TextureVertices.Add(ParseTextureVertex(remainder));
                                break;
                            case "vn":
                                obj.VertexNormals.Add(ParseVertexNormal(remainder));
                                break;
                        }
                    }
                }
            }
            return ObjFormat.ToMesh(obj);
        }

        internal Vector3 ParseVertexNormal(string str)
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

        internal Vector3 ParseTextureVertex(string str)
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

        internal Vector4 ParseGeometricVertex(string str)
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

        private int ParseFaceElement(string faceElement, int referenceListLength)
        {
            var vertexRef = int.Parse(faceElement);
            if (vertexRef < 0)
            {
                vertexRef += referenceListLength + 1;
            }
            return vertexRef;
        }
        
        internal ObjFormat.Face ParseFace(string str, ObjFormat obj)
        {
            var faceLayout = DetermineFaceLayout(str);
            var faceElementsPerLine = str.Split(' ');
            var result = new ObjFormat.Face();
            foreach (var faceElement in faceElementsPerLine)
            {
                if (faceLayout != null && faceLayout.IsMatch(faceElement))
                {
                    if (faceLayout == OnlyVertices)
                    {
                        var vertexRef = ParseFaceElement(faceElement, obj.GeometricVertices.Count);
                        result.GeometricVertexReferences.Add(vertexRef);
                    }
                    else if (faceLayout == Complete)
                    {
                        var elements = faceElement.Split('/');
                        result.GeometricVertexReferences.Add(ParseFaceElement(elements[0], obj.GeometricVertices.Count));
                        result.TextureVertexReferences.Add(ParseFaceElement(elements[1], obj.TextureVertices.Count));
                        result.NormalVertexReferences.Add(ParseFaceElement(elements[2], obj.VertexNormals.Count));
                    }
                    else if (faceLayout == VerticesAndNormals)
                    {
                        var elements = faceElement.Split('/');
                        result.GeometricVertexReferences.Add(ParseFaceElement(elements[0], obj.GeometricVertices.Count));
                        result.NormalVertexReferences.Add(ParseFaceElement(elements[2], obj.VertexNormals.Count));
                    }
                    else if (faceLayout == VerticesAndTexture)
                    {
                        var elements = faceElement.Split('/');
                        result.GeometricVertexReferences.Add(ParseFaceElement(elements[0], obj.GeometricVertices.Count));
                        result.TextureVertexReferences.Add(ParseFaceElement(elements[1], obj.TextureVertices.Count));
                    }
                }
                else
                {
                    throw new FormatException($"Not recognizable .obj face element layout: {faceElement}");
                }
            }

            return result;
        }

        internal Regex DetermineFaceLayout(string str)
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