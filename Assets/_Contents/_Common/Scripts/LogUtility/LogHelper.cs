namespace LogUtility
{
    public static class LogHelper
    {
        [System.Diagnostics.Conditional("SHOW_LOG")]
        public static void Log(string message)
            => UnityEngine.Debug.Log($"    >>> <color=cyan>{message}</color>");

        [System.Diagnostics.Conditional("SHOW_LOG")]
        public static void LogWarning(string message)
            => UnityEngine.Debug.LogWarning($"    >>> <color=yellow>{message}</color>");

        [System.Diagnostics.Conditional("SHOW_LOG")]
        public static void LogError(string message)
            => UnityEngine.Debug.LogError($"    >>> <color=red>{message}</color>");
    }
}
