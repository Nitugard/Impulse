using Unity.Mathematics;

namespace ibc
{

    public struct Simplex
    {
        private const double Epsilon = math.EPSILON;
        private const double EpsilonSquared = Epsilon * Epsilon;

        private SupportPoint[] _simplexPoints;
        private int _usedPoints;

        /// <summary>
        /// Test to see whether points can contain the origin.
        /// </summary>
        /// <returns>True if they can not contain the origin</returns>
        public bool IsAffinelyDependent()
        {
            switch (_usedPoints)
            {
                //trivial independence
                case 0: case 1: return false;
                //two points are independent if their distance is larger then zero
                case 2: return math.lengthsq(_simplexPoints[0].W - _simplexPoints[1].W) < EpsilonSquared;
                //three points are independent if they form a triangle with area larger than a zero
                case 3:
                    double3 l1 = _simplexPoints[1].W - _simplexPoints[0].W;
                    double3 l2 = _simplexPoints[2].W - _simplexPoints[0].W;
                    return math.lengthsq(math.cross(l1, l2)) < EpsilonSquared;
                //four points are independent if they form a tetrahedron with volume greater than a zero
                case 4:
                    double3 ad = _simplexPoints[0].W - _simplexPoints[3].W;
                    double3 bd = _simplexPoints[1].W - _simplexPoints[3].W;
                    double3 cd = _simplexPoints[2].W - _simplexPoints[3].W;
                    return math.abs(math.dot(ad, math.cross(bd, cd))) <= Epsilon;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the simplex encloses origin.
        /// </summary>
        public bool ContainsOrigin(ref double3 v)
        {
            switch (_usedPoints)
            {
                case 1:
                    v = -v;
                    return false;
                case 2:
                    double3 a = _simplexPoints[1].W;
                    double3 b = _simplexPoints[0].W;
                    double3 ab = b - a;
                    v = TripleProduct(ab, -a, ab);

                    //origin is on this line
                    if (math.lengthsq(v) < EpsilonSquared)
                    {
                        v = math.cross(b - a, new double3(1, 0, 0));
                        if (math.lengthsq(v) < EpsilonSquared) v = math.cross(b - a, new double3(0, 0, 1));
                    }
                    return false;
                case 3:
                    Triangle(_simplexPoints[2].W, _simplexPoints[1].W, _simplexPoints[0].W, ref v);
                    return false;
                case 4:
                    return Tetrahedron(_simplexPoints[3].W, _simplexPoints[2].W, _simplexPoints[1].W, _simplexPoints[0].W, ref v);

            }

            return false;
        }

        private void Triangle(double3 a, double3 b, double3 c, ref double3 searchDir)
        {
            double3 n = math.cross(b - a, c - a); //triangle normal
            double3 AO = -a;

            if (math.dot(math.cross(b - a, n), AO) > 0)
            {
                Remove(0);
                searchDir = math.cross(math.cross(b - a, AO), b - a);
                return;
            }

            if (math.dot(math.cross(n, c - a), AO) > 0)
            {
                Remove(1);
                searchDir = math.cross(math.cross(c - a, AO), c - a);
                return;
            }

            //above triangle
            if (math.dot(n, AO) > 0)
            {
                searchDir = n;
                return;
            }

            //below triangle
            searchDir = -n;
        }


        private bool Tetrahedron(double3 a, double3 b, double3 c, double3 d, ref double3 searchDir)
        {
            //We know a priori that origin is above bcd and below a
            double3 ABC = math.cross(b - a, c - a);
            double3 ACD = math.cross(c - a, d - a);
            double3 ADB = math.cross(d - a, b - a);

            double3 AO = -a;

            if (math.dot(ABC, AO) > 0)
            {
                Remove(0);
                searchDir = ABC;
                return false;
            }

            if (math.dot(ACD, AO) > 0)
            {
                Remove(2);
                searchDir = ACD;
                return false;
            }

            if (math.dot(ADB, AO) > 0)
            {
                Remove(1);
                searchDir = ADB;
                return false;
            }

            return true;
        }


        private double3 TripleProduct(double3 a, double3 b, double3 c)
        {
            return math.cross(math.cross(a, b), c);
        }


        public void Add(SupportPoint w)
        {
            _simplexPoints[_usedPoints] = w;
            _usedPoints++;
        }

        public void Remove(int index)
        {

            _usedPoints--;
            _simplexPoints[index] = _simplexPoints[_usedPoints];
        }

        public SupportPoint GetLast()
        {
            return _simplexPoints[_usedPoints - 1];
        }

        public void Clear()
        {
            _usedPoints = 0;
        }
    }
}