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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class comment_edit : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
		{
			commentID = _Request.Get<int>("commentid", Method.Get, 0);

			if (commentID.HasValue == false)
				ShowError("缺少必要参数");

			m_Comment = CommentBO.Instance.GetCommentForEdit(MyUserID, commentID.Value);

            if (m_Comment == null)
				ShowError("你要编辑的评论不存在");

            if (_Request.IsClick("editcomment"))
                Edit();

            //using (ErrorScope es = new ErrorScope())
            //{
            //    m_Comment = CommentBO.Instance.GetCommentByID(_Request.Get<int>("commentid", Method.Get, 0));

            //    if (m_Comment == null)
            //    {
            //        es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
            //            ShowError(error);
            //        });
            //    }
            //}
        }

		public string Title
		{
			get
			{
				if (_Request.Get("type", Method.Get, string.Empty).ToLower().Trim() == "board")
					return "编辑留言";
				else
					return "编辑评论";
			}
		}

		private Comment m_Comment;

		public Comment Comment
		{
			get { return m_Comment; }
		}

		int? commentID = null;

        private void Edit()
        {
            string content = _Request.Get("Content", Method.Post, string.Empty, false);
            CommentType type = _Request.Get<CommentType>("type", Method.Get, CommentType.All);


            string newContent = null;
            MessageDisplay msgDisplay = CreateMessageDisplay();
            bool success = false;
            string warningMessage = null;

            try
            {
                using (ErrorScope es = new ErrorScope())
                {

                    success = CommentBO.Instance.ModifyComment(MyUserID, commentID.Value, content, type, out newContent);

                    if (success == false)
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UnapprovedError)
                            {
                                warningMessage = error.Message;
                                //AlertWarning(error.Message);
                                //ShowWarning(error.Message);
                            }
                            else
                                msgDisplay.AddError(error);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }

            if (success)
                Return("content", newContent);
            else if (warningMessage != null)
            {
                JsonBuilder jb = new JsonBuilder();
                jb.Set("iswarning", true);
                jb.Set("message", warningMessage);
                Return(jb);
            }
        }


        protected override bool EnableClientBuffer
        {
            get
            {
                return false;
            }
        }
    }
}