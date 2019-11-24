using System;
using UnityEngine;
using ParticlePool;

namespace Main.ScriptableObjects
{
    [CreateAssetMenu(fileName = nameof(GameSettings), menuName = "ScriptableObjects/" + nameof(GameSettings))]
    public sealed class GameSettings : ScriptableObject
    {
        [Serializable]
        public sealed class PlayerConfig
        {
            [SerializeField] float _moveSpeed = default;
            [SerializeField] float _maxLife = default;
            [SerializeField] float _damage = default;
            [SerializeField] float _autoRecoveryInterval = default;
            [SerializeField] float _autoRecoveryPoint = default;
            [SerializeField] Vector2 _movableRange = default;
            public float MoveSpeed => _moveSpeed;
            public float MaxLife => _maxLife;
            public float Damage => _damage;
            public float AutoRecoveryInterval => _autoRecoveryInterval;
            public float AutoRecoveryPoint => _autoRecoveryPoint;
            public Vector2 MovableRange => _movableRange;
        }

        [Serializable]
        public sealed class BananaTreeConfig
        {
            [Serializable]
            public sealed class Barrage
            {
                [SerializeField] float _shotSpan = default;
                [SerializeField, Range(0f, 1f)] float _bananaRate = default;

                public float ShotSpan => _shotSpan;
                public float BananaRate => _bananaRate;
            }

            [Serializable]
            public sealed class FruitfulBanana
            {
                [SerializeField] int _createCount = default;
                [SerializeField] float _defaultRadius = default;
                [SerializeField] Vector2 _rotationSpeedRange = default;
                [SerializeField] Vector2 _thetaRange = default;

                public int CreateCount => _createCount;
                public float DefaultRadius => _defaultRadius;
                public Vector2 RotationSpeedRange => _rotationSpeedRange;
                public Vector2 ThetaRange => _thetaRange;
            }

            [SerializeField] FruitfulBanana _fruitfulBanana = default;
            [SerializeField] Barrage _aimingConfig = default;
            [SerializeField] Barrage _circleConfig = default;
            public FruitfulBanana FruitfulBananaConfig => _fruitfulBanana;
            public Barrage AimingConfig => _aimingConfig;
            public Barrage CircleConfig => _circleConfig;
        }

        [Serializable]
        public sealed class ActorConfig
        {
            [SerializeField] GameObject _bananaTreePrefab = default;
            [SerializeField] Vector3[] _spawnPoints = default;
            public GameObject BananaTreePrefab => _bananaTreePrefab;
            public Vector3[] SpawnPoints => _spawnPoints;

#if UNITY_EDITOR
        public void SetSpawnPoints(Vector3[] points) => _spawnPoints = points;
#endif
        }

        [Serializable]
        public sealed class ParticlesConfig
        {
            [SerializeField] PoolableParticleElement _getBanana = default;
            public PoolableParticleElement GetBanana => _getBanana;
        }

        [SerializeField] PlayerConfig _playerConfig = default;
        [SerializeField] BananaTreeConfig _bananaTreeConfig = default;
        [SerializeField] ActorConfig _actorConfig = default;
        [SerializeField] ParticlesConfig _particlesConfig = default;

        public PlayerConfig Player => _playerConfig;
        public BananaTreeConfig BananaTree => _bananaTreeConfig;
        public ActorConfig Actor => _actorConfig;
        public ParticlesConfig Particles => _particlesConfig;
    }
}
