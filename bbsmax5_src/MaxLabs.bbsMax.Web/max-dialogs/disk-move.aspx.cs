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
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class disk_move : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!DiskBO.Instance.CanUseNetDisk(MyUserID))
            {
                ShowError("您没有使用网络硬盘的权限！");
            }

            if (files.Length == 0 && directories.Length == 0)
            {
                ShowError("没有选中任何文件或文件夹！");
            }
            if (_Request.IsClick("move"))
            {
                Move();
            }
        }

        private string menuString;

        private string tempSpace = string.Empty;

        private Dictionary<int, DiskDirectoryCollection> dirs;

        protected string GetDiskMenu(string space, string html, string selectedImg, string noselectedImg, string selectedClass, string noselectedClass, string dirNameStr, string rootNameStr)
        {
            tempSpace = space;
            menuString = string.Empty;
            int dID = CurrentDirectory.DirectoryID;

            if (_Request.Get<int>("dID", Method.Get, 0)>0)
            {
 
                    dID = _Request.Get<int>("dID", Method.Get, 0);
            }

            dirs = DiskBO.Instance.GetParentDiskDirectories(MyUserID, dID);
            DiskDirectory root = DiskBO.Instance.GetDiskRootDirectory(MyUserID);
            string rootstr = string.Empty;

            if (root == null || root.DirectoryID <= 0)
            {
                rootstr = string.Format(html, selectedClass, GetDiskCenterMoveUrl(CurrentDirectory.DirectoryID, CurrentDirectory.DirectoryID), rootNameStr);// UrlHelper.GetDiskCenterUrl() + "?SourceID=" + CurrentDirectory.DirectoryID, rootNameStr);
                return rootstr;
            }
            rootstr = string.Format(html, ((root.DirectoryID == dID || dID == 0) ? selectedClass : noselectedClass), GetDiskCenterMoveUrl(root.DirectoryID,CurrentDirectory.DirectoryID), rootNameStr);// UrlHelper.GetDiskCenterUrl() + "?SourceID=" + CurrentDirectory.DirectoryID, rootNameStr);           
            menuString += rootstr;
            html = string.Format(html, "{0}", "{1}", "{2}" + dirNameStr);
            GetDirectoriesByDirectory(root.DirectoryID, dirs, space,
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

        public void GetDirectoriesByDirectory(int directoryID, Dictionary<int, DiskDirectoryCollection> dirs, string space, string html, string selectedImg, string noselectedImg, string selectedClass, string noselectedClass)
        {
            foreach (int dID in dirs.Keys)
            {
                if (dID == directoryID)
                {
                    foreach (DiskDirectory d in dirs[dID])
                    {
                        int currentID = CurrentDirectory.DirectoryID;
                        
                            currentID = _Request.Get<int>("dID", Method.Get, 0);
                        
                        bool isCurrent = MenuTreeIDs(currentID).Contains(d.DirectoryID);
                        string url = GetDiskCenterMoveUrl(d.DirectoryID,  CurrentDirectory.DirectoryID);
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

        string GetDiskCenterMoveUrl(int directoryID, int sourceID)
        {
            UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();

            scheme.AttachQuery("dID", directoryID.ToString());
            scheme.AttachQuery("SourceID", sourceID.ToString());
            scheme.AttachQuery("diskDirectoryID", StringUtil.Join(directories));
            scheme.AttachQuery("diskFileID", StringUtil.Join(files));

            return scheme.ToString();
        }
        
        DiskDirectory m_currentDirectory;

        protected DiskDirectory CurrentDirectory
        {
            get
            {
                if (m_currentDirectory == null)
                {
                    m_currentDirectory =DiskBO.Instance.GetDiskDirectory(MyUserID, _Request.Get<int>("directoryid", Method.Get, 0));
                }
                return m_currentDirectory;
            }
        }
        private int[] m_files;
        protected string FileIDString
        {
            get {
                return StringUtil.Join(files);
            }

        }

        protected string directoryIdString
        {
            get
            {
                return StringUtil.Join(directories);
            }
        }

        private int[] files
        {
            get
            {
                if (m_files == null) m_files = _Request.GetList<int>("diskFileID", Method.All, new int[0]);
                return m_files;
            }
        }
        private int[] m_directories;
        private int[] directories
        {
            get
            {
                if (m_directories == null) m_directories = _Request.GetList<int>("diskDirectoryID", Method.All, new int[0]);
                return m_directories;
            }
        }

        private void Move()
        {

            int newDirectoryID =  _Request.Get<int>("dID", Method.Get , 0);
            int sourceID = _Request.Get<int> ("SourceID",Method.Get , 0);
            if (sourceID == 0)
            {
                sourceID = CurrentDirectory.DirectoryID;
            }

            MoveStatus status = DiskBO.Instance.MoveDiskDirectoriesAndFiles(My.UserID, sourceID, newDirectoryID,new List<int>( files), new List<int>( directories));
            if (status != MoveStatus.Success)
            {
                switch (status)
                {
                    case MoveStatus.NoMoveChildDirectory:
                        ShowError("不能移动到子目录");
                        break;
                    case MoveStatus.NoMoveSelecteDirectory:
                        ShowError("不能移动到当前目录");
                        break;
                    case MoveStatus.DuplicateFileName:
                        ShowError("重复的文件名");
                        break;
                    case MoveStatus.DirectoryIDNotExist:
                        ShowError("目录不存在");
                        break;
                    default:
                        ShowError("发生错误， 无法移动到指定的目录下");
                        break;
                }
               // ShowError(status);
            }
            else
            {
                Return(true);
            }
        }
    }
}