﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Converter
{
    internal class Converter
    {
        public static void Main(string[] args)
        {
            WriteSTLFile();
        }

        struct Vector3
        {
            private float x;
            private float y;
            private float z;

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public void Write(BinaryWriter writer)
            {
                writer.Write(x);
                writer.Write(y);
                writer.Write(z);
            }
        }

        struct Triangle
        {
            private Vector3[] vertices;
            private Vector3 norm;
            private UInt16 attributeByteCount;

            public Triangle(Vector3 norm, Vector3[] vertices)
            {
                this.vertices = vertices;
                this.norm = norm;
                attributeByteCount = 0;
            }
            
            public void Write(BinaryWriter writer)
            {
                norm.Write(writer);
                for (var i = 0; i < 3; ++i)
                {
                    vertices[i].Write(writer);
                }
                writer.Write(attributeByteCount);
            }
        }

        private static void WriteSTLFile()
        {
            var box = new List<Triangle>
            {
                new Triangle(
                    new Vector3(-0.904534f, 0.301511f, -0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(-0.5f, 0.5f, 0.5f), 
                        new Vector3(-0.5f, 0.5f, -0.5f), 
                        new Vector3(-0.5f, -0.5f, -0.5f)
                    }
                ),
                new Triangle(
                    new Vector3(-0.904534f, -0.301511f, 0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(-0.5f, 0.5f, 0.5f), 
                        new Vector3(-0.5f, -0.5f, -0.5f), 
                        new Vector3(-0.5f, -0.5f, 0.5f)
                    }
                ),
                new Triangle(
                    new Vector3(0.301511f, -0.904534f, -0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(-0.5f, -0.5f, -0.5f), 
                        new Vector3(0.5f, -0.5f, -0.5f), 
                        new Vector3(0.5f, -0.5f, 0.5f)
                    }
                ),
                new Triangle(
                    new Vector3(-0.301511f, -0.904534f, 0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(-0.5f, -0.5f, -0.5f), 
                        new Vector3( 0.5f, -0.5f, 0.5f), 
                        new Vector3(-0.5f, -0.5f, 0.5f)
                    }
                ),                
                new Triangle(
                    new Vector3(0.301511f, 0.301511f, -0.904534f),
                    new Vector3[3]
                    {
                        new Vector3(-0.5f, 0.5f, -0.5f), 
                        new Vector3( 0.5f, 0.5f, -0.5f), 
                        new Vector3(0.5f, -0.5f, -0.5f)
                    }
                ),
                new Triangle(
                    new Vector3(-0.301511f, -0.301511f, -0.904534f),
                    new Vector3[3]
                    {
                        new Vector3(-0.5f, 0.5f, -0.5f), 
                        new Vector3( 0.5f, -0.5f, -0.5f), 
                        new Vector3(-0.5f, -0.5f, -0.5f)
                    }
                ),
                new Triangle(
                    new Vector3(0.301511f, 0.904534f, -0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(0.5f, 0.5f, 0.5f), 
                        new Vector3(0.5f, 0.5f, -0.5f), 
                        new Vector3(-0.5f, 0.5f, -0.5f)
                    }
                ),new Triangle(
                    new Vector3(-0.301511f, 0.904534f, 0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(0.5f, 0.5f, 0.5f), 
                        new Vector3(-0.5f, 0.5f, -0.5f), 
                        new Vector3(-0.5f, 0.5f, 0.5f)
                    }
                ),new Triangle(
                    new Vector3(0.301511f, 0.301511f, 0.904534f),
                    new Vector3[3]
                    {
                        new Vector3(0.5f, -0.5f, 0.5f), 
                        new Vector3(0.5f, 0.5f, 0.5f), 
                        new Vector3(-0.5f, 0.5f, 0.5f)
                    }
                ),new Triangle(
                    new Vector3(-0.301511f, -0.301511f, 0.904534f),
                    new Vector3[3]
                    {
                        new Vector3(0.5f, -0.5f, 0.5f), 
                        new Vector3(-0.5f, 0.5f, 0.5f), 
                        new Vector3(-0.5f, -0.5f, 0.5f)
                    }
                ),new Triangle(
                    new Vector3(0.904534f, 0.301511f, -0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(0.5f, -0.5f, -0.5f), 
                        new Vector3(0.5f, 0.5f, -0.5f), 
                        new Vector3(0.5f, 0.5f, 0.5f)
                    }
                ),new Triangle(
                    new Vector3(0.904534f, -0.301511f, 0.301511f),
                    new Vector3[3]
                    {
                        new Vector3(0.5f, -0.5f, -0.5f), 
                        new Vector3(0.5f, 0.5f, 0.5f), 
                        new Vector3(0.5f, -0.5f, 0.5f)
                    }
                )
            };
            
            WriteBinary(box, File.Open("box.stl", FileMode.Create));
        }

        private static void WriteBinary(List<Triangle> triangles, Stream stream)
        {
            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                var header = new byte[80];
                writer.Write(header);
                writer.Write((UInt32) triangles.Count);
                triangles.ForEach(triangle => triangle.Write(writer));
            }
        }
    }
}