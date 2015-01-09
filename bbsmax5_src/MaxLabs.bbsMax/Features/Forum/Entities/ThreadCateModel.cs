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
    public class ThreadCateModel : IPrimaryKey<int>
    {
        public ThreadCateModel()
        { }

        public ThreadCateModel(DataReaderWrap readerWrap)
        {
            ModelID = readerWrap.Get<int>("ModelID");
            CateID = readerWrap.Get<int>("CateID");
            ModelName = readerWrap.Get<string>("ModelName");
            Enable = readerWrap.Get<bool>("Enable");
            SortOrder = readerWrap.Get<int>("SortOrder");
        }

        public int ModelID { get; set; }

        public int CateID { get; set; }

        public string ModelName { get; set; }

        public bool Enable { get; set; }
        public int SortOrder { get; set; }




        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ModelID;
        }

        #endregion
    }

    public class ThreadCateModelCollection : HashedCollectionBase<int, ThreadCateModel>
    {
        public ThreadCateModelCollection()
        { }

        public ThreadCateModelCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                ThreadCateModel model = new ThreadCateModel(readerWrap);

                this.Add(model);
            }
        }
    }
}