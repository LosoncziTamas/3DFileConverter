using System.IO;
using System.Numerics;
using Converter.MeshFormat;
using Converter.MeshFormat.Writer;
using Moq;
using NUnit.Framework;

namespace ConverterTest.MeshFormat.Writer
{
    [TestFixture]
    public class StlFormatWriterTest
    {
        private StlFormatWriter writer;
        
        [SetUp]
        public void SetUp()
        {
            writer = new StlFormatWriter();
        }
        
        [Test]
        public void TestWriteVector3()
        {
            // given
            var binaryWriterMock = new Mock<BinaryWriter>();
            var vec = new Vector3(1.0f, 2.0f, 3.0f);
            
            // when
            writer.WriteVector3(vec, binaryWriterMock.Object);
            
            // then
            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(vec.X), Times.Once);
            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(vec.Y), Times.Once);
            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(vec.Z), Times.Once);
            binaryWriterMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void TestWriteTriangle()
        {
            // given
            var binaryWriterMock = new Mock<BinaryWriter>();
            var vertices = new Vector3[3]
            {
                new Vector3(-1.0f, -2.0f, 3.0f),
                new Vector3(-4.0f, 5.0f, -6.0f),
                new Vector3(7.0f, 8.0f, 9.0f)
            };
            var norm = new Vector3(10.0f, 11.0f, 12.0f);
            var triangle = new StlFormat.Triangle(norm, vertices);
            
            // when
            writer.WriteTriangle(triangle, binaryWriterMock.Object);
            
            // then
            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(norm.X), Times.Once);
            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(norm.Y), Times.Once);
            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(norm.Z), Times.Once);

            for (var i = 0; i < 3; ++i)
            {
                binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(vertices[i].X), Times.Once);
                binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(vertices[i].Y), Times.Once);
                binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(vertices[i].Z), Times.Once);
            }

            binaryWriterMock.Verify(binaryWriter => binaryWriter.Write(triangle.AttributeByteCount), Times.Once);
        }
    }
}