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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;

namespace MaxLabs.bbsMax.Web
{
    public abstract partial class AddCommentPageBase : BbsPageBase//RequiredLoginPageBase
    {
        /// <summary>
        /// 添加评论按扭的名称
        /// </summary>
        protected abstract string ButtonName
        {
            get;
        }

        protected abstract string ValidateCodeActionType { get; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick(ButtonName))
                Add();
        }
        protected override bool EnableClientBuffer
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 添加评论
        /// 注:对留言进行评论时会引用原留言并显示到对方留言板！
        /// </summary>
        private void Add()
        {
            //TODO
            //MessageDisplay msgDisplay = CreateMessageDisplay("content", GetValidateCodeInputName(ValidateCodeActionType));

            //int targetID    = _Request.Get<int>("targetid", Method.All, 0);
            //string content  = _Request.Get("Content", Method.Post,"",false);
            //CommentType commentType = _Request.Get<CommentType>("type", Method.All, CommentType.All);
            //int commentID   = _Request.Get<int>("commentid", Method.Get, 0);
            //string createIP = IPUtil.GetCurrentIP();
            //int userID      = MyUserID;

            //content = UbbUtil.UbbToHtml(content, ParserType.Simple);

            //if (!CheckValidateCode(ValidateCodeActionType, msgDisplay))
            //{
            //    return;
            //}

            //if (commentID != 0)
            //{
            //    Comment comment = CommentBO.Instance.GetCommentByID(commentID);
            //    if (commentType == CommentType.Board)
            //        targetID = comment.UserID;
            //    content = string.Format("<div class=\"quote\"><span class=\"q\"><b>{0}</b>: {1}</span></div>", UserBO.Instance.GetUser(userID).Username, comment.Content) + content;
            //}

            //try
            //{
            //    using (new ErrorScope())
            //    {
            //        int newCommentId;
            //        Success = CommentBO.Instance.AddComment(userID, targetID, commentID, commentType, content, createIP, out newCommentId);
            //        if (Success == false)
            //        {
            //            CatchError<ErrorInfo>(delegate(ErrorInfo error)
            //            {
            //                msgDisplay.AddError(error);
            //            });
            //        }
            //        else
            //        {
            //            MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateValidateCodeActionRecode(ValidateCodeActionType);
            //            if (!IsDialog) msgDisplay.ShowInfo(this);
            //            _jsonComment = PackingComment(targetID,newCommentId , content);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    msgDisplay.AddError(ex.Message);
            //}
        }

        private string _jsonComment="{}";
        protected string JsonComment
        {
            get
            {
                return _jsonComment;
            }
        }

        private string PackingComment(int parentId,int commentId, string  content)
        {
            StringBuffer buffer = new StringBuffer();
            buffer += "{";
            buffer += string.Format( "\"parentId\":{0}", parentId);
            buffer += ",";
            buffer += string.Format("\"id\":{0}", commentId);
            buffer += ",";
            buffer += string.Format("\"date\":\"{0}\"", DateTimeUtil.GetFriendlyDate(DateTimeUtil.Now));
            buffer += ",";
            buffer += string.Format("\"userId\":{0}", My.UserID);
            buffer += ",";
            buffer += string.Format("\"username\":\"{0}\"",StringUtil.ToJavaScriptString(My.Realname));
            buffer += ",";
            buffer +=string.Format("\"content\":\"{0}\"",StringUtil.ToJavaScriptString(content));
            buffer += "}";
            return buffer.ToString();
        }
    }
}