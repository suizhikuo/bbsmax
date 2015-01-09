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

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class disk_upload : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!DiskBO.Instance.CanUseNetDisk(MyUserID))
            {
                ShowError("您没有使用网络硬盘的权限！");
            }

            if (_Request.IsClick("save"))
            {
                SaveTempFile();
            }
        }

        protected string AllowedFileType
        {
            get
            {
                ExtensionList fileTypes = AllSettings.Current.DiskSettings.AllowFileExtensions.GetValue(MyUserID);

                return fileTypes.GetFileTypeForSwfUpload();
            }
        }

        protected void SaveTempFile()
        {
            //int tempFileIDs = _Request.GetList<int>("tempfileids", Method.Post, new int[0]);
        }

        private DiskDirectory m_curentDirectory;
        protected DiskDirectory CurrentDirectory
        {
            get
            {
                if (m_curentDirectory == null)
                {
                    m_curentDirectory = DiskBO.Instance.GetDiskDirectory(MyUserID, _Request.Get<int>("directoryid", Method.Get, 0));
                }
                return m_curentDirectory;
            }
        }

        protected string MaxFileSizeForSwfUpload
        {
            get
            {
                return CommonUtil.FormatSizeForSwfUpload(MaxFileSize);
            }
        }

        protected long MaxFileSize
        {
            get
            {
                return DiskBO.Instance.GetMaxFileSize(MyUserID);
            }
        }

        protected int MaxFileCount
        {
            get
            {
                return DiskBO.Instance.GetMaxFileCount(MyUserID) - My.TotalDiskFiles;
            }
        }
    }
}