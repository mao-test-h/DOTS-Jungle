using System;
using UnityEngine;
using UniRx.Async;
using UnityEngine.Assertions;

namespace ParticlePool
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class PoolableParticleElement : MonoBehaviour
    {
        ParticleSystem _particleSystem;
        PoolingParticleSystem _poolSystem;

        void Awake() => _particleSystem = GetComponent<ParticleSystem>();

        public void Initialize(PoolingParticleSystem poolSystem) => _poolSystem = poolSystem;

        public async void Play(Vector3 position)
        {
            // 親パーティクルのStartLifetimeを見て停止.
            Assert.IsTrue(_poolSystem != null);
            transform.localPosition = position;
            _particleSystem.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(_particleSystem.main.startLifetimeMultiplier));
            _particleSystem.Stop();
            _poolSystem.Return(this);
        }
    }
}
