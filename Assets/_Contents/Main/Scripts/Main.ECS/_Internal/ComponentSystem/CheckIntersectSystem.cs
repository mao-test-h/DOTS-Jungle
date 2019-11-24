using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Main.ECS
{
    // 衝突判定検知処理
    // HACK: 愚直に総当りチェックしているので、空間分割とか試してみても良いかもしれない.
    [UpdateInGroup(typeof(CollisionUpdateGroup))]
    [UpdateAfter(typeof(UpdateColliderSystem))]
    sealed class CheckIntersectSystem : JobComponentSystem
    {
        enum HitState : byte
        {
            None,
            Hit,
            HitPlayer,
        }

        // ヒットチェック
        // ※このJob中でEntityCommandBufferに対する操作を同時に行うと、
        //   Burstが適用できなくなって重くなるので敢えて破棄処理とJobを分けている.
        [BurstCompile]
        struct CheckIntersectJob : IJobParallelFor
        {
            // バナナの木の弾
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Destroyable> BulletDestroyables;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BulletType> BulletTypes;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<ECSSphereCollider> BulletColliders;

            // プレイヤー
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<ECSSphereCollider> PlayerColliders;

            // ドラミング弾
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<ECSSphereCollider> DrummingColliders;

            // 破棄される弾の情報
            [WriteOnly] public NativeArray<PlayerHitEventState> PlayerHitEventStates;
            [WriteOnly] public NativeArray<HitState> HitStates;

            public void Execute(int index)
            {
                // 判定対象のバナナの木弾
                var bulletDestroyable = BulletDestroyables[index];
                var bulletType = BulletTypes[index];
                var bulletCollider = BulletColliders[index];

                HitStates[index] = HitState.None;

                // バナナの木弾 → プレイヤーの衝突検知
                // ぶつかったバナナの木弾にPlayerHitEventStateを貼り付けてDestroyしていく
                for (int j = 0; j < PlayerColliders.Length; j++)
                {
                    // 既に寿命などで死んでいる弾が混じっているのでチェック
                    if (bulletDestroyable.IsKilled) continue;

                    var playerCollider = PlayerColliders[j];
                    if (bulletCollider.Intersect(playerCollider) == false) continue;

                    // プレイヤーと衝突検知したらヒット通知用の状態にしてEntity自体は破棄する
                    var hitEventState = new PlayerHitEventState()
                    {
                        HitPosition = playerCollider.Position,
                        BulletType = bulletType.Value,
                    };
                    PlayerHitEventStates[index] = hitEventState;
                    HitStates[index] = HitState.HitPlayer;
                    bulletDestroyable.IsKilled = true;
                }

                // ドラミング → バナナの木弾の衝突検知
                for (int j = 0; j < DrummingColliders.Length; j++)
                {
                    if (bulletDestroyable.IsKilled) continue;
                    var drummingCollider = DrummingColliders[j];

                    // ヒットしたら破棄するだけ.
                    if (bulletCollider.Intersect(drummingCollider) == false) continue;

                    HitStates[index] = HitState.Hit;
                    bulletDestroyable.IsKilled = true;
                }
            }
        }

        struct DestroyBulletJob : IJobParallelFor
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> BulletEntities;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<PlayerHitEventState> PlayerHitEventStates;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<HitState> HitStates;

            public EntityCommandBuffer.Concurrent CommandBuffer;

            public void Execute(int index)
            {
                var hitState = HitStates[index];

                if (hitState == HitState.None) return;

                var bulletEntity = BulletEntities[index];

                // プレイヤー衝突判定がついている場合には付与
                if (hitState == HitState.HitPlayer)
                {
                    CommandBuffer.AddComponent(index, bulletEntity, PlayerHitEventStates[index]);
                }

                CommandBuffer.SetComponent(index, bulletEntity, new Destroyable {IsKilled = true});
            }
        }

        EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        EntityQuery _bananaTreeBulletQuery;
        EntityQuery _playerQuery;
        EntityQuery _drummingBulletQuery;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            _bananaTreeBulletQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<Destroyable>(),
                    ComponentType.ReadOnly<BulletType>(),
                    ComponentType.ReadOnly<ECSSphereCollider>(),
                },
                Any = new ComponentType[]
                {
                    ComponentType.ReadOnly<AimingTag>(),
                    ComponentType.ReadOnly<CircleTag>(),
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<DrummingTag>(),
                },
            });

            _playerQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<HitReceiverTag>(),
                    ComponentType.ReadOnly<ECSSphereCollider>(),
                },
            });

            _drummingBulletQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadWrite<Destroyable>(),
                    ComponentType.ReadOnly<BulletType>(),
                    ComponentType.ReadOnly<ECSSphereCollider>(),
                    ComponentType.ReadOnly<DrummingTag>(),
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<AimingTag>(),
                    ComponentType.ReadOnly<CircleTag>(),
                },
            });
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = inputDeps;

            const Allocator jobAllocator = Allocator.TempJob;

            // バナナの木の弾
            var bulletLength = _bananaTreeBulletQuery.CalculateEntityCount();
            var bulletEntities = _bananaTreeBulletQuery.ToEntityArray(jobAllocator);
            var bulletDestroyables = _bananaTreeBulletQuery.ToComponentDataArray<Destroyable>(jobAllocator);
            var bulletTypes = _bananaTreeBulletQuery.ToComponentDataArray<BulletType>(jobAllocator);
            var bulletColliders = _bananaTreeBulletQuery.ToComponentDataArray<ECSSphereCollider>(jobAllocator);

            // プレイヤー
            var playerColliders = _playerQuery.ToComponentDataArray<ECSSphereCollider>(jobAllocator);

            // ドラミング弾
            var drummingColliders = _drummingBulletQuery.ToComponentDataArray<ECSSphereCollider>(jobAllocator);

            // 破棄される弾の情報
            var playerHitEventStates = new NativeArray<PlayerHitEventState>(bulletLength, jobAllocator);
            var hitStates = new NativeArray<HitState>(bulletLength, jobAllocator);

            // ヒットチェック
            handle = new CheckIntersectJob()
            {
                BulletDestroyables = bulletDestroyables,
                BulletTypes = bulletTypes,
                BulletColliders = bulletColliders,

                PlayerColliders = playerColliders,
                DrummingColliders = drummingColliders,

                PlayerHitEventStates = playerHitEventStates,
                HitStates = hitStates,
            }.Schedule(bulletLength, 16, handle);

            // 弾の破棄
            handle = new DestroyBulletJob()
            {
                BulletEntities = bulletEntities,
                PlayerHitEventStates = playerHitEventStates,
                HitStates = hitStates,
                CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            }.Schedule(bulletLength, 16, handle);

            _entityCommandBufferSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}
