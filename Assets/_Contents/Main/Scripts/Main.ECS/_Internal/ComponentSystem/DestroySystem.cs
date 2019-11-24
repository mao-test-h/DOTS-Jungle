using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Main.ECS
{
    [UpdateInGroup(typeof(UpdateGroup))]
    [UpdateAfter(typeof(BarrageSystem))]
    sealed class DestroySystem : JobComponentSystem
    {
        struct DestroyJob : IJobForEachWithEntity<Destroyable>
        {
            public EntityCommandBuffer.Concurrent CommandBuffer;
            public float DeltaTime;

            public void Execute(
                Entity entity,
                int index,
                ref Destroyable destroyable)
            {
                if (destroyable.Lifespan > 0)
                {
                    destroyable.Lifespan -= DeltaTime;
                    if (destroyable.Lifespan <= 0f)
                    {
                        destroyable.IsKilled = true;
                    }
                }

                if (destroyable.IsKilled)
                {
                    CommandBuffer.DestroyEntity(index, entity);
                }
            }
        }


        EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = inputDeps;

            handle = new DestroyJob()
            {
                CommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
                DeltaTime = Time.deltaTime,
            }.Schedule(this, handle);

            _entityCommandBufferSystem.AddJobHandleForProducer(handle);
            return handle;
        }
    }
}
