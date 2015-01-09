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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 相片
    /// </summary>
    public class Photo : IPrimaryKey<int>, ITextRevertable2, IFillSimpleUser
    {


        public Photo() { }

        public Photo(DataReaderWrap readerWrap)
        {
            this.PhotoID = readerWrap.Get<int>("PhotoID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.AlbumID = readerWrap.Get<int>("AlbumID");
            this.TotalViews = readerWrap.Get<int>("TotalViews");
            this.TotalComments = readerWrap.Get<int>("TotalComments");

            this.FileID = readerWrap.Get<string>("FileID");
			this.FileType = readerWrap.Get<string>("FileType");
            this.Exif = readerWrap.Get<string>("Exif");
            
            this.Name = readerWrap.Get<string>("Name");
            this.Description = readerWrap.Get<string>("Description");

            this.CreateIP = readerWrap.Get<string>("CreateIP");

            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");

            this.LastEditUserID = readerWrap.Get<int>("LastEditUserID");

			this.ThumbPath = readerWrap.Get<string>("ThumbPath");
			this.ThumbWidth = readerWrap.Get<int>("ThumbWidth");
			this.ThumbHeight = readerWrap.Get<int>("ThumbHeight");

            this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");

            this.FileSize = readerWrap.Get<long>("FileSize");
        }

        /// <summary>
        /// 照片ID
        /// </summary>
        public int PhotoID { get; set; }

        [Obsolete]
        public int ID { get { return PhotoID; } }

        /// <summary>
        /// 相片所有者ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 相片所属相册ID
        /// </summary>
        public int AlbumID { get; set; }

        /// <summary>
        /// 相片的浏览数
        /// </summary>
        public int TotalViews { get; set; }

        /// <summary>
        /// 相片的评论数
        /// </summary>
        public int TotalComments { get; set; }

        /// <summary>
        /// 相片对应的物理文件ID
        /// </summary>
        public string FileID { get; set; }

		/// <summary>
		/// 文件类型，如：.jpg、.jpeg、.png、.gif等
		/// </summary>
		public string FileType { get; set; }

        /// <summary>
        /// 相片Exif参数信息
        /// </summary>
        public string Exif { get; set; }

        /// <summary>
        /// 相片名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 相片描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 相片创建者IP
        /// </summary>
        public string CreateIP { get; set; }

        /// <summary>
        /// 相片创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 相片更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 最后更新者ID
        /// </summary>
        public int LastEditUserID { get; set; }

		public string ThumbPath { get; set; }

		public int ThumbWidth { get; set; }

		public int ThumbHeight { get; set; }

        /// <summary>
        /// 关键字版本
        /// </summary>
        public string KeywordVersion { get; set; }

        public long FileSize { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return PhotoID;
        }

        #endregion

        #region 扩展属性

        /// <summary>
        /// 相片的原始名称，未经关键字过滤的
        /// </summary>
        public string OriginalName { get; private set; }

        /// <summary>
        /// 相片原始描述,未经过关键字过滤的
        /// </summary>
        public string OriginalDescription { get; private set; }

		public string Src
		{
			get
			{
				return BbsRouter.GetUrl("handler/down", "action=album&id=" + this.PhotoID);
			}
		}

		public string DownloadUrl
		{
			get
			{
				return BbsRouter.GetUrl("handler/down", "action=album&mode=download&id=" + this.PhotoID);
			}
		}

		public string ThumbSrc 
		{
			get
			{
				if (string.IsNullOrEmpty(this.ThumbPath))
					return "";

				return AlbumBO.Instance.GetPhotoThumbSrc(this.ThumbPath);
			}
		}

        public SimpleUser LastEditUser
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.LastEditUserID);
            }
        }

		private Album m_Album;

		public Album Album
		{
			get 
			{
				if (m_Album == null)
					m_Album = AlbumBO.Instance.GetAlbum(this.AlbumID);

				return m_Album;
			}
		}

		public void SetAlbum(Album album)
		{
			m_Album = album;
		}

		public string FriendlyCreateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.CreateDate);
			}
	
		}

		public SimpleUser User
		{
			get
			{
				return UserBO.Instance.GetFilledSimpleUser(this.UserID);
			}
		}

        #endregion

        #region ITextRevertable2 成员

        public string Text1
        {
            get { return Name; }
        }

        public void SetNewRevertableText1(string text, string textVersion)
        {
            Name = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText1(string originalText)
        {
            OriginalName = originalText;
        }

        public string Text2
        {
            get { return Description; }
        }

        public string TextVersion2
        {
            get { return KeywordVersion; }
        }

        public void SetNewRevertableText2(string text, string textVersion)
        {
            Description = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText2(string originalText)
        {
            OriginalDescription = originalText;
        }

        #endregion

		#region IFillSimpleUser 成员

		public int GetUserIDForFill()
		{
			return this.UserID;
		}

		#endregion
	}


    /// <summary>
    /// 相片集合
    /// </summary>
    public class PhotoCollection : EntityCollectionBase<int, Photo>
    {
        //private int m_TotalRecords = -1;

        ///// <summary>
        ///// 分页查询时的总记录数
        ///// </summary>
        //public int TotalRecords
        //{
        //    get { return m_TotalRecords; }
        //    set { m_TotalRecords = value; }
        //}

        public PhotoCollection() { }

        public PhotoCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Photo photo = new Photo(readerWrap);
                this.Add(photo);
            }
        }

		public int[] GetUserIDs()
		{
			int[] result = new int[this.Count];

			for(int i=0; i<this.Count; i++)
			{
				result[i] = this[i].UserID;
			}

			return result;
		}
	}
}