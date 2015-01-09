//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.ValidateCodes;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using System.Text;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class post_cancelrate : DialogPageBase
    {
        private const int pageSize = 10;

        protected PostMarkCollection PostMarkList;
        protected int TotalCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Post == null)
            {
                ShowError(new InvalidParamError("postid").Message);
            }

            if (false == AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(post.ForumID).HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.CancelRate))
            {
                ShowError("您所在的用户组没有撤消评分的权限!");
            }

            if (_Request.IsClick("cancelrate"))
            {
                CancelRate();
            }

            int postID = _Request.Get<int>("postid", Method.Get, 0);
            PostMarkList = PostBOV5.Instance.GetPostMarks(postID, PageNumber, pageSize, out TotalCount);

            SetPager("list", null, PageNumber, pageSize, TotalCount);
        }

        private void CancelRate()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();

            if (Post == null)
            {
                msgDisplay.AddError(new InvalidParamError("postid").Message);
                return;
            }

            int[] postMarkIDs = _Request.GetList<int>("postMarkIDs",Method.Post,new int[0]);

            string reason = _Request.Get("reason",Method.Post);

            bool sendMessage = _Request.Get("cbsendMessage", Method.Post, "0") == "1";


            try
            {
                if (!PostBOV5.Instance.CancelRates(My, Post, postMarkIDs, reason, sendMessage, _Request.IpAddress))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    Return(true);
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }


        private PostV5 post;
        protected PostV5 Post
        {
            get
            {
                if (post == null)
                {
                    int postID = _Request.Get<int>("postid", Method.Get, 0);
                    if (postID > 0)
                        post = PostBOV5.Instance.GetPost(postID, false);
                }
                return post;
            }
        }

        private int? m_PageNumber;
        protected int PageNumber
        {
            get
            {
                if (m_PageNumber == null)
                    m_PageNumber = _Request.Get<int>("page", Method.Get, 1);
                return m_PageNumber.Value;
            }
        }



        private UserCollection users = null;
        protected User GetUser(int userID)
        {
            if (users == null)
            {
                List<int> userIDs = new List<int>();

                foreach (PostMark postMark in PostMarkList)
                {
                    if (!userIDs.Contains(postMark.UserID))
                        userIDs.Add(postMark.UserID);
                }

                users = UserBO.Instance.GetUsers(userIDs, GetUserOption.WithAll);
            }

            return users.GetValue(userID);

        }

    }
}