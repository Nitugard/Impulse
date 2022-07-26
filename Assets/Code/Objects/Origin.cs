using Unity.Mathematics;

namespace ibc
{
    public struct Origin 
    {
        public double3 Position;
        public quaternion Rotation;
        public quaternion iRotation;
        
        public Origin(double3 p, quaternion r)
        {
            Position = p;
            Rotation = r;
            iRotation = math.inverse(r);
        }

        public void UpdateTransform()
        {
            iRotation = math.inverse(Rotation);
        }

        public void SetPosition(double3 pos)
        {
            Position = pos;
        }

        public void SetRotation(quaternion r)
        {
            Rotation = r;
            iRotation = math.inverse(r);
        }

        public double3 WorldToLocalDirection(double3 dir)
        {
            return math.mul(iRotation, (float3)dir);
        }

        public double3 LocalToWorldDirection(double3 dir)
        {
            return math.mul(Rotation, (float3)dir);
        }

        public double3 WorldToLocalPoint(double3 point)
        {
            return math.mul(iRotation, (float3)(point - Position));
        }

        public double3 LocalToWorldPoint(double3 point)
        {
            return math.mul(Rotation, (float3)point) + Position;
        }
    }
}