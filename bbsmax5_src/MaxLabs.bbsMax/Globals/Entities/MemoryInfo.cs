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

namespace MaxLabs.bbsMax.Entities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryInfo
    {
        public uint dwLength;
        /// <summary>
        /// 正在使用的内存(?%)
        /// </summary>
        public uint dwMemoryLoad;
        /// <summary>
        /// 物理内存(单位字节)
        /// </summary>
        public uint dwTotalPhys;

        /// <summary>
        /// 当前程序占用的物理内存
        /// </summary>
        public int dwCurrentMemorySize;
        /// <summary>
        /// 可使用的物理内存(单位字节)
        /// </summary>
        public uint dwAvailPhys;
        /// <summary>
        /// 交换文件总大小(单位字节)
        /// </summary>
        public uint dwTotalPageFile;
        /// <summary>
        /// 尚可交换文件大小(单位字节)
        /// </summary>
        public uint dwAvailPageFile;
        /// <summary>
        /// 总虚拟内存(单位字节)
        /// </summary>
        public uint dwTotalVirtual;
        /// <summary>
        /// 未用虚拟内存(单位字节)
        /// </summary>
        public uint dwAvailVirtual;

        public MemoryInfo GetMemoryInfo()
        {
            MemoryInfo memoryInfo = new MemoryInfo();
            GlobalMemoryStatus(ref  memoryInfo);
            System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
            memoryInfo.dwCurrentMemorySize = p.NonpagedSystemMemorySize;
            return memoryInfo;
        }

        [DllImport("kernel32")]
        public static extern void GlobalMemoryStatus(ref  MemoryInfo memoryInfo);
    }
}