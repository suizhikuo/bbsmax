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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class feed_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
                DeleteFeed();
            }
        }

        private void DeleteFeed()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int userID = UserBO.Instance.GetCurrentUserID();
            int? feedID = _Request.Get<int>("FeedID", Method.Get);
            if (feedID == null)
            {
                msgDisplay.AddError(new InvalidParamError("FeedID").Message);
                return;
            }

            bool deleteOK = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    if (_Request.Get("feedtype", Method.Get, "") == "0")//管理员删除全站动态
                    {
                        deleteOK = FeedBO.Instance.DeleteAnyFeed(MyUserID, feedID.Value);
                    }
                    else
                    {
                        int targetUserID = _Request.Get<int>("uid", Method.Get, 0);
                        deleteOK = FeedBO.Instance.DeleteFeed(userID, targetUserID, feedID.Value);
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (deleteOK == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }

            if (deleteOK)
                Return("id", feedID); ;
        }
    }

}