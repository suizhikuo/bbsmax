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
using MaxLabs.bbsMax.Entities;
using System.Collections;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.App_Album
{
    public partial class index : CenterAlbumPageBase
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

        private int? m_AlbumTotalCount;
        private AlbumCollection m_AlbumList = null;
		private int m_AlbumListPageSize;

        protected void Page_Load(object sender, EventArgs e)
        {
            int pageNumber = _Request.Get<int>("page", Method.Get, 1);

			m_AlbumListPageSize = 20;

            m_View = _Request.Get("view", Method.Get, "");

            if (IsSpace)
            {
                m_AlbumList = AlbumBO.Instance.GetUserAlbums(MyUserID, AppOwnerUserID, pageNumber, m_AlbumListPageSize);
                m_AlbumTotalCount = m_AlbumList.TotalRecords;

                m_CommentList = CommentBO.Instance.GetLastestCommentsForSomeone(AppOwnerUserID, CommentType.Photo, 10);

                UserBO.Instance.WaitForFillSimpleUsers<Album>(m_AlbumList);
                UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);
            }
            else
            {
                if (SelectedMy)
                {
                    m_AlbumList = AlbumBO.Instance.GetUserAlbums(MyUserID, MyUserID, pageNumber, m_AlbumListPageSize);
                    m_AlbumTotalCount = m_AlbumList.TotalRecords;

                    m_CommentList = CommentBO.Instance.GetLastestCommentsForSomeone(MyUserID, CommentType.Photo, 10);

                    UserBO.Instance.WaitForFillSimpleUsers<Album>(m_AlbumList);
                    UserBO.Instance.WaitForFillSimpleUsers<Comment>(m_CommentList);
                }
                else if (SelectedFriend)
                {
                    m_AlbumList = AlbumBO.Instance.GetFriendAlbums(MyUserID, pageNumber, m_AlbumListPageSize);
                    m_AlbumTotalCount = m_AlbumList.TotalRecords;

                    UserBO.Instance.WaitForFillSimpleUsers<Album>(m_AlbumList);
                }
                else if (SelectedEveryone)
                {
                    m_AlbumList = AlbumBO.Instance.GetEveryoneAlbums(pageNumber, m_AlbumListPageSize);
                    m_AlbumTotalCount = m_AlbumList.TotalRecords;

                    UserBO.Instance.WaitForFillSimpleUsers<Album>(m_AlbumList);
                }
            }

            SetPager("pager1", null, pageNumber, AlbumListPageSize, AlbumTotalCount);

            if (IsSpace == false)
            {
                AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/album/index"));
                if (SelectedMy)
                    AddNavigationItem(string.Concat("我的", FunctionName));
                else if (SelectedFriend)
                    AddNavigationItem(string.Concat("好友的", FunctionName));
                else if (SelectedEveryone)
                    AddNavigationItem(string.Concat("大家的", FunctionName));
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"),UrlHelper.GetSpaceUrl(AppOwner.UserID));
                AddNavigationItem(string.Concat("主人的", FunctionName));
            }

        }


        protected override string PageTitle
        {
            get
            {
                if (IsSpace)
                {
                    return string.Concat(AppOwner.Name, " - ", FunctionName, " - ", base.PageTitle);
                }
                else if (SelectedMy)
                    return string.Concat("我的", FunctionName, " - ", base.PageTitle);
                else if (SelectedEveryone)
                    return string.Concat("大家的", FunctionName, " - ", base.PageTitle);
                else //if (SelectedFriend)
                    return string.Concat("好友的", FunctionName, " - ", base.PageTitle);
            }
        }

        private Hashtable m_PhotoList;

        public PhotoCollection PhotoList(int albumID)
        {
            if (m_PhotoList == null)
            {
                m_PhotoList = AlbumBO.Instance.GetPhotos(m_AlbumList.GetKeys(), 3);
            }

            return m_PhotoList[albumID] as PhotoCollection;
        }

        /// <summary>
        /// 每页显示的条数
        /// </summary>
		protected int AlbumListPageSize { get { return m_AlbumListPageSize; } }

        protected int AlbumTotalCount { get { return m_AlbumList.TotalRecords; } }

        /// <summary>
        /// 相册列表
        /// </summary>
        protected AlbumCollection AlbumList
        {
            get
            {
                return m_AlbumList;
            }
        }

		private CommentCollection m_CommentList;

		protected CommentCollection CommentList
		{
			get { return m_CommentList; }
		}

		private string m_View;

		/// <summary>
		/// 是否显是显示“我的相册”
		/// </summary>
		public bool SelectedMy
		{
			get
            {
                return SelectedFriend == false && SelectedEveryone == false && SelectedVisited == false;
			}
		}

		/// <summary>
		/// 是否显是显示“我访问过的相册”
		/// </summary>
		public bool SelectedVisited
		{
			get
			{
				return string.Compare(m_View, "visited", true) == 0;
			}
		}

		/// <summary>
		/// 是否显是显示“好友的相册”
		/// </summary>
		public bool SelectedFriend
		{
			get
			{ return string.Compare(m_View, "friend", true) == 0; }
		}


		/// <summary>
		/// 是否显是显示“大家的相册”
		/// </summary>
		public bool SelectedEveryone
		{
			get
			{ return string.Compare(m_View, "everyone", true) == 0; }
		}


        private bool? m_CanManageAlbums;
        protected bool CanManageAlbums
        {
            get
            {
                if (m_CanManageAlbums == null)
                {
                    m_CanManageAlbums = AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Album, AppOwnerUserID);
                }

                return m_CanManageAlbums.Value;
            }
        }

        protected bool CanSeePhoto(Album album)
        {
            if (IsSpace == false && SelectedMy)
                return true;

            if (album.UserID == MyUserID)
                return true;

            if (album.PrivacyType == PrivacyType.AllVisible)
                return true;

            if (IsSpace)
            {
                if (CanManageAlbums)
                    return true;
            }

            if (album.PrivacyType == PrivacyType.FriendVisible)
            {
                if (SelectedFriend)
                    return true;
                else if (FriendBO.Instance.IsFriend(MyUserID, album.UserID))
                    return true;
            }
            else if (album.PrivacyType == PrivacyType.NeedPassword)
            {
                if (AlbumBO.Instance.HasAlbumPassword(My, album.AlbumID))
                    return true;
            }

            if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Manage_Album, album.UserID))
                return true;

            return false;
        }
    }
}