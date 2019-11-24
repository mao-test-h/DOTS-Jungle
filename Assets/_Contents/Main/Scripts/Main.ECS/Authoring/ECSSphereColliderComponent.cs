using Unity.Mathematics;
using UnityEngine;

namespace Main.ECS
{
    public sealed class ECSSphereColliderComponent : MonoBehaviour, IConvertGameObjectToComponentData<ECSSphereCollider>
    {
        [Header("【Settings】")]
        [SerializeField] float _radius = default;
        [SerializeField] float3 _offset = default;

        [Header("【Gizmos】")]
        [SerializeField] bool _isShowGizmos = default;
        [SerializeField] Color _gizmosColor = Color.cyan;

        public ECSSphereCollider GetComponentData => new ECSSphereCollider(_radius, _offset);

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_isShowGizmos == false) return;
            Gizmos.color = _gizmosColor;
            var trs = transform;
            var position = math.mul(trs.rotation, _offset) + (float3) trs.position;
            Gizmos.DrawSphere(position, _radius);
        }
#endif
    }
}
