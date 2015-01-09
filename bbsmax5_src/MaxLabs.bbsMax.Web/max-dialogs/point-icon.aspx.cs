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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class point_icon : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Setting_PostIcon;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savepointicon"))
            {
                SavePointIcon();
            }
            else if (_Request.IsClick("deletepointicon"))
            {
                DeletePointIcon();
            }
        }
        private void DeletePointIcon()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            try
            {
                if (!PointActionManager.DeletePointIcon(CurrentUserPoint.Type))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    Return(true);
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void SavePointIcon()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("pointValue", "iconCount", "icons");

            string valueString = _Request.Get("pointValue",Method.Post,string.Empty);
            int pointValue;

            if(!int.TryParse(valueString,out pointValue))
            {
                msgDisplay.AddError("pointValue",string.Format(Lang_Error.User_UserPointIconValueError,CurrentUserPoint.Name));
            }

            valueString = _Request.Get("iconCount",Method.Post);
            int iconCount;

            if (!int.TryParse(valueString, out iconCount))
            {
                msgDisplay.AddError("iconCount", Lang_Error.User_UserPointUpgradeIconCountError);
            }

            string[] icons = StringUtil.GetLines(_Request.Get("icons",Method.Post,string.Empty));

            try
            {
                if (!PointActionManager.UpdatePointIcon(CurrentUserPoint.Type, pointValue, iconCount, icons))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                    Return(true);
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private UserPoint currentUserPoint;
        protected UserPoint CurrentUserPoint
        {
            get
            {
                if (currentUserPoint == null)
                {
                    UserPointType type = _Request.Get<UserPointType>("pointtype",Method.Get,UserPointType.GeneralPoint);
                    currentUserPoint = AllSettings.Current.PointSettings.GetUserPoint(type);
                }
                return currentUserPoint;
            }
        }


        private bool hasGet = false;
        private PointIcon pointIcon;
        protected PointIcon PointIcon
        {
            get
            {
                if (pointIcon == null && !hasGet)
                {
                    foreach (PointIcon tempPointIcon in AllSettings.Current.PointSettings.PointIcons)
                    {
                        if (tempPointIcon.PointType == CurrentUserPoint.Type)
                        {
                            hasGet = true;
                            pointIcon = tempPointIcon;
                            break;
                        }
                    }
                }
                return pointIcon;
            }
        }


        protected string PointIcons
        {
            get
            {
                if (PointIcon == null)
                {
                    return string.Empty;
                }
                return StringUtil.Join(PointIcon.Icons,"\r\n");
            }
        }

    }


}