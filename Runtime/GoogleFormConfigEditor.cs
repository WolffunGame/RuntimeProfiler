#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
namespace Wolffun.RuntimeProfiler
{
    public class GoogleFormConfigEditor : Editor
    {
        private const string ResourcePath = "RuntimeProfiler/";
        private const string ConfigAssetName = "GoogleFormConfig";
        
        [InitializeOnLoadMethod]
        static void InitGoogleFormConfigEditor()
        {
            GoogleFormConfig config = Resources.Load<GoogleFormConfig>(ResourcePath + ConfigAssetName);

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<GoogleFormConfig>();
                AssetDatabase.CreateAsset(config, "Assets/Resources/" + ResourcePath + ConfigAssetName);
                AssetDatabase.SaveAssets();
                Debug.Log("GoogleFormConfig created at Assets/Resources/" + ResourcePath + ConfigAssetName);
            }
            else
            {
                Debug.Log("GoogleFormConfig already exists at Assets/Resources/" + ResourcePath + ConfigAssetName);
            }
        }
    }
}

#endif