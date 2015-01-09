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
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;


namespace MaxLabs.bbsMax.Web.App_Album
{
	public partial class list : CenterAlbumPageBase
    {
        protected override string PageName
        {
            get
            {
                return "album";
            }
        }

        protected override string NavigationKey
        {
            get
            {
                return "album";
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Album == null)
            {
                ShowError("您要查看的相册不存在");
                return;
            }
            base.OnLoad(e);
        }

        private PhotoCollection m_PhotoList;
        private int m_TotalPhotoCount;
        private int m_PageNumber;
        private int m_PhotoListPageSize;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AlbumID < 1)
                ShowError(new InvalidParamError("id"));

            if (Album == null)
                ShowError("您要查看的相册不存在");
			

			if (_Request.IsClick("Save") || _Request.IsClick("SaveAndUpload"))
			{
				SavePhotos();
			}
			else if (_Request.IsClick("Move"))
			{
				MovePhotos();
			}
			else if (_Request.IsClick("Cover"))
			{
				SetAlbumCover();
			}
			else if(_Request.IsClick("DeleteAlbum"))
			{
				DeleteAlbum();
			}

            if (_Request.IsClick("submitPassword"))
            {
                ProcessPassword();
            }

            using (ErrorScope es = new ErrorScope())
            {
                bool canVisit = AlbumBO.Instance.CanVisitAlbum(My, Album);
                if (canVisit == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        if (error is NoPermissionVisitAlbumBeacuseNeedPasswordError)
                            IsShowPasswordBox = true;
                        else
                            ShowError(error.Message);
                    });
                }
            }






            ////指定好友可见  暂时还没有该功能
            //if (Article.PrivacyType == MaxLabs.bbsMax.Enums.PrivacyType.AppointUser)
            //{ 
            //}



            m_PageNumber = _Request.Get<int>("page", Method.Get, 1);

            m_PhotoListPageSize = 20;

            if (string.IsNullOrEmpty(_Request.Get("photoids")))
            {
                m_PhotoList = AlbumBO.Instance.GetPhotos(AlbumID, null, m_PageNumber, m_PhotoListPageSize);
                //m_Album = AlbumBO.Instance.GetAlbumForVisitWithPhotos(MyUserID, m_AlbumID, null, null, m_PageNumber, m_PhotoListPageSize, out m_PhotoList);
            }
            else
            {
                int[] photoids = StringUtil.Split<int>(_Request.Get("photoids"));

                m_PhotoList = AlbumBO.Instance.GetPhotos(AlbumID, photoids, m_PageNumber, m_PhotoListPageSize);
                //m_Album = AlbumBO.Instance.GetAlbumForVisitWithPhotos(MyUserID, m_AlbumID, null, photoids, m_PageNumber, m_PhotoListPageSize, out m_PhotoList);
            }


			if (m_PhotoList != null)  
                m_TotalPhotoCount = m_PhotoList.TotalRecords;

            SetPager("pager2", null, m_PageNumber, PhotoListPageSize, TotalPhotoCount);

            if (IsSpace == false)
            {
                AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/album/index"));
                AddNavigationItem(string.Concat("我的", FunctionName), BbsRouter.GetUrl("app/album/index"));
                AddNavigationItem(Album.Name);
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"), UrlHelper.GetSpaceUrl(AppOwnerUserID));
                AddNavigationItem(string.Concat("主人的", FunctionName),UrlHelper.GetPhotoIndexUrl(AppOwnerUserID));
                AddNavigationItem(Album.Name);
            }
        }

        protected override string PageTitle
        {
            get
            {
                if (IsSpace)
                {
                    return string.Concat(Album.Name, " - ", FunctionName, " - ", AppOwner.Name, " - ", base.PageTitle);
                }
                else
                {
                    return string.Concat(Album.Name, " - ", "我的" + FunctionName, " - ", base.PageTitle);
                }
            }
        }

        protected override int AppOwnerUserID
        {
            get
            {
                if (Album == null)
                    return 0;

                return Album.UserID;
            }
        }

		private AlbumCollection m_AlbumList;

		public AlbumCollection AlbumList
		{
			get 
            {
                if (m_AlbumList == null)
                    m_AlbumList = AlbumBO.Instance.GetUserAlbums(MyUserID);

                return m_AlbumList; 
            }
		}


        private int? m_AlbumID;
        protected int AlbumID 
        { 
            get 
            {
                if (m_AlbumID == null)
                {
                    m_AlbumID = _Request.Get<int>("id", Method.Get, 0);
                }
                return m_AlbumID.Value; 
            } 
        }

        private Album m_Album;
        protected Album Album 
        { 
            get 
            {
                if (m_Album == null)
                {
                    m_Album = AlbumBO.Instance.GetAlbum(AlbumID);
                }

                return m_Album;
            } 
        }

        
        protected PhotoCollection PhotoList
        {
            get
            {
                return m_PhotoList;
            }
        }

		protected int PhotoListPageSize
		{
			get { return m_PhotoListPageSize; }
		}

		protected int TotalPhotoCount
		{
			get { return m_TotalPhotoCount; }
		}

        private void SavePhotos()
        {
			using (ErrorScope es = new ErrorScope())
            {
                MessageDisplay md = CreateMessageDisplay();

				string[] ids = _Request.Get("PhotoID", Method.Post, string.Empty).Split(',');

				if (ids.Length == 1 && ids[0] == string.Empty)
					return;

				int[] photoIDs = new int[ids.Length];
				string[] photoNames = new string[ids.Length];
				string[] photoDescs = new string[ids.Length];

				for (int i = 0; i < ids.Length; i++)
				{
					photoIDs[i] = int.Parse(ids[i]);

					photoNames[i] = _Request.Get("PhotoName_" + ids[i], Method.Post, "未命名图片");
					photoDescs[i] = _Request.Get("PhotoDesc_" + ids[i], Method.Post, "");
				}

                if (AlbumBO.Instance.UpdatePhotos(MyUserID, AlbumID, photoIDs, photoNames, photoDescs))
				{
                    if (_Request.IsClick("Save"))
                    {
                        BbsRouter.JumpTo("app/album/list", "id=" + AlbumID);
                    }
                    else
                    {
                        BbsRouter.JumpTo("app/album/upload", "id=" + AlbumID);
                    }
				}
				else
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
                        md.AddError(error);
					});
				}
			}
        }

        private void MovePhotos()
        {
			using (ErrorScope es = new ErrorScope())
			{
				MessageDisplay md = CreateMessageDisplay();

				int? desAlbumID = _Request.Get<int>("DesAlbumID", Method.Post);

				if (desAlbumID.HasValue)
				{
                    string[] ids = _Request.Get("photoids", Method.Post, string.Empty).Split(',');

					if (ids.Length == 0)
					{
						md.AddError("请先选择要移动的相片");
						return;
					}

					if (ids.Length == 1 && ids[0] == string.Empty)
					{
						md.AddError("请先选择要移动的相片");
						return;
					}

					int[] photoIDs = new int[ids.Length];

					for (int i = 0; i < ids.Length; i++)
					{
						photoIDs[i] = int.Parse(ids[i]);
					}

					if (AlbumBO.Instance.MovePhotos(MyUserID, AlbumID, desAlbumID.Value, photoIDs))
                    {
                        BbsRouter.JumpTo("app/album/list", "mode=manage&id=" + AlbumID);
					}
					else
					{
						es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
						{
							ShowError(error);
						});
					}
				}
				else
				{
					md.AddError("请选择目标相册");
				}
			}
        }

		private void SetAlbumCover()
		{
			using (ErrorScope es = new ErrorScope())
			{
				MessageDisplay md = CreateMessageDisplay();

				int? photoID = _Request.Get<int>("CoverPhotoID");

				if(photoID.HasValue)
				{
                    if (AlbumBO.Instance.UpdateAlbumCover(MyUserID, AlbumID, photoID.Value) == false)
					{
						es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
						{
							ShowError(error);
						});
					}
				}
			}
		}

		private void DeleteAlbum()
		{
			using (ErrorScope es = new ErrorScope())
			{
                if (AlbumBO.Instance.DeleteAlbum(MyUserID, AlbumID, true))
				{
					ShowSuccess("删除相册完成，将自动跳转到相册列表页", BbsRouter.GetUrl("app/album/index"));
				}
				else
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
						ShowError(error);
					});
				}
			}
		}

        private bool? m_CanManageUserAlbum;
        protected bool CanManageUserAlbum
        {
            get
            {
                if (m_CanManageUserAlbum == null)
                {
                    if(AllSettings.Current.BackendPermissions.Can(My,BackendPermissions.ActionWithTarget.Manage_Album,Album.UserID))
                        m_CanManageUserAlbum = true;
                    else
                        m_CanManageUserAlbum = false;
                }
                return m_CanManageUserAlbum.Value;
            }
        }


        protected bool IsShowPasswordBox;

        private void ProcessPassword()
        {
            MessageDisplay msgDisplay = CreateMessageDisplayForForm("passwordform", "password");

            string password = _Request.Get("password", Method.Post, string.Empty);

            if (AlbumBO.Instance.CheckAlbumPassword(My, AlbumID, password) == false)
            {
                msgDisplay.AddError("password", "密码错误");
            }
        }
    }
}