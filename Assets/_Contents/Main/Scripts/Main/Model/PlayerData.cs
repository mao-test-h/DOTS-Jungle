using UniRx;

namespace Main.Model
{
    sealed class PlayerData
    {
        public readonly ReactiveProperty<int> BananaCount = new ReactiveProperty<int>();
        public readonly ReactiveProperty<float> Life = new ReactiveProperty<float>();
        readonly float _maxLife;

        public PlayerData(float maxLife)
        {
            _maxLife = maxLife;
            BananaCount = new ReactiveProperty<int>();
            Life = new ReactiveProperty<float>(maxLife);
        }

        public void Reset()
        {
            BananaCount.Value = 0;
            Life.Value = _maxLife;
        }
    }
}
