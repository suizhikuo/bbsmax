//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Reflection;
using System.Configuration;
using System.Web.Compilation;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Caching;

using MaxLabs.bbsMax;
using MaxLabs.bbsMax.RegExp;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using ICSharpCode.SharpZipLib.Zip;
using System.Xml;

namespace MaxLabs.WebEngine.Template
{
    /// <summary>
    /// 模板管理器
    /// </summary>
    public static class TemplateManager
    {
        private static bool s_Initialized = false;
        private static object s_InitLocker = new object();
        private static object s_LoadSkinLocker = new object();
        private static Dictionary<string, TemplateFile> s_TemplateFiles = null;
        private static CacheDependency s_TemplateFilesWatcher = null;

        private static CacheDependency s_SkinsWatcher;
        private static SkinCollection s_AllSkins;
        private static SkinCollection s_EnabledSkins;

        //private const string SkinedPath = "~/max-templates/default/";

        /// <summary>
        /// 初始化模板管理器，加载模板成员和模板文件夹内容等。在开发模式下（!Publish），不管withAdminAndDialogs的值如何，都会加载Admin和Dialogs
        /// </summary>
        public static void Init(bool withAdminAndDialogs)
        {
            if (s_Initialized)
                return;

            lock (s_InitLocker)
            {
                if (s_Initialized)
                    return;

                s_Initialized = true;

                //LoadSkins();

                s_TemplateFiles = ImportAllTemplateFiles(withAdminAndDialogs, out s_TemplateFilesWatcher);
            }
        }

        #region 皮肤管理相关

        public static SkinCollection GetAllSkins()
        {
            SkinCollection allSkins = s_AllSkins;//new SkinCollection();
            CacheDependency skinsWatcher = s_SkinsWatcher;

            if (allSkins == null || skinsWatcher == null || skinsWatcher.HasChanged)
            {
                lock (s_LoadSkinLocker)
                {
                    allSkins = s_AllSkins;
                    skinsWatcher = s_SkinsWatcher;

                    if (allSkins == null || skinsWatcher == null || skinsWatcher.HasChanged)
                    {
                        allSkins = new SkinCollection();

                        //SkinCollection enabledSkins = new SkinCollection();

                        string skinsPath = Globals.GetPath(SystemDirecotry.Skins);
                        //获得保存主题的目录实体
                        DirectoryInfo skinsDirectory = new DirectoryInfo(skinsPath);

                        //所有主题的目录集合
                        DirectoryInfo[] skinDirectorys = skinsDirectory.GetDirectories();

                        List<string> watcher = new List<string>();

                        //string[] tempFileNames = new string[skinDirectorys.Length];
                        //int index = 0;

                        //遍历所有主题的目录集合，把存在Theme.Config文件的主题添加到主题列表中
                        foreach (DirectoryInfo skinDirectory in skinDirectorys)
                        {
                            string file = IOUtil.JoinPath(skinDirectory.FullName, MaxLabs.bbsMax.Consts.SkinConfigFile);

                            if (File.Exists(file) == true)
                            {
                                Skin skin = new Skin();
                                skin.SkinID = skinDirectory.Name;
                                skin.Name = skin.SkinID;

                                //读取_skin.config配置文件
                                XmlDocument doc = new XmlDocument();

                                try
                                {
                                    doc.Load(file);

                                    #region 循环取 Skin 的属性

                                    foreach (XmlNode configNode in doc.DocumentElement.ChildNodes)
                                    {
                                        if (configNode.NodeType != XmlNodeType.Comment)
                                        {
                                            switch (configNode.Name.ToLower())
                                            {
                                                case "name":
                                                    skin.Name = configNode.InnerText.Trim();
                                                    break;
                                                case "version":
                                                    skin.Version = configNode.InnerText.Trim();
                                                    break;
                                                case "publishdate":
                                                    try
                                                    {
                                                        skin.PublishDate = DateTime.Parse(configNode.InnerText.Trim());
                                                    }
                                                    catch
                                                    {
                                                        skin.PublishDate = DateTime.MinValue;
                                                    }
                                                    break;
                                                case "author":
                                                    skin.Author = configNode.InnerText.Trim();
                                                    break;
                                                case "website":
                                                    skin.WebSite = configNode.InnerText.Trim();
                                                    break;

                                                case "description":
                                                    skin.Description = configNode.InnerText.Trim();
                                                    break;

                                                case "skinbase":
                                                    skin.SkinBase = configNode.InnerText.Trim();
                                                    break;
                                            }
                                        }
                                    }

                                    #endregion

                                }
                                catch { }

                                allSkins.Add(skin);

                                //if (skin.Enabled)
                                //    enabledSkins.Add(skin);

                                watcher.Add(file);

                                //tempFileNames[index] = file;
                                //index++;
                            }
                            else
                                watcher.Add(skinDirectory.FullName);
                        }

                        watcher.Add(skinsPath);

                        s_AllSkins = allSkins;
                        s_EnabledSkins = null;
                        s_SkinsWatcher = new CacheDependency(watcher.ToArray());
                    }
                }
            }

            if (allSkins == null || allSkins.Count == 0 || allSkins.ContainsKey(MaxLabs.bbsMax.Consts.DefaultSkinID) == false)
            {
                throw new Exception("载入默认模板失败。默认模板位于“max-templates/default”目录下，该目录不能删除。");
            }

            return allSkins;
        }

        /// <summary>
        /// 获取所有已经启用的皮肤
        /// </summary>
        /// <returns></returns>
        public static SkinCollection GetEnabledSkins()
        {
            SkinCollection enabledSkins = s_EnabledSkins;
            CacheDependency skinsWatcher = s_SkinsWatcher;

            if (enabledSkins == null || skinsWatcher == null || skinsWatcher.HasChanged)
            {
                enabledSkins = new SkinCollection();

                foreach (Skin skin in GetAllSkins())
                {
                    if (skin.Enabled)
                        enabledSkins.Add(skin);
                }

                s_EnabledSkins = enabledSkins;
            }

            return enabledSkins;
        }

        /// <summary>
        /// 获取指定名称的系统皮肤，如果不存在将返回null
        /// </summary>
        /// <param name="skinID"></param>
        /// <returns></returns>
        public static Skin GetSkin(string skinID)
        {
            return GetSkin(skinID, false);
        }

        /// <summary>
        /// 获取指定名称的系统皮肤，且可以指定是否包含已经禁用的皮肤，如果不存在将返回null
        /// </summary>
        /// <param name="themeID"></param>
        /// <returns></returns>
        public static Skin GetSkin(string skinID, bool includeDisabledSkin)
        {
            if (string.IsNullOrEmpty(skinID))
                return null;

            Skin skin;// = GetSkins()[skinID];

            if (includeDisabledSkin)
            {
                if (GetAllSkins().TryGetValue(skinID, out skin) == false)
                    return null;
            }
            else
            {
                if (GetEnabledSkins().TryGetValue(skinID, out skin) == false)
                    return null;
            }

            return skin;
        }

        public static void ClearSkinCache()
        {
            s_EnabledSkins = null;
        }

        #endregion

        #region 模板文件对象的管理

        /// <summary>
        /// 导入指定目录下的模板文件
        /// </summary>
        /// <param name="templateFiles"></param>
        /// <param name="watchDirectories"></param>
        /// <param name="directoryName"></param>
        private static void ImportTemplateFiles(Dictionary<string, TemplateFile> templateFiles, List<string> watchDirectories, string directoryName)
        {
            ImportTemplateFiles(templateFiles, watchDirectories, directoryName, null);
        }

        /// <summary>
        /// 导入指定目录下的模板文件为默认模板，并指定哪个目录为他的扩展模板（可以覆盖掉默认模板同名文件的内容）
        /// </summary>
        /// <param name="templateFiles"></param>
        /// <param name="watchDirectories"></param>
        /// <param name="directoryName"></param>
        /// <param name="skinDirectory"></param>
        private static void ImportTemplateFiles(Dictionary<string, TemplateFile> templateFiles, List<string> watchDirectories, string directoryName, string skinDirectory)
        {

            string path = IOUtil.JoinPath(Globals.ApplicationPath, directoryName);

            string[] files = Directory.GetFiles(path, "*.as?x", SearchOption.AllDirectories);

            string[] watcherPaths;

            //如果不支持模板，或者支持模板但默认的模板就在所有模板目录内，则跳过对默认模板目录的单独监视
            if (string.IsNullOrEmpty(skinDirectory)
                ||
                (string.IsNullOrEmpty(skinDirectory) == false && directoryName.StartsWith(skinDirectory, StringComparison.OrdinalIgnoreCase) == false)
                )
            {
                watcherPaths = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                watchDirectories.Add(path);
                foreach (string p in watcherPaths)
                {
                    if (StringUtil.EndsWithIgnoreCase(p, "\\.svn") || StringUtil.ContainsIgnoreCase(p, "\\.svn\\"))
                        continue;

                    watchDirectories.Add(p);
                }
            }

            string[] skins = null;

            //支持模板
            if (string.IsNullOrEmpty(skinDirectory) == false)
            {
                skinDirectory = IOUtil.JoinPath(Globals.ApplicationPath, skinDirectory);

                if (Directory.Exists(skinDirectory))
                {
                    skins = Directory.GetDirectories(skinDirectory);

                    watcherPaths = Directory.GetDirectories(skinDirectory, "*", SearchOption.AllDirectories);

                    watchDirectories.Add(skinDirectory);
                    foreach (string p in watcherPaths)
                    {
                        if (StringUtil.EndsWithIgnoreCase(p, "\\.svn") || StringUtil.ContainsIgnoreCase(p, "\\.svn\\"))
                            continue;

                        watchDirectories.Add(p);
                    }
                }
                else
                    skins = new string[0];
            }

            foreach (string file in files)
            {
                if (StringUtil.ContainsIgnoreCase(file, "\\.svn\\"))
                    continue;

                TemplateFile templateFile = new TemplateFile(file);
                templateFiles.Add(templateFile.VirtualPath, templateFile);

                if (skinDirectory != null)
                {
                    foreach (string skin in skins)
                    {
                        if (StringUtil.EqualsIgnoreCase(MaxLabs.bbsMax.Consts.DefaultSkinID, Path.GetFileName(skin)))
                            continue;

                        string partFileName = file.Substring(path.Length);

                        string skinFileName = IOUtil.JoinPath(skin, partFileName);

                        TemplateFile skinFile = new TemplateFile(skinFileName, templateFile);

                        templateFile.AddSkin(Path.GetFileName(skin), skinFile);

                        templateFiles.Add(skinFile.VirtualPath, skinFile);
                    }
                }
            }

            if (skinDirectory != null)
            {
                foreach (string skin in skins)
                {
                    if (StringUtil.EqualsIgnoreCase(MaxLabs.bbsMax.Consts.DefaultSkinID, Path.GetFileName(skin)))
                        continue;

                    string[] skinFiles = Directory.GetFiles(skin, "*.as?x", SearchOption.AllDirectories);

                    foreach (string file in skinFiles)
                    {
                        if (StringUtil.ContainsIgnoreCase(file, "\\.svn\\"))
                            continue;

                        TemplateFile skinFile = new TemplateFile(file);

                        if (templateFiles.ContainsKey(skinFile.VirtualPath) == false)
                            templateFiles.Add(skinFile.VirtualPath, skinFile);
                    }
                }
            }
        }

        private static Dictionary<string, TemplateFile> ImportAllTemplateFiles(bool withAdminAndDialogs, out CacheDependency watcher)
        {
            Dictionary<string, TemplateFile> result = new Dictionary<string, TemplateFile>(StringComparer.OrdinalIgnoreCase);
            List<string> dirs = new List<string>();

            ImportTemplateFiles(result, dirs, "max-templates\\" + MaxLabs.bbsMax.Consts.DefaultSkinID, "max-templates");
#if Publish 
            if (withAdminAndDialogs)
            {
                ImportTemplateFiles(result, dirs, "max-dialogs");
			    ImportTemplateFiles(result, dirs, "max-admin");

                ImportTemplateFiles(result, dirs, "archiver");
            }
#else
            ImportTemplateFiles(result, dirs, "max-dialogs");
            ImportTemplateFiles(result, dirs, "max-admin");

            ImportTemplateFiles(result, dirs, "archiver");
#endif
            ImportTemplateFiles(result, dirs, "max-plugins");
            ImportTemplateFiles(result, dirs, "max-js");

            watcher = new CacheDependency(dirs.ToArray());

            return result;
        }

        private static object s_GetTemplateFilesLock = new object();

        public static Dictionary<string, TemplateFile> GetTemplateFiles()
        {
            Dictionary<string, TemplateFile> result = s_TemplateFiles;
            CacheDependency watcher = s_TemplateFilesWatcher;

            if (result == null || watcher == null || watcher.HasChanged)
            {
                lock (s_GetTemplateFilesLock)
                {
                    result = s_TemplateFiles;
                    watcher = s_TemplateFilesWatcher;

                    if (result == null || watcher == null || watcher.HasChanged)
                    {
                        Dictionary<string, TemplateFile> newResult = ImportAllTemplateFiles(false, out watcher);

                        //if (IsKeysSame(result, newResult) == false)
                        //{
                        LogHelper.CreateErrorLog(null, "TemplateFiles列表重建");

                        s_TemplateFiles = newResult;
                        result = newResult;
                        //}

                        //else
                        //    LogHelper.CreateErrorLog(null, "TemplateFiles列表重读，但因为列表一致没有重新创建");

                        s_TemplateFilesWatcher = watcher;
                    }
                }
            }

            return result;
        }

        private static bool IsKeysSame(Dictionary<string, TemplateFile> list1, Dictionary<string, TemplateFile> list2)
        {
            if (list1 == null && list2 == null)
                return true;

            else if (list1 == null || list2 == null)
                return false;

            else if (list1.Count != list2.Count)
                return false;

            foreach (KeyValuePair<string, TemplateFile> item in list1)
            {
                if (list2.ContainsKey(item.Key) == false)
                    return false;
            }

            return true;
        }

        public static bool IsTemplateFile(string virtualPath)
        {
            return (GetTemplateFiles().ContainsKey(virtualPath));
        }

        private static TemplateFile GetTemplateFile(string virtualPath)
        {
            TemplateFile templateFile;

            if (GetTemplateFiles().TryGetValue(virtualPath, out templateFile) == false)
                return null;

            return templateFile;
        }

        #endregion

        #region 解析模板相关

        /// <summary>
        /// 解析指定路径下的模板文件，并返回模板解析结果的路径，如果所给的路径不是模板文件路径，则返回原路径
        /// </summary>
        /// <param name="virtualPath">模板文件的虚拟路径，支持.aspx和.ascx文件</param>
        /// <returns></returns>
        public static string ParseTemplate(string path)
        {
            string virtualPath = null;
            string queryString = null;

            int queryStart = path.IndexOf('?');

            if (queryStart >= 0)
            {
                virtualPath = path.Substring(0, queryStart);
                queryString = path.Substring(queryStart);
            }
            else
            {
                virtualPath = path;
            }

            //if (virtualPath.StartsWith(SkinedPath, StringComparison.OrdinalIgnoreCase))
            //{
            //    string skinID = User.Current.SkinID;

            //    virtualPath = "~/max-templates/default/" + skinID + virtualPath.Substring(1);
            //}

            TemplateFile templateFile = GetTemplateFile(virtualPath);

            if (templateFile == null)
                return path;//virtualPath + queryString;

            Skin skin = s_IsPreParsing == false ? Context.Current.Skin : s_PreParseSkin;

            bool useSkin;
            string skinID;

            if (skin == null)
            {
                useSkin = false;
                skinID = MaxLabs.bbsMax.Consts.DefaultSkinID;
            }
            else
            {
                useSkin = skin.SkinID != MaxLabs.bbsMax.Consts.DefaultSkinID;
                skinID = skin.SkinID;
            }

            if (useSkin)
                templateFile = templateFile.GetSkin(skin.SkinID);

            if (templateFile.IsParsed == false)
            {
                lock (templateFile)
                {
                    if (templateFile.IsParsed == false)
                    {
                        using (TemplateParser parser = new TemplateParser())
                        {

                            //string forumPath = "~/max-templates/" + skinID + "/forums/";

                            ////UrlFormat urlFormat = s_IsPreParsing ? s_PreParseUrlFormat : AllSettings.Current.FriendlyUrlSettings.UrlFormat;

                            //if (templateFile.VirtualPath.StartsWith(forumPath, StringComparison.OrdinalIgnoreCase))
                            //{
                            //    parser.GenerateV30AspxCode(skinID, templateFile, Config.Current.TemplateImports);
                            //}
                            //else
                            //{
                                parser.GenerateAspxCode(skinID, templateFile, Config.Current.TemplateImports);
                            //}

                            templateFile.IsParsed = true;
                        }
                    }
                }
            }

            return templateFile.ParsedFileVirtualPath + queryString;
        }

        #endregion

        #region 预解析模板相关

        private static bool s_IsPreParsing;

        internal static bool IsPreParsing
        {
            get { return s_IsPreParsing; }
        }

        private static Skin s_PreParseSkin;

        //private static UrlFormat s_PreParseUrlFormat;

        private static object s_PreParseLock = new object();

        public static void PreParseTemplates(bool withAdminAndDialog, bool withUserTemplate, EventHandler callback)
        {
            lock (s_PreParseLock)
            {
                Config.Current = new WebEngineConfig();
                TemplateManager.Init(withAdminAndDialog);

                s_IsPreParsing = true;

                if (Directory.Exists(Globals.GetPath(SystemDirecotry.Temp_ParsedTemplate)))
                    Directory.Delete(Globals.GetPath(SystemDirecotry.Temp_ParsedTemplate), true);

                if (withAdminAndDialog && withUserTemplate)
                {
                    //archiver
                    PreParseTemplates(false, "archiver", 0, 25, callback);
                    PreParseTemplates(false, "max-admin", 0, 25, callback);
                    PreParseTemplates(false, "max-dialogs", 25, 50, callback);
                    PreParseTemplates(false, "max-plugins", 50, 60, callback);
                    PreParseTemplates(false, "max-js", 60, 70, callback);
                    PreParseTemplates(true, "max-templates/default", 70, 100, callback);
                }
                else if (withAdminAndDialog)
                {
                    PreParseTemplates(false, "archiver", 0, 25, callback);
                    PreParseTemplates(false, "max-admin", 0, 50, callback);
                    PreParseTemplates(false, "max-dialogs", 50, 100, callback);
                }
                else if (withUserTemplate)
                {
                    PreParseTemplates(false, "max-plugins", 0, 20, callback);
                    PreParseTemplates(false, "max-js", 20, 30, callback);
                    PreParseTemplates(true, "max-templates/default", 30, 100, callback);
                }
                else
                    throw new ArgumentException("请至少编译一种类型的模板");

                s_IsPreParsing = false;
                s_PreParseSkin = null;
            }
        }

        private static void PreParseTemplates(bool supportSkin, string path, int startPercent, int endPercent, EventHandler callback)
        {
            string rootPath = Globals.GetPath(SystemDirecotry.Root);
            string pagesPath = IOUtil.JoinPath(rootPath, path);

            string[] files = Directory.GetFiles(pagesPath, "*.as?x", SearchOption.AllDirectories);

            int totalFiles = files.Length;

            int i = 0;
            foreach (string file in files)
            {
                i++;

                string virtualPath = "~" + file.Substring(rootPath.Length).Replace("\\", "/");

                if (Path.GetFileNameWithoutExtension(file).StartsWith("_") == false && Path.GetFileNameWithoutExtension(file).EndsWith("_") == false)
                    PreParseTemplate(supportSkin, virtualPath);

                else
                {
                    string codePath;

                    if (supportSkin)
                    {
                        int index = file.IndexOf("\\", pagesPath.Length);
                        codePath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Code), file.Substring(index + 1) + ".cs");
                    }
                    else
                        codePath = file + ".cs";


                    //string subPath = file.Substring(pagesPath.Length);
                    //string codeFilePath = IOUtil.JoinPath(maxPagesCodePath, subPath + ".cs");

                    if (File.Exists(codePath))
                        PreParseTemplate(supportSkin, virtualPath);
                    else
                    {
                        string head = string.Empty;

                        using (StreamReader reader = new StreamReader(file))
                            head = reader.ReadLine();

                        if (StringUtil.StartsWithIgnoreCase(head, "<!--[page "))
                            PreParseTemplate(supportSkin, virtualPath);
                    }
                }

                callback((i * (endPercent - startPercent) / totalFiles + startPercent).ToString() + "|解析文件 " + virtualPath, null);
            }

            callback("100|解析文件完成", null);
        }

        private static void PreParseTemplate(bool supportSkin, string path)
        {
            if (supportSkin)
            {
                foreach (Skin skin in TemplateManager.GetAllSkins())
                {
                    s_PreParseSkin = skin;

                    ParseTemplate(path);
                }
            }
            else
                ParseTemplate(path);
        }

        #endregion

        #region 模板的导入导出

        private static object s_SkinImportExportLocker = new object();

        public static string ExportSkin(string skinID)
        {
            lock (s_SkinImportExportLocker)
            {
                string skinPath = Globals.GetPath(SystemDirecotry.Skins, skinID);

                DateTime now = DateTimeUtil.Now;

                string skinPackagePath = IOUtil.JoinPath(
                    Globals.GetPath(SystemDirecotry.Skins),
                    string.Format(
                        "{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}.zip",
                        skinID,
                        now.Year,
                        now.Month,
                        now.Day,
                        now.Hour,
                        now.Minute,
                        now.Second,
                        new Random().Next(0, int.MaxValue) % 1000
                    )
                );

                if (File.Exists(skinPackagePath))
                    File.Delete(skinPackagePath);

                FastZip fastZip = new FastZip();

                fastZip.CreateEmptyDirectories = true;
                fastZip.CreateZip(skinPackagePath, skinPath, true, "");

                return skinPackagePath;
            }
        }

        public static void ImportSkin(string skinPackagePath)
        {
            lock (s_SkinImportExportLocker)
            {
                string skinPath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), Path.GetFileNameWithoutExtension(skinPackagePath));

                FastZip fastZip = new FastZip();

                fastZip.CreateEmptyDirectories = true;
                fastZip.ExtractZip(skinPackagePath, skinPath, "");
            }
        }

        #endregion

        #region 在html输出到浏览器之前对html进行的处理

        private static readonly Regex s_FormRegex = new FormRegex();

        private static readonly Regex s_CheckBoxRegex = new CheckBoxRegex();

        private static readonly Regex s_CheckBoxNameRegex = new CheckBoxNameRegex();

        private static readonly Regex s_HtmlEndBodyRegex = new HtmlEndBodyRegex();

        public static StringBuilder ProcessExecuteResult(StringBuilder html)
        {
            MatchCollection matchForms = s_FormRegex.Matches(html.ToString());

            int index = 0;
            foreach (Match matchForm in matchForms)
            {
                MatchCollection matchCheckBoxs = s_CheckBoxRegex.Matches(matchForm.Groups[2].Value);

                List<string> checkBoxNames = new List<string>();

                foreach (Match matchCheckBox in matchCheckBoxs)
                {
                    //if (s_CheckBoxNameRegex.IsMatch(matchCheckBox.Value))
                    //{
                        Match m = s_CheckBoxNameRegex.Match(matchCheckBox.Value);

                        if (m != null && m.Success)
                        {
                            if (!checkBoxNames.Contains(m.Groups[1].Value))
                                checkBoxNames.Add(m.Groups[1].Value);
                        }
                    //}
                }

                if (checkBoxNames.Count > 0)
                {
                    StringBuilder valueBuilder = new StringBuilder();

                    bool isFirst = true;

                    foreach (string name in checkBoxNames)
                    {
                        if (isFirst)
                            isFirst = false;
                        else
                            valueBuilder.Append("#");

                        valueBuilder.Append(name);
                    }

                    string hiddenInput = string.Concat("<input type=\"hidden\" name=\"", Consts.TemplateInputViewstate, "\" value=\"", SecurityUtil.Base64Encode(valueBuilder.ToString()), "\" />");

                    html.Insert(matchForm.Groups[2].Index + index, hiddenInput);

                    index += hiddenInput.Length;

                }

            }

            //TODO : 应该改为使用ajax请求，此处不予处理
//            if (MaxLabs.bbsMax.Jobs.JobManager.IsAfterRequestJobsExecuteTime())
//            {
//                string iframe = string.Concat(@"
//<iframe height=""0"" width=""0"" style=""display:none"" src=""", BbsRouter.GetUrl(MaxLabs.bbsMax.Consts.Handler_JobUrl), @"""></iframe>
//");

//                Match matchEndBody = s_HtmlEndBodyRegex.Match(html.ToString());

//                if (matchEndBody != null)
//                {
//                    html.Insert(matchEndBody.Index, iframe);
//                }
//            }

            return html;
        }

        #endregion

    }
}