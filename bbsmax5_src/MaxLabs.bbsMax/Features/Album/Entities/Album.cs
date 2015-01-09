//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Providers;


namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 相册
    /// </summary>
    public class Album : IPrimaryKey<int>, ITextRevertable2, IFillSimpleUser
    {

        public Album() { }

        public Album(DataReaderWrap readerWrap)
        {
            this.AlbumID = readerWrap.Get<int>("AlbumID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.PrivacyType = (PrivacyType)readerWrap.Get<byte>("PrivacyType");
            this.TotalPhotos = readerWrap.Get<int>("TotalPhotos");

            this.Name = readerWrap.Get<string>("Name");
            this.Cover = readerWrap.Get<string>("Cover");
            this.Password = readerWrap.Get<string>("Password");


            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");

            this.LastEditUserID = readerWrap.Get<int>("LastEditUserID");

            this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");

            this.Description = readerWrap.Get<string>("Description");
        }

        /// <summary>
        /// 相册ID
        /// </summary>
        public int AlbumID { get; set; }

        [Obsolete]
        public int ID { get { return AlbumID; } }

        /// <summary>
        /// 用户ID
        /// </summary>
		public int UserID { get; set; }

		public SimpleUser User
		{
			get
			{
				return UserBO.Instance.GetFilledSimpleUser(this.UserID);
			}
		}

        /// <summary>
        /// 图片数量
        /// </summary>
        public int TotalPhotos { get; set; }

        /// <summary>
        /// 权限类型 0.所有用户见 1.全好友可见 2.仅自己可见 3.凭密码查看
        /// </summary>
        public PrivacyType PrivacyType { get; set; }

        /// <summary>
        /// 相册名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 相册名称的版本，用来检查是否需要更新关键字
        /// </summary>
        public string KeywordVersion { get; set; }

        /// <summary>
        /// 相册封面
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 访问密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

		/// <summary>
		/// 最后更新者ID
		/// </summary>
		public int LastEditUserID { get; set; }

        public string Description { get; set; }

        public string OriginalDescription { get; set; }

		public SimpleUser LastEditUser
		{
			get
			{
				return UserBO.Instance.GetFilledSimpleUser(this.LastEditUserID);
			}
		}
        
        public int GetKey()
        {
            return AlbumID;
        }

		#region TemplateOnly

		[Obsolete("For Template Usage Only!")]
		public bool DisplayForPasswordHolderOnly
		{
			get { return this.PrivacyType == PrivacyType.NeedPassword; }
		}

		[Obsolete("For Template Usage Only!")]
		public bool DisplayForFriendOnly
		{
			get { return this.PrivacyType == PrivacyType.FriendVisible; }
		}

		[Obsolete("For Template Usage Only!")]
		public bool DisplayForOwnerOnly
		{
			get { return this.PrivacyType == PrivacyType.SelfVisible; }
		}

		[Obsolete("For Template Usage Only!")]
		public bool IsBelongsToVisitor
		{
			get
			{
				return UserBO.Instance.GetCurrentUserID() == this.UserID;
			}
		}

        [Obsolete("For Template Usage Only!")]
        public bool CanVisit
        {
            get
            {
				return AlbumBO.Instance.CheckAlbumVisitPermission(UserBO.Instance.GetCurrentUserID(), this);
            }
        }

		[Obsolete("For Template Usage Only!")]
		public bool CanEdit
		{
			get
			{
				return AlbumBO.Instance.CheckAlbumEditPermission(UserBO.Instance.GetCurrentUserID(), this);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanDelete
		{
			get 
			{
				return AlbumBO.Instance.CheckAlbumDeletePermission(UserBO.Instance.GetCurrentUserID(), this);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanUploadPhoto
		{
			get { return UserID == MaxLabs.bbsMax.Entities.User.CurrentID; } //老达TODO:还没整理到 }
		}

		private TagCollection m_TagList;

		[Obsolete("For Template Usage Only!")]
		public TagCollection TagList
		{
			get 
			{
				if (m_TagList == null)
				{
					m_TagList = TagBO.Instance.GetTags(TagType.Blog, this.ID);
				}

				return m_TagList; 
			}
		}

		public void SetTagList(TagCollection taglist)
		{
			m_TagList = taglist;
		}

		[Obsolete("For Template Usage Only!")]
		public string FriendlyCreateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.CreateDate);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public string FriendlyUpdateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.UpdateDate);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public string CoverSrc
		{
			get
			{
				//if (string.IsNullOrEmpty(this.Cover) || CanVisit == false)
					//return UrlUtil.JoinUrl(Globals.AppRoot, @"max-assets\images\default_photo.gif");

                if (string.IsNullOrEmpty(this.Cover))
                    return UrlUtil.JoinUrl(Globals.AppRoot, @"max-assets\images\default_photo.gif");

				return UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Album_Thumbs), this.Cover);
			}
		}

		#endregion

        #region 扩展属性

        public string OriginalName { get; private set; }

        #endregion

		#region IFillSimpleUser 成员

		public int GetUserIDForFill()
		{
			return this.UserID;
		}

		#endregion

        #region ITextRevertable2 成员

        public string Text1
        {
            get { return this.Name; }
        }

        public void SetNewRevertableText1(string text, string textVersion)
        {
            this.Name = text;
            this.KeywordVersion = textVersion;
        }

        public void SetOriginalText1(string originalText)
        {
            this.OriginalName = originalText;
        }

        public string Text2
        {
            get { return this.Description; }
        }

        public string TextVersion2
        {
            get { return this.KeywordVersion; }
        }

        public void SetNewRevertableText2(string text, string textVersion)
        {
            this.Description = text;
            this.KeywordVersion = textVersion;
        }

        public void SetOriginalText2(string originalText)
        {
            this.OriginalDescription = originalText;
        }

        #endregion
    }

    public class AlbumCollection : EntityCollectionBase<int, Album>
    {
        public AlbumCollection() { }

        public AlbumCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Album album = new Album(readerWrap);

                this.Add(album);
            }
        }


		public IEnumerable<int> GetUserIds()
		{
			List<int> userIds = new List<int>();

			foreach (Album album in this)
			{
				if (userIds.Contains(album.UserID) == false)
					userIds.Add(album.UserID);
			}

			return userIds;
		}
    }
}