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
using MaxLabs.bbsMax.Settings;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.ValidateCodes;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class _default : CenterPageBase
    {

        protected override string PageTitle
        {
get { return "互动中心 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "userhome"; }
        }
        protected override string NavigationKey
        {
            get { return "mydefault"; }
        }

        protected bool IsEmptyDoing
        {
            get
            {
                return string.IsNullOrEmpty(My.Doing) || My.Doing.Trim() == string.Empty;
            }
        }

        protected int MaxDoingLength
        {
            get
            {
                return Consts.Doing_Length;
            }
        }

        //所有公告
        private AnnouncementCollection m_Announcements;
        protected AnnouncementCollection Announcements
        {
            get
            {
                if (m_Announcements == null)
                    m_Announcements = AnnouncementBO.CurrentAnnouncements;
                return m_Announcements;
            }
        }


        private const int DefaultGetFeedCount = 20;
        protected void Page_Load(object sender, EventArgs e)
        {


#if !Passport
            if (_Request.IsClick("adddoing"))
                AddDoing();

			m_VisitorList = SpaceBO.Instance.GetSpaceVisitors(MyUserID, 10, 1);

			if (m_VisitorList != null)
			{
				WaitForFillSimpleUsers<Visitor>(m_VisitorList, 0);
			}

            if (_Request.IsClick("addfeedcomment"))
            {
                CreateComment();
            }
#endif
        }

#if !Passport

        private void AddDoing()
        {
            //string validateType = "CreateDoing";
            MessageDisplay msgDisplay = CreateMessageDisplay("content");//, GetValidateCodeInputName(validateType));

            int userID = MyUserID;
            string content = _Request.Get("Content", Method.Post);//TODO:文本过虑
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
                    else
                    {
                        //ValidateCodeManager.CreateValidateCodeActionRecode(validateType);
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

		private VisitorCollection m_VisitorList;

		public VisitorCollection VisitorList
		{
			get { return m_VisitorList; }
		}

        #region  任务

        private bool? m_HaveNewMission;
        protected bool HaveNewMission
        {
            get
            {
                if (m_HaveNewMission == null)
                {
                    if (NewMission == null)
                        m_HaveNewMission = false;
                    else
                        m_HaveNewMission = true;
                }
                return m_HaveNewMission.Value;
            }
        }

        private Mission m_NewMission;
        protected Mission NewMission
        {
            get
            {
                if (m_NewMission == null && m_HaveNewMission == null)
                {
                    m_NewMission = MissionBO.Instance.GetNewUserMission(MyUserID);
                    if (m_NewMission == null)
                        m_HaveNewMission = false;
                    else
                        m_HaveNewMission = true;
                }

                return m_NewMission;
            }
        }

        #endregion

        #region 动态 Feed

        protected string SelectedFeed(FeedType feedType, string className)
        {
            if (feedType == FeedType)
                return className;
            else
                return string.Empty;
        }

        private bool? m_HaveMoreFeeds;
        protected bool HaveMoreFeeds
        {
            get
            {
                if (m_HaveMoreFeeds == null)
                    GetFeeds();
                return m_HaveMoreFeeds.Value;
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

        private void GetFeeds()
        {
            bool haveMore;
            if (FeedType == FeedType.AllUserFeed)
            {
                m_FeedList = FeedBO.Instance.GetAllUserFeeds(MyUserID, int.MaxValue, GetFeedCount, FeedAppID, FeedAction, out haveMore);
            }
            else if (FeedType == FeedType.FriendFeed)
            {
                m_FeedList = FeedBO.Instance.GetFriendFeeds(MyUserID, int.MaxValue, GetFeedCount, FeedAppID, FeedAction, out haveMore);
            }
            else
                m_FeedList = FeedBO.Instance.GetUserFeeds(MyUserID, MyUserID, int.MaxValue, GetFeedCount, FeedAppID, FeedAction, out haveMore);

            m_HaveMoreFeeds = haveMore;
        }

        private bool? m_CanDeleteFeed;
        protected bool CanDeleteFeed
        {
            get
            {
                if (m_CanDeleteFeed == null)
                {
                    if (FeedType == FeedType.MyFeed)
                        m_CanDeleteFeed = true;
                    else
                        m_CanDeleteFeed = false;
                }
                return m_CanDeleteFeed.Value;
            }
        }

        private bool? m_CanDeleteAnyFeed;
        protected bool CanDeleteAnyFeed
        {
            get
            {
                if (m_CanDeleteAnyFeed == null)
                {
                    if (FeedType == FeedType.AllUserFeed)
                        m_CanDeleteAnyFeed = FeedBO.Instance.ManagePermission.Can(My, BackendPermissions.Action.Manage_Feed_Data);
                    else
                        m_CanDeleteAnyFeed = false;
                }
                return m_CanDeleteAnyFeed.Value;
            }
        }

        protected bool CanShieldFeed(Feed feed)
        {
            return FeedType == FeedType.FriendFeed && false == FeedBO.Instance.IsSiteFeed(feed);
        }

        protected int GetShieldUserID(Feed feed)
        {
            if (feed.IsSpecial && FriendBO.Instance.IsFriend(MyUserID, feed.TargetUserID))
                return feed.TargetUserID;
            else
            {
                if (feed.Users.Count > 0)
                    return feed.Users[0].UserID;
                else
                    return 0;
            }
        }


        private int? m_GetFeedCount;
        protected int GetFeedCount
        {
            get
            {
                if (m_GetFeedCount == null)
                {
                    m_GetFeedCount = _Request.Get<int>("feedcount", Method.Get, DefaultGetFeedCount);
                }
                return m_GetFeedCount.Value;
            }
        }

        protected int NextFeedCount
        {
            get
            {
                return GetFeedCount + DefaultGetFeedCount;
            }
        }

        private FeedType? m_FeedType;
        protected FeedType FeedType
        {
            get
            {
                if (m_FeedType == null)
                {
                    m_FeedType = _Request.Get<FeedType>("feedtype", Method.Get, AllSettings.Current.SiteSettings.DefaultFeedType);
                }
                return m_FeedType.Value;
            }
        }

        protected string FormatFeedTitle(Feed feed)
        {
            float timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(My);

            return FeedBO.Instance.FormatFeedTitle(MyUserID, timeDiffrence, FeedType, feed);
        }

        protected string FormatFeedDescription(Feed feed)
        {
            return FeedBO.Instance.FormatFeedDescription(MyUserID, feed);
        }

        protected string GetAppActionIconUrl(Guid appID, int actionType)
        {
            FeedTemplate template = FeedBO.Instance.GetFeedTemplate(appID, actionType);

            if (template != null)
                return template.IconUrl;

            AppBase app = AppManager.GetApp(appID);
            if (app == null)
                return string.Empty;

            AppAction appAction = app.AppActions.GetValue(actionType);
            if (appAction == null)
                return string.Empty;

            return appAction.IconUrl;
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

                CommentBO.Instance.ProcessKeyword(temp,ProcessKeywordMode.TryUpdateKeyword);

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
                int count =  GetCommentCount - DefaultGetCommentCount;
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

        protected string GetSafeJs(string text)
        {
            return StringUtil.ToJavaScriptString(text);
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

#endif

        protected IList<PointInfo> PointList
        {
            get
            {
                return UserBO.Instance.GetUserPointInfos(MyUserID);
            }
        }

        private bool? m_IsShowMedal;
        protected bool IsShowMedal
        {
            get
            {
                if (m_IsShowMedal == null)
                {
                    if (AllSettings.Current.MedalSettings.Medals.Count == 0)
                        m_IsShowMedal = false;
                    else
                        m_IsShowMedal = true;
                }

                return m_IsShowMedal.Value;
            }
        }


        private PointExpressionColumCollection m_Colums;
        protected PointExpressionColumCollection Colums
        {
            get
            {
                if (m_Colums == null)
                    m_Colums = UserBO.Instance.GetGeneralPointExpressionColums();
                return m_Colums;
            }
        }
        protected string GetConditionName(string condition)
        {
            if (string.Compare("Point_0", condition, true) == 0)
                return "总积分";

            foreach (PointExpressionColum colum in Colums)
            {
                if (string.Compare(colum.Colum, condition, true) == 0)
                    return colum.Description;
            }

            return condition;
        }

        protected string GetMedals(string urlFormat, string imgFormat, bool isHave)
        {
            StringBuilder imgs = new StringBuilder();

            if (isHave)
            {
                foreach (Medal medal in AllSettings.Current.MedalSettings.Medals)
                {
                    MedalLevel medalLevel = medal.GetMedalLevel(My);
                    if (medalLevel != null)
                    {
                        MedalLevel nextLevel = null;
                        int i = 0;
                        foreach (MedalLevel level in medal.Levels)
                        {
                            if (level.ID == medalLevel.ID)
                            {
                                if (medal.Levels.Count > i + 1)
                                    nextLevel = medal.Levels[i + 1];
                            }
                            i++;
                        }

                        string title;
                        if (medalLevel.Name != string.Empty)
                            title = medal.Name + "(" + medalLevel.Name + ")";
                        else
                            title = medal.Name;

                        if (nextLevel != null)
                        {
                            if (medal.IsCustom)
                            {
                                title = title + "；点亮下一级图标条件：" + nextLevel.Condition;
                            }
                            else
                            {
                                title = title + "；点亮下一级图标条件：" + GetConditionName(medal.Condition) + "达到" + nextLevel.Value;
                            }
                        }


                        UserMedal userMedal = My.UserMedals.GetValue(medal.ID, medalLevel.ID);
                        if (userMedal != null && string.IsNullOrEmpty(userMedal.ShowUrl) == false)
                        {

                            string url = userMedal.ShowUrl;
                            if (string.IsNullOrEmpty(userMedal.UrlTitle) == false)
                                title = userMedal.UrlTitle;

                            imgs.Append(string.Format(urlFormat, url, string.Format(imgFormat, medalLevel.LogoUrl, title)));
                        }
                        else
                        {
                            imgs.Append(string.Format(imgFormat, medalLevel.LogoUrl, title));
                        }
                    }
                }
            }
            else
            {
                foreach (Medal medal in AllSettings.Current.MedalSettings.Medals)
                {
                    if (medal.IsHidden)
                        continue;

                    MedalLevel medalLevel = medal.GetMedalLevel(My);
                    if (medalLevel == null)
                    {
                        if (medal.Levels.Count == 0)
                            continue;
                        medalLevel = medal.Levels[0];

                        string title;

                        if (medalLevel.Name != string.Empty)
                            title = medal.Name + "(" + medalLevel.Name + ")";
                        else
                            title = medal.Name;

                        if (medal.IsCustom)
                        {
                            title = title + "；点亮该图标条件：" + medalLevel.Condition;
                        }
                        else
                        {
                            title = title + "；点亮该图标条件：" + GetConditionName(medal.Condition) + "达到" + medalLevel.Value;
                        }

                        imgs.Append(string.Format(imgFormat, medalLevel.LogoUrl, title));
                    }
                }
            }

            return imgs.ToString();
        }

        public bool IsBound
        {
            get
            {
                return My.MobilePhone != 0;
            }
        }


    }
}