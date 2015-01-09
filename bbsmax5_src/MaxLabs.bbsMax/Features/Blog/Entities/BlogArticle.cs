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
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Providers;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 日志实体
    /// </summary>
    public class BlogArticle : IPrimaryKey<int>, ITextRevertable2, IFillSimpleUser
    {

		public BlogArticle() { EnableComment = true; }

        public BlogArticle(DataReaderWrap readerWrap)
        {
            this.ArticleID = readerWrap.Get<int>("ArticleID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.LastEditUserID = readerWrap.Get<int>("LastEditUserID");
            this.CategoryID = readerWrap.Get<int>("CategoryID");
            this.TotalViews = readerWrap.Get<int>("TotalViews");
            this.PrivacyType = (PrivacyType)readerWrap.Get<byte>("PrivacyType");
            this.TotalComments = readerWrap.Get<int>("TotalComments");

            this.IsApproved = readerWrap.Get<bool>("IsApproved");
            this.EnableComment = readerWrap.Get<bool>("EnableComment");

            this.CreateIP = readerWrap.Get<string>("CreateIP");

            this.Thumb = readerWrap.Get<string>("Thumb");
            this.Subject = readerWrap.Get<string>("Subject");
            this.Password = readerWrap.Get<string>("Password");

            this.Content = readerWrap.Get<string>("Content");

            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            this.LastCommentDate = readerWrap.Get<DateTime>("LastCommentDate");

			this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");
        }

        #region Properties

        /// <summary>
        /// ArticleID
        /// </summary>
        public int ArticleID { get; set; }

        [Obsolete]
        public int ID { get { return ArticleID; } }

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
        /// 最后编辑者ID
        /// </summary>
        public int LastEditUserID { get; set; }

        public SimpleUser LastEditUser
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.LastEditUserID);
            }
        }

        /// <summary>
        /// 日志分类
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 查看数
        /// </summary>
        public int TotalViews { get; set; }

        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool EnableComment { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        public PrivacyType PrivacyType { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int TotalComments { get; set; }

        /// <summary>
        /// 标题
        /// </summary>.
        public string Subject { get; set; }

        /// <summary>
        /// 标题关键字还原信息
        /// </summary>
        public string KeywordVersion { get; set; }

        /// <summary>
        /// 日志略缩图
        /// </summary>
        public string Thumb { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string CreateIP { get; set; }

        /// <summary>
        /// 凭密码查看时的密码 
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后评论时间
        /// </summary>
        public DateTime LastCommentDate { get; set; }

        /// <summary>
        /// 日志使用的标签
        /// </summary>
        public TagCollection Tags { get; set; }

        /// <summary>
        /// 日志的最近访问者
        /// </summary>
        public BlogArticleVisitorCollection LastVisitors { get; set; }


        #region 扩展字段

        /// <summary>
        /// 摘要内容,一般显示于列表TODO。。。。。
        /// </summary>
        public string SummaryContent
        {
            get
            {
                return StringUtil.CutString(StringUtil.ClearAngleBracket(Content,true), 100);
            }
        }

        /// <summary>
        /// 原始标题内容,不经过标题关键字过滤的
        /// </summary>
        public string OriginalSubject { get; private set; }

        /// <summary>
        /// 原始正文内容,不经过关键字过滤的
        /// </summary>
        public string OriginalContent { get; private set; }

        #endregion

        #endregion

		#region For Template Only

		/// <summary>
		/// 获取当前日志是否只显示给密码持有者
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public bool DisplayForPasswordHolderOnly
		{
			get
			{
				return this.PrivacyType == PrivacyType.NeedPassword;
			}
		}

		/// <summary>
		/// 获取当前日志是否只显示给作者的好友
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public bool DisplayForFriendOnly
		{
			get
			{
				return this.PrivacyType == PrivacyType.FriendVisible;
			}
		}

		/// <summary>
		/// 获取当前日志是否只显示给作者
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public bool DisplayForOwnerOnly
		{
			get
			{
				return this.PrivacyType == PrivacyType.SelfVisible;
			}
		}

		/// <summary>
		/// 获取当前访问者是否可以阅读当前日志
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public bool CanVisit
		{
			get
			{
				return BlogBO.Instance.CheckBlogArticleVisitPermission(UserBO.Instance.GetCurrentUserID(), this);
			}
		}

		/// <summary>
		/// 获取当前访问者是否可以编辑当前日志
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public bool CanEdit 
		{
			get
			{
				return BlogBO.Instance.CheckBlogArticleEditPermission(UserBO.Instance.GetCurrentUserID(), this);
			}
		}

		/// <summary>
		/// 获取当前访问者是否可以删除当前日志
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public bool CanDelete 
		{ 
			get 
			{ 
				return BlogBO.Instance.CheckBlogArticleDeletePermission(UserBO.Instance.GetCurrentUserID(), this); 
			} 
		}

		/// <summary>
		/// 获取友好的创建时间
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public string FriendlyCreateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.CreateDate);
			}
		}

		/// <summary>
		/// 获取有好的更新时间
		/// </summary>
		[Obsolete("For Template Usage Only!")]
		public string FriendlyUpdateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.UpdateDate);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanDisplayThumb { get { return string.IsNullOrEmpty(this.Thumb) == false; } }

		[Obsolete("For Template Usage Only!")]
		public string CategoryName 
		{
			get
			{
				BlogCategory category = BlogBO.Instance.GetUserBlogCategory(this.UserID, this.CategoryID);

				if (category != null)
					return category.Name;

				return string.Empty;
			}
		}

		#endregion

		#region IPrimaryKey<int> Members

		public int GetKey()
        {
            return this.ArticleID;
        }

        #endregion

        #region ITextRevertable2 成员

        public string Text1
        {
            get { return Subject; }
        }

        public void SetNewRevertableText1(string text, string textVersion)
        {
            Subject = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText1(string originalText)
        {
            OriginalSubject = originalText;
        }

        public string Text2
        {
            get { return Content; }
        }

        public string TextVersion2
        {
            get { return KeywordVersion; }
        }

        public void SetNewRevertableText2(string text, string textVersion)
        {
            Content = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText2(string originalText)
        {
            OriginalContent = originalText;
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
    /// 日志集合
    /// </summary>
    public class BlogArticleCollection : EntityCollectionBase<int, BlogArticle>
    {
        public BlogArticleCollection() 
		{ 
		}

        public BlogArticleCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new BlogArticle(readerWrap));
            }
		}

        public IEnumerable<int> GetUserIds()
        {
            List<int> userIds = new List<int>();

            foreach (BlogArticle article in this)
            {
                if (userIds.Contains(article.UserID) == false)
                    userIds.Add(article.UserID);
            }

            return userIds;
        }
    }
}