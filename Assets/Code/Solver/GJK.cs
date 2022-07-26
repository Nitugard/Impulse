
using Unity.Mathematics;

namespace ibc
{

    public struct GJK
    {
        private const uint GJK_MAX_NUM_ITERATIONS = 32;

        public bool Run(IShape s1, Origin o1, IShape s2, Origin o2, Simplex simplex)
        {
            simplex.Clear();

            var collision = false;

            double3 v = new double3(0, 1, 0);
            SupportPoint support;

            int iteration = 0;
            while (iteration < GJK_MAX_NUM_ITERATIONS)
            {
                support = GetSupport(s1, o1, s2, o2, v);
                simplex.Add(support);

                //check if the last point that got added passed the origin
                if (math.dot(simplex.GetLast().W, v) <= 0f)
                    break;

                if (simplex.IsAffinelyDependent())
                    break;

                //check if simplex contains the origin
                if (simplex.ContainsOrigin(ref v))
                {
                    collision = true;
                    break;
                }
                
                ++iteration;
            }

            return collision;
        }
        
        private double3 GetSupport(IShape s1, Origin o1, double3 d)
        {
            return o1.LocalToWorldPoint(s1.GetSupportLocal(o1.WorldToLocalDirection(d)));
        }
        
        private SupportPoint GetSupport(IShape e1, Origin o1, IShape e2, Origin o2, double3 d)
        {
            double3 support1 = GetSupport(e1, o1, d);
            double3 support2 = GetSupport(e2, o2, -d);
            double3 w = support1 - support2;

            return new SupportPoint(support1, support2, w);
        }
    }
}