using Unity.Entities;
using Unity.Mathematics;

namespace Main.ECS
{
    public struct Angle : IComponentData
    {
        public float3 Value;
    }
}
