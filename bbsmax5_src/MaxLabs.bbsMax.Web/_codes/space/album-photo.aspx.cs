//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class album_photo : SpaceAlbumPageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			if (_Request.IsClick("updatephoto"))
			{
				UpdatePhoto();
			}

			int? photoID = _Request.Get<int>("id");
			string password = _Request.Get("password");
			int pageNumber = _Request.Get<int>("page", 1);

			m_CommentPageSize = Consts.DefaultPageSize;

			using (ErrorScope es = new ErrorScope())
			{
				if (photoID != null)
				{
					m_Photo = AlbumBO.Instance.GetPhotoForVisit(MyUserID, photoID.Value, password);

					if (m_Photo != null)
					{
						m_AlbumPhotoIDs = AlbumBO.Instance.GetAlbumPhotoIDs(m_Photo.AlbumID);

						m_CommentTargetID = m_Photo.PhotoID;

						if (_Request.IsClick("addcomment"))
							AddComment("space/" + SpaceOwnerID + "/album/photo-" + photoID, "#comment");

						m_CommentList = CommentBO.Instance.GetComments(m_Photo.PhotoID, CommentType.Photo, pageNumber, m_CommentPageSize, true, out m_TotalCommentCount);

						UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);
					}
					else
					{
						es.CatchError<NoPermissionVisitAlbumBeacuseNeedPasswordError>(delegate(NoPermissionVisitAlbumBeacuseNeedPasswordError error)
						{
							m_DisplayPasswordForm = true;
						});

						if(m_DisplayPasswordForm == false)
							ShowError("相册图片不存在");
					}
				}
				else
				{
					ShowError("相册图片不存在");
				}
			}

			base.OnLoadComplete(e);
        }

        public override string SpaceTitle
        {
            get
            {
                if (m_Photo != null && m_Photo.Album.CanVisit)
                    return m_Photo.Name + " - " + m_Photo.Album.Name  + " - " + SpaceOwner.Name + "的相册";

                return SpaceOwner.Name + "的相册";
            }
        }

		private bool m_DisplayPasswordForm;

		public bool DisplayPasswordForm
		{
			get { return m_DisplayPasswordForm; }
		}

        private void UpdatePhoto()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplayForForm("UpdatePhoto");

                int? photoID = _Request.Get<int>("id", Method.All);
                string photoName = _Request.Get("photoname", Method.Post);
                string photoDescription = _Request.Get("description", Method.Post);

                if (photoID == null)
                {
                    msgDisplay.AddError(new InvalidParamError("photoid").Message);
                    return;
                }

                try
                {

                    bool success = AlbumBO.Instance.UpdatePhoto(MyUserID, photoID.Value, photoName, photoDescription);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            }
                        );
                    }
                    else
                    {

                    }

                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
            }
        }


		//private void SendComment()
		//{
		//    int? targetID = _Request.Get<int>("targetid", Method.Post);
		//    CommentType type = CommentType.Photo;
		//    string content = _Request.Get("content");
		//    int userID = UserBO.Instance.GetUserID();

		//    using (new ErrorScope())
		//    {
		//        MessageDisplay msgDisplay = new MessageDisplay();

		//        if (targetID == null)
		//        {
		//            msgDisplay.AddError(new InvalidParamError("targetID").Message);
		//            return;
		//        }

		//        try
		//        {
		//            int newCommentId;
		//            bool success = CommentBO.Instance.AddComment(userID, targetID.Value, 0, type, content, IPUtil.GetCurrentIP(),out newCommentId);
		//            if (!success)
		//            {
		//                CatchError<ErrorInfo>(delegate(ErrorInfo error)
		//                {
		//                    msgDisplay.AddError(error);
		//                });
		//            }
		//            else
		//            {

		//            }

		//        }
		//        catch (Exception ex)
		//        {
		//            msgDisplay.AddError(ex.Message);
		//        }
		//    }
		//}

		protected override CommentType CommentType
		{
			get { return CommentType.Photo; }
		}

		private Photo m_Photo;

		public Photo Photo
		{
			get { return m_Photo; }
		}

		private int[] m_AlbumPhotoIDs;

		public int[] AlbumPhotoIDs
		{
			get
			{
				return m_AlbumPhotoIDs;
			}
		}

		public int PhotoIndex
		{
			get
			{
				for (int i = 0; i < m_AlbumPhotoIDs.Length; i++)
				{
					if (m_AlbumPhotoIDs[i] == m_Photo.PhotoID)
					{
						return i + 1;
					}
				}

				return 1;
			}
		}

		public int NextPhotoID
		{
			get 
			{
				for (int i = 0; i < m_AlbumPhotoIDs.Length; i++)
				{
					if (m_AlbumPhotoIDs[i] == m_Photo.PhotoID)
					{
						if (i < m_AlbumPhotoIDs.Length - 1)
							return m_AlbumPhotoIDs[i + 1];
					}
				}

				return m_AlbumPhotoIDs[0];
			}
		}

		public int PreviousPhotoID
		{
			get
			{
				for (int i = 0; i < m_AlbumPhotoIDs.Length; i++)
				{
					if (m_AlbumPhotoIDs[i] == m_Photo.PhotoID)
					{
						if (i > 0)
							return m_AlbumPhotoIDs[i - 1];
					}
				}

				return m_AlbumPhotoIDs[m_AlbumPhotoIDs.Length - 1];
			}
		}

		public Album Album
		{
			get { return m_Photo.Album; }
		}

		private int m_CommentTargetID;

		public override int CommentTargetID
		{
			get
			{
				return m_CommentTargetID;
			}
		}

		private CommentCollection m_CommentList;

		public CommentCollection CommentList
		{
			get { return m_CommentList; }
		}

		private int m_CommentPageSize;

		public int CommentPageSize
		{
			get { return m_CommentPageSize; }
		}

		private int m_TotalCommentCount;

		public int TotalCommentCount
		{
			get { return m_TotalCommentCount; }
		}
    }
}