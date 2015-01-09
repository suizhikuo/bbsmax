//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Collections;
using System.IO;
using System.Resources;
using System.Threading;

using ICSharpCode.SharpZipLib.Zip;
using Max.Installs;

namespace Max.WinUI
{
    public class InstallWaiter
    {
        public bool running = true;
        public InstallDelegate InstallBarEvent;

        public InstallWaiter(MainForm mainForm)
        {
            //监视winForm 风吹草动...用于暂停当前线程
            mainForm.PauseEvent += this.onPauseClient;
        }

        public void onPauseClient()
        {
            this.running = !this.running;
        }

        public void UpZip()
        {
            int k = 0;
            ResourceManager rm = Install_Bin.ResourceManager;
            string rootPath = SiteInfo.Current.WebPath + "\\";
            Stream stream = new MemoryStream((byte[])rm.GetObject("bbsmax_zip"));
            Stream zip = new MemoryStream((byte[])rm.GetObject("bbsmax_zip"));
            //先计算出文件个数
            using (ZipInputStream zipInputStream = new ZipInputStream(stream))
            {
                ZipEntry zipEntry;
                while ((zipEntry = zipInputStream.GetNextEntry()) != null)
                    k++;
                zipInputStream.Close();
            }

            using (ZipInputStream s = new ZipInputStream(zip))
            {
                ZipEntry entry;
                int i = 0;
                while ((entry = s.GetNextEntry()) != null)
                {
                    while (!this.running)
                    {
                        Thread.Sleep(1000);
                    }
                    InstallBarEvent(k, i++);//触发事件
                    string directoryName = Path.GetDirectoryName(entry.Name);
                    string fileName = Path.GetFileName(entry.Name);
                    try
                    {
                        if (directoryName.Length > 0 && !Directory.Exists(rootPath + directoryName))
                        {
                            Directory.CreateDirectory(rootPath + directoryName);
                        }

                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string eName = entry.Name;
                            eName = eName.ToLower();
                            SetupManager.CreateFile(rootPath + eName, s);
                        }
                    }
                    catch (Exception ex)
                    {
                        SetupManager.CreateLog("解压文件时出错：" + ex.ToString());
                        throw ex;
                    }
                }
                s.Close();
            }
        }
    }
}