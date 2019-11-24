using UnityEngine;
using Main.Model;
using Main.ScriptableObjects;

namespace Main.Interfaces
{
    internal interface IPlayerActor
    {
        GameObject GameObject { get; }

        void Initialize(PlayerData data, GameSettings settings, IAudioPlayer audioPlayer);
        void Reset();

        // Control
        void Move(in float horizontalAxis, in float verticalAxis);
        void Idle();
        void StartDrumming();
        void StopDrumming();
    }
}
