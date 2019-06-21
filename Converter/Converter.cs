using System;

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
                
                public GeometricVertex(float x, float y, float z, float w = 1.0f)
                {
                    this.x = x;
                    this.y = y;
                    this.z = z;
                    this.w = w;
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
                //TODO   
            }
            
            
        public static void Main(string[] args)
        {
            var lines = TestObjFile.Split('\n');
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                // Other whitespaces?
                var wordEnd = trimmedLine.IndexOf(' ');
                var firstWord = wordEnd > -1 ? trimmedLine.Substring(0, wordEnd) : trimmedLine;
                switch (firstWord)
                {
                    case "v":
                    case "vt":
                    case "vn":
                    case "f":
                        Console.WriteLine("{0} processed", trimmedLine);
                        break;
                    default:
                        Console.WriteLine("{0} ignored", trimmedLine);
                        break;
                }
            }
        }
    }
}