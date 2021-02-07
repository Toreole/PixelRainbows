using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelRainbows
{
    ///<summary>Utility class with extension methods and other neat things</summary>
    public static class Util 
    {
        ///<summary>Returns the Vector with a different Z coordinate</summary>
        public static Vector3 WithZ(this Vector3 orig, float z)
        {
            Vector3 val = orig;
            val.z = z;
            return val;
        }
        public static Vector3 WithZ(this Vector2 orig, float z)
        {
            Vector3 val = orig;
            val.z = z;
            return val;
        }

        public static bool InRange(this int i, int lower, int upper)
            => lower <= i && i <= upper;
    }
}
