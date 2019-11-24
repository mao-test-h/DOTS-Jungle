using Unity.Entities;

namespace Main.ECS
{
    public unsafe struct BarrageConfig : IComponentData
    {
        public ECSSettings.BarrageConfig* ConfigPtr;

        public ECSSettings.BarrageConfig Config => *ConfigPtr;
    }
}
