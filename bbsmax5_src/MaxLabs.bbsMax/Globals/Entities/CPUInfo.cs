//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;

namespace MaxLabs.bbsMax.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CPUInfo
    {
        /// <summary>
        /// OEM  ID
        /// </summary>
        public uint dwOemId;
        /// <summary>
        /// 页面大小
        /// </summary>
        public uint dwPageSize;
        public uint lpMinimumApplicationAddress;
        public uint lpMaximumApplicationAddress;
        public uint dwActiveProcessorMask;
        /// <summary>
        /// 个数
        /// </summary>
        public uint dwNumberOfProcessors;
        /// <summary>
        /// 类型
        /// </summary>
        public uint dwProcessorType;

        public uint dwAllocationGranularity;
        /// <summary>
        /// 等级
        /// </summary>
        public uint dwProcessorLevel;
        public uint dwProcessorRevision;

        public string cpuType;
        public float cpuLoad;

        private const string CategoryName = "Processor";
        private const string CounterName = "% Processor Time";
        private const string InstanceName = "_Total";

        private const string cacheKey = "Max_CpuType";
        public CPUInfo GetCPUInfo()
        {
            CPUInfo CpuInfo;
            if (!CacheUtil.TryGetValue<CPUInfo>(cacheKey, out CpuInfo))
            {
                CpuInfo = new CPUInfo();
                GetSystemInfo(ref  CpuInfo);


                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT   *   FROM   Win32_Processor");
                foreach (ManagementObject mo in mos.Get())
                {
                    CpuInfo.cpuType = mo["name"].ToString();
                    break;
                }
                CacheUtil.Set<CPUInfo>(cacheKey, CpuInfo, CacheTime.NotRemovable);
            }


            PerformanceCounter pc = new PerformanceCounter(CategoryName, CounterName, InstanceName);
            CpuInfo.cpuLoad = pc.NextValue();
            return CpuInfo;
        }

        [DllImport("kernel32")]
        public static extern void GetSystemInfo(ref  CPUInfo cpuinfo);
    }
}