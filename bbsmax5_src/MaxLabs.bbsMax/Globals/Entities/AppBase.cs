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

namespace MaxLabs.bbsMax.Entities
{
    public abstract class AppBase : IPrimaryKey<Guid>
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public abstract string AppName { get; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public abstract Guid AppID { get; }


        /// <summary>
        /// 应用的动作集合
        /// </summary>
        public abstract AppActionCollection AppActions { get; }

        #region IEntityWithKey<Guid> 成员

        public Guid GetKey()
        {
            return AppID;
        }

        #endregion
    }

    public class AppBaseCollection : EntityCollectionBase<Guid, AppBase> //ICollection<Feed> //Dictionary<int, Feed>//Collection<Feed>
    {
        public AppBaseCollection()
        { 
        }
    }
}