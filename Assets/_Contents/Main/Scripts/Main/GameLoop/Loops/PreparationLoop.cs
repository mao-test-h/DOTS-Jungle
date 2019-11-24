using UnityEngine.Assertions;
using UniRx.Async;
using Main.Interfaces;
using Main.Presenter;

namespace Main
{
    sealed class PreparationLoop : IGameLoop
    {
        readonly GameLoop _gameLoop;
        PreparationPresenter _preparationPresenter;

        public PreparationLoop(GameLoop gameLoop) => _gameLoop = gameLoop;

        public async UniTask LoopIn()
        {
            // 敵の抽選
            _gameLoop.BananaTreeGenerator.SetBananaTreesType();

            // Presenterの参照取得
            if (_preparationPresenter == null)
            {
                _preparationPresenter = this.GetPresenter<PreparationPresenter>();
                Assert.IsTrue(_preparationPresenter != null);
            }

            // BGM再生
            //_gameLoop.AudioPlayer.PlayBGM();

            // 開始演出
            _preparationPresenter.Canvas.enabled = true;
            await _preparationPresenter.TransitionOut();
            _preparationPresenter.Canvas.enabled = false;

            _gameLoop.SetLoop(GameLoopType.Main);
        }

        public void Looping() { }
        public void LoopOut() { }
    }
}
