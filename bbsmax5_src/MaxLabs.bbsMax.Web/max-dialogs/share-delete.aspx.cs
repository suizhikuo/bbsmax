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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class share_delete : DialogPageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("delete"))
            {
				shareID = _Request.Get<int>("shareID", Method.Get, 0);

				if (shareID.HasValue == false)
					ShowError("缺少必要参数");

				Share share = ShareBO.Instance.GetShareForDelete(MyUserID, shareID.Value);

				if (share == null)
					ShowError("您要删除的分享不存在");

                DeleteShare();
            }
        }

        public string Name
        {
            get { return _Request.Get("isfav") == "1" ? "收藏" : "分享"; }
        }

		int? shareID = null;

        private void DeleteShare()
        {

            MessageDisplay msgDisplay = CreateMessageDisplay();
            
            if (shareID < 1)
            {
                msgDisplay.AddError(new InvalidParamError("shareID").Message);
                return;
            }
            try
            {
                using (new ErrorScope())
                {

                    bool success = false;

                    if (_Request.Get("isfav") == "1")
                        success = FavoriteBO.Instance.DeleteShare(MyUserID, shareID.Value, true);
                    else
                        success = ShareBO.Instance.DeleteShare(MyUserID, shareID.Value, true);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                        Return(shareID);
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

    }
}