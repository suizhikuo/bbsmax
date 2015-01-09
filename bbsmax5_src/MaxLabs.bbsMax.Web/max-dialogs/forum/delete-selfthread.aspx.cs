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
    public partial class delete_selfthread : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Thread == null)
                ShowError("您要删除的主题不存在或已被删除");
            else if (Thread.PostUserID != MyUserID)
                ShowError("非法操作");

            if (_Request.IsClick("ok"))
                DeleteSelfThread();
        }

        private void DeleteSelfThread()
        {
            bool success = false;
            try
            {
                success = PostBOV5.Instance.DeleteThreads(My, new int[] { ThreadID }, false, true, false, true, "用户自己删除");
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
                ShowSuccess("删除成功", true);
        }

        private BasicThread m_thread = null;
        private BasicThread Thread
        {
            get
            {
                if (m_thread == null)
                {
                    m_thread = PostBOV5.Instance.GetThread(ThreadID);
                }

                return m_thread;
            }
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