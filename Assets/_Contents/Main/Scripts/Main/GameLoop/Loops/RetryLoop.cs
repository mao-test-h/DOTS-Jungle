using UnityEngine.Assertions;
using Main.Interfaces;
using Main.Presenter;
using UniRx.Async;

namespace Main
{
    sealed class RetryLoop : IGameLoop
    {
        readonly GameLoop _gameLoop;
        PreparationPresenter _preparationPresenter;

        public RetryLoop(GameLoop gameLoop) => _gameLoop = gameLoop;

        public async UniTask LoopIn()
        {
            // Presenterの参照取得
            if (_preparationPresenter == null)
            {
                _preparationPresenter = this.GetPresenter<PreparationPresenter>();
                Assert.IsTrue(_preparationPresenter != null);
            }

            // プレイヤーと初期化と敵の配置の変更
            _gameLoop.BananaTreeGenerator.SetBananaTreesType();
            _gameLoop.PlayerActor.Reset();
            _gameLoop.PlayerData.Reset();

            // 開始演出
            _preparationPresenter.Canvas.enabled = true;
            await _preparationPresenter.ResultTransitionIn();
            await _preparationPresenter.TransitionOut();
            _preparationPresenter.Canvas.enabled = false;

            _gameLoop.SetLoop(GameLoopType.Main);
        }

        public void Looping() { }
        public void LoopOut() { }
    }
}
