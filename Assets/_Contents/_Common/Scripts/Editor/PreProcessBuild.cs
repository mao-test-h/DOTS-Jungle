// referenced:
//     https://arakade.itch.io/extra-connect-the-unity-dots/devlog/83621/inception-webgl-build

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Common.Editor
{
    sealed class MyEditorExtensions : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            const int MaxHeapSize = 512 * 1024 * 1024;
            PlayerSettings.WebGL.emscriptenArgs = $"-s TOTAL_MEMORY={MaxHeapSize} -s ALLOW_MEMORY_GROWTH=1";
            Debug.Log($"Set emscriptenArgs = {PlayerSettings.WebGL.emscriptenArgs}");
        }
    }
}
