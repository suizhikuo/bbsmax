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
using System.IO;
using System.Web;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.AppHandlers
{
	public class DoingHandler : IAppHandler
	{
		public IAppHandler CreateInstance()
		{
			return new DoingHandler();
		}

		public string Name
		{
			get { return "Doing"; }
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{ 
            bool success;
			if (context.Request.HttpMethod == "POST")
			{
                string from = context.Request.QueryString["from"];
				string content = context.Request.Form["content"];
                content = HttpUtility.HtmlEncode(content);

				if (content == null)
					return;

                string message = "";

                using (ErrorScope es = new ErrorScope())
                {
                    try
                    {
                        success = DoingBO.Instance.UpdateDoing(User.Current, IPUtil.GetCurrentIP(), content);

                        if (success == false)
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                message = error.Message;
                            });
                        }
                        else
                            message = User.Current.Doing;
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                        success = false;
                    }
                }

                message = StringUtil.ToJavaScriptString(message);
                string js = @"
<Script language='JavaScript'>
      parent.parent.SubmitBack(" + (success ? "true" : "false") + ",'" + message + @"');
</Script>";
                HttpContext.Current.Response.Write(js);
			}
		}
	}
}