using Unity.Entities;
using UnityEngine;

namespace Main.ECS
{
    sealed class FruitfulBananaAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        void IConvertGameObjectToEntity.Convert(
            Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            // non Serialize ComponentData.
            dstManager.AddComponentData(entity, new Speed());
            dstManager.AddComponentData(entity, new SphericalCoordinates());
            dstManager.AddComponentData(entity, new OriginalPosition());
        }
    }
}
