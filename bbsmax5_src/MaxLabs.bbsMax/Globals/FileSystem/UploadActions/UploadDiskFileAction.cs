//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.AppHandlers;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using System.Web;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax.FileSystem
{
    public class UploadDiskFileAction : FileActionBase
    {
        long userRemnantDiskSpace = 0;

        public override FileActionBase CreateInstance()
        {
            return new UploadDiskFileAction();
        }

        public override string Name
        {
            get { return "disk"; }
        }

        #region 上传文件

        public override bool AfterUpload(HttpContext context, string fileName, string serverFilePath, long fileSize, int tempUploadFileID, string md5, NameValueCollection queryString, ref object customResult)
        {

            int directoryID = int.Parse(queryString["directory"]);
            bool replaceExistsFile = queryString["onexist"] == "replace";

            return DiskBO.Instance.SaveUploadedFile(User.Current, directoryID, tempUploadFileID, replaceExistsFile);
        }

        public override bool Uploading(HttpContext context, string fileName, string serverFilePath, long fileSize, long uploadedSize, ref object customResult)
        {
            if (userRemnantDiskSpace < fileSize)
                return false;
            return true;
        }

        public override bool BeforeUpload(HttpContext context, string fileName, string serverFilePath, NameValueCollection queryString, ref object customResult)
        {
            int directory;
            int.TryParse(queryString["directory"], out directory);
            bool replaceExistsFile = queryString["onexist"] == "replace";
            return DiskBO.Instance.CanUpload(UserBO.Instance.GetCurrentUserID(), directory, fileName, 0, replaceExistsFile, out userRemnantDiskSpace);
        }

        #endregion

        #region 下载文件

        public override bool Downloading(System.Web.HttpContext context)
        {
            HttpRequest Request = context.Request;
            int fileID = 0;
            int.TryParse(Request.QueryString["FileID"], out fileID);
            int userId = UserBO.Instance.GetCurrentUserID();

            if (fileID > 0)
            {

                DiskFile file = DiskBO.Instance.GetDiskFile(fileID);

                if (file == null)
                    return false;

                if (file.UserID != userId)
                {
                    if (file.UserID != userId && !DiskBO.Instance.CanManageUserNetDisk(userId, file.UserID))
                    {
                        return false;
                    }
                }

                else if (!DiskBO.Instance.CanUseNetDisk(userId))
                {
                    return false;
                }

                return OutputFileByID(context, file.FileID, file.FileName, file.FileName.Contains(".") ? file.FileName.Substring(file.FileName.LastIndexOf('.')) : string.Empty, OutputFileMode.Attachment);
            }
            return false;
        }

        #endregion
    }
}