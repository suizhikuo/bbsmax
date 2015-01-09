//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;


namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 日志分类实体
    /// </summary>
	public class BlogCategory : IPrimaryKey<int>, ITextRevertable, IFillSimpleUser
    {

        public BlogCategory() { }

        public BlogCategory(DataReaderWrap readerWrap) 
        {
            this.CategoryID = readerWrap.Get<int>("CategoryID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.TotalArticles = readerWrap.Get<int>("TotalArticles");
            this.Name = readerWrap.Get<string>("Name");
			this.CreateDate = readerWrap.Get<DateTime>("CreateDate");

			this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");
        }

        public int CategoryID { get; set; }

        public int ID { get { return CategoryID; } }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 日志文章总数
        /// </summary>
        public int TotalArticles { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

		#region For Template Only

		private SpacePermissionSet Permission
		{
			get { return AllSettings.Current.SpacePermissionSet; }
		}

        private BackendPermissions ManagePermission
		{
			get { return AllSettings.Current.BackendPermissions; }
		}

		private bool CanManage
		{
			get
			{
				int userID = UserBO.Instance.GetCurrentUserID();

				if (this.UserID == userID)
				{
					if (this.Permission.Can(userID, SpacePermissionSet.Action.UseBlog))
						return true;
				}
				else
				{
                    if (this.ManagePermission.Can(userID, BackendPermissions.ActionWithTarget.Manage_Blog, this.UserID))
						return true;
				}

				return false;
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanEdit { get { return CanManage; } }

		[Obsolete("For Template Usage Only!")]
		public bool CanDelete { get { return CanManage; } }

		[Obsolete("For Template Usage Only!")]
		public string FriendlyCreateDate
		{
			get { return DateTimeUtil.GetFriendlyDate(this.CreateDate); }
		}

		[Obsolete("For Template Usage Only!")]
		public SimpleUser User
		{
			get { return UserBO.Instance.GetSimpleUser(this.UserID); }
		}

		#endregion

		#region IPrimaryKey<int> 成员

		public int GetKey()
        {
            return CategoryID;
        }

        #endregion

		#region ITextRevertable 成员

		public string OriginalName
		{
			get;
			private set;
		}

		public string KeywordVersion
		{
			get;
			private set;
		}

		public string Text
		{
			get { return this.Name; }
		}

		public void SetNewRevertableText(string text, string textVersion)
		{
			this.Name = text;
			this.KeywordVersion = textVersion;
		}

		public void SetOriginalText(string originalText)
		{
			this.OriginalName = originalText;
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
    /// 日志分类集合
    /// </summary>
    public class BlogCategoryCollection : EntityCollectionBase<int, BlogCategory>
    {
        #region Constructors

        public BlogCategoryCollection() { }

        public BlogCategoryCollection(DataReaderWrap readerWrap) 
        {
            while (readerWrap.Next)
            {
                this.Add(new BlogCategory(readerWrap));
            }
        }

        #endregion
    }
}