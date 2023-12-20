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
            var config = Resources.Load<PerformanceGoogleFormConfig>(PERFORMANCE_CONFIG_PATH);
            if (config == null) throw new Exception($"Runtime Profiler is not initialize yet,To init: Tools/RuntimeProfiler/Init Google Form Config");

            var form = config.GetFormAddedField(stats);
            
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