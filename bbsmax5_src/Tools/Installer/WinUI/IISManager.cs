//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.DirectoryServices;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace Max.WinUI
{
    class IISManager
    {
        /// <summary>
        /// 创建网站
        /// </summary>
        public static void CreateWebsite()
        {
            string entPath = String.Format("IIS://{0}/w3svc", SiteInfo.Current.DomainName);
            string newSiteNum = GetNewWebSiteID();

            DirectoryEntry rootEntry = new DirectoryEntry(entPath);
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum, "IIsWebServer");
            newSiteEntry.CommitChanges();

            //属性设置
            newSiteEntry.Properties["ServerBindings"].Value = SiteInfo.Current.IP + ":" + SiteInfo.Current.Port + ":" + SiteInfo.Current.Host;//Ip 端口 主机头
            newSiteEntry.Properties["ServerComment"].Value = SiteInfo.Current.ServerComment;//网站描述
            newSiteEntry.Properties["DefaultDoc"].Value = SiteInfo.Current.DefaultDoc;//默认文档
            if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                newSiteEntry.Properties["AppPoolId"].Value = SiteInfo.Current.AppPool;//应用程序池

            newSiteEntry.Properties["AccessFlags"][0] = 513;//AccessRead:1+ AccessScript:512 执行权限:纯脚本  记录访问 索引资源?
            newSiteEntry.Properties["AuthFlags"][0] = 1;//AuthPassport:64 AuthMD5:16 AuthNTLM:4 AuthBasic:2 AuthAnonymous:1 匿名访问  
            newSiteEntry.Properties["AnonymousUserName"][0] = SiteInfo.Current.UserName_iusr;//匿名访问用户名
            newSiteEntry.Properties["AnonymousUserPass"][0] = SiteInfo.Current.PassWord_iusr;//密码

            //脚本映射 Extension, ScriptProcessor, Flags, IncludedVerbs
            string ScriptMaps = ".aspx," + System.Environment.GetFolderPath(System.Environment.SpecialFolder.System).ToUpper().Replace("SYSTEM32", "") + @"microsoft.net\framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            if (!newSiteEntry.Properties["ScriptMaps"].Contains(ScriptMaps))
                newSiteEntry.Properties["ScriptMaps"][0] = ScriptMaps;

            //创建默认应用程序
            DirectoryEntry vdEntry = newSiteEntry.Children.Add("ROOT", "IIsWebVirtualDir");
            vdEntry.Properties["AppRoot"][0] = "/LM/W3SVC/" + newSiteNum + "/ROOT";
            vdEntry.Properties["AppFriendlyName"][0] = SiteInfo.Current.ServerComment;
            vdEntry.Invoke("AppCreate", true);
            vdEntry.Properties["Path"][0] = SiteInfo.Current.WebPath;
            if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                CreateAppPool();

            AspNetRegIIS("-s  W3SVC/" + newSiteNum);//.net配置
            vdEntry.CommitChanges();
            vdEntry.RefreshCache();
            newSiteEntry.CommitChanges();
            newSiteEntry.RefreshCache();
        }
        /// <summary>
        /// 创建虚拟目录
        /// </summary>
        public static void CreateVirtual()
        {
            DirectoryEntry currentSite = SiteInfo.Current.CurrentSite;//站点或虚拟目录
            if (currentSite.SchemaClassName == "IIsWebServer")
                currentSite = new DirectoryEntry(currentSite.Path + "/root");

            DirectoryEntry vdEntry = currentSite.Children.Add(SiteInfo.Current.VirtualName, "IIsWebVirtualDir");
            vdEntry.CommitChanges();

            //属性设置
            vdEntry.Properties["Path"][0] = SiteInfo.Current.WebPath;
            vdEntry.Properties["AppFriendlyName"].Value = SiteInfo.Current.VirtualName;
            vdEntry.Properties["DefaultDoc"].Value = SiteInfo.Current.DefaultDoc;
            if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                vdEntry.Properties["AppPoolId"][0] = SiteInfo.Current.AppPool;

            vdEntry.Properties["AppRoot"][0] = vdEntry.Path.Replace("IIS://" + SiteInfo.Current.DomainName, "/LM");//路径特殊处理 创建应用程序
            vdEntry.Invoke("AppCreate", true);
            vdEntry.Properties["AccessFlags"][0] = 513;
            vdEntry.Properties["AuthFlags"][0] = 1;
            vdEntry.Properties["AnonymousUserName"][0] = SiteInfo.Current.UserName_iusr;
            vdEntry.Properties["AnonymousUserPass"][0] = SiteInfo.Current.PassWord_iusr;
            string ScriptMaps = ".aspx," + System.Environment.GetFolderPath(System.Environment.SpecialFolder.System).ToUpper().Replace("SYSTEM32", "") + @"microsoft.net\framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            if (!vdEntry.Properties["ScriptMaps"].Contains(ScriptMaps))
                vdEntry.Properties["ScriptMaps"][0] = ScriptMaps;

            if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                CreateAppPool();

            AspNetRegIIS("-s  " + vdEntry.Path.Replace("IIS://" + SiteInfo.Current.DomainName + "/", ""));//路径特殊处理
            vdEntry.CommitChanges();
            vdEntry.RefreshCache();
            currentSite.CommitChanges();
            currentSite.RefreshCache();
        }
        /// <summary>
        /// 创建应用程序池 IIS5.0以上
        /// </summary>
        public static void CreateAppPool()
        {
            string entPath = String.Format("IIS://{0}/w3svc/AppPools", SiteInfo.Current.DomainName);
            DirectoryEntry rootAppPool = new DirectoryEntry(entPath);
            DirectoryEntry newAppPool = rootAppPool.Children.Add(SiteInfo.Current.AppPool, "IIsApplicationPool");
            newAppPool.Properties["WAMUserName"][0] = SiteInfo.Current.UserName_iwam;
            newAppPool.Properties["WAMUserPass"][0] = SiteInfo.Current.PassWord_iwam;
            newAppPool.Properties["AppPoolIdentityType"][0] = 3;
            newAppPool.CommitChanges();
            newAppPool.RefreshCache();
        }
        /// <summary>
        /// 删除网站或虚拟目录
        /// </summary>
        /// <param name="virPath">地址 1/root/bbsmax</param>
        /// <returns></returns>
        public static void DeleteSiteVirtual(string path)
        {
            DirectoryEntry deDir = new DirectoryEntry(path);
            try
            {
                deDir.DeleteTree();
            }
            catch (Exception ex)
            {
                throw new Exception("删除失败：" + ex.Message);
            }
            finally
            {
                deDir.Close();
            }
        }
        /// <summary>
        /// 获取新站点Id
        /// </summary>
        /// <returns></returns>
        public static string GetNewWebSiteID()
        {
            ArrayList idList = new ArrayList();
            string tmpStr;
            string entPath = string.Format("IIS://{0}/W3SVC",SiteInfo.Current.DomainName);
            DirectoryEntry entry = new DirectoryEntry(entPath);
            foreach (DirectoryEntry child in entry.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    tmpStr = child.Name.ToString();
                    idList.Add(Convert.ToInt32(tmpStr));
                }
            }
            idList.Sort();
            int i = 1;
            foreach (int id in idList)
            {
                if (i == id)
                    i++;
            }
            return i.ToString();
        }
        /// <summary>
        /// 通过主机名取站点Id www.bbsmax.com
        /// </summary>
        /// <param name="webSiteName"></param>
        /// <returns></returns>
        public static string GetSiteID(string webSiteName)
        {
            DirectoryEntry root = new DirectoryEntry(string.Format("IIS://{0}/W3SVC", SiteInfo.Current.DomainName));
            try
            {
                string SiteID = null;
                string hostname;
                foreach (DirectoryEntry bb in root.Children)
                {
                    try
                    {
                        PropertyValueCollection pvc = bb.Properties["ServerBindings"];
                        String[] srvBindings = ((string)pvc[0]).Split(new char[] { ':' });
                        hostname = srvBindings[2].Trim();
                        if (webSiteName == hostname) SiteID = bb.Name;
                        hostname = "";
                    }
                    catch { }
                }
                if (SiteID == null) return null;
                return SiteID;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 机器名
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch
            {
                return "";
            }
        }

        public static void CheckProperties(DirectoryEntry dirEnt)
        {
            dirEnt.Properties["DefaultDoc"].Value = SiteInfo.Current.DefaultDoc;//默认文档
            dirEnt.Properties["AccessFlags"][0] = 513;
            dirEnt.Properties["AuthFlags"][0] = 1;
            string ScriptMaps = ".aspx," + System.Environment.GetFolderPath(System.Environment.SpecialFolder.System).ToUpper().Replace("SYSTEM32", "") + @"Microsoft.net\Framework\v2.0.50727\aspnet_isapi.dll,1,GET,HEAD,POST,DEBUG";
            if (!dirEnt.Properties["ScriptMaps"].Contains(ScriptMaps))
                dirEnt.Properties["ScriptMaps"][0] = ScriptMaps;
            dirEnt.CommitChanges();
            dirEnt.RefreshCache();

        }
        /// <summary>
        /// 设置站点信息
        /// </summary>
        /// <param name="path">站点路径</param>
        public static void SetSiteInfo(string path)
        {
            DirectoryEntry ent = new DirectoryEntry(path);
            if (ent.SchemaClassName == "IIsWebServer" || ent.SchemaClassName.Equals("IIsWebVirtualDir"))
            {
                //检查iis属性
                CheckProperties(ent);
                if (ent.SchemaClassName.Equals("IIsWebServer"))
                {
                    //取根目录
                    ent = new DirectoryEntry(path + "/root");
                    SiteInfo.Current.WebPath = ent.Properties["Path"].Value.ToString();
                    SiteInfo.Current.UserName_iusr = ent.Properties["AnonymousUserName"].Value.ToString();
                    SiteInfo.Current.VirtualName = "";//根目录
                    if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                        SiteInfo.Current.AppPool = ent.Properties["AppPoolId"].Value.ToString();//当前程序的应用程序池

                    ent = new DirectoryEntry(path);
                    //192.168.1.1:81:www.bbsmax.com
                    string[] Host = ent.Properties["ServerBindings"][0].ToString().Split(':');
                    SiteInfo.Current.IP = Host[0];
                    SiteInfo.Current.Port = Host[1];
                    SiteInfo.Current.Host = Host[2];
                }
                else
                {
                    Regex regRoot = new Regex(".*root/", RegexOptions.IgnoreCase);
                    SiteInfo.Current.WebPath = ent.Properties["Path"].Value.ToString();
                    SiteInfo.Current.VirtualName = regRoot.Replace(path, "");
                    SiteInfo.Current.UserName_iusr = ent.Properties["AnonymousUserName"].Value.ToString();
                    if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                        SiteInfo.Current.AppPool = ent.Properties["AppPoolId"].Value.ToString();
                    regRoot = new Regex(".*(?=/root)", RegexOptions.IgnoreCase);
                    ent = new DirectoryEntry(regRoot.Match(path).Value.ToString());
                    string[] Host = ent.Properties["ServerBindings"][0].ToString().Split(':');
                    SiteInfo.Current.IP = Host[0];
                    SiteInfo.Current.Port = Host[1];
                    SiteInfo.Current.Host = Host[2];
                }
                if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
                {
                    ent = new DirectoryEntry(String.Format("IIS://{0}/w3svc/AppPools/{1}", SiteInfo.Current.DomainName, SiteInfo.Current.AppPool));
                    SiteInfo.Current.UserName_iwam = ent.Properties["WAMUserName"].Value.ToString();
                }
            }
        }
        /// <summary>
        /// 主机Ip 取第一个
        /// </summary>
        /// <returns></returns>
        public static string GetHostIP()
        {
            string strHostIP = "";
            IPHostEntry oIPHost = Dns.GetHostEntry(SiteInfo.Current.DomainName);
            if (oIPHost.AddressList.Length > 0)
                strHostIP = oIPHost.AddressList[0].ToString();
            return strHostIP;
        }
        /// <summary>
        /// 是否安装IIS
        /// </summary>
        /// <returns></returns>
        public static bool IsIISRegistered()
        {
            bool result = false;
            DirectoryEntry entry = new DirectoryEntry("IIS://Localhost/W3SVC");
            //遍历目录中的web站点
            foreach (DirectoryEntry site in entry.Children)
            {
                if (site.SchemaClassName.Equals("IIsWebServer"))
                {
                    //遍历web站点属性，通过ScriptMaps属性来判断IIS是否注册
                    foreach (string propertyName in site.Properties.PropertyNames)
                    {
                        if (propertyName.Equals("ScriptMaps"))
                        {
                            PropertyValueCollection valueCollection = site.Properties[propertyName];
                            for (int i = 0; i < valueCollection.Count; i++)
                            {
                                string propertyValue = valueCollection[i].ToString();
                                //通过ScriptMaps属性的值中是否包含web后缀来判断
                                if (propertyValue.Contains(".aspx"))
                                {
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 执行IIS命令
        /// </summary>
        /// <param name="args"></param>
        public static void AspNetRegIIS(string args)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System).ToUpper().Replace("SYSTEM32", "") + @"Microsoft.NET\Framework\v2.0.50727\aspnet_regiis.exe";
            if (!File.Exists(path))
            {
                throw new Exception("请确认已安装.net2.0版本");
            }
            else
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = args;
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
            }
        }
        /// <summary>
        /// 获取IIS版本信息
        /// </summary>
        /// <returns></returns>
        public static IISVersion GetIISVersion()
        {
            string path = string.Format("IIS://{0}/W3SVC/INFO", SiteInfo.Current.DomainName);
            DirectoryEntry entry = null;
            try
            {
                entry = new DirectoryEntry(path);
            }
            catch
            {
                return IISVersion.Unknown;
            }
            int num = 5;
            try
            {
                num = (int)entry.Properties["MajorIISVersionNumber"].Value;
            }
            catch
            {
                return IISVersion.IIS5;
            }
            switch (num)
            {
                case 6:
                    return IISVersion.IIS6;

                case 7:
                    return IISVersion.IIS7;
            }
            return IISVersion.IIS6;
        }
    }

    public enum IISVersion
    {
        IIS5 = 5,
        IIS6 = 6,
        IIS7 = 7,
        Unknown = 0
    }
}