//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;


using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.StepByStepTasks;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_userpoint : AdminPageBase //: SettingPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_UserPoint; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("saveuserpoints"))
            {
                SaveUserPoints();
            }
            else if (_Request.IsClick("updateuserpoints"))
            {
                UpdateUserPoints();
            }
            else if (_Request.IsClick("savePointOtherSetting"))
            {
                SavePointOtherSetting();
            }
            else if (_Request.IsClick("updatePoints"))
            {
                UpdatePoints();
            }
        }
        private void UpdateUserPoints()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            try
            {
                UserBO.Instance.UpdateAllUserPoints();
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        protected bool CanRecharge(UserPointType type)
        {
            if (AllSettings.Current.PaySettings.EnablePointRecharge == false)
                return false;

            foreach (PointRechargeRule temp in AllSettings.Current.PointSettings.PointRechargeRules)
            {
                if (type == temp.UserPointType && temp.Enable)
                {
                    return true;
                }
            }

            return false;
        }

        private void SaveUserPoints()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("maxValue", "minValue", "InitialValue", "name");
            string type = _Request.Get("type", Method.Get, string.Empty);

            UserPointCollection userPoints = new UserPointCollection();
            int i = 0;
            foreach (UserPoint tempUserPoint in AllSettings.Current.PointSettings.UserPoints)
            {
                int pointID = (int)tempUserPoint.Type;
                UserPoint userPoint = new UserPoint();
                userPoint.Enable = _Request.Get<bool>("enable." + pointID, Method.Post, false);
                userPoint.Display = _Request.Get<bool>("display." + pointID, Method.Post, false);
                int value;
                string valueString = _Request.Get("maxValue."+pointID,Method.Post,string.Empty);
                if(valueString == string.Empty)
                {
                    userPoint.MaxValue = int.MaxValue;
                }
                else if(int.TryParse(valueString,out value))
                {
                    userPoint.MaxValue = value;
                }
                else
                {
                    msgDisplay.AddError("maxValue", i, Lang_Error.User_UserPointMaxValueFormatError);
                }

                valueString = _Request.Get("minValue." + pointID, Method.Post, string.Empty);
                if (valueString == string.Empty)
                {
                    userPoint.MinValue = 0;
                }
                else if (int.TryParse(valueString, out value))
                {
                    userPoint.MinValue = value;
                }
                else
                {
                    msgDisplay.AddError("minValue", i, Lang_Error.User_UserPointMinValueFormatError);
                }

                valueString = _Request.Get("InitialValue." + pointID, Method.Post, string.Empty);
                if (valueString == string.Empty)
                {
                    userPoint.InitialValue = 0;
                }
                else if (int.TryParse(valueString, out value))
                {
                    userPoint.InitialValue = value;
                }
                else
                {
                    msgDisplay.AddError("InitialValue", i, Lang_Error.User_UserPointInitialValueFormatError);
                }

                userPoint.UnitName = _Request.Get("unitName." + pointID, Method.Post, string.Empty);
                userPoint.Name = _Request.Get("name." + pointID, Method.Post, string.Empty);
                userPoint.Type = tempUserPoint.Type;

                userPoints.Add(userPoint);

                i++;
            }

            if (msgDisplay.HasAnyError())
                return;


            try
            {
                using (ErrorScope errorScope = new ErrorScope())
                {
                    if (!PointActionManager.UpdateUserPointSetting(userPoints))
                    {
                        errorScope.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error.TatgetName, error.TargetLine, error.Message);
                        });
                    }
                    else
                    {
                        PostBOV5.Instance.ClearShowChargePointLinks(); 
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void SavePointOtherSetting()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("generalPointName", "generalPointExpression", "pointTradeRate");
            string generalPointName = _Request.Get("generalPointName", Method.Post, string.Empty);
            string generalPointExpression = _Request.Get("generalPointExpression", Method.Post, string.Empty);
            string pointTradeRateString = _Request.Get("pointTradeRate",Method.Post,string.Empty);

            if(generalPointName == string.Empty)
            {
                msgDisplay.AddError("generalPointName",Lang_Error.User_UserPointEmptyGeneralPointNameError);
            }
            
            int pointTradeRate;
            if (!int.TryParse(pointTradeRateString, out pointTradeRate))
            {
                msgDisplay.AddError("pointTradeRate",Lang_Error.User_UserPointTradeRateFormatError);
            }

            bool displayGeneralPoint = _Request.Get<bool>("displayGeneralPoint",Method.Post,true);

            if (msgDisplay.HasAnyError())
            {
                return;
            }
            try
            {
                if (!UserBO.Instance.UpdatePointsExpression(generalPointExpression))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {


                    PointSettings setting = SettingManager.CloneSetttings<PointSettings>(AllSettings.Current.PointSettings);

                    string oldExpression = setting.GeneralPointExpression;

                    setting.GeneralPointName = generalPointName;
                    setting.GeneralPointExpression = generalPointExpression;
                    setting.TradeRate = pointTradeRate;
                    setting.DisplayGeneralPoint = displayGeneralPoint;
                    if (!SettingManager.SaveSettings(setting))
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        if (oldExpression != AllSettings.Current.PointSettings.GeneralPointExpression)
                        {
                            if (TaskManager.BeginTask(MyUserID, new ReCountUsersPointTask(), string.Empty))
                            {

                            }
                        }
                    }
                    //else
                    //    AllSettings.Current.PointSettings = setting;

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void UpdatePoints()
        {
            if (TaskManager.BeginTask(MyUserID, new ReCountUsersPointTask(), string.Empty))
            {

            }
        }

        protected string GeneralPointPointIconUpdateDescription
        {
            get
            {
                return GetPointIconUpdateDescription(UserPointType.GeneralPoint);
            }
        }
        protected string GetPointIconUpdateDescription(UserPointType pointType)
        {
            return AllSettings.Current.PointSettings.PointIcons.GetPointIconUpgradeDescription(pointType);
        }

        protected string PointExpressionColumsDescription
        {
            get
            {
                StringBuilder description = new StringBuilder();
                foreach (PointExpressionColum colum in UserBO.Instance.GetGeneralPointExpressionColums())
                {
                    description.Append(colum.FriendlyShow).Append("(").Append(colum.Description).Append("),");
                }
                if (description.Length > 0)
                {
                    return description.ToString(0, description.Length - 1);
                }
                return string.Empty;
            }
        }
    }
}