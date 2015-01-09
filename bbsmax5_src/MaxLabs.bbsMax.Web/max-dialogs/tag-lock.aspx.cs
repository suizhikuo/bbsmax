//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Web;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax;
using MaxLabs.WebEngine;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class tag_lock : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("locktag"))
            {
                LockTag();
            }
        }
        

        private void LockTag()
        {
            using (new ErrorScope())
            {
				MessageDisplay msgDisplay = CreateMessageDisplay();

                
                try
                {
                    int? tagID = _Request.Get<int>("tagid", Method.All);
                    if (tagID == null)
                    {
                        msgDisplay.AddError(new InvalidParamError("tagID").Message);
                        return;
                    }

                    bool success = TagBO.Instance.LockTag(tagID.Value);

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        }
                        );
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddError(ex.Message);
                }
            }
        }
    }
}