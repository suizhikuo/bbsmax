//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class UserIPLog : IPrimaryKey<int>,IFillSimpleUser
    {
        public int LogID { get; set; }
        public int UserID { set; get; }
        public string UserName { set; get; }
        public string NewIP { set; get; }
        public DateTime CreateDate { set; get; }
        public string OldIP { set; get; }
        public string VisitUrl { get; set; }
        public int? BannedForumID { get; set; }
        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(this.UserID);
            }
        }

        public UserIPLog() { }

        public UserIPLog(int userid, string username, string newip,string oldip,string visiturl)
        {
            this.UserID = userid;
            this.UserName = username;
            this.NewIP = newip;
            this.CreateDate = DateTime.Now;
            this.OldIP = oldip;
            this.VisitUrl = visiturl;
        }

        public UserIPLog(DataReaderWrap wrap)
        {
            this.LogID = wrap.Get<int>("LogID");
            this.UserID = wrap.Get<int>("UserID");
            this.UserName = wrap.Get<string>("Username");
            this.NewIP = wrap.Get<string>("NewIP");
            this.CreateDate = wrap.Get<DateTime>("CreateDate");
            this.OldIP = wrap.Get<string>("OldIP");
            this.VisitUrl = wrap.Get<string>("VisitUrl");
            if (wrap.ContainsField("ForumID"))
            {
                BannedForumID = wrap.Get<int?>("ForumID", null);
            }
            else
            {
                BannedForumID = null;
            }

        }




        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return LogID;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion
    }

    public class UserIPLogCollection : EntityCollectionBase<int, UserIPLog>
    {
        public UserIPLogCollection() { }

        public UserIPLogCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                this.Add(new UserIPLog(wrap));
            }

        }

    }

}