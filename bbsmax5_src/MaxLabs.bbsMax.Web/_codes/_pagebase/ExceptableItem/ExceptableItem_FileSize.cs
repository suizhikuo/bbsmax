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
    public class ExceptableItem_FileSize : ExceptableItemBase<long>
    {

        protected virtual string CheckValue(long fileSize)
        {
            string error = null;
            if (fileSize < 0)
            {
                error = "大小不能小于0";
            }

            return error;
        }

        protected override long GetItemValue(string name, int id, bool isNew, MessageDisplay msgDisplay, out bool hasError)
        {
            hasError = false;

            string valueName, filetypeName;

            if (isNew)
            {
                valueName = "new_" + name + "_value";
                filetypeName = "new_" + name + "_filesize";
            }
            else
            {
                valueName = name + "_value_" + id;
                filetypeName = name + "_filesize_" + id;
            }

            string valueString = _Request.Get(valueName, Method.Post, string.Empty);

            long tempValue;
            if (long.TryParse(valueString, out tempValue) == false)
            {
                hasError = true;
                if (isNew)
                    msgDisplay.AddError("new_" + name, "大小必须为整数");
                else
                    msgDisplay.AddError(name, id, "大小必须为整数");
                return 0;
            }

            //long tempValue = long.Parse(value.ToString());

            string errors = CheckValue(tempValue);
            if (errors != null)
            {
                hasError = true;
                if (isNew)
                    msgDisplay.AddError("new_" + name, errors);
                else
                    msgDisplay.AddError(name, id, errors);
                return 0;
            }

            FileSizeUnit fileSizeUnit = _Request.Get<FileSizeUnit>(filetypeName, Method.Post, FileSizeUnit.K);

            return ConvertUtil.GetFileSize(tempValue, fileSizeUnit);
        }
    }
}