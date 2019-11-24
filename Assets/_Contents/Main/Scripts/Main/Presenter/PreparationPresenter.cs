using System.Collections;
using UnityEngine;
using TMPro;
using Main.Interfaces;
using Main.Utility;
using Main.View;

namespace Main.Presenter
{
    [RequireComponent(typeof(Canvas))]
    sealed class PreparationPresenter : MonoBehaviour, IPresenter
    {
        const float TransitionOutTime = 3f;
        const float ResultTransitionInTime = 1.5f;

        [Header("【View】")]
        [SerializeField] GlitchView _glitchView = default;
        [SerializeField] TextMeshProUGUI _textCountDown = default;

        public Canvas Canvas { get; private set; }

        public void Initialize(GameLoop gameLoop)
            => Canvas = GetComponent<Canvas>();

        public IEnumerator TransitionOut()
        {
            _glitchView.SetAnalogNoiseIntensity = 1f;

            var count = 0f;
            while (true)
            {
                if (count >= TransitionOutTime) break;
                count += Time.deltaTime;
                var num = Mathf.CeilToInt(TransitionOutTime - count);
                if(num <= 0) continue;
                _textCountDown.text = $"{TranslateLanguage.NumberToGorilla(num)}";
                _glitchView.SetAnalogNoiseIntensity = (1f - (count / TransitionOutTime));
                yield return null;
            }

            _glitchView.SetAnalogNoiseIntensity = 0f;
            yield break;
        }

        public IEnumerator ResultTransitionIn()
        {
            _glitchView.SetAnalogNoiseIntensity = 1f;
            _glitchView.DigitalNoiseIntensity = 1f;
            yield return new WaitForSeconds(0.2f);

            var count = 0f;
            while (true)
            {
                if (count >= ResultTransitionInTime) break;
                count += Time.deltaTime;
                _glitchView.DigitalNoiseIntensity = (1f - (count / ResultTransitionInTime));
                yield return null;
            }

            _glitchView.DigitalNoiseIntensity = 0f;
            yield break;
        }
    }
}
