
using Unity.Mathematics;

namespace ibc
{

    public enum BodyType
    {
        Static, //static bodies are not integrated, never moved
        Kinematic, //kinematic bodies are not integrated, can be moved 
        Dynamic //dynamic bodies are integrated, moved by physics
    }


    public struct BodyProperties
    {
        public readonly float Restitution;
        public readonly float Friction;

        public BodyProperties(float friction = .25f, float restitution = .0f)
        {
            Restitution = restitution;
            Friction = friction;
        }


        public static BodyProperties Default = new BodyProperties(friction: 0.25f, restitution: 0);


    }

    public struct DynamicProperties 
    {
        public const float AngularDamping = 0.98f;
        public const float LinearDamping = 0.98f;

        public float3 Velocity;
        public float3 AngularVelocity;

        public float3 Torque;
        public float3 Force;

        public float InverseMass;
        public float3x3 InverseInertia;
        public float3x3 WorldInverseInertia;


        public static DynamicProperties Zero => 
            new DynamicProperties()
            {
                Velocity = float3.zero,
                AngularVelocity = float3.zero,
                InverseMass = Common.SafeInverse(0),
                InverseInertia = float3x3.zero,
                WorldInverseInertia = float3x3.zero
            };
        

        public DynamicProperties(float mass, IShape shape)
        {
            Velocity = float3.zero;
            AngularVelocity = float3.zero;
            Torque = float3.zero;
            Force = float3.zero;

            InverseMass = Common.SafeInverse(mass);
            if(InverseMass < math.EPSILON) InverseInertia = float3x3.zero;
            else InverseInertia = math.inverse(shape.GetInertia(mass));
            WorldInverseInertia = InverseInertia;
        }

        public float3x3 GetWorldInertia(quaternion qrot)
        {
            //I = RDR^t where R rotation matrix, D diagonal inertia tensor in object space
            float4x4 x = float4x4.TRS(float3.zero, qrot, new float3(1, 1, 1));
            float3x3 rot = new float3x3(x.c0.xyz, x.c1.xyz, x.c2.xyz);
            return math.mul(math.mul(rot, InverseInertia), math.transpose(rot));
        }
    }

    public struct Body 
    {
        public ShapeType ShapeType;
        public BodyType BodyType;
    }
}