using Main.Interfaces;

namespace Main.Audio
{
    sealed class DummyAudioPlayer : IAudioPlayer
    {
        public void Initialize() { }
        public void PlayBGM() { }
        public void StopBGM() { }
        public void PlayStartDrumming() { }
        public void PlayDrumming() { }
        public void PlayGetBanana() { }
        public void PlayGameStart() { }
        public void PlayGameOver() { }
    }
}
