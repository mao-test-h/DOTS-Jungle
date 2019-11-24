using System.Collections;
using UnityEngine;
using TMPro;
using Main.Interfaces;
using Main.Utility;

namespace Main.Presenter
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    sealed class ResultPresenter : MonoBehaviour, IPresenter
    {
        const float FadeInTime = 1.5f;

        [Header("【View】")]
        [SerializeField] TextMeshProUGUI _textBananaCount = default;

        GameLoop _gameLoop;
        CanvasGroup _canvasGroup;
        public Canvas Canvas { get; private set; }

        public void Initialize(GameLoop gameLoop)
        {
            _gameLoop = gameLoop;
            Canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetTotalBanana()
            => _textBananaCount.text = TranslateLanguage.NumberToGorilla(_gameLoop.PlayerData.BananaCount.Value);

        public IEnumerator FadeIn()
        {
            _canvasGroup.alpha = 0f;

            var count = 0f;
            while (true)
            {
                if (count >= FadeInTime) break;
                count += Time.deltaTime;
                _canvasGroup.alpha = (count / FadeInTime);
                yield return null;
            }

            _canvasGroup.alpha = 1f;
            yield break;
        }
    }
}
