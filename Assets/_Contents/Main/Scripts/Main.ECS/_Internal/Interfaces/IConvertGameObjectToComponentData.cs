using Unity.Entities;

namespace Main.ECS
{
    interface IConvertGameObjectToComponentData<out T>
        where T : IComponentData
    {
        T GetComponentData { get; }
    }
}
