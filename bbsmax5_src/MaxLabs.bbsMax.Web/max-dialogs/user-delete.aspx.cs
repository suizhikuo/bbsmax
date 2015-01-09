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
    public partial class user_delete : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.DeleteUser, UserID))
            {
                ShowError("您没有删除该用户的权限！");
                return;
            }

            if (User == null || User.UserID == 0)
            {
                ShowError(new UserNotExistsError("userid", UserID));
            }

            if (User.Roles.IsInRole(Role.Owners))
            {
                ShowError("无法删除创始人！ 如果确实要删除请转到用户组管理，把该用户从创始人组移除后再删除。");
                return;
            }

            if (MyUserID == User.UserID)
            {
                ShowError("不能删除当期正在使用的帐号（自己的帐号）");
                return;
            }

            if (_Request.IsClick("delete"))
            {
                DeleteUser();
            }
        }

        private bool? m_DeletePost;
        protected bool DeletePost
        {
            get
            {
                if (m_DeletePost == null)
                    m_DeletePost = _Request.Get<bool>("deletepost", Method.Post, false);

                return m_DeletePost.Value;
            }
        }

        private string m_TaskParam;
        private string TaskParam
        {
            get
            {
                if (m_TaskParam == null)
                    m_TaskParam = UserID + "|" + DeletePost;
                return m_TaskParam;
                 
            }
        }

        private void DeleteUser()
        {
            TaskManager.BeginTask(MyUserID, new DeleteUserTask(),TaskParam);
            //UserBO.Instance.DeleteUser(My, UserID, _Request.Get<bool>("deletePost", Method.Post, false));
            //  Return(true);
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
                    m_UserIDs = _Request.GetList<int>("userid", Method.Get, new int[0]);
                }

                return m_UserIDs;
            }
        }

        private User m_User;
        protected User User
        {
            get
            {
                if (m_User == null)
                {
                    m_User = UserBO.Instance.GetUser(UserID);
                }
                return m_User;
            }
        }

        protected int UserID
        {
            get
            {
                return _Request.Get<int>("userid", Method.All, 0);
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
                    RunningTaskCollection tasks = TaskManager.GetRunningTasks(MyUserID, typeof(DeleteUserTask), TaskParam);

                    if (tasks != null && tasks.Count != 0)
                        m_CurrentTask = tasks[0];
                }
                return m_CurrentTask;
            }
        }
    }
}