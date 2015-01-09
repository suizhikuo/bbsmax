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
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_roles_level : RoleSettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Roles_Level; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }
        }

        protected override bool ValidateRoleDate(RoleCollection roles, MessageDisplay msgdisp)
        {
            bool returnValue = true;
            for (int i = 0; i < roles.Count; i++)
            {
                if (string.IsNullOrEmpty(roles[i].Title) || roles[i].Title.Trim() == string.Empty)
                {
                    msgdisp.AddError("title", i, Lang_Error.Role_EmptyTitleError);
                    returnValue = false;
                }

                for (int j = i + 1; j < roles.Count; j++)
                {
                    if (i!=j)
                    {
                        if (roles[i].RequiredPoint == roles[j].RequiredPoint)
                        {
                            msgdisp.AddError("RequiredPoint", i, string.Format(Lang_Error.Role_DuplicatePointError,i+1,j+1));
                            msgdisp.AddError("RequiredPoint", j, string.Format(Lang_Error.Role_DuplicatePointError, j+1,i+1));
                            returnValue = false;
                        }
                    }
                }
            }

            return returnValue;
        }

        protected LevelLieOn LevelLieOn
        {
            get
            {
                return RoleSettings.LevelLieOn;
            }
        }

        protected override RoleCollection RoleList
        {
            get
            {
                return m_RoleList == null ? RoleSettings.GetLevelRoles() : m_RoleList;
            }
            set
            {
                m_RoleList = value;
            }
        }

        protected override Role CreateRole()
        {
            return Role.CreateLevelRole();
        }

        public override bool BeforeSaveSettings(RoleSettings roleSettings)
        {
            roleSettings.LevelLieOn = _Request.Get<LevelLieOn>("levellieon", Method.Post, LevelLieOn.Point);
            return true;
        }
    }
}