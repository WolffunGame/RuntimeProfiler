using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Wolffun.RuntimeProfiler
{
    public class LoadingTimeTracker : MonoBehaviour
    {
        public bool dontDestroyOnLoad;

        #region MonoBehaviour

        private void Start()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }

        private async void OnDestroy()
        {
            foreach (var (key, value) in LoadingTimeTracker.LoadingTimes)
            {
                var stat = new LoadingTimeStats();
                stat.LoadingTime = key;
                if (value.Count > 0)
                {
                    stat.FirstTime = value[0];
                    stat.Mean = value.Average();
                    await SendToGoogleSheet.Send(stat);
                }
            }
        }

        #endregion

        #region Static

        private static readonly Dictionary<string, Stopwatch> Stopwatches = new Dictionary<string, Stopwatch>();
        public static readonly Dictionary<string, List<long>> LoadingTimes = new Dictionary<string, List<long>>();

        public static bool Start(string key)
        {
#if RUNTIME_PROFILER
            if (!Stopwatches.ContainsKey(key))
            {
                var stopwatch = new Stopwatch();
                Stopwatches.Add(key, stopwatch);
                stopwatch.Start();
                return true;
            }

            if (Stopwatches[key].IsRunning)
            {
                Stopwatches[key].Restart();
                return true;
            }

#endif
            return false;
        }

        public static bool Stop(string key)
        {
#if RUNTIME_PROFILER
            if (Stopwatches.TryGetValue(key, out var stopwatch))
            {
                if (!Stopwatches[key].IsRunning) return false;
                stopwatch.Stop();

                AddSample(key, Stopwatches[key].ElapsedMilliseconds);
                return true;
            }
#endif
            return false;
        }

        private static void AddSample(string key, long value)
        {
            if (LoadingTimes.TryGetValue(key, out var data))
            {
                data.Add(value);
            }
            else
            {
                var newData = new List<long>();
                newData.Add(value);
                LoadingTimes.Add(key, newData);
            }
        }

        #endregion
    }
}