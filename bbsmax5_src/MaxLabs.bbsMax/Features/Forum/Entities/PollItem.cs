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

namespace MaxLabs.bbsMax.Entities
{
    public class PollItem : IPrimaryKey<int>
    {
        public PollItem()
        { }


        public PollItem(DataReaderWrap readerWrap)
        {
            ThreadID = readerWrap.Get<int>("ThreadID");
            ItemID = readerWrap.Get<int>("ItemID");
            m_ItemName = readerWrap.Get<string>("ItemName");
            PollItemCount = readerWrap.Get<int>("PollItemCount");
        }


        public PollItem(string extendDataString)
        {
            StringTable table = StringTable.Parse(extendDataString);
            this.ThreadID = int.Parse(table["threadID"]);
            this.ItemID = int.Parse(table["itemID"]);
            this.ItemName = table["itemName"];
            this.PollItemCount = int.Parse(table["pollItemCount"]);
        }

        /// <summary>
        /// 外键
        /// </summary>
         public int ThreadID { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
         public int ItemID { get; set; }

        private string FormatItemName(string itemName)
        {
            UbbParser ubbParser = new UbbParser();
            ubbParser.AddTagHandler(new IMG());
            return ubbParser.UbbToHtml(itemName);
        }

        private string m_ItemName;
        /// <summary>
        /// 名称或标题
        /// </summary>
        public string ItemName
        {
            get
            {
                return FormatItemName(m_ItemName);
            }
            set
            {
                m_ItemName = value;
            }
        }

        public string ItemNameForEdit
        {
            get
            {
                return m_ItemName;
            }
        }
        public int PollItemCount { get; set; }

        //private List<SimpleUser> votedUsers = new List<SimpleUser>();
        //public List<SimpleUser> VotedUsers
        //{
        //    get
        //    {
        //        return votedUsers;
        //    }
        //    set { votedUsers = value; }
        //}

        public override string ToString()
        {
            StringTable table = new StringTable();
            table.Add("threadID", ThreadID.ToString());
            table.Add("itemID", ItemID.ToString());
            table.Add("itemName", m_ItemName.ToString());
            table.Add("pollItemCount", PollItemCount.ToString());

            return table.ToString();
        }


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ItemID;
        }

        #endregion
    }

    public class PollItemCollectionV5 : HashedCollectionBase<int, PollItem>
    {
        public PollItemCollectionV5()
        { }

        public PollItemCollectionV5(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                PollItem pollItem = new PollItem(readerWrap);

                this.Add(pollItem);
            }
        }

        public PollItemCollectionV5(string value)
        {
            StringTable table = StringTable.Parse(value);

            PollItemCollectionV5 pollItems = new PollItemCollectionV5();

            for (int i = 0; i < table.Count; i++)
            {
                PollItem item = new PollItem(table["pollItemCollection_" + i]);
                this.Add(item);
            }
        }

        public override string ToString()
        {
            StringTable table = new StringTable();

            int index = 0;
            foreach (PollItem item in this)
            {
                table.Add("pollItemCollection_" + index, item.ToString());

                index++;
            }

            return table.ToString();
        }

    }
}