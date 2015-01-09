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

namespace MaxLabs.bbsMax.Filters
{
    public class EmoticonFilter : FilterBase<EmoticonFilter>
    {
        public EmoticonFilter() { IsDesc = true; }

        [FilterItem(FormName = "Username")]
        public string UserName { get; set; }

        [FilterItem(FormName = "pagesize")]
        public int Pagesize
        {
            get;
            set;
        }

        [FilterItem(FormName = "orderBy")]
        public OrderBy? Order { get; set; }

        [FilterItem(FormName = "IsDesc")]
        public bool IsDesc
        {
            get;
            set;
        }
        
        /// <summary>
        /// 用户列表排序字段
        /// </summary>
        public enum OrderBy : byte
        {
            SpaceSize,
            EmoticonCount
        }
    }
}