using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine.Jobs;

namespace Main.ECS.Provider.Hybrid
{
    // Unity.Transforms.HybridにあるCopy関連のSystemは
    // Translation/Rotationと言ったコンポーネントには反映されないので、
    // 自前で反映できるものを作成して運用.
    
    [UnityEngine.ExecuteAlways]
    [UpdateInGroup(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(EndFrameTRSToLocalToWorldSystem))]
    sealed class SyncTRSFromGameObjectSystem : JobComponentSystem
    {
        [BurstCompile]
        struct StashJob : IJobParallelForTransform
        {
            public NativeArray<Translation> Translations;
            public NativeArray<Rotation> Rotations;

            public void Execute(int index, TransformAccess transform)
            {
                Translations[index] = new Translation {Value = transform.localPosition};
                Rotations[index] = new Rotation {Value = transform.localRotation};
            }
        }

        [BurstCompile]
        struct CopyJob : IJobForEachWithEntity<SyncTRSFromGameObject, Translation, Rotation>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> Translations;
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Rotation> Rotations;

            public void Execute(
                [ReadOnly] Entity entity,
                [ReadOnly] int index,
                [ReadOnly] ref SyncTRSFromGameObject _,
                ref Translation translation,
                ref Rotation rotation)
            {
                translation.Value = Translations[index].Value;
                rotation.Value = Rotations[index].Value;
            }
        }

        EntityQuery _transformGroup;

        protected override void OnCreate()
        {
            _transformGroup = GetEntityQuery(
                ComponentType.ReadOnly<SyncTRSFromGameObject>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Rotation>(),
                typeof(UnityEngine.Transform));

            RequireForUpdate(_transformGroup);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = inputDeps;

            var transforms = _transformGroup.GetTransformAccessArray();
            var translations = new NativeArray<Translation>(transforms.length, Allocator.TempJob);
            var rotations = new NativeArray<Rotation>(transforms.length, Allocator.TempJob);

            handle = new StashJob
            {
                Translations = translations,
                Rotations = rotations,
            }.Schedule(transforms, handle);

            handle = new CopyJob
            {
                Translations = translations,
                Rotations = rotations,
            }.Schedule(this, handle);

            return handle;
        }
    }
}
