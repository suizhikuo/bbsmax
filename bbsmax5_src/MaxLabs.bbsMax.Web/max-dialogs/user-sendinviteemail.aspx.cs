//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class user_sendinviteemail : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("sendmail"))
            {
                SendEmail();
            }
        }

        private void SendEmail()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string serialString, email;
            serialString = _Request.Get("serial", MaxLabs.WebEngine.Method.Post);
            email = _Request.Get("email", MaxLabs.WebEngine.Method.Post);

            bool success;

            using (ErrorScope es = new ErrorScope())
            {
                Guid serial;
                try
                {
                    serial = new Guid(serialString);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                    return;
                }
                success = InviteBO.Instance.SendInviteByEmail(My, serial, email);
                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }

            if (success)
            {
                ShowSuccess("邀请邮件发送成功！");
            }
        }
    }
}