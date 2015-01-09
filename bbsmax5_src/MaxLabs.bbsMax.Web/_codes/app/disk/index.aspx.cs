//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.App_Disk
{
    public partial class index : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "网络硬盘 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "disk"; }
        }

        protected override string NavigationKey
        {
            get { return "disk"; }
        }

        private DiskDirectory m_CurrentDirectory;

        private Dictionary<int, DiskDirectoryCollection> m_ParentDirectories;
        private DiskFileCollection m_DiskFiles = null;
        private DiskDirectoryCollection m_DiskDirectories = null;
        private List<IFile> m_AllFiles;
        private IFile[] m_PagedFiles = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("网络硬盘");
            if (!DiskBO.Instance.CanUseNetDisk(MyUserID))
            {
                ShowError("您没有使用网络硬盘的权限");
                return;
            }

            //if (_Request.IsClick("Zip"))
            //{
            //    if (!CanPackZip)
            //    {
            //        ShowError("您没有权限打包文件"); 
            //        return;
            //    }
            //    Zip();
            //}

            else if (_Request.IsClick("Delete"))
            {
                Delete();
            }

            string pageSizeMode = _Request.Get("pagesize");

            switch (pageSizeMode)
            {
                case "a":
                    m_PageSize = 20;
                    break;

                case "b":
                    m_PageSize = 40;
                    break;

                case "c":
                    m_PageSize = 60;
                    break;

                default:
                    m_PageSize = 20;
                    break;
            }

            SetPager("pager1", null, PageIndex, PageSize, AllFiles.Count);
        }

        #region 排序、分页

        protected FileOrderBy OrderBy
        {
            get
            {
                return _Request.Get<FileOrderBy>("orderby", Method.Get, FileOrderBy.None);
            }
        }
        protected bool IsDesc
        {
            get
            {
                return _Request.Get<bool>("desc", Method.Get, false);
            }
        }
        protected int PageIndex
        {
            get
            {
                return _Request.Get<int>("page", Method.Get, 1);
            }
        }

        private int m_PageSize;

        protected int PageSize
        {
            get
            {
                return m_PageSize;
            }
        }

        #endregion

        #region 空间计算
        protected string TotalUseSizesFormated
        {
            get
            {
                return ConvertUtil.FormatSize(TotalUseSizes);
            }
        }

        protected long TotalUseSizes
        {
            get
            {
                return My.UsedDiskSpaceSize;
            }
        }

        protected int TotalRemnantPercent
        {
            get
            {
                return (int)(((double)TotalUseSizes / (double)TotalSizes) * 100);
            }
        }

        protected string TotalRemnantFormated
        {
            get
            {
                return ConvertUtil.FormatSize(TotalSizes - TotalUseSizes);
            }
        }
        protected long TotalSizes
        {
            get
            {
                return DiskBO.Instance.GetDiskSpaceSize(MyUserID);
            }
        }

        protected string TotalSizesFormated
        {
            get
            {
                return ConvertUtil.FormatSize(DiskBO.Instance.GetDiskSpaceSize(MyUserID));
            }
        }

        protected long MaxFileSize
        {
            get
            {
                long maxFileSize = DiskBO.Instance.GetMaxFileSize(MyUserID);
                return maxFileSize;
            }
        }

        protected string SingleFileSizeString
        {
            get
            {
                return ConvertUtil.FormatSize(MaxFileSize);
            }
        }

        protected int MaxFileCount
        {
            get
            {
                return DiskBO.Instance.GetMaxFileCount(MyUserID);
            }
        }

        private string m_allowedFiletypes;
        protected string AllowedFileTypes
        {
            get
            {
                if (m_allowedFiletypes == null)
                {
                    m_allowedFiletypes = string.Empty;
                    ExtensionList list = AllSettings.Current.DiskSettings.AllowFileExtensions.GetValue(MyUserID);
                    foreach (string s in list)
                    {
                        m_allowedFiletypes += m_allowedFiletypes;
                    }
                }
                return m_allowedFiletypes;
            }
        }
        #endregion

        //#region 权限
        //public bool CanPackZip
        //{
        //    get
        //    {
        //        return DiskBO.Instance.CanPackZip(MyUserID);
        //    }
        //}

        //#endregion

        protected FileViewMode ViewMode
        {
            get
            {
                if (!string.IsNullOrEmpty(_Request.Get("viewmode", Method.Get)))
                {
                    return _Request.Get<FileViewMode>("viewmode", Method.Get, FileViewMode.Thumbnail);
                }
                else
                {
                    return AllSettings.Current.DiskSettings.DefaultViewMode.GetValue(My);
                }
            }
        }

        #region 属性、数据
        
        protected DiskDirectory CurrentDirectory
        {
            get
            {
                if (m_CurrentDirectory == null)
                {
                    int directoryID = _Request.Get<int>("directoryID", Method.Get, 0);
                    if (_Request.Get("action", Method.Get, string.Empty).ToLower() == "move" || _Request.Get<int>("SourceID", Method.Get, 0) > 0)
                    {
                        directoryID = _Request.Get<int>("SourceID", Method.Get, 0);
                    }
                    //当前目录实例
                    m_CurrentDirectory = DiskBO.Instance.GetDiskDirectory(MyUserID, directoryID);
                }
                return m_CurrentDirectory;
            }
        }

        /// <summary>
        /// 当前目录下的所有文件
        /// </summary>
        protected DiskDirectoryCollection DiskDirectories
        {
            get
            {
                if (m_DiskDirectories == null)
                {
                    m_AllFiles = DiskBO.Instance.GetDiskFiles(MyUserID, CurrentDirectory.DirectoryID, out m_DiskDirectories, out m_DiskFiles, this.OrderBy, this.IsDesc);
                }
                return m_DiskDirectories;
            }
        }

        /// <summary>
        /// 当前目录下的所有文件
        /// </summary>
        protected DiskFileCollection DiskFiles
        {
            get
            {
                if (m_DiskFiles == null)
                {
                    m_AllFiles = DiskBO.Instance.GetDiskFiles(MyUserID, CurrentDirectory.DirectoryID, out m_DiskDirectories, out m_DiskFiles, this.OrderBy, this.IsDesc);
                }
                return m_DiskFiles;
            }
        }

        protected List<IFile> AllFiles
        {
            get
            {
                if (m_AllFiles == null)
                {
                    m_AllFiles = DiskBO.Instance.GetDiskFiles(MyUserID, CurrentDirectory.DirectoryID
                      , out m_DiskDirectories, out m_DiskFiles, this.OrderBy, this.IsDesc);
                }
                return m_AllFiles;
            }
        }

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
                        int start = (this.PageIndex - 1) * this.PageSize;
                        int end = this.PageIndex * this.PageSize;
                        if (end > this.AllFiles.Count - 1) end = this.AllFiles.Count - 1;
                        m_PagedFiles = new IFile[end - start + 1];
                        AllFiles.CopyTo(start, m_PagedFiles, 0, m_PagedFiles.Length);
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

        private string menuString;

        private string tempSpace = string.Empty;

        protected string GetDiskMenu(string space, string html, string selectedImg, string noselectedImg, string selectedClass, string noselectedClass, string dirNameStr, string rootNameStr)
        {
            tempSpace = space;
            menuString = string.Empty;
            int dID = CurrentDirectory.DirectoryID;

            m_ParentDirectories = DiskBO.Instance.GetParentDiskDirectories(MyUserID, dID);
            DiskDirectory root = DiskBO.Instance.GetDiskRootDirectory(MyUserID);

            string rootstr = string.Empty;
            if (root == null || root.DirectoryID <= 0)
            {
                rootstr = string.Format(html, selectedClass, BbsRouter.GetUrl("app/disk/index") + "?directoryID=" + CurrentDirectory.DirectoryID + "&ViewMode=" + ViewMode, rootNameStr);// UrlHelper.GetDiskCenterUrl(), rootNameStr);

                return rootstr;
            }

            rootstr = string.Format(html, ((root.DirectoryID == dID || dID == 0) ? selectedClass : noselectedClass), BbsRouter.GetUrl("app/disk/index") + "?ViewMode=" + ViewMode, rootNameStr);// UrlHelper.GetDiskCenterUrl(), rootNameStr);

            menuString += rootstr;
            
            html = string.Format(html, "{0}", "{1}", "{2}" + dirNameStr);

            GetDirectoriesByDirectory(root.DirectoryID, m_ParentDirectories, space,
                html, selectedImg, noselectedImg, selectedClass, noselectedClass);
            
            return menuString;
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
            foreach (DiskDirectoryCollection ds in m_ParentDirectories.Values)
            {
                foreach (DiskDirectory d in ds)
                {
                    if (d.DirectoryID == directoryID)
                        return d;
                }
            }
            return null;
        }

        protected bool IsIE
        {
            get
            {
                if (Request.Browser != null && Request.Browser.Browser == "IE")
                    return true;
                else
                    return false;
            }
        }

        protected int BrowserMajorVersion
        {
            get
            {
                return Request.Browser.MajorVersion;
            }
        }

        public void GetDirectoriesByDirectory(int directoryID, Dictionary<int, DiskDirectoryCollection> dirs, string space, string html, string selectedImg, string noselectedImg, string selectedClass, string noselectedClass)
        {
            foreach (int dID in dirs.Keys)
            {
                if (dID == directoryID)
                {
                    foreach (DiskDirectory d in dirs[dID])
                    {
                        int currentID = CurrentDirectory.DirectoryID;

                        bool isCurrent = MenuTreeIDs(currentID).Contains(d.DirectoryID);
                        string url = BbsRouter.GetUrl("app/disk/index") + "?directoryID=" + d.DirectoryID + "&ViewMode=" + ViewMode;


                        string dirname = string.Format(html, "%className%", url, space, "%imgPath%", d.DirectoryName);
                        string className = noselectedClass;
                        string imgPath = noselectedImg;

                        if (isCurrent)
                        {
                            if (d.DirectoryID == currentID)
                            {
                                className = selectedClass;
                            }

                            imgPath = selectedImg;
                        }

                        menuString += dirname.Replace("%className%", className).Replace("%imgPath%", imgPath);
                        GetDirectoriesByDirectory(d.DirectoryID, dirs, space + tempSpace, html, selectedImg, noselectedImg, selectedClass, noselectedClass);
                    }
                }
            }
        }

        //private void Zip()
        //{
        /*
        int[] files = _Request.GetList<int>("diskFileID",Method.Post ,new int[0]);

        int[] directories = _Request.GetList<int>("diskDirectoryID", Method.Post ,new int[0] );

        if (files.Length == 0 && directories.Length == 0)
        {
            ShowError("没有选中任何文件或文件夹！");
        }

        CreateUpdateDiskFileStatus status = DiskBO.Instance.ZipFilesAndDirectories(MyUserID , CurrentDirectory.DirectoryID, new List<int>( directories), new List<int>( files));
        if (status != CreateUpdateDiskFileStatus.Success)
        {
            ShowError(status.ToString());
        }
        */
        //}

        /// <summary>
        /// 删除目录
        /// </summary>
        private void Delete()
        {
            int[] dirIds = _Request.GetList<int>("diskDirectoryID", Method.Post, new int[0]);
            int[] fileIds = _Request.GetList<int>("diskFileID", Method.Post, new int[0]);
            if (dirIds.Length == 0 && fileIds.Length == 0)
            {
                ShowError("没有选中任何文件或文件夹！");
            }

            DeleteStatus status = DiskBO.Instance.DeleteDiskFiles(My.UserID, CurrentDirectory.DirectoryID, fileIds);

            if (status != DeleteStatus.Success)
            {
                ShowError(status.ToString());
            }
            else
            {
                status = DiskBO.Instance.DeleteDiskDirectories(My.UserID, dirIds);

                if (status != DeleteStatus.Success)
                {
                    ShowError(status.ToString());
                }
                //else
                //{
                    // ShowInformation(status);
                //}
            }
        }
    }
}