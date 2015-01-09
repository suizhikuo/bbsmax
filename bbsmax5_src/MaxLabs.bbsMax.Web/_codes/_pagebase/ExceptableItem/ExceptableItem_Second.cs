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
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">T 只能为 能转换为 long 的类型 </typeparam>
    public class ExceptableItem_Second<T> : ExceptableItemBase<T> where T : struct
    {

        protected virtual string CheckValue(long seconds)
        {
            string error = null;
            if (seconds < 0)
            {
                error = "时间不能小于0";
            }

            return error;
        }

        protected override T GetItemValue(string name, int id, bool isNew, MessageDisplay msgDisplay, out bool hasError)
        {
            hasError = false;

            string valueName, timeTypeName;

            if (isNew)
            {
                valueName = "new_" + name + "_value";
                timeTypeName = "new_" + name + "_timetype";
            }
            else
            {
                valueName = name + "_value_" + id;
                timeTypeName = name + "_timetype_" + id;
            }

            string valueString = _Request.Get(valueName, Method.Post, string.Empty);

            T value;
            if (StringUtil.TryParse<T>(valueString, out value) == false)
            {
                hasError = true;
                if (isNew)
                    msgDisplay.AddError("new_" + name, "时间必须为整数");
                else
                    msgDisplay.AddError(name, id, "时间必须为整数");
                return default(T);
            }

            long tempValue = long.Parse(value.ToString());

            string errors = CheckValue(tempValue);
            if (errors != null)
            {
                hasError = true;
                if (isNew)
                    msgDisplay.AddError("new_" + name, errors);
                else
                    msgDisplay.AddError(name, id, errors);
                return default(T);
            }

            TimeUnit timeUnit = _Request.Get<TimeUnit>(timeTypeName, Method.Post, TimeUnit.Second);

            StringUtil.TryParse<T>(DateTimeUtil.GetSeconds(tempValue, timeUnit).ToString(), out value);

            return value;
        }
    }
}