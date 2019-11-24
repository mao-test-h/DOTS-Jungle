using Unity.Entities;

namespace Main.ECS
{
    public struct Destroyable : IComponentData
    {
        public float Lifespan;
        public bool IsKilled;
    }
}
