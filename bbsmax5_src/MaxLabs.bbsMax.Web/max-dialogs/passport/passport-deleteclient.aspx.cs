//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Web.plugins;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs.passport
{
    public partial class passport_deleteclient : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!My.IsOwner)
            {
                ShowError("没有权限");
                return;
            }

            if (Client == null)
            {
                ShowError("错误的passport客户端ID");
                return;
            }

            if (_Request.IsClick("delete"))
            {
                DeleteClient();
            }
        }

        private void DeleteClient()
        {
            using (ErrorScope es = new ErrorScope())
            {
                if (PassportBO.Instance.DeleteClient(My, Client.ClientID))
                {
                    ShowSuccess("客户端删除成功！", Client.ClientID);

                }
                else
                {
                    string msg = string.Empty;
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
                        msg += error.Message;
                    });
                     
                    ShowError(msg);
                }
            }
        }

        private PassportClient m_client;
        protected PassportClient Client
        {
            get
            {
                if (m_client == null)
                {
                    int clientID = _Request.Get<int>("clientid", Method.All, 0);
                    m_client = PassportBO.Instance.GetPassportClient(clientID);
                }
                return m_client;
            }
        }
    }
}