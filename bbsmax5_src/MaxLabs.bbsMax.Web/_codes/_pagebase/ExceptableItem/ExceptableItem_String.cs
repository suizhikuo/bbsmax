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
    public abstract class ExceptableItem_String<T> : ExceptableItemBase<T>
    {

        protected abstract string CheckValue(string value);

        protected abstract T GetValue(string value);

        protected override T GetItemValue(string name, int id, bool isNew, MessageDisplay msgDisplay, out bool hasError)
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

            string value = _Request.Get(valueName, Method.Post, string.Empty).Trim();

            string error = CheckValue(value);
            if (error != null)
            {
                hasError = true;
                if (isNew)
                    msgDisplay.AddError("new_" + name, error);
                else
                    msgDisplay.AddError(name, id, error);
                return default(T);
            }

            return GetValue(value);

        }
    }


    public class ExceptableItem_ExtensionList : ExceptableItem_String<ExtensionList>
    {
        protected override string CheckValue(string value)
        {
            string errorMessage = null;
            if (value == string.Empty)
            {
                errorMessage = "允许上传的文件扩展名不能为空";
            }
            return errorMessage;
        }

        protected override ExtensionList GetValue(string value)
        {
            return ExtensionList.Parse(value);
        }
    }

    public class ExceptableItem_Int : ExceptableItem_String<int>
    {
        protected override string CheckValue(string value)
        {
            string errorMessage = null;
            int v;
            if (false == int.TryParse(value, out v))
            {
                errorMessage = "请填写整数";
            }
            return errorMessage;
        }

        protected override int GetValue(string value)
        {
            return int.Parse(value);
        }
    }

    /// <summary>
    /// 大于等于0的 整数
    /// </summary>
    public class ExceptableItem_Int_MoreThenZero : ExceptableItem_Int
    {
        protected override string CheckValue(string value)
        {
            string message = base.CheckValue(value);
            if (message == null)
            {
                if (int.Parse(value) < 0)
                {
                    message = "请填写大于0的整数";
                }
            }
            return message;
        }
    }
}