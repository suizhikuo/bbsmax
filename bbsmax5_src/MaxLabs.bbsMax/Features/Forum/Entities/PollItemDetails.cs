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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Ubb;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class PollItemDetailsV5
    {
        public PollItemDetailsV5()
        { }


        public PollItemDetailsV5(DataReaderWrap readerWrap)
        {
            UserID = readerWrap.Get<int>("UserID");
            ItemID = readerWrap.Get<int>("ItemID");
            Nickname = readerWrap.Get<string>("Nickname");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }


         public int ItemID { get; set; }

         public int UserID { get; set; }

         public string Nickname { get; set; }

         public DateTime CreateDate { get; set; }

    }

    public class PollItemDetailsCollectionV5 : Collection<PollItemDetailsV5>
    {
        public PollItemDetailsCollectionV5()
        { }

        public PollItemDetailsCollectionV5(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                PollItemDetailsV5 pollItemDetails = new PollItemDetailsV5(readerWrap);

                this.Add(pollItemDetails);
            }
        }

    }
}