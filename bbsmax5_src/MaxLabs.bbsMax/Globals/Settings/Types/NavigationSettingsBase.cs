//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Settings
{
    public abstract class NavigationSettingsBase:SettingBase
    {
        public NavigationSettingsBase()
        {
           
        }


        public abstract NavigationItemCollection Navigations
        {
            get;
            set;
        }

        public abstract int MaxID
        {
            get;
            set;
        }

        public NavigationItemCollection GetChildItems(int id)
        {
            NavigationItemCollection items = new NavigationItemCollection();
            foreach (NavigationItem item in Navigations)
            {
                if (item.ParentID == id)
                    items.Add(item);
            }

            return items;
        }

        private NavigationItemCollection m_ParentItems;
        public NavigationItemCollection ParentItems
        {
            get
            {
                NavigationItemCollection items = m_ParentItems;
                if (items == null)
                {
                    items = GetChildItems(0);
                    m_ParentItems = items;
                }

                return items;
            }
        }


        private int? m_ItemsCount;
        public int ItemsCount
        {
            get
            {
                if (m_ItemsCount == null)
                {
                    int count = 0;
                    foreach (NavigationItem item in ParentItems)
                    {
                        count += item.ChildItems.Count;
                    }
                    m_ItemsCount = count;
                }
                return m_ItemsCount.Value;
            }
        }


        private int? m_GuestItemsCount;
        public int GuestItemsCount
        {
            get
            {
                if (m_GuestItemsCount == null)
                {
                    int count = 0;
                    foreach (NavigationItem item in GuestParentItems)
                    {
                        count += item.ChildItems.Count;
                    }
                    m_GuestItemsCount = count;
                }
                return m_GuestItemsCount.Value;
            }
        }

        private NavigationItemCollection m_GuestParentItems;
        /// <summary>
        /// 游客可以看到的
        /// </summary>
        public NavigationItemCollection GuestParentItems
        {
            get
            {
                NavigationItemCollection items = m_GuestParentItems;
                if (items == null)
                {
                    items = new NavigationItemCollection();
                    foreach (NavigationItem item in ParentItems)
                    {
                        if (item.OnlyLoginCanSee)
                        {
                            continue;
                        }
                        items.Add(item);
                    }
                    m_GuestParentItems = items;
                }

                return items;
            }
        }

        private Hashtable m_HashNavigations;
        /// <summary>
        /// KEY:版块ID 或者 内部链接的KEY
        /// VALUE:NavigationItem
        /// (自定义链接除外)
        /// </summary>
        public Hashtable HashNavigations
        {
            get
            {
                Hashtable hashNavigations = m_HashNavigations;
                if (hashNavigations == null)
                {
                    hashNavigations = new Hashtable();
                    foreach (NavigationItem item in Navigations)
                    {
                        if (item.Type != MaxLabs.bbsMax.Enums.NavigationType.Custom)
                        {
                            if (hashNavigations.ContainsKey(item.UrlInfo) == false)
                                hashNavigations.Add(item.UrlInfo, item);
                        }
                    }

                    m_HashNavigations = hashNavigations;
                }
                return hashNavigations;
            }
        }


        public static Dictionary<string, string> InternalLinks()
        {
            Dictionary<string, string> links = new Dictionary<string, string>();
            //KEY全部用小写
            //这里添加一项后 1: GetInternalUrl(string key) 要对应添加一项  2:另外对应的页面 要重写 BbsPageBase中的属性 NavigationKey 设为这里的KEY
            links.Add("index", "首页");
            links.Add("login", "登陆");
            links.Add("register", "注册");
            links.Add("recoverpassword", "找回密码");
            links.Add("mydefault", "用户中心");

            links.Add("share", AllSettings.Current.ShareSettings.FunctionName);
            links.Add("favorite", AllSettings.Current.FavoriteSettings.FunctionName);
            links.Add("album", AllSettings.Current.AlbumSettings.FunctionName);
            links.Add("blog", AllSettings.Current.BlogSettings.FunctionName);
            links.Add("disk", "网络硬盘");
            links.Add("doing", AllSettings.Current.DoingSettings.FunctionName);
            links.Add("emoticon", "表情");
            links.Add("mission", "最新任务");
            links.Add("mymission", "我的任务");

            links.Add("avatar", "设置头像");
            links.Add("setting", "修改个人资料");
            links.Add("changepassword", "修改密码");
            links.Add("privacy", "隐私设置");
            links.Add("notify-setting", "通知接收设置 ");
            links.Add("feedfilter", "好友动态过滤");
            links.Add("point", "我的积分");
            links.Add("point-exchange", "积分兑换");
            links.Add("point-transfer", "积分转账");
            links.Add("spacestyle", "空间风格设置");

            links.Add("notify", "通知");
            links.Add("chat", "对话");
            links.Add("friends", "好友");

            links.Add("prop", "道具");
            links.Add("myprop", "我的道具");
            links.Add("sellingprop", "二手道具");
            links.Add("proplog", "道具记录");


            links.Add("mythreads", "我的主题");

            links.Add("friends-invite", "邀请好友");
            links.Add("friends-impression", "好友印象");
            links.Add("blacklist", "黑名单");


            links.Add("search", "搜索");
            links.Add("members", "会员");
            links.Add("announcements", "公告");
            links.Add("new", "最新主题");

            return links;
        }

        public static string GetInternalUrl(string key)
        {
            switch (key)
            {
                case "index": return UrlHelper.GetIndexUrl();
                case "login": return UrlHelper.GetLoginUrl();
                case "register": return UrlHelper.GetRegisterUrl();
                case "recoverpassword": return UrlHelper.GetRecoverPasswordUrl();
                case "mydefault": return UrlHelper.GetMyDefaultUrl();

                case "share": return UrlHelper.GetShareUrl();
                case "favorite": return UrlHelper.GetFavoriteUrl();
                case "album": return UrlHelper.GetAlbumUrl();
                case "blog": return UrlHelper.GetBlogUrl();
                case "disk": return UrlHelper.GetDiskUrl();
                case "doing": return UrlHelper.GetDiskUrl();
                case "emoticon": return UrlHelper.GetEmoticonUrl();
                case "mission": return UrlHelper.GetMissionUrl();
                case "mymission": return UrlHelper.GetMyMissionUrl();

                case "avatar": return UrlHelper.GetMyAvatarUrl();
                case "setting": return UrlHelper.GetMySettingUrl();
                case "changepassword": return UrlHelper.GetMyChagePasswordUrl();
                case "privacy": return UrlHelper.GetMyPrivacyUrl();
                case "notify-setting": return UrlHelper.GetMyNotifySettingUrl();
                case "feedfilter": return UrlHelper.GetMyFeedFilterUrl();
                case "point": return UrlHelper.GetMyPointUrl();
                case "point-exchange": return UrlHelper.GetMyPointExchangeUrl();
                case "point-transfer": return UrlHelper.GetMyPointTransferUrl();
                case "spacestyle": return UrlHelper.GetMySpaceStyleUrl();

                case "notify": return UrlHelper.GetMyNotifyUrl();
                case "chat": return UrlHelper.GetMyChatUrl();
                case "friends": return UrlHelper.GetMyFriendUrl();

                case "prop": return UrlHelper.GetPropUrl();
                case "myprop": return UrlHelper.GetMyPropUrl();
                case "sellingprop": return UrlHelper.GetSellingPropUrl();
                case "proplog": return UrlHelper.GetPropLogUrl();
                case "mythreads": return UrlHelper.GetMyThreadUrl();

                case "friends-invite": return UrlHelper.GetFriendsInviteUrl();
                case "friends-impression": return UrlHelper.GetFriendsImpressionUrl();
                case "blacklist": return UrlHelper.GetBlackListUrl();


                case "search": return UrlHelper.GetSearchUrl();
                case "members": return UrlHelper.GetMembersUrl();
                case "announcements": return UrlHelper.GetAnnouncementsUrl();
                case "new": return UrlHelper.GetNewThreadUrl();
                default: return string.Empty;
            }
        }

        public void ClearCache()
        {
            m_ParentItems = null;
            m_HashNavigations = null;
            m_GuestItemsCount = null;
            m_GuestParentItems = null;
            m_ItemsCount = null;
            foreach (NavigationItem item in Navigations)
            {
                item.ClearCache();
            }
        }
    }
}