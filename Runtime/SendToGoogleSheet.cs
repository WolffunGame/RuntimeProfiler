using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Wolffun.RuntimeProfiler
{
    public static class SendToGoogleSheet
    {
        public const string PERFORMANCE_CONFIG_PATH = "RuntimeProfiler/PerformanceGoogleFormConfig";
        public const string LOADING_TIME_CONFIG_PATH = "RuntimeProfiler/LoadingTimeGoogleFormConfig";
        private static async UniTask Post(PerformanceStats stats)
        {
            var config = Resources.Load<PerformanceGoogleFormConfig>(PERFORMANCE_CONFIG_PATH) as PerformanceGoogleFormConfig;
            if (config == null) throw new Exception($"Runtime Profiler is not initialize yet,To init: Tools/RuntimeProfiler/Init Google Form Config");

            var form = new WWWForm();
            form.AddField(config.deviceNameEntry, stats.DeviceName);
            form.AddField(config.deviceStatsEntry, stats.DeviceStats);
            form.AddField(config.appVersionEntry, stats.AppVersion);
            form.AddField(config.featureNameEntry, stats.FeatureName);
            form.AddField(config.meanFrameTimeEntry, stats.MeanFrameTime.ToString("F"));
            form.AddField(config.maxFrameTimeEntry, stats.MaxFrameTime.ToString("F"));
            form.AddField(config.frameTimeExceededEntry, stats.FrameTimeExceeded.ToString("F"));
            form.AddField(config.meanDrawCallEntry, stats.MeanDrawCall.ToString("F"));
            form.AddField(config.maxDrawCallEntry, stats.MaxDrawCall.ToString("F"));
            //form.AddField(config.screenTimeEntry, stats.ScreenTime.ToString("F"));
            form.AddField(config.reservedMemorySizeEntry, stats.ReservedMemorySize.ToString("F"));
            form.AddField(config.peakMemoryUsageEntry, stats.PeakMemoryUsage.ToString("F"));
            form.AddField(config.platformEntry, stats.Platform);
            form.AddField(config.appNameEntry, stats.AppName);
            form.AddField(config.getFrameTimesEntry, stats.GetFrameTimes());
            form.AddField(config.textureMemoryUsageEntry, stats.TextureMemoryUsage.ToString("F"));
            form.AddField(config.meshMemoryUsageEntry, stats.MeshMemryUsage.ToString("F"));
            form.AddField(config.qualityLevelEntry, stats.QualityLevel);
            form.AddField(config.buildNumberEntry, stats.BuildNumber);
            form.AddField(config.medianFrameTimeEntry, stats.MedianFrameTime.ToString("F"));
            form.AddField(config.leftQuartileFrameTimeEntry, stats.LeftQuartileFrameTime.ToString("F"));
            form.AddField(config.rightQuartileFrameTimeEntry, stats.RightQuartileFrameTime.ToString("F"));
            form.AddField(config.medianDrawCallEntry, stats.MedianDrawCall.ToString("F"));
            form.AddField(config.leftQuartileDrawCallEntry, stats.LeftQuartileDrawCall.ToString("F"));
            form.AddField(config.rightQuartileDrawCallEntry, stats.RightQuartileDrawCall.ToString("F"));

            var www = UnityWebRequest.Post(config.googleFormUrl, form);
            var rq = await www.SendWebRequest();

            if (rq.result == UnityWebRequest.Result.ConnectionError || rq.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(rq.error);
            else
                Debug.Log("Form upload complete!");
        }

        public static UniTask Send(PerformanceStats stats)
        {
            return Post(stats);
        }
        
        private static async UniTask Post(LoadingTimeStats stat)
        {
            var config = Resources.Load<LoadingTimeGoogleFormConfig>(LOADING_TIME_CONFIG_PATH) as LoadingTimeGoogleFormConfig;
            if (config == null) throw new Exception($"Runtime Profiler is not initialize yet, To init: Tools/RuntimeProfiler/Init Google Form Config");
            
            var form = new WWWForm();
            //Game
            form.AddField(config.appNameEntry, stat.AppName);
            //Game Version
            form.AddField(config.appVersionEntry, stat.AppVersion);
            //Platform
            form.AddField(config.platformEntry, stat.Platform);
            //Loading Time
            form.AddField(config.loadingTimeEntry, stat.LoadingTime);
            //First Time
            form.AddField(config.firstTimeEntry, stat.FirstTime.ToString("F"));
            //Mean
            form.AddField(config.meanEntry, stat.Mean.ToString("F"));
            //Device Stats
            form.AddField(config.deviceStatsEntry, stat.DeviceStats);

            var www = UnityWebRequest.Post(config.googleFormUrl, form);
            var rq = await www.SendWebRequest();

            if (rq.result == UnityWebRequest.Result.ConnectionError || rq.result == UnityWebRequest.Result.ProtocolError)
                UnityEngine.Debug.LogError(rq.error);
            else
                UnityEngine.Debug.Log("Loading Time Form upload complete!");
        }
        
        public static UniTask Send(LoadingTimeStats stats)
        {
            return Post(stats);
        }
    }
}