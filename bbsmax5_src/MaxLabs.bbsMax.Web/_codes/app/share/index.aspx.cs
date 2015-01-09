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
using System.Collections;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Common;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.App_Share
{
    public partial class index : CenterSharePageBase
    {
        protected override string PageName
        {
            get
            {
                if (IsFav)
                    return "favorite";
                else
                    return "share";
            }
        }

        protected override string NavigationKey
        {
            get
            {
                if (IsFav)
                    return "favorite";
                else
                    return "share";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
		{


			m_ShareListPageSize = 8;


			if (_Request.IsClick("agree"))
            {
                int? shareId = _Request.Get<int>("shareid");

                if(shareId != null)
                    ShareBO.Instance.AgreeShare(My, shareId.Value);
            }
            else if (_Request.IsClick("oppose"))
            {
                int? shareId = _Request.Get<int>("shareid");

                if (shareId != null)
                    ShareBO.Instance.OpposeShare(My, shareId.Value);
            }
            else if (_Request.IsClick("addcomment"))
            {
                AddComment();
            }


            if (IsFav)
            {
                m_ShareList = FavoriteBO.Instance.GetUserFavorites(MyUserID, ShareType, PageNumber, m_ShareListPageSize);
                m_ShareTotalCount = m_ShareList.TotalRecords;
            }
            else
            {
                if (IsSpace)
                {
                    m_ShareList = ShareBO.Instance.GetUserShares(MyUserID, AppOwnerUserID, ShareType, PageNumber, m_ShareListPageSize);
                    m_ShareTotalCount = m_ShareList.TotalRecords;
                }
                else
                {

                    if (SelectedMy)
                    {
                        m_ShareList = ShareBO.Instance.GetUserShares(MyUserID, MyUserID, ShareType, PageNumber, m_ShareListPageSize);
                        m_ShareTotalCount = m_ShareList.TotalRecords;
                    }
                    else if (SelectedFriend)
                    {
                        //只有热门分享 才要按天  周  月
                        //string scope = _Request.Get("scope");

                        //switch (scope)
                        //{
                        //    case "day":
                        //        m_ShareList = ShareBO.Instance.GetFriendSharesOrderByRank(MyUserID, m_ShareType, DateTime.Now.Date, m_ShareListPageSize, pageNumber);
                        //        break;

                        //    case "week":
                        //        m_ShareList = ShareBO.Instance.GetFriendSharesOrderByRank(MyUserID, m_ShareType, DateTime.Now.Date.AddDays(-7), m_ShareListPageSize, pageNumber);
                        //        break;

                        //    case "month":
                        //        m_ShareList = ShareBO.Instance.GetFriendSharesOrderByRank(MyUserID, m_ShareType, DateTime.Now.Date.AddDays(30), m_ShareListPageSize, pageNumber);
                        //        break;

                        //    default:
                        m_ShareList = ShareBO.Instance.GetFriendShares(MyUserID, ShareType, PageNumber, m_ShareListPageSize);
                        //        break;
                        //}

                        m_ShareTotalCount = m_ShareList.TotalRecords;
                    }
                    else if (SelectedEveryone)
                    {
                        //只有热门分享 才要按天  周  月
                        //string scope = _Request.Get("scope");

                        //switch (scope)
                        //{
                        //case "day":
                        //    m_ShareList = ShareBO.Instance.GetEveryoneSharesOrderByRank(m_ShareType, DateTime.Now.Date, m_ShareListPageSize, pageNumber);
                        //    break;

                        //case "week":
                        //    m_ShareList = ShareBO.Instance.GetEveryoneSharesOrderByRank(m_ShareType, DateTime.Now.Date.AddDays(-7), m_ShareListPageSize, pageNumber);
                        //    break;

                        //case "month":
                        //    m_ShareList = ShareBO.Instance.GetEveryoneSharesOrderByRank(m_ShareType, DateTime.Now.Date.AddDays(30), m_ShareListPageSize, pageNumber);
                        //    break;

                        //default:
                        m_ShareList = ShareBO.Instance.GetEveryoneShares(ShareType, PageNumber, m_ShareListPageSize);
                        //        break;
                        //}

                        m_ShareTotalCount = m_ShareList.TotalRecords;
                    }
                    else if (SelectedCommented)
                    {
                        m_ShareList = ShareBO.Instance.GetUserCommentedShares(MyUserID, ShareType, PageNumber, ShareListPageSize);

                        m_ShareTotalCount = m_ShareList.TotalRecords;
                    }
                    else
                    {
                        m_ShareList = ShareBO.Instance.GetHotShares(ShareType, HotShareTimeType, PageNumber, ShareListPageSize, out m_ShareTotalCount);
                    }
                }
            }
            if (m_ShareList != null)
            {
                foreach (Share share in m_ShareList)
                {
                    WaitForFillSimpleUsers<Comment>(share.CommentList);
                }

                WaitForFillSimpleUsers<Share>(m_ShareList, 0);


                m_AgreeStates = ShareBO.Instance.GetAgreeStates(My, m_ShareList.GetKeys());

                //if (IsAjaxRequest)
                //{
                //    m_GetCommentShareID = _Request.Get<int>("getcommentshareid", 0);

                //    m_CommentList = ShareBO.Instance.GetShareComments(m_GetCommentShareID);
                //}
            }

            SetPager("pager1", null, PageNumber, m_ShareListPageSize, ShareTotalCount);

            if (IsSpace == false)
            {
                if (IsFav)
                    AddNavigationItem(FunctionName);
                else
                {
                    AddNavigationItem(FunctionName, BbsRouter.GetUrl("app/share/index"));

                    if (SelectedMy)
                        AddNavigationItem("我的" + FunctionName);
                    else if (SelectedFriend)
                        AddNavigationItem("好友的" + FunctionName);
                    else if (SelectedEveryone)
                        AddNavigationItem("最新" + FunctionName);
                    else if (SelectedHot)
                        AddNavigationItem("热门" + FunctionName);
                    else
                        AddNavigationItem("我评论过的" + FunctionName);
                }
            }
            else
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"), UrlHelper.GetSpaceUrl(AppOwner.UserID));
                AddNavigationItem("主人的"+FunctionName);
            }
        }

        protected new bool IsSpace
        {
            get
            {
                if (IsFav)
                    return false;
                else
                    return base.IsSpace;
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

        //private void AddComment()
        //{
        //    MessageDisplay msgDisplay = CreateMessageDisplay(GetValidateCodeInputName("CreateComment"));

        //    int targetID = _Request.Get<int>("shareid", Method.All, 0);
        //    string content = _Request.Get("content", Method.Post);
        //    string createIP = _Request.IpAddress;
        //    int userID = MyUserID;

        //    if (!CheckValidateCode("CreateComment", msgDisplay))
        //    {
        //        return;
        //    }
        //    try
        //    {
        //        using (new ErrorScope())
        //        {
        //            int newCommentId;
        //            bool success = CommentBO.Instance.AddComment(userID, targetID, 0, CommentType.Share, content, createIP, out newCommentId);
        //            if (success == false)
        //            {
        //                CatchError<ErrorInfo>(delegate(ErrorInfo error)
        //                {
        //                    if (error is UnapprovedError)
        //                        AlertWarning(error.Message);
        //                    else
        //                        msgDisplay.AddError(error.Message);
        //                });
        //            }
        //            else
        //            {
        //                MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");
        //            }
        //            //else
        //            //    msgDisplay.ShowInfo(this);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        msgDisplay.AddError(ex.Message);
        //    }
        //}

        //private int m_GetCommentShareID;

        //public int GetCommentShareID
        //{
        //    get { return m_GetCommentShareID; }
        //}

        private CommentCollection m_CommentList = new CommentCollection();

        public CommentCollection CommentList
        {
            get { return m_CommentList; }
        }

        private Hashtable m_AgreeStates;

        public Hashtable AgreeStates
        {
            get { return m_AgreeStates; }
        }

		public string IsSelected(ShareType type, string result)
		{
            if (ShareType == type)
				return result;
			else
				return string.Empty;
		}

		private ShareType? m_ShareType;
        protected ShareType ShareType
        {
            get
            {
                if (m_ShareType == null)
                    m_ShareType = _Request.Get<ShareType>("type", Method.Get, ShareType.All);

                return m_ShareType.Value;
            }
        }

		private ShareCollection m_ShareList;

		public ShareCollection ShareList
		{
			get
			{
				return m_ShareList;
			}
		}

		private int m_ShareListPageSize;

		public int ShareListPageSize
		{
			get
			{
				return m_ShareListPageSize;
			}
		}

		private int m_ShareTotalCount;

		public int ShareTotalCount
		{
			get
			{
				return m_ShareTotalCount;
			}
		}

		private string m_View;
        protected string View
        {
            get
            {
                if (m_View == null)
                {
                    m_View = _Request.Get("view", Method.Get, "");
                }

                return m_View;
            }
        }

        private string m_Scope;
        protected string Scope
        {
            get
            {
                if (m_Scope == null)
                {
                    m_Scope = _Request.Get("scope", Method.Get, "0");
                }

                return m_Scope;
            }
        }

        private HotShareTimeType? m_HotShareTimeType;
        protected HotShareTimeType HotShareTimeType
        {
            get
            {
                if (m_HotShareTimeType == null)
                    m_HotShareTimeType = (HotShareTimeType)StringUtil.GetInt(Scope, 0);

                return m_HotShareTimeType.Value;
            }
        }

		/// <summary>
		/// 是否显是显示“我的相册”
		/// </summary>
		public bool SelectedMy
		{
			get
			{
                return SelectedHot == false && SelectedVisited == false
                    && SelectedFriend == false && SelectedEveryone == false
                    && SelectedCommented == false;
			}
		}

		/// <summary>
		/// 是否显是显示“我访问过的相册”
		/// </summary>
		public bool SelectedVisited
		{
			get
			{
                return string.Compare(View, "visited", true) == 0;
			}
		}

		/// <summary>
		/// 是否显是显示“好友的相册”
		/// </summary>
		public bool SelectedFriend
		{
			get
            { return string.Compare(View, "friend", true) == 0; }
		}


		/// <summary>
		/// 是否显是显示“大家的相册”
		/// </summary>
		public bool SelectedEveryone
		{
			get
            { return string.Compare(View, "everyone", true) == 0; }
		}

        public bool SelectedHot
        {
            get
            {
                return string.Compare(View, "hot", true) == 0; 
            }
        }

        public bool SelectedCommented
        {
            get
            {
                return string.Compare(View, "commented", true) == 0;
            }
        }

        private string m_ValidateActionType;
        protected string ValidateActionType
        {
			get
			{
				if (m_ValidateActionType == null)
					m_ValidateActionType = IsFav ? "CreateCollection" : "CreateShare";

				return m_ValidateActionType;
			}
        }




        protected override string PageTitle
        {
            get
            {
                if (IsFav)
                {
                    return string.Concat(FunctionName, " - ", base.PageTitle);
                }
                else
                {
                    if (IsSpace)
                    {
                        return string.Concat(AppOwner.Name, " - ", FunctionName, " - ", base.PageTitle);
                    }
                    else if (SelectedMy)
                        return string.Concat("我的", FunctionName, " - ", base.PageTitle);
                    else if (SelectedCommented)
                        return string.Concat("我评论过的", FunctionName, " - ", base.PageTitle);
                    else if (SelectedEveryone)
                        return string.Concat("最新", FunctionName, " - ", base.PageTitle);
                    else if (SelectedFriend)
                        return string.Concat("好友的", FunctionName, " - ", base.PageTitle);
                    else
                        return string.Concat("热门", FunctionName, " - ", base.PageTitle);
                }
            }
        }





        #region 评论

        private int? m_CommentListTargetID;
        protected int CommentListTargetID
        {
            get
            {
                if (m_CommentListTargetID == null)
                {
                    m_CommentListTargetID = _Request.Get<int>("cdid", Method.Get, 0);
                }

                return m_CommentListTargetID.Value;
            }
        }

        private int? m_CommentListPageNumber;
        protected int CommentListPageNumber
        {
            get
            {
                if (m_CommentListPageNumber == null)
                {
                    m_CommentListPageNumber = _Request.Get<int>("cp", Method.Get, 1);
                }

                return m_CommentListPageNumber.Value;
            }
        }

        private CommentCollection comments;
        private int CommentPageSize = 10;
        protected CommentCollection GetComments(Share share)
        {
            if (CommentListTargetID != share.UserShareID)
                return share.CommentList;

            if (comments == null)
            {
                int count;
                comments = CommentBO.Instance.GetComments(share.UserShareID, CommentType.Share, CommentListPageNumber, CommentPageSize, false, out count);

                FillSimpleUsers<Comment>(comments);

                SetPager("commentlist", null, CommentListPageNumber, CommentPageSize, count);
            }

            return comments;
        }

        protected bool IsShowGetAll(Share share)
        {
            if (share.UserShareID != CommentListTargetID)
            {
                if (share.TotalComments > 2)
                    return true;
                else
                    return false;
            }

            return false;
        }

        protected bool IsShowCommentPager(Share share)
        {
            if (share.UserShareID != CommentListTargetID)
            {
                return false;
            }

            if (share.TotalComments > CommentPageSize)
                return true;
            else
                return false;
        }

        private void AddComment()
        {
            int targetID = _Request.Get<int>("targetid", Method.Post, 0);

            m_CommentListPageNumber = _Request.Get<int>("clpn", Method.Post, 1);
            m_CommentListTargetID = _Request.Get<int>("cld", Method.Post, 0);

            MessageDisplay msgDisplay = CreateMessageDisplayForForm("form_" + targetID, "content", GetValidateCodeInputName("CreateComment", targetID.ToString()));

            if (CheckValidateCode("CreateComment", targetID.ToString(), msgDisplay))
            {
                string content = _Request.Get("content", Method.Post, string.Empty);
                string createIP = _Request.IpAddress;
                int userID = MyUserID;

                int replyUserID = _Request.Get<int>("replyuserid", Method.Post, 0);
                int replyCommentID = _Request.Get<int>("replycommentid", Method.Post, 0);

                bool isReply = replyUserID > 0;

                try
                {
                    using (ErrorScope es = new ErrorScope())
                    {
                        int newCommentId;
                        string newContent;
                        bool success = CommentBO.Instance.AddComment(My, targetID, replyCommentID, CommentType.Share, content, createIP, isReply, replyUserID, out newCommentId, out newContent);
                        if (success == false)
                        {
                            es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                if (error is UnapprovedError)
                                    AlertWarning(error.Message);
                                else
                                {
                                    AlertError("");
                                    msgDisplay.AddError(error);
                                }
                            });
                        }
                        else
                        {
                            Share share = ShareBO.Instance.GetUserShare(targetID);
                            if (share != null)
                            {
                                m_CommentListPageNumber = share.TotalComments / CommentPageSize;
                                if (share.TotalComments % CommentPageSize > 0)
                                    m_CommentListPageNumber += 1;
                            }

                            ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");
                        }
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                    AlertError(ex.Message);
                }
            }
            else
            {
                AlertError("验证码错误");
            }
        }

        #endregion
    }
}