using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Main.Enums;

namespace Main.ECS.Provider.Hybrid
{
    [RequireComponent(typeof(ECSSphereColliderComponent))]
    [RequireComponent(typeof(ConvertToEntity))]
    [RequiresEntityConversion]
    public sealed class HitReceiverComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public event Action<float3> OnDamageHitEvent = default;
        public event Action<float3> OnBananaHitEvent = default;

        Entity _entity;
        ECSSphereColliderComponent _sphereColliderComponent;

        void IConvertGameObjectToEntity.Convert(
            Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            // GameObject → Entityの方向で情報を同期させる.
            _entity = entity;
            dstManager.AddComponentData(entity, new SyncTRSFromGameObject());

            _sphereColliderComponent = GetComponent<ECSSphereColliderComponent>();
            var sphereCollider = _sphereColliderComponent.GetComponentData;
            dstManager.AddComponentData(entity, sphereCollider);

            dstManager.AddComponentData(entity, new HitReceiverTag());
        }

        void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null) return;

            var entityManager = world.EntityManager;
            if (entityManager.Exists(_entity))
            {
                entityManager.DestroyEntity(_entity);
            }
        }

        public void OnCollisionHit(float3 hitPosition, Bullet bullet)
        {
            //Debug.Log($"    hit >>> {hitPosition}, {bullet}");
            switch (bullet)
            {
                case Bullet.Damage:
                    OnDamageHitEvent?.Invoke(hitPosition);
                    break;
                case Bullet.Banana:
                    OnBananaHitEvent?.Invoke(hitPosition);
                    break;
            }
        }
    }
}
