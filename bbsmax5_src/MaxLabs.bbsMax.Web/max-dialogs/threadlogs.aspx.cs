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
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class threadlogs : DialogPageBase
    {
        protected ThreadManageLogCollectionV5 threadlogList;
        protected BasicThread thread;

        protected void Page_Load(object sender, EventArgs e)
        {
            int threadid = _Request.Get<int>("threadid", Method.Get, 0);
            if (threadid < 0)
            {
                ShowError(new InvalidParamError("threadid"));
            }

            thread = PostBOV5.Instance.GetThread(threadid);

            if (thread == null)
            {
                ShowError(new ThreadNotExistsError());
            }

            if (!AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(thread.ForumID).Can(My, ForumPermissionSetNode.Action.ViewThread))
            {
                ShowError("您没有权限查看主题操作记录！");
            }

            threadlogList = PostBOV5.Instance.GetThreadManageLogs(threadid);

        }

    }
}