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
    public partial class delete_selfpost : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Post == null)
                ShowError("帖子不存在");
            else if (Post.UserID != My.UserID)
                ShowError("非法操作");

            if (_Request.IsClick("ok"))
                DeleteSelfPost();
        }

        private void DeleteSelfPost()
        {
            bool success = false;
            try
            {
                success = PostBOV5.Instance.DeletePosts(My, new int[] { PostID }, false, true, false, "用户自己删除");
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

        private PostV5 m_post = null;
        private PostV5 Post
        {
            get
            {
                if (m_post == null)
                {
                    m_post = PostBOV5.Instance.GetPost(PostID, false);
                }

                return m_post;
            }
        }

        private int PostID
        {
            get
            {
                return _Request.Get<int>("PostId", Method.Get, 0);
            }
        }
    }
}