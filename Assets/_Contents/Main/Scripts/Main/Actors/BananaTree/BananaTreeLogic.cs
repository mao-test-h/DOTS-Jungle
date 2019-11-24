using System;
using UnityEngine;
using Main.Interfaces;
using Main.ScriptableObjects;
using Main.ECS.Provider;
using Main.Enums;

namespace Main.Actor
{
    sealed class BananaTreeLogic : MonoBehaviour, IUpdatable, IECSProviderUsable
    {
        GameSettings.BananaTreeConfig _bananaTreeConfig;
        GameSettings.BananaTreeConfig.Barrage _barrageConfig;
        IAudioPlayer _audioPlayer;
        Transform _targetTrs;

        IDisposable _fruitfulBananasDisposer;
        Barrage _barrageType;
        float _time;

        public ECSProvider Provider { get; private set; }


        public void Initialize(Transform target, GameSettings settings, IAudioPlayer audioPlayer)
        {
            _targetTrs = target;
            _audioPlayer = audioPlayer;
            _bananaTreeConfig = settings.BananaTree;
        }

        public void SetBarrageType(in Barrage barrage)
        {
            _barrageType = barrage;
            switch (_barrageType)
            {
                case Barrage.Aiming:
                    _barrageConfig = _bananaTreeConfig.AimingConfig;
                    break;
                case Barrage.Circle:
                    _barrageConfig = _bananaTreeConfig.CircleConfig;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetProvider(in ECSProvider provider)
        {
            // ECSが利用可能なタイミングで必要となるEntityを生成.
            Provider = provider;
            _fruitfulBananasDisposer = CreateFruitfulBananas();
        }

        public void CallUpdate()
        {
            if (_targetTrs == null || _barrageConfig == null) return;

            // HACK: Updateで直にEntityを生成しているが、パフォーマンスが気になるなら生成機構をECSに回しても良いかも
            if (_time > _barrageConfig.ShotSpan)
            {
                var trs = transform;
                var isBanana = UnityEngine.Random.value <= _barrageConfig.BananaRate;

                switch (_barrageType)
                {
                    case Barrage.Aiming:
                        Provider.CreateAimingBullet(
                            trs.position, trs.rotation,
                            _targetTrs.position, isBanana);
                        break;
                    case Barrage.Circle:
                        Provider.CreateCircleBullet(
                            trs.position, isBanana);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _time = 0f;
                return;
            }

            _time += Time.deltaTime;
        }

        IDisposable CreateFruitfulBananas()
        {
            var trs = transform;
            var config = _bananaTreeConfig.FruitfulBananaConfig;
            return Provider.CreateFruitfulBananas(
                trs.localPosition, trs.localRotation,
                config.CreateCount, config.DefaultRadius,
                config.RotationSpeedRange, config.ThetaRange);
        }

        #region Unity Events

        void OnDestroy()
        {
            _fruitfulBananasDisposer?.Dispose();
        }

        #endregion
    }
}
