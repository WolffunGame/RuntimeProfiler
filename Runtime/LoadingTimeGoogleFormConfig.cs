using UnityEngine;

namespace Wolffun.RuntimeProfiler
{
    [CreateAssetMenu(fileName = "LoadingTimeGoogleFormConfig", menuName = "RuntimeProfiler/Performance Google Form Config", order = 0)]
    public class LoadingTimeGoogleFormConfig : ScriptableObject
    {
        [Tooltip("Example: https://docs.google.com/forms/u/0/d/e/1FAIpQLSf5HeCzA5Tw_vOJX-Kz7Oti6ixo6amHUqPF9aqQECwLwVB43Q/formResponse")]
        public string googleFormUrl;
        
        [Header("Entry"), Tooltip("Example: entry.629291100")]
        public string appNameEntry;
        public string appVersionEntry;
        public string platformEntry;
        public string loadingTimeEntry;
        public string firstTimeEntry;
        public string meanEntry;
        public string deviceStatsEntry;
        
        
    }
}