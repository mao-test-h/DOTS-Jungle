using Unity.Entities;

namespace Main.ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    sealed class UpdateGroup : ComponentSystemGroup { }
}
