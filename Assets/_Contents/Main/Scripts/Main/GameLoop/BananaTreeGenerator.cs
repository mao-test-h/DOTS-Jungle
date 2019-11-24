using UnityEngine;
using UnityEngine.Assertions;
using Main.Actor;
using Main.Enums;
using Main.Interfaces;
using Main.ScriptableObjects;

namespace Main
{
    sealed class BananaTreeGenerator
    {
        readonly Transform _target;
        readonly GameSettings _settings;
        readonly IAudioPlayer _audioPlayer;
        readonly BananaTreeLogic[] _createdInstances;

        public BananaTreeLogic[] CreatedBananaTrees => _createdInstances;

        public BananaTreeGenerator(Transform target, GameSettings settings, IAudioPlayer audioPlayer)
        {
            _target = target;
            _settings = settings;
            _audioPlayer = audioPlayer;
            _createdInstances = new BananaTreeLogic[settings.Actor.SpawnPoints.Length];

            GenerateBananaTrees();
        }

        public void SetBananaTreesType()
        {
            foreach (var bananaTree in _createdInstances)
            {
                // HACK: 適当
                var barrageType = Random.value > 0.1f ? Barrage.Aiming : Barrage.Circle;
                bananaTree.SetBarrageType(barrageType);
            }
        }

        void GenerateBananaTrees()
        {
            var points = _settings.Actor.SpawnPoints;
            for (int i = 0; i < _createdInstances.Length; i++)
            {
                var position = points[i];
                var instance = Object.Instantiate(_settings.Actor.BananaTreePrefab, position, Quaternion.identity);
                var bananaTree = instance.GetComponent<BananaTreeLogic>();
                Assert.IsTrue(bananaTree != null);
                bananaTree.Initialize(_target, _settings, _audioPlayer);
                _createdInstances[i] = bananaTree;
            }

#if UNITY_EDITOR
            // テスト用に直配置しているObjectの初期化
            var objs = Object.FindObjectsOfType<GameObject>();
            foreach (var obj in objs)
            {
                foreach (var bananaTreeLogic in obj.GetComponents<BananaTreeLogic>())
                {
                    bananaTreeLogic.Initialize(_target, _settings, _audioPlayer);
                }

            }
#endif
        }
    }
}
