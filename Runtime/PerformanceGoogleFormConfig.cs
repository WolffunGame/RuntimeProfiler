using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Wolffun.RuntimeProfiler
{
    [CreateAssetMenu(fileName = "PerformanceGoogleFormConfig", menuName = "RuntimeProfiler/Performance Google Form Config", order = 0)]
    public class PerformanceGoogleFormConfig : ScriptableObject
    {
        [Tooltip("Example: https://docs.google.com/forms/u/0/d/e/1FAIpQLSf5HeCzA5Tw_vOJX-Kz7Oti6ixo6amHUqPF9aqQECwLwVB43Q/formResponse")]
        public string googleFormUrl;

        [Header("Entry"), Tooltip("Example: entry.629291100")]
        public string appNameEntry;
        public string appVersionEntry;
        public string platformEntry;
        public string featureNameEntry;
        public string meanFrameTimeEntry;
        public string maxFrameTimeEntry;
        public string frameTimeExceededEntry; // percentage > time frame budget.
        public string meanDrawCallEntry;
        public string maxDrawCallEntry;
        public string reservedMemorySizeEntry;
        public string peakMemoryUsageEntry;
        public string deviceNameEntry;
        public string textureMemoryUsageEntry;
        public string meshMemoryUsageEntry;
        public string deviceStatsEntry;
        public string getFrameTimesEntry; //[GPU-CPU] bound times
        public string qualityLevelEntry;
        public string buildNumberEntry;
        public string medianFrameTimeEntry;
        public string leftQuartileFrameTimeEntry;
        public string rightQuartileFrameTimeEntry;
        public string medianDrawCallEntry;
        public string leftQuartileDrawCallEntry;
        public string rightQuartileDrawCallEntry;
        
        [NonSerialized] public string screenTimeEntry;

        public virtual WWWForm GetFormAddedField<T>(T stats) where T : PerformanceStats
        {
            var form = new WWWForm();
            AddField(form, stats);

            return form;
        }
        
        protected virtual WWWForm AddField<T>(WWWForm form, T stats) where T : PerformanceStats
        {
            form.AddField(deviceNameEntry, stats.DeviceName);
            form.AddField(deviceStatsEntry, stats.DeviceStats);
            form.AddField(appVersionEntry, stats.AppVersion);
            form.AddField(featureNameEntry, stats.FeatureName);
            form.AddField(meanFrameTimeEntry, stats.MeanFrameTime.ToString("F"));
            form.AddField(maxFrameTimeEntry, stats.MaxFrameTime.ToString("F"));
            form.AddField(frameTimeExceededEntry, stats.FrameTimeExceeded.ToString("F"));
            form.AddField(meanDrawCallEntry, stats.MeanDrawCall.ToString("F"));
            form.AddField(maxDrawCallEntry, stats.MaxDrawCall.ToString("F"));
            //form.AddField(screenTimeEntry, stats.ScreenTime.ToString("F"));
            form.AddField(reservedMemorySizeEntry, stats.ReservedMemorySize.ToString("F"));
            form.AddField(peakMemoryUsageEntry, stats.PeakMemoryUsage.ToString("F"));
            form.AddField(platformEntry, stats.Platform);
            form.AddField(appNameEntry, stats.AppName);
            form.AddField(getFrameTimesEntry, stats.GetFrameTimes());
            form.AddField(textureMemoryUsageEntry, stats.TextureMemoryUsage.ToString("F"));
            form.AddField(meshMemoryUsageEntry, stats.MeshMemryUsage.ToString("F"));
            form.AddField(qualityLevelEntry, stats.QualityLevel);
            form.AddField(buildNumberEntry, stats.BuildNumber);
            form.AddField(medianFrameTimeEntry, stats.MedianFrameTime.ToString("F"));
            form.AddField(leftQuartileFrameTimeEntry, stats.LeftQuartileFrameTime.ToString("F"));
            form.AddField(rightQuartileFrameTimeEntry, stats.RightQuartileFrameTime.ToString("F"));
            form.AddField(medianDrawCallEntry, stats.MedianDrawCall.ToString("F"));
            form.AddField(leftQuartileDrawCallEntry, stats.LeftQuartileDrawCall.ToString("F"));
            form.AddField(rightQuartileDrawCallEntry, stats.RightQuartileDrawCall.ToString("F"));

            return form;
        }
    }
}