using System;
using Unity.Mathematics;

namespace ibc
{

    [Serializable]
    public struct AABB
    {
        public double3 Min;
        public double3 Max;
        
        public AABB(double3 min, double3 max)
        {
            Min = min;
            Max = max;
        }


        public double3 GetCenter()
        {
            return Min + Max / 2.0f;
        }


        public AABB Transform(Origin origin)
        {
            double3 max = Max;
            double3 min = Min;
            quaternion rot = origin.Rotation;
            double3 pos = origin.Position;

            double w = (max.x - min.x) / 2.0f;
            double h = (max.y - min.y) / 2.0f;
            double d = (max.z - min.z) / 2.0f;

            double3 center = (Min + Max) / 2;
            float3 fwd = math.mul(rot, new float3(0, 0, (float)d));
            float3 up = math.mul(rot, new float3(0, (float)h, 0));
            float3 right = math.mul(rot, new float3((float)w, 0, 0));

            double3 pt0 = center - fwd - up + right;
            double3 pt1 = center - fwd - up - right;
            double3 pt2 = center + fwd - up + right;
            double3 pt3 = center + fwd - up - right;

            double3 pt4 = center - fwd + up + right;
            double3 pt5 = center - fwd + up - right;
            double3 pt6 = center + fwd + up + right;
            double3 pt7 = center + fwd + up - right;


//todo: improve this?
            max = math.max(pt0, math.max(pt1, math.max(pt2, math.max(pt3, math.max(pt4, math.max(pt5, math.max(pt6, pt7)))))));
            min = math.min(pt0, math.min(pt1, math.min(pt2, math.min(pt3, math.min(pt4, math.min(pt5, math.min(pt6, pt7)))))));

            return new AABB(min + pos, max + pos);
        }


        public AABB GrowBox(AABB other)
        {
            return new AABB(math.min(Min, other.Min), math.max(Max, other.Max));
        }



        public bool ContainsPoint(double3 point)
        {
            return !(point.x < Min.x || point.x > Max.x ||
                   point.y < Min.y || point.y > Max.y ||
                   point.z < Min.z || point.z > Max.z);
        }

        public bool ContainsBox(AABB box)
        {
            return Min.x <= box.Min.x && box.Max.x <= Max.x &&
                   Min.y <= box.Min.y && box.Max.y <= Max.y &&
                   Min.z <= box.Min.z && box.Max.z <= Max.z;
        }



        public bool IntersectsBox(AABB b)
        {

            return Min.x <= b.Max.x && Max.x >= b.Min.x &&
                   Min.y <= b.Max.y && Max.y >= b.Min.y &&
                   Min.z <= b.Max.z && Max.z >= b.Min.z;
        }

        public bool IntersectsSphere(double3 center, float radius)
        {
            return math.lengthsq(ClampPoint(center) - center) <= (radius * radius);
        }


        public double3 ClampPoint(double3 point)
        {
            return new double3(
                math.clamp(point.x, Min.x, Max.x),
                math.clamp(point.y, Min.y, Max.y),
                math.clamp(point.z, Min.z, Max.z)
                );
        }

        public double ClosestDistanceToPoint(double3 point)
        {
            return math.length(ClampPoint(point) - point);
        }
    }
}