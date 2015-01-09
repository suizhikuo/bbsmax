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

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 日志访问记录表
    /// </summary>
    public class BlogArticleVisitor : IFillSimpleUser
    {
        #region Properties

        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 日志ID
        /// </summary>
        public int BlogArticleID
        {
            get;
            set;
        }

        /// <summary>
        /// 访问者ID
        /// </summary>
        public int UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime ViewDate
        {
            get;
            set;
        }

        #endregion

		public SimpleUser User
		{
			get { return UserBO.Instance.GetFilledSimpleUser(this.UserID); }
		}

        #region Constructors

        public BlogArticleVisitor() { }


        public BlogArticleVisitor(DataReaderWrap readerWrap)
        {
            this.ID = readerWrap.Get<int>("ID");
            this.BlogArticleID = readerWrap.Get<int>("BlogArticleID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.ViewDate = readerWrap.Get<DateTime>("ViewDate");
        }

        #endregion

		#region IFillSimpleUser 成员

		public int GetUserIDForFill()
		{
			return this.UserID;
		}

		#endregion
	}


    public class BlogArticleVisitorCollection : Collection<BlogArticleVisitor>
    {
        #region Constructors

        public BlogArticleVisitorCollection() { }

        public BlogArticleVisitorCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new BlogArticleVisitor(readerWrap));
            }
        }

        #endregion
    }
}