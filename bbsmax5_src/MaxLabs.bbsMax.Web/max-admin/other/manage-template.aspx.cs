//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.WebEngine.Template;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_template : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_Template; }
        }

        public void Page_Load()
        {
            string skin = _Request.Get("skin");
            string file = FilePath;
            string version = _Request.Get("history");

            if (skin != null && file != null)
            {
                if (skin.IndexOf("..") >= 0
                    || skin.IndexOf('/') >= 0
                    || skin.IndexOf('\\') >= 0
                    || file.IndexOf("..") >= 0
                    || (StringUtil.EqualsIgnoreCase(file, "\\_skin.config") == false
                        && false == (
                            file.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase)
                            || file.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)
                            || file.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                            || file.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                            || file.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
                        )
                    ))
                    ShowError("关键参数不在正常范围内");

                string path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), skin, file);

                if (File.Exists(path))
                {
                    string skinRoot = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), skin);

                    string historyDir = GetBackupFileDir(skin, file);

                    m_BackupFileList = GetBackupFileInfos(historyDir);

                    if(_Request.IsClick("save"))
                    {
                        FileInfo fileInfo = new FileInfo(path);

                        if (fileInfo.IsReadOnly)
                            fileInfo.IsReadOnly = false;

                        if (Directory.Exists(historyDir) == false)
                            Directory.CreateDirectory(historyDir);

                        if (m_BackupFileList.Length == 0)
                            File.Copy(path, IOUtil.JoinPath(historyDir, "1.config"));
                        else
                            File.Copy(path, IOUtil.JoinPath(historyDir, (m_BackupFileList[0].Version + 1) + ".config"));

                        m_FileContent = _Request.Get("FileContent", Method.Post, m_FileContent, false);

                        File.WriteAllText(path, m_FileContent, System.Text.Encoding.UTF8);

                        m_FileContent = StringUtil.HtmlEncode(m_FileContent);

                        BbsRouter.JumpToCurrentUrl("skin=" + skin + "&file=" + FilePath);
                    }

                    m_FileContent = File.ReadAllText(path, System.Text.Encoding.Default);

                    m_FileContent = StringUtil.HtmlEncode(m_FileContent);

                    if (version != null)
                    {
                        if (version.IndexOf(',') < 0)
                        {
                            int ver2 = int.Parse(version);

                            BackupFileInfo file2 = null;

                            foreach (BackupFileInfo backupFile in m_BackupFileList)
                            {
                                if (backupFile.Version == ver2)
                                {
                                    file2 = backupFile;
                                    break;
                                }
                            }

                            m_FileContent2 = File.ReadAllText(file2.FullName, System.Text.Encoding.Default);
                            m_FileContent2 = StringUtil.HtmlEncode(m_FileContent2);
                        }
                        else
                        {
                            string[] vers = version.Split(',');

                            int ver1 = int.Parse(vers[0]);
                            int ver2 = int.Parse(vers[1]);

                            BackupFileInfo file1 = null;
                            BackupFileInfo file2 = null;

                            foreach (BackupFileInfo backupFile in m_BackupFileList)
                            {
                                if (backupFile.Version == ver1)
                                    file1 = backupFile;
                                else if (backupFile.Version == ver2)
                                    file2 = backupFile;

                                if (file1 != null && file2 != null)
                                    break;
                            }

                            m_FileContent = File.ReadAllText(file1.FullName, System.Text.Encoding.Default);
                            m_FileContent = StringUtil.HtmlEncode(m_FileContent);
                            m_FileContent2 = File.ReadAllText(file2.FullName, System.Text.Encoding.Default);
                            m_FileContent2 = StringUtil.HtmlEncode(m_FileContent2);
                        }
                    }
                }
            }
        }

        private string GetBackupFileDir(string skin, string subPath)
        {
            return IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Temp), "__模板编辑历史记录__", skin, subPath);
        }

        public class BackupFileInfo : IComparable<BackupFileInfo>
        {
            public BackupFileInfo(FileInfo fileInfo)
            {
                m_FileInfo = fileInfo;
            }

            private FileInfo m_FileInfo;

            private int? m_Version;

            public int Version
            {
                get 
                {
                    if (m_Version == null)
                    {
                        int version = 0;

                        int.TryParse(m_FileInfo.Name.Substring(0, m_FileInfo.Name.IndexOf('.')), out version);

                        m_Version = version;
                    }

                    return m_Version.Value;
                }
            }

            public string FullName
            {
                get
                {
                    return m_FileInfo.FullName;
                }
            }

            public DateTime CreationTime
            {
                get
                {
                    return m_FileInfo.CreationTime;
                }
            }

            #region IComparable<BackupFileInfo> 成员

            public int CompareTo(BackupFileInfo other)
            {
                return  other.Version - this.Version;
            }

            #endregion
        }

        private BackupFileInfo[] GetBackupFileInfos(string historyDir)
        {
            if (Directory.Exists(historyDir) == false)
            {
                return new BackupFileInfo[0];
            }

            List<BackupFileInfo> result = new List<BackupFileInfo>();

            DirectoryInfo dir = new DirectoryInfo(historyDir);

            foreach (FileInfo fileInfo in dir.GetFiles("*.config"))
            {
                result.Add(new BackupFileInfo(fileInfo));
            }

            result.Sort();

            return result.ToArray();
        }

        public string FileType
        {
            get
            {
                string file = FilePath;

                if (file == null)
                    return string.Empty;

                string type = Path.GetExtension(file);

                switch (type)
                {
                    case ".css":
                        return @"
                            parserfile: ['parsecss.js'],
                            stylesheet: ['" + Root + "/max-assets/codemirror/css/csscolors.css']";

                    case ".js":
                        return @"
                            parserfile: ['tokenizejavascript.js', 'parsejavascript.js'],
                            stylesheet: ['" + Root + "/max-assets/codemirror/css/jscolors.css']";

                    default:
                        return @"
                            parserfile: ['parsexml.js', 'parsecss.js', 'tokenizejavascript.js', 'parsejavascript.js', 'parsehtmlmixed.js'],
                            stylesheet: ['" + Root + "/max-assets/codemirror/css/xmlcolors.css', '" + Root + "/max-assets/codemirror/css/jscolors.css', '" + Root + "/max-assets/codemirror/css/csscolors.css']";
                }
            }
        }

        private BackupFileInfo[] m_BackupFileList;

        public BackupFileInfo[] BackupFileList
        {
            get { return m_BackupFileList; }
        }

        private string m_FileContent;

        public string FileContent
        {
            get
            {
                return m_FileContent;
            }
        }

        private string m_FileContent2;

        public string FileContent2
        {
            get
            {
                return m_FileContent2;
            }
        }

        private string m_FileName;

        public string FilePath
        {
            get
            {
                if (_Request.Get("file") == null)
                {
                    if (TemplateFileList.Length > 0)
                        return "\\_skin.config";
                    else
                        return null;
                }
                else
                    return _Request.Get("file");
            }
        }

        public class TemplateFileInfo
        {
            private string m_Name;

            private string m_Path;

            private int m_Level;

            private bool m_IsDir;

            public TemplateFileInfo(string name, string subPath, int level)
            {
                m_Name = name;
                m_Path = subPath;
                m_Level = level;
            }

            public string Name
            {
                get { return m_Name; }
            }

            public string Path
            {
                get { return m_Path; }
            }

            public int Level
            {
                get { return m_Level; }
            }

            public bool IsDir
            {
                get { return m_IsDir; }
                set { m_IsDir = value; }
            }
        }

        public string Loop(int from, int to, string repeat)
        {
            StringBuffer sb = new StringBuffer();

            for (int i = from; i < to; i++)
            {
                sb += repeat;
            }

            return sb.ToString();
        }

        private TemplateFileInfo[] m_TemplateFileList;

        public TemplateFileInfo[] TemplateFileList
        {
            get
            {
                if (m_TemplateFileList == null)
                {
                    string skin = _Request.Get("skin");

                    if (skin == null)
                    {
                        m_TemplateFileList = new TemplateFileInfo[0];
                        return m_TemplateFileList;
                    }

                    List<TemplateFileInfo> result = new List<TemplateFileInfo>();

                    string skinPath = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), skin);

                    LoadTemplateFileInfos(skinPath, result);

                    for (int i = 0; i < result.Count - 1; )
                    {
                        if (result[i].IsDir && result[i].Level >= result[i + 1].Level)
                        {
                            result.RemoveAt(i);

                            if(i - 1 >= 0)
                                i = i - 1;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    m_TemplateFileList = result.ToArray();
                }

                return m_TemplateFileList;
            }
        }

        private void LoadTemplateFileInfos(string directory, List<TemplateFileInfo> result)
        {
            LoadTemplateFileInfos1(directory, new DirectoryInfo(directory), 0, result);
        }

        private void LoadTemplateFileInfos1(string root, DirectoryInfo dirInfo, int level, List<TemplateFileInfo> result)
        {
            foreach (DirectoryInfo directory in dirInfo.GetDirectories())
            {
                if (directory.Name == ".svn" || directory.Name == "__模板编辑历史记录__")
                    continue;

                TemplateFileInfo dir = new TemplateFileInfo(directory.Name, directory.FullName.Substring(root.Length), level);
                dir.IsDir = true;

                result.Add(dir);

                LoadTemplateFileInfos1(root, directory, level + 1, result);

                //LoadTemplateFileInfos2(root, directory, level + 1, isMaxPages, result);
            }

            LoadTemplateFileInfos2(root, dirInfo, level, result);
        }

        private void LoadTemplateFileInfos2(string root, DirectoryInfo dirInfo, int level, List<TemplateFileInfo> result)
        {
            TemplateFileInfo skinConfigFile = null;

            foreach (FileInfo file in dirInfo.GetFiles("*"))
            {
                if (
                    StringUtil.EqualsIgnoreCase(file.Extension, ".aspx") ||
                    StringUtil.EqualsIgnoreCase(file.Extension, ".ascx") ||
                    StringUtil.EqualsIgnoreCase(file.Extension, ".js") ||
                    StringUtil.EqualsIgnoreCase(file.Extension, ".css") ||
                    StringUtil.EqualsIgnoreCase(file.Extension, ".html")
                    )
                {
                    result.Add(new TemplateFileInfo(file.Name, file.FullName.Substring(root.Length), level));
                }
                else if (StringUtil.EqualsIgnoreCase(file.Name, "_skin.config"))
                {
                    skinConfigFile = new TemplateFileInfo(file.Name, file.FullName.Substring(root.Length), level);
                }
            }

            if (skinConfigFile != null)
                result.Add(skinConfigFile);
        }

        private SkinCollection m_allSkinList;
        protected SkinCollection AllSkinList
        {
            get
            {
                if (m_allSkinList == null)
                {
                    m_allSkinList = TemplateManager.GetAllSkins();

                }

              return   m_allSkinList;
            }
        }
    }
}