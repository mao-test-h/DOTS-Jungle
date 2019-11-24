namespace Main.Interfaces
{
    interface IAudioPlayer
    {
        void Initialize();

        void PlayBGM();
        void StopBGM();

        void PlayDrumming();
        void PlayGetBanana();

        void PlayGameStart();
        void PlayGameOver();
    }
}
