//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 论坛版块信息
    /// </summary>
    public class Forum : ICloneable<Forum>, IPrimaryKey<int>, IParentable<Forum>
    {
        public Forum()
        {
            ExtendedAttribute = new ForumExtendedAttribute();
        }

        public Forum(DataReaderWrap readerWrap)
        {
            ExtendedAttribute = new ForumExtendedAttribute();

            #region 填充论坛实体

            this.ForumID = readerWrap.Get<int>("ForumID");
            this.ForumName = readerWrap.Get<string>("ForumName");
            this.ParentID = readerWrap.Get<int>("ParentID");
            this.ForumStatus = readerWrap.Get<ForumStatus>("ForumStatus");
            if (this.ParentID == 0)
                this.ForumType = Enums.ForumType.Catalog;
            else
                this.ForumType = readerWrap.Get<ForumType>("ForumType");//(ForumType)Convert.ToInt32(reader["ForumType"]);
            this.ThreadCatalogStatus = readerWrap.Get<ThreadCatalogStatus>("ThreadCatalogStatus");//(ThreadCatalogStatus)Convert.ToInt32(reader["ThreadCatalogStatus"]);

            this.VisitPaths = StringList.Parse(readerWrap.Get<string>("VisitPaths"));

            this.CodeName = readerWrap.Get<string>("CodeName");

            this.Description = readerWrap.Get<string>("Description");
            this.Readme = readerWrap.Get<string>("Readme");

            this.LogoSrc = readerWrap.Get<string>("LogoSrc");
            this.ThemeID = readerWrap.Get<string>("ThemeID");

            this.TotalThreads = readerWrap.Get<int>("TotalThreads");
            this.TotalPosts = readerWrap.Get<int>("TotalPosts");
            this.TodayThreads = readerWrap.Get<int>("TodayThreads");
            this.TodayPosts = readerWrap.Get<int>("TodayPosts");

            this.LastThreadID = readerWrap.Get<int>("LastThreadID");

            this.Password = readerWrap.Get<string>("Password");

            this.ColumnSpan = (int)readerWrap.Get<byte>("ColumnSpan");
            this.SortOrder = readerWrap.Get<int>("SortOrder");

            ClubID = readerWrap.Get<int>("ClubID");

            string extendedAttributesString = readerWrap.Get<string>("ExtendedAttributes");

            if (!string.IsNullOrEmpty(extendedAttributesString))
            {
                ExtendedAttribute.Parse(extendedAttributesString);
            }


            #endregion
        }

        private string m_ForumName;
        private string m_ForumNameText;
        private int totalThreads;
        private int totalPosts;
        private int todayThreads;
        private int todayPosts;
        private int lastThreadID;

        [Obsolete]
        public int ID { get { return ForumID; } }

        [Obsolete]
        public string Name { get { return ForumName; } }

        /// <summary>
        /// 论坛ID
        /// </summary>
        public int ForumID { get; set; }

        public int ClubID { get; set; }

        /// <summary>
        /// 论坛名
        /// </summary>
        public string ForumName
        {
            get { return m_ForumName; }
            set
            {
                m_ForumNameText = null;
                m_ForumName = value;
            }
        }

        /// <summary>
        /// 已经清理了html的、纯文本的论坛名
        /// </summary>
        public string ForumNameText
        {
            get
            {
                string forumNameText = m_ForumNameText;

                if (forumNameText == null)
                {
                    forumNameText = StringUtil.ClearAngleBracket(ForumName);
                    forumNameText = StringUtil.HtmlEncode(StringUtil.HtmlDecode(forumNameText));
                    m_ForumNameText = forumNameText;
                }

                return forumNameText;
            }
        }

        /// <summary>
        /// 上级论坛ID
        /// </summary>
        public int ParentID { get; private set; }

        /// <summary>
        /// 论坛状态
        /// </summary>
        public ForumStatus ForumStatus { get; private set; }

        /// <summary>
        /// 论坛类型
        /// </summary>
        public ForumType ForumType { get; private set; }

        /// <summary>
        /// 论坛内部名，用于url中的显示
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// bbsmax.bbsmax.com/abc|abc.bbsmax.com/abc|*/
        /// </summary>
        public StringList VisitPaths { get; private set; }

        private string m_Description = null;
        private string m_DescriptionText = null;
        private string m_MetaDescription = null;

        /// <summary>
        /// 论坛简介
        /// </summary>
        public string Description
        {
            get { return m_Description; }
            set
            {
                m_DescriptionText = null;
                m_MetaDescription = null;
                m_Description = value;
            }
        }

        /// <summary>
        /// 过滤了html标签后的论坛简介
        /// </summary>
        public string DescriptionText
        {
            get
            {
                string descriptionText = m_DescriptionText;
                if (descriptionText == null)
                {
                    descriptionText = StringUtil.ClearAngleBracket(Description);
                    m_DescriptionText = descriptionText;
                }
                return descriptionText;
            }
        }


        public string MetaDescription
        {
            get
            {
                string result = m_MetaDescription;

                if (result == null)
                {
                    result = StringUtil.HtmlEncode(StringUtil.CutString(DescriptionText, 200));
                    m_MetaDescription = result;
                }

                return result;
            }
        }

        /// <summary>
        /// 论坛导读
        /// </summary>
        public string Readme { get; set; }

        private string m_LogoSrc = null;
        private string m_LogoUrl = null;

        /// <summary>
        /// 论坛图标的原始地址
        /// </summary>
        public string LogoSrc
        {
            get
            {
                return m_LogoSrc;
            }
            set
            {
                m_LogoUrl = null;
                m_LogoSrc = value;
            }
        }

        /// <summary>
        /// 论坛图标的输出地址
        /// </summary>
        public string LogoUrl
        {
            get
            {
                string logoUrl = m_LogoUrl;
                if (logoUrl == null)
                {
                    logoUrl = UrlUtil.ResolveUrl(this.LogoSrc);
                    m_LogoUrl = logoUrl;
                }
                return logoUrl;
            }
        }

        /// <summary>
        /// 论坛的风格
        /// </summary>
        public string ThemeID { get; set; }

        /// <summary>
        /// 总主题数
        /// </summary>
        public int TotalThreads
        {
            get
            {
                if (totalThreads < 0)
                    return 0;
                return totalThreads;
            }
            set { totalThreads = value; }
        }

        /// <summary>
        /// 总帖子数
        /// </summary>
        public int TotalPosts
        {
            get
            {
                if (totalPosts < 0)
                    return 0;
                return totalPosts;
            }
            set { totalPosts = value; }
        }

        /// <summary>
        /// 今日主题数
        /// </summary>
        public int TodayThreads
        {
            get
            {
                if (todayThreads > totalThreads)
                    return totalThreads;
                return todayThreads;
            }
            set { todayThreads = value; }
        }

        /// <summary>
        /// 今日帖子数
        /// </summary>
        public int TodayPosts
        {
            get
            {
                if (todayPosts > totalPosts)
                    return totalPosts;
                return todayPosts;
            }
            set { todayPosts = value; }
        }


        /// <summary>
        /// 合计当前版块和所有子版块的总主题数
        /// </summary>
        public int TotalThreadsWithSubForums
        {
            get
            {
                ForumCollection subForums = SubForumsForList;

                if (subForums == null || subForums.Count == 0)
                    return TotalThreads;

                int totalThreads = TotalThreads;
                foreach (Forum subForum in subForums)
                {
                    totalThreads += subForum.TotalThreadsWithSubForums;
                }
                return totalThreads;
            }
        }

        /// <summary>
        /// 合计当前版块和所有子版块的总帖子数
        /// </summary>
        public int TotalPostsWithSubForums
        {
            get
            {
                ForumCollection subForums = SubForumsForList;

                if (subForums == null || subForums.Count == 0)
                    return TotalPosts;

                int totalPosts = TotalPosts;
                foreach (Forum subForum in subForums)
                {
                    totalPosts += subForum.TotalPostsWithSubForums;
                }
                return totalPosts;
            }
        }

        /// <summary>
        /// 合计当前版块和所有子版块的今日主题数
        /// </summary>
        public int TodayThreadsWithSubForums
        {
            get
            {
                ForumCollection subForums = SubForumsForList;

                if (subForums == null || subForums.Count == 0)
                    return TodayThreads;

                int todayThreads = TodayThreads;
                foreach (Forum subForum in subForums)
                {
                    todayThreads += subForum.TodayThreadsWithSubForums;
                }
                return todayThreads;
            }
        }

        /// <summary>
        /// 合计当前版块和所有子版块的今日发帖数
        /// </summary>
        public int TodayPostsWithSubForums
        {
            get
            {
                ForumCollection subForums = SubForumsForList;

                if (subForums == null || subForums.Count == 0)
                    return TodayPosts;

                int todayPosts = TodayPosts;
                foreach (Forum subForum in subForums)
                {
                    todayPosts += subForum.TodayPostsWithSubForums;
                }
                return todayPosts;
            }
        }

        /// <summary>
        /// 最后更新的主题
        /// </summary>
        public int LastThreadID
        {
            get { return lastThreadID; }
            set { lastThreadID = value; }
        }



        string key_forumLastThread = "forumLastThread/{0}";
        /// <summary>
        /// 最后更新的主题
        /// </summary>
        public BasicThread LastThread
        {
            get
            {
                string key = string.Format(key_forumLastThread, LastThreadID);
                BasicThread thread = null;
                if (!PageCacheUtil.TryGetValue<BasicThread>(key, out thread))
                {
                    if (LastThreadID == 0)
                        thread = null;
                    else
                    {
                        thread = PostBOV5.Instance.GetForumLastThreadFromCache(this);
                        if (thread == null)
                        {
                            thread = PostBOV5.Instance.GetThread(LastThreadID);
                            if (thread == null)
                            {
                                ThreadCollectionV5 threads = PostBOV5.Instance.GetTopThreads(ForumID);
                                if (threads != null && threads.Count > 0)
                                {
                                    thread = threads[0];
                                }
                            }

                            if (thread == null)
                            {
                                LastThreadID = 0;
                                return null;
                            }
                            else
                            {
                                PostBOV5.Instance.SetForumLastThreadCache(ForumID, thread);
                            }
                        }
                        thread = ProcessLastThread(thread, key);
                    }
                }
                return thread;
            }
            set
            {
                if (value != null)
                {
                    string key = string.Format(key_forumLastThread, LastThreadID);

                    ProcessLastThread(value, key);
                }
            }
        }

        private BasicThread ProcessLastThread(BasicThread thread,string key)
        {
            if (thread.ThreadStatus == ThreadStatus.Recycled || thread.ThreadStatus == ThreadStatus.UnApproved)
            {
                //PostBOV5.Instance.ClearTopThreadsCache(ForumID);
                ThreadCachePool.ClearAllCache();
                ThreadCollectionV5 threads = PostBOV5.Instance.GetTopThreads(ForumID);
                if (threads != null && threads.Count > 0)
                {
                    thread = threads[0];
                    LastThreadID = thread.ThreadID;
                }
                else
                {
                    LastThreadID = 0;
                    return null;
                }
            }
            if (thread.ThreadType == ThreadType.Join || thread.ThreadType == ThreadType.Move)
            {
                int threadID;
                if (int.TryParse(thread.Subject.Substring(0, thread.Subject.IndexOf(',')), out threadID))
                {
                    thread = PostBOV5.Instance.GetThread(threadID);
                    if (thread == null)
                        return null;
                }
                else
                    return null;
            }
            if (thread != null)
            {
                PageCacheUtil.Set(key, thread);
            }

            return thread;
        }


        /// <summary>
        /// 论坛访问密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否链接版块
        /// </summary>
        public bool IsLink
        {
            get
            {
                return this.ForumType == ForumType.Link;
            }
        }

        /// <summary>
        /// 如果类型是外部链接，链接的地址
        /// </summary>
        public string Link
        {
            get
            {
                if (ForumType == ForumType.Link)
                    return Password;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 主题分类状态
        /// </summary>
        public ThreadCatalogStatus ThreadCatalogStatus { get; set; }

        /// <summary>
        /// 分为几列显示
        /// </summary>
        public int ColumnSpan { get; set; }

        /// <summary>
        /// 版块的排列顺序
        /// </summary>
        public int SortOrder { get; set; }

        public ForumExtendedAttribute ExtendedAttribute
        {
            get;
            set;
        }

        public bool IsShieldedUser(int userID)
        {
            return UserBO.Instance.IsBanned(userID, ForumID);
        }

        public int ColumnSpanx
        {
            get { return this.ColumnSpan - (this.SubForumsForList.Count % this.ColumnSpan); }
        }

        #region 获取当前版块所在的分类

        private Forum m_Category = null;

        public Forum Category
        {
            get
            {
                if (ParentID == 0)
                    return this;

                Forum category = m_Category;

                if (category == null)
                {
                    category = this;

                    while (true)
                    {
                        if (category.Parent == null)
                            break;

                        category = category.Parent;
                    }

                    m_Category = category;
                }

                return category;
            }
        }

        #endregion

        #region 获取子版块

        private ForumCollection m_AllSubForums = null;

        /// <summary>
        /// 获取所有子版块
        /// </summary>
        public ForumCollection AllSubForums
        {
            get
            {
                ForumCollection result = m_AllSubForums;

                if (result == null)
                {
                    result = ForumBO.Instance.GetForums(delegate(Forum forum)
                    {
                        return forum.ParentID == this.ForumID;
                    });

                    m_AllSubForums = result;
                }

                return result;
            }
        }

        /// <summary>
        /// 获取所有当前用户可以列出的子版块
        /// </summary>
        public ForumCollection SubForumsForList
        {
            get
            {
                string cachekey = string.Format("ForumV5/{0}/SubForMe", this.ForumID);

                ForumCollection result;

                if (PageCacheUtil.TryGetValue(cachekey, out result) == false)
                {
                    result = new ForumCollection();

                    User user = User.Current;
                    ForumBO forumBOInstance = ForumBO.Instance;

                    foreach (Forum forum in AllSubForums)
                    {
                        if (forum.CanDisplayInList(user))
                            result.Add(forum);
                    }

                    PageCacheUtil.Add(cachekey, result);
                }

                return result;
            }
        }

        #endregion

        #region 版主相关

        //========================================================================

        private ModeratorCollection m_AllModerators = null;

        /// <summary>
        /// 本版块的所有版主，包括还未上任和已经过期的（并不包括版块所在分类的分类版主）
        /// </summary>
        public ModeratorCollection AllModerators
        {
            get
            {
                ModeratorCollection result = m_AllModerators;

                if (result == null)
                {
                    result = new ModeratorCollection();

                    foreach (Moderator moderator in ForumBO.Instance.GetAllModerators())
                    {
                        if (moderator.ForumID == this.ForumID)
                            result.Add(moderator);
                    }

                    m_AllModerators = result;
                }
                return result;
            }
            private set
            {
                m_AllModerators = value;
            }
        }

        /// <summary>
        /// 本版块的所有有效期内的版主（并不包括版块所在分类的分类版主）
        /// </summary>
        public ModeratorCollection Moderators
        {
            get { return AllModerators.Limited; }
        }

        /// <summary>
        /// 本版块的所有版主别表，包括还未上任和已经过期的（包括版块所在分类的分类版主）  ALL
        /// </summary>
        /// <returns></returns>
        public ModeratorCollection InheritedModerators
        {
            get
            {
                return this.Category.AllModerators;
                //Forum category = this.Category;

                //if (category == this)
                //    return AllModerators;

                //ModeratorCollection allModerators = new ModeratorCollection();

                //foreach (Moderator m in category.AllModerators)
                //{
                //    if (m.ModeratorType == ModeratorType.CategoryModerators)
                //        allModerators.Insert(0, m);
                //}

                //allModerators.Sort();

                //return allModerators;
            }
        }

        /// <summary>
        /// 还未到任期的版主
        /// </summary>
        public ModeratorCollection NoEffectModerators
        {
            get
            {
                ModeratorCollection all = AllModerators.GetListWithoutExpired();
                if (all.Count == Moderators.Count)
                    return new ModeratorCollection();

                ModeratorCollection m = new ModeratorCollection();

                foreach (Moderator md in all)
                {
                    if (md.BeginDate > DateTimeUtil.Now)
                        m.Add(md);
                }

                return m;
            }
        }

        internal void ClearModeratorCache()
        {
            this.AllModerators = null;
        }

        /// <summary>
        /// 判断用户是不是某类版主
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsModerator(int userID, ModeratorType type)
        {
            return (Moderators.GetUserModeratorType(userID) & type) == type;
        }

        #endregion

        #region 屏蔽用户相关

        private BannedUserCollection m_AllBannedUsers = null;

        public BannedUserCollection AllBannedUsers
        {
            get
            {
                BannedUserCollection result = m_AllBannedUsers;

                if (result == null)
                {
                    result = new BannedUserCollection();

                    foreach (BannedUser bannedUser in BannedUserBO.Instance.GetAllBannedUsers())
                    {
                        if (bannedUser.ForumID == this.ForumID)
                            result.Add(bannedUser);
                    }

                    m_AllBannedUsers = result;
                }
                return result;
            }
        }

        public BannedUserCollection BannedUsers
        {
            get { return AllBannedUsers.Limited; }
        }

        public void ClearBannedUserCache()
        {
            m_AllBannedUsers = null;
        }

        public bool IsBanned(User user)
        {
            return true;
        }

        #endregion

        public ForumSettingItem ForumSetting
        {
            get
            {
                string key = string.Concat("forumSetting_", ForumID.ToString());
                ForumSettingItem setting;

                if (PageCacheUtil.TryGetValue<ForumSettingItem>(key, out setting) == false)
                {
                    setting = AllSettings.Current.ForumSettings.Items.GetForumSettingItem(ForumID);
                    PageCacheUtil.Set(key, setting);
                }

                return setting;
            }
        }

        public bool IsNormalOrReadOnly { get { return ForumStatus == ForumStatus.Normal || ForumStatus == ForumStatus.ReadOnly; } }

        /// <summary>
        /// 是否可以访问当前版块
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CanVisit(User user)
        {
            if (IsNormalOrReadOnly)
            {
                if (user.UserID == 0)
                    return ForumSetting.AllowGuestVisitForum;
                else
                    return ForumSetting.VisitForum.GetValue(user);
            }
            else
                return false;
        }

        /// <summary>
        /// 是否可以看到当前版块
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool CanDisplayInList(User user)
        {
            if (IsNormalOrReadOnly)
            {
                if (user.UserID == 0)
                    return ForumSetting.DisplayInListForGuest;
                else
                    return ForumSetting.DisplayInList.GetValue(user);
            }
            else
                return false;
        }

        /// <summary>
        /// 进入版块不需要密码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool SigninWithoutPassword(User user)
        {
            if (user.UserID == 0)
                return false;
            else
                return ExtendedAttribute.SigninForumWithoutPassword.GetValue(user);
        }

        public bool IsVisitCheckPassed(AuthUser user)
        {
            return ForumBO.Instance.IsVisitCheckPassed(user, this);
        }

        /// <summary>
        /// 调用时请注意性能 AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(ForumID);
        /// </summary>
        public ManageForumPermissionSetNode ManagePermission
        {
            get { return AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(ForumID); }
        }

        /// <summary>
        /// 调用时请注意性能 AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(ForumID)
        /// </summary>
        public ForumPermissionSetNode Permission
        {
            get { return AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(ForumID); }
        }



        #region ICloneable<ForumV5> 成员

        public Forum Clone()
        {
            Forum forum = new Forum();
            forum.CodeName = CodeName;
            forum.ColumnSpan = ColumnSpan;
            forum.Description = Description;
            forum.ForumID = ForumID;
            forum.ForumName = ForumName;
            forum.ForumStatus = ForumStatus;
            forum.ForumType = ForumType;
            forum.LastThreadID = LastThreadID;
            forum.LogoSrc = LogoSrc;
            forum.ParentID = ParentID;
            forum.Password = Password;
            forum.Readme = Readme;
            forum.SortOrder = SortOrder;
            forum.ThemeID = ThemeID;
            forum.ThreadCatalogStatus = ThreadCatalogStatus;
            forum.TodayPosts = TodayPosts;
            forum.TodayThreads = TodayThreads;
            forum.TotalPosts = TotalPosts;
            forum.TotalThreads = TotalThreads;
            forum.ExtendedAttribute = ExtendedAttribute;

            return forum;
        }

        #endregion

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ForumID;
        }

        #endregion

        #region IParentable<ForumV5> 成员

        private Forum m_Parent = null;
        private bool m_InitedParent = false;

        public Forum Parent
        {
            get
            {
                if (m_InitedParent == false)
                {
                    m_Parent = ForumBO.Instance.GetForum(this.ParentID);
                    m_InitedParent = true;
                }
                return m_Parent;
            }
        }

        #endregion
    }

    public class ForumCollection : HashedCollectionBase<int, Forum>
    {
        public ForumCollection()
        { }

        public ForumCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Forum forum = new Forum(readerWrap);

                this.Add(forum);
            }
        }
    }
}