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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class ForumThreadCatalog : IBatchSave, IPrimaryKey<string>
    {
        public ForumThreadCatalog()
        { 
        }

        public ForumThreadCatalog(DataReaderWrap readerWrap)
        {
            this.ForumID = readerWrap.Get<int>("ForumID");
            this.ThreadCatalogID = readerWrap.Get<int>("ThreadCatalogID");
            this.TotalThreads = readerWrap.Get<int>("TotalThreads");
            this.SortOrder = readerWrap.Get<int>("SortOrder");
        }

        public int ForumID { get; set; }

        public int ThreadCatalogID { get; set; }

        public int TotalThreads { get; set; }

        public int SortOrder { get; set; }

        private ThreadCatalog threadCatalog;
        public ThreadCatalog ThreadCatalog
        {
            get
            {
                if (threadCatalog == null)
                {
                    threadCatalog = ForumBO.Instance.GetThreadCatalog(ThreadCatalogID);
                }
                return threadCatalog;
            }
            set { threadCatalog = value; }
        }

        #region IBatchSave 成员

        public bool IsNew
        {
            get;
            set;
        }

        #endregion

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return ForumID + "-" + ThreadCatalogID;
        }

        #endregion
    }

    public class ForumThreadCatalogCollection : HashedCollectionBase<string, ForumThreadCatalog>
    {
        public ForumThreadCatalog GetValue(int forumID, int threadCatalogID)
        {
            return this.GetValue(forumID + "-" + threadCatalogID);
        }

        public ForumThreadCatalogCollection()
        { }

        public ForumThreadCatalogCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ForumThreadCatalog forumThreadCatalog = new ForumThreadCatalog(readerWrap);

                this.Add(forumThreadCatalog);
            }
        }
    }
}