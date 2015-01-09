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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities 
{
    /// <summary>
    /// 分享
    /// </summary>
    public class Share : IPrimaryKey<int>, ITextRevertable, ITextRevertable2, IFillSimpleUsers
    {

        public Share()
        { }

        public Share(DataReaderWrap readerWrap)
        {
            ShareID = readerWrap.Get<int>("ShareID");
            UserID = readerWrap.Get<int>("UserID");
            //TotalComments = readerWrap.Get<int>("TotalComments");
            TotalComments = readerWrap.Get<int>("CommentCount");

            Type = (ShareType)((int)readerWrap.Get<byte>("Type"));
            PrivacyType = (PrivacyType)(int)readerWrap.Get<byte>("PrivacyType");

            Content = readerWrap.Get<string>("Content");
            Description = readerWrap.Get<string>("Description");
            LastCommentDate = readerWrap.Get<DateTime>("LastCommentDate");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
            KeywordVersion = readerWrap.Get<string>("KeywordVersion");

            Url = readerWrap.Get<string>("Url");
            Subject = readerWrap.Get<string>("Subject");
            ShareCount = readerWrap.Get<int>("ShareCount");
            AgreeCount = readerWrap.Get<int>("AgreeCount");
            OpposeCount = readerWrap.Get<int>("OpposeCount");
            UserID2 = readerWrap.Get<int>("UserID2");
            UserShareID = readerWrap.Get<int>("UserShareID");
        }

        public int ShareID { get; set; }

        [Obsolete]
        public int ID { get { return ShareID; } }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }


        /// <summary>
        /// 评论数
        /// </summary>
        public int TotalComments { get; set; }



        /// <summary>
        /// 分享类型
        /// </summary>
        public ShareType Type { get; set; }


        /// <summary>
        /// 隐私类型
        /// </summary>
        public PrivacyType PrivacyType { get; set; }


        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }


        /// <summary>
        /// 分享者评论
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后评论时间
        /// </summary>
        public DateTime LastCommentDate { get; set; }

        public int AgreeCount { get; set; }

        public int OpposeCount { get; set; }

        public int ShareCount { get; set; }

        public string Subject { get; set; }

        public string Url { get; set; }

        public int UserID2 { get; set; }

        public SimpleUser User2 
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID2);
            }
        }

        public int UserShareID { get; set; }


        #region 扩展字段     
   
        private CommentCollection m_CommentList = new CommentCollection();
        public CommentCollection CommentList
        {
            get
            {
                return m_CommentList;
            }
            set
            {
                m_CommentList = value;
            }
        }


		[Obsolete("For Template Usage Only!")]
        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        private Video m_Video;

		[Obsolete("For Template Usage Only!")]
        public Video Video
        {
            get
            {
                if (m_Video == null)
                {
                    if (ShareType.Video == Type)
                    {
                        m_Video = new Video();
                        StringTable stringTable = StringTable.Parse(Content);
                        if (stringTable.ContainsKey("url"))
                            m_Video.URL = stringTable["url"];
                        if (stringTable.ContainsKey("videoid"))
                            m_Video.VideoID = stringTable["videoid"];
                        if (stringTable.ContainsKey("domain"))
                            m_Video.Domain = stringTable["domain"];
                        if (stringTable.ContainsKey("imgurl"))
                            m_Video.ImgUrl = stringTable["imgurl"];
                        else
                            m_Video.ImgUrl = string.Empty;
                    }
                }
                return m_Video;
            }
        }

        public string OriginalContent { get; private set; }

        public string OriginalSubject { get; private set; }

        /// <summary>
        /// 原始描述内容。未经过关键字过滤的。
        /// </summary>
        public string OriginalDescription { get; private set; }

		[Obsolete("For Template Usage Only!")]
		public string FriendlyCreateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.CreateDate);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanDelete
		{
			get
			{
				return ShareBO.Instance.CheckShareDeletePermission(UserBO.Instance.GetCurrentUserID(), this);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanAccess
		{
			get
			{
				return ShareBO.Instance.CheckShareVisitPermission(UserBO.Instance.GetCurrentUserID(), this);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public string TypeName
		{
			get
			{
				return ShareBO.Instance.GetShareTypeName(this.Type);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool DisplayForFriendOnly
		{
			get { return this.PrivacyType == PrivacyType.FriendVisible; }
		}
        #endregion


        #region ITextRevertable 成员

        public string Text
        {
            get { return this.Content; }
        }

        public void SetNewRevertableText(string text, string textVersion)
        {
            this.Content = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText(string originalText)
        {
            this.OriginalContent = originalText;
        }

        #endregion

        #region ITextRevertable2 成员

        public string Text1
        {
            get { return this.Subject; }
        }

        public string KeywordVersion
        {
            get;
            set;
        }

        public void SetNewRevertableText1(string text, string textVersion)
        {
            this.Subject = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText1(string originalText)
        {
            this.OriginalSubject = originalText;
        }

        public string Text2
        {
            get { return this.Description; }
        }

        public string TextVersion2
        {
            get { return KeywordVersion; }
        }

        public void SetNewRevertableText2(string text, string textVersion)
        {
            this.Description = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText2(string originalText)
        {
            this.OriginalDescription = originalText;
        }

        #endregion

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ShareID;
        }

        #endregion

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            return new int[] { this.UserID, this.UserID2 }; 
        }

        #endregion
    }
    /// <summary>
    /// 通知对象集合
    /// </summary>
    public class ShareCollection : EntityCollectionBase<int, Share>
    {

        public ShareCollection()
        {
        }



        public ShareCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new Share(readerWrap));
            }
        }

		private int m_TotalRecords = -1;

		/// <summary>
		/// 分页查询时的总记录数
		/// </summary>
		public int TotalRecords
		{
			get { return m_TotalRecords; }
			set { m_TotalRecords = value; }
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

        public int[] GetUserShareIDs()
        {
            int[] result = new int[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                result[i] = this[i].UserShareID;
            }

            return result;
        }
	}
}