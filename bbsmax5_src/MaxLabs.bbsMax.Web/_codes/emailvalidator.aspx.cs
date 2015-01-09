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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class emailvalidator : BbsPageBase
    {
        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            string validateCode = _Request.Get("code", Method.Get);
            if (!string.IsNullOrEmpty(validateCode))
            {
                validateCode = HttpUtility.UrlDecode(validateCode);
                if (UserBO.Instance.ResetEmailByValidateCode(validateCode))
                {
                    ShowSuccess("您的邮箱已成功通过验证！");
                    return;
                }
            }

            ShowError("无效的邮箱验证码！");
        }
    }
}