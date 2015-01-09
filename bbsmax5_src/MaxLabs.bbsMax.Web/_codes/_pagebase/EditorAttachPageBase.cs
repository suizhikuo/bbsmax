//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public class EditorAttachPageBase:DialogPageBase
    {
        protected  ExtensionList ImageFileTypes = new ExtensionList(new string[] { "jpg", "gif", "bmp", "png", "jpeg" });
        protected  ExtensionList VideoFileTypes = new ExtensionList(new string[] { "wmv", "rm", "rmvb", "avi", "mov", "3gp","mp4", "flv", "mkv" });
        protected  ExtensionList AudioFileTypes = new ExtensionList(new string[] { "mp3", "wav", "wma", "mid", "ra", "midi", "asf", "ape" });
        protected  ExtensionList FlashFileTypes = new ExtensionList(new string[] { "swf" });
        protected ExtensionList m_ListFileTypes = null;

        private Dictionary<int, DiskDirectoryCollection> dirs;

        private DiskFileCollection diskFiles = null;

        private DiskDirectoryCollection diskDirectories = null;

        private List<IFile> allFiles;

        protected DiskFile DiskFile;

        protected DiskDirectory DiskDirectory;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //pageNumber = _Request.Get<int>("page", Method.Get, 1);

            int tab = _Request.Get<int>("tab", Method.Get, 0);
            if (tab == 3 || tab == 2)
            {
                int total = tab == 2 ? this.AttachmentList.TotalRecords : this.AllFiles.Count;
                SetPager("list", string.Format("?id={0}&tab={2}&page={1}&forumid={3}", _Request.Get("id", Method.Get), "{0}",tab,ForumID), PageNumber, PageSize, total);
            }
        }

        /// <summary>
        /// 文件列表列出的文件类型
        /// </summary>
        protected virtual ExtensionList ListFileTypes
        {
            get
            {
                return ForumAllowedFileTypes;
            }
        }

        private ExtensionList m_forumAllowedFileTypes;
        protected ExtensionList ForumAllowedFileTypes
        {
            get
            {
                if (m_forumAllowedFileTypes == null)
                {
                    if (ForumID == 0)
                        m_forumAllowedFileTypes = new ExtensionList();
                    else
                    m_forumAllowedFileTypes = ForumSetting.AllowFileExtensions.GetValue(My);
                }
                return m_forumAllowedFileTypes;
            }
        }
        long? m_allowFileSize=null;
        protected virtual long MaxFileSize
        {
            get
            {
                if (m_allowFileSize == null)
                {
                    if (ForumID == 0)
                    {
                        m_allowFileSize = 0;
                    }
                    else
                    {
                        long v = ForumSetting.MaxSignleAttachmentSize.GetValue(My);
                        if (v == 0)
                            m_allowFileSize = long.MaxValue;
                        m_allowFileSize = v;
                    }
                }
                return m_allowFileSize.Value;
            }
        }

        protected virtual string TypeName
        {
            get
            {
                return string.Empty;
            }
        }

        private string m_MaxFileSizeForSwfUpload;
        protected string MaxFileSizeForSwfUpload
        {
            get
            {
                if (m_MaxFileSizeForSwfUpload == null)
                {
                    string[] units = {"B","KB","MB","GB","TB"};
                    int ui = 0;
                    double s = MaxFileSize;
                    while (s > 1024)
                    {
                        s /= 1024.0d;
                        ui++;
                    }
                    m_MaxFileSizeForSwfUpload = string.Format("{0} {1}",Math.Round(s,2),units[ui]);
                }

                return m_MaxFileSizeForSwfUpload;
            }
        }

        protected int MaxFileCount
        {
            get
            {
                return int.MaxValue;
                //if (ForumID == 0)
                //    return 0;

                //int v= ForumSetting.max.GetValue(My);

                //if (v == 0)
                //    return int.MaxValue;
                //return v;
            }
        }

        protected virtual bool AllowAttachment
        {
            get
            {
                if (ForumID == 0)
                    return false;

                return ForumSetting.AllowAttachment.GetValue(My);
            }
        }

        protected int ForumID
        {
            get
            {
                return _Request.Get<int>("forumid", 0);
            }
        }

        #region 网络硬盘

        #region 排序、分页

        protected FileOrderBy OrderBy
        {
            get
            {
                return FileOrderBy.CreateDate;
            }
        }

        protected bool IsDesc
        {
            get
            {
                return _Request.Get<bool>("desc", Method.Get, true);
            }
        }

        protected int PageNumber
        {
            get
            {
                return _Request.Get<int>("page", Method.Get, 1);
            }
        }

       
        #endregion

        #region 属性、数据
        DiskDirectory m_currentDirectory;
        protected DiskDirectory CurrentDirectory
        {
            get
            {
                if (m_currentDirectory == null)
                {
                    int directoryID = _Request.Get<int>("directoryID", Method.Get, 0);
                    if (_Request.Get("action", Method.Get, string.Empty).ToLower() == "move" || _Request.Get<int>("SourceID", Method.Get, 0) > 0)
                    {
                        directoryID = _Request.Get<int>("SourceID", Method.Get, 0);
                    }
                    //当前目录实例
                    m_currentDirectory = DiskBO.Instance.GetDiskDirectory(MyUserID, directoryID);
                }
                return m_currentDirectory;
            }
        }

        /// <summary>
        /// 当前目录下的所有文件
        /// </summary>
        protected DiskDirectoryCollection DiskDirectories
        {
            get
            {
                if (diskDirectories == null)
                {
                    allFiles = DiskBO.Instance.GetDiskFiles(MyUserID, CurrentDirectory.DirectoryID, out diskDirectories, out diskFiles, this.OrderBy, this.IsDesc);
                }
                return diskDirectories;
            }
        }

        protected string GetUrl(object diskDirctoryID)
        {
            UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();

            scheme.AttachQuery("directoryID", diskDirctoryID.ToString());

            return scheme.ToString(false);
        }

        protected string ToJs(string s)
        {
            return StringUtil.ToJavaScriptString(s);
        }


        protected List<IFile> AllFiles
        {
            get
            {
                if (allFiles == null)
                {
                    allFiles = DiskBO.Instance.GetDiskFiles(MyUserID, CurrentDirectory.DirectoryID
                      , out diskDirectories, out diskFiles, this.OrderBy, this.IsDesc, this.ListFileTypes);
                }
                return allFiles;
            }
        }

        IFile[] m_PagedFiles = null;
        protected IFile[] PagedFiles
        {
            get
            {
                if (m_PagedFiles == null)
                {
                    if (this.AllFiles.Count == 0)
                    {
                        m_PagedFiles = new IFile[0];
                    }
                    else
                    {
                        int start = (this.PageNumber - 1) * this.PageSize;
                        int end = this.PageNumber * this.PageSize;
                        if (end > this.AllFiles.Count - 1) end = this.AllFiles.Count - 1;
                        int fileCount = end - start + 1;
                        if (fileCount < 0) fileCount = 0;
                        m_PagedFiles = new IFile[fileCount];
                        if (start < m_PagedFiles.Length)
                            AllFiles.CopyTo(start, m_PagedFiles, 0, m_PagedFiles.Length);
                        else
                            return new IFile[0];
                    }
                }
                return m_PagedFiles;
            }
        }

        //protected List<DiskDirectory> SelectedDirectories
        //{
        //    get
        //    {
        //        return new List<DiskDirectory>();
        //    }
        //}

        //protected List<DiskFile> SeledctedFiles
        //{
        //    get
        //    {
        //        return new List<DiskFile>();
        //    }
        //}
        #endregion

        #region  文件夹列表

            private string tempSpace = string.Empty;

            protected string GetDiskDirectoryDropDowm(string space, string html, string onSelect)
            {
                StringBuffer bufer = new StringBuffer();
                GetDiskMenu(DiskBO.Instance.GetDiskRootDirectory(MyUserID), space, html, onSelect, 0, bufer);
                return bufer.ToString();
            }

            protected void GetDiskMenu(DiskDirectory directory, string space, string html, string onSelect, int depth, StringBuffer buffer)
            {
                if (directory == null) return;

                dirs = DiskBO.Instance.GetParentDiskDirectories(MyUserID, directory.DirectoryID);
                string spaceString = "";

                for (int i = 0; i < depth; i++)
                {
                    spaceString += space;
                }
                buffer += string.Format(html, spaceString, directory.DirectoryID, directory.ParentID == 0 ? "\\" : directory.Name, directory.DirectoryID == CurrentDirectory.DirectoryID ? onSelect : "");

                DiskDirectoryCollection childDirectory;

                if (dirs.ContainsKey(directory.DirectoryID))
                {
                    childDirectory = dirs[directory.DirectoryID];

                    if (childDirectory != null)
                    {
                        foreach (DiskDirectory dir in childDirectory)
                        {
                            GetDiskMenu(dir, space, html, onSelect, depth + 1, buffer);
                        }
                    }
                }

                //tempSpace = space;
                //menuString = string.Empty;
                //int dID = CurrentDirectory.DirectoryID;
                //
                //DiskDirectory root = DiskBO.Instance.GetDiskRootDirectory(MyUserID);
                //string rootstr = string.Empty;
                //if (root == null || root.DirectoryID <= 0)
                //{
                //    rootstr = string.Format(html, root==null?0:root.DirectoryID, rootNameStr);// UrlHelper.GetDiskCenterUrl(), rootNameStr);
                //    return rootstr;
                //}
                // rootstr = string.Format(html, dID, rootNameStr);// UrlHelper.GetDiskCenterUrl(), rootNameStr);
                //menuString += rootstr;
                ////html = string.Format(html, "{0}", "{1}", "{2}" + dirNameStr);
                //GetDirectoriesByDirectory(root.DirectoryID, dirs, space,
                //    html);
                //return menuString;
            }

            private List<int> MenuTreeIDs(int currentID)
            {
                List<int> menus = new List<int>();
                DiskDirectory idd = GetParentEntry(currentID);
                while (idd != null)
                {
                    menus.Add(idd.DirectoryID);
                    idd = GetParentEntry(idd.ParentID);
                }
                menus.Add(currentID);
                return menus;
            }

            private DiskDirectory GetParentEntry(int directoryID)
            {
                if (directoryID == 0)
                    return null;
                foreach (DiskDirectoryCollection ds in dirs.Values)
                {
                    foreach (DiskDirectory d in ds)
                    {
                        if (d.DirectoryID == directoryID)
                            return d;
                    }
                }
                return null;
            }

            public void GetDirectoriesByDirectory(int directoryID, Dictionary<int, DiskDirectoryCollection> dirs, string space, string html)
            {
                foreach (int dID in dirs.Keys)
                {
                    if (dID == directoryID)
                    {
                        foreach (DiskDirectory d in dirs[dID])
                        {
                            int currentID = CurrentDirectory.DirectoryID;

                            bool isCurrent = MenuTreeIDs(currentID).Contains(d.DirectoryID);
                            string url = BbsRouter.GetUrl("my/disk/" + d.DirectoryID);


                            //string dirname = string.Format(html, "%className%", url, space, "%imgPath%", d.DirectoryName);
                            //string className = noselectedClass;
                            //string imgPath = noselectedImg;

                            if (isCurrent)
                            {
                                if (d.DirectoryID == currentID)
                                {
                                    //className = selectedClass;
                                }

                                //imgPath = selectedImg;
                            }

                            GetDirectoriesByDirectory(d.DirectoryID, dirs, space + tempSpace, html);
                        }
                    }
                }
            }
        #endregion

        #endregion

        protected string Keyword
        {
            get
            {
                return _Request.Get("keyword", Method.Post, string.Empty);
            }
            
        }

        protected int? Year
        {
            get
            {
                return _Request.Get<int>("year", Method.Post);
            }
        }

        protected int? Month
        {
            get
            {
                return _Request.Get<int>("month", Method.Post);
            }
        }

        protected int? Day
        {
            get
            {
                return _Request.Get<int>("day", Method.Post);
            }
        }

        private Forum m_forum;

        protected Forum Forum
        {
            get
            {
                if (m_forum == null)
                    m_forum = ForumBO.Instance.GetForum(ForumID);
                return m_forum;
                
            }
        }

        protected int PageSize
        {

            get
            {
                return 20;
            }
        }

        private AttachmentCollection m_attachlist;

        protected AttachmentCollection AttachmentList
        {
            get
            {
                if (m_attachlist == null)
                {
                    m_attachlist = PostBOV5.Instance.GetAttachments(MyUserID
                        ,Year
                        ,Month
                        ,Day
                        ,Keyword
                        , _Request.Get<int>("page", Method.Get, 1), PageSize,this.ListFileTypes);
                }

                return m_attachlist;
            }
        }

        protected bool? m_canUserNetDisk = null;

        protected bool CanUseNetDisk
        {
            get
            {
                if(m_canUserNetDisk==null)
                    m_canUserNetDisk=  DiskBO.Instance.CanUseNetDisk(My.UserID);
                return m_canUserNetDisk.Value;
            }
        }

        ForumSettingItem m_ForumSetting;

        protected ForumSettingItem ForumSetting
        {
            get
            {
                if (this.ForumID == 0)
                    return null;

                if (m_ForumSetting == null)
                    m_ForumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(ForumID);
                return m_ForumSetting;
            }
        }

     

        protected virtual string FileTypeForSwfUpload
        {
            get
            {
                return this.ListFileTypes.GetFileTypeForSwfUpload();
            }
        }

        protected bool IsImage( string ext)
        {
            return ImageFileTypes.Contains(ext);
        }

        protected bool IsAudio(string ext)
        {
            return AudioFileTypes.Contains(ext);
        }

        protected bool IsVideo(string ext)
        {
            return VideoFileTypes.Contains(ext);
        }

        protected bool IsFlash(string ext)
        {
            return FlashFileTypes.Contains(ext);
        }

        protected bool CanSelect(long size)
        {
            return size <= MaxFileSize;
           // return ForumAllowedFileTypes.Contains(ext);
        }

        protected ExtensionList FilterFileType(ExtensionList filetypes)
        {
            ExtensionList ext = new ExtensionList();
            foreach (string s in filetypes )
            {
                if (ForumAllowedFileTypes.Contains(s))
                    ext.Add(s);
            }
            return ext;
        }
    }
}