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
using System.Web.UI;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using System.Text;
using System.Data;
using MaxLabs.bbsMax.ValidateCodes;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Errors;
namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    public partial class _Default : AppBbsPageBase
    {
        //最新贴或热门贴分页大小
        private const int topThreadPageSize = 8;

        //最新注册用户分页大小
        private const int recentRegUsersPageSize = 9;

        protected void Page_Load(object sender, EventArgs e)
        {
            //关闭侧边栏
            if (_Request.IsClick("Default_Close_Sidebar"))
            {
                ProcessUpdateUserOption(true);
            }
            //开启侧边栏
            if (_Request.IsClick("Default_Open_Sidebar"))
            {
                ProcessUpdateUserOption(false);
            }

            //快速登录
            if (_Request.IsClick("btLogin"))
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                using (ErrorScope es = new ErrorScope())
                {
                    if (CheckValidateCode("login", msgDisplay))
                    {
                        ValidateCodeManager.CreateValidateCodeActionRecode("login");
                        string username = _Request.Get("username", Method.Post, string.Empty, false);
                        string password = _Request.Get("password", Method.Post, string.Empty, false);

                        //如果全局UserLoginType为Username -或者- 后台设置全局UserLoginType为All且用户选择了账号登陆  则为true
                        UserLoginType loginType = _Request.Get<UserLoginType>("logintype", Method.Post, UserLoginType.Username);
                        bool isUsernameLogin = (LoginType == UserLoginType.Username || (LoginType == UserLoginType.All && loginType == UserLoginType.Username));

                        int cookieTime = _Request.Get<int>("cookietime", Method.Post, 0);
                        bool success;

                        try
                        {
                            success = UserBO.Instance.Login(username, password, _Request.IpAddress, cookieTime > 0, isUsernameLogin);
                            if (success == false)
                            {
                                if (es.HasUnCatchedError)
                                {
                                    es.CatchError<UserNotActivedError>(delegate(UserNotActivedError err)
                                    {
                                        Response.Redirect(err.ActiveUrl);
                                    });
                                    es.CatchError<EmailNotValidatedError>(delegate(EmailNotValidatedError err)
                                    {
                                        Response.Redirect(err.ValidateUrl);
                                    });
                                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                                    {
                                        msgDisplay.AddError(error);
                                    });
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            msgDisplay.AddException(ex);
                            success = false;
                        }

                        if (success)
                        {
                            BbsRouter.JumpToCurrentUrl();
                        }
                    }
                }
            }
            else
            {
                UpdateOnlineStatus(OnlineAction.ViewIndexPage, 0, "");

                //OnlineManager.UpdateOnlineUser(MyUserID, 0, 0, My.OnlineStatus, OnlineAction.ViewIndexPage, Request, Response);

                AddNavigationItem("欢迎光临，现在是" + UserNow);
            }

            ForumCollection tempForums = new ForumCollection();
            foreach (Forum forum in ForumCatalogs)
            {
                WaitForFillSimpleUsers<Moderator>(forum.Moderators);
                foreach (Forum subForum in forum.SubForumsForList)
                {
                    WaitForFillSimpleUsers<Moderator>(subForum.Moderators);
                }
                tempForums.AddRange(forum.SubForumsForList);
            }

            ForumBO.Instance.SetForumsLastThread(tempForums);

            SubmitFillUsers();
        }

        protected override string NavigationKey
        {
            get { return "index"; }
        }

        protected override string PageName
        {
            get { return "default"; }
        }

        public UserLoginType LoginType
        {
            get { return AllSettings.Current.LoginSettings.LoginType; }
        }

        /// <summary>
        /// 更新用户侧边栏选项
        /// </summary>
        private void ProcessUpdateUserOption(bool isClose)
        {
            UserBO.Instance.SetSidebarStatus(My, (isClose ? EnableStatus.Disabled : EnableStatus.Enabled));
        }

        protected override bool NeedProcessOutput
        {
            get { return false; }
        }

        protected override bool HasSideBar
        {
            get { return true; }
        }

        #region 侧边栏 的新主题

        private ThreadCollectionV5 m_ValuedThreads;
        protected ThreadCollectionV5 GetValuedThreads(int count, int days)
        {
            if (m_ValuedThreads == null)
            {
                m_ValuedThreads = PostBOV5.Instance.GetValuedThreads(My, 1, count, days, ThreadSortField.CreateDate, true);
                PostBOV5.Instance.ProcessKeyword(m_ValuedThreads, ProcessKeywordMode.TryUpdateKeyword);
            }
            return m_ValuedThreads;
        }
        /*
        private ThreadCollectionV5 m_HotThreads;
        protected ThreadCollectionV5 GetHotThreads(int count, int days)
        {
            if (m_HotThreads == null)
            {
                m_HotThreads = PostBOV5.Instance.GetHotThreads(My, null, 1, count, days, ThreadSortField.Replies, true);
                PostBOV5.Instance.ProcessKeyword(m_HotThreads, ProcessKeywordMode.TryUpdateKeyword);
            }
            return m_HotThreads;
        }
        */
        private ThreadCollectionV5 m_NewThreads;
        protected ThreadCollectionV5 GetNewThreads(int count)
        {
            if (null == m_NewThreads)
            {
                int total;
                m_NewThreads = PostBOV5.Instance.GetNewThreads(My, null, 1, count, out total);
                PostBOV5.Instance.ProcessKeyword(m_NewThreads, ProcessKeywordMode.TryUpdateKeyword);
            }
            return m_NewThreads;
        }

        #endregion

        #region 首页统计变量

        private string m_TopThreadType;
        protected string TopThreadType
        {
            get
            {
                if (m_TopThreadType == null)
                    m_TopThreadType = _Request.Get("topthreadtype", Method.Get, "recent");
                return m_TopThreadType;
            }
        }

        //private int? m_TotalThreads;
        //protected int TotalThreads
        //{
        //    get
        //    {
        //        if (m_TotalThreads == null)
        //        {
        //            SetStats();
        //        }
        //        return m_TotalThreads.Value;
        //    }
        //}
        //private int? m_TotalPosts;
        //protected int TotalPosts
        //{
        //    get
        //    {
        //        if (m_TotalPosts == null)
        //        {
        //            SetStats();
        //        }
        //        return m_TotalPosts.Value;
        //    }
        //}
        //private int? m_TodayPosts;
        //protected int TodayPosts
        //{
        //    get
        //    {
        //        if (m_TodayPosts == null)
        //        {
        //            SetStats();
        //        }
        //        return m_TodayPosts.Value;
        //    }
        //}

        //private void SetStats()
        //{
        //    int totalThreads = 0;
        //    int totalPosts = 0;
        //    int todayPosts = 0;
        //    foreach (Forum forum in ForumBO.Instance.GetAllForums())
        //    {
        //        if (forum.ForumID > 0)
        //        {
        //            totalThreads += forum.TotalThreads;
        //            totalPosts += forum.TotalPosts;
        //            todayPosts += forum.TodayPosts;
        //        }
        //    }

        //    m_TotalPosts = totalPosts;
        //    m_TodayPosts = todayPosts;
        //    m_TotalThreads = totalThreads;

        //}

        //protected int TotalUsers
        //{
        //    get
        //    {
        //        return VarsManager.Stat.TotalUsers;
        //    }
        //}
        //protected string NewUser
        //{
        //    get
        //    {
        //        return VarsManager.Stat.NewUsername;
        //    }
        //}
        //protected int NewUserID
        //{
        //    get
        //    {
        //        return VarsManager.Stat.NewUserID;
        //    }
        //}

        //protected int YestodayPosts
        //{
        //    get
        //    {
        //        return VarsManager.Stat.YestodayPosts;
        //    }
        //}
        //private int? m_DayMaxPosts;
        //protected int DayMaxPosts
        //{
        //    get
        //    {
        //        if (m_DayMaxPosts == null)
        //        {
        //            m_DayMaxPosts = VarsManager.Stat.MaxPosts;

        //            if (TodayPosts > m_DayMaxPosts.Value)
        //            {
        //                m_DayMaxPosts = TodayPosts;
        //            }
        //        }
        //        return m_DayMaxPosts.Value;
        //    }
        //}
        #endregion

        //所有图片链接
        protected LinkCollection ImgLinks
        {
            get { return AllSettings.Current.LinkSettings.ImageLinks; }
        }

        //所有文字链接
        protected LinkCollection TextLinks
        {
            get { return AllSettings.Current.LinkSettings.TextLinks; }
        }

        #region  online


        //在线会员
        private OnlineMemberCollection m_OnlineMemberList;
        protected override OnlineMemberCollection OnlineMemberList
        {
            get
            {
                OnlineMemberCollection onlineMemberList = m_OnlineMemberList;

                if (onlineMemberList == null)
                {
                    if (OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow)
                    {
                        onlineMemberList = OnlineUserPool.Instance.GetAllOnlineMembers();
                        m_OnlineMemberList = onlineMemberList;
                    }
                }
                return onlineMemberList;
            }
        }



        //在线游客
        private OnlineGuestCollection m_OnlineGuestList;
        protected override OnlineGuestCollection OnlineGuestList
        {
            get
            {
                OnlineGuestCollection onlineGuestList = m_OnlineGuestList;

                if (onlineGuestList == null)
                {
                    if (OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow)
                    {
                        onlineGuestList = OnlineUserPool.Instance.GetAllOnlineGuests();
                        m_OnlineGuestList = onlineGuestList;
                    }
                }
                return onlineGuestList;
            }
        }




        #endregion
    }
}