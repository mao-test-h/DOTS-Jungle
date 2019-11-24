using Unity.Entities;
using Unity.Mathematics;

namespace Main.ECS
{
    public struct ECSSphereCollider : IComponentData
    {
        public float3 Position;
        public bool IsUpdated;

        float radius;
        float3 offsetPosition;

        public float Radius => radius;
        public float3 OffsetPosition => offsetPosition;

        public ECSSphereCollider(float radius, float3 offsetPosition)
        {
            this.radius = radius;
            this.offsetPosition = offsetPosition;
            Position = default;
            IsUpdated = default;
        }

        public bool Intersect(in ECSSphereCollider another)
        {
            if (IsUpdated == false) return false;

            var diff = another.Position - Position;
            var dist2 = math.lengthsq(diff);
            var rad = Radius + another.Radius;
            var rad2 = rad * rad;
            return (dist2 < rad2);
        }
    }
}
