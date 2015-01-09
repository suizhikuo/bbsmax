//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{

    public class Vars : ICloneable
    {
        public Vars()
        {
        }

        public Vars(DataReaderWrap readerWrap)
        {
            MaxPosts = readerWrap.Get<int>("MaxPosts");
            NewUserID = readerWrap.Get<int>("NewUserID");
            TotalUsers = readerWrap.Get<int>("TotalUsers");
            YestodayPosts = readerWrap.Get<int>("YestodayPosts");
            YestodayTopics = readerWrap.Get<int>("YestodayTopics");
            MaxOnlineCount = readerWrap.Get<int>("MaxOnlineCount");

            MaxPostDate = readerWrap.Get<DateTime>("MaxPostDate");
            MaxOnlineDate = readerWrap.Get<DateTime>("MaxOnlineDate");
            LastResetDate = readerWrap.Get<DateTime>("LastResetDate");

            NewUsername = readerWrap.Get<string>("NewUsername");
        }

        public int MaxPosts { get; set; }

        public int NewUserID { get; set; }

        public int TotalUsers { get; set; }

        public int YestodayPosts { get; set; }

        public int YestodayTopics { get; set; }

        public int MaxOnlineCount { get; set; }


        public DateTime MaxPostDate { get; set; }

        public DateTime MaxOnlineDate { get; set; }

        public DateTime LastResetDate { get; set; }

        public string NewUsername { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            Vars stat = new Vars();
            stat.MaxOnlineCount = MaxOnlineCount;
            stat.MaxOnlineDate = MaxOnlineDate;
            stat.MaxPostDate = MaxPostDate;
            stat.MaxPosts = MaxPosts;
            stat.NewUserID = NewUserID;
            stat.NewUsername = NewUsername;
            stat.TotalUsers = TotalUsers;
            stat.YestodayPosts = YestodayPosts;
            stat.LastResetDate = LastResetDate;
            stat.YestodayTopics = YestodayTopics;

            return stat;
        }

        #endregion
    }

}