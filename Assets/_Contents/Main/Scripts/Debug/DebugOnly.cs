using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugUtility
{
#if ENABLE_DEBUG
    sealed class DebugOnly : MonoBehaviour { }
#else
    sealed class DebugOnly : MonoBehaviour
    {
        void Awake() => Destroy(this.gameObject);
    }
#endif
}
