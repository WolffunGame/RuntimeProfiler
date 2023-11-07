using System;
using System.Collections.Generic;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;

namespace Wolffun.RuntimeProfiler
{
    public class PerformanceTracker : MonoBehaviour
    {
        private ProfilerRecorder _systemMemoryRecorder;
        private ProfilerRecorder _gcMemoryRecorder;
        private ProfilerRecorder _mainThreadTimeRecorder;
        private ProfilerRecorder _drawCallsCountRecorder;
        private ProfilerRecorder _batchCallsCountRecorder;
        private ProfilerRecorder _meshMemoryRecorder;
        private ProfilerRecorder _textureMemoryRecorder;

        private ProfilerRecorder _renderThreadRecorder;

        //GPU time
        private ProfilerRecorder _gpuTimeRecorder;

        private static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Count;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }

        private static double GetRecorderGPUFrameTimeAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }

        private static double GetRecorderRenderThreadAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }

        //Get peak memory usage in Mb of a recoder
        private static long GetRecorderPeakMemoryUsage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            long r = 0;
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r = Math.Max(r, samples[i].Value);
            }

            return r / (1024 * 1024);
        }

        public bool dontDestroyOnLoad;
        private void Start()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        
        private void OnEnable()
        {
            _systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            _gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
            _mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            _drawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            _meshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Mesh Memory");
            _textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Texture Memory");

            //GPU time
            _gpuTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "GPU Frame Time");

            _renderThreadRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "CPU Main Thread Present Wait Time");
        }

        private void OnDisable()
        {
            _systemMemoryRecorder.Dispose();
            _gcMemoryRecorder.Dispose();
            _mainThreadTimeRecorder.Dispose();
            _drawCallsCountRecorder.Dispose();
            _meshMemoryRecorder.Dispose();
            _textureMemoryRecorder.Dispose();
            _gpuTimeRecorder.Dispose();
            _renderThreadRecorder.Dispose();
        }

        private void Update()
        {
            var drawCall = _drawCallsCountRecorder.LastValue;
            var frameTime = Math.Round(GetRecorderFrameAverage(_mainThreadTimeRecorder) * (1e-6f), 2);
            var gpuFrameTime = Math.Round(GetRecorderGPUFrameTimeAverage(_gpuTimeRecorder) * (1e-6f), 2);
            var renderThreadTime = Math.Round(GetRecorderRenderThreadAverage(_renderThreadRecorder) * (1e-6f), 2);
            var memoryInFrame = GetRecorderPeakMemoryUsage(_systemMemoryRecorder);
            var textureMemory = _textureMemoryRecorder.LastValue;
            var meshMemory = _meshMemoryRecorder.LastValue;

            foreach (var (key, value) in GameFeature)
            {
                if (value.Recording)
                {
                    value.AddFrameTime(frameTime);
                    value.AddGpuFrameTime(gpuFrameTime);
                    value.MeanFrameTime = frameTime;
                    value.AddDrawCall(drawCall);

                    value.SetPeakMemoryUsage(memoryInFrame);
                    value.SetMeshMemorySize(meshMemory);
                    value.SetTextureMemorySize(textureMemory);
                }
            }


            FrameTimingManager.CaptureFrameTimings();

            var frameTiming = FrameTimingManager.GetLatestTimings((uint) _mFrameTimings.Length, _mFrameTimings);
            if (frameTiming > 0)
            {
                var gpuTime = Math.Round(_mFrameTimings[0].gpuFrameTime * 0.001f, 2);
                var cpuTime = Math.Round(_mFrameTimings[0].cpuFrameTime * 0.001f, 2);
                
                foreach (var (key, value) in GameFeature)
                {
                    if (value.Recording)
                    {
                        value.AddFrameTime(gpuTime, cpuTime);
                    }
                }
            }
        }

        private FrameTiming[] _mFrameTimings = new FrameTiming[1];
        
        private static readonly Dictionary<string, PerformanceStats> GameFeature = new Dictionary<string, PerformanceStats>();

        
        public static void StartMeasure(string feature)
        {
            if (GameFeature.TryGetValue(feature, out var value))
            {
                if (value.Recording)
                {
                    //reset
                    value.Reset();
                    value.Recording = true;
                }
            }
            else
            {
                var newStats = new PerformanceStats();
                newStats.QualityLevel = "Not Available";
                newStats.FeatureName = ZString.Format("Feature: {0}", feature);
                newStats.StartCountScreenTime();
                GameFeature.Add(feature, newStats);
                newStats.Recording = true;
                newStats.SetReservedMemorySize();
            }
        }
        
        public static void StartMeasure(string feature, string qualityLevel)
        {
            if (GameFeature.TryGetValue(feature, out var value))
            {
                if (value.Recording)
                {
                    //reset
                    value.Reset();
                    value.Recording = true;
                }
            }
            else
            {
                var newStats = new PerformanceStats();
                newStats.QualityLevel = qualityLevel;
                newStats.FeatureName = ZString.Format("Feature: {0}", feature);
                newStats.StartCountScreenTime();
                GameFeature.Add(feature, newStats);
                newStats.Recording = true;
                newStats.SetReservedMemorySize();
            }
        }

        public static async UniTask StopMeasure(string feature)
        {
            if (GameFeature.TryGetValue(feature, out var value))
            {
                if (value.Recording)
                {
                    value.Recording = false;
                    value.SetComplete();
                    await SendToGoogleSheet.Send(value);

                    GameFeature.Remove(feature);
                }
            }
        }
    }
}