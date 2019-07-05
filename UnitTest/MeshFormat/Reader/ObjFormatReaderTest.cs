using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Converter.MeshFormat;
using Converter.MeshFormat.Reader;
using NUnit.Framework;

namespace ConverterTest.MeshFormat.Reader
{
    [TestFixture]
    public class ObjFormatReaderTest
    {
        private ObjFormatReader reader;
        
        [SetUp]
        protected void SetUp()
        {
            reader = new ObjFormatReader();
        }
        
        [Test]
        public void TestParseGeometricVertex()
        {
            // given
            var geoVertex = "2.229345 -0.992723 -0.862826";
            var expected = new Vector4(2.229345f, -0.992723f, -0.862826f, 1f);
            
            // when
            var result = reader.ParseGeometricVertex(geoVertex);

            // then
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void TestParseGeometricVertexShouldFail()
        {
            // given
            var geoVertex = "-0.992723 -0.862826";
            
            // when
            Assert.Throws<FormatException>(() => { reader.ParseGeometricVertex(geoVertex); });
        }
        
        [Test]
        public void TestParseFace1()
        {
            // given
            var expected = new ObjFormat.Face();
            var obj = new ObjFormat();
            expected.GeometricVertexReferences.AddRange(new List<int> {3, 4, 25, 26});
            var onlyVertexLayout = "3 4 25 26";

            // when
            var result = reader.ParseFace(onlyVertexLayout, obj);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
        }

        [Test]
        public void TestParseFace2()
        {
            // given
            var expected = new ObjFormat.Face();
            var obj = new ObjFormat();
            expected.GeometricVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.NormalVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            var vertexAndNormalLayout = "1//1 2//2 3//3 4//4";

            // when
            var result = reader.ParseFace(vertexAndNormalLayout, obj);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
            Assert.True(expected.NormalVertexReferences.SequenceEqual(result.NormalVertexReferences));
        }

        [Test]
        public void TestParseFace3()
        {
            // given
            var expected = new ObjFormat.Face();
            var obj = new ObjFormat();
            expected.GeometricVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.NormalVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.TextureVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            var completeLayout = "1/1/1 2/2/2 3/3/3 4/4/4";

            // when
            var result = reader.ParseFace(completeLayout, obj);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
            Assert.True(expected.NormalVertexReferences.SequenceEqual(result.NormalVertexReferences));
            Assert.True(expected.TextureVertexReferences.SequenceEqual(result.TextureVertexReferences));
        }

        [Test]
        public void TestParseFace4()
        {
            // given
            var expected = new ObjFormat.Face();
            var obj = new ObjFormat();
            expected.GeometricVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.TextureVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            var vertexAndTextureLayout = "1/1 2/2 3/3 4/4";

            // when
            var result = reader.ParseFace(vertexAndTextureLayout, obj);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
            Assert.True(expected.TextureVertexReferences.SequenceEqual(result.TextureVertexReferences));
        }
    }
}