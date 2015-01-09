//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.ExceptableSetting
{
    public class ExceptableItem_bool : ExceptableItemBase<bool>
    {
        protected override bool GetItemValue(string name, int id, bool isNew, MessageDisplay msgDisplay, out bool hasError)
        {
            hasError = false;
            string valueName;

            if (isNew)
            {
                valueName = "new_" + name + "_value";
            }
            else
            {
                valueName = name + "_value_" + id;
            }

            bool value = _Request.Get<bool>(valueName, Method.Post, false);


            return value;
        }
    }
}