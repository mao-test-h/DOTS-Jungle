using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using MathematicsExtensions;
using Main.Defines;
using Random = UnityEngine.Random;

namespace Main.ECS.Provider
{
    [RequireComponent(typeof(PrefabProvider))]
    public sealed class ECSProvider
    {
        enum BulletIndex
        {
            Drumming = 0,
            Damage = 2,
            Banana = 3,
        }

        const string PrefabPath = "Prefabs/Systems/PrefabProvider";
        const int FruitfulBananaIndex = 1;

        readonly EntityManager _entityManager;
        readonly PrefabProvider _prefabProvider;


        #region Public Methods

        public ECSProvider()
        {
            // Create PrefabProvider.
            {
                var prefab = Resources.Load<GameObject>(PrefabPath);
                var instance = UnityEngine.Object.Instantiate(prefab);
                _prefabProvider = instance.GetComponent<PrefabProvider>();
                Assert.IsTrue(_prefabProvider != null);
                _prefabProvider.hideFlags = HideFlags.HideInHierarchy;
            }

            _entityManager = World.Active.EntityManager;
        }

        public void CreateDrummingBarrage(in float3 position, in quaternion rotation)
        {
            var prefabEntity = _prefabProvider.PrefabEntities[(int) BulletIndex.Drumming];
            var config = _entityManager.GetComponentData<BarrageConfig>(prefabEntity);
            var barrageParam = config.Config.Drumming;

            var count = barrageParam.BurstCount;
            for (int i = 0; i < count; i++)
            {
                var entity = Instantiate(prefabEntity, position, rotation);
                _entityManager.SetComponentData(entity, new OriginalPosition {Value = position});
                _entityManager.SetComponentData(entity, new SphericalCoordinates
                {
                    Value = new MathematicsExtensions.SphericalCoordinates()
                    {
                        Radius = barrageParam.DefaultRadius,
                        Theta = math.radians((i / (float) count) * 360f),
                    },
                });
                _entityManager.SetComponentData(entity, new Destroyable()
                {
                    IsKilled = false,
                    Lifespan = barrageParam.Lifespan,
                });
            }
        }

        public void CreateAimingBullet(
            in float3 position, in quaternion rotation, in float3 targetPosition,
            bool isBanana = false)
        {
            var prefabEntity = _prefabProvider.PrefabEntities[(int) (isBanana ? BulletIndex.Banana : BulletIndex.Damage)];
            var barrageParam = _entityManager.GetComponentData<BarrageConfig>(prefabEntity).Config.Aiming;

            var count = barrageParam.BurstCount;
            for (int i = 0; i < count; i++)
            {
                var radius = Random.Range(-barrageParam.ConeRadius, barrageParam.ConeRadius);
                var angle = Random.Range(-barrageParam.ConeAngle, barrageParam.ConeAngle);

                // 円錐状にランダムに散らばせる.
                var dir = targetPosition - position;
                dir = MathEx.CalcConeVector(dir, radius, math.radians(angle));
                var rotResult = quaternion.LookRotation(dir, Vector3.up);
                rotResult = math.mul(rotResult, Constants.HalfTurnRotateY);

                var entity = Instantiate(prefabEntity, position, rotResult);
                _entityManager.SetComponentData(entity, new Angle {Value = dir});
                _entityManager.SetComponentData(entity, new Destroyable
                {
                    IsKilled = false,
                    Lifespan = barrageParam.Lifespan,
                });
                _entityManager.AddComponentData(entity, new AimingTag());
            }
        }

        public void CreateCircleBullet(in float3 position, bool isBanana)
        {
            var prefabEntity = _prefabProvider.PrefabEntities[(int) (isBanana ? BulletIndex.Banana : BulletIndex.Damage)];
            var barrageParam = _entityManager.GetComponentData<BarrageConfig>(prefabEntity).Config.Circle;

            var sinTime = math.sin(Time.time * barrageParam.PitchSpeed);
            var pitch = MathEx.Map(sinTime, 1, -1, barrageParam.PitchRange.x, barrageParam.PitchRange.y);

            var count = barrageParam.BurstCount;
            for (int i = 0; i < count; i++)
            {
                // 射出方向のベクトル
                var theta = math.radians((i / (float) count) * 360f);
                var target = MathEx.SphericalCoordToPos(10f, theta, 0);
                target = new float3(target.x + position.x, pitch, target.z + position.z);

                // 射出方向の角度
                var dir = math.normalize(target - position);
                var rotResult = quaternion.LookRotation(dir, Vector3.up);
                rotResult = math.mul(rotResult, Constants.HalfTurnRotateY);

                var entity = Instantiate(prefabEntity, position, rotResult);
                _entityManager.SetComponentData(entity, new Angle {Value = dir});
                _entityManager.SetComponentData(entity, new Destroyable
                {
                    IsKilled = false,
                    Lifespan = barrageParam.Lifespan,
                });
                _entityManager.AddComponentData(entity, new CircleTag());
            }
        }

        public IDisposable CreateFruitfulBananas(
            in float3 position, in quaternion rotation,
            int createCount, float defaultRadius,
            Vector2 rotationSpeedRange, Vector2 thetaRange)
        {
            var prefabEntity = _prefabProvider.PrefabEntities[FruitfulBananaIndex];
            var arr = new NativeArray<Entity>(createCount, Allocator.Temp);
            for (int i = 0; i < createCount; i++)
            {
                var entity = Instantiate(prefabEntity, position, rotation);
                _entityManager.SetComponentData(entity, new OriginalPosition {Value = position});

                // 球面座標系の回転情報
                var thetaRand = Random.Range(thetaRange.x, thetaRange.y);
                const float MaxPhi = 6.28319f; // math.radians(360f)
                var phiRand = Random.Range(0f, MaxPhi);
                _entityManager.SetComponentData(entity, new SphericalCoordinates
                {
                    Value = new MathematicsExtensions.SphericalCoordinates()
                    {
                        Radius = defaultRadius,
                        Theta = thetaRand,
                        Phi = phiRand,
                    },
                });

                // 回転速度
                _entityManager.SetComponentData(entity, new Speed
                {
                    Value = Random.Range(rotationSpeedRange.x, rotationSpeedRange.y),
                });

                arr[i] = entity;
            }

            var disposer = new EntityDisposer(_entityManager, arr);
            arr.Dispose();
            return disposer;
        }

        #endregion

        #region Private Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Entity Instantiate(in Entity prefabEntity, in float3 position, in quaternion rotation)
        {
            var entity = _entityManager.Instantiate(prefabEntity);
            _entityManager.SetComponentData(entity, new Translation {Value = position});
            _entityManager.SetComponentData(entity, new Rotation {Value = rotation});
            return entity;
        }

        #endregion
    }
}
