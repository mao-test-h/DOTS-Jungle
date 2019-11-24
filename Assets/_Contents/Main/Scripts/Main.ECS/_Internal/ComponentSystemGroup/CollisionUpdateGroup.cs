using Unity.Entities;

namespace Main.ECS
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    sealed class CollisionUpdateGroup : ComponentSystemGroup { }
}
