#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Wolffun.RuntimeProfiler
{
    public class GoogleFormConfigEditor : Editor
    {
        [MenuItem("Tools/RuntimeProfiler/Init Google Form Config")]
        public static void Open()
        {
            InitGoogleFormConfigEditor();
        }
        
        
        static void InitGoogleFormConfigEditor()
        {
            string performanceDirectory = "Assets/Resources/" + SendToGoogleSheet.PERFORMANCE_CONFIG_PATH;
            string loadingTimeDirectory = "Assets/Resources/" + SendToGoogleSheet.LOADING_TIME_CONFIG_PATH;
            string runtimeProfilerDirectory ="Assets/Resources/RuntimeProfiler" ;

            if (!System.IO.Directory.Exists(runtimeProfilerDirectory))
            {
                System.IO.Directory.CreateDirectory(performanceDirectory);
            }

            PerformanceGoogleFormConfig config = Resources.Load<PerformanceGoogleFormConfig>(SendToGoogleSheet.PERFORMANCE_CONFIG_PATH);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<PerformanceGoogleFormConfig>();
                AssetDatabase.CreateAsset(config, performanceDirectory + ".asset");
                AssetDatabase.SaveAssets();
                Debug.Log("PerformanceGoogleFormConfig created at " + performanceDirectory);
            }

            LoadingTimeGoogleFormConfig config1 = Resources.Load<LoadingTimeGoogleFormConfig>(SendToGoogleSheet.LOADING_TIME_CONFIG_PATH);
            if (config1 == null)
            {
                config1 = ScriptableObject.CreateInstance<LoadingTimeGoogleFormConfig>();
                AssetDatabase.CreateAsset(config1, loadingTimeDirectory + ".asset");
                AssetDatabase.SaveAssets();
                Debug.Log("LoadingTimeGoogleFormConfig created at " + loadingTimeDirectory);
            }
        }
    }
}
#endif 