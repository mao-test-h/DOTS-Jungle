using System.Linq;
using UnityEngine;
using Main.ScriptableObjects;

namespace Main.Tools
{
    sealed class SetSpawnPoints : MonoBehaviour
    {
        [SerializeField] GameSettings _gameSettings = default;
        [SerializeField] Transform[] _spawnPoints = default;

        public GameSettings GameSettings
        {
            get => _gameSettings;
            set => _gameSettings = value;
        }

        public Transform[] SpawnPoints
        {
            get => _spawnPoints;
            set => _spawnPoints = value;
        }
    }
}

#if UNITY_EDITOR
namespace Main.Tools.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(SetSpawnPoints))]
    sealed class SpawnPointSetterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var target = (SetSpawnPoints) base.target;
            if (GUILayout.Button("Set SpawnPoints"))
            {
                var points = target.SpawnPoints.Select(_ => _.transform.position).ToArray();
                target.GameSettings.Actor.SetSpawnPoints(points);
                EditorUtility.SetDirty(target);
                EditorUtility.SetDirty(target.GameSettings);
            }
        }
    }
}
#endif
