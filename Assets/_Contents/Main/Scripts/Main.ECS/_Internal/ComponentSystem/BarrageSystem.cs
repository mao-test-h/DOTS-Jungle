using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using MathematicsExtensions;
using Unity.Mathematics;

namespace Main.ECS
{
    [UpdateInGroup(typeof(UpdateGroup))]
    sealed class BarrageSystem : JobComponentSystem
    {
        [BurstCompile]
        struct DrummingJob : IJobForEach<
            Translation,
            SphericalCoordinates,
            DrummingTag,
            OriginalPosition,
            BarrageConfig>
        {
            public float DeltaTime;

            public unsafe void Execute(
                ref Translation translation,
                ref SphericalCoordinates sphericalCoordinates,
                [ReadOnly] ref DrummingTag _,
                [ReadOnly] ref OriginalPosition origPosition,
                [ReadOnly] ref BarrageConfig barrageConfig)
            {
                var @params = barrageConfig.ConfigPtr->Drumming;
                var origPos = origPosition.Value;
                var radius = sphericalCoordinates.Value.Radius;
                var theta = sphericalCoordinates.Value.Theta;
                var phi = sphericalCoordinates.Value.Phi;

                // 回転の計算.
                radius += DeltaTime * @params.BulletSpeed;
                theta += DeltaTime * @params.RotationSpeed;
                phi = 0f;
                var pos = MathEx.SphericalCoordToPos(radius, theta, phi);

                // apply params.
                sphericalCoordinates.Value = new MathematicsExtensions.SphericalCoordinates()
                {
                    Radius = radius,
                    Theta = theta,
                    Phi = phi,
                };

                translation.Value = pos + origPos;
            }
        }

        [BurstCompile]
        struct AimingJob : IJobForEach<
            Translation,
            AimingTag,
            Angle,
            BarrageConfig>
        {
            public float DeltaTime;

            public unsafe void Execute(
                ref Translation translation,
                [ReadOnly] ref AimingTag _,
                [ReadOnly] ref Angle angle,
                [ReadOnly] ref BarrageConfig barrageConfig)
            {
                var @params = *barrageConfig.ConfigPtr;
                float3 add = DeltaTime * @params.Aiming.BulletSpeed * angle.Value;
                translation.Value += add;
            }
        }

        [BurstCompile]
        struct CircleJob : IJobForEach<
            Translation,
            CircleTag,
            Angle,
            BarrageConfig>
        {
            public float DeltaTime;

            public unsafe void Execute(
                ref Translation translation,
                [ReadOnly] ref CircleTag _,
                [ReadOnly] ref Angle angle,
                [ReadOnly] ref BarrageConfig barrageConfig)
            {
                var @params = barrageConfig.ConfigPtr->Circle;
                float3 add = DeltaTime * @params.BulletSpeed * angle.Value;
                translation.Value += add;
            }
        }

        [BurstCompile]
        struct CheckKillLineJob : IJobForEach<
            Destroyable,
            Translation,
            BarrageConfig>
        {
            public unsafe void Execute(
                ref Destroyable destroyable,
                [ReadOnly] ref Translation translation,
                [ReadOnly] ref BarrageConfig barrageConfig)
            {
                var @params = *barrageConfig.ConfigPtr;
                var pos = translation.Value;
                if (pos.y <= @params.KillLineY)
                {
                    destroyable.IsKilled = true;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = inputDeps;
            handle = new DrummingJob {DeltaTime = Time.DeltaTime}.Schedule(this, handle);
            handle = new AimingJob {DeltaTime = Time.DeltaTime}.Schedule(this, handle);
            handle = new CircleJob {DeltaTime = Time.DeltaTime}.Schedule(this, handle);
            handle = new CheckKillLineJob().Schedule(this, handle);
            return handle;
        }
    }
}
