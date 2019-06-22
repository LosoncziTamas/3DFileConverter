using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Numerics;
using Converter.Documents;

namespace Converter
{
    public class StlWriter : Converter.IDocumentWriter<StlDocument>
    {

        private static void DummyNormalization()
        {
            var v1 = new Vector3(-0.5f, 0.5f, 0.5f);
            var v2 = new Vector3(-0.5f, 0.5f, -0.5f);
            var v3 = new Vector3(-0.5f, -0.5f, -0.5f);

            var u = v2 - v1;
            var v = v3 - v1;

            var norm = Vector3.Cross(u, v);
            norm = Vector3.Normalize(norm);
        }
        
        private static void WriteDummySTLFile()
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
            
            // Write(File.Open("box.stl", FileMode.Create), new StlDocument(box));
        }

        public void Write(Stream stream, StlDocument d)
        {
            using (var writer = new BinaryWriter(stream, Encoding.ASCII, true))
            {
                var header = new byte[80];
                writer.Write(header);
                writer.Write((UInt32) d.Triangles.Count);
                d.Triangles.ForEach(triangle => triangle.Write(writer));
            }
        }
    }
}