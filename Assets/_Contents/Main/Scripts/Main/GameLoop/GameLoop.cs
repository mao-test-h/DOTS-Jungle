using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Main.Interfaces;
using Main.Model;
using Main.ScriptableObjects;
using Main.ECS.Provider;

namespace Main
{
    internal enum GameLoopType
    {
        Title = 0,
        Preparation,
        Main,
        Result,
        Retry,
    }

    sealed class GameLoop
    {
        const string Cancel = "Cancel";

        readonly PlayerData _playerData;
        readonly GameSettings _gameSettings;
        readonly IAudioPlayer _audioPlayer;
        readonly ECSProvider _ecsProvider;

        readonly IGameLoop[] _gameLoops;

        BananaTreeGenerator _bananaTreeGenerator;
        IPlayerActor _playerActor;

        IGameLoop _currentLoop;
        bool _isUpdatable;

        public PlayerData PlayerData => _playerData;
        public GameSettings GameSettings => _gameSettings;
        public IAudioPlayer AudioPlayer => _audioPlayer;

        public IPlayerActor PlayerActor => _playerActor;
        public BananaTreeGenerator BananaTreeGenerator => _bananaTreeGenerator;


        public GameLoop(GameSettings settings, IAudioPlayer audioPlayer)
        {
            _gameSettings = settings;
            _audioPlayer = audioPlayer;
            _playerData = new PlayerData(_gameSettings.Player.MaxLife);
            _ecsProvider = new ECSProvider();
            
            _gameLoops = new IGameLoop[]
            {
                new TitleLoop(this),
                new PreparationLoop(this),
                new MainLoop(this),
                new ResultLoop(this),
                new RetryLoop(this),
            };

        }

        public void StartLoop(GameLoopType startType)
        {
            _playerActor = GetPlayerActor();
            _bananaTreeGenerator = new BananaTreeGenerator(_playerActor.GameObject.transform, _gameSettings, _audioPlayer);

            // HACK. 面倒なので全オブジェクト拾ってきてinterfaceを対象に初期化していく.
            var objs = Object.FindObjectsOfType<GameObject>();
            foreach (var obj in objs)
            {
                // Presenterを初期化.
                foreach (var presenter in obj.GetComponents<IPresenter>())
                {
                    presenter.Initialize(this);
                }
                
                // ECS利用箇所にPresenterを流し込む
                foreach (var usable in obj.GetComponents<IECSProviderUsable>())
                {
                    usable.SetProvider(_ecsProvider);
                }
            }

            SetLoop(startType); 
        }

        public void CallUpdate()
        {
            // 常時判定操作
            EveryController();

            if (_isUpdatable == false) return;
            _currentLoop.Looping();
        }

        public async void SetLoop(GameLoopType type)
        {
            _currentLoop?.LoopOut();

            _isUpdatable = false;
            _currentLoop = _gameLoops[(int) type];
            await _currentLoop.LoopIn();
            _isUpdatable = true;
        }

        void EveryController()
        {
            // ゲーム終了
            if (Input.GetButtonDown(Cancel))
            {
                Application.Quit();
                return;
            }
        }

        IPlayerActor GetPlayerActor()
        {
            // プレイヤーの取得
            // HACK. 面倒なので全オブジェクトから検索
            var actor = Object.FindObjectsOfType<GameObject>()
                .First(_ => _.GetComponent<IPlayerActor>() != null)
                .GetComponent<IPlayerActor>();
            Assert.IsTrue(actor != null);
            actor.Initialize(_playerData, _gameSettings, _audioPlayer);
            return actor;
        }
    }
}
