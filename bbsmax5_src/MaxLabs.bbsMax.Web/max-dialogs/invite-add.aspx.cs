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

using System.Collections.Generic;

using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class invite_add : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!InviteBO.Instance.CanAddInviteSerial(My))
            {
                ShowError(new NoPermissionManageInviteSerialError());
                return;
            }


            if (_Request.IsClick("add"))
            {
                AddSerial();
            }


        }

        private void AddSerial()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int addCount;
            string[] userNames;
            IList<int> userIDs = new List<int>();
            addCount = _Request.Get<int>("addnum", 0);
            userNames = StringUtil.GetLines(_Request.Get("usernames", Method.Post, string.Empty, false));
            userIDs = UserBO.Instance.GetUserIDs(userNames);
            InviteBO.Instance.CreateInviteSerials(My, userIDs, addCount);
        }
    }
}