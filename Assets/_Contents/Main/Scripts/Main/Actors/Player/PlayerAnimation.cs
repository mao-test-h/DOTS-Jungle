using System;
using UnityEngine;
using UnityEngine.Events;

namespace Main.Actor
{
    // AnimationClip Event Function受信の為にMonoBehaviourを継承.
    [RequireComponent(typeof(Animator))]
    sealed class PlayerAnimation : MonoBehaviour
    {
        enum Params
        {
            IsRunning = 0,
            IsDrumming,
        }

        enum MovableState
        {
            Idle = 0,
            Run,
        }

        Animator _animator;
        int[] _paramsTbl;
        int[] _movableStateTbl;

        public event UnityAction OnStartDrummingEvents;
        public event UnityAction OnDrummingEvents;

        public bool IsMovable
            => _animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(
                   _movableStateTbl[(int) MovableState.Idle])
               || _animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(
                   _movableStateTbl[(int) MovableState.Run]);


        public void Initialize()
        {
            _animator = GetComponent<Animator>();

            var length = Enum.GetNames(typeof(Params)).Length;
            _paramsTbl = new int[length];
            for (int i = 0; i < length; i++)
            {
                var val = (Params) i;
                _paramsTbl[i] = Animator.StringToHash(val.ToString());
            }

            length = Enum.GetNames(typeof(MovableState)).Length;
            _movableStateTbl = new int[length];
            for (int i = 0; i < length; i++)
            {
                var val = (MovableState) i;
                _movableStateTbl[i] = Animator.StringToHash(val.ToString());
            }
        }

        public void Running()
            => _animator.SetBool(_paramsTbl[(int) Params.IsRunning], true);

        public void Idle()
            => _animator.SetBool(_paramsTbl[(int) Params.IsRunning], false);

        public void StartDrumming()
            => _animator.SetBool(_paramsTbl[(int) Params.IsDrumming], true);

        public void StopDrumming()
            => _animator.SetBool(_paramsTbl[(int) Params.IsDrumming], false);

        // Call AnimationClip Event.
        void OnStartDrumming() => OnStartDrummingEvents?.Invoke();
        void OnDrumming() => OnDrummingEvents?.Invoke();
    }
}
