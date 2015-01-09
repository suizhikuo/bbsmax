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
    public class ThreadCatalog : ICloneable<ThreadCatalog>, IPrimaryKey<int>
    {
        public ThreadCatalog()
        { }

        public ThreadCatalog(DataReaderWrap readerWrap)
        {
            this.ThreadCatalogID = readerWrap.Get<int>("ThreadCatalogID");
            this.ThreadCatalogName = readerWrap.Get<string>("ThreadCatalogName");
            this.LogoUrl = readerWrap.Get<string>("LogoUrl");
        }
        public int ThreadCatalogID { get; set; }

        public string ThreadCatalogName { get; set; }

        public string LogoUrl { get; set; }
        public int ThreadCount { get; set; }

        #region ICloneable<ThreadCatalogV5> 成员

        public ThreadCatalog Clone()
        {
            ThreadCatalog threadCatalog = new ThreadCatalog();
            threadCatalog.ThreadCatalogID = ThreadCatalogID;
            threadCatalog.ThreadCatalogName = ThreadCatalogName;
            threadCatalog.ThreadCount = ThreadCount;
            threadCatalog.LogoUrl = LogoUrl;
            return threadCatalog;
        }

        #endregion


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ThreadCatalogID;
        }

        #endregion
    }

    public class ThreadCatalogCollection : HashedCollectionBase<int, ThreadCatalog>
    {
        public ThreadCatalogCollection()
        { }

        //public ThreadCatalogCollection(ThreadCatalogCollection catalogs)
        //{ 
        //    for
        //}

        public ThreadCatalogCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ThreadCatalog threadCatalog = new ThreadCatalog(readerWrap);

                this.Add(threadCatalog);
            }
        }
    }
}