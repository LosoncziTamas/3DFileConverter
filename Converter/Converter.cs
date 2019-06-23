﻿using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Converter.Documents;

namespace Converter
{
    public static class Converter
    {
        public interface IConversionStrategy<Source, Destination> 
            where Source : IDocument 
            where Destination : IDocument
        {
            IDocumentReader<Source> GetReader();
            IDocumentWriter<Destination> GetWriter();

            Destination apply(Source source);
        }

        public interface IDocument
        {
            
        }

        public interface IDocumentReader<Source> where Source : IDocument
        {
            Source Read(Stream stream);
        }

        public interface IDocumentWriter<Destination> where Destination : IDocument
        {
            void Write(Stream stream, Destination d);
        }

        public class ObjToStlConversionStrategy : IConversionStrategy<ObjDocument, StlDocument>
        {
            public IDocumentReader<ObjDocument> GetReader()
            {
                return new ObjReader();
            }

            public IDocumentWriter<StlDocument> GetWriter()
            {
                return new StlWriter();
            }

            public StlDocument apply(ObjDocument source)
            {
                return Triangulate(source);
            }
        }
        
        public static void Main(string[] args)
        {
            var inputPath = "mini.obj";
            var outputPath = "obj_test.stl";

            var strategy = new ObjToStlConversionStrategy();
            var reader = strategy.GetReader();
            var obj = reader.Read(File.Open(inputPath, FileMode.Open));
            var stl = strategy.apply(obj);
            var writer = strategy.GetWriter();
            writer.Write(File.Open(outputPath, FileMode.Create), stl);
        }

        private static StlDocument Triangulate(ObjDocument objDocument)
        {
            var result = new List<Triangle>();
            
            foreach (var face in objDocument.faces)
            {
                if (IsClockWiseOrder(face))
                {
                    ReverseVertexOrder(face);
                }

                if (face.GeometricVertexReferences.Count > 3)
                {
                    var clippedTriangles = EarClip(objDocument.geometricVertices, face);
                    foreach (var clippedTriangle in clippedTriangles)
                    {
                        result.Add(clippedTriangle);
                    }
                }
                else
                {           
                    //TODO: calculate norm if not provided
                    // Reference numbers start from 1
                    var v1 = objDocument.geometricVertices[face.GeometricVertexReferences[0] - 1];
                    var v2 = objDocument.geometricVertices[face.GeometricVertexReferences[1] - 1];
                    var v3 = objDocument.geometricVertices[face.GeometricVertexReferences[2] - 1];
                    
                    result.Add(new Triangle(Vector3.Zero, new Vector3[3]
                    {
                        new Vector3(v1.X, v1.Y, v1.Z), 
                        new Vector3(v2.X, v2.Y, v2.Z), 
                        new Vector3(v3.X, v3.Y, v3.Z) 
                    }));
                }
            }
            
            return new StlDocument(result);
        }

        private static List<Triangle> EarClip(List<Vector4> geometricVertices, ObjReader.Face face)
        {
            var faceVertexCount = face.GeometricVertexReferences.Count;
            Vector3[] faceVertices = new Vector3[faceVertexCount];
            for (var i = 0; i < faceVertexCount; i++)
            {
                var geoVertex = geometricVertices[face.GeometricVertexReferences[i] - 1];
                faceVertices[i] =  new Vector3(geoVertex.X, geoVertex.Y, geoVertex.Z);
            }

            var result = new List<Triangle>();

            for (var i = 1; i < faceVertices.Length - 1; ++i)
            {
                result.Add(new Triangle(Vector3.Zero, new Vector3[3]
                {
                   faceVertices[0], 
                   faceVertices[i], 
                   faceVertices[i + 1]
                }));
            }

            return result;
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