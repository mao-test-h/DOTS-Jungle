using UnityEngine;
using UnityEngine.Assertions;
using UniRx.Async;
using Main.Interfaces;
using Main.Presenter;

namespace Main
{
    sealed class ResultLoop : IGameLoop
    {
        const string Submit = "Submit";

        readonly GameLoop _gameLoop;
        ResultPresenter _resultPresenter;

        public ResultLoop(GameLoop gameLoop) => _gameLoop = gameLoop;

        public async UniTask LoopIn()
        {
            // Presenterの参照取得
            if (_resultPresenter == null)
            {
                _resultPresenter = this.GetPresenter<ResultPresenter>();
                Assert.IsTrue(_resultPresenter != null);
            }

            _resultPresenter.SetTotalBanana();
            _resultPresenter.Canvas.enabled = true;
            await _resultPresenter.FadeIn();
        }

        public void Looping() => UpdateController();

        public void LoopOut() => _resultPresenter.Canvas.enabled = false;

        void UpdateController()
        {
            // リトライ
            if (Input.GetButtonDown(Submit))
            {
                _gameLoop.SetLoop(GameLoopType.Retry);
                return;
            }
        }
    }
}
