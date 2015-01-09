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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_invitation : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Invitation; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SaveSetting<InvitationSettings>("savesetting");
		}

        protected override bool SetSettingItemValue(SettingBase setting, System.Reflection.PropertyInfo property)
        {
            if (property.Name == "IntiveSerialPoint")
            {
                int value = _Request.Get<int>("IntiveSerialPoint", Method.Post,0);
                UserPointType type = _Request.Get<UserPointType>("PointFieldIndex", Method.Post, UserPointType.Point1);
                UserPoint point = AllSettings.Current.PointSettings.GetUserPoint(type);

                if (point.MaxValue < value)
                {
                    ThrowError(new CustomError("IntiveSerialPoint", "超出" + point.Name + "允许的最大值"));
                    return false;
                }

                if (value <= 0)
                {
                    ThrowError(new CustomError("IntiveSerialPoint", "所需的积分值不能小于等于0"));
                    return false;
                }
            }
             if (property.Name == "ShowRegisterInviteInput")
            {
                bool value = _Request.Get<bool>("ShowRegisterInviteInput", Method.Post, true);
                InvitationSettings temp = (InvitationSettings)setting;
                temp.ShowRegisterInviteInput = value;
                return true;
            }

             return base.SetSettingItemValue(setting, property);
        }

        protected RoleCollection RoleListForAdd
        {
            get
            {
                return AllSettings.Current.RoleSettings.GetRolesForAutoAdd();
            }
        }

        public UserPointCollection PointList
        {
            get
            {

                return AllSettings.Current.PointSettings.EnabledUserPoints;
            }
        }
    }
}