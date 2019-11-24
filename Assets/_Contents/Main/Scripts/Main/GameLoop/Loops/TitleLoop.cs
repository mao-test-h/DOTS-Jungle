using System;
using UnityEngine.Assertions;
using UniRx.Async;
using Main.Interfaces;
using Main.Presenter;

namespace Main
{
    sealed class TitleLoop : IGameLoop
    {
        readonly GameLoop _gameLoop;
        TitlePresenter _titlePresenter;

        public TitleLoop(GameLoop gameLoop) => _gameLoop = gameLoop;

        public async UniTask LoopIn()
        {
            // Presenterの参照取得
            if (_titlePresenter == null)
            {
                _titlePresenter = this.GetPresenter<TitlePresenter>();
                Assert.IsTrue(_titlePresenter != null);
            }

            // ゴリラ語でUI実装するの面倒なので自動遷移させる
            _titlePresenter.Canvas.enabled = true;
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            await _titlePresenter.TransitionIn();
            _titlePresenter.Canvas.enabled = false;

            _gameLoop.SetLoop(GameLoopType.Preparation);
        }

        public void Looping() { }
        public void LoopOut() { }
    }
}
