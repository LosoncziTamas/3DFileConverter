using System.Numerics;

namespace Converter.Utils
{
    public static class Extensions
    {
        public static Vector3 ToVector3(this Vector4 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
    }
}