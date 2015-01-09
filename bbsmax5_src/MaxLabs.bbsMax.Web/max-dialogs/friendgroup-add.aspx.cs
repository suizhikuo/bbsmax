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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class friendgroup_add : DialogPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            if (_Request.IsClick("add"))
            {
                Add();
            }
        }

        private void Add()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            string groupName = _Request.Get("groupName", Method.Post);//"!"号作为特殊标记

            //bool success = false;
            int newGroupID = 0;
            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    newGroupID = FriendBO.Instance.AddFriendGroup(MyUserID, groupName);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (newGroupID == 0)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }

            if (newGroupID > 0)
            {
                JsonBuilder json = new JsonBuilder();
                json.Set("groupID", newGroupID);
                json.Set("groupName", groupName);
                Return(json);
            }
        }
    }
}