using Unity.Mathematics;

namespace ibc
{
    public struct SupportPoint
    {
        public double3 S1;
        public double3 S2;
        public double3 W;

        public SupportPoint(double3 s1, double3 s2, double3 w)
        {
            S1 = s1;
            S2 = s2;
            W = w;
        }
    }
}
