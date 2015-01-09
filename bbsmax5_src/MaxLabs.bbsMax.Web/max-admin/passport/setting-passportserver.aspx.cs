//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MaxLabs.bbsMax.Settings;
using MaxLabs.Passport.InstructEngine;

namespace MaxLabs.bbsMax.Web.max_admin
{
    public partial class setting_passportserver : AdminPageBase
    {
        protected bool Restarting = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!My.IsOwner)
            {
                ShowError("您没有权限管理passport客户端，此功能只有创始人帐号可用！");
                return;
            }

            if (_Request.IsClick("savesetting"))
            {
                bool passportService = PassportServerSettings.EnablePassportService;
                SaveSetting<PassportServerSettings>("savesetting");

                if (passportService != PassportServerSettings.EnablePassportService)
                {
                    HttpRuntime.UnloadAppDomain();
                    Restarting = true;
                }
            }
        }

        private InstructDriver[] m_DriverList;
        protected InstructDriver[] DriverList
        {
            get
            {
                if (m_DriverList == null)
                {
                    if (PassportServerSettings.EnablePassportService && Restarting == false)
                    {
                        InstructEngine engine = PassportBO.InstructEngine as InstructEngine;
                        m_DriverList = new InstructDriver[engine.DriverList.Count];
                        engine.DriverList.Values.CopyTo(m_DriverList, 0);
                    }
                    else
                    {
                        m_DriverList = new InstructDriver[0];
                    }
                }
                return m_DriverList;
            }
        }

        protected bool EnableService
        {
            get
            {
                return PassportServerSettings.EnablePassportService;
            }
        }
    }
}