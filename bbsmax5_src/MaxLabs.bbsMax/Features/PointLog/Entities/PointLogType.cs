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
    public class PointLogType:IPrimaryKey<int>
    {
        public PointLogType() { }

        public PointLogType(DataReaderWrap wrap)
        {
            this.OperateID = wrap.Get<int>("OperateID");
            this.OperateName = wrap.Get<string>("OperateName");
        }

        public int OperateID { get; set; }

        public string OperateName { get; set; }

        #region IPrimaryKey<int> Members

        public int GetKey()
        {
            return this.OperateID;
        }

        #endregion
    }

    public class PointLogTypeCollection : EntityCollectionBase<int, PointLogType>
    {
        public PointLogTypeCollection(){}

        public PointLogTypeCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                Add(new PointLogType(wrap));
            }
        }
    }
}