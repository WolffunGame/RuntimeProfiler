using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Wolffun.RuntimeProfiler
{
    [CreateAssetMenu(fileName = "GoogleFormConfig", menuName = "RuntimeProfiler/Google Form Config", order = 0)]
    public class GoogleFormConfig : ScriptableObject
    {
        [Tooltip("Example: https://docs.google.com/forms/u/0/d/e/1FAIpQLSf5HeCzA5Tw_vOJX-Kz7Oti6ixo6amHUqPF9aqQECwLwVB43Q/formResponse")]
        public string googleFormUrl;

        [Header("Entry"), Tooltip("Example: entry.629291100")]
        public string deviceNameEntry;

        public string deviceStatsEntry;
        public string appVersionEntry;
        public string featureNameEntry;
        public string meanFrameTimeEntry;
        public string maxFrameTimeEntry;
        public string frameTimeExceededEntry;
        public string meanDrawCallEntry;
        public string maxDrawCallEntry;
        public string screenTimeEntry;
        public string reservedMemorySizeEntry;
        public string peakMemoryUsageEntry;
        public string platformEntry;
        public string appNameEntry;
        public string getFrameTimesEntry;
        public string textureMemoryUsageEntry;
        public string meshMemryUsageEntry;
        public string qualityLevelEntry;
        public string buildNumberEntry;
        public string medianFrameTimeEntry;
        public string leftQuartileFrameTimeEntry;
        public string rightQuartileFrameTimeEntry;
        public string medianDrawCallEntry;
        public string leftQuartileDrawCallEntry;
        public string rightQuartileDrawCallEntry;
    }
}