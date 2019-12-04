using Main.Defines;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using MathematicsExtensions;

namespace Main.ECS
{
    [UpdateInGroup(typeof(UpdateGroup))]
    sealed class FruitfulBananaSystem : JobComponentSystem
    {
        [BurstCompile]
        struct UpdateJob : IJobForEach<
            Translation,
            Rotation,
            SphericalCoordinates,
            Speed,
            OriginalPosition>
        {
            public float DeltaTime;
            public quaternion HalfTurnRotateY;

            public void Execute(
                ref Translation translation,
                ref Rotation rotation,
                ref SphericalCoordinates sphericalCoordinates,
                [ReadOnly] ref Speed speed,
                [ReadOnly] ref OriginalPosition origPosition)
            {
                var origPos = origPosition.Value;

                var radius = sphericalCoordinates.Value.Radius;
                var theta = sphericalCoordinates.Value.Theta;
                var phi = sphericalCoordinates.Value.Phi;

                // 回転の計算.
                theta += DeltaTime * speed.Value;
                var pos = MathEx.SphericalCoordToPos(radius, theta, phi);
                pos += origPos;

                // LookAt
                var dir = math.normalize(pos - origPos);
                var rotResult = quaternion.LookRotation(dir, Vector3.up);
                rotResult = math.mul(rotResult, HalfTurnRotateY);

                // apply params.
                sphericalCoordinates.Value = new MathematicsExtensions.SphericalCoordinates()
                {
                    Radius = radius,
                    Theta = theta,
                    Phi = phi,
                };

                rotation.Value = rotResult;
                translation.Value = pos;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
            => new UpdateJob
            {
                DeltaTime = Time.DeltaTime,
                HalfTurnRotateY = Constants.HalfTurnRotateY,
            }.Schedule(this, inputDeps);
    }
}
