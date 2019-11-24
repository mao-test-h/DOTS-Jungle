using UnityEngine;
using Main.ScriptableObjects;
using Main.Interfaces;

namespace Main
{
    sealed class Bootstrap : MonoBehaviour
    {
        [Header("【Settings】")]
        [SerializeField] GameSettings _gameSettings = default;
        [SerializeField] GameLoopType _startLoopType = default;

        [Header("【Audios】")]
        [SerializeField] GameObject _audioPlayerPrefab = default;

        GameLoop _gameLoop;
        IAudioPlayer _audioPlayer;

        void Start()
        {
#if !ENABLE_DEBUG
            Debug.unityLogger.logEnabled = false;
#endif
            _audioPlayer = AudioInitialize();
            _gameLoop = new GameLoop(_startLoopType, _gameSettings, _audioPlayer);
        }

        void Update() => _gameLoop?.CallUpdate();

        IAudioPlayer AudioInitialize()
        {
            IAudioPlayer ret = null;
#if USE_DUMMY
            ret = new DummyAudioPlayer();
#else
            var obj = Instantiate(_audioPlayerPrefab);
            ret = obj.GetComponent<IAudioPlayer>();
#endif
            ret.Initialize();
            return ret;
        }
    }
}
