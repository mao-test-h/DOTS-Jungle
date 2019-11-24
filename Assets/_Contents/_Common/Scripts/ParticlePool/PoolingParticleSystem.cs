using UnityEngine;
using UnityEngine.Assertions;
using UniRx.Toolkit;

namespace ParticlePool
{
    public sealed class PoolingParticleSystem : ObjectPool<PoolableParticleElement>
    {
        readonly PoolableParticleElement _particle;
        readonly Transform _parentTrs;

        public PoolingParticleSystem(PoolableParticleElement particle)
        {
            _particle = particle;
            var parentObj = new GameObject {hideFlags = HideFlags.HideInHierarchy};
            _parentTrs = parentObj.transform;
            _parentTrs.position = Vector3.zero;
        }

        protected override PoolableParticleElement CreateInstance()
        {
            var instance = UnityEngine.Object.Instantiate(_particle, _parentTrs, true);
            var particle = instance.GetComponent<PoolableParticleElement>();
            Assert.IsTrue(particle != null);
            particle.Initialize(this);
            return particle;
        }
    }
}
