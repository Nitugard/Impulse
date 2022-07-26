

using Unity.Mathematics;

namespace ibc
{

    public struct Common
    {

        public static float SafeInverse(float a)
        {
            if (a > math.EPSILON)
                return 1.0f / a;
            return 0f;
        }
        
        public static void ComputeBasis(float3 a, out float3 b, out float3 c)
        {
            if (a.x >= 0.57735027f)
                b = new float3(a.y, -a.x, 0.0f);
            else
                b = new float3(0.0f, a.z, -a.y);

            b = math.normalizesafe(b);
            c = math.cross(a, b);
        }
        
        public static void DebugDrawArrow(float3 p0, float3 p1, float arrowSize, UnityEngine.Color color)
        {
            var dir = math.normalize(p1 - p0);
            p1 -= dir * arrowSize;

            ComputeBasis(dir, out var t1, out var t2);
            if (math.lengthsq(t1) <= math.EPSILON) ComputeBasis(-dir, out t1, out t2);

            UnityEngine.Debug.DrawLine(p0, p1, color);
            UnityEngine.Debug.DrawLine(p1, p1 - (dir + t1) * arrowSize, color);
            UnityEngine.Debug.DrawLine(p1, p1 - (dir + t2) * arrowSize, color);
            UnityEngine.Debug.DrawLine(p1, p1 - (dir - t1) * arrowSize, color);
            UnityEngine.Debug.DrawLine(p1, p1 - (dir - t2) * arrowSize, color);
        }
    }
}