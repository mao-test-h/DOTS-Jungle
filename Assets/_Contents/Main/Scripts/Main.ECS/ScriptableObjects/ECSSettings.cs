#pragma warning disable 0649
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Main.ECS
{
    [CreateAssetMenu(fileName = nameof(ECSSettings), menuName = "ScriptableObjects/ECS/" + nameof(ECSSettings))]
    public sealed class ECSSettings : UnmanagedScriptableObjectBase
    {
        public NativeObject<BarrageConfig> NativeBarrageConfig => _nativeBarrageConfig;

        [Serializable]
        public struct BarrageConfig
        {
            [Serializable]
            public struct DrummingParams
            {
                public int BurstCount;
                public int DefaultRadius;
                public float RotationSpeed;
                public float BulletSpeed;
                public float Lifespan;
            }

            [Serializable]
            public struct AimingParams
            {
                public int BurstCount;
                public float BulletSpeed;
                public float ConeRadius;
                public float ConeAngle;
                public float Lifespan;
            }

            [Serializable]
            public struct CircleParams
            {
                public int BurstCount;
                public float BulletSpeed;
                public float2 PitchRange;
                public float PitchSpeed;
                public float YawSpeed;
                public float Lifespan;
            }

            public DrummingParams Drumming;
            public AimingParams Aiming;
            public CircleParams Circle;
            public float KillLineY;
        }


        [SerializeField] BarrageConfig _barrageConfig = default;

        NativeObject<BarrageConfig> _nativeBarrageConfig;


        public override void Initialize()
        {
            _nativeBarrageConfig = new NativeObject<BarrageConfig>(Allocator.Persistent, _barrageConfig);
        }

        public override void CallUpdate()
        {
            _nativeBarrageConfig.Value = _barrageConfig;
        }

        public override void Dispose()
        {
            _nativeBarrageConfig.Dispose();
        }
    }
}
