using Unity.Entities;

namespace Main.ECS
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed class InitializationEventGroup : ComponentSystemGroup { }
}
