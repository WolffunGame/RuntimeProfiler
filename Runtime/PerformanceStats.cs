using System;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace Wolffun.RuntimeProfiler
{
    public struct FrameTime
    {
        public double CpuFrameTime;
        public double GpuFrameTime;

        public override string ToString()
        {
            return $"{CpuFrameTime:F},{GpuFrameTime:F}";
        }
    }
    
    public class PerformanceStats
    {
        public readonly string DeviceName;
        public readonly string DeviceStats;
        public readonly string AppVersion;
        public readonly string BuildNumber;
        public readonly string AppName;
        public readonly string Platform;

        public string FeatureName;
        public double MeanFrameTime;
        public double MaxFrameTime;
        public double LeftQuartileFrameTime;
        public double MedianFrameTime;
        public double RightQuartileFrameTime;
        public double FrameTimeExceeded; // percentage of frame time exceeded 33.3ms

        public float MeanDrawCall;
        public float MaxDrawCall;
        public float MedianDrawCall;
        public float LeftQuartileDrawCall;
        public float RightQuartileDrawCall;


        public double ScreenTime;
        public long ReservedMemorySize;
        public long PeakMemoryUsage;
        public long MeshMemryUsage;
        public long TextureMemoryUsage;


        public double GpuFrameTime;

        public string QualityLevel;

        public float MeanIngameSimulationTime;
        public float IngameSimulationTimeExceeded; // percentage of ingame simulation time exceeded 5ms

        public int PlayerCountOnStartBattle;
        
        private readonly List<DateTime> _timeStamps = new();
        public readonly List<FrameTime> FrameTimes = new();
        public readonly List<double> MainThreadFrameTimes = new();
        public readonly List<float> MainThreadIngameSimulationTimes = new();
        public readonly List<float> DrawCalls = new();

        private long _totalDrawCall = 0;
        private double _totalFrameTime = 0;
        private int _countExceededFrameTime = 0;
        
        private float _totalIngameSimulationTime = 0;
        private int _countExceededIngameSimulationTime = 0;
        
        
        public const float FRAME_TIME_THRESHOLD = 33.33f;
        public const float INGAME_SIMULATION_TIME_THRESHOLD = 5f;
        
        public PerformanceStats()
        {
            DeviceName = SystemInfo.deviceModel;
            AppName = Application.productName;
            AppVersion = Application.version;
            //bundle version
            BuildNumber = Application.buildGUID;

            Platform = Application.platform.ToString();
            DeviceStats = ZString.Format("Device {0} Ram = {1} OS = {2}", SystemInfo.deviceModel,
                SystemInfo.systemMemorySize, SystemInfo.operatingSystem);
            _totalDrawCall = 0L;
            _totalFrameTime = 0;
            _countExceededFrameTime = 0;

            _totalIngameSimulationTime = 0;
            _countExceededIngameSimulationTime = 0;
        }

        public bool Recording { get; set; } = false;

        public void AddDrawCall(long drawCall = 1)
        {
            _totalDrawCall += drawCall;
            DrawCalls.Add(drawCall);

            if (drawCall > MaxDrawCall) MaxDrawCall = drawCall;
        }

        public void AddFrameTime(double frameTime)
        {
            _totalFrameTime += frameTime;
            MainThreadFrameTimes.Add(frameTime);

            if (frameTime > MaxFrameTime) MaxFrameTime = frameTime;

            if (frameTime > FRAME_TIME_THRESHOLD) _countExceededFrameTime++;
        }
        
        public void AddIngameSimulationTime(float updateTime)
        {
            _totalIngameSimulationTime += updateTime;
            MainThreadIngameSimulationTimes.Add(updateTime);

            if (updateTime > INGAME_SIMULATION_TIME_THRESHOLD) _countExceededIngameSimulationTime++;
        }
        
        public void SetPlayerCountOnStartBattle(int playerCount)
        {
            PlayerCountOnStartBattle = playerCount;
        }

        public void AddGpuFrameTime(double gpuFrameTime)
        {
            GpuFrameTime += gpuFrameTime;
        }

        public void StartCountScreenTime()
        {
            _timeStamps.Add(DateTime.Now);
        }

        public void StopCountScreenTime()
        {
            foreach (var timeStamp in _timeStamps)
            {
                var currentTimeStampIndex = _timeStamps.IndexOf(timeStamp);
                if (currentTimeStampIndex > 0)
                    ScreenTime += (float) (timeStamp - _timeStamps[currentTimeStampIndex - 1]).TotalSeconds;
            }
            
            _timeStamps.Clear();
        }

        private void SetFrameTimeExceeded()
        {
            FrameTimeExceeded = _countExceededFrameTime * 1.0f / MainThreadFrameTimes.Count;
        }
        
        private void SetIngameSimulationTimeExceeded()
        {
            IngameSimulationTimeExceeded = _countExceededIngameSimulationTime * 1.0f / MainThreadIngameSimulationTimes.Count;
        }

        private void SetAvgFrameTime()
        {
            MeanFrameTime = _totalFrameTime / MainThreadFrameTimes.Count;

            //calculate quartile
            var frameTimes = new double[MainThreadFrameTimes.Count];
            MainThreadFrameTimes.CopyTo(frameTimes);
            Array.Sort(frameTimes);
            if (frameTimes.Length == 0) return;
            var medianIndex = frameTimes.Length / 2;
            MedianFrameTime = frameTimes[medianIndex];
            var leftQuartileIndex = medianIndex / 2;
            LeftQuartileFrameTime = frameTimes[leftQuartileIndex];
            var rightQuartileIndex = medianIndex + leftQuartileIndex;
            RightQuartileFrameTime = frameTimes[rightQuartileIndex];
        }
        
        private void SetAvgIngameSimulationTime()
        {
            MeanIngameSimulationTime = _totalIngameSimulationTime / MainThreadIngameSimulationTimes.Count;
        }

        private void SetAvgDrawCall()
        {
            MeanDrawCall = _totalDrawCall * 1.0f / DrawCalls.Count;

            //calculate quartile
            var drawCalls = new float[DrawCalls.Count];
            DrawCalls.CopyTo(drawCalls);
            Array.Sort(drawCalls);
            if (drawCalls.Length == 0) return;
            var medianIndex = drawCalls.Length / 2;
            MedianDrawCall = drawCalls[medianIndex];
            var leftQuartileIndex = medianIndex / 2;
            LeftQuartileDrawCall = drawCalls[leftQuartileIndex];
            var rightQuartileIndex = medianIndex + leftQuartileIndex;
            RightQuartileDrawCall = drawCalls[rightQuartileIndex];
        }

        public void SetComplete()
        {
            SetAvgFrameTime();
            SetAvgIngameSimulationTime();
            SetAvgDrawCall();
            SetFrameTimeExceeded();
            SetIngameSimulationTimeExceeded();
        }

        public void SetReservedMemorySize()
        {
            ReservedMemorySize = UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024 / 1024;
        }

        public void SetTextureMemorySize(long textureMemoryUsage)
        {
            TextureMemoryUsage = textureMemoryUsage / 1024 / 1024;
        }

        public void SetMeshMemorySize(long meshtureMemoryUsage)
        {
            MeshMemryUsage = meshtureMemoryUsage / 1024 / 1024;
        }

        public void SetPeakMemoryUsage(long peakMemoryUsage)
        {
            PeakMemoryUsage = peakMemoryUsage;
        }

        public void AddFrameTime(double gpuFrameTime, double cpuFrameTime)
        {
            if (cpuFrameTime > FRAME_TIME_THRESHOLD || gpuFrameTime > FRAME_TIME_THRESHOLD)
            {
                _countExceededFrameTime++;
                FrameTimes.Add(new FrameTime
                {
                    GpuFrameTime = gpuFrameTime,
                    CpuFrameTime = cpuFrameTime
                });
            }
        }

        public string GetFrameTimes()
        {
            var sb = ZString.CreateStringBuilder();
            sb.Append(ZString.Format("FrameTimes: {0}", FrameTimes.Count));
            sb.Append("[");
            for (var i = 0; i < (FrameTimes.Count > 100 ? 100 : FrameTimes.Count); i++)
            {
                var frameTime = FrameTimes[i];
                sb.Append("{");
                sb.Append(frameTime);
                sb.Append("}");
                if (i < FrameTimes.Count - 1) sb.Append(",");
            }

            sb.Append("]");

            return sb.ToString();
        }

        public void Reset()
        {
            _totalDrawCall = 0L;
            _totalFrameTime = 0;
            _countExceededFrameTime = 0;
            MeanFrameTime = 0;
            MaxFrameTime = 0;
            LeftQuartileFrameTime = 0;
            MedianFrameTime = 0;
            RightQuartileFrameTime = 0;
            FrameTimeExceeded = 0; // percentage of frame time exceeded 33.3ms

            MeanDrawCall = 0;
            MaxDrawCall = 0;
            MedianDrawCall = 0;
            LeftQuartileDrawCall = 0;
            RightQuartileDrawCall = 0;

            ScreenTime = 0;
            ReservedMemorySize = 0;
            PeakMemoryUsage = 0;
            MeshMemryUsage = 0;
            TextureMemoryUsage = 0;

            _totalIngameSimulationTime = 0;
            _countExceededIngameSimulationTime = 0;
            MeanIngameSimulationTime = 0;
            IngameSimulationTimeExceeded = 0;
            PlayerCountOnStartBattle = 0;
        }
    }
}

public struct IngamePerformanceStats
{
    public float simulationTime;
    public int playerCountOnStartBattle;
}