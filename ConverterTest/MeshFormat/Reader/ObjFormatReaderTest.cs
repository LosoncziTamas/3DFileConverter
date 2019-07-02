using System.Numerics;
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
    }
}