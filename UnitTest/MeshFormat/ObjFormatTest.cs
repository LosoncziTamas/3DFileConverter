using System.Collections.Generic;
using System.Numerics;
using Converter.MeshFormat;
using NUnit.Framework;

namespace ConverterTest.MeshFormat
{
    [TestFixture]
    public class ObjFormatTest
    {
        [Test]
        public void TestCalculateTriangleNormal()
        {
            // given
            const float delta = 0.0001f;
            var expected = new Vector3(0.4422141f, -0.1679461f, -0.8810453f);
            var v1 = new Vector3(2.410367f, -0.777999f, -0.841105f);
            var v2 = new Vector3(2.407309f, -0.97498f, -0.805091f);
            var v3 = new Vector3(2.292449f, -0.871852f, -0.8824f);
            
            // when
            var result = ObjFormat.CalculateTriangleNormal(v1, v2, v3);
            
            // then
            TestUtils.AssertVector3sAreEqualWithPrecision(result, expected, delta);
        }

        [Test]
        public void TestPerformEarClipping()
        {
            // given
            var vertices = new List<Vector4>()
            {
                new Vector4(-0.5f, -0.5f, 0.5f, 1.0f),
                new Vector4(-0.5f, -0.5f, -0.5f, 1.0f),
                new Vector4(-0.5f, 0.5f, -0.5f, 1.0f),
                new Vector4(-0.5f, 0.5f, 0.5f, 1.0f),
                
                new Vector4(0.5f, -0.5f, 0.5f, 1.0f),
                new Vector4(0.5f, -0.5f, -0.5f, 1.0f),
                new Vector4(0.5f, 0.5f, -0.5f, 1.0f),
                new Vector4(0.5f, 0.5f, 0.5f, 1.0f),
            };

            var vertexReferences = new List<int>()
            {
                4, 3, 2, 1
            };

            var expected = new List<Mesh.Triangle>()
            {
                new Mesh.Triangle(new []
                {
                    new Vector3(-0.5f, 0.5f, 0.5f), 
                    new Vector3(-0.5f, 0.5f, -0.5f), 
                    new Vector3(-0.5f, -0.5f, -0.5f) 
                },
                new Vector3(-1f, 0f, 0f)),
                new Mesh.Triangle(new []
                    {
                        new Vector3(-0.5f, 0.5f, 0.5f), 
                        new Vector3(-0.5f, -0.5f, -0.5f), 
                        new Vector3(-0.5f, -0.5f, 0.5f) 
                    },
                    new Vector3(-1f, 0f, 0f))
            };

            // when
            var result = ObjFormat.PerformEarClipping(vertices, vertexReferences);
            
            // then
            Assert.AreEqual(expected.Count, result.Count);
            for (var i = 0; i < expected.Count; i++)
            {
                var t1 = expected[i];
                var t2 = result[i];
                for (var j = 0; j < 3; j++)
                {
                    TestUtils.AssertVector3sAreEqualWithPrecision(t1.Vertices[j], t2.Vertices[j]);
                    Assert.AreEqual(t1.Norm, t2.Norm);
                }
            }
        }
    }
}