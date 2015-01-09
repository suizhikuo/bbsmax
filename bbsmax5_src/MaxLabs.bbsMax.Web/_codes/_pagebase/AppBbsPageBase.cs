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
using System.Diagnostics;
using System.Web.Configuration;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Text.RegularExpressions;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web
{
    public class AppBbsPageBase : BbsPageBase
    {

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

        private string m_Ticket;
        /// <summary>
        ///  用于Rss
        /// </summary>
        protected virtual string Ticket
        {
            get
            {
                if (m_Ticket == null)
                {
                    m_Ticket = Server.UrlEncode(My.GetRssUserTicket(string.Empty));
                }
                return m_Ticket;
            }
        }

        protected string GetModeratorLinks(Forum forum, string linkStyle, string separator)
        {
            if (forum.Moderators == null || forum.Moderators.Count < 1)
                return "-";
            else
            {
                return GetModeratorLinks(forum.Moderators, linkStyle, separator);
            }
        }

        protected string GetModeratorLinks(ModeratorCollection moderators, string linkStyle, string separator)
        {
            if (string.IsNullOrEmpty(linkStyle))
                linkStyle = "<a href=\"{0}\" target=\"_blank\">{1}</a>{2}";
            if (separator == null)
                separator = ", ";

            StringBuilder simpleUserLinksString = new StringBuilder();

            //FillSimpleUsers<Moderator>(moderators);

            foreach (Moderator moderator in moderators)
            {
                if (moderator.User == null)
                    continue;

                string str = string.Empty;
                if (moderator.ModeratorType == ModeratorType.JackarooModerators)
                    str = "(实习)";

                simpleUserLinksString.Append(string.Format(linkStyle, BbsRouter.GetUrl("space/" + moderator.UserID), moderator.User.Name, str) + separator);
            }
            if (simpleUserLinksString.Length > 0)
                simpleUserLinksString.Remove(simpleUserLinksString.Length - separator.Length, separator.Length);

            return simpleUserLinksString.ToString();
        }



        protected bool CanSeeLastUpdate(Forum forum)
        {
            if (MyUserID == 0 && forum.ForumSetting.AllowGuestVisitForum == false)
                return false;
            else if (MyUserID != 0 && false == forum.ForumSetting.VisitForum.GetValue(My))
                return false;
            if (forum.ForumType == ForumType.Normal && string.IsNullOrEmpty(forum.Password) == false && My.IsValidatedForum(forum) == false)
                return false;

            return true;
        }


        protected string GetUserLink(SimpleUser simpleUser)
        {
            return GetUserLink(simpleUser, null);
        }
        protected string GetUserLink(SimpleUser simpleUser, string linkStyle)
        {
            if (simpleUser == null || simpleUser.UserID < 1)
            {
                return "游客";
            }
            else
            {
                if (string.IsNullOrEmpty(linkStyle))
                    linkStyle = "<a href=\"{0}\"  target=\"_blank\">{1}</a>";
                return string.Format(linkStyle, BbsUrlHelper.GetUserSpaceUrl(simpleUser.UserID), simpleUser.Username);
            }
        }


        /*

        protected string ErrorMessage
        {
            get
            {
                MessageDisplay messageDisplay = MessageDisplay.GetFrom(null);
                if (messageDisplay == null)
                    return null;

                MessageDisplay.MessageCollection errors = messageDisplay.GetAllErrors();

                if (errors == null || errors.Count == 0)
                    return null;

                return errors[0].Message;
            }
        }

        protected string GetErrorMessage(string formName)
        {
            MessageDisplay messageDisplay = MessageDisplay.GetFrom(null);
            if (messageDisplay == null)
                return null;

            MessageDisplay.MessageItem item = messageDisplay.GetFirstError(formName);

            if (item == null)
                return null;

            return item.Message;
        }

        */


        protected string GetPoints(User user)
        {
            return GetPoints(user, "{0}：{1}{2}<br />");
        }
        protected string GetPoints(User user, string format)
        {
            return GetPoints(user, format, true);
        }
        protected string GetPoints(User user, string format, bool displayPointName)
        {
            UserPointCollection extendedPoints = AllSettings.Current.PointSettings.UserPoints;//PointManager.GetAllExtendedPoints();
            string points = string.Empty;
            for (int i = 0; i < user.ExtendedPoints.Length; i++)//i为0的 属于总积分 所以i从1开始
            {
                UserPoint extendedPoint = extendedPoints[i];

                if (extendedPoint != null && extendedPoint.Enable && extendedPoint.Display)
                    points += string.Format(format, displayPointName ? extendedPoint.Name : string.Empty, user.ExtendedPoints[i].ToString(), extendedPoint.UnitName);

            }
            return points;
        }

        #region  GetThreadLink

        protected string GetThreadLink(BasicThread thread, int subjectLength, int page, string type, string linkStyle, int listPage)
        {
            return GetThreadLink(linkStyle, thread, subjectLength, page, type, null, false, listPage, true);
        }
        protected string GetThreadLink(BasicThread thread, int subjectLength, int page, string type, string linkStyle)
        {
            return GetThreadLink(linkStyle, thread, subjectLength, page, type, null, false, 1, true);
        }
        protected string GetThreadLink(BasicThread thread, int subjectLength, int page, string type, string linkStyle, bool addStyle)
        {
            return GetThreadLink(linkStyle, thread, subjectLength, page, type, null, false, 1, addStyle);
        }
        protected string GetThreadLink(BasicThread thread, int subjectLength, string linkStyle, int listPage)
        {
            return GetThreadLink(linkStyle, thread, subjectLength, 1, null, null, false, listPage, true);
        }

        protected string GetThreadLink(BasicThread thread, int subjectLength, string linkStyle)
        {
            return GetThreadLink(thread, subjectLength, linkStyle, true);
        }

        protected string GetThreadLink(BasicThread thread, int subjectLength, string linkStyle, bool addStyle)
        {
            return GetThreadLink(linkStyle, thread, subjectLength, 1, null, null, false, 1, addStyle);
        }

        protected string GetThreadLinkForExtParms(BasicThread thread, int subjectLength, int page, bool newWindow, string extParms, string linkStyle)
        {
            return GetThreadLink(linkStyle, thread, subjectLength, page, extParms, null, true, 1, true);
        }
        protected string GetThreadLink(string linkStyle, BasicThread thread, int subjectLength, int page, string type, string codeName, bool typeIsExtParms, int listPage, bool addStyle)
        {
            if (thread == null || thread.ThreadID < 1)
                return "-";
            else
            {
                if (string.IsNullOrEmpty(linkStyle))
                {
                    linkStyle = "<a href=\"{0}\">{1}</a>";
                }

                string threadSubjectStyle;
                if (addStyle == false || string.IsNullOrEmpty(thread.SubjectStyle))
                    threadSubjectStyle = "<span>";
                else
                    threadSubjectStyle = "<span style=\"" + thread.SubjectStyle + "\">";


                string subject;
                if (thread.ThreadType == ThreadType.Move || thread.ThreadType == ThreadType.Join)
                {
                    int index = thread.Subject.IndexOf(',');
                    if (index > 0)
                    {
                        string threadIDStr = thread.Subject.Substring(0, index);

                        int threadID;
                        if (int.TryParse(threadIDStr, out threadID))
                        { 
                            BasicThread tempThread = PostBOV5.Instance.GetThread(threadID);
                            Forum forum = null;
                            if (tempThread != null)
                                forum = ForumBO.Instance.GetForum(tempThread.ForumID, false);

                            subject = thread.Subject.Substring(index + 1);

                            string threadTypeString;
                            if (thread.ThreadType == ThreadType.Move)
                                threadTypeString = "已移动: ";
                            else
                                threadTypeString = "已合并: ";

                            if (forum == null)
                                forum = ForumBO.Instance.GetForum(thread.ForumID);

                            string url;
                            if (typeIsExtParms)
                            {
                                url = BbsUrlHelper.GetThreadUrlForExtParms(forum.CodeName, threadID, page, type);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(type))
                                    url = BbsUrlHelper.GetThreadUrl(forum.CodeName, threadID, thread.ThreadTypeString, page, listPage);
                                else
                                    url = BbsUrlHelper.GetThreadUrl(forum.CodeName, threadID, thread.ThreadTypeString, page, type);

                            }
                            string cutSubject = CutString(threadTypeString + subject, subjectLength);

                            return string.Format(linkStyle, url, threadSubjectStyle + cutSubject + "</span>", subject);
                        }
                    }
                }

                subject = CutString(thread.Subject, subjectLength);
                Forum forum2 = ForumBO.Instance.GetForum(thread.ForumID);
                string endStatus;
                if (thread.ThreadType == ThreadType.Question)
                    endStatus = thread.IsClosed ? " [已解决]" : " [未解决]";
                else if (thread.ThreadType == ThreadType.Poll)
                    endStatus = thread.IsClosed ? " [已结束]" : " [投票中]";
                else
                    endStatus = string.Empty;

                string url2;
                string tempCodeName;
                if (string.IsNullOrEmpty(codeName))
                {
                    tempCodeName = forum2.CodeName;
                }
                else
                    tempCodeName = codeName;

                if (typeIsExtParms)
                {
                    url2 = BbsUrlHelper.GetThreadUrlForExtParms(tempCodeName, thread.ThreadID, thread.ThreadTypeString, page, type);
                }
                else
                {
                    if (string.IsNullOrEmpty(type))
                        url2 = BbsUrlHelper.GetThreadUrl(tempCodeName, thread.ThreadID, thread.ThreadTypeString, page, listPage);
                    else
                        url2 = BbsUrlHelper.GetThreadUrl(tempCodeName, thread.ThreadID, thread.ThreadTypeString, page, type);
                }

                return string.Format(linkStyle, url2, threadSubjectStyle + subject + "</span>", thread.Subject) + endStatus;
            }
        }

        #endregion

        private string rawUrl;
        protected string RawUrl
        {
            get
            {
                if (rawUrl == null)
                {
                    rawUrl = Request.RawUrl;
                    if (rawUrl == null)
                        rawUrl = string.Empty;
                    else
                        rawUrl = UrlUtil.SafeUrl(rawUrl);
                }
                return rawUrl;
            }
        }

        #region 在线 online

        protected string GetOnlineRoleImgs(string style)
        {
            if (string.IsNullOrEmpty(style))
                style = "<img src=\"{0}\" border=\"0\" alt=\"{1}\" />{1}&nbsp;&nbsp;";

            RoleInOnlineCollection roles = AllSettings.Current.OnlineSettings.RolesInOnline;

            StringBuilder logoUrlString = new StringBuilder();
            foreach (RoleInOnline rio in roles)
            {
                if (!string.IsNullOrEmpty(rio.LogoUrl))
                    logoUrlString.AppendFormat(style, rio.LogoUrl, rio.RoleName);
            }
            return logoUrlString.ToString();
        }

        private string everyoneIcon = null;
        protected string GetEveryoneIcon(string imgStyle)
        {
            if (everyoneIcon == null)
            {
                RoleInOnline roleInOnline = OnlineUserPool.Instance.GetEveryoneRole();
                if (roleInOnline != null)
                    everyoneIcon = string.Format(imgStyle, roleInOnline.LogoUrl, roleInOnline.RoleName);
                else
                    everyoneIcon = "";
            }
            return everyoneIcon;
        }

        private string guestIcon = null;
        protected string GetOnlineGuestIcon(string imgStyle)
        {
            if (guestIcon == null)
            {
                if (GuestRole == null)
                    guestIcon = string.Empty;
                else
                    guestIcon = string.Format(imgStyle, GuestRole.LogoUrl, GuestRole.RoleName);
            }

            return guestIcon;
        }

        private RoleInOnline m_guestRole;
        protected RoleInOnline GuestRole
        {
            get
            {
                if (m_guestRole == null)
                {
                    m_guestRole = OnlineUserPool.Instance.GetGuestRole();
                }
                return m_guestRole;
            }
        }

        private string roleScript;
        protected string RoleScript
        {
            get
            {
                if (roleScript == null)
                {
                    RoleInOnlineCollection rolesInOnline = AllSettings.Current.OnlineSettings.RolesInOnline;
                    StringBuilder sb = new StringBuilder();
                    foreach (RoleInOnline roleInOnline in rolesInOnline)
                    {
                        sb.Append("'" + roleInOnline.SortOrder + "':['" + clearnOutput(roleInOnline.LogoUrl) + "','" + clearnOutput(roleInOnline.RoleName) + "'],");
                    }
                    string str = "var roles = {";
                    if (sb.Length == 0)
                    {
                        str = str + "'0':['" + Role.Everyone.IconUrl + "','" + clearnOutput(Role.Everyone.Name) + "']";
                    }
                    else
                    {
                        str = str + sb.ToString(0, sb.Length - 1);
                    }
                    str = str + "};";
                    roleScript = str;
                }
                return roleScript;
            }
        }

        private string clearnOutput(string content)
        {
            if (content == null)
                return string.Empty;
            return content.Replace("'", "\'").Replace("\\", "\\\\");
        }

        protected string GetOnlineMemberScriptData(OnlineMemberCollection onlineMembers)
        {
            if (onlineMembers != null && onlineMembers.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                string formatString = "m({0},'{1}',{2},{3});";
                sb.Append(RoleScript);
                sb.Append(@"
function m(userID,nickName,roleSortOrder,IsInvisible)
{
    var logoUrl = roles[roleSortOrder][0];
    var roleName = roles[roleSortOrder][1];
    var tempHtml;
    if(IsInvisible) 
        tempHtml=memberHTML2;
    else
        tempHtml=memberHTML;
    document.write(tempHtml.replace(/\[userID\]/g,userID).replace('{logoUrl}',logoUrl).replace('{roleName}',roleName).replace('{nickName}',nickName));
}
function i()
{
    document.write(invisibleHTML);
}
");
                int maxCount = (OnlineSetting.ShowOnlineCount == 0 ? int.MaxValue : OnlineSetting.ShowOnlineCount);
                int count = 0;
                foreach (OnlineMember member in onlineMembers)
                {
                    bool showDetial = false;
                    if (member.IsInvisible)
                    {
                        if (AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.SeeInvisibleUserInfo, member.UserID))
                        {
                            showDetial = true;
                        }
                    }
                    else
                        showDetial = true;

                    if (showDetial)
                    {
                        sb.Append(string.Format(formatString, member.UserID, member.Username.Replace("\\", "\\\\").Replace("'", "\\'"), member.RoleSortOrder, member.IsInvisible.ToString().ToLower()));
                    }
                    else
                        sb.Append("i();");

                    count++;

                    if (count >= maxCount)
                        break;
                }

                return sb.ToString();
            }
            else
                return "";
        }

        protected string GetOnlineGuestScriptData(OnlineGuestCollection onlineGuests)
        {
            if (onlineGuests != null && onlineGuests.Count > 0)
            {
                int maxCount;
                if (OnlineSetting.ShowOnlineCount == 0)
                    maxCount = int.MaxValue;
                else
                {
                    maxCount = OnlineSetting.ShowOnlineCount - OnlineMemberList.Count;
                    if (maxCount < 1)
                        return string.Empty;
                }

                StringBuilder sb = new StringBuilder();
                string formatString = "g('{0}');";
                //sb.Append(RoleScript);
                sb.Append(@"
function g(guestID)
{
    document.write(guestHTML.replace('{guestID}',guestID));
}
");
                int count = 0;
                foreach (OnlineGuest guest in onlineGuests)
                {
                    sb.Append(string.Format(formatString, guest.GuestID));
                    count++;

                    if (count >= maxCount)
                        break;
                }

                return sb.ToString();
            }
            else
                return "";

        }











        protected virtual bool IsShowOnline
        {
            get
            {
                return OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow;// && OnlineSetting.OnlineGuestShow != OnlineShowType.NeverShow;
            }
        }

        protected bool IsShowOnlineDetail
        {
            get
            {
                if (OnlineSetting.OnlineMemberShow == OnlineShowType.OnlyShowCount || OnlineSetting.OnlineMemberShow == OnlineShowType.NeverShow)
                    return false;
                return OnlineSetting.ShowOnlineMemberNum == 0 || (OnlineMemberCount + OnlineGuestCount) <= OnlineSetting.ShowOnlineMemberNum;
            }
        }

        protected bool IsShowOnlineGuestData
        {
            get
            {
                if (DisplayGuest == DisplayStatus.Yes && OnlineSetting.OnlineMemberShow == OnlineShowType.ShowAll )// && (OnlineSetting.ShowOnlineGuestNum == 0 || OnlineGuestCount <= OnlineSetting.ShowOnlineGuestNum))
                {
                    return true;
                }
                else
                    return false;
            }
        }

        protected OnlineSettings OnlineSetting
        {
            get
            {
                return AllSettings.Current.OnlineSettings;
            }
        }

        private DisplayStatus? m_DisplayOnline = null;
        protected DisplayStatus DisplayOnline
        {
            get
            {
                if (m_DisplayOnline == null)
                {
                    m_DisplayOnline = _Request.Get<DisplayStatus>("displayOnline", Method.Get, DisplayStatus.Default);

                    if (m_DisplayOnline == DisplayStatus.Default)
                    {
                        if ((OnlineSetting.OnlineMemberShow == OnlineShowType.ShowAll
                            || OnlineSetting.OnlineMemberShow == OnlineShowType.ShowMember)
                            && OnlineSetting.DefaultOpen == false)
                            m_DisplayOnline = DisplayStatus.No;
                        else
                        {
                            if (DisplayGuest == DisplayStatus.Yes || DisplayMember == DisplayStatus.Yes)
                                m_DisplayOnline = DisplayStatus.Yes;
                        }
                    }


                    if (/*OnlineSetting.OnlineGuestShow == OnlineShowType.NeverShow &&*/ OnlineSetting.OnlineMemberShow == OnlineShowType.NeverShow)
                    {
                        m_DisplayOnline = DisplayStatus.No;
                    }
                }
                return m_DisplayOnline.Value;
            }
        }
        private DisplayStatus? m_DisplayMember = null;
        protected DisplayStatus DisplayMember
        {
            get
            {
                if (m_DisplayMember == null)
                {
                    m_DisplayMember = _Request.Get<DisplayStatus>("displayMember", Method.Get, DisplayStatus.Default);
                    if (OnlineSetting.OnlineMemberShow == OnlineShowType.NeverShow)
                    {
                        m_DisplayMember = DisplayStatus.No;
                    }
                    if (m_DisplayMember == DisplayStatus.Default)
                    {
                        if (OnlineSetting.OnlineMemberShow == OnlineShowType.OnlyShowCount)
                        {
                            m_DisplayMember = DisplayStatus.No;
                        }
                        else
                        {
                            m_DisplayMember = DisplayStatus.Yes;
                        }
                    }

                }
                return m_DisplayMember.Value;
            }
        }
        private DisplayStatus? m_DisplayGuest = null;
        protected DisplayStatus DisplayGuest
        {
            get
            {
                if (m_DisplayGuest == null)
                {
                    m_DisplayGuest = _Request.Get<DisplayStatus>("displayGuest", Method.Get, DisplayStatus.Default);
                    if (OnlineSetting.OnlineMemberShow == OnlineShowType.NeverShow)//(OnlineSetting.OnlineGuestShow == OnlineShowType.NeverShow)
                    {
                        m_DisplayGuest = DisplayStatus.No;
                    }
                    if (m_DisplayGuest == DisplayStatus.Default)
                    {
                        if (OnlineSetting.OnlineMemberShow == OnlineShowType.OnlyShowCount) //(OnlineSetting.OnlineGuestShow == OnlineShowType.NotShow)
                        {
                            m_DisplayGuest = DisplayStatus.No;
                        }
                        else
                        {
                            m_DisplayGuest = DisplayStatus.Yes;
                        }
                    }
                }
                return m_DisplayGuest.Value;
            }
        }


        private bool? m_HasMoreOnline;
        /// <summary>
        /// 是否显示更多在线 链接
        /// </summary>
        protected bool HasMoreOnline
        {
            get
            {
                if(m_HasMoreOnline == null)
                {
                    if (ShowOnlineCount == 0)
                        m_HasMoreOnline = false;
                    else
                    {
                        int maxCount = ShowOnlineCount;

                        int count = 0;
                        if (IsShowOnlineDetail)
                        {
                            count = OnlineMemberList.Count;
                            if (count > maxCount)
                                m_HasMoreOnline = true;
                        }

                        if (m_HasMoreOnline == null)
                        {
                            if (IsShowOnlineGuestData)
                            {
                                if (count + OnlineGuestList.Count > maxCount)
                                    m_HasMoreOnline = true;
                            }
                        }

                        if (m_HasMoreOnline == null)
                            m_HasMoreOnline = false;
                    }
                }

                return m_HasMoreOnline.Value;
            }
        }

        protected int ShowOnlineCount
        {
            get
            {
                return OnlineSetting.ShowOnlineCount;
            }
        }

        protected virtual OnlineMemberCollection OnlineMemberList
        {
            get
            {
                return null;
            }
        }


        private string m_OnlineMemberScriptData;
        protected string OnlineMemberScriptData
        {
            get
            {
                if (m_OnlineMemberScriptData == null)
                    m_OnlineMemberScriptData = GetOnlineMemberScriptData(OnlineMemberList);
                return m_OnlineMemberScriptData;
            }
        }

        protected virtual OnlineGuestCollection OnlineGuestList
        {
            get
            {
                return null;
            }
        }


        private string m_OnlineGuestScriptData;
        protected string OnlineGuestScriptData
        {
            get
            {
                if (m_OnlineGuestScriptData == null)
                    m_OnlineGuestScriptData = GetOnlineGuestScriptData(OnlineGuestList);
                return m_OnlineGuestScriptData;
            }
        }


        protected virtual bool IsForumPage
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// 在线用户的总数量（包括注册用户和游客）
        /// </summary>
        protected int TotalOnline
        {
            get
            {
                return OnlineMemberCount + OnlineGuestCount;
            }
        }

        private int? m_OnlineMemberCount;
        /// <summary>
        /// 在线的注册用户数量
        /// </summary>
        protected int OnlineMemberCount
        {
            get
            {
                if (m_OnlineMemberCount == null)
                {
                    if (OnlineMemberList != null)
                        m_OnlineMemberCount = OnlineMemberList.Count;
                    else
                        m_OnlineMemberCount = 0;
                }
                return m_OnlineMemberCount.Value;
            }
        }

        private int? m_OnlineGuestCount;
        /// <summary>
        /// 在线的游客的数量
        /// </summary>
        protected int OnlineGuestCount
        {
            get
            {
                if (m_OnlineGuestCount == null)
                {
                    if (OnlineGuestList != null)
                        m_OnlineGuestCount = OnlineGuestList.Count;
                    else
                        m_OnlineGuestCount = 0;
                }

                return m_OnlineGuestCount.Value;
            }
        }


        protected int ShowOnlineMemberNum
        {
            get
            {
                return OnlineSetting.ShowOnlineMemberNum;
            }
        }


        /// <summary>
        /// 历史最高在线数
        /// </summary>
        protected int MaxOnlineCount
        {
            get { return VarsManager.Stat.MaxOnlineCount; }
        }

        /// <summary>
        /// 历史最高在线数发生的时间
        /// </summary>
        protected DateTime MaxOnlineDate
        {
            get { return VarsManager.Stat.MaxOnlineDate; }
        }



        protected virtual int ForumOnlineMemberCount
        {
            get
            {
                return 0;
            }
        }
        protected virtual int ForumOnlineGuestCount
        {
            get
            {
                return 0;
            }
        }
        protected int ForumOnlineCount
        {
            get
            {
                return ForumOnlineMemberCount + ForumOnlineGuestCount;
            }
        }


        protected bool IsOnlyShowCount
        {
            get
            {
                return OnlineSetting.OnlineMemberShow == OnlineShowType.OnlyShowCount;
            }
        }

        #endregion

        #region BbsSettings

        protected BbsSettings BbsSettings
        {
            get { return AllSettings.Current.BbsSettings; }
        }

        protected bool DisplaySubforumsInIndexpage
        {
            get { return AllSettings.Current.BbsSettings.DisplaySubforumsInIndexpage; }
        }

        protected int HotThreadRequireReplies
        {
            get { return BbsSettings.HotThreadRequireReplies; }
        }

        #endregion

        #region GetThreadPager

        protected string GetThreadPager(BasicThread thread, string style, string urlStyle)
        {
            return GetThreadPager(thread, style, urlStyle, null, false, null, 1);
        }
        protected string GetThreadPager(BasicThread thread, string style, string urlStyle, string type, bool typeIsExtParms, string codeName, int listPage)
        {

            if (thread == null || thread.RedirectThreadID < 1)
                return string.Empty;

            if (thread.ThreadType == ThreadType.Polemize)
                return string.Empty;

            int totalPosts;

            if (type != null && SystemForum.UnapprovePosts.ToString().ToLower() == type.ToLower())
            {
                totalPosts = thread.UnApprovedPostsCount;
            }
            else
            {
                if (thread.ThreadType == ThreadType.Question && thread.IsClosed)////减掉最佳答案那个回复，最佳答案单独显示[这里有个问题，如果最佳答案被删除了，统计又与其他主题一样（未解决）]
                    totalPosts = thread.TotalReplies;
                else
                    totalPosts = thread.TotalReplies + 1;
            }

            int postsPageSize = AllSettings.Current.BbsSettings.PostsPageSize;
            int totalPage = totalPosts % postsPageSize == 0 ? totalPosts / postsPageSize : (totalPosts / postsPageSize + 1);

            if (totalPage < 1)
                return string.Empty;

            Forum forum = ForumBO.Instance.GetForum(thread.ForumID);
            StringBuilder sb = new StringBuilder();

            string url;
            if (string.IsNullOrEmpty(codeName))
            {
                if (typeIsExtParms)
                {
                    url = BbsUrlHelper.GetThreadUrlForPagerExtParms(forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString, type);
                }
                else
                {
                    if (string.IsNullOrEmpty(type))
                        url = BbsUrlHelper.GetThreadUrlForPager(forum.CodeName, thread.RedirectThreadID, listPage, thread.ThreadTypeString);
                    else
                        url = BbsUrlHelper.GetThreadUrlForPager(forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString, type);
                }
            }
            else
            {
                if (typeIsExtParms)
                {
                    url = BbsUrlHelper.GetThreadUrlForPagerExtParms(forum.CodeName, thread.RedirectThreadID, thread.ThreadTypeString, type);
                }
                else
                {
                    if (string.IsNullOrEmpty(type))
                        url = BbsUrlHelper.GetThreadUrlForPager(codeName, thread.RedirectThreadID, listPage, thread.ThreadTypeString);
                    else
                        url = BbsUrlHelper.GetThreadUrlForPager(codeName, thread.RedirectThreadID, thread.ThreadTypeString, type);
                }
            }

            if (totalPage > 1 && totalPage <= 5)
            {
                for (int i = 1; i < totalPage + 1; i++)
                {
                    //string url=UrlHelper.GetThreadUrl(forum.CodeName, thread.RedirectThreadID, i);
                    //if (!string.IsNullOrEmpty(urlParams))
                    //    url = url + (url.IndexOf('?') > 0 ? "&" : "?") + urlParams;

                    sb.Append(string.Format(urlStyle, string.Format(url, i), i));
                    if (i < totalPage)
                        sb.Append(" ");
                }
            }
            else if (totalPage > 5)
            {
                for (int i = 1; i < totalPage + 1; i++)
                {
                    if (i < 3 || i > totalPage - 3)
                    {

                        sb.Append(string.Format(urlStyle, string.Format(url, i), i) + " ");
                    }
                    else if (i == 3)
                        sb.Append("... ");
                }
            }
            if (string.IsNullOrEmpty(style))
                return sb.ToString();
            else
            {
                if (sb.Length > 0)
                    return string.Format(style, sb.ToString());
                else
                    return "";
            }
        }

        #endregion



        protected virtual bool ShowLoginDialog
        {
            get
            {
                return false;
            }
        }
    }
}