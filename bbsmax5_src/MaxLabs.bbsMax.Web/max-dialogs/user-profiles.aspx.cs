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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_profiles : DialogPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }


        protected bool IsShow(int privacyType)
        {
            return IsShow(privacyType, User);
        }

        private User m_User;
        protected new User User
        {
            get 
            {
                if (m_User == null)
                {
                    int userID = _Request.Get<int>("uid", Method.Get, 0);

                    if (userID < 1)
                        ShowError(new InvalidParamError("uid"));

                    m_User = UserBO.Instance.GetUser(userID);

                    if (m_User == null)
                        ShowError("您要查看的用户不存在");
                }
                return m_User;
            }
        }

        private int? m_IpPart;
        protected int IpPart
        {
            get
            {
                if (m_IpPart == null)
                {
                    if (AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Setting_AccessLimit))
                        m_IpPart = int.MaxValue;
                    else
                        m_IpPart = AllSettings.Current.SiteSettings.ViewIPFields.GetValue(My);
                }
                return m_IpPart.Value;
            }
        }

        protected bool IsShowAddress
        {
            get
            {
                return IpPart >= 4;
            }
        }

        protected string GetIpAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return "无效的IP地址";
            }
            if (ip.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                return IPUtil.GetIpArea(ip.Replace('*', '1'));
            else
                return IPUtil.GetIpArea(ip);
        }
    }
}