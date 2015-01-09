//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Settings;
using System.Text;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class members : BbsPageBase
    {
        protected override string PageTitle
        {
            get
            {
                if (View == "onlineuser")
                    return "在线会员 - " + base.PageTitle;

                else if (View == "onlineguest")
                    return "在线游客 - " + base.PageTitle;

                else
                    return "会员 - " + base.PageTitle;
            }
        }

        protected override string PageName
        {
            get { return "members"; }
        }

        protected override string NavigationKey
        {
            get { return "members"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            OnlineUserPool.Instance.Update(My, _Request, OnlineAction.OtherAction, 0, 0, "");

            processView();
            AddNavigationItem("会员");

            if (_Request.IsClick("searchuser"))
            {
                Search();
            }
            else if (_Request.IsClick("addPointShow"))
            {
                CreatePointShow();
            }
            else if (_Request.IsClick("updatepointshow"))
            {
                UpdatePointShowInfo();
            }

            if (View != "onlineuser" && View != "onlineguest")
                SetPager("list", null, PageNumber, PageSize, TotalCount);

            
        }

        protected UserPointCollection EnabledPoints
        {
            get
            {
                return AllSettings.Current.PointSettings.EnabledUserPoints;
            }
        }

        protected bool InBirthday( User user )
        {
            int m1,m2, y ;
            DateTime d1;
            
            m1 = user.Birthday.Month;
            m2= DateTimeUtil.Now.Month;

            y=DateTimeUtil.Now.Year;

            if (Math.Abs(m1 - m2) == 11)
            {
                if (m1 == 1)
                {
                    y++;
                }
                else if (m2 == 1)
                { 
                    y--;
                }
            }
            d1 = new DateTime(y, user.Birthday.Month, user.Birthday.Day);

            return Math.Abs((d1 - DateTimeUtil.Now).Days) <= 3;
        }
             
        protected bool HasBirthday( User user )
        {
            if ( DateTimeUtil.Now.Year-user.Birthday.Year > 100)
                return false;
            return true;
        }

        protected int GetUserPointValue(User user, UserPoint up)
        {
            return user.ExtendedPoints[(int)up.Type];
        }
       

        protected UserOrderBy UserOrderField { get; set; }


        protected bool IsSortByPoint(UserPointType type)
        {
            return SortField.ToString() == type.ToString();
        }

        protected bool ShowSearchForm
        {
            get
            {
                return !string.IsNullOrEmpty(_Request.Get("filter", Method.Get));
            }
        }

        int m_index = 1;
        protected int Index
        {
            get
            {
                int k = (this.PageNumber - 1) * this.PageSize;
                return k+m_index++;
            }
        }

        private void processView()
        {
            switch (View)
            {
                case  "show":
                    break;
                case "search":
                    break;
                case "onlineuser":
                    break;
                case "online":
                    break;
                case "onlineguest":
                    break;
                case "friendnumber":
                    SortField = UserOrderBy.TotalFriends;
                    break;
                case "onlinetime":
                    this.SortField = UserOrderBy.TotalOnlineTime;
                    break;
                case "viewnumber":
                    this.SortField = UserOrderBy.TotalViews;
                    break;
                case "female":
                    this.SortField = UserOrderBy.TotalViews;
                    this.Filter.Gender = Gender.Female;
                    break;
                case "male":
                    this.Filter.Gender = Gender.Male;
                    this.SortField = UserOrderBy.TotalViews;
                    break;
                case "point":
                    this.SortField = UserOrderBy.Points;
                    break;
                case "postcount":
                    this.SortField = UserOrderBy.TotalPosts;
                    break;
            }
            UserOrderBy order = _Request.Get<UserOrderBy>("sort",Method.Get, UserOrderBy.UserID);
            if (order != UserOrderBy.UserID) SortField = order;
        }

        #region 竞价排名相关

        private PointShowSettings settings = AllSettings.Current.PointShowSettings;
        protected int MinShowPoint
        {
            get
            {
                return settings.MinPrice < settings.PointBalance ? settings.PointBalance : settings.MinPrice;
            }
        }

        /// <summary>
        /// 当前用户是否是竞价用户
        /// </summary>
        private bool? m_IsPointShowUser;
        protected bool IsPointShowUser
        {
            get
            {
                if (m_IsPointShowUser==null)
                {
                    m_IsPointShowUser = PointShowBO.Instance.IsPointShowUser(MyUserID);
                }

                return m_IsPointShowUser.Value;
            }
        }

        /// <summary>
        /// 竞价排名所用积分类型
        /// </summary>
        protected string AddPointName
        {
            get
            {
                return AddPoint.Name;
            }
        }
        private UserPoint m_AddPoint;
        protected UserPoint AddPoint
        {
            get
            {
                if (m_AddPoint == null)
                {
                    m_AddPoint = AllSettings.Current.PointSettings.GetUserPoint(AllSettings.Current.PointShowSettings.UsePointType);
                }
                return m_AddPoint;
            }
        }

        /// <summary>
        /// 竞价排名点击链接
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        protected string GetPointShowSpaceLink(int userID)
        {
            return BbsRouter.GetUrl("space/" + userID, "source=show");
        }

        private PointShow m_myPointShowInfo = null;
        protected PointShow MyPointShowInfo
        {
            get
            {
                if (m_myPointShowInfo == null)
                {
                    m_myPointShowInfo = PointShowBO.Instance.GetMyPointShowInfo(MyUserID);
                }
                return m_myPointShowInfo;
            }
        }

        private void CreatePointShow()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("price","pointcount");

            string content = _Request.Get("pointshowOath", Method.Post, string.Empty);
            int point = _Request.Get<int>("pointcount", Method.Post, 0);
            int price = _Request.Get<int>("price", Method.Post, 0);

            using (ErrorScope es = new ErrorScope())
            {
                bool success = false;
                
                try
                {
                    success = PointShowBO.Instance.CreatePointShow(My, point, price, content, out m_myPointShowInfo);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (!success)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        if (error is UserPointOverMinValueError)
                        {
                            UserPointOverMinValueError e = (UserPointOverMinValueError)error;
                            msgDisplay.AddError(error.TatgetName, "您的积分不足，最多只能再增加竞价" + e.CanReduceValue);
                        }
                        else if (error is UserPointTradeMinValueError)
                        {
                            UserPointTradeMinValueError e = (UserPointTradeMinValueError)error;
                            msgDisplay.AddError(error.TatgetName, "竞价最少必须增加" + e.MinValue);
                        }
                        else if (error is UserPointTradeMaxValueError)
                        {
                            UserPointTradeMaxValueError e = (UserPointTradeMaxValueError)error;
                            msgDisplay.AddError(error.TatgetName, "竞价最多只能增加" + e.MaxValue);
                        }
                        else if (error is UserPointTradeRemainingError)
                        {
                            UserPointTradeRemainingError e = (UserPointTradeRemainingError)error;
                            msgDisplay.AddError(error.TatgetName, "您的积分不足，增加竞价后，您的" + e.PointName + "小于系统允许的最小剩余值：" + e.MinRemainingValue);
                        }
                        else
                            msgDisplay.AddError(error);
                    });

                    this.m_IsPointShowUser = false;
                }
                else
                {
                    this.m_IsPointShowUser = true;
                    ShowSuccess(false, string.Format("上榜成功， 您当前的排名是：<font color=\"red\">{0}</font>", m_myPointShowInfo.Rank));
                    //BbsRouter.JumpToCurrentUrl("page=1&succeed=1");
                }
            }
        }

        private void UpdatePointShowInfo()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            PointShowBO.Instance.UpdatePointShow(My
                , _Request.Get<int>("pointadd", Method.Post, 0)
                , _Request.Get<int>("price", Method.Post, 0)
                , _Request.Get("pointshowOath", Method.Post),out m_myPointShowInfo);

            if (HasUnCatchedError)
            {
                CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    msgDisplay.AddError(error);
                });
            }
            else
            {

                ShowSuccess(false, string.Format("更新成功， 您当前的排名是：<font color=\"red\">{0}</font>， 您的账户还有<font color=\"red\">{1}</font>{2}", m_myPointShowInfo.Rank, m_myPointShowInfo.ShowPoints, AddPointName));
            }
        }

        #endregion

        Dictionary<string, int> m_rankinfos;
        public Dictionary<string, int> RankInfos
        {
            get
            {
                if (m_rankinfos==null)
                {
                    string[] rankField=null;

                    if (View == "point")
                    {
                        UserPointCollection enbaledPoints = AllSettings.Current.PointSettings.EnabledUserPoints;

                        rankField = new string[4 + enbaledPoints.Count];
                        int i = 0;
                        foreach (UserPoint p in EnabledPoints)
                        {
                            rankField[i] = "Point_" + ((int)p.Type+1);
                            i++;
                        }
                        rankField[i++] = "Points";
                        rankField[i++] = "TotalPosts";
                        rankField[i++] = "TotalOnlineTime";
                        rankField[i++] = "MonthOnlineTime";
                    }
                    else if (View == "viewnumber" ||View=="female"||View=="male")
                    {
                        rankField = new string[] { "TotalFriends", "SpaceViews" };
                    }
                    m_rankinfos = UserBO.Instance.GetUserRankInfo(MyUserID, rankField);
                }

                return m_rankinfos;
            }
        }

        protected int GetPointRank(  UserPoint up)
        {
            return RankInfos["Point_" + (((int)up.Type) + 1)];
        }

        private void Search()
        {
            m_IsSearch = true;
            UserFilter filter = UserFilter.GetFromForm();
            filter.Pagesize = PageSize;

            filter.Apply("filter", "page");
        }

        private UserFilter m_Filter;

        public UserFilter Filter
        {
            get
            {
                if (m_Filter == null)
                {
                    m_Filter = UserFilter.GetFromFilter("filter");

                    if (m_Filter.IsNull)
                    {
                        m_Filter = new UserFilter();


                        m_Filter.Pagesize = PageSize;
                    }
                    else
                        m_IsSearch = true;
                }
                return m_Filter;
            }
        }

        protected int PageSize
        {
            get
            {
                return 20;
            }
        }

        private bool? m_IsPointShow;
        public bool IsPointShow
        {
            get
            {
                if (m_IsPointShow == null)
                {
                    if (IsSearch)
                        m_IsPointShow = false;
                    else if (View == "show")
                    {
                        m_IsPointShow = true;
                    }
                    else
                        m_IsPointShow = false;
                }
                return m_IsPointShow.Value;
            }
        }

        private UserOrderBy? m_SortField;
        protected UserOrderBy SortField
        {
            get
            {
                if (m_SortField == null)
                {
                    m_SortField = UserOrderBy.UserID;
                }
                return m_SortField.Value;
            }
            set
            {
                m_SortField = value;
            }
        }

        private bool m_IsSearch = false;
        protected bool IsSearch
        {
            get
            {
                return View == "search";
            }
        }

        /// <summary>
        /// 获取当前的视图类型
        /// </summary>

        private string m_View = null;
        protected string View
        {
            get
            {
                if (m_View == null)
                {
                    m_View = _Request.Get("view", Method.Get, "show");
                }
                return m_View;
            }
        }

        protected string GetCurrentClass(string type,string className)
        {
            
            if(IsSearch)
                return string.Empty;
            if (type.ToLower() == "show")
            {
                if (IsPointShow)
                    return className;
                else
                    return string.Empty;
            }
            else
            {
                if (IsPointShow)
                    return string.Empty;
                if (type==View)
                    return className;
                else
                    return string.Empty;
            }
        }

        protected int GetUserShowPoint(int userID)
        {
            if (ShowPoints.ContainsKey(userID))
                return ShowPoints[userID];
            else
                return 0;
        }
        protected string GetUserShowContent(int userID)
        {
            if (ShowContents.ContainsKey(userID))
                return ShowContents[userID];
            else
                return string.Empty;
        }

        private UserCollection m_Users;
        protected UserCollection UserList
        {
            get
            {
                if (m_Users == null)
                {
                    GetData();
                }
                return m_Users;
            }
        }

        private void GetData()
        {
            if (IsSearch)
            {
                m_Users = UserBO.Instance.GetUsers(Filter, PageNumber);
            }
            else
            {
                if (IsPointShow)
                {
                    m_Users = PointShowBO.Instance.GetUserShows(MyUserID, PageNumber, PageSize , out m_ShowPoints, out m_ShowContents);
                }
                else
                {
                    Filter.Order = SortField;
                    Filter.Pagesize = PageSize;
                    m_Users = UserBO.Instance.GetUsers(Filter,PageNumber);
                }

            }
            m_TotalCount = m_Users.TotalRecords;
        }

        private int? m_TotalCount;
        protected int TotalCount
        {
            get
            {
                if (m_TotalCount == null)
                {
                    GetData();
                }
                return m_TotalCount.Value;
            }
        }

        protected string Headtags = string.Empty;
        protected string RawUrl = string.Empty;


        private Dictionary<int, int> m_ShowPoints;
        protected Dictionary<int, int> ShowPoints
        {
            get
            {
                if (m_ShowPoints == null)
                {
                    GetData();
                }
                return m_ShowPoints;
            }
        }

        private Dictionary<int, string> m_ShowContents;
        protected Dictionary<int, string> ShowContents
        {
            get
            {
                if (m_ShowContents == null)
                {
                    GetData();
                }
                return m_ShowContents;
            }
        }


        protected int PageNumber
        {
            get
            {
                return _Request.Get<int>("Page", Method.Get, 1);
            }
        }

        protected bool IsFriend(int userID)
        {
            return FriendBO.Instance.IsFriend(MyUserID,userID);
        }

        #region  在线列表

        protected bool IsShowOnlineMember
        {
            get
            {
                return AllSettings.Current.OnlineSettings.OnlineMemberShow != OnlineShowType.NeverShow;
            }
        }
        protected bool IsShowOnlineGuest
        {
            get
            {
                return AllSettings.Current.OnlineSettings.OnlineMemberShow != OnlineShowType.NeverShow;
            }
        }

        protected bool IsShowOnline
        {
            get
            {
                return IsShowOnlineMember || IsShowOnlineGuest;
            }
        }

        private OnlineMemberCollection m_OnlineMembers;
        protected OnlineMemberCollection OnlineMembers
        {
            get
            {
                if (m_OnlineMembers == null)
                {
                    m_OnlineMembers = new OnlineMemberCollection();

                    if (IsShowOnlineMember)
                    {
                        OnlineMemberCollection allOnlineMembers = OnlineUserPool.Instance.GetAllOnlineMembers();

                        for (int i = (PageNumber - 1) * PageSize; i < PageNumber * PageSize; i++)
                        {
                            if (i >= allOnlineMembers.Count)
                                break;

                            m_OnlineMembers.Add(allOnlineMembers[i]);
                        }

                        if (m_OnlineMembers.Count > 0)
                            FillSimpleUsers<OnlineMember>(m_OnlineMembers);

                        m_TotalCount = allOnlineMembers.Count;
                        SetPager("list", null, PageNumber, PageSize, allOnlineMembers.Count);
                    }


                }

                return m_OnlineMembers;
            }
        }

        protected bool CanSeeInvisibleUser(int userID)
        {
            return AllSettings.Current.ManageUserPermissionSet.Can(My, ManageUserPermissionSet.ActionWithTarget.SeeInvisibleUserInfo, userID);
        }

        protected bool DisplayUserInfo(OnlineMember member)
        {
            if (member.IsInvisible == false)
                return true;

            if (CanSeeInvisibleUser(member.UserID))
                return true;

            
            return false;
        }

        protected string GetMemberPosition(OnlineMember member)
        {
            return GetPosition<int>(member);
        }

        protected string GetPosition<T>(OnlineUser<T> onlineUser) where T : IComparable<T>
        {
            StringBuilder sb = new StringBuilder();
            if (onlineUser.ForumID > 0)
            {
                Forum forum = ForumBO.Instance.GetForum(onlineUser.ForumID);
                if (forum != null)
                {
                    if (forum.CanDisplayInList(My))
                    {
                        sb.AppendFormat(@"<a href=""{0}"" target=""_blank"">{1}</a>", BbsUrlHelper.GetForumUrl(forum.CodeName), forum.ForumNameText);

                        if (onlineUser.ThreadID > 0 && forum.CanVisit(My))
                        {
                            sb.AppendFormat(@" - <a href=""{0}"" target=""_blank"">{1}</a>", BbsUrlHelper.GetThreadUrl(forum.CodeName, onlineUser.ThreadID), onlineUser.ThreadSubject);
                        }
                    }
                }
            }

            return sb.ToString();
        }


        protected string GetIpArea(string ip)
        {
            return IPUtil.GetIpArea(ip);
        }


        private Dictionary<int, RoleInOnline> RoleInOnlines = null;
        protected RoleInOnline GetRoleInOnline(int sortOrder)
        {
            if (RoleInOnlines == null)
            {
                RoleInOnlines = new Dictionary<int, RoleInOnline>();

                foreach (RoleInOnline role in AllSettings.Current.OnlineSettings.RolesInOnline)
                {
                    if (RoleInOnlines.ContainsKey(role.SortOrder) == false)
                        RoleInOnlines.Add(role.SortOrder,role);
                }
            }

            RoleInOnline tempRole;

            RoleInOnlines.TryGetValue(sortOrder, out tempRole);

            return tempRole;
        }

        private string m_EveryoneRoleLogoUrl;
        protected string EveryoneRoleLogoUrl
        {
            get
            {
                if (m_EveryoneRoleLogoUrl == null)
                {
                    RoleInOnline role = OnlineUserPool.Instance.GetEveryoneRole();
                    if (role != null)
                        m_EveryoneRoleLogoUrl = role.LogoUrl;
                    else
                        m_EveryoneRoleLogoUrl = string.Empty;
                }

                return m_EveryoneRoleLogoUrl;
            }
        }

        protected string GetRoleLogoUrl(OnlineMember member)
        {
            if (DisplayUserInfo(member) == false)
            {
                return EveryoneRoleLogoUrl;
            }

            RoleInOnline role = GetRoleInOnline(member.RoleSortOrder);
            if (role == null)
                return string.Empty;
            else
                return role.LogoUrl;
        }

        private OnlineGuestCollection m_OnlineGuests;
        protected OnlineGuestCollection OnlineGuests
        {
            get
            {
                if (m_OnlineGuests == null)
                {
                    m_OnlineGuests = new OnlineGuestCollection();
                    OnlineGuestCollection onlineGuests;
                    if (IsShowOnlineGuest == false)
                        onlineGuests = new OnlineGuestCollection();
                    else
                        onlineGuests = OnlineUserPool.Instance.GetAllOnlineGuests();

                    for (int i = (PageNumber - 1) * PageSize; i < PageNumber * PageSize; i++)
                    {
                        if (i >= onlineGuests.Count)
                            break;

                        m_OnlineGuests.Add(onlineGuests[i]);
                    }


                    m_TotalCount = onlineGuests.Count;
                    SetPager("list", null, PageNumber, PageSize, TotalCount);
                }

                return m_OnlineGuests;
            }
        }

        private string m_GuestRoleLogoUrl;
        protected string GuestRoleLogoUrl
        {
            get
            {
                if (m_GuestRoleLogoUrl == null)
                {
                    RoleInOnline role = OnlineUserPool.Instance.GetGuestRole();
                    if (role != null)
                        m_GuestRoleLogoUrl = role.LogoUrl;
                    else
                        m_GuestRoleLogoUrl = string.Empty;
                }

                return m_GuestRoleLogoUrl;
            }
        }

        protected string GetGuestPosition(OnlineGuest guest)
        {
            return GetPosition<string>(guest);
        }
        #endregion


    }
}