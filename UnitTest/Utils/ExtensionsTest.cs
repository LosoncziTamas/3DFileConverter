using System.Numerics;
using Converter.Utils;
using NUnit.Framework;

namespace ConverterTest.Utils
{
    public class ExtensionsTest
    {
        [Test]
        public void TestToVector3()
        {
            // given
            var vec = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            
            // when
            var result = vec.ToVector3();
            
            // then
            Assert.AreEqual(vec.X, result.X);
            Assert.AreEqual(vec.Y, result.Y);
            Assert.AreEqual(vec.Z, result.Z);
        }
    }
}