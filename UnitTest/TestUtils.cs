using System;
using System.Numerics;
using NUnit.Framework;

namespace ConverterTest
{
    public static class TestUtils
    {
        public static void AssertVector3sAreEqualWithPrecision(Vector3 result, Vector3 expected, float delta = Single.Epsilon)
        {
            Assert.AreEqual(result.X, expected.X, delta);
            Assert.AreEqual(result.Y, expected.Y, delta);
            Assert.AreEqual(result.Z, expected.Z, delta);
        }
    }
}