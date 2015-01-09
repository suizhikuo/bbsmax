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

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public class RoleSettingPageBase : AdminPageBase
    {
        protected RoleCollection m_RoleList = null;

        protected virtual RoleCollection RoleList
        {
            get
            {
                return m_RoleList == null ? RoleSettings.Roles : m_RoleList;
            }
            set
            {
                m_RoleList = value;
            }
        }

        protected virtual Role CreateRole()
        {
            return new Role();
        }

        protected bool CanManageRole( Role role)
        {
            if (role.IsNew)
                return true;
            return  AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Setting_Roles_Manager, role);
        }

        protected virtual bool ValidateRoleDate(RoleCollection roles, MessageDisplay msgdisp)
        {
            Role myMaxRole = My.MaxRole;
            bool returnValue = true;
            for (int i = 0; i < roles.Count; i++)
            {
                if (string.IsNullOrEmpty(roles[i].Name) || roles[i].Name.Trim() == string.Empty)
                {
                    msgdisp.AddError("name", i, Lang_Error.Role_EmptyRoleNameError);
                    returnValue = false;
                }

                if ( !My.IsOwner && roles[i].IsNew && roles[i].IsManager && roles[i].Level >= myMaxRole.Level)
                {
                    msgdisp.AddError("level", i, string.Format("您当前的用户组为{0}。因此，您不能添加和{0}同级别或级别更高的用户组",myMaxRole.Name));
                    returnValue = false;
                }
            }

            return returnValue;
        }

        public virtual bool BeforeSaveSettings(RoleSettings roleSettings)
        {
            return true;
        }

        public bool SaveSettings()
        {
            MessageDisplay msdDisplay = CreateMessageDisplay("name", "title", "requiredPoint", "level");
            RoleCollection tempRoles = new RoleCollection();

            RoleSettings settings = SettingManager.CloneSetttings<RoleSettings>(AllSettings.Current.RoleSettings);

            Role temp;

            Guid[] oldRoleids = _Request.GetList<Guid>("roleid", Method.Post, new Guid[00]);

            foreach (Guid r in oldRoleids)
            {
                temp = settings.GetRole(r);

                if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.ActionWithTarget.Setting_Roles_Manager, temp))
                {
                    temp = temp == null ? CreateRole() : temp.Clone(); //不采用克隆的话可能会有些属性丢失掉
                    temp.Name = _Request.Get("Name." + r, Method.Post);
                    temp.Title = _Request.Get("title." + r, Method.Post);
                    temp.RequiredPoint = _Request.Get<int>("RequiredPoint." + r, Method.Post, 0);
                    temp.Color = _Request.Get("Color." + r, Method.Post);
                    temp.IconUrlSrc = _Request.Get("IconUrl." + r, Method.Post);
                    temp.StarLevel = _Request.Get<int>("StarLevel." + r, Method.Post, 0);
                    temp.Level = _Request.Get<int>("Level." + r, Method.Post, 0);
                    temp.IsNew = _Request.Get<bool>("isnew." + r, Method.Post, false);
                    temp.CanLoginConsole = _Request.Get<bool>("CanLoginConsole." + r, Method.Post, false) && temp.IsManager;
                    temp.RoleID = r;
                    if (temp.IsLevel)
                        temp.Name = temp.Title;
                }
                else
                {
                    if (temp == null)
                        continue;
                }

                tempRoles.Add(temp);
            }

            //Nonsupport Javascript
            if (_Request.Get("newroleid", Method.Post, "").Contains("{0}"))
            {
                Role r = CreateRole();
                r.Name = _Request.Get("name.new.{0}", Method.Post);
                r.Title = _Request.Get("title.new.{0}", Method.Post);
                r.StarLevel = _Request.Get<int>("starlevel.new.{0}", Method.Post, 0);
                r.RequiredPoint = _Request.Get<int>("RequiredPoint.new.{0}", Method.Post, 0);
                r.Color = _Request.Get("color.new.{0}", Method.Post);
                r.IconUrlSrc = _Request.Get("IconUrl.new.{0}", Method.Post);
                r.Level = _Request.Get<int>("Level.new.{0}", Method.Post, 0);
                r.CanLoginConsole = _Request.Get<bool>("CanLoginConsole.new.{0}", Method.Post, false) && r.IsManager;
                if (r.IsLevel)
                    r.Name = r.Title;
                if (!string.IsNullOrEmpty(r.Name)) tempRoles.Add(r);
            }
            else
            {

                int[] newroleid = _Request.GetList<int>("newroleid", Method.Post, new int[0]);

                foreach (int i in newroleid)
                {
                    Role r = CreateRole();
                    r.Name = _Request.Get("name.new." + i, Method.Post);
                    r.Title = _Request.Get("title.new." + i, Method.Post);
                    r.StarLevel = _Request.Get<int>("starlevel.new." + i, Method.Post, 0);
                    r.RequiredPoint = _Request.Get<int>("RequiredPoint.new." + i, Method.Post, 0);
                    r.Color = _Request.Get("color.new." + i, Method.Post);
                    r.IconUrlSrc = _Request.Get("IconUrl.new." + i, Method.Post);
                    r.Level = _Request.Get<int>("Level.new." + i, Method.Post, 0);
                    r.CanLoginConsole = _Request.Get<bool>("CanLoginConsole.new." + i, Method.Post, false) && r.IsManager;
                    if (r.IsLevel)
                        r.Name = r.Title;
                    tempRoles.Add(r);
                }
            }

            if (ValidateRoleDate(tempRoles, msdDisplay))
            {
                foreach (Role r in tempRoles)
                {
                    settings.SetRole(r);
                }

                if (BeforeSaveSettings(settings))
                {
                    SettingManager.SaveSettings(settings);
                    m_RoleList = null;
                    return true;
                }
            }

            m_RoleList = tempRoles;
            msdDisplay.AddError(new DataNoSaveError());
            return false;

        }



    }
}