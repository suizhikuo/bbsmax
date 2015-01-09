//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MaxLabs.bbsMax
{
    public class NameObjectCollection : NameObjectCollectionBase
    {
        public NameObjectCollection()
        {
        }

        public NameObjectCollection(int capacity)
            : base(capacity)
        {
        }

        public NameObjectCollection(params object[] nameAndValues)
            : base(nameAndValues == null ? 0 : nameAndValues.Length / 2)
        {
            if (nameAndValues != null)
            {
                for (int i = 0; i < nameAndValues.Length; i += 2)
                {
                    Add((string)nameAndValues[i], nameAndValues[i + 1]);
                }
            }
        }

        public object this[string key]
        {
            get { return this.BaseGet(key); }
            set { this.BaseSet(key, value); }
        }

        public void Add(string key, object value)
        {
            this.BaseSet(key, value);
        }
    }
}