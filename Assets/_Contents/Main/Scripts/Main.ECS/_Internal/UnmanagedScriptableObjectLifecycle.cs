using UnityEngine;

namespace Main.ECS
{
    sealed class UnmanagedScriptableObjectLifecycle : MonoBehaviour
    {
        const string PrefabPath = "Prefabs/Systems/" + nameof(UnmanagedScriptableObjectLifecycle);

        [SerializeField] UnmanagedScriptableObjectBase _scriptableObject = default;

        [RuntimeInitializeOnLoadMethod]
        static void Bootstrap()
        {
            var prefab = Resources.Load<GameObject>(PrefabPath);
            var instance = Instantiate(prefab);
            instance.hideFlags = HideFlags.HideInHierarchy;
            var lifecycle = instance.GetComponent<UnmanagedScriptableObjectLifecycle>();
            lifecycle.Initialize();
        }

        void Initialize() => _scriptableObject.Initialize();

#if UNITY_EDITOR
        void Update() => _scriptableObject.CallUpdate();
#endif

        void OnDestroy() => _scriptableObject.Dispose();
    }
}
