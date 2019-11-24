using Unity.Entities;
using UnityEngine;

namespace Main.ECS.Provider.Hybrid
{
    // ECSからMonoBehaviourへのヒット通知(Hybridで処理)
    [UpdateInGroup(typeof(InitializationEventGroup))]
    sealed class HitEventDispatcherSystem : ComponentSystem
    {
        EntityQuery _playerHybridQuery;
        EntityQuery _playerHitEventQuery;

        protected override void OnCreate()
        {
            _playerHybridQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<HitReceiverTag>(),
                    typeof(Transform),
                },
            });

            _playerHitEventQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<PlayerHitEventState>(),
                },
            });
        }


        protected override void OnUpdate()
        {
            var transforms = _playerHybridQuery.GetTransformAccessArray();

            Entities.With(_playerHitEventQuery).ForEach((
                Entity entity,
                ref PlayerHitEventState state) =>
            {
                for (int i = 0; i < transforms.length; i++)
                {
                    var authoring = transforms[i].gameObject.GetComponent<HitReceiverComponent>();
                    authoring.OnCollisionHit(state.HitPosition, state.BulletType);
                }

                EntityManager.RemoveComponent<PlayerHitEventState>(entity);
                EntityManager.DestroyEntity(entity);
            });
        }
    }
}
