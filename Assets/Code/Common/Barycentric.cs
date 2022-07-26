using Unity.Mathematics;

namespace ibc
{
    public struct Barycentric
    {
        
        public readonly double U;
        public readonly double V;
        public readonly double W;
        
        public Barycentric(double u, double v, double w)
        {
            this.U = u;
            this.V = v;
            this.W = w;
        }

        public Barycentric(double3 v1, double3 v2, double3 v3, double3 point)
        {
            double3 a = v2 - v3;
            double3 b = v1 - v3;
            double3 c = point - v3;
            double aLength = a.x * a.x + a.y * a.y + a.z * a.z;
            double bLength = b.x * b.x + b.y * b.y + b.z * b.z;
            double ab = a.x * b.x + a.y * b.y + a.z * b.z;
            double ac = a.x * c.x + a.y * c.y + a.z * c.z;
            double bc = b.x * c.x + b.y * c.y + b.z * c.z;
            double d = aLength * bLength - ab * ab;

            if (d < math.EPSILON)
            {
                U = 0;
                V = 0;
                W = 1.0f;
            }
            else
            {
                U = (aLength * bc - ab * ac) / d;
                V = (bLength * ac - ab * bc) / d;
                W = 1.0f - U - V;
            }
        }

        public double3 Interpolate(double3 v1, double3 v2, double3 v3)
        {
            return v1 * U + v2 * V + v3 * W;
        }
    }
}