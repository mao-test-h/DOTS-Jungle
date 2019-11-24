using System;

namespace Main.ECS
{
    interface IUnmanagedScriptableObject : IDisposable
    {
        void Initialize();
        void CallUpdate();
    }
}
