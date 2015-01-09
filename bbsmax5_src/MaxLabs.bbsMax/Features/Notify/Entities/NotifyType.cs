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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class NotifyType
    {
        public NotifyType() { }

        public NotifyType(DataReaderWrap wrap)
        {
            this.TypeID = wrap.Get<int>("TypeID");
            this.TypeName = wrap.Get<string>("TypeName");
            this.Keep = wrap.Get<bool>("Keep");
        }

        public int TypeID { get; set; }

        public string TypeName { get; set; }

        /// <summary>
        /// 改类型的通知是否忽略以后仍然保存在数据库
        /// </summary>
        public bool Keep { get; set; }
    }

    public class NotifyTypeCollection : Collection<NotifyType>
    {
        public NotifyTypeCollection() { }

        public NotifyTypeCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
                this.Add(new NotifyType(wrap));
        }

        public NotifyType this[int typeID]
        {
            get
            {
                foreach (NotifyType nt in this)
                    if (nt.TypeID == typeID)
                        return nt;

                return null;
            }
        }

        public NotifyType this[string typeName]
        {
            get
            {
                foreach (NotifyType nt in this)
                    if (nt.TypeName == typeName)
                        return nt;

                return null;
            }
        }
    }
}