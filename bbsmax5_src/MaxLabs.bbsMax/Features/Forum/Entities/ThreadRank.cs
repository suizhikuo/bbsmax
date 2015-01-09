//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;
using System.Text;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 主题评级信息
    /// </summary>
    public class ThreadRank : IFillSimpleUser
    {
        public ThreadRank()
        {
        }

        public ThreadRank(DataReaderWrap readerWrap)
        {
            this.ThreadID = readerWrap.Get<int>("ThreadID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.Rank = readerWrap.Get<byte>("Rank");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
        }

        public int ThreadID { get; set; }
        
        public int UserID { get; set; }
        
        public int Rank { get; set; }
        
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public SimpleUser RankUser { get { return UserBO.Instance.GetSimpleUser(UserID, GetUserOption.WithAll); } }


        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion
    }

    public class ThreadRankCollection : Collection<ThreadRank>
    {
        public ThreadRankCollection()
        { }

        public ThreadRankCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ThreadRank rank = new ThreadRank(readerWrap);

                this.Add(rank);
            }
        }
    }
}