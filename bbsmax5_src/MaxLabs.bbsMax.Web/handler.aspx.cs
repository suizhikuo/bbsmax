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

namespace MaxLabs.bbsMax.Web
{
    public partial class handler : BbsPageBase
    {
        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        protected override bool NeedCheckAccess
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected override bool NeedLogin
        {
            get { return false; }
        }

        protected override bool NeedProcessOutput
        {
            get { return false; }
        }

        protected override bool NeedAdminLogin
        {
            get { return false; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string handlerName = Request.QueryString["max_handler"];

            if (string.IsNullOrEmpty(handlerName))
                return;
                
            using (ErrorScope es = new ErrorScope())
            {
                MaxLabs.bbsMax.AppHandlers.AppHandlerManager.ExecuteHandler(handlerName, HttpContext.Current);

                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    ShowError(error);
                });
            }
        }
    }
}