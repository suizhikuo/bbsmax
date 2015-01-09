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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class feed_list : SpacePageBase
    {
		protected override void OnLoadComplete(EventArgs e)
		{
			m_VisitorIsAdmin = FeedBO.Instance.ManagePermission.Can(My, BackendPermissions.Action.Manage_Feed_Data);

            base.OnLoadComplete(e); 
            
            if (_Request.IsClick("addfeedcomment"))
            {
                CreateComment();
            }
		}

		private bool m_VisitorIsAdmin;

		protected override bool VisitorIsAdmin
		{
			get { return m_VisitorIsAdmin; }
		}

        public override string SpaceTitle
        {
            get
            {
                return SpaceOwner.Name + "的动态";
            }
        }

		protected override SpacePrivacyType FunctionPrivacy
		{
			get { return SpaceOwner.FeedPrivacy; }
		}

		public override bool IsSelectedFeed
		{
			get
			{
				return true;
			}
		}

        private const int DefaultGetCount = 50;
        protected int CurrentGetCount
        {
            get
            {
                return _Request.Get<int>("getcount", Method.Get, DefaultGetCount);
            }
        }

        protected int NextGetCount
        {
            get
            {
                return CurrentGetCount + DefaultGetCount;
            }
        }

        private FeedCollection m_FeedList; 
        protected FeedCollection FeedList
        {
            get
            {
                if (m_FeedList == null)
                {
                    GetFeeds();
                }
                return m_FeedList;
            }
        }

        private bool? m_HaveMoreFeeds;
        public bool HaveMoreFeeds
        {
            get
            {
                if (m_HaveMoreFeeds == null)
                {
                    GetFeeds();
                }
                return m_HaveMoreFeeds.Value;
            }
        }

        private void GetFeeds()
        {

            bool haveMore;
            m_FeedList = FeedBO.Instance.GetUserFeeds(MyUserID, UserID, int.MaxValue, CurrentGetCount, FeedAppID, FeedAction, out haveMore);
            m_HaveMoreFeeds = haveMore;
        }

        private Guid? m_FeedAppID;
        protected Guid FeedAppID
        {
            get
            {
                if (m_FeedAppID == null)
                {
                    m_FeedAppID = _Request.Get<Guid>("appid", Method.Get, Guid.Empty);
                }

                return m_FeedAppID.Value;
            }
        }

        private int? m_FeedAction = int.MinValue;
        protected int? FeedAction
        {
            get
            {

                if (m_FeedAction != null && m_FeedAction.Value == int.MinValue)
                {
                    if (FeedAppID == Guid.Empty)
                        m_FeedAction = null;
                    else
                    {
                        m_FeedAction = _Request.Get<int>("actionType", Method.Get);
                    }
                }
                return m_FeedAction;
            }
        }

        protected int UserID
        {
            get
            {
                return _Request.Get<int>("uid", Method.Get, 0);
            }
        }

        private bool? m_CanDelete;
        protected bool CanDelete
        {
            get
            {
                if (m_CanDelete == null)
                {
                    if (SpaceOwnerID == MyUserID)
                        m_CanDelete = true;
                    else
                        m_CanDelete = FeedBO.Instance.ManagePermission.Can(My, BackendPermissions.Action.Manage_Feed_Data);
                }
                return m_CanDelete.Value;
            }
        }

        protected override bool EnableFunction
        {
            get { return true; }
        }


        private bool? m_FeedCanDisplay;
        protected bool FeedCanDisplay
        {
            get
            {
                if (m_FeedCanDisplay == null)
                    m_FeedCanDisplay = CheckFunctionCanDisplay(SpaceOwner.FeedPrivacy);

                return m_FeedCanDisplay.Value;
            }
        }

        protected bool CanDeleteFeed
        {
            get
            {
                return CanDelete;
            }
        }

        protected override string PageTitle
        {
            get
            {
                return string.Concat(SpaceOwner.Name, "的动态 - ", base.PageTitle);
            }
        }


        #region  动态里的评论

        private int? m_CommentFeedID;
        protected int CommentFeedID
        {
            get
            {
                if (m_CommentFeedID == null)
                {
                    m_CommentFeedID = _Request.Get<int>("commentfeedid", Method.Get, 0);
                }
                return m_CommentFeedID.Value;
            }
        }

        protected int DefaultGetCommentCount = 20;

        private int? m_GetCommentCount;
        protected int GetCommentCount
        {
            get
            {
                if (m_GetCommentCount == null)
                {
                    m_GetCommentCount = _Request.Get<int>("getcommentcount", Method.Get, 0) + DefaultGetCommentCount;
                }

                return m_GetCommentCount.Value;
            }
        }

        private Dictionary<CommentType, Dictionary<int, CommentCollection>> comments;
        private Dictionary<int, CommentCollection> feedComments = new Dictionary<int, CommentCollection>();
        protected CommentCollection GetComments(Feed feed)
        {
            CommentCollection result;
            if (feedComments.TryGetValue(feed.ID, out result))
                return result;

            CommentType type = CommentBO.Instance.GetCommentType(feed.AppID, feed.ActionType);
            if (comments == null)
            {
                List<int> commentIDs = new List<int>();
                foreach (Feed tempFeed in FeedList)
                {
                    if (tempFeed.CommentIDs.Count == 0)
                        continue;

                    commentIDs.AddRange(tempFeed.CommentIDs);
                }

                CommentCollection temp = CommentBO.Instance.GetComments(commentIDs);

                CommentBO.Instance.ProcessKeyword(temp, ProcessKeywordMode.TryUpdateKeyword);

                WaitForFillSimpleUsers<Comment>(temp);

                comments = new Dictionary<CommentType, Dictionary<int, CommentCollection>>();

                foreach (Comment comment in temp)
                {
                    if (comment.IsApproved == false)
                        continue;

                    Dictionary<int, CommentCollection> tempComments;

                    if (comments.TryGetValue(comment.Type, out tempComments) == false)
                    {
                        tempComments = new Dictionary<int, CommentCollection>();
                        CommentCollection tempItems = new CommentCollection();
                        tempItems.Add(comment);
                        tempComments.Add(comment.TargetID, tempItems);
                        comments.Add(comment.Type, tempComments);
                    }
                    else
                    {
                        CommentCollection tempItems;
                        if (tempComments.TryGetValue(comment.TargetID, out tempItems) == false)
                        {
                            tempItems = new CommentCollection();
                            tempItems.Add(comment);
                            tempComments.Add(comment.TargetID, tempItems);
                        }
                        else
                        {
                            tempItems.Add(comment);
                        }
                    }
                }

            }

            if (feed.ID == CommentFeedID && GetCommentCount > DefaultGetCommentCount)
            {
                int count = GetCommentCount - DefaultGetCommentCount;
                result = CommentBO.Instance.GetComments(feed.CommentTargetID, type, count, feed.CommentCount <= count);
                WaitForFillSimpleUsers<Comment>(result);
                CommentBO.Instance.ProcessKeyword(result, ProcessKeywordMode.TryUpdateKeyword);
            }
            else
            {
                Dictionary<int, CommentCollection> resultDic;

                if (comments.TryGetValue(type, out resultDic))
                {
                    if (resultDic.TryGetValue(feed.CommentTargetID, out result) == false)
                        result = new CommentCollection();
                }
                else
                    result = new CommentCollection();
            }

            feedComments.Add(feed.ID, result);

            SubmitFillUsers();

            return result;
        }

        private void CreateComment()
        {
            m_CommentFeedID = _Request.Get<int>("commentfeedid", Method.Post, 0);
            m_GetCommentCount = _Request.Get<int>("getcommentcount", Method.Post, 0);

            string validateCodeActionType = "CreateComment";
            int feedID = _Request.Get<int>("feedID", Method.Post, 0);

            MessageDisplay msgDisplay = CreateMessageDisplay("content", ValidateCodeManager.GetValidateCodeInputName(validateCodeActionType, feedID.ToString()));

            if (CheckValidateCode(validateCodeActionType, feedID.ToString(), msgDisplay))
            {
                string content = _Request.Get("comment_content", Method.Post, string.Empty);

                int targetID = _Request.Get<int>("targetID", Method.Post, 0);
                int actionType = _Request.Get<int>("actiontype", Method.Post, 0);
                Guid appID = new Guid(_Request.Get("appid", Method.Post, string.Empty));

                CommentType commentType = CommentBO.Instance.GetCommentType(appID, actionType);
                int replyTargetUserID = _Request.Get<int>("replyuserid", Method.Post, 0);
                int replycommentid = _Request.Get<int>("replycommentid", Method.Post, 0);
                bool isReply = replyTargetUserID > 0;

                int tempCommentID;
                string tempContent;

                bool success;
                try
                {
                    success = CommentBO.Instance.AddComment(My, targetID, replycommentid, commentType, content, _Request.IpAddress, isReply, replyTargetUserID, out tempCommentID, out tempContent);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                    success = false;
                }

                if (success == false)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        if (error is UnapprovedError)
                        {
                            _Request.Clear();
                            AlertWarning(error.Message);
                        }
                        else
                        {
                            msgDisplay.AddError(error);
                            AlertError(error.Message);
                        }
                    });
                }
                else
                    ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeActionType);
            }
            else
            {
                AlertError("验证码错误！");
            }

        }


        protected bool IsShowGetMoreCommentLink(Feed feed)
        {
            if (feed.CommentCount > 2)
            {
                if (feed.ID != CommentFeedID)
                    return true;
                else if (feed.CommentCount > GetComments(feed).Count)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        #endregion
    }
}