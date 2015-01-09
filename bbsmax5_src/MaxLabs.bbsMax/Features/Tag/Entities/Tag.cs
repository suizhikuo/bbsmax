//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class Tag
    {

        #region Properties

        public int ID { get; set; }

        /// <summary>
        /// 是否被锁定,被锁定的标签将不再能使用
        /// </summary>
        public bool IsLock { get; set; }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 使用该标签的对象个数,如日志篇数
        /// </summary>
        public int TotalElements { get; set; }

        #endregion

        #region Constructors

        public Tag() { }


        public Tag(DataReaderWrap readerWrap)
        {
            this.ID = readerWrap.Get<int>("ID");
            this.IsLock = readerWrap.Get<bool>("IsLock");
            this.Name = readerWrap.Get<string>("Name");
            this.TotalElements = readerWrap.Get<int>("TotalElements");
        }

        #endregion

    }


    public class TagCollection : Collection<Tag>
    {
        public TagCollection() { }

        public TagCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Tag tag = new Tag(readerWrap);
                this.Add(tag);
            }
        }
    }


    /// <summary>
    /// 标签类型
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// 日志标签
        /// </summary>
        Blog = 1
    }
}