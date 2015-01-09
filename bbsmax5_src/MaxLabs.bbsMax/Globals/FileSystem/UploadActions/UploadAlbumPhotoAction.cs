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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using System.Web;
using System.Collections.Specialized;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.FileSystem
{
    /// <summary>
    /// 上传/下载照片的Action
    /// </summary>
	public class UploadAlbumPhotoAction : FileActionBase
	{
		public override FileActionBase CreateInstance()
		{
			return new UploadAlbumPhotoAction();
		}

		public override string Name
		{
			get { return "album"; }
		}

        #region 上传文件

        bool m_ValidatedFreeSpace = false;

        public override bool BeforeUpload(HttpContext context, string fileName, string serverFilePath, NameValueCollection queryString, ref object customResult)
		{
			return true;
		}

        public override bool Uploading(HttpContext context, string fileName, string serverFilePath, long fileSize, long uploadedSize, ref object customResult)
		{
			if (m_ValidatedFreeSpace == false)
			{
				AuthUser user = User.Current;

				TempUploadFileCollection tempFiles = FileManager.GetUserTempUploadFiles(user.UserID, "album", null);

                long maxPhotoFileSize = AllSettings.Current.AlbumSettings.MaxPhotoFileSize[user];

                if(fileSize > maxPhotoFileSize)
                {
                    WebEngine.Context.ThrowError<OverMaxPhotoFileSizeError>(new OverMaxPhotoFileSizeError(maxPhotoFileSize));
                }

				long maxAlbumCapacity = AllSettings.Current.AlbumSettings.MaxAlbumCapacity[user];

				if (user.UsedAlbumSize + tempFiles.GetTotalFileSize() + fileSize > maxAlbumCapacity)
				{
					WebEngine.Context.ThrowError<OverMaxAlbumCapacityError>(new OverMaxAlbumCapacityError(maxAlbumCapacity));

					return false;
				}

				m_ValidatedFreeSpace = true;
			}

			return true;
		}

        public override bool AfterUpload(HttpContext context, string fileName, string serverFilePath, long filesize, int tempUploadFileID, string md5, NameValueCollection queryString, ref object customResult)
        {
            int albumID = 0;

            if (int.TryParse(queryString["albumID"], out albumID))
            {
                int[] newPhotoIDs = null;

                bool succeed = AlbumBO.Instance.CreatePhotos(User.Current.UserID, albumID, new string[] { fileName }, new string[] { string.Empty }, new int[] { tempUploadFileID }, new bool[] { false }, out newPhotoIDs);

                if (succeed)
                    customResult = newPhotoIDs[0];

                return succeed;
            }

            return false;
        }

        #endregion

        #region 下载文件

        public override bool Downloading(System.Web.HttpContext context)
		{
			string id = context.Request.QueryString["id"];

			int photoID = -1;

			if (StringUtil.TryParse(id, out photoID))
			{
                Photo photo = AlbumBO.Instance.GetPhoto(photoID);

				if (photo != null)
				{
                    //using (ErrorScope es = new ErrorScope())
                    //{
                        bool canVisit = AlbumBO.Instance.CanVisitAlbum(User.Current, photo.Album);
                        if (canVisit == false)
                        {
                            return false;
                        }
                    //}


					string fileName = photo.Name;
					string contentType = string.Empty;

					if(StringUtil.EndsWithIgnoreCase(photo.Name, photo.FileType) == false)
					{
						fileName += photo.FileType;
					}

					OutputFileMode outputFileMode = OutputFileMode.Inline;

					string mode = context.Request.QueryString["mode"];

					if (mode == "download")
						outputFileMode = OutputFileMode.Attachment;

					OutputFileByID(context, photo.FileID, fileName, photo.FileType, outputFileMode);

					return true;
				}
			}

			return false;
        }

        #endregion

    }
}