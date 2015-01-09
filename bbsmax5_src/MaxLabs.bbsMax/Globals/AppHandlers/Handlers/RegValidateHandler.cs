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
using MaxLabs.bbsMax.AppHandlers;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class RegValidateHandler:IAppHandler
    {

        public IAppHandler CreateInstance()
        {
            return  new RegValidateHandler();
        }

        public string Name
        {
            get { return "RegValidate"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            string field = context.Request.QueryString["field"];
            string value = context.Request.QueryString["value"];
            if (string.IsNullOrEmpty( field ))
            {
                context.Response.End(); 
                return;
            }

            field = field.ToLower();

            string msg = string.Empty;
            int state = 0;
            using (ErrorScope es = new ErrorScope())
            {

                switch (field)
                {
                    case "username":
                        {
                            UserBO.Instance.ValidateUsername(value, string.Empty);
                            break;
                        }
                    case "email":
                        {
                            UserBO.Instance.ValidateEmail(value, string.Empty);
                            break;
                        }
                    //case "invatecode":
                    //{
                    //InviteBO.Instance.ValidateInvideCode(value,
                    //break;
                    //}
                    default:
                        {
                            context.Response.End();
                            return;
                        }
                }


                if (es.HasUnCatchedError)
                {
                    state = 1;
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msg += error.Message + "<br />";
                    });
                }
            }

            context.Response.Write(string.Format("{{state:{0},message:\"{1}\"}}", state,StringUtil.ToJavaScriptString( msg)));
            context.Response.End();
        }
    }
}