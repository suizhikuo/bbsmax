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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class feed_shield : DialogPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("shieldfeed"))
            {
                ShieldFeed();
            }
        }

        private void ShieldFeed()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int userID = UserBO.Instance.GetCurrentUserID();
            int? friendUserID = _Request.Get<int>("frienduserid", Method.Get);
            if (friendUserID == null)
            {
                msgDisplay.AddError(new InvalidParamError("frienduserid").Message);
                return;
            }

            Guid appID = Guid.Empty;
            try
            {
                appID = new Guid(_Request.Get("appID", Method.Get));
            }
            catch
            {
                msgDisplay.AddError(new InvalidParamError("appID").Message);
                return;
            }

            int? actionType = _Request.Get<int>("actionType", Method.Get);
            if (actionType == null)
            {
                msgDisplay.AddError(new InvalidParamError("actionType").Message);
                return;
            }

            string type = _Request.Get("shieldType", Method.Post);
            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                if (type == null || type.Trim() != "1")
                {
                    try
                    {
                        success = FeedBO.Instance.FiltrateFeed(friendUserID.Value, appID, actionType.Value);
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddException(ex);
                    }

                    if (success)
                    {
                        JsonBuilder json = new JsonBuilder();
                        json.Set("friendUserID", friendUserID.Value);
                        json.Set("appID", appID);
                        json.Set("actionType", actionType.Value);
                        Return(json);
                    }
                }
                else
                {
                    try
                    {
                        success = FeedBO.Instance.FiltrateFeed(appID, actionType.Value);
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddException(ex);
                    }
                    if (success)
                    {
                        JsonBuilder json = new JsonBuilder();
                        json.Set("appID", appID);
                        json.Set("actionType", actionType.Value);
                        Return(json);
                    }
                }

                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }
    }
}