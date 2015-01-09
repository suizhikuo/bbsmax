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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class disk_rename : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!DiskBO.Instance.CanUseNetDisk(MyUserID))
            {
                ShowError("您没有使用网络硬盘的权限！");
            }

            if (CurrentDirectory == null)
            {
                ShowError("无效的目录");
            }

            if (fileIDs.Length == 0 && directoryIDs.Length == 0)
            {
                ShowError("未选择任何文件或文件夹！");
            }

            if (_Request.IsClick("rename"))
            {
                ProcessReNameDiskDirectoryAndFiles();
            }
        }

        DiskDirectory m_CurrentDirectory;
        protected DiskDirectory CurrentDirectory
        {
            get
            {
                if (m_CurrentDirectory == null)
                {
                    int dirid = _Request.Get<int>("Directoryid", Method.Get, 0);
                    m_CurrentDirectory = DiskBO.Instance.GetDiskDirectory(MyUserID, dirid);
                }
                return m_CurrentDirectory;
            }
        }

        int[] m_directoryIDs = null;
        private int[] directoryIDs
        {
            get
            {
                if (m_directoryIDs == null)
                    m_directoryIDs = _Request.GetList<int>("diskDirectoryID", Method.Post, new int[0]);
                return m_directoryIDs;
            }
        }

        private int[] m_fileIDs;
        private int[] fileIDs
        {
            get
            {
                if (m_fileIDs == null)
                    m_fileIDs = _Request.GetList<int>("diskFileID", Method.Post, new int[0]);
                return m_fileIDs;
            }
        }

        private DiskDirectoryCollection m_directorys;
        private DiskFileCollection m_files;
        private List<IFile> m_fileList;

        protected List<IFile> FileList
        {
            get
            {
                if (m_fileList == null)
                {
                    DiskBO.Instance.GetDiskDirectoriesAndDiskFiles(MyUserID, CurrentDirectory.DirectoryID
                        , directoryIDs
                        , fileIDs
                        , out m_directorys
                        , out m_files
                        );
                    m_fileList = new List<IFile>(m_directorys.Count + m_files.Count);

                    foreach (IFile file in m_directorys)
                        m_fileList.Add(file);
                    foreach (IFile file in m_files)
                        m_fileList.Add(file);
                }
                return m_fileList;
            }
        }
        //protected void RenameDirectory()
        //{
        //    MessageDisplay msgDisplay = CreateMessageDisplay();
        //    int dirId = _Request.Get<int>("directoryid", Method.Get, 0);
        //    DiskDirectory currentDir = DiskBO.Instance.GetDiskDirectory(MyUserID, dirId);
        //    string dirName = _Request.Get("newdirectoryname", Method.Post);
        //    CreateUpdateDiskDirectoryStatus status;
        //    status = DiskBO.Instance.RenameDiskDirectory(MyUserID, dirId, dirName);
        //}
        /// <summary>
        /// 重命名目录
        /// </summary>
        private void ProcessReNameDiskDirectoryAndFiles()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();


            string[] newDirectoryNames = StringUtil.Split(_Request.Get("newDirectoryName", Method.Post, null));
            string[] newFileNames = StringUtil.Split(_Request.Get("newFileName", Method.Post, null));

            if (directoryIDs.Length != newDirectoryNames.Length ||
                fileIDs.Length != newFileNames.Length)
            {
                ShowError("无效文件名");
            }

            CreateUpdateDiskFileStatus status;
            if (directoryIDs.Length != 0 || fileIDs.Length != 0)
            {
                status =
                DiskBO.Instance.RenameDiskDirectoryAndFiles(My.UserID, CurrentDirectory.DirectoryID
                , directoryIDs
                , fileIDs
                , newDirectoryNames
                , newFileNames);

                switch (status)
                {
                    case CreateUpdateDiskFileStatus.Success:
                        Return(true);
                        break;
                    case CreateUpdateDiskFileStatus.DuplicateFileName:
                        msgDisplay.AddError("文件的名称重复");
                        break;
                    case CreateUpdateDiskFileStatus.EmptyFileName:
                        msgDisplay.AddError("文件的名称不能为空");
                        break;
                    case CreateUpdateDiskFileStatus.InvalidFileName:
                        msgDisplay.AddError("文件名不符合规范");
                        break;
                    case CreateUpdateDiskFileStatus.InvalidFileNameLength:
                        msgDisplay.AddError("文件名称长度超出限制");
                        break;
                    case CreateUpdateDiskFileStatus.InvalidFileExtension:
                        msgDisplay.AddError("无效的文件扩展名");
                        break;
                    default:
                        msgDisplay.AddError("发生了未知错误，请联系管理员");
                        break;
                }
            }
            else
            {
                ShowError("没有选择任何文件或者文件夹！");
            }
        }
    }
}