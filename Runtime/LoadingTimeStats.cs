using Cysharp.Text;
using UnityEngine;

namespace Wolffun.RuntimeProfiler
{
    public class LoadingTimeStats
    {
        public readonly string DeviceStats;
        public readonly string AppVersion;
        public readonly string BuildNumber;
        public readonly string AppName;
        public readonly string Platform;

        public string LoadingTime;
        public double FirstTime;
        public double Mean;

        public LoadingTimeStats()
        {
            AppName = Application.productName;
            AppVersion = Application.version;
            //bundle version
            BuildNumber = Application.buildGUID;

            Platform = Application.platform.ToString();
            DeviceStats = ZString.Format("Device {0} Ram = {1} OS = {2}", SystemInfo.deviceModel,
                SystemInfo.systemMemorySize, SystemInfo.operatingSystem);
        }
    }
}