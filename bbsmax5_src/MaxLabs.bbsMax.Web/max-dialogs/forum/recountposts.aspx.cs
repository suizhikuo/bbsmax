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
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs.forum
{
    public partial class recountposts : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (My.IsManager == false)
                ShowError("您所在的用户组没有修复主题回复数的权限！");

            if (ThreadID < 1)
                ShowError("非法的参数“ThreadID”");

            if (_Request.IsClick("ok"))
                Recount();
        }

        private void Recount()
        {
            bool success = false;
            try
            {
                success = PostBOV5.Instance.RepairTotalReplyCount(My, ThreadID);
            }
            catch(Exception ex)
            {
                ShowError(ex.Message);
            }

            if (success == false)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    ShowError(error.Message);
                });
            }
            else
                ShowSuccess("修复主题回复数成功", true);
        }

 

        private int ThreadID
        {
            get
            {
                return _Request.Get<int>("threadid", Method.Get, 0);
            }
        }
    }
}