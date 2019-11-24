using System.Linq;
using UnityEngine;
using UniRx.Async;

namespace Main.Interfaces
{
    interface IGameLoop
    {
        UniTask LoopIn();
        void Looping();
        void LoopOut();
    }

    internal static class IGameLoopExtensions
    {
        public static T GetPresenter<T>(this IGameLoop gameLoop)
            where T : IPresenter
        {
            // HACK. 面倒なので全オブジェクト拾ってきてinterfaceを対象に初期化していく.
            return Object.FindObjectsOfType<GameObject>()
                .First(_ => _.GetComponent<T>() != null)
                .GetComponent<T>();
        }
    }
}
