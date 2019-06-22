using System.IO;
using System.Numerics;

namespace Converter
{
    public static class Extensions
    {
        public static void Write(this Vector3 vec, BinaryWriter writer)
        {
            writer.Write(vec.X);
            writer.Write(vec.Y);
            writer.Write(vec.Z);   
        }
    }
}