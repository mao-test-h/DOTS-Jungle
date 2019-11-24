using Unity.Entities;
using UnityEngine;
using Main.Enums;

namespace Main.ECS
{
    [RequireComponent(typeof(ECSSphereColliderComponent))]
    sealed unsafe class DrummingBulletAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [SerializeField] ECSSettings _settings = default;
        [SerializeField] Bullet _bullet = default;
        ECSSphereColliderComponent _sphereColliderComponent;

        void IConvertGameObjectToEntity.Convert(
            Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BulletType {Value = _bullet});
            dstManager.AddComponentData(entity, new BarrageConfig
            {
                ConfigPtr = _settings.NativeBarrageConfig.GetUnsafePtr,
            });

            _sphereColliderComponent = GetComponent<ECSSphereColliderComponent>();
            var sphereCollider = _sphereColliderComponent.GetComponentData;
            dstManager.AddComponentData(entity, sphereCollider);

            // non Serialize ComponentData.
            dstManager.AddComponentData(entity, new Destroyable());
            dstManager.AddComponentData(entity, new OriginalPosition());
            dstManager.AddComponentData(entity, new SphericalCoordinates());
            dstManager.AddComponentData(entity, new DrummingTag());
        }
    }
}
