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
using System.Web.UI.WebControls.WebParts;

using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_onlinelist_role : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_OnlineList; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }
        }

        protected RoleCollection LevelRoleList
        {
            get
            {
                return RoleSettings.GetLevelRoles();
            }
        }

        protected RoleCollection NormalRoleList
        {
            get
            {
                return RoleSettings.GetNormalRoles();
            }
        }

        protected RoleCollection ManageRoleList
        {
            get
            {
                return RoleSettings.GetManagerRoles();
            }
        }

        private RoleInOnlineCollection m_RolesInOnlineList = null;
        protected RoleInOnlineCollection RolesInOnlineList
        {
            get
            {
                if (m_RolesInOnlineList == null)
                    m_RolesInOnlineList = OnlineSettings.RolesInOnline;

                return m_RolesInOnlineList;
            }
        }

        protected string JsonRoleinOnlineList
        {
            get
            {
                return StringUtil.BuildJsonObject(this.RolesInOnlineList);
            }
        }

        protected bool RoleInList(Guid roleid)
        {
            return this.RolesInOnlineList.ContainsKey(roleid);
        }

        protected RoleInOnline GetData(Guid roleId)
        {
            RoleInOnline temp = new RoleInOnline();
            temp.SortOrder = _Request.Get<int>("sortorder." + roleId, Method.Post, 0);
            temp.RoleID = roleId;
            temp.RoleName = _Request.Get("rolename." + roleId, Method.Post);
            temp.LogoUrlSrc = _Request.Get("logourl." + roleId, Method.Post);
            return temp;
        }

        public bool SaveSettings()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("rolename", "logourl");
            RoleInOnlineCollection tempCollection = new RoleInOnlineCollection();
            RoleInOnline temp;
            foreach (Role r in RoleSettings.Roles)
            {
                if (_Request.Get(r.RoleID.ToString(), Method.Post) == "1")
                {
                    temp = GetData(r.RoleID);
                    tempCollection.Add(temp);
                }
            }

            /*注册用户和游客不能删除， 始终都必须有*/
            temp = GetData(Role.Users.RoleID);
            tempCollection.Add(temp);
            temp = GetData(Role.Guests.RoleID);
            tempCollection.Add(temp);
            /*======================================*/

            //重新排序序号， 并且避免重复
            bool flag = false;
            do
            {
                flag = false;
                for (int i = 0; i < tempCollection.Count - 1; i++)
                {
                    for (int j = i + 1; j < tempCollection.Count; j++)
                    {
                        if (tempCollection[j].SortOrder == tempCollection[i].SortOrder)
                        {
                            tempCollection[j].SortOrder++;
                            flag = true;
                        }
                    }
                }
            } while (flag);

            for (int i = 0; i < tempCollection.Count; i++)
            {
                if (string.IsNullOrEmpty(tempCollection[i].RoleName))
                {
                    msgDisplay.AddError("rolename", i, Lang_Error.Online_EmpryRoleNameError);
                }
                if (string.IsNullOrEmpty(tempCollection[i].LogoUrlSrc))
                {
                    msgDisplay.AddError("logourl", i, Lang_Error.Online_EmptyLogoUrlError);
                }
            }

            if (msgDisplay.HasAnyError())
            {
                m_RolesInOnlineList = tempCollection;
                msgDisplay.AddError(new DataNoSaveError());
            }
            else
            {
                OnlineSettings.RolesInOnline = tempCollection;
                SettingManager.SaveSettings(OnlineSettings);
                OnlineUserPool.Instance.UpdateUsersOnlineRole();
                //OnlineManager.UpdateUserOnlineRoleLogo();
            }

            return true;
        }

        protected bool IsGuests(Guid roleId)
        {
            return roleId == Role.Guests.RoleID;
        }

        protected bool IsUsers(Guid roleId)
        {
            return roleId == Role.Users.RoleID;
        }

        protected bool IsEveryone(Guid roleId)
        {
            return roleId == Role.Everyone.RoleID;
        }

    }
}