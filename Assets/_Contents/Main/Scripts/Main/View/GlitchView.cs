using UnityEngine;
using UnityEngine.Rendering.Universal.Glitch;
using LogUtility;

namespace Main.View
{
    sealed class GlitchView : MonoBehaviour
    {
        [SerializeField] DigitalGlitchFeature _digitalGlitchFeature = default;
        [SerializeField] AnalogGlitchFeature _analogGlitchFeature = default;

        public float DigitalNoiseIntensity
        {
            get => _digitalGlitchFeature.Intensity;
            set
            {
                CheckRange(value);
                _digitalGlitchFeature.Intensity = value;
            }
        }

        public float SetAnalogNoiseIntensity
        {
            set
            {
                CheckRange(value);
                ScanLineJitter = value;
                VerticalJump = value;
                HorizontalShake = value;
                ColorDrift = value;
            }
        }

        public float ScanLineJitter
        {
            get => _analogGlitchFeature.ScanLineJitter;
            set
            {
                CheckRange(value);
                _analogGlitchFeature.ScanLineJitter = value;
            }
        }

        public float VerticalJump
        {
            get => _analogGlitchFeature.VerticalJump;
            set
            {
                CheckRange(value);
                _analogGlitchFeature.VerticalJump = value;
            }
        }

        public float HorizontalShake
        {
            get => _analogGlitchFeature.HorizontalShake;
            set
            {
                CheckRange(value);
                _analogGlitchFeature.HorizontalShake = value;
            }
        }

        public float ColorDrift
        {
            get => _analogGlitchFeature.ColorDrift;
            set
            {
                CheckRange(value);
                _analogGlitchFeature.ColorDrift = value;
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG")]
        static void CheckRange(float val)
        {
            if (val > 1f || val < 0f)
            {
                LogHelper.LogWarning("Set between 0 and 1");
            }
        }

        void Awake()
        {
            DigitalNoiseIntensity = 0f;
            ScanLineJitter = 0f;
            VerticalJump = 0f;
            HorizontalShake = 0f;
            ColorDrift = 0f;
        }
    }
}
