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
using MaxLabs.bbsMax.Web;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class systemnotify_edit : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_SystemNotify; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savenotify"))
            {
                SaveNotify();
            }
        }


        private void SaveNotify()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            Guid[] roles = _Request.GetList<Guid>("roles", Method.Post, new Guid[0]);
            List<int> userIDs = UserBO.Instance.GetUserIDs(StringUtil.GetLines(_Request.Get("usernames", Method.Post,string.Empty,false)));
            DateTime beginDate, endDate;
            string subject, content;

            beginDate = DateTimeUtil.ParseBeginDateTime(_Request.Get("begindate"));
            endDate = DateTimeUtil.ParseEndDateTime(_Request.Get("endDate"));
            subject = _Request.Get("subject", Method.Post);
            content = _Request.Get("content", Method.Post,string.Empty, false);

            if (IsNew)
            {
                NotifyBO.Instance.CreateSystemNotify(MyUserID, subject, content, roles, userIDs, beginDate, endDate);
            }
            else
            {
                NotifyBO.Instance.UpdateSystemNotify(MyUserID, subject, Notify.NotifyID, content, roles, userIDs, beginDate, endDate);
            }

            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
            else
            {
                if (IsNew)
                {
                    ShowSuccess("发布通知成功！", "manage-systemnotify.aspx");
                }

                else
                {
                    ShowSuccess("修改通知成功！", "manage-systemnotify.aspx");
                }
            }
        }

        protected RoleCollection LevelRoles
        {
            get
            {
                return RoleSettings.GetLevelRoles();
            }
        }

        protected bool IsNew
        {
            get
            {
                return Notify == null;
            }
        }

        protected string Subject
        {
            get
            {
                return IsNew ? string.Empty : Notify.Subject;
            }
        }

        protected string Content
        {
            get
            {
                return IsNew ? string.Empty : Notify.Content;
            }
        }

        protected string BeginDate
        {
            get
            {
                return IsNew ? string.Empty : OutputDateTime(Notify.BeginDate);
            }
        }

        /// <summary>
        /// 注册用户特殊处理
        /// </summary>
        protected Role UsersRole
        {
            get
            {
                return Role.Users;
            }
        }

        protected string EndDate
        {
            get
            {
                return IsNew ? string.Empty : OutputDateTime(Notify.EndDate);
            }
        }

        private string m_usernames = null;
        protected string Usernames
        {
            get
            {
                if (m_usernames != null)
                    return m_usernames;
                if (IsNew)
                    return string.Empty;
                SimpleUserCollection users = UserBO.Instance.GetSimpleUsers(Notify.ReceiveUserIDs);
                m_usernames = string.Empty;
                foreach (SimpleUser user in users)
                {
                    m_usernames += user.Username + "\r\n";
                }

                return m_usernames;
            }
        }

        protected bool IsChecked( Guid roleid )
        {
            if (IsNew)
                return false;
            return Notify.ReceiveRoles.Contains(roleid);
        }

        private SystemNotify m_notify;
        protected SystemNotify Notify
        {
            get
            {
                if (m_notify == null)
                {
                    m_notify = NotifyBO.Instance.GetSystemNotify(NotifyID);
                }
                return m_notify;
            }
        }

        protected int NotifyID
        {
            get
            {
                return _Request.Get<int>("notifyID", Method.Get, 0);
            }
        }

        protected RoleCollection BasicRoles
        {
            get
            {
                return RoleSettings.GetNormalRoles();
            }
        }

        protected RoleCollection ManagerRoles
        {
            get
            {
                return RoleSettings.GetManagerRoles();
            }
        }
    }
}