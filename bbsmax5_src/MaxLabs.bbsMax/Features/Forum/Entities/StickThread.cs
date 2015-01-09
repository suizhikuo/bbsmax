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

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// ForumV5 的摘要说明。
    /// </summary>
    public class StickThread : IPrimaryKey<string>
    {
        public StickThread()
        {
        }

        public StickThread(DataReaderWrap readerWrap)
        {
            this.ID = readerWrap.Get<int>("ID");
            this.ThreadID = readerWrap.Get<int>("ThreadID");
            this.ForumID = readerWrap.Get<int>("ForumID");
        }

        public int ID { get; set; }

        public int ThreadID { get; set; }

        public int ForumID { get; set; }



        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return string.Concat(ThreadID, "-", ForumID);
        }

        #endregion
    }

    public class StickThreadCollection : EntityCollectionBase<string, StickThread>
    {
        public StickThreadCollection()
        { }

        public StickThreadCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                StickThread stickThread = new StickThread(readerWrap);

                this.Add(stickThread);
            }
        }

        public StickThread GetValue(int threadID, int forumID)
        {
            return this.GetValue(threadID + "-" + forumID);
        }
    }
}