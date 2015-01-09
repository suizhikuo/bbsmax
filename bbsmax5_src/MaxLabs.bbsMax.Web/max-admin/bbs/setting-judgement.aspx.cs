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

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_judgement : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Setting_Judgement; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
                DeleteJudgement();
            }
            else if (_Request.IsClick("savesetting"))
            {
                SaveSettings();
            }


        }

        private JudgementCollection m_Judgements = null;
        protected JudgementCollection JudgementList
        {
            get
            {
                return m_Judgements == null ? JudgementSettings.Judgements : m_Judgements;
            }
        }

        private void DeleteJudgement()
        {
            int[] judIds = StringUtil.Split<int>(_Request.Get("deletejudgementid", Method.Post));

            if (ValidateUtil.HasItems<int>(judIds))
            {
                JudgementSettings.DeleteJudgements(judIds);
                SettingManager.SaveSettings(JudgementSettings);
            }
        }

        public bool SaveSettings()
        {
            int rowIndex = 0;
            MessageDisplay msgDisplay = CreateMessageDisplay("logourl", "description");

            int[] oldJudgement = StringUtil.Split<int>(_Request.Get("judgementids", Method.Post));
            Judgement temp;
            JudgementCollection judgements = new JudgementCollection();

            foreach (int j in oldJudgement)
            {
                temp = new Judgement();
                temp.ID = j;
                temp.Description = _Request.Get("description." + j, Method.Post);
                temp.LogoUrlSrc = _Request.Get("logourl." + j, Method.Post);
                temp.IsNew = _Request.Get<bool>("isnew." + j, Method.Post, false);
                ValidateJudgment(temp, msgDisplay, rowIndex);
                rowIndex++;
                judgements.Add(temp);
            }



            //客户端无脚本  
            if (_Request.Get("newjudgements", Method.Post) != null
                && _Request.Get("newjudgements", Method.Post).Contains("{0}"))
            {
                temp = JudgementSettings.CreateJudgemnet();
                temp.LogoUrlSrc = _Request.Get("logourl.new.{0}", Method.Post);
                temp.Description = _Request.Get("description.new.{0}", Method.Post);

                if (string.IsNullOrEmpty(temp.Description) && string.IsNullOrEmpty(temp.LogoUrl)) judgements.Add(temp);
            }
            else
            {
                oldJudgement = StringUtil.Split<int>(_Request.Get("newjudgements", Method.Post));
                foreach (int j in oldJudgement)
                {
                    temp = JudgementSettings.CreateJudgemnet();
                    temp.LogoUrlSrc = _Request.Get("logourl.new." + j, Method.Post);
                    temp.Description = _Request.Get("description.new." + j, Method.Post);
                    ValidateJudgment(temp, msgDisplay, rowIndex);
                    rowIndex++;
                    judgements.Add(temp);
                }
            }


            if (!msgDisplay.HasAnyError())
            {
                JudgementSettings.Judgements = judgements;
                foreach (Judgement j in judgements) { j.IsNew = false; }
                SettingManager.SaveSettings(JudgementSettings);
            }
            else
            {
                this.m_Judgements = judgements;
                msgDisplay.AddError(new DataNoSaveError());
            }

            return true;
        }

        private void ValidateJudgment(Judgement jud, MessageDisplay msgDisp, int rowindex)
        {

            if (string.IsNullOrEmpty(jud.Description))
            {
                msgDisp.AddError(new CustomError("description", rowindex, Lang_Error.Judgement_EmptyDescriptionError));
            }

            if (string.IsNullOrEmpty(jud.LogoUrl))
            {
                msgDisp.AddError(new CustomError("logourl", rowindex, Lang_Error.Judgement_EmptyLogoUrlError));
            }
        }
    }
}