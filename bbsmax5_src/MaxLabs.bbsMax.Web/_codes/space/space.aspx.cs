//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages.space
{
    public partial class space : SpacePageBase
    {
        private const int GetFeedCount = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (VisitorIsOwner)
            {
                if (_Request.Get("op", Method.Get) == "apply")
                {
                    string theme = _Request.Get("theme", Method.Get);

                    if (theme != null)
                    {
                        SpaceBO.Instance.UpdateSpaceTheme(theme);

                        BbsRouter.JumpTo("space/" + SpaceOwnerID);
                    }
                }
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Name, "的空间"));
            }
           

            if (_Request.IsClick("articlepassword"))
            {
                int articleID = _Request.Get<int>("id", Method.Post, 0);

                BlogBO.Instance.SaveBlogArticlePassword(MyUserID, articleID, _Request.Get("password", Method.Post));

                BbsRouter.JumpTo("space/" + SpaceOwnerID + "/blog/article-" + articleID);
            }
            else if (_Request.IsClick("adddoing"))
                AddDoing();
            else if (_Request.IsClick("addcomment"))
                AddComment(null, null, "boardform");
            else if (_Request.IsClick("CreateImpression"))
            {
                MessageDisplay msgDisplay = CreateMessageDisplayForForm("ImpressionForum", "text");
                string text = _Request.Get("Text");

                bool success;
                try
                {
                    success = ImpressionBO.Instance.CreateImpression(My, SpaceOwner, text);
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        m_IsShowImpressionInput = false;
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
            }
            else if (_Request.IsClick("DeleteImpression"))
            {
                int? typeID = _Request.Get<int>("TypeID");

                if (typeID != null)
                    ImpressionBO.Instance.DeleteImpressionTypeForUser(My, typeID.Value);
            }
            else if (_Request.IsClick("addfeedcomment"))
            {
                CreateComment();
            }


            SpaceData spaceData = SpaceBO.Instance.GetSpaceDataForVisit(MyUserID, SpaceOwnerID);

            m_AlbumList = spaceData.AlbumList;
            m_ArticleList = spaceData.ArticleList;
            m_CommentList = spaceData.CommentList;
            m_DoingList = spaceData.DoingList;
            m_VisitorList = spaceData.VisitorList;
            m_FriendList = spaceData.FriendList;
            m_ShareList = spaceData.ShareList;
            m_ImpressionList = spaceData.ImpressionList;

            WaitForFillSimpleUsers<Album>(spaceData.AlbumList);
            WaitForFillSimpleUsers<BlogArticle>(spaceData.ArticleList);
            WaitForFillSimpleUsers<Comment>(spaceData.CommentList);
            WaitForFillSimpleUsers<Doing>(spaceData.DoingList);
            WaitForFillSimpleUsers<Visitor>(spaceData.VisitorList, 0);
            WaitForFillSimpleUsers<Friend>(spaceData.FriendList);
            WaitForFillSimpleUsers<Share>(spaceData.ShareList, 0);
            WaitForFillSimpleUsers<Impression>(spaceData.ImpressionList);

            if (IsShowImpressionInput)
            {
                m_ImpressionTypeList = ImpressionBO.Instance.GetImpressionTypesForUse(SpaceOwnerID, 8, 4);
            }

            if (VisitorIsOwner)
            {
                int page = _Request.Get<int>("page", Method.Get, 1);

                m_ImpressionRecordList = ImpressionBO.Instance.GetTargetUserImpressionRecords(SpaceOwnerID, page, 5);

                WaitForFillSimpleUsers<ImpressionRecord>(m_ImpressionRecordList, 0);
            }

            #region 访问者源判断

            string source = _Request.Get("source", Method.Get);

            switch (source)
            {
                case "show":                //竞价排名
                    PointShowBO.Instance.CheckPointShow(My, SpaceOwnerID, IPUtil.GetCurrentIP());
                    break;
            }

            #endregion
        }

        private bool? m_IsShowImpressionInput;
        protected bool IsShowImpressionInput
        {
            get
            {
                if (m_IsShowImpressionInput == null)
                {
                    m_IsShowImpressionInput = _Request.Get("imp", Method.Get, "0") == "1";
                }

                return m_IsShowImpressionInput.Value;
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            m_AdminOnly = SpaceDisplayAdminNote;
            base.OnLoadComplete(e);
        }

        protected override bool VisitorIsAdmin
        {
            get
            {
                return VisitorIsAlbumAdmin || VisitorIsBlogAdmin || VisitorIsCommentAdmin || VisitorIsDoingAdmin || VisitorIsFeedAdmin || VisitorIsFriend || VisitorIsImpressionAdmin || VisitorIsShareAdmin;
            }
        }

        private bool m_AdminOnly;

        protected bool AdminOnly
        {
            get { return m_AdminOnly; }
        }

		

        private ImpressionRecord m_LastImpressionRecord;

        public string LastImpression
        {
            get
            {
                if (m_LastImpressionRecord != null)
                    return m_LastImpressionRecord.Text;

                return string.Empty;
            }
        }

        public string NextImpressionTime
        {
            get
            {
                if (m_LastImpressionRecord != null)
                {
                    DateTime nextTime = m_LastImpressionRecord.CreateDate.AddHours(AllSettings.Current.ImpressionSettings.TimeLimit);

                    TimeSpan span = nextTime - DateTimeUtil.Now;

                    int hours = (int)Math.Round(span.TotalHours);

                    if (hours >= 24)
                    {
                        int days = (int)Math.Round(span.TotalDays);

                        if (days == 1)
                            return "明天";
                        else if (days == 2)
                            return "后天";
                        else if (days == 3)
                            return "大后天";
                        else if (days == 7)
                            return "一星期";

                        return days + "天";
                    }
                    else
                        return hours + "小时";
                }

                return string.Empty;
            }
        }

        private bool? m_CanImpression;

        public bool CanImpression
        {
            get
            {
                if (m_CanImpression == null)
                {
                    m_LastImpressionRecord = ImpressionBO.Instance.GetLastImpressionRecord(My.UserID, SpaceOwnerID);

                    if (m_LastImpressionRecord == null)
                        m_CanImpression = true;
                    else
                    {
                        m_CanImpression = m_LastImpressionRecord.CreateDate.AddHours(AllSettings.Current.ImpressionSettings.TimeLimit) <= DateTimeUtil.Now;
                    }
                }

                return m_CanImpression.Value;
            }
        }

        private ImpressionTypeCollection m_ImpressionTypeList;

        protected ImpressionTypeCollection ImpressionTypeList
        {
            get { return m_ImpressionTypeList; }
        }

        private ImpressionRecordCollection m_ImpressionRecordList;

        protected ImpressionRecordCollection ImpressionRecordList
        {
            get { return m_ImpressionRecordList; }
        }

        protected bool UserInfoCanDisplay
        {
            get
            {
                return CheckFunctionCanDisplay(SpaceOwner.InformationPrivacy);
            }
        }

		protected bool DoingCanDisplay
		{
			get 
			{
				return CheckFunctionCanDisplay(SpaceOwner.DoingPrivacy);
			}
		}

		protected bool ShareCanDisplay
		{
			get
			{
				return CheckFunctionCanDisplay(SpaceOwner.SharePrivacy);
			}
		}

		protected bool AlbumCanDisplay
		{
			get
			{
				return CheckFunctionCanDisplay(SpaceOwner.AlbumPrivacy);
			}
		}

		protected bool FeedCanDisplay
		{
			get
			{
				return CheckFunctionCanDisplay(SpaceOwner.FeedPrivacy);
			}
		}

		protected bool BlogCanDisplay
		{
			get
			{
				return CheckFunctionCanDisplay(SpaceOwner.BlogPrivacy);
			}
		}

		

		protected bool FriendCanDisplay
		{
			get
			{
				return CheckFunctionCanDisplay(SpaceOwner.FriendListPrivacy);
			}
		}

		protected override SpacePrivacyType FunctionPrivacy
		{
			get { return SpacePrivacyType.All; }
		}

		protected IList<PointInfo> PointList
		{
			get
			{
				return UserBO.Instance.GetUserPointInfos(SpaceOwnerID);
			}
		}

		public override bool IsSelectedSpace
		{
			get
			{
				return true;
			}
		}

		protected override CommentType CommentType
		{
			get
			{
				return CommentType.Board;
			}
		}

		public override int CommentTargetID
		{
			get
			{
				return SpaceOwnerID;
			}
		}

		private BlogArticleCollection m_ArticleList;

		public BlogArticleCollection ArticleList
		{
			get { return m_ArticleList; }
		}

		private CommentCollection m_CommentList;

		public CommentCollection CommentList
		{
			get { return m_CommentList; }
		}

		private DoingCollection m_DoingList;

		public DoingCollection DoingList
		{
			get { return m_DoingList; }
		}

		private AlbumCollection m_AlbumList;

		public AlbumCollection AlbumList
		{
			get { return m_AlbumList; }
		}

		private VisitorCollection m_VisitorList;

		public VisitorCollection VisitorList
		{
			get { return m_VisitorList; }
		}

		private FriendCollection m_FriendList;

		public FriendCollection FriendList
		{
			get { return m_FriendList; }
		}

		private ShareCollection m_ShareList;

		public ShareCollection ShareList
		{
			get { return m_ShareList; }
		}

        private ImpressionCollection m_ImpressionList;

        public ImpressionCollection ImpressionList
        {
            get { return m_ImpressionList; }
        }

        private void AddDoing()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("doingContent");

            int userID = MyUserID;
            string content = _Request.Get("doingContent", Method.Post);//TOTO:文本过虑
            string createIP = _Request.IpAddress;

            try
            {
                using (new ErrorScope())
                {
                    bool success = DoingBO.Instance.CreateDoing(userID, createIP, content);
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    //else
                    //    msgDisplay.ShowInfo(this);
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }
        }

		protected override bool EnableFunction
		{
			get { return true; }
        }






        #region

        private FeedCollection m_FeedList;
        protected FeedCollection FeedList
        {
            get
            {
                if (m_FeedList == null)
                {
                    bool haveMore;
                    m_FeedList = FeedBO.Instance.GetUserFeeds(MyUserID, SpaceOwnerID, int.MaxValue, GetFeedCount, Guid.Empty, null, out haveMore);
                }
                return m_FeedList;
            }
        }



        private bool? m_CanDeleteFeed;
        protected bool CanDeleteFeed
        {
            get
            {
                if (m_CanDeleteFeed == null)
                {
                    if (SpaceOwnerID == MyUserID)
                        m_CanDeleteFeed = true;
                    else
                        m_CanDeleteFeed = FeedBO.Instance.ManagePermission.Can(My, MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Feed_Data);
                }
                return m_CanDeleteFeed.Value;
            }
        }





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

        private bool? m_VisitorIsImpressionAdmin;

        protected bool VisitorIsImpressionAdmin
        {
            get
            {
                if(m_VisitorIsImpressionAdmin == null)
                    m_VisitorIsImpressionAdmin = VisitorIsOwner || AllSettings.Current.BackendPermissions.Can(My, BackendPermissions.Action.Manage_ImpressionRecord);

                return m_VisitorIsImpressionAdmin.Value;
            }
        }

        private bool? m_VisitorIsBlogAdmin;

        protected bool VisitorIsBlogAdmin
        {
            get
            {
                if(m_VisitorIsBlogAdmin == null)
                    m_VisitorIsBlogAdmin = VisitorIsOwner || BlogBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Blog, SpaceOwnerID);

                return m_VisitorIsBlogAdmin.Value;
            }
        }

        private bool? m_VisitorIsAlbumAdmin;

        protected bool VisitorIsAlbumAdmin
        {
            get
            {
                if(m_VisitorIsAlbumAdmin == null)
                    m_VisitorIsAlbumAdmin = VisitorIsOwner || AlbumBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Album, SpaceOwnerID);

                return m_VisitorIsAlbumAdmin.Value;
            }
        }

        private bool? m_VisitorIsDoingAdmin;

        protected bool VisitorIsDoingAdmin
        {
            get
            {
                if(m_VisitorIsDoingAdmin == null)
                    m_VisitorIsDoingAdmin = VisitorIsOwner || DoingBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Doing, SpaceOwnerID);

                return m_VisitorIsDoingAdmin.Value;
            }
        }

        private bool? m_VisitorIsShareAdmin;

        protected bool VisitorIsShareAdmin
        {
            get
            {
                if(m_VisitorIsShareAdmin == null)
                    m_VisitorIsShareAdmin = VisitorIsOwner || ShareBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Share, SpaceOwnerID);

                return m_VisitorIsShareAdmin.Value;
            }
        }

        private bool? m_VisitorIsFeedAdmin;

        protected bool VisitorIsFeedAdmin
        {
            get
            {
                if(m_VisitorIsFeedAdmin == null)
                    m_VisitorIsFeedAdmin = VisitorIsOwner || FeedBO.Instance.ManagePermission.Can(My, BackendPermissions.Action.Manage_Feed_Data);

                return m_VisitorIsFeedAdmin.Value;
            }
        }

        private bool? m_VisitorIsCommentAdmin;

        protected bool VisitorIsCommentAdmin
        {
            get
            {
                if(m_VisitorIsCommentAdmin == null)
                    m_VisitorIsCommentAdmin = VisitorIsOwner || CommentBO.Instance.ManagePermission.Can(My, BackendPermissions.ActionWithTarget.Manage_Comment, SpaceOwnerID);
 
                return m_VisitorIsCommentAdmin.Value;
            }
        }















//==========================================================================================================

        protected override string PageTitle
        {
            get
            {
                return string.Concat(SpaceOwner.Name, " - ", base.PageTitle);
            }
        }

        protected bool IsShowRealName
        {
            get
            {
                if (string.IsNullOrEmpty(SpaceOwner.Realname))
                    return false;
                else
                    return true;
            }
        }

        private bool? m_IsShowDoingInput;
        protected bool IsShowDoingInput
        {
            get
            {
                if (m_IsShowDoingInput == null)
                {
                    if (EnableDoingFunction &&  VisitorIsOwner)//自己
                        m_IsShowDoingInput = true;
                    else
                        m_IsShowDoingInput = false;
                }

                return m_IsShowDoingInput.Value;
            }
        }

        private bool? m_IsShowDoing;
        protected bool IsShowDoing
        {
            get
            {
                if (m_IsShowDoing == null)
                {
                    if (EnableDoingFunction && string.IsNullOrEmpty(SpaceOwner.Doing) == false && (AdminOnly == false || (AdminOnly && VisitorIsDoingAdmin)))
                        m_IsShowDoing = true;
                    else
                        m_IsShowDoing = false;
                }
                return m_IsShowDoing.Value;
            }
        }

        private bool? m_IsShowAlbums;
        protected bool IsShowAlbums
        {
            get
            {
                if (m_IsShowAlbums == null)
                {
                    if (EnableAlbumFunction && (AdminOnly == false || (AdminOnly && VisitorIsAlbumAdmin)))
                        m_IsShowAlbums = true;
                    else
                        m_IsShowAlbums = false;
                }

                return m_IsShowAlbums.Value;
            }
        }


        private bool? m_IsShowBlog;
        protected bool IsShowBlog
        {
            get
            {
                if (m_IsShowBlog == null)
                {
                    if (EnableBlogFunction && (AdminOnly == false || (AdminOnly && VisitorIsBlogAdmin)))
                        m_IsShowBlog = true;
                    else
                        m_IsShowBlog = false;
                }

                return m_IsShowBlog.Value;
            }
        }


        private bool? m_IsShowImpression;
        protected bool IsShowImpression
        {
            get
            {
                if (m_IsShowImpression == null)
                {
                    if (EnableImpressionFunction && (AdminOnly == false || (AdminOnly && VisitorIsImpressionAdmin)))
                        m_IsShowImpression = true;
                    else
                        m_IsShowImpression = false;
                }

                return m_IsShowImpression.Value;
            }
        }


        protected override bool IsSpacePage
        {
            get
            {
                return true;
            }
        }

	}
}