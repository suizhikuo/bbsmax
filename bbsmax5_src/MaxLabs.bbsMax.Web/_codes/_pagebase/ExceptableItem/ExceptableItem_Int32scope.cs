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
    public class ExceptableItem_Int32scope : ExceptableItemBase<Int32Scope>
    {

        protected virtual string CheckValue(int minValue, int maxValue)
        {
            string errors = null;
            if (minValue < 0 || maxValue < 0)
            {
                errors = "最小值或者最大值必须大于0";
            }
            else if (minValue > maxValue)
            {
                errors = "最小值不能大于最大值";
            }

            return errors;
        }

        protected override Int32Scope GetItemValue(string name, int id, bool isNew, MessageDisplay msgDisplay, out bool hasError)
        {
            hasError = false;

            string minValueName, maxValueName;
            if (isNew)
            {
                minValueName = "new_" + name + "_minvalue";
                maxValueName = "new_" + name + "_maxvalue";
            }
            else
            {
                minValueName = name + "_minvalue_" + id;
                maxValueName = name + "_maxvalue_" + id;
            }

            string minValueString = _Request.Get(minValueName, Method.Post, string.Empty);
            string maxvalueString = _Request.Get(maxValueName, Method.Post, string.Empty);

            int minvalue, maxvalue;

            string errors = null;
            if (int.TryParse(minValueString, out minvalue) == false)
            {
                errors = "最小值";
            }

            if (int.TryParse(maxvalueString, out maxvalue) == false)
            {
                if (errors != null)
                    errors += ",最大值";
                else
                    errors = "最大值";

            }
            if (errors != null)
            {
                errors = errors + "必须为整数";
            }
            else
            {
                errors = CheckValue(minvalue, maxvalue);
            }
            if (errors != null)
            {
                hasError = true;
                if (isNew)
                    msgDisplay.AddError("new_" + name, errors);
                else
                    msgDisplay.AddError(name, id, errors);
                return null;
            }

            return new Int32Scope(minvalue, maxvalue);
        }
    }
}