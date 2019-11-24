using Unity.Entities;

namespace Main.ECS
{
    public struct SphericalCoordinates : IComponentData
    {
        public MathematicsExtensions.SphericalCoordinates Value;
    }
}
