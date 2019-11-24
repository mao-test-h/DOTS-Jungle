using System;
using UnityEngine;

namespace Main.ECS
{
    public abstract class UnmanagedScriptableObjectBase : ScriptableObject, IDisposable
    {
        public abstract void Initialize();
        public abstract void CallUpdate();
        public abstract void Dispose();
    }
}
