using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using Main.Interfaces;
using Main.Utility;
using Main.View;

namespace Main.Presenter
{
    [RequireComponent(typeof(Canvas))]
    sealed class MainPresenter : MonoBehaviour, IPresenter
    {
        [Header("【View】")]
        [SerializeField] GlitchView _glitchView = default;

        GameLoop _gameLoop;
        public Canvas Canvas { get; private set; }

        public void Initialize(GameLoop gameLoop)
        {
            _gameLoop = gameLoop;
            Canvas = GetComponent<Canvas>();

            BindEvents();
            BindDebugEvents();
        }

        void BindEvents()
        {
            var data = _gameLoop.PlayerData;
            data.Life.Subscribe(life =>
            {
                var maxLife = _gameLoop.GameSettings.Player.MaxLife;
                var intensity = ((maxLife - life) / maxLife);
                _glitchView.DigitalNoiseIntensity = intensity;
            });
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        void BindDebugEvents()
        {
            // HACK: 適当実装.
            var obj = GameObject.Find("DebugLife");
            if (obj != null)
            {
                var debugText = obj.GetComponent<Text>();
                _gameLoop.PlayerData.Life.Subscribe(life => { debugText.text = "LIFE: " + life.ToString("0.00"); });
            }

            obj = GameObject.Find("DebugBananaCount");
            if (obj != null)
            {
                var debugText = obj.GetComponent<Text>();
                _gameLoop.PlayerData.BananaCount.Subscribe(count => { debugText.text = "SCORE: " + count; });
            }
        }
    }
}
