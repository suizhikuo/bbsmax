//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;


using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Settings
{
    public class FriendSettings : SettingBase
    {
        public FriendSettings()
        {
            MaxFriendCount = 300;
            MaxFriendGroupCount = 5;
            //FriendGroupNames = new LineCollection(true, 10);
        }

        /// <summary>
        /// 最多好友数量
        /// </summary>
        [SettingItem]
        public int MaxFriendCount { get; set; }

        /// <summary>
        /// 最多好友分组数
        /// </summary>
        [SettingItem]
        public int MaxFriendGroupCount { get; set; }

        ///// <summary>
        ///// 默认好友分组
        ///// </summary>
        //[SettingItem]
        //public LineCollection FriendGroupNames { get; set; }

        //public override void SetPropertyValue(PropertyInfo property, string value, bool isParse)
        //{
        //    if (property.Name == "FriendGroupNames")
        //    {
        //        Type type = property.PropertyType.GetInterface("ISettingItem");

        //        if (type != null)
        //        {
        //            MethodInfo setValueMethod = type.GetMethod("SetValue");
        //            setValueMethod.Invoke(property.GetValue(this, null), new object[] { value });
        //        }
        //        if (FriendGroupNames.Count > MaxFriendGroupCount)
        //        {
        //            Context.ThrowError(new FriendGroupNumberError(MaxFriendGroupCount));
        //        }
        //    }

        //    base.SetPropertyValue(property, value, isParse);
        //}
    }
}