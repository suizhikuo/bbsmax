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
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class comment_delete : DialogPageBase
    {
        private int? commentID = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            commentID = _Request.Get<int>("commentid", Method.Get);

            if (commentID.HasValue == false)
                ShowError("缺少必要参数");

            Comment comment = CommentBO.Instance.GetCommentForDelete(MyUserID, commentID.Value);

            if (comment == null)
                ShowError("您要删除的评论不存在");

            if (_Request.IsClick("delete"))
            {
                Delete();
            }
        }

        public string Title
        {
            get
            {
                if (_Request.Get("board", Method.Get) == "1")
                    return "留言";
                else
                    return "评论";
            }
        }



        private void Delete()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            CommentType type = _Request.Get<CommentType>("type", Method.Get, CommentType.All);

            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = CommentBO.Instance.RemoveComment(MyUserID, commentID.Value);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
                if (success == false)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }


            if (success)
                Return("commentID", commentID);
        }
    }
}