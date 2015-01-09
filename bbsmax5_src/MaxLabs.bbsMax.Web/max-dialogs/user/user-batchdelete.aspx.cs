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

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.StepByStepTasks;

namespace MaxLabs.bbsMax.Web.max_pages.dialogs
{
    public partial class user_batchdelete : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AllSettings.Current.ManageUserPermissionSet.HasPermissionForSomeone(My, ManageUserPermissionSet.ActionWithTarget.DeleteUser))
            {
                ShowError("您没有删除用户的权限！");
                return;
            }

            bool containsSelf = false;

            foreach (int uid in UserIDs)
            {
                if (uid == MyUserID)
                {
                    containsSelf = true;
                }
            }

            if (containsSelf)
            {
                ShowError("待删除用户列表里不能包含自己的帐号！");
                return;
            }

            if (_Request.IsClick("delete"))
            {
                DeleteUsers();
            }
        }

        private void DeleteUsers()
        {

            TaskManager.BeginTask(MyUserID, new BatchDeleteUser(), StringUtil.Join(UserIDs));
        }

        protected int Index
        {
            get;
            set;
        }

        protected int[] m_UserIDs;
        protected int[] UserIDs
        {
            get
            {
                if (m_UserIDs == null)
                {
                    m_UserIDs = _Request.GetList<int>("userids", Method.Post, new int[0]);
                }

                return m_UserIDs;
            }
        }

        private bool m_TaskGeted = false;
        private RunningTask m_CurrentTask = null;
        protected RunningTask CurrentTask
        {
            get
            {
                if (m_TaskGeted == false)
                {
                    m_TaskGeted = true;
                    RunningTaskCollection tasks = TaskManager.GetRunningTasks(MyUserID, typeof(BatchDeleteUser),StringUtil.Join(UserIDs));

                    if (tasks != null && tasks.Count != 0)
                        m_CurrentTask = tasks[0];
                }
                return m_CurrentTask;
            }
        }
    }
}