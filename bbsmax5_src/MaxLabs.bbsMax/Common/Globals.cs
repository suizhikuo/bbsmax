//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Collections.Generic;

using MaxLabs.bbsMax.AppHandlers;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using System.Web.Hosting;
using MaxLabs.bbsMax.Providers;

namespace MaxLabs.bbsMax
{
    public class Globals
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public const string Version = "bbsmax 5 (1008)";

        /// <summary>
        /// 内部版本号
        /// </summary>
        public const string InternalVersion = "5.0.1.1008";

        private const string SystemDirecotry_Code = "_codes";
        private const string SystemDirecotry_Error = "error";
        private const string SystemDirecotry_Dialogs = "max-dialogs";
        private const string SystemDirecotry_Admin = "max-admin";
        private const string SystemDirecotry_Pages = "max-templates/default";
        private const string SystemDirecotry_Plugins = "max-plugins";
        private const string SystemDirecotry_Temp = "max-temp";
        private const string SystemDirecotry_Temp_Upload = "upload";
        private const string SystemDirecotry_Temp_Avatar = "avatar";
        private const string SystemDirecotry_Temp_ParsedTemplate = "parsed-template";
        private const string SystemDirecotry_Upload = "userfiles";
        private const string SystemDirecotry_Upload_Avatar = @"A";
        private const string SystemDirecotry_Upload_Emoticon = @"emoticons";
        private const string SystemDirecotry_Upload_File = @"diskfiles";
        private const string SystemDirecotry_Upload_IDCard = "idcard";
        private const string SystemDirecotry_Assets = "max-assets";
        private const string SystemDirecotry_Assets_Images = "images";
        private const string SystemDirecotry_Assets_OnlineIcon = "icon-online";
        private const string SystemDirecotry_Assets_ForumLogo = "logo-forum";
        private const string SystemDirecotry_Assets_MedalIcon = "icon-medal";
        private const string SystemDirecotry_Assets_PointIcon = "icon-point";
        private const string SystemDirecotry_Assets_MissionIcon = "icon-mission";
        private const string SystemDirectory_Assets_FileIcon = "icon-file";
        private const string SystemDirecotry_Assets_RoleIcon = "icon-role";
        private const string SystemDirecotry_Assets_ZodiacIcon = "icon-zodiac";
        private const string SystemDirecotry_Assets_LinkIcon = "logo-link";
        private const string SystemDirecotry_Assets_judgement = "icon-judgement";
        private const string SystemDirecotry_Assets_Post = "icon-post";
        private const string SystemDirecotry_Assets_Face = "icon-emoticon";
        private const string SystemDirecotry_Assets_IconStar = "icon-star";
        private const string SystemDirecotry_Assets_Icon = "icon";
        private const string SystemDirecotry_Assets_AdvertIcon = "icon-advert";
        private const string SystemDirectory_Assets_PropIcons = "icon-prop";
        private const string SystemDirecotry_Assets_Sounds = "sound-msg";
        private const string SystemDirecotry_Assets_Flash = "flash";

        private const string SystemDirecotry_Themes = "max-templates";
        private const string SystemDirecotry_SpaceStyles = "max-spacestyles";
        private const string SystemDirectory_Album_Thumbs = "album_thumbs";

        private const string SystemDirectory_Js = "max-js";

        private static AppConfig s_AppConfig = null;

        private static string s_AppRoot;
        private static string s_ApplicationPath;
        private static string s_BinDirectory;

#if DEBUG
        public const bool DebugVersion = true;
#else
        public const bool DebugVersion = false;
#endif

        static Globals()
        {
            try
            {
                s_ApplicationPath = HttpRuntime.AppDomainAppPath;

                string url = VirtualPathUtility.RemoveTrailingSlash(HttpRuntime.AppDomainAppVirtualPath);
                if (url == "/")
                    s_AppRoot = string.Empty;
                else
                    s_AppRoot = url;

                s_BinDirectory = HttpRuntime.BinDirectory;
            }
            catch
            {
                s_AppRoot = string.Empty;
            }
        }

        public static void Init()
        {
            string dataAccessProviderName = DataProviderName;

            ProviderManager.Set<IDataProvider>(
                (IDataProvider)Activator.CreateInstance(Type.GetType(dataAccessProviderName))
            );

            //一次性读取并缓存系统设置
            SettingManager.CacheAllSettings();
        }

        #region 配置文件bbsmax.config中的各项配置

        private static string GetConfigFilePath()
        {
            return IOUtil.JoinPath(ApplicationPath, "bbsmax.config");
        }

        /// <summary>
        /// 一旦bbsmax.config发生了变动会立即触发本静态方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cacheItem"></param>
        /// <param name="reason"></param>
        private static void OnAppConfigRemove(string key, object cacheItem, System.Web.Caching.CacheItemRemovedReason reason)
        {
            AppConfig newConfig = null;
            try
            {
                newConfig = ReadAppConfig();
            }
            catch
            {
                //读取配置文件发生错误
                s_AppConfig = null;
                HttpRuntime.UnloadAppDomain();
                throw;
            }

            //一旦连接字符串和提供程序发生了变化，马上重启应用程序
            if (newConfig.ConnectionString != s_AppConfig.ConnectionString
                ||
                newConfig.DataProviderName != s_AppConfig.DataProviderName
                ||
                newConfig.InstallDirectory != s_AppConfig.InstallDirectory
                ||
                newConfig.PassportClient != s_AppConfig.PassportClient
                )
            {
                s_AppConfig = newConfig;
                HttpRuntime.UnloadAppDomain();
            }
            else
            {
                string configFilePath = GetConfigFilePath();
                //创建一个缓存，目的是监视bbsmax.config文件的改动
                HttpRuntime.Cache.Add("bbsmax.config", true, new System.Web.Caching.CacheDependency(configFilePath), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, OnAppConfigRemove);

                //如果压缩设置发生了变化
                if (newConfig.StaticCompress != s_AppConfig.StaticCompress
                    ||
                    newConfig.DynamicCompress != s_AppConfig.DynamicCompress
                    ||
                    newConfig.Licence != s_AppConfig.Licence
                    ||
                    IsOwnerUsernamesSame(newConfig.OwnerUsernames, s_AppConfig.OwnerUsernames)

                    )
                {
                    s_AppConfig = newConfig;
                }
            }
        }

        private static bool IsOwnerUsernamesSame(List<string> list1, List<string> list2)
        {
            if (list1 == null && list2 == null)
                return true;
            else if (list1 == null || list2 == null)
                return false;
            else if (list1.Count != list2.Count)
                return false;
            else
            {
                for (int i = 0; i < list1.Count; i++)
                {
                    if (list1[i] != list2[i])
                        return false;
                }

                return true;
            }
        }

        internal static void SaveAppConfig(AppConfig config)
        {
            string configFilePath = GetConfigFilePath();

            config.Write(configFilePath);
        }

        private static AppConfig ReadAppConfig()
        {
            string configFilePath = GetConfigFilePath();

            AppConfig config = new AppConfig();
            config.Read(configFilePath);

            return config;
        }

        internal static AppConfig CurrentAppConfig
        {
            get
            {
                AppConfig config = s_AppConfig;

                if (config == null)
                {
                    string configFilePath = GetConfigFilePath();
                    //创建一个缓存，目的是监视bbsmax.config文件的改动
                    HttpRuntime.Cache.Add("bbsmax.config", true, new System.Web.Caching.CacheDependency(configFilePath), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, OnAppConfigRemove);

                    config = ReadAppConfig();
                    //safeString = SecurityUtil.MD5(s_AppConfig.ConnectionString).Substring(0, 10);

                    s_AppConfig = config;
                }

                return config;
            }
        }

        /// <summary>
        /// 数据库联接字符串
        /// </summary>
        public static string ConnectionString
        {
            get { return CurrentAppConfig.ConnectionString; }
        }

        /// <summary>
        /// 数据提供器
        /// </summary>
        public static string DataProviderName
        {
            get { return CurrentAppConfig.DataProviderName; }
        }

        /// <summary>
        /// 是否启用了动态页面gzip压缩
        /// </summary>
        public static bool UseDynamicCompress
        {
            get { return CurrentAppConfig.DynamicCompress; }
        }

        /// <summary>
        /// 是否启用了静态页面gzip压缩
        /// </summary>
        public static bool UseStaticCompress
        {
            get { return CurrentAppConfig.StaticCompress; }
        }

        /// <summary>
        /// 安全字符串，避免被其他人猜到，长度为10
        /// </summary>
        public static string SafeString
        {
            get { return CurrentAppConfig.SafeString; }
        }

        /// <summary>
        /// 为了防止临时文件被猜到文件名，给临时文件起名的时候应该包含这个字符串。长度为6
        /// </summary>
        public static string FileNamePart
        {
            get { return CurrentAppConfig.FileNamePart; }
        }

        /// <summary>
        /// 商业授权许可证
        /// </summary>
        public static string Licence
        {
            get { return CurrentAppConfig.Licence; }
        }

        public static string[] LicencedServers
        {
            get { return CurrentAppConfig.LicencedServers; }
        }

        public static PassportClientConfig PassportClient
        {
            get { return CurrentAppConfig.PassportClient; }
        }

        #endregion

        #region 取系统目录的方法

        private static string Join(JoinType joinType, string str1, string str2)
        {
            if (joinType == JoinType.Path)
                return IOUtil.JoinPath(str1, str2);
            else if (joinType == JoinType.Url)
                return UrlUtil.JoinUrl(str1, str2);
            else if (joinType == JoinType.RelativeUrl)
            {
                if (StringUtil.StartsWith(str1, '/'))
                    return UrlUtil.JoinUrl("~", str1, str2);
                else
                    return UrlUtil.JoinUrl("~/", str1, str2);
            }
            return string.Empty;
        }

        private static string Join(JoinType joinType, string str1, string str2, string str3)
        {
            if (joinType == JoinType.Path)
                return IOUtil.JoinPath(str1, str2, str3);
            else if (joinType == JoinType.Url)
                return UrlUtil.JoinUrl(str1, str2, str3);
            else if (joinType == JoinType.RelativeUrl)
            {
                if (StringUtil.StartsWith(str1, '/'))
                    return UrlUtil.JoinUrl("~", str1, str2, str3);
                else
                    return UrlUtil.JoinUrl("~/", str1, str2, str3);
            }
            return string.Empty;
        }

        private static string Join(JoinType joinType, params string[] paths)
        {
            if (joinType == JoinType.Path)
                return IOUtil.JoinPath(paths);
            else if (joinType == JoinType.Url)
                return UrlUtil.JoinUrl(paths);
            else if (joinType == JoinType.RelativeUrl)
            {
                string temp = UrlUtil.JoinUrl(paths);
                return StringUtil.StartsWith(temp, '/') ? "~" + temp : "~/" + temp;
            }
            return string.Empty;
        }

        private static string GetPath(JoinType joinType, string root, SystemDirecotry systemDirecotry)
        {
            switch (systemDirecotry)
            {
                case SystemDirecotry.Root:
                    return root;

                case SystemDirecotry.Code:
                    return Join(joinType, root, SystemDirecotry_Code);

                case SystemDirecotry.Dialogs:
                    return Join(joinType, root, SystemDirecotry_Dialogs);

                case SystemDirecotry.Pages:
                    return Join(joinType, root, SystemDirecotry_Pages);

                case SystemDirecotry.Plugins:
                    return Join(joinType, root, SystemDirecotry_Plugins);

                case SystemDirecotry.Error:
                    return Join(joinType, root, SystemDirecotry_Error);

                case SystemDirecotry.Temp:
                    return Join(joinType, root, SystemDirecotry_Temp);

                case SystemDirecotry.Temp_Upload:
                    return Join(joinType, root, SystemDirecotry_Temp, SystemDirecotry_Temp_Upload);

                case SystemDirecotry.Temp_Avatar:
                    return Join(joinType, root, SystemDirecotry_Temp, SystemDirecotry_Temp_Avatar);

                case SystemDirecotry.Temp_ParsedTemplate:
                    return Join(joinType, root, SystemDirecotry_Temp, SystemDirecotry_Temp_ParsedTemplate);

                case SystemDirecotry.Upload:
                    //让用户上传目录支持虚拟目录的特殊处理
                    if (joinType == JoinType.Path)
                        return HostingEnvironment.MapPath("~/" + SystemDirecotry_Upload);
                    else
                        return Join(joinType, root, SystemDirecotry_Upload);

                case SystemDirecotry.Upload_Avatar:
                    //让用户上传目录支持虚拟目录的特殊处理
                    if (joinType == JoinType.Path)
                        return HostingEnvironment.MapPath(string.Concat("~/", SystemDirecotry_Upload, "/", SystemDirecotry_Upload_Avatar));
                    else
                        return Join(joinType, root, SystemDirecotry_Upload, SystemDirecotry_Upload_Avatar);

                case SystemDirecotry.Upload_Emoticons:
                    //让用户上传目录支持虚拟目录的特殊处理
                    if (joinType == JoinType.Path)
                        return HostingEnvironment.MapPath(string.Concat("~/", SystemDirecotry_Upload, "/", SystemDirecotry_Upload_Emoticon));
                    else
                        return Join(joinType, root, SystemDirecotry_Upload, SystemDirecotry_Upload_Emoticon);

                case SystemDirecotry.Upload_File:
                    //让用户上传目录支持虚拟目录的特殊处理
                    if (joinType == JoinType.Path)
                        return HostingEnvironment.MapPath(string.Concat("~/", SystemDirecotry_Upload, "/", SystemDirecotry_Upload_File));
                    else
                        return Join(joinType, root, SystemDirecotry_Upload, SystemDirecotry_Upload_File);

                case SystemDirecotry.Album_Thumbs:
                    //让用户上传目录支持虚拟目录的特殊处理
                    if (joinType == JoinType.Path)
                        return HostingEnvironment.MapPath(string.Concat("~/", SystemDirecotry_Upload, "/", SystemDirectory_Album_Thumbs));
                    else
                        return Join(joinType, root, SystemDirecotry_Upload, SystemDirectory_Album_Thumbs);

                    //return Join(joinType, root, SystemDirecotry_Upload, SystemDirectory_Album_Thumbs);

                case SystemDirecotry.Admin:
                    return Join(joinType, root, SystemDirecotry_Admin);

                case SystemDirecotry.Assets:
                    return Join(joinType, root, SystemDirecotry_Assets);

                case SystemDirecotry.Assets_Judgement:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_judgement);

                case SystemDirecotry.Assets_LinkIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_LinkIcon);

                case SystemDirecotry.Assets_OnlineIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_OnlineIcon);

                case SystemDirecotry.Assets_RoleIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_RoleIcon);

                case SystemDirecotry.Assets_ZodiacIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_ZodiacIcon);

                case SystemDirecotry.Assets_PostIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_Post);

                case SystemDirecotry.Assets_Face:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_Face);

                case SystemDirecotry.Assets_IconStar:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_IconStar);

                case SystemDirecotry.Assets_Icon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_Icon);

                case SystemDirecotry.Skins:
                    return Join(joinType, root, SystemDirecotry_Themes);

                case SystemDirecotry.SpaceStyles:
                    return Join(joinType, root, SystemDirecotry_SpaceStyles);

                case SystemDirecotry.Assets_ForumLogo:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_ForumLogo);

                case SystemDirecotry.Assets_MedalIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_MedalIcon);

                case SystemDirecotry.Assets_PointIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_PointIcon);

                case SystemDirecotry.Assets_MissionIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_MissionIcon);

                case SystemDirecotry.Assets_FileIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirectory_Assets_FileIcon);

                case SystemDirecotry.Assets_Images:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_Images);

                case SystemDirecotry.Assets_AdvertIcon:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_AdvertIcon);

                case SystemDirecotry.Assets_Sounds:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_Sounds);
                case SystemDirecotry.Assets_Flash:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirecotry_Assets_Flash);
                case SystemDirecotry.Assets_PropIcons:
                    return Join(joinType, root, SystemDirecotry_Assets, SystemDirectory_Assets_PropIcons);

                case SystemDirecotry.Js:
                    return Join(joinType, root, SystemDirectory_Js);

                case SystemDirecotry.Upload_IDCard:
                    return Join(joinType, root, SystemDirecotry_Upload, SystemDirecotry_Upload_IDCard);
                default:
                    return root;
            }
        }

        /// <summary>
        /// 返回指定系统目录的物理路径
        /// </summary>
        /// <param name="systemDirecotry"></param>
        /// <returns></returns>
        public static string GetPath(SystemDirecotry systemDirecotry)
        {
            return GetPath(JoinType.Path, s_ApplicationPath, systemDirecotry);
        }

        /// <summary>
        /// 返回指定系统目录的物理路径
        /// </summary>
        /// <param name="systemDirecotry"></param>
        /// <param name="addPath"></param>
        /// <returns></returns>
        public static string GetPath(SystemDirecotry systemDirecotry, string addPath)
        {
            string basePath = GetPath(JoinType.Path, s_ApplicationPath, systemDirecotry);
            return IOUtil.JoinPath(basePath, addPath);
        }

        /// <summary>
        /// 返回指定系统目录的物理路径
        /// </summary>
        /// <param name="systemDirecotry"></param>
        /// <param name="addPath"></param>
        /// <returns></returns>
        public static string GetPath(SystemDirecotry systemDirecotry, string addPath1, string addPath2)
        {
            string basePath = GetPath(JoinType.Path, s_ApplicationPath, systemDirecotry);
            return IOUtil.JoinPath(basePath, addPath1, addPath2);
        }

        /// <summary>
        /// 返回带～的虚拟路径
        /// </summary>
        /// <param name="direcotry"></param>
        /// <returns></returns>
        public static string GetRelativeUrl(SystemDirecotry direcotry)
        {
            return GetPath(JoinType.RelativeUrl, string.Empty, direcotry);

        }

        /// <summary>
        /// 返回带～的虚拟路径
        /// </summary>
        /// <param name="direcotry"></param>
        /// <returns></returns>
        public static string GetRelativeUrl(SystemDirecotry direcotry, string addPath)
        {
            string basePath = GetPath(JoinType.RelativeUrl, string.Empty, direcotry);
            return UrlUtil.JoinUrl(basePath, addPath);
        }

        /// <summary>
        /// 返回带～的虚拟路径
        /// </summary>
        /// <param name="direcotry"></param>
        /// <returns></returns>
        public static string GetRelativeUrl(SystemDirecotry direcotry, string addPath1, string addPath2)
        {
            string basePath = GetPath(JoinType.RelativeUrl, string.Empty, direcotry);
            return UrlUtil.JoinUrl(basePath, addPath1, addPath2);
        }

        /// <summary>
        /// 返回指定系统目录的虚拟路径
        /// </summary>
        /// <param name="systemDirecotry"></param>
        /// <returns></returns>
        public static string GetVirtualPath(SystemDirecotry systemDirecotry)
        {
            return GetPath(JoinType.Url, s_AppRoot, systemDirecotry);
        }

        /// <summary>
        /// 返回指定系统目录的虚拟路径
        /// </summary>
        /// <param name="systemDirecotry"></param>
        /// <returns></returns>
        public static string GetVirtualPath(SystemDirecotry systemDirecotry, string addPath)
        {
            string basePath = GetPath(JoinType.Url, s_AppRoot, systemDirecotry);
            return UrlUtil.JoinUrl(basePath, addPath);
        }

        /// <summary>
        /// 返回指定系统目录的虚拟路径
        /// </summary>
        /// <param name="systemDirecotry"></param>
        /// <returns></returns>
        public static string GetVirtualPath(SystemDirecotry systemDirecotry, string addPath1, string addPath2)
        {
            string basePath = GetPath(JoinType.Url, s_AppRoot, systemDirecotry);
            return UrlUtil.JoinUrl(basePath, addPath1, addPath2);
        }

        #endregion

        #region 取系统目录的属性

        public static string BinDirectory
        {
            get { return s_BinDirectory; }
        }

        /// <summary>
        /// 应用程序物理根路径
        /// </summary>
        public static string ApplicationPath
        {
            get { return s_ApplicationPath; }
            internal set
            {
                s_ApplicationPath = value;
                s_BinDirectory = IOUtil.JoinPath(s_ApplicationPath, "bin");
            }
        }


        /// <summary>
        /// 获取不带任何目录和文件名的当前站点的首页地址，始终以http://或https://开头，如http://www.bbsmax.com
        /// </summary>
        public static string SiteRoot
        {
            get
            {
                HttpContext context = HttpContext.Current;
                return context.Request.Url.Scheme + "://" + context.Request.Url.Authority;
            }
        }

        /// <summary>
        /// 获取本程序安装的虚拟路径，始终不以/结尾。如果安装在根目录，将返回空字符串
        /// </summary>
        public static string AppRoot
        {
            get { return s_AppRoot; }
        }

        /// <summary>
        /// 获取本程序安装的完整网址，始终以http://或https://开头，如http://www.abc.com/bbs
        /// </summary>
        public static string FullAppRoot
        {
            get
            {
                HttpContext context = HttpContext.Current;
                return context.Request.Url.Scheme + "://" + context.Request.Url.Authority + AppRoot;
            }
        }

        #endregion

        private static string ajaxSpliter = null;
        private static DateTime lastUpdateAjaxSpliter = DateTime.Now;

        [Obsolete]
        public static string AjaxSpliter
        {
            get
            {
                if (string.IsNullOrEmpty(ajaxSpliter) || lastUpdateAjaxSpliter.AddSeconds(3) < DateTime.Now)
                {
                    ajaxSpliter = Guid.NewGuid().ToString("B");
                    lastUpdateAjaxSpliter = DateTime.Now;
                }
                return ajaxSpliter;
            }
        }


#if DEBUG

        public static void LogRunInfo(string info)
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
                return;

            string dir = IOUtil.JoinPath(ApplicationPath, "max-temp\\run");

            string filePath = IOUtil.JoinPath(dir, "log.txt");

            if (context.Items["RunInfo"] == null)
            {
                try
                {
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                    else if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch { }
                context.Items["RunInfo"] = true;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    writer.WriteLine(info);
                    writer.Close();
                }
            }
            catch { }

        }

#endif

        private enum JoinType
        {
            /// <summary>
            /// 物理路径
            /// </summary>
            Path,

            /// <summary>
            /// 网站访问路径
            /// </summary>
            Url,

            /// <summary>
            /// 相对路径 带~的
            /// </summary>
            RelativeUrl
        }

    }

    public enum SystemDirecotry : byte
    {
        Root,

        Error,

        Admin,

        Assets,

        Assets_OnlineIcon,

        Assets_RoleIcon,

        Assets_ZodiacIcon,

        Assets_LinkIcon,

        Assets_Judgement,

        Assets_PostIcon,

        Assets_Face,

        Assets_IconStar,

        Assets_Icon,

        Assets_AdvertIcon,

        Assets_FileIcon,

        Assets_Sounds,

        Assets_Flash,

        Temp,

        Temp_Avatar,

        Temp_Upload,

        Temp_ParsedTemplate,

        Code,

        Pages,

        Dialogs,

        Plugins,

        Assets_ForumLogo,

        Assets_MedalIcon,

        Assets_PointIcon,

        Assets_MissionIcon,

        Upload,

        Upload_Avatar,

        Upload_Emoticons,

        Upload_File,

        Upload_IDCard,

        Skins,

        SpaceStyles,

        Album_Thumbs,

        Assets_Images,

        Assets_PropIcons,

        Js,
    }
}