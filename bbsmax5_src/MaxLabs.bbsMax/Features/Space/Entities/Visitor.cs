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

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 好友访问
    /// </summary>
    public class Visitor : IFillSimpleUsers
    {
        public Visitor()
        { }

        public Visitor(DataReaderWrap readerWrap)
        {
            this.UserID = readerWrap.Get<int>("UserID");
            this.VisitorUserID = readerWrap.Get<int>("VisitorUserID");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 好友ID
        /// </summary>
        public int VisitorUserID { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        #region 需要填充的属性

		public SimpleUser User
		{
			get
			{
				return UserBO.Instance.GetFilledSimpleUser(this.VisitorUserID);
			}
		}

		public SimpleUser TargetUser
		{
			get
			{
				return UserBO.Instance.GetFilledSimpleUser(this.UserID);
			}
		}

        //public SimpleUser Visitor { get; set; }

        //public string VisitorUsername { get; set; }

        //public string VisitorRealname { get; set; }

        //public string VisitorAvatar { get; set; }

        //public Gender VisitorGender { get; set; }

        //public string VisitorDoing { get; set; }

        //public int VisitorPoint { get; set; }

        #endregion

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            return new int[] { UserID, VisitorUserID };
        }

        #endregion
	}

    public class VisitorCollection : Collection<Visitor>
    {
        public VisitorCollection()
        { }

        public VisitorCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Visitor visitor = new Visitor(readerWrap);
                this.Add(visitor);
            }
        }

		public int TotalRecords { get; set; }
    }
}