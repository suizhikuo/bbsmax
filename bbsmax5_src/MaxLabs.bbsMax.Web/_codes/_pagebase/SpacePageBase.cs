//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web
{
    public abstract class SpacePageBase : AppPageBase
	{



        protected override void OnInit(EventArgs e)
		{
            base.OnInit(e);

            if (AllSettings.Current.SpaceSettings.AllowGuestAccess == false)
            {
                if (MyUserID <= 0)
                {
                    ShowError(new Errors.CustomError("", "您必须登录后才能访问用户空间"), BbsRouter.GetUrl("login"));
                }
            }

			if (EnableFunction == false)
				BbsRouter.JumpTo("space/" + SpaceOwnerID);


			if (SpaceOwner == null)
				ShowError("用户空间不存在");

            OnlineUserPool.Instance.Update(My, _Request, OnlineAction.ViewSpace, 0, 0, "");

		}

		protected override void OnLoadComplete(EventArgs e)
		{
			MaxLabs.bbsMax.Entities.User user = UserBO.Instance.GetUser(SpaceOwnerID);

			if (user != null)
			{
                if (IsSpaceOwnerFullSiteBanned)
                {
                    if (VisitorIsAdmin)
                    {
                        m_SpaceCanAccess = true;
                        m_SpaceDisplayAdminNote = true;
                    }
                    else
                    {
                        m_SpaceCanAccess = false;
                    }
                }
				else if (SpaceOwnerID == MyUserID)
				{
					m_SpaceCanAccess = true;
				}
				else if (VisitorInBlackList)
				{
					m_SpaceCanAccess = false;
				}
				else if (SpaceOwner.SpacePrivacy == SpacePrivacyType.All)
				{
					m_SpaceCanAccess = true;
				}
				else if (SpaceOwner.SpacePrivacy == SpacePrivacyType.Self)
				{
					m_SpaceCanAccess = SpaceOwnerID == MyUserID;
				}
				else if (SpaceOwner.SpacePrivacy == SpacePrivacyType.Friend)
				{
					m_SpaceCanAccess = VisitorIsFriend;
				}

				if (m_SpaceCanAccess == false)
				{
					if (VisitorIsAdmin)
					{
						m_SpaceCanAccess = true;
						m_SpaceDisplayAdminNote = true;
					}
				}

                if (IsSpaceOwnerFullSiteBanned)
                {
                    if (VisitorIsAdmin)
                    {
                        m_FunctionCanAccess = true;
                        m_FunctionDisplayAdminNote = true;
                    }
                    else
                    {
                        m_FunctionCanAccess = false;
                    }
                }
				if (SpaceOwnerID == MyUserID)
				{
					m_FunctionCanAccess = true;
				}
				else if (VisitorInBlackList)
				{
					m_FunctionCanAccess = false;
				}
				else if (FunctionPrivacy == SpacePrivacyType.All)
				{
					m_FunctionCanAccess = true;
				}
				else if (FunctionPrivacy == SpacePrivacyType.Self)
				{
					m_FunctionCanAccess = SpaceOwnerID == MyUserID;
				}
				else if (FunctionPrivacy == MaxLabs.bbsMax.Enums.SpacePrivacyType.Friend)
				{
					m_FunctionCanAccess = VisitorIsFriend;
				}
            
				if (m_FunctionCanAccess == false)
				{
					if (VisitorIsAdmin)
					{
						m_FunctionCanAccess = true;
						m_SpaceDisplayAdminNote = true;
					}
				}

				m_SpaceName = user.Name + "的个人空间"; //TODO:在User表加上SpaceName字段，并在用户中心提供设置

				if (SpaceCanAccess && FunctionCanAccess && !My.IsInvisible )
					SpaceBO.Instance.VisitSpace(MyUserID, SpaceOwnerID, _Request.IpAddress);
			}

			base.OnLoadComplete(e);
		}

        protected override bool NeedLogin
        {
            get
            {
                return false;
            }
        }


        protected bool IsPreviewTheme
        {
            get
            {
                return PreviewTheme != null;
            }
        }

        protected string PreviewTheme
        {
            get
            {
                string theme = _Request.Get("theme", Method.Get, null, false);

                if (theme != null && _Request.Get("op") == null)
                {
                    SpaceThemeCollection themes = SpaceBO.Instance.GetSpaceThemes();

                    if (themes != null)
                    {
                        foreach (SpaceTheme themeItem in themes)
                        {
                            if (string.Compare(themeItem.Dir, theme, true) == 0)
                                return StringUtil.HtmlEncode(theme);
                        }
                    }
                }

                return null;
            }
        }






        private bool? m_IsSpaceOwnerFullSiteBanned;

        public bool IsSpaceOwnerFullSiteBanned
        {
            get
            {
                if (m_IsSpaceOwnerFullSiteBanned == null)
                {
                    m_IsSpaceOwnerFullSiteBanned = SpaceOwner.Roles.IsInRole(Role.FullSiteBannedUsers);
                }

                return m_IsSpaceOwnerFullSiteBanned.Value;
            }
        }

		private bool? m_VisitorInBlackList;

		protected bool VisitorInBlackList
		{
			get
			{
				if (m_VisitorInBlackList.HasValue == false)
					m_VisitorInBlackList = FriendBO.Instance.InMyBlacklist(SpaceOwnerID, MyUserID);

				return m_VisitorInBlackList.Value;
			}
		}




		protected abstract SpacePrivacyType FunctionPrivacy
		{
			get;
		}

		protected abstract bool EnableFunction
		{
			get;
		}

		private bool m_FunctionCanAccess;

		public bool FunctionCanAccess
		{
			get { return m_FunctionCanAccess; }
		}

		private bool m_FunctionDisplayAdminNote;

		public bool FunctionDisplayAdminNote
		{
			get { return m_FunctionDisplayAdminNote; }
		}

		//老达TODO:改为抽象
        //zzbird NOTE:不改也没啥，不重写默认就显示这个
        //sek:我改了  在 onload 里  SetPageTitle(m_SpaceName);
        //protected virtual string PageTitle
        //{
        //    get 
        //    { 
        //        return m_SpaceName;
        //        
        //    }
        //}

		//private int? m_SpaceOwnerID;

		//老达TODO:改为抽象
		protected int SpaceOwnerID
		{
			get 
            { 
                //return m_SpaceOwnerID.GetValueOrDefault(-1); 
                return AppOwnerUserID;
            }
		}

		public User SpaceOwner
		{
			get
			{
                return AppOwner;
			}
		}

		//老达TODO:改为抽象
		protected virtual bool VisitorIsAdmin
		{
			get { return false; }
		}



		private CommentType m_CommentType;


		//老达TODO:改为抽象
		protected virtual CommentType CommentType
		{
			get { return m_CommentType; }
		}

		private int m_CommentTargetID;

		public virtual int CommentTargetID
		{
			get { return m_CommentTargetID; }
		}

		private bool m_SpaceCanAccess;

		/// <summary>
		/// 获取空间是否可以访问
		/// </summary>
		public bool SpaceCanAccess
		{
			get { return m_SpaceCanAccess; }
		}

		private bool m_SpaceDisplayAdminNote;

		/// <summary>
		/// 获取空间是否因为当前访问者是管理员所以可见
		/// </summary>
		public bool SpaceDisplayAdminNote
		{
			get { return m_SpaceDisplayAdminNote; }
		}

		private string m_SpaceName;

		/// <summary>
		/// 获取用户空间名称
		/// </summary>
		public string SpaceName
		{
			get { return m_SpaceName; }
		}

        public virtual string SpaceTitle
        {
            get { return SpaceOwner.Name; }
        }

		private bool m_AddCommentSucceed;

		public bool AddCommentSucceed
		{
			get { return m_AddCommentSucceed; }
		}

		public virtual bool IsSelectedSpace
		{
			get 
			{
				return false;
			}
		}

		public virtual bool IsSelectedDoing
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSelectedShare
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSelectedBlog
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSelectedAlbum
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSelectedFeed
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSelectedBoard
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSelectedFriend
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
		protected void AddComment()
		{
			AddComment(null, null, null);
		}

		protected void AddComment(string jumpUrl)
		{
			AddComment(jumpUrl, null, null);
		}

        protected void AddComment(string jumpUrl, string jumpUrlQueryString)
        {
            AddComment(jumpUrl, jumpUrlQueryString, null);
        }

		protected void AddComment(string jumpUrl, string jumpUrlQueryString, string form)
		{
            MessageDisplay msgDisplay = CreateMessageDisplayForForm(form, "content", GetValidateCodeInputName("CreateComment"));

			string content = _Request.Get("Content", Method.Post, "", false);

			CommentType commentType = this.CommentType;// _Request.Get<CommentType>("type", Method.All, CommentType.All);

			int? commentID = _Request.Get<int>("commentid", Method.Get);

			string createIP = _Request.IpAddress;

			int userID = MyUserID;

			if (!CheckValidateCode("CreateComment", msgDisplay))
			{
                AlertError("验证码错误");
				return;
			}

            if (commentID.HasValue)
            {

                if (commentType == CommentType.Board)
                {
                    Comment comment = CommentBO.Instance.GetCommentByID(commentID.Value);
                    m_CommentTargetID = comment.UserID;
                }
                //content = string.Format("<div class=\"quote\"><span class=\"q\"><b>{0}</b>: {1}</span></div>", UserBO.Instance.GetUser(userID).Name, comment.Content) + content;
            }

			try
			{
				using (new ErrorScope())
				{
					int newCommentId;
					
					m_AddCommentSucceed = CommentBO.Instance.AddComment(My, CommentTargetID, commentID.GetValueOrDefault(), CommentType, content, createIP, out newCommentId);
					
					if (m_AddCommentSucceed == false)
					{
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UnapprovedError)
                                AlertWarning(error.Message);
                            else
                            {
                                AlertError(error.Message);
                                msgDisplay.AddError(error);
                            }
                        });
					}
					else
					{
						MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateValidateCodeActionRecode("CreateComment");

						if (jumpUrl != null)
						{
							if (jumpUrlQueryString != null)
								BbsRouter.JumpTo(jumpUrl, jumpUrlQueryString);
							else
								BbsRouter.JumpTo(jumpUrl);
						}

						//if (!IsDialog)

						//m_JsonComment = PackingComment(CommentTargetID, newCommentId, content);
					}
				}
			}
			catch (Exception ex)
			{
                msgDisplay.AddException(ex);
			}
		}

		private string m_JsonComment = "{}";

		protected string JsonComment
		{
			get
			{
				return m_JsonComment;
			}
		}

		private string PackingComment(int parentId, int commentId, string content)
		{
			StringBuffer buffer = new StringBuffer();
			buffer += "{";
			buffer += string.Format("\"parentId\":{0}", parentId);
			buffer += ",";
			buffer += string.Format("\"id\":{0}", commentId);
			buffer += ",";
			buffer += string.Format("\"date\":\"{0}\"", DateTimeUtil.GetFriendlyDate(DateTimeUtil.Now));
			buffer += ",";
			buffer += string.Format("\"userId\":{0}", My.UserID);
			buffer += ",";
			buffer += string.Format("\"username\":\"{0}\"", StringUtil.ToJavaScriptString(My.Realname));
			buffer += ",";
			buffer += string.Format("\"content\":\"{0}\"", StringUtil.ToJavaScriptString(content));
			buffer += "}";
			return buffer.ToString();
        }

        protected bool CheckFunctionCanDisplay(SpacePrivacyType privacyType)
        {
            if (SpaceOwnerID == MyUserID)
            {
                return true;
            }
            else if (VisitorInBlackList)
            {
                return false;
            }
            else if (privacyType == SpacePrivacyType.All)
            {
                return true;
            }
            else if (privacyType == SpacePrivacyType.Self)
            {
                return SpaceOwnerID == MyUserID;
            }
            else if (privacyType == MaxLabs.bbsMax.Enums.SpacePrivacyType.Friend)
            {
                return VisitorIsFriend || VisitorIsAdmin;
            }

            return true;
        }






        private bool? m_BoardCanDisplay;
        protected bool BoardCanDisplay
        {
            get
            {
                if (m_BoardCanDisplay == null)
                {
                    m_BoardCanDisplay = CheckFunctionCanDisplay(SpaceOwner.BoardPrivacy);
                }

                return m_BoardCanDisplay.Value;
            }
        }



        protected bool IsOverrideSpaceTheme
        {
            get
            {
                return OverrideSpaceTheme != null;
            }
        }

        protected string OverrideSpaceTheme
        {
            get
            {
                if (VisitorIsOwner && IsSelectedSpace && IsPreviewTheme)
                {
                    return PreviewTheme;
                }
                else if (String.IsNullOrEmpty(SpaceOwner.SpaceTheme) == false)
                {
                    return SpaceOwner.SpaceTheme;
                }

                return null;
            }
        }


        #region 动态 Feed

        protected string FormatFeedTitle(Feed feed)
        {
            float timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(My);

            return FeedBO.Instance.FormatFeedTitle(SpaceOwnerID, timeDiffrence, FeedType.MyFeed, feed);
        }

        protected string FormatFeedDescription(Feed feed)
        {
            return FeedBO.Instance.FormatFeedDescription(SpaceOwnerID, feed);
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
        #endregion









        protected virtual bool IsSpacePage
        {
            get
            {
                return false;
            }
        }

    }
}