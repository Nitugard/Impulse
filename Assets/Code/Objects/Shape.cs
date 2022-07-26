using Unity.Mathematics;

namespace ibc
{
    public enum ShapeType : int
    {
        Box,
        Sphere
    }

    public interface IShape
    {
        ShapeType ShapeType { get; }
        float3x3 GetInertia(float mass);
        double3 GetSupportLocal(double3 dir);
    }

    public struct BoxShape : IShape
    {
        public float3 Extents;
        public ShapeType ShapeType => ShapeType.Box;

        public BoxShape(float3 extents)
        {
            Extents = extents;
        }

        public double3 GetSupportLocal(double3 direction)
        {
            var localPoint = new double3(direction.x < 0.0f ? -Extents.x : Extents.x,
                direction.y < 0.0f ? -Extents.y : Extents.y,
                direction.z < 0.0f ? -Extents.z : Extents.z);

            return localPoint;
        }


        public float3x3 GetInertia(float mass)
        {
            float w = Extents.x * 2;
            float h = Extents.y * 2;
            float d = Extents.z * 2;

            float ww = w * w;
            float hh = h * h;
            float dd = d * d;

            float m = 1 / 12.0f * mass;
            var i = new float3(m * (hh + dd), 0, 0);
            var j = new float3(0, m * (ww + dd), 0);
            var k = new float3(0, 0, m * (ww + hh));

            return new float3x3(i, j, k);
        }
    }

    public struct SphereShape : IShape
    {
        public float Radius;
        public ShapeType ShapeType => ShapeType.Sphere;

        public double3 GetSupportLocal(double3 direction)
        {
            return math.normalizesafe(direction) * Radius;
        }

        public float3x3 GetInertia(float mass)
        {
            return new float3x3(new float3(mass, 0, 0), new float3(0, mass, 0), new float3(0, 0, mass));
        }
    }
}
