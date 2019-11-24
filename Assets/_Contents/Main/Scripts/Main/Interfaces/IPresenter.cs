using UnityEngine;
using Main.Model;
using Main.ScriptableObjects;

namespace Main.Interfaces
{
    internal interface IPresenter
    {
        Canvas Canvas { get; }
        void Initialize(GameLoop gameLoop);
    }
}
