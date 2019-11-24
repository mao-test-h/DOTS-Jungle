using Unity.Mathematics;
using UnityEngine;
using Cinemachine;
using Main.Interfaces;
using Main.Model;
using Main.ScriptableObjects;
using Main.ECS.Provider;
using Main.ECS.Provider.Hybrid;
using ParticlePool;
using MathematicsExtensions;

namespace Main.Actor
{
    [RequireComponent(typeof(HitReceiverComponent))]
    sealed class PlayerLogic : MonoBehaviour, IPlayerActor, IUpdatable, IECSProviderUsable
    {
        [Header("【Components】")]
        [SerializeField] CinemachineImpulseSource _cinemachineImpulseSource = default;

        Transform _trs;
        HitReceiverComponent _hitReceiverComponent;

        PlayerData _playerData;
        GameSettings.PlayerConfig _playerConfig;
        IAudioPlayer _audioPlayer;

        PlayerController _playerController;
        PoolingParticleSystem _particlePool;

        float _autoRecoveryCounter;
        Rhombus2D _rhombus2D;

        public GameObject GameObject => gameObject;
        public ECSProvider Provider { get; private set; }
        bool IsInitialized => _playerData != null;

        bool IsGameOver => (_playerData.Life.Value <= 0f);

        public void Initialize(PlayerData data, GameSettings settings, IAudioPlayer audioPlayer)
        {
            _trs = transform;
            _hitReceiverComponent = GetComponent<HitReceiverComponent>();

            _playerData = data;
            _playerConfig = settings.Player;
            _audioPlayer = audioPlayer;

            // 移動可能領域の指定
            // ※Y軸の移動は無いので平面として定義
            var range = _playerConfig.MovableRange;
            _rhombus2D = new Rhombus2D
            {
                P1 = new float2(0f, range.y),
                P2 = new float2(-range.x, 0f),
                P3 = new float2(0f, -range.y),
                P4 = new float2(range.x, 0f),
            };

            // Initialization ParticleSystem.
            // HACK: 現状ここでしか使わないので一先ずは直に持たせる
            _particlePool = new PoolingParticleSystem(settings.Particles.GetBanana);

            // Initialization Controller.
            _playerController = new PlayerController(this);

            // Bind Events.
            _hitReceiverComponent.OnDamageHitEvent += OnDamageHit;
            _hitReceiverComponent.OnBananaHitEvent += OnBananaHit;
        }

        public void Reset()
        {
            _trs.localPosition = Vector3.zero;
            _trs.localRotation = Quaternion.identity;
        }

        public void SetProvider(in ECSProvider provider) => Provider = provider;

        public void CallUpdate()
        {
            if (IsInitialized == false) return;

            _playerController.CallUpdate();

            if (_playerData.Life.Value < _playerConfig.MaxLife)
            {
                if (_autoRecoveryCounter <= 0f)
                {
                    ApplyRecovery();
                    _autoRecoveryCounter = _playerConfig.AutoRecoveryInterval;
                }
                else
                {
                    _autoRecoveryCounter -= Time.deltaTime;
                }
            }
        }

        #region // Control

        public void Move(in float horizontalAxis, in float verticalAxis)
        {
            var deltaTime = Time.deltaTime;

            // 移動
            var vec = new float3(-horizontalAxis, 0f, -verticalAxis);
            var moveRet = _trs.localPosition + (Vector3) (vec * _playerConfig.MoveSpeed * deltaTime);
            if (_rhombus2D.Contains(moveRet.x, moveRet.z)) _trs.localPosition = moveRet;

            // 回転
            var rot = quaternion.LookRotation(vec, new float3(0f, 1f, 0f));
            _trs.localRotation = math.slerp(_trs.localRotation, rot, 8f * deltaTime); // 0.08f ・・・回転補完速度
        }

        public void Idle() { }
        public void StartDrumming()
        {
            _cinemachineImpulseSource.GenerateImpulse();
            _audioPlayer.PlayDrumming();
            Provider.CreateDrummingBarrage(_trs.position, _trs.rotation);
        }

        public void StopDrumming() { }

        #endregion  // Control

        #region // Event Method

        void OnDamageHit(float3 hitPosition) => ApplyDamage();

        void OnBananaHit(float3 hitPosition) => AddBanana();

        #endregion // Event Method

        #region // Unity Events

        void OnDestroy()
        {
            _particlePool?.Dispose();
        }

        #endregion // Unity Events

        void ApplyDamage()
        {
            var maxLife = _playerConfig.MaxLife;
            var damage = _playerConfig.Damage;

            var currentLife = _playerData.Life.Value;
            currentLife = Mathf.Clamp(currentLife - damage, 0f, maxLife);
            _playerData.Life.Value = currentLife;
            _autoRecoveryCounter = _playerConfig.AutoRecoveryInterval;

            if (IsGameOver)
            {
                StopDrumming();
            }
        }

        void ApplyRecovery()
        {
            var maxLife = _playerConfig.MaxLife;
            var recovery = _playerConfig.AutoRecoveryPoint;

            var currentLife = _playerData.Life.Value;
            currentLife = Mathf.Clamp(currentLife + recovery, 0f, maxLife);
            _playerData.Life.Value = currentLife;
        }

        void AddBanana()
        {
            if (IsGameOver) return;
            _audioPlayer.PlayGetBanana();
            var particle = _particlePool.Rent();
            particle.Play(_trs.localPosition);
            _playerData.BananaCount.Value += 1;
        }
    }
}
