#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Wolffun.RuntimeProfiler
{
    public class GoogleFormConfigEditor : Editor
    {
        [InitializeOnLoadMethod]
        static void InitGoogleFormConfigEditor()
        {
            PerformanceGoogleFormConfig config = Resources.Load<PerformanceGoogleFormConfig>(SendToGoogleSheet.PERFORMANCE_CONFIG_PATH);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<PerformanceGoogleFormConfig>();
                AssetDatabase.CreateAsset(config, "Assets/Resources/" + SendToGoogleSheet.PERFORMANCE_CONFIG_PATH + ".asset");
                AssetDatabase.SaveAssets();
                Debug.Log("PerformanceGoogleFormConfig created at Assets/Resources/" + SendToGoogleSheet.PERFORMANCE_CONFIG_PATH);
            }

            LoadingTimeGoogleFormConfig config1 = Resources.Load<LoadingTimeGoogleFormConfig>(SendToGoogleSheet.LOADING_TIME_CONFIG_PATH);
            if (config1 == null)
            {
                config1 = ScriptableObject.CreateInstance<LoadingTimeGoogleFormConfig>();
                AssetDatabase.CreateAsset(config1, "Assets/Resources/" + SendToGoogleSheet.LOADING_TIME_CONFIG_PATH + ".asset");
                AssetDatabase.SaveAssets();
                Debug.Log("LoadingTimeGoogleFormConfig created at Assets/Resources/" + SendToGoogleSheet.LOADING_TIME_CONFIG_PATH);
            }
        }
    }
}

#endif