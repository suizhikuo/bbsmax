//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using System.Diagnostics;
using System.Web.Configuration;
using System.Text;
using MaxLabs.bbsMax.Common;
using System.Configuration;
using MaxLabs.bbsMax.PointActions;
using System.Text.RegularExpressions;

namespace MaxLabs.bbsMax.Web
{
    public class ForumPageBase : AppBbsPageBase
    {
        /// <summary>
        /// 论坛版块页面的浏览器标题栏，需要显示板块名称
        /// </summary>
        protected override string PageTitle
        {
            get 
            {
                if (Forum == null)
                    return string.Concat("版块不存在", " - ", base.PageTitle);
                return string.Concat(Forum.ForumNameText, " - ", base.PageTitle); 
            }
        }

        protected override string PageTitleAttach
        {
            get
            {
                if (Forum == null)
                    return "";

                if (string.IsNullOrEmpty(Forum.ExtendedAttribute.TitleAttach))
                    return base.PageTitleAttach;
                else
                    return Forum.ExtendedAttribute.TitleAttach;
            }
        }

        protected override int NavigationForumID
        {
            get
            {
                return ForumID;
            }
        }

        protected override string MetaKeywords
        {
            get
            {
                if (Forum == null)
                    return "版块不存在";

                if (string.IsNullOrEmpty(Forum.ExtendedAttribute.MetaKeywords))
                    return base.MetaKeywords;
                else
                    return Forum.ExtendedAttribute.MetaKeywords;
            }
        }

        /// <summary>
        /// OnInit中是否发生了错误  一般出错了子级不再执行
        /// </summary>
        protected bool BaseHasError = false;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            bool success = CheckPermission();

            if (success == false)
            {
                BaseHasError = true;
                return;
            }
            ProcessForumType();

            //开始设置导航条

            //在导航条显示上级版块的链接（可能有多层上级版块）
            if (Forum == null)
                ShowError("版块不存在或者无法访问");
            if (Forum.ParentID > 0)
            {
                Forum parentForum;
                Forum currentForum = Forum;
                do
                {
                    parentForum = ForumBO.Instance.GetForum(currentForum.ParentID);
                    if (parentForum == null)
                        break;

                    if (parentForum.ForumType == ForumType.Normal)
                    {
                        if (!SiteSettings.DisplaySiteNameInNavigation)
                            InsertNavigationItem(1, parentForum.ForumName, GetNavigationForumUrl(parentForum));
                        else
                            InsertNavigationItem(2, parentForum.ForumName, GetNavigationForumUrl(parentForum));
                    }
                    currentForum = parentForum;
                }
                while (parentForum.ParentID > 0);
            }
        }

        protected virtual bool CheckPermission()
        {
            if (Forum == null)
            {
                ShowError("版块不存在或者无法访问");
                return false;
            }
            if (false == Forum.CanVisit(My))
            {
                if (!Forum.CanDisplayInList(My))
                {
                    ShowError("版块不存在或者无法访问");
                    return false;
                }
                else
                {
                    ShowError("您没有权限进入该版块");
                    return false;
                }

            }
            //版块类型正常，但需要密码
            if (Forum.ForumType == ForumType.Normal && !string.IsNullOrEmpty(Forum.Password))
            {
                //如果当前用户不拥有“进入版块不需要密码”的权限，继续检查
                if (!Forum.SigninWithoutPassword(My))
                {
                    //检查这个用户之前是否已经通过这个版块的验证，避免重复输入密码
                    if (!My.IsValidatedForum(Forum))
                        Response.Redirect(SignInForumUrl);
                }
            }

            return true;
        }

        protected virtual void ProcessForumType()
        {
            //如果论坛版块类型是链接，则跳转
            if (Forum.ForumType == ForumType.Link)
                Response.Redirect(Forum.Link);
            //如果论坛版块类型是分类，则停止处理
            else if (Forum.ForumType == ForumType.Catalog)
            {
                //AddNavigationItem(Forum.ForumName);
                return;
            }
        }

        protected override int CurrentForumID
        {
            get { return ForumID; }
        }

        private MenuPermissions m_MenuPermission = null;
        protected override MenuPermissions MenuPermission
        {
            get
            {
                if (m_MenuPermission == null)
                    m_MenuPermission = new MenuPermissions(My, CurrentForumID);

                return m_MenuPermission;
            }
        }

        protected virtual string SignInForumUrl
        {
            get { return BbsUrlHelper.GetSignInForumUrl(Forum.CodeName); }
        }

        protected virtual string GetNavigationForumUrl(Forum forum)
        {
            return BbsUrlHelper.GetForumUrl(forum.CodeName);
        }

        private Forum m_Forum;
        protected virtual Forum Forum
        {
            get
            {
                if (m_Forum == null)
                {
                    string codeName = _Request.Get("CodeName", Method.Get, string.Empty);

                    if (string.IsNullOrEmpty(codeName) == false)
                        m_Forum = ForumBO.Instance.GetForum(codeName);
                }

                return m_Forum;
            }
        }

        protected string CodeName
        {
            get { return Forum.CodeName; }
        }

        protected int ForumID
        {
            get { return Forum.ForumID; }
        }

        private string m_Ticket;
        protected override string Ticket
        {
            get
            {
                if (m_Ticket == null)
                    m_Ticket = Server.UrlEncode(My.GetRssUserTicket(Forum.Password));
                return m_Ticket;
            }
        }

        #region ModeratorActionLinks


        private void appendModeratorActionLink(StringBuilder sb, string outputFormat, string separator, string actionName, ThreadManageAction Action, string CodeName)
        {
            sb.AppendFormat(outputFormat, actionName, UrlUtil.JoinUrl(Dialog, "Forum", string.Concat(Action.ToString().ToLower(), ".aspx?codename=", CodeName.Replace("'", "\\'"))));
            sb.Append(separator);
        }

        protected virtual string GetModeratorActionLinks(string outputFormat, string separator)
        {
            if (string.IsNullOrEmpty(outputFormat))
                outputFormat = @"<a href=""javascript:postToDialog({{url:'{1}',callback:refresh}})"">{0}</a> ";

            StringBuilder sb = new StringBuilder();
            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled))
                appendModeratorActionLink(sb, outputFormat, separator, "回收主题", ThreadManageAction.RecycleThread, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads))
                appendModeratorActionLink(sb, outputFormat, separator, "删除主题", ThreadManageAction.DeleteThread, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.MoveThreads))
                appendModeratorActionLink(sb, outputFormat, separator, "移动主题", ThreadManageAction.MoveThread, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock))
                appendModeratorActionLink(sb, outputFormat, separator, "设置锁定", ThreadManageAction.LockThread, CodeName);

            if (ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle))
                appendModeratorActionLink(sb, outputFormat, separator, "高亮显示", ThreadManageAction.SetThreadSubjectStyle, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick)
                || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick))
                appendModeratorActionLink(sb, outputFormat, separator, "设置置顶", ThreadManageAction.SetThreadIsTop, CodeName);

            if (ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsUp))
                appendModeratorActionLink(sb, outputFormat, separator, "提升主题", ThreadManageAction.UpThread, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder))
                appendModeratorActionLink(sb, outputFormat, separator, "自动沉帖", ThreadManageAction.SetThreadNotUpdateSortOrder, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued))
                appendModeratorActionLink(sb, outputFormat, separator, "设置精华", ThreadManageAction.SetThreadElite, CodeName);

            if (Forum.ThreadCatalogStatus != ThreadCatalogStatus.DisEnable &&
                ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog))
                appendModeratorActionLink(sb, outputFormat, separator, "设置分类", ThreadManageAction.UpdateThreadCatalog, CodeName);

            if (ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads))
                appendModeratorActionLink(sb, outputFormat, separator, "鉴定主题", ThreadManageAction.JudgementThread, CodeName);

            if (sb.Length > separator.Length)
                return sb.ToString(0, sb.Length - separator.Length);
            return sb.ToString();
        }

        #endregion

        #region 权限


        protected bool ViewValuedThread
        {
            get { return Can(ForumPermissionSetNode.Action.ViewValuedThread); }
        }

        private bool? m_IsShowModeratorManageLink;
        protected virtual bool IsShowModeratorManageLink
        {
            get
            {
                if (m_IsShowModeratorManageLink == null)
                {
                    if (CanManageThread
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.ApproveThreads)
                        || ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.ApprovePosts)
                        || ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.UpdateForumReadme)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.BanUser))
                        m_IsShowModeratorManageLink = true;
                    else
                        m_IsShowModeratorManageLink = false;
                }
                return m_IsShowModeratorManageLink.Value;
            }
        }

        private bool? m_CanManageThread;
        protected virtual bool CanManageThread
        {
            get
            {
                if (m_CanManageThread == null)
                    if (//Forum.IsModerator(MyUserID) ||
                        ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsRecycled)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.DeleteAnyThreads)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.MoveThreads)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsLock)
                        || ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsSubjectStyle)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsStick)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsGlobalStick)
                        || ForumManagePermission.Can(My, ManageForumPermissionSetNode.Action.SetThreadsUp)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadNotUpdateSortOrder)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.SetThreadsValued)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.UpdateThreadCatalog)
                        || ForumManagePermission.HasPermissionForSomeone(My, ManageForumPermissionSetNode.ActionWithTarget.JudgementThreads))
                        m_CanManageThread = true;
                    else
                        m_CanManageThread = false;

                return m_CanManageThread.Value;
            }
        }

        private bool? m_CanCreateThread;
        protected bool CanCreateThread
        {
            get
            {
                if (m_CanCreateThread == null)
                {
                    m_CanCreateThread = Can(ForumPermissionSetNode.Action.CreateThread);
                }
                return m_CanCreateThread.Value;
            }
        }


        private bool? m_CanCreateQuestion;
        protected bool CanCreateQuestion
        {
            get
            {
                if (m_CanCreateQuestion == null)
                {
                    m_CanCreateQuestion = Can(ForumPermissionSetNode.Action.CreateQuestion);
                }
                return m_CanCreateQuestion.Value;
            }
        }


        private bool? m_CanCreatePoll;
        protected bool CanCreatePoll
        {
            get
            {
                if (m_CanCreatePoll == null)
                {
                    m_CanCreatePoll = Can(ForumPermissionSetNode.Action.CreatePoll);
                }
                return m_CanCreatePoll.Value;
            }
        }


        private bool? m_CanCreatePolemize;
        protected bool CanCreatePolemize
        {
            get
            {
                if (m_CanCreatePolemize == null)
                {
                    m_CanCreatePolemize = Can(ForumPermissionSetNode.Action.CreatePolemize);
                }
                return m_CanCreatePolemize.Value;
            }
        }

        private bool? m_PostEnableReplyNotice;
        protected bool PostEnableReplyNotice
        {
            get
            {
                if (m_PostEnableReplyNotice == null)
                    m_PostEnableReplyNotice = Can(ForumPermissionSetNode.Action.PostEnableReplyNotice);
                return m_PostEnableReplyNotice.Value;
            }
        }


        protected bool Can(ForumPermissionSetNode.Action action)
        {
            return ForumPermission.Can(My, action);
        }

        protected bool CanManage(ManageForumPermissionSetNode.Action action)
        {
            return ForumManagePermission.Can(My, action);
        }

        protected bool CanManage(ManageForumPermissionSetNode.ActionWithTarget action, int targetUserID)
        {
            return ForumManagePermission.Can(My, action, targetUserID);
        }


        private ForumPermissionSetNode m_forumPermission;
        protected ForumPermissionSetNode ForumPermission
        {
            get
            {
                if (m_forumPermission == null)
                {
                    if (Forum != null)
                    {
                        m_forumPermission = AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(Forum.ForumID);
                    }
                }
                return m_forumPermission;
            }
        }

        private ManageForumPermissionSetNode m_forumManagePermission;
        protected ManageForumPermissionSetNode ForumManagePermission
        {
            get
            {
                if (m_forumManagePermission == null)
                {
                    if (Forum != null)
                    {
                        m_forumManagePermission = AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(Forum.ForumID);
                    }
                }
                return m_forumManagePermission;
            }
        }

        #endregion

        #region 帖子图标
        protected string GetIcon(int iconID)
        {
            return GetIcon(iconID, null);
        }
        protected string GetIcon(int iconID, string imgStyle)
        {
            if (EnablePostIcon == false)
                return string.Empty;

            if (string.IsNullOrEmpty(imgStyle))
                imgStyle = "<img src=\"{0}\" border=\"0\" alt=\"\" />";

            if (iconID <= 0)
                return string.Empty;
            PostIcon postIcon = PostBOV5.Instance.GetPostIcon(iconID);
            if (postIcon == null)
                return string.Empty;
            return string.Format(imgStyle, postIcon.IconUrl);
        }

        #endregion

        #region ThreadCatalog

        private ThreadCatalogCollection m_ThreadCatalogs;
        protected ThreadCatalogCollection ThreadCatalogs
        {
            get
            {
                if (m_ThreadCatalogs == null)
                {
                    m_ThreadCatalogs = ForumBO.Instance.GetThreadCatalogs(ForumID);
                }
                return m_ThreadCatalogs;
            }
        }

        //根据主题分类ID返回该分类的名称
        protected string GetThreadCatalogName(int threadCatalogID, string style)
        {
            if (threadCatalogID != 0)
            {
                ThreadCatalog catalog = ForumBO.Instance.GetAllThreadCatalogs().GetValue(threadCatalogID);
                if (catalog != null)
                    return string.Format(style, catalog.ThreadCatalogName);
            }

            return string.Empty;
        }

        protected string GetThreadCatalogList(int selectCatalogID, bool isShow)
        {
            ThreadCatalogCollection threadCatalogs = ForumBO.Instance.GetThreadCatalogs(ForumID);
            if (!isShow || threadCatalogs.Count == 0)
                return string.Empty;


            if (Forum.ThreadCatalogStatus == ThreadCatalogStatus.DisEnable)
                return string.Empty;

            StringBuilder selectString = new StringBuilder();
            selectString.Append("<select name=\"threadCatalogs\" size=\"1\">\r\n");

            int i = 0;
            foreach (ThreadCatalog threadCatalog in threadCatalogs)
            {
                if (i == 0)
                {
                    if (threadCatalog.ThreadCatalogID != 0)
                        if (Forum.ThreadCatalogStatus == ThreadCatalogStatus.Enable)
                            selectString.Append("<option value=\"0\">不选</option>\r\n");
                        else
                            selectString.Append("<option value=\"0\">请选择分类</option>\r\n");
                }

                if (selectCatalogID == threadCatalog.ThreadCatalogID)
                    selectString.Append("<option value=\"").Append(threadCatalog.ThreadCatalogID).Append("\" selected=\"selected\">").Append(threadCatalog.ThreadCatalogName).Append("</option>\r\n");
                else
                    selectString.Append("<option value=\"").Append(threadCatalog.ThreadCatalogID).Append("\">").Append(threadCatalog.ThreadCatalogName).Append("</option>\r\n");
                i++;
            }
            selectString.Append("</select>");

            return selectString.ToString();
        }

        #endregion

        #region 积分

        Dictionary<int, UserPoint> m_SellThreadPoints = new Dictionary<int, UserPoint>();
        protected UserPoint GetSellThreadPoint(int userID)
        {
            if (m_SellThreadPoints.ContainsKey(userID))
                return m_SellThreadPoints[userID];
            else
            {
                UserPoint userPoint = ForumPointAction.Instance.GetUserPoint(userID, ForumPointValueType.SellThread, Forum.ForumID);
                m_SellThreadPoints.Add(userID, userPoint);
                return userPoint;
            }
        }

        #endregion

        #region forumSetting 设置

        private ForumSettingItem m_ForumSetting;
        protected ForumSettingItem ForumSetting
        {
            get
            {
                if (m_ForumSetting == null)
                    m_ForumSetting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(Forum.ForumID);
                return m_ForumSetting;
            }
        }

        protected bool IsEnableThreadRank
        {
            get
            {
                return ForumSetting.EnableThreadRank;
            }
        }

        private bool? m_AllowHtml;
        protected bool AllowHtml
        {
            get
            {
                if (m_AllowHtml == null)
                    m_AllowHtml = ForumSetting.CreatePostAllowHTML.GetValue(My);

                return m_AllowHtml.Value;
            }
        }

        private bool? m_AllowMaxcode;
        protected bool AllowMaxcode
        {
            get
            {
                if (m_AllowMaxcode == null)
                    m_AllowMaxcode = ForumSetting.CreatePostAllowMaxcode.GetValue(My);

                return m_AllowMaxcode.Value;
            }
        }

        protected bool IsShowHtmlAndMaxCode
        {
            get
            {
                return AllowHtml && AllowMaxcode;
            }
        }

        private bool? m_AllowAudioTag;
        protected bool AllowAudioTag
        {
            get
            {
                if (m_AllowAudioTag == null)
                    m_AllowAudioTag = ForumSetting.CreatePostAllowAudioTag.GetValue(My);

                return m_AllowAudioTag.Value;
            }
        }

        private bool? m_AllowEmoticon;
        public bool AllowEmoticon
        {
            get
            {
                if (m_AllowEmoticon == null)
                    m_AllowEmoticon = ForumSetting.CreatePostAllowEmoticon.GetValue(My);

                return m_AllowEmoticon.Value;
            }
        }

        private bool? m_AllowFlashTag;
        protected bool AllowFlashTag
        {
            get
            {
                if (m_AllowFlashTag == null)
                    m_AllowFlashTag = ForumSetting.CreatePostAllowFlashTag.GetValue(My);
                return m_AllowFlashTag.Value;
            }
        }

        private bool? m_AllowImageTag;
        protected bool AllowImageTag
        {
            get
            {
                if (m_AllowImageTag == null)
                    m_AllowImageTag = ForumSetting.CreatePostAllowImageTag.GetValue(My);

                return m_AllowImageTag.Value;
            }
        }
        private bool? m_AllowTableTag;
        public bool AllowTableTag
        {
            get
            {
                if (m_AllowTableTag == null)
                    m_AllowTableTag = ForumSetting.CreatePostAllowTableTag.GetValue(My);
                return m_AllowTableTag.Value;
            }
        }

        private bool? m_AllowUrlTag;
        protected bool AllowUrlTag
        {
            get
            {
                if (m_AllowUrlTag == null)
                    m_AllowUrlTag = ForumSetting.CreatePostAllowUrlTag.GetValue(My);

                return m_AllowUrlTag.Value;
            }
        }

        private bool? m_AllowVideoTag;
        protected bool AllowVideoTag
        {
            get
            {
                if (m_AllowVideoTag == null)
                    m_AllowVideoTag = ForumSetting.CreatePostAllowVideoTag.GetValue(My);

                return m_AllowVideoTag.Value;

            }
        }


        private bool? m_ShowSignatureInPost;
        protected bool ShowSignatureInPost
        {
            get
            {
                if (m_ShowSignatureInPost == null)
                    m_ShowSignatureInPost = ForumSetting.ShowSignatureInThread.GetValue(My);
                return m_ShowSignatureInPost.Value;
            }
        }

        protected long SellThreadDays
        {
            get
            {
                return ForumSetting.SellThreadDays;
            }
        }

        protected long SellAttachmentDays
        {
            get
            {
                return ForumSetting.SellAttachmentDays;
            }
        }

        protected bool IsOverSellThreadDays(BasicThread thread)
        {
            if(SellThreadDays == 0)
                return false;
            return thread.CreateDate.AddSeconds(SellThreadDays) <= DateTimeUtil.Now;
        }

        protected bool IsOverSellAttachmentDays(Attachment attachment)
        {
            return attachment.IsOverSellDays(ForumSetting);
        }

        #endregion

        #region Settings

        protected bool EnableGuestNickName
        {
            get
            {
                return AllSettings.Current.BbsSettings.EnableGuestNickName;
            }
        }


        protected bool QuicklyPostUseAjax
        {
            get
            {
                return AllSettings.Current.AjaxSettings.QuicklyPostUseAjax;
            }
        }

        protected int HotThreadRequireReplies
        {
            get
            {
                return BbsSettings.HotThreadRequireReplies;
            }
        }

        protected bool EnablePostIcon
        {
            get
            {
                return AllSettings.Current.PostIconSettings.EnablePostIcon;
            }
        }

        #endregion

        #region  ForumList  论坛跳转

        private ForumCollection m_ForumsTree;
        protected ForumCollection ForumsTree
        {
            get
            {
                if (m_ForumsTree == null)
                {
                    GetForums();
                }
                return m_ForumsTree;
            }
        }

        private List<string> m_ForumTreeSeparators;
        protected List<string> ForumTreeSeparators
        {
            get
            {
                if (m_ForumTreeSeparators == null)
                {
                    GetForums();
                }
                return m_ForumTreeSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", delegate(Forum forum)
            {
                if (forum.CanDisplayInList(My))
                    return true;
                else
                    return false;
            }, out m_ForumsTree, out m_ForumTreeSeparators);
        }


        #endregion

        #region 快速发帖 表情相关


        private List<EmoticonGroupBase> m_userEmoticnGroups;
        protected List<EmoticonGroupBase> UserEmoticonGroups
        {
            get
            {
                if (m_userEmoticnGroups == null)
                {
                    m_userEmoticnGroups = EmoticonBO.Instance.GetUserEmoticonGroupList(MyUserID);
                }
                return m_userEmoticnGroups;
            }
        }

        /// <summary>
        /// 系统默认表情
        /// </summary>
        private List<IEmoticonBase> m_DefaultEmoticons;
        protected List<IEmoticonBase> DefaultEmoticons
        {
            get
            {
                if (m_DefaultEmoticons == null)
                {
                    m_DefaultEmoticons = new List<IEmoticonBase>();

                    if (CurrentEmoticonGroup.IsDefault)
                        foreach (DefaultEmoticon emote in (CurrentEmoticonGroup as DefaultEmoticonGroup).Emoticons)
                            m_DefaultEmoticons.Add(emote);
                    else
                        foreach (Emoticon emote in EmoticonBO.Instance.GetEmoticons(MyUserID, CurrentEmoticonGroup.GroupID))
                            m_DefaultEmoticons.Add(emote);
                }
                return m_DefaultEmoticons;
            }
        }

        private List<IEmoticonBase> pageDefaultEmoticons;
        protected List<IEmoticonBase> PageDefaultEmoticons
        {
            get
            {
                if (pageDefaultEmoticons == null)
                {
                    if (EmoticonPage > DefaultEmoticonPageCount)
                    {
                        pageDefaultEmoticons = new List<IEmoticonBase>();
                    }
                    else
                    {
                        List<IEmoticonBase> tempEmoticons = DefaultEmoticons;

                        int count = (EmoticonPage * emoticonPageSize > tempEmoticons.Count) ? tempEmoticons.Count : EmoticonPage * emoticonPageSize;

                        pageDefaultEmoticons = new List<IEmoticonBase>();
                        for (int i = (EmoticonPage - 1) * emoticonPageSize; i < count; i++)
                        {
                            pageDefaultEmoticons.Add(tempEmoticons[i]);
                        }
                    }
                }
                return pageDefaultEmoticons;
            }
        }

        protected int DefaultEmoticonPageCount
        {
            get
            {
                return DefaultEmoticons.Count % emoticonPageSize == 0 ? (DefaultEmoticons.Count / emoticonPageSize) : (DefaultEmoticons.Count / emoticonPageSize + 1);
            }
        }

        private List<DefaultEmoticon> userDefaultEmoticons;


        /// <summary>
        /// 用户自己添加的默认表情
        /// </summary>
        protected List<DefaultEmoticon> UserDefaultEmoticons
        {
            get
            {
                if (userDefaultEmoticons == null)
                {
                    userDefaultEmoticons = new List<DefaultEmoticon>();
                    //if (DefaultEmoticonGroup == null)
                    //    userDefaultEmoticons = new List<DefaultEmoticon>();
                    //else
                    //{
                    //    userDefaultEmoticons = EmoticonManager.GetEmoticons(MyUserID, DefaultEmoticonGroup.GroupID);

                    //}
                }
                return userDefaultEmoticons;
            }
        }

        private EmoticonCollection pageUserDefaultEmoticons;
        protected EmoticonCollection PageUserDefaultEmoticons
        {
            get
            {
                if (pageUserDefaultEmoticons == null)
                {
                    //int tempPageCount = DefaultEmoticons.Count % emoticonPageSize == 0 ? (DefaultEmoticonPageCount + 1) : DefaultEmoticonPageCount;
                    //if (DefaultEmoticonGroup == null || EmoticonPage < tempPageCount)
                    //    pageUserDefaultEmoticons = new List<DefaultEmoticon>();
                    //else
                    //{
                    pageUserDefaultEmoticons = EmoticonBO.Instance.GetEmoticons(MyUserID, CurrentEmoticonGroup.GroupID, EmoticonPage, emoticonPageSize, false);

                    //    int a = DefaultEmoticons.Count % emoticonPageSize;

                    //    int count, start;
                    //    if (EmoticonPage == tempPageCount)
                    //    {
                    //        start = 0;
                    //        count = (emoticonPageSize > (tempDefaultEmoticons.Count+a)) ? tempDefaultEmoticons.Count : (emoticonPageSize - a);
                    //    }
                    //    else
                    //    {
                    //        start = (EmoticonPage - 1) * emoticonPageSize - DefaultEmoticons.Count;
                    //        count = ((EmoticonPage * emoticonPageSize - DefaultEmoticons.Count) > tempDefaultEmoticons.Count) ? tempDefaultEmoticons.Count : (EmoticonPage * emoticonPageSize - DefaultEmoticons.Count);
                    //    }
                    //    pageUserDefaultEmoticons = new List<DefaultEmoticon>();
                    //    for (int i = start; i < count; i++)
                    //    {
                    //        pageUserDefaultEmoticons.Add(tempDefaultEmoticons[i]);
                    //    }
                    //}
                }
                return pageUserDefaultEmoticons;
            }
        }


        private bool IsDefaultGroup
        {
            get
            {
                //Warning 如果是系统表情排列在前面的话， 后面的默认要是 true,反之应该是false
                //这个属性是为了避免用户表情分组和系统表情分组编号相同的情况下区分， 一般几率不会很大
                return _Request.Get<bool>("issys", Method.Get, true);
            }
        }

        private EmoticonGroupBase m_currentEmoticonGroup;
        protected EmoticonGroupBase CurrentEmoticonGroup
        {
            get
            {
                if (m_currentEmoticonGroup == null)
                {
                    foreach (EmoticonGroupBase group in UserEmoticonGroups)
                    {
                        if (group.GroupID == EmoticonGroupID)// && group.IsDefault==IsDefaultGroup)
                        {
                            m_currentEmoticonGroup = group;
                            break;
                        }
                    }




                    if (m_currentEmoticonGroup == null)
                    {
                        if (UserEmoticonGroups != null && UserEmoticonGroups.Count > 0)
                        {
                            m_currentEmoticonGroup = UserEmoticonGroups[0];
                        }
                        else
                        {
                            m_currentEmoticonGroup = new DefaultEmoticonGroup();
                        }
                    }

                }
                return m_currentEmoticonGroup;
            }
        }

        /// <summary>
        /// 获取当前表情的X坐标（默认表情是合并成一张图片）
        /// </summary>
        /// <param name="loopIndex"></param>
        /// <param name="emoticonWidth"></param>
        /// <returns></returns>
        protected int GetDefaultEmoticonX(int loopIndex, int emoticonWidth)
        {
            if (!CurrentEmoticonGroup.IsDefault) return 0;
            return (loopIndex + (EmoticonPage - 1) * emoticonPageSize) * emoticonWidth;
        }

        protected const int emoticonPageSize = 32;


        protected DefaultEmoticon Emoticon;
        protected int EmoticonGroupID
        {
            get
            {
                return _Request.Get<int>("EmoticonGroupID", Method.Get, 0);
            }
        }
        protected int EmoticonPage
        {
            get
            {
                return _Request.Get<int>("emoticonPage", Method.Get, 1);
            }
        }

        /// <summary>
        /// 当前表情分组的表情个数
        /// </summary>
        protected int CurrentGroupEmotioconCount
        {
            get
            {
                return CurrentEmoticonGroup.TotalEmoticons;
            }
        }

        protected bool IsGetAllDefaultEmoticon
        {
            get
            {
                if (EmoticonPage > 1)
                    return true;
                //if (EmoticonPage >= DefaultEmoticonPageCount&&EmoticonPage>1)
                //        return true;
                if (Request.QueryString["IsGetAllDefaultEmoticon"] == null)
                {
                    return false;
                }
                else
                {
                    //    if (EmoticonPage < DefaultEmoticonPageCount)
                    //        return false;
                    //    else
                    return _Request.Get("IsGetAllDefaultEmoticon", Method.Get, "false") == "true";
                }
            }
        }

        protected EmoticonGroupBase tempGroup;
        protected IEmoticonBase tempEmoticon;

        /// <summary>
        /// 表情分页 显示按钮个数
        /// </summary>
        protected int EmoticonPagerButtonCount
        {
            get
            {
                return 5;
            }
        }

        #endregion

        protected override string PageName
        {
            get
            {
                return "forums";
            }
        }
    }
}