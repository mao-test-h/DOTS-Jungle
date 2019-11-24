using Unity.Entities;
using Unity.Mathematics;

namespace Main.ECS
{
    public struct OriginalPosition : IComponentData
    {
        public float3 Value;
    }
}
