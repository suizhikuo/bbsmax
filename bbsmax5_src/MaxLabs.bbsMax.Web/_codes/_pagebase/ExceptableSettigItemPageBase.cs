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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web
{
    public class ExceptableSettigItemPageBase : PagePartBase
    {
        private RoleCollection m_BasicRoleList;
        protected RoleCollection BasicRoleList
        {
            get
            {
                if (m_BasicRoleList == null)
                {
                    RoleCollection roles = AllSettings.Current.RoleSettings.GetBasicRoles();

                    m_BasicRoleList = new RoleCollection();
                    foreach (Role role in roles)
                    {
                        if (role.RoleID == Role.Guests.RoleID || role.RoleID == Role.Everyone.RoleID || role.RoleID == Role.Users.RoleID)
                            continue;

                        m_BasicRoleList.Add(role);
                    }
                }
                return m_BasicRoleList;
            }
        }

        private RoleCollection m_ManagerRoleList;
        protected RoleCollection ManagerRoleList
        {
            get
            {
                if (m_ManagerRoleList == null)
                {
                    m_ManagerRoleList = AllSettings.Current.RoleSettings.GetManagerRoles();
                }
                return m_ManagerRoleList;
            }
        }
        private RoleCollection m_LevelRoleList;
        protected RoleCollection LevelRoleList
        {
            get
            {
                if (m_LevelRoleList == null)
                {
                    m_LevelRoleList = AllSettings.Current.RoleSettings.GetLevelRoles();
                }
                return m_LevelRoleList;
            }
        }

        private RoleCollection m_NormalRoleList;
        protected RoleCollection NormalRoleList
        {
            get
            {
                if (m_NormalRoleList == null)
                {
                     RoleCollection roles  = AllSettings.Current.RoleSettings.GetNormalRoles();

                     m_NormalRoleList = new RoleCollection();
                    foreach (Role role in roles)
                    { 
                        if(role.RoleID == Role.Guests.RoleID || role.RoleID == Role.Everyone.RoleID || role.RoleID == Role.Users.RoleID)
                            continue;

                        m_NormalRoleList.Add(role);
                    }
                }
                return m_NormalRoleList;
            }
        }


        protected bool HasNode
        {
            get
            {
                if (Parameters["hasNode"] != null)
                {
                    return Parameters["hasNode"].ToString().ToLower() == "true"; 
                }
                return false;
            }
        }

        protected string NodeName
        {
            get
            {
                if (Parameters["NodeName"] != null)
                {
                    return Parameters["NodeName"].ToString();
                }
                return string.Empty;
            }
        }

        protected string Name
        {
            get
            {
                return Parameters["name"].ToString();
            }
        }

        protected string Title
        {
            get
            {
                return Parameters["title"].ToString();
            }
        }

        protected string Description
        {
            get
            {
                return Parameters["description"].ToString();
            }
        }

        protected int Index
        {
            get
            {
                return int.Parse(Parameters["index"].ToString());
            }
        }

        protected int ItemCount
        {
            get
            {
                if (Parameters["ItemCount"] != null)
                    return int.Parse(Parameters["ItemCount"].ToString());
                else
                    return 0;
            }
        }


        protected string GetRoleName(Guid roleID)
        {
            Role role = AllSettings.Current.RoleSettings.GetRole(roleID);
            if (role != null)
                return role.Name;
            else
                return "已被删除的用户组";
        }

    }
}