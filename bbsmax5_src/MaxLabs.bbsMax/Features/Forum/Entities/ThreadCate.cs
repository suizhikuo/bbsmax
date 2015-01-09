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
    /// <summary>
    /// 分类信息
    /// </summary>
    public class ThreadCate : IPrimaryKey<int>
    {
        public ThreadCate()
        { }

        public ThreadCate(DataReaderWrap readerWrap)
        {
            CateID = readerWrap.Get<int>("CateID");
            CateName = readerWrap.Get<string>("CateName");
            Enable = readerWrap.Get<bool>("Enable");
            SortOrder = readerWrap.Get<int>("SortOrder");
        }
        public int CateID { get; set; }

        public string CateName { get; set; }

        public bool Enable { get; set; }
        public int SortOrder { get; set; }




        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return CateID;
        }

        #endregion
    }

    public class ThreadCateCollection : HashedCollectionBase<int, ThreadCate>
    {
        public ThreadCateCollection()
        { }

        public ThreadCateCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ThreadCate cate = new ThreadCate(readerWrap);

                this.Add(cate);
            }
        }
    }
}