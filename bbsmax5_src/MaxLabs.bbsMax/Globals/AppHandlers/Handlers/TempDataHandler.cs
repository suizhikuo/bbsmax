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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using System.Threading;

namespace MaxLabs.bbsMax.AppHandlers
{
    /// <summary>
    /// 临时数据处理器
    /// </summary>
    public class TempDataHandler : IAppHandler
    {
 
        public IAppHandler CreateInstance()
        {
           return new TempDataHandler();
        }

        public string Name
        {
            get { return "tempdata"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            UserTempDataType type = StringUtil.TryParse<UserTempDataType>( context.Request.QueryString["type"], UserTempDataType.None);
            string action = context.Request.QueryString["action"];
            User user = User.Current;
            context.Response.CacheControl = "no-cache";
            if (user != null)
            {
                if (type != UserTempDataType.None
                    && type!= UserTempDataType.Avatar
                )
                {
                    if (action == "get")
                    {
                        UserTempData data = UserTempDataBO.Instance.GetTempData(user.UserID, type);

                        if (data != null)
                        {
                            string value = data.Data.ToString();
                            context.Response.Write(value);
                            context.Response.End();
                        }
                    }
                    else if (action == "save")
                    {
                        string format = context.Request.Form["format"];
                        string value = context.Request.Form["data"] + "";

                        if (!string.IsNullOrEmpty(format))
                            value = format + "|" + value;

                        UserTempDataBO.Instance.SaveData(user.UserID, type, value, true);
                    }
                    else if (action == "delete")
                    {
                        UserTempDataBO.Instance.Delete(user.UserID, type);
                    }
                }
            }
        }
    }
}