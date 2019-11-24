using System.Collections;
using System.Text;
using UnityEngine;
using TMPro;
using Main.Interfaces;
using Main.View;

namespace Main.Presenter
{
    [RequireComponent(typeof(Canvas))]
    sealed class TitlePresenter : MonoBehaviour, IPresenter
    {
        const float TransitionInTime = 1f;
        const string TitleStr = "ウホッ";

        [Header("【View】")]
        [SerializeField] GlitchView _glitchView = default;
        [SerializeField] TextMeshProUGUI _textTitle = default;

        public Canvas Canvas { get; private set; }

        public void Initialize(GameLoop gameLoop)
        {
            Canvas = GetComponent<Canvas>();

            // タイトルはランダム生成(ゴリラ語の文法に従ってプレフィックスの"ウ"のみ固定)
            {
                var builder = new StringBuilder(8);
                builder.Append(TitleStr[0]);
                for (int i = 0; i < 7; i++)
                {
                    var index = Random.Range(0, TitleStr.Length);
                    builder.Append(TitleStr[index]);
                }

                _textTitle.text = builder.ToString();
            }
        }

        public IEnumerator TransitionIn()
        {
            _glitchView.SetAnalogNoiseIntensity = 0f;

            var count = 0f;
            while (true)
            {
                if (count >= TransitionInTime) break;
                count += Time.deltaTime;
                _glitchView.SetAnalogNoiseIntensity = count / TransitionInTime;
                yield return null;
            }

            _glitchView.SetAnalogNoiseIntensity = 1f;
            yield break;
        }
    }
}
