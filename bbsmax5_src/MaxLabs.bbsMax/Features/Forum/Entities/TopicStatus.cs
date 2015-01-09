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
    /// 主题状态
    /// </summary>
    public class TopicStatus : IPrimaryKey<int>
    {
        public TopicStatus()
        {
        }

        public TopicStatus(DataReaderWrap readerWrap)
        {

            this.ID = readerWrap.Get<int>("ID");
            this.ThreadID = readerWrap.Get<int>("ThreadID");
            this.Type = (TopicStatuType)readerWrap.Get<byte>("Type");
            this.EndDate = readerWrap.Get<DateTime>("EndDate");
        }

        public int ID { get; set; }

        public int ThreadID { get; set; }

        public TopicStatuType Type { get; set; }

        public DateTime EndDate { get; set; }



        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ID;
        }

        #endregion
    }

    public class TopicStatusCollection : EntityCollectionBase<int, TopicStatus>
    {
        public TopicStatusCollection()
        { }

        public TopicStatusCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                TopicStatus topicStatus = new TopicStatus(readerWrap);

                this.Add(topicStatus);
            }
        }
    }
}