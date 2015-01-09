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
using System.Collections.Specialized;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class AjaxHandler:IAppHandler
    {
        #region IAppHandler 成员

        public IAppHandler CreateInstance()
        {
            return new AjaxHandler();
        }

        public string Name
        {
            get { return "ajax"; }
        }

        private int UserID {
            get
            {
                return User.CurrentID;
            }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            string action = context.Request["action"] + string.Empty;
            StringDictionary parameters = new StringDictionary();
            
            foreach( string s in context.Request.QueryString.Keys)
            {
                if (s.Equals("action", StringComparison.OrdinalIgnoreCase)) continue;
                parameters.Add(s, context.Request.QueryString[s]);
            }

            context.Response.Clear();
            context.Response.Write(this.Proccess(action, parameters));
            context.Response.End();
        }

        private string Proccess(string action,StringDictionary parameters)
        {
            switch (action.ToLower())
            {
                case "ignorenotify":
                    int[]  notifyIDs = StringUtil.Split<int>(parameters["notifyids"]);
                    NotifyBO.Instance.IgnoreNotifies(User.Current, notifyIDs);
                    break;

            }
            return string.Empty;
        }

        #endregion
    }
}