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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class passport_test : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string passportRoot = _Request.Get("passportroot", Method.Post);

            if (string.IsNullOrEmpty(passportRoot))
            {
                ShowError("请填写Passport服务器地址");
                return;
            }

            bool success;
            string errMsg = string.Empty;

            using (ErrorScope es = new ErrorScope())
            {
                PassportClientConfig setting = new PassportClientConfig();

                success = setting.TestPassportService(passportRoot, 5000);

                if (es.HasUnCatchedError)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        ShowError(error);
                        return;
                        //errMsg += error.Message;
                    });
                }
            }

            if (success)
                ShowSuccess("Passport服务器通讯正常！");
            else
                ShowError("无法连接" + passportRoot + "上的Passport服务！");
        }
    }
}