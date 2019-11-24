using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UniRx;
using UniRx.Async;
using Main.Interfaces;
using Main.Presenter;
using Object = UnityEngine.Object;

namespace Main
{
    sealed class MainLoop : IGameLoop
    {
        readonly GameLoop _gameLoop;

        MainPresenter _mainPresenter;
        IUpdatable[] _updatables;
        IDisposable _lifeObserver;

        public MainLoop(GameLoop gameLoop)
        {
            _gameLoop = gameLoop;
        }

        public UniTask LoopIn()
        {
            // ゲームオーバーの監視
            _lifeObserver = _gameLoop.PlayerData.Life.Subscribe(life =>
            {
                if (life <= 0f)
                {
                    _gameLoop.SetLoop(GameLoopType.Result);
                }
            });

            // 初回プレイ時には初期化を行う
            if (_mainPresenter == null)
            {
                // HACK. 面倒なので全オブジェクト拾ってきてinterfaceを対象に初期化していく.
                var objects = Object.FindObjectsOfType<GameObject>();

                // Presenterの参照取得
                _mainPresenter = this.GetPresenter<MainPresenter>();
                Assert.IsTrue(_mainPresenter != null);

                // 更新が必要なインスタンスの収集
                _updatables = objects
                    .Select(_ => _.GetComponent<IUpdatable>())
                    .Where(_ => _ != null)
                    .ToArray();
            }

            _mainPresenter.Canvas.enabled = true;
            return UniTask.CompletedTask;
        }

        public void Looping()
        {
            if (_updatables == null || _updatables.Length <= 0) return;
            foreach (var updatable in _updatables)
            {
                updatable.CallUpdate();
            }
        }

        public void LoopOut()
        {
            _mainPresenter.Canvas.enabled = false;
            _lifeObserver.Dispose();
            _lifeObserver = null;
        }
    }
}
