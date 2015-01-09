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
using System.Configuration;
using System.Collections.Generic;
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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_medals : AdminPageBase //: SettingPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Medals; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("savesetting"))
            {
                SaveSetting();
            }
            else if (_Request.IsClick("deletemedals"))
            {
                DeleteMedals();
            }
            else if (string.Compare(_Request.Get("action", Method.Get, string.Empty), "delete") == 0)
            {
                DeleteMedal();
            }
        }

        private void SaveSetting()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("name", "levelname", "iconsrc", "condition");

            MedalCollection medals = new MedalCollection();

            int i = 0;
            foreach (Medal medal in MedalList)
            {
                Medal tempMedal = new Medal();
                tempMedal.SortOrder = _Request.Get<int>("sortorder_" + medal.ID, Method.Post, 0);
                tempMedal.Name = _Request.Get("name_" + medal.ID, Method.Post, string.Empty).Trim();

                if (tempMedal.Name == string.Empty)
                {
                    msgDisplay.AddError("name", i, "勋章名称不能为空");
                }

                tempMedal.Enable = _Request.Get<bool>("enable_" + medal.ID, Method.Post, false);
                tempMedal.IsHidden = _Request.Get<bool>("isHidden_" + medal.ID, Method.Post, false);

                tempMedal.Levels = new MedalLevelCollection();

                tempMedal.Condition = medal.Condition;
                //if (medal.IsCustom)
                //{
                //    //tempMedal.Condition = _Request.Get("condition_" + medal.ID, Method.Post, string.Empty).Trim();
                //    //msgDisplay.AddError("condition", i, "点亮规则不能为空");
                //}
                //else
                //{
                    List<int> values = new List<int>();
                    foreach (MedalLevel level in medal.Levels)
                    {
                        MedalLevel tempLevel = new MedalLevel();

                        tempLevel.Name = level.Name;
                        tempLevel.Value = level.Value;
                        tempLevel.IconSrc = level.IconSrc;
                        tempLevel.Condition = level.Condition;
                        //tempLevel.Name = _Request.Get("levelName_" + medal.ID + "_" + level.ID, Method.Post, string.Empty).Trim();
                        //if (tempLevel.Name == string.Empty)
                        //{
                        //    msgDisplay.AddError("levelname", i, "等级名称不能为空");
                        //}
                        //tempLevel.Value = _Request.Get<int>("levelValue_" + medal.ID + "_" + level.ID, Method.Post, 0);

                        //if (values.Contains(tempLevel.Value))
                        //{
                        //    msgDisplay.AddError("Condition",i,"不同等级的点亮规则必须不同");
                        //}
                        //values.Add(tempLevel.Value);

                        //tempLevel.IconSrc = _Request.Get("iconSrc_" + medal.ID + "_" + level.ID, Method.Post, string.Empty).Trim();
                        //if (tempLevel.IconSrc == string.Empty)
                        //{
                        //    msgDisplay.AddError("iconSrc", i, "等级图标不能为空");
                        //}

                        tempLevel.ID = level.ID;

                        tempMedal.Levels.Add(tempLevel);
                    }
                //}

                tempMedal.ID = medal.ID;
                tempMedal.IsCustom = medal.IsCustom;
                tempMedal.MaxLevelID = medal.MaxLevelID;

                medals.Add(tempMedal);

                i++;
            }

            if (msgDisplay.HasAnyError())
                return;

            MedalSettings setting = new MedalSettings();
            setting.Medals = medals;
            setting.MaxMedalID = AllSettings.Current.MedalSettings.MaxMedalID;

            try
            {
                if (!SettingManager.SaveSettings(setting))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    UserBO.Instance.RemoveAllUserCache();
                    m_MedalList = null;
                    _Request.Clear(Method.Post);
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private void DeleteMedal()
        {
            int medalID = _Request.Get<int>("medalid", Method.Get, 0);

            if (medalID < 0)
            {
                ThrowError<InvalidParamError>(new InvalidParamError("medalID"));
                return;
            }

            List<int> ids = new List<int>();
            ids.Add(medalID);

            DeleteMedals(ids);
        }

        private void DeleteMedals()
        {
            int[] medalIDs = _Request.GetList<int>("medalids", Method.Post, new int[0] { });

            List<int> temp = new List<int>();

            foreach (int id in medalIDs)
            {
                temp.Add(id);
            }

            if (temp.Count == 0)
                return;

            DeleteMedals(temp);
        }

        private void DeleteMedals(List<int> medalIDs)
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            MedalCollection medals = new MedalCollection();

            int i = 0;
            foreach (Medal medal in MedalList)
            {
                if (medalIDs.Contains(medal.ID) == false)
                    medals.Add(medal);
            }

            MedalSettings setting = new MedalSettings();
            setting.Medals = medals;
            setting.MaxMedalID = AllSettings.Current.MedalSettings.MaxMedalID;

            try
            {
                if (!SettingManager.SaveSettings(setting))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    UserBO.Instance.RemoveAllUserCache();

                    m_MedalList = null;
                    _Request.Clear(Method.Post);
                    Logs.LogManager.LogOperation(
                        new Medal_DeleteMedalByIDs(MyUserID, My.Name, _Request.IpAddress, medalIDs)
                    );
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private MedalCollection m_MedalList;
        protected MedalCollection MedalList
        {
            get
            {
                if (m_MedalList == null)
                    m_MedalList = AllSettings.Current.MedalSettings.Medals;

                return m_MedalList;
            }
        }

        private PointExpressionColumCollection m_Colums;
        protected PointExpressionColumCollection Colums
        {
            get
            {
                if(m_Colums == null)
                    m_Colums = UserBO.Instance.GetGeneralPointExpressionColums();
                return m_Colums;
            }
        }

        protected string GetConditionName(string condition)
        {
            if (string.Compare("Point_0", condition, true) == 0)
                return "总积分";

            foreach (PointExpressionColum colum in Colums)
            {
                if (string.Compare(colum.Colum, condition, true) == 0)
                    return colum.Description;
            }

            return condition;
        }
    }
}