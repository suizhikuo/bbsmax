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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Errors;
using System.Collections.Generic;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.App_Doing
{
    public partial class index : CenterDoingPageBase
    {
        protected override string PageTitle
        {
            get
            {
                if (IsSpace)
                {
                    return string.Concat(AppOwner.Name, " - ", FunctionName, " - ", base.PageTitle);
                }
                else if (SelectedMy)
                {
                    return string.Concat("我的" + FunctionName, " - ", base.PageTitle);
                }
                else if (SelectedFriend)
                {
                    return string.Concat("好友的" + FunctionName, " - ", base.PageTitle);
                }
                else if (SelectedEveryone)
                {
                    return string.Concat("大家的" + FunctionName, " - ", base.PageTitle);
                }
                else
                {
                    return string.Concat("我评论过的" + FunctionName, " - ", base.PageTitle);
                }
            }
        }

        protected override string PageName
        {
            get { return "doing"; }
        }

        protected override string NavigationKey
        {
            get { return "doing"; }
        }


        protected int MaxDoingLength
        {
            get
            {
                return Consts.Doing_Length;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
			if (_Request.IsClick("adddoing"))
				AddDoing();
			else if (_Request.IsClick("addcomment"))
				AddComment();


            m_View = _Request.Get("view", Method.Get, "");

			m_DoingListPageSize = 10;

			using (ErrorScope es = new ErrorScope())
			{
                if (IsSpace)
                {
                    m_DoingList = DoingBO.Instance.GetUserDoingsWithComments(MyUserID, AppOwnerUserID, PageNumber, m_DoingListPageSize);
                }
                else
                {
                    if (SelectedMy)
                    {
                        AddNavigationItem(string.Concat("我的", FunctionName));
                        m_DoingList = DoingBO.Instance.GetUserDoingsWithComments(MyUserID, MyUserID, PageNumber, m_DoingListPageSize);
                    }
                    else if (SelectedFriend)
                    {
                        AddNavigationItem(string.Concat("好友的", FunctionName));
                        m_DoingList = DoingBO.Instance.GetFriendDoingsWithComments(MyUserID, PageNumber, m_DoingListPageSize);
                    }
                    else if (SelectedEveryone)
                    {
                        AddNavigationItem(string.Concat("大家的", FunctionName));
                        m_DoingList = DoingBO.Instance.GetEveryoneDoingsWithComments(PageNumber, m_DoingListPageSize);
                    }
                    else if (SelectedCommented)
                    {
                        AddNavigationItem(string.Concat("我评论的", FunctionName));
                        m_DoingList = DoingBO.Instance.GetUserCommentedDoingsWithComments(MyUserID, PageNumber, m_DoingListPageSize);
                    }

                }

                if (m_DoingList != null)
                {
                    m_TotalDoingCount = m_DoingList.TotalRecords;

                    foreach (Doing doing in m_DoingList)
                    {
                        WaitForFillSimpleUsers<Comment>(doing.CommentList);
                    }

                    WaitForFillSimpleUsers<Doing>(m_DoingList);
                }

				if (es.HasUnCatchedError)
				{
					es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
					{
						ShowError(error);
					});
				}
            }

            SetPager("doinglist", null, PageNumber, DoingListPageSize, DoingList.TotalRecords);

            if (IsSpace)
            {
                AddNavigationItem(string.Concat(AppOwner.Username, "的空间"), UrlHelper.GetSpaceUrl(AppOwner.UserID));
                AddNavigationItem("主人的"+FunctionName);
            }

		}

        private int? m_PageNumber;
        protected int PageNumber
        {
            get
            {
                if(m_PageNumber == null)
                    m_PageNumber = _Request.Get<int>("page", 1);

                return m_PageNumber.Value;
            }
        }

		private DoingCollection m_DoingList;

		public DoingCollection DoingList
		{
			get { return m_DoingList; }
		}

		private int m_DoingListPageSize;

		public int DoingListPageSize
		{
			get { return m_DoingListPageSize; }
		}

		private int m_TotalDoingCount;

		public int TotalDoingCount
		{
			get { return m_TotalDoingCount; }
		}

        private void AddDoing()
        {

            if (My.Roles.IsInRole(Role.FullSiteBannedUsers))
                ShowError("您已经被整站屏蔽不能发表记录");

            MessageDisplay msgDisplay = CreateMessageDisplayForForm("doingform");//, GetValidateCodeInputName("CreateDoing"));

            //if (CheckValidateCode("CreateDoing", msgDisplay))
            //{
				string content = _Request.Get("content", Method.Post, string.Empty);

				try
				{
					using (new ErrorScope())
					{
						bool success = DoingBO.Instance.CreateDoing(MyUserID, _Request.IpAddress, content);

						if (success == false)
						{
							CatchError<ErrorInfo>(delegate(ErrorInfo error)
							{
								msgDisplay.AddError(error);
							});
						}
						else
                        {
                            if (IsSpace)
                            {
                                BbsRouter.JumpTo("app/doing/index", "uid=" + AppOwnerUserID);
                            }
                            else
                                BbsRouter.JumpTo("app/doing/index");

							//ValidateCodeManager.CreateValidateCodeActionRecode("CreateDoing");
						}
					}
				}
				catch (Exception ex)
				{
                    msgDisplay.AddException(ex);
				}
            //}
        }

		private string m_View;

		/// <summary>
		/// 是否显是显示“我的记录”
		/// </summary>
		public bool SelectedMy
		{
			get
            {
                return SelectedFriend == false && SelectedEveryone == false && SelectedCommented == false;
			}
		}

		/// <summary>
		/// 是否显是显示“评论过的记录”
		/// </summary>
		public bool SelectedCommented
		{
			get
			{
				return string.Compare(m_View, "commented", true) == 0;
			}
		}

		/// <summary>
		/// 是否显是显示“好友的记录”
		/// </summary>
		public bool SelectedFriend
		{
			get
			{ return string.Compare(m_View, "friend", true) == 0; }
		}


		/// <summary>
		/// 是否显是显示“大家的记录”
		/// </summary>
		public bool SelectedEveryone
		{
			get
			{ return string.Compare(m_View, "everyone", true) == 0; }
		}


        //===============================================

        protected bool IsShowDoingForm
        {
            get
            {
                if (IsSpace)
                {
                    if (VisitorIsOwner)
                        return true;
                }
                else if (SelectedMy)
                    return true;

                return false;
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

        private CommentCollection doingComments;
        private int CommentPageSize = 10;
        protected CommentCollection GetComments(Doing doing)
        {
            if (CommentListTargetID != doing.DoingID)
                return doing.CommentList;

            if (doingComments == null)
            {
                int count;
                doingComments = CommentBO.Instance.GetComments(doing.DoingID, CommentType.Doing, CommentListPageNumber, CommentPageSize, false, out count);

                FillSimpleUsers<Comment>(doingComments);

                SetPager("commentlist", null, CommentListPageNumber, CommentPageSize, count);
            }

            return doingComments;
        }

        protected bool IsShowGetAll(Doing doing)
        {
            if (doing.DoingID != CommentListTargetID)
            {
                if (doing.TotalComments > 2)
                    return true;
                else
                    return false;
            }

            return false;
        }

        protected bool IsShowCommentPager(Doing doing)
        {
            if (doing.DoingID != CommentListTargetID)
            {
                return false;
            }

            if (doing.TotalComments > CommentPageSize)
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
                    using (new ErrorScope())
                    {
                        int newCommentId;
                        string newContent;
                        bool success = CommentBO.Instance.AddComment(My, targetID, replyCommentID, CommentType.Doing, content, createIP, isReply, replyUserID, out newCommentId, out newContent);
                        if (success == false)
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                if (error is UnapprovedError)
                                    AlertWarning(error.Message);
                                else
                                    msgDisplay.AddError(error);
                            });
                        }
                        else
                        {
                            Doing doing = DoingBO.Instance.GetDoing(targetID);
                            if (doing != null)
                            {
                                m_CommentListPageNumber = doing.TotalComments / CommentPageSize;
                                if (doing.TotalComments % CommentPageSize > 0)
                                    m_CommentListPageNumber += 1;
                            }

                            ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");
                        }
                    }
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }
            }
        }

        #endregion
    }
}