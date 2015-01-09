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
    public class Doing : IPrimaryKey<int>, ITextRevertable, IFillSimpleUser
    {
        public Doing()
        { }

        public Doing(DataReaderWrap readerWrap)
        {
            this.DoingID = readerWrap.Get<int>("DoingID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.TotalComments = readerWrap.Get<int>("TotalComments");
            this.CreateIP = readerWrap.Get<string>("CreateIP");
            this.Content = readerWrap.Get<string>("Content");
            this.KeywordVersion = readerWrap.Get<string>("KeywordVersion");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.LastCommentDate = readerWrap.Get<DateTime>("LastCommentDate");
            this.CommentList = new CommentCollection();
        }

        /// <summary>
        /// DoingID
        /// </summary>
        public int DoingID { get; set; }

        /// <summary>
        /// DoingID
        /// </summary>
        [Obsolete]
        public int ID { get { return DoingID; } }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int TotalComments { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string CreateIP { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
		public string KeywordVersion { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后评论时间
        /// </summary>
        public DateTime LastCommentDate { get; set; }

        /// <summary>
        /// 这条记录的评论
        /// </summary>
        public CommentCollection CommentList { get; set; }

		#region For Template Only

		public bool CanDelete
		{
			get { return DoingBO.Instance.CheckDoingDeletePermission(UserBO.Instance.GetCurrentUserID(), this); }
		}

		public string FriendlyCreateDate 
		{
			get { return DateTimeUtil.GetFriendlyDate(this.CreateDate); }
		}

		public SimpleUser User
		{
			get
			{
				return UserBO.Instance.GetFilledSimpleUser(this.UserID);
			}
		}

		#endregion

        #region IPrimaryKey<int> Members

        public int GetKey()
        {
            return DoingID;
        }

        #endregion


		#region ITextRevertable 成员

		public string OriginalContent { get; private set; }

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

		#region IFillSimpleUser 成员

		public int GetUserIDForFill()
		{
			return this.UserID;
		}

		#endregion
	}

    public class DoingCollection : EntityCollectionBase<int, Doing>
    {
        public DoingCollection()
        { }

        public DoingCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Doing doing = new Doing(readerWrap);
                this.Add(doing);
            }
        }

        public IEnumerable<int> GetUserIds()
        {
            List<int> userIds = new List<int>();

            foreach (Doing doing in this)
            {
                if (userIds.Contains(doing.UserID) == false)
                    userIds.Add(doing.UserID);
            }

            return userIds;
        }


    }
}