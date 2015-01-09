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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class ChatMessageHandler : IAppHandler
    {

        #region IAppHandler 成员

        public IAppHandler CreateInstance()
        {
            return new ChatMessageHandler();
        }

        public string Name
        {
            get { return "message"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            int maxId = 0, userId = 0, targetUserID = 0, messageCount = 0;
            string action = context.Request["issend"];
            string data = string.Empty;

            userId = UserBO.Instance.GetCurrentUserID();
            int.TryParse(context.Request["maxid"], out maxId);
            int.TryParse(context.Request["tuid"], out targetUserID);
            int.TryParse(context.Request["count"], out messageCount);

            context.Response.CacheControl = "no-cache";

            ChatMessageCollection messages;

            string validateActionName = "sendmessage";

            //messages = ChatBO.Instance.GetMessages(userId, targetUserID, maxId, messageCount);

            if (action != "true")
            {
                messages = ChatBO.Instance.GetLastChatMessages(userId, targetUserID, maxId, messageCount);
            }
            else
            {
                //ChatMessage message;
                string content = context.Request["content"];

                using (ErrorScope es = new ErrorScope())
                {
                    if (!ValidateCodeManager.CheckValidateCode(validateActionName, false))
                    {
                        context.Response.Write("{state:2,data:'验证码错误'}");
                        context.Response.End();
                        return;
                    }
                    else
                    {
                        messages = ChatBO.Instance.SendMessage(userId, targetUserID, content, IPUtil.GetCurrentIP(), true, maxId);
                    }

                    if (es.HasUnCatchedError)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo er)
                        {
                            if (!string.IsNullOrEmpty(data))
                                data += "<br />";
                            data += er.Message;
                        });
                        context.Response.Write("{state:1,data:'" + StringUtil.ToJavaScriptString(data) + "'}");
                        context.Response.End();
                        return;
                    }
                }

                //messages = new ChatMessageCollection();
                //if (message != null)
                //    messages.Add(message);
            }

            data = JsonBuilder.GetJson(messages);
            context.Response.ClearContent();

            if (messages.Count > 0)
            {
                context.Response.Write("{state:0,data:" + data + "}");
            }
            else
            {
                context.Response.Write("null");
            }
            context.Response.End();
        }

        #endregion
    }
}