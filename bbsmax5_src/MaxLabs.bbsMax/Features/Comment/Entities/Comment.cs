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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class Comment : ITextRevertable, IPrimaryKey<int>, IFillSimpleUser
    {
        public Comment()
        { }

        public Comment(DataReaderWrap readerWrap)
        {
            this.CommentID = readerWrap.Get<int>("CommentID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.TargetID = readerWrap.Get<int>("TargetID");
            this.TargetUserID = readerWrap.Get<int>("TargetUserID");
            this.LastEditUserID = readerWrap.Get<int>("LastEditUserID");
            this.IsApproved = readerWrap.Get<bool>("IsApproved");
            this.Type = (CommentType)readerWrap.Get<int>("Type");
            this.Content = readerWrap.Get<string>("Content");
            this.CreateIP = readerWrap.Get<string>("CreateIP");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");

			//给视图用的
			if (readerWrap.ContainsField("TargetName"))
				this.TargetName = readerWrap.Get<string>("TargetName");
        }

        public int CommentID { get; set; }

        [Obsolete]
        public int ID { get { return CommentID; } }

        /// <summary>
        /// 评论者ID
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
        /// 被评论者对象ID
        /// </summary>
        public int TargetID { get; set; }

        /// <summary>
        /// 被评论用户ID
        /// </summary>
        public int TargetUserID { get; set; }


        /// <summary>
        /// 最后编辑者
        /// </summary>
        public int LastEditUserID { get; set; }

        /// <summary>
        /// 是否审核通过
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// 评论类型日志 记录 相册 留言 分享
        /// </summary>
        public CommentType Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 内容的版本信息
        /// </summary>
        public string KeywordVersion { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string CreateIP { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }

		public string TargetName { get; set; }


        #region 扩展属性

        public string OriginalContent { get; private set; }

        #endregion

        #region For Template Only

        private bool? m_CanManage;

		private bool CanManage
		{
			get
			{
				if (m_CanManage.HasValue == false)
				{
                    m_CanManage = CommentBO.Instance.ManagePermission.Can(Entities.User.Current, MaxLabs.bbsMax.Settings.BackendPermissions.ActionWithTarget.Manage_Comment, this.UserID);
				}

				return m_CanManage.GetValueOrDefault();
			}
		}

		[Obsolete("For Template Usage Only!")]
		public bool CanEdit { get { return CommentBO.Instance.CheckCommentEditPermission(UserBO.Instance.GetCurrentUserID(), this); } }

		[Obsolete("For Template Usage Only!")]
		public bool CanDelete { get { return CommentBO.Instance.CheckCommentDeletePermission(UserBO.Instance.GetCurrentUserID(), this); } }

		[Obsolete("For Template Usage Only!")]
		public bool CanReply 
		{
			get
			{
				int visitorID = UserBO.Instance.GetCurrentUserID();

				return CommentBO.Instance.Permission.Can(visitorID, MaxLabs.bbsMax.Settings.SpacePermissionSet.Action.AddComment);
			}
		}

		[Obsolete("For Template Usage Only!")]
		public string FriendlyCreateDate
		{
			get
			{
				return DateTimeUtil.GetFriendlyDate(this.CreateDate);
			}
		}
		#endregion

        #region ITextRevertable 成员

        public string Text
        {
            get { return Content; }
        }

        public void SetNewRevertableText(string text, string textVersion)
        {
            Content = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText(string originalText)
        {
            OriginalContent = originalText;
        }

        #endregion

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return CommentID;
        }

        #endregion

		#region IFillSimpleUser 成员

		public int GetUserIDForFill()
		{
			return this.UserID;
		}

		#endregion
	}

	public class CommentCollection : EntityCollectionBase<int, Comment>
    {
        public CommentCollection()
        { }

        public CommentCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Comment comment = new Comment(readerWrap);
                this.Add(comment);
            }
        }


		public IEnumerable<int> GetUserIds()
		{
			List<int> userIds = new List<int>();

			foreach (Comment comment in this)
			{
				if (userIds.Contains(comment.UserID) == false)
					userIds.Add(comment.UserID);
			}

			return userIds;
		}
	}

    public class SearchCommentCondition
    {
        /// <summary>
        /// 评论类型
        /// </summary>
        public CommentType Type { get; set; }

        /// <summary>
        /// 评论者用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 被评论者用户名
        /// </summary>
        public string TargetUsername { get; set; }

        /// <summary>
        /// 评论起始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 评论结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 是否审核通过
        /// </summary>
        public int ApproveType { get; set; }
    }
}