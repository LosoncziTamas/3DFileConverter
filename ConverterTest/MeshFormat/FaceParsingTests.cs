using System.Collections.Generic;
using System.Linq;
using Converter.MeshFormat;
using Converter.MeshFormat.Reader;
using NUnit.Framework;

namespace ConverterTest.MeshFormat
{
    [TestFixture]
    public class FaceParsingTests
    {
        [Test]
        public void TestFaceParse1()
        {
            // given
            var expected = new ObjFormat.Face();
            expected.GeometricVertexReferences.AddRange(new List<int> {3, 4, 25, 26});
            var onlyVertexLayout = "3 4 25 26";

            // when
            var result = ObjFormatReader.ParseFace(onlyVertexLayout);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
        }

        [Test]
        public void TestFaceParse2()
        {
            // given
            var expected = new ObjFormat.Face();
            expected.GeometricVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.NormalVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            var vertexAndNormalLayout = "1//1 2//2 3//3 4//4";

            // when
            var result = ObjFormatReader.ParseFace(vertexAndNormalLayout);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
            Assert.True(expected.NormalVertexReferences.SequenceEqual(result.NormalVertexReferences));
        }

        [Test]
        public void TestFaceParse3()
        {
            // given
            var expected = new ObjFormat.Face();
            expected.GeometricVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.NormalVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.TextureVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            var completeLayout = "1/1/1 2/2/2 3/3/3 4/4/4";

            // when
            var result = ObjFormatReader.ParseFace(completeLayout);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
            Assert.True(expected.NormalVertexReferences.SequenceEqual(result.NormalVertexReferences));
            Assert.True(expected.TextureVertexReferences.SequenceEqual(result.TextureVertexReferences));
        }

        [Test]
        public void TestFaceParse4()
        {
            // given
            var expected = new ObjFormat.Face();
            expected.GeometricVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            expected.TextureVertexReferences.AddRange(new List<int> {1, 2, 3, 4});
            var vertexAndTextureLayout = "1/1 2/2 3/3 4/4";

            // when
            var result = ObjFormatReader.ParseFace(vertexAndTextureLayout);

            // then
            Assert.True(expected.GeometricVertexReferences.SequenceEqual(result.GeometricVertexReferences));
            Assert.True(expected.TextureVertexReferences.SequenceEqual(result.TextureVertexReferences));
        }
    }
}