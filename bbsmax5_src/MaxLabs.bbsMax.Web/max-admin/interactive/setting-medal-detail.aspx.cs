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
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using System.Collections.Generic;


namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class setting_medal_detail : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Medals; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MedalID > 0 && Medal == null)
            {
                ShowError(new InvalidParamError("medalID"));
                return;
            }

            if (_Request.IsClick("savesetting"))
            {
                SaveSetting();
            }
		}

        private void SaveSetting()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("medalname", "sortorder", "medallevel");

            Medal medal = new Medal();

            if (IsEdit)
            {
                medal.ID = MedalID;
            }
            else
            {
                medal.ID = AllSettings.Current.MedalSettings.MaxMedalID + 1;
            }

            medal.Name = _Request.Get("medalname", Method.Post, string.Empty);

            if (medal.Name == string.Empty)
                msgDisplay.AddError("medalname", "图标名称不能为空");

            medal.SortOrder = _Request.Get<int>("sortorder", Method.Post, 0);
            medal.Enable = _Request.Get<bool>("enable", Method.Post, false);
            medal.IsHidden = _Request.Get<bool>("IsHidden", Method.Post, false);

            medal.IsCustom = _Request.Get<bool>("isAuto", Method.Post, false) == false;
            if (medal.IsCustom)
                medal.Condition = string.Empty;
            else
                medal.Condition = _Request.Get("condition", Method.Post, string.Empty);

            m_MedalCondition = medal.Condition;

            bool hasMedallevelError = false;
            if (medal.Condition == string.Empty && medal.IsCustom == false)
            {
                msgDisplay.AddError("medallevel", "请选择规则");
                hasMedallevelError = true;
            }

            medal.Levels = new MedalLevelCollection();

            int[] ids = _Request.GetList<int>("ids", Method.Post, new int[0] { });

            if (IsEdit)
                medal.MaxLevelID = Medal.MaxLevelID;

            List<int> values = new List<int>();

            m_MedalLevels = new MedalLevelCollection();
            foreach (int id in ids)
            {
                MedalLevel level = new MedalLevel();

                if (IsEdit)
                {
                    foreach (MedalLevel tempMedalLevel in Medal.Levels)
                    {
                        if (id == tempMedalLevel.ID)
                        {
                            level.ID = id;
                            break;
                        }
                    }
                }
                if (level.ID == 0)
                {
                    medal.MaxLevelID = medal.MaxLevelID + 1;
                    level.ID = medal.MaxLevelID;
                }

                if (_Request.Get("levelName_" + id, Method.Post) == null)
                    continue;

                level.Name = _Request.Get("levelName_" + id, Method.Post, string.Empty).Trim();
                level.IconSrc = _Request.Get("IconSrc_" + id, Method.Post, string.Empty).Trim();

                if (medal.IsCustom)
                    level.Condition = _Request.Get("conditionDescription_" + id, Method.Post, string.Empty).Trim();
                else
                {
                    level.Condition = string.Empty;
                    level.Value = _Request.Get<int>("levelValue_" + id, Method.Post, 0);
                }

                if (hasMedallevelError == false)
                {
                    //if (level.Name == string.Empty)
                    //{
                    //    msgDisplay.AddError("medallevel", "等级名称不能为空");
                    //}
                    if (level.IconSrc == string.Empty)
                    {
                        msgDisplay.AddError("medallevel", "等级图标不能为空");
                    }
                    //else if (medal.IsCustom && level.Condition == string.Empty)
                    //{
                    //    msgDisplay.AddError("medallevel", "点亮图标说明不能为空");
                    //}
                    else if (medal.IsCustom == false && values.Contains(level.Value))
                    {
                        msgDisplay.AddError("medallevel", "点亮图标需达到的值不能相同");
                    }
                }
                if (medal.IsCustom == false)
                    values.Add(level.Value);

                medal.Levels.Add(level,medal.IsCustom == false);
                m_MedalLevels.Add(level, false);
            }

            m_IsCustom = medal.IsCustom;

            if (msgDisplay.HasAnyError())
                return;

            MedalSettings medalSetting = new MedalSettings();

            medalSetting.Medals = new MedalCollection();
            foreach (Medal tempMedal in AllSettings.Current.MedalSettings.Medals)
            {
                if (IsEdit && medal.ID == tempMedal.ID)
                {
                    medalSetting.Medals.Add(medal);
                }
                else
                    medalSetting.Medals.Add(tempMedal);
            }

            if (IsEdit)
                medalSetting.MaxMedalID = AllSettings.Current.MedalSettings.MaxMedalID;
            else
            {
                medalSetting.Medals.Add(medal);
                medalSetting.MaxMedalID = medal.ID;
            }

            bool success = false;
            try
            {
                using (new ErrorScope())
                {

                    if (SettingManager.SaveSettings(medalSetting) == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        if(IsEdit)
                            UserBO.Instance.RemoveAllUserCache();
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }

            if (success)
                JumpTo("interactive/setting-medals.aspx");
        }

        private MedalLevelCollection m_MedalLevels;
        protected MedalLevelCollection MedalLevels
        {
            get
            {
                if (m_MedalLevels == null)
                {
                    if (Medal != null)
                        m_MedalLevels = Medal.Levels;
                    else
                        m_MedalLevels = new MedalLevelCollection();
                }

                return m_MedalLevels;
            }
        }

        private bool? m_IsCustom;
        protected bool IsCustom
        {
            get
            {
                if (m_IsCustom == null)
                {
                    if (Medal != null)
                        m_IsCustom = Medal.IsCustom;
                    else
                        m_IsCustom = false;
                }
                return m_IsCustom.Value;
            }
        }

        private string m_MedalCondition;
        protected string MedalCondition
        {
            get
            {
                if (m_MedalCondition == null)
                {
                    if (Medal != null)
                        m_MedalCondition = Medal.Condition;
                    else
                        m_MedalCondition = string.Empty;
                }

                return m_MedalCondition;
            }
        }

        protected int MedalID
        {
            get
            {
                return _Request.Get<int>("ID", Method.Get, 0);
            }
        }

        protected bool IsEdit
        {
            get
            {
                return MedalID > 0;
            }
        }

        private Medal m_Medal;
        protected Medal Medal
        {
            get
            {
                if (m_Medal == null)
                    m_Medal = AllSettings.Current.MedalSettings.Medals.GetValue(MedalID);

                return m_Medal;
            }
        }

        private PointExpressionColumCollection m_Colums;
        protected PointExpressionColumCollection Colums
        {
            get
            {
                if (m_Colums == null)
                {
                    m_Colums = UserBO.Instance.GetGeneralPointExpressionColums();
                }

                return m_Colums;
            }
        }

    }
}