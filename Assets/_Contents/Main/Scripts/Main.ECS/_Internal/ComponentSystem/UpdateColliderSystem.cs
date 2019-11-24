using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Main.ECS
{
    // 衝突判定プリミティブの更新
    [UpdateInGroup(typeof(CollisionUpdateGroup))]
    sealed class UpdateColliderSystem : JobComponentSystem
    {
        [BurstCompile]
        struct Job : IJobForEach<Translation, Rotation, ECSSphereCollider>
        {
            public void Execute(
                [ReadOnly] ref Translation translation,
                [ReadOnly] ref Rotation rotation,
                ref ECSSphereCollider collider)
            {
                collider.Position = math.mul(rotation.Value, collider.OffsetPosition) + translation.Value;
                collider.IsUpdated = true;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) =>
            new Job().Schedule(this, inputDeps);
    }
}
