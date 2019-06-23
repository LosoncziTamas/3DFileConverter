using System.IO;
using System.Numerics;

namespace Converter.Utils
{
    public static class Extensions
    {
        public static void Write(this Vector3 vec, BinaryWriter writer)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);   
        }
        
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
    }
}