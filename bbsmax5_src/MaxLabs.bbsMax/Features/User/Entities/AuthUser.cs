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
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.DataAccess;
using System.Web;

namespace MaxLabs.bbsMax.Entities
{
    public class AuthUser : User
    {
        public AuthUser() {

        }

        public AuthUser(DataReaderWrap readerWrap)
            : base(readerWrap)
        {

            this.Password = readerWrap.Get<string>("Password");
            this.PasswordFormat = readerWrap.Get<EncryptFormat>("PasswordFormat");
            this.EverAvatarChecked = readerWrap.Get<bool>("EverAvatarChecked");

            //====================

            this.UnreadMessages = readerWrap.Get<int>("UnreadMessages");

            this.LastReadSystemNotifyID = readerWrap.Get<int>("LastReadSystemNotifyID");

            //====================



            //=====================

            this.UsedAlbumSize = readerWrap.Get<long>("UsedAlbumSize");
            this.AddedAlbumSize = readerWrap.Get<long>("AddedAlbumSize");
            this.TimeZone = readerWrap.Get<float>("TimeZone");
            this.SkinID = readerWrap.Get<string>("SkinID");
            //this.LastAvatarUpdateDate = readerWrap.Get<DateTime>("LastAvatarUpdateDate");

            //=====================

            this.TotalDiskFiles = readerWrap.Get<int>("TotalDiskFiles");
            this.UsedDiskSpaceSize = readerWrap.Get<long>("UsedDiskSpaceSize");
            this.OnlineStatus = readerWrap.Get<OnlineStatus>("OnlineStatus");
            this.EnableDisplaySidebar = readerWrap.Get<EnableStatus>("EnableDisplaySidebar");
            //this.MessageSound = readerWrap.Get<string>("MessageSound");

            this.SelectFriendGroupID = readerWrap.Get<int>("SelectFriendGroupID");
            this.ReplyReturnThreadLastPage = readerWrap.GetNullable<bool>("ReplyReturnThreadLastPage");
            //=====================

        }

        public string FirendVersion
        {
            get
            {
           
                int lastGroup = -1,
                gvalue = 0,
                fvalue = 0,
                bvalue = 0,
                bcount = this.Friends.Blacklist.Count,
                fcount = this.Friends.Count,
                gcount = 0;

                foreach (Friend f in this.Friends)
                {
                    if(lastGroup!=f.GroupID && f.GroupID>0){
                        lastGroup = f.GroupID;
                        gvalue +=f.GroupID;
                        gcount++;
                    }
                    fvalue += f.GroupID + f.UserID;
                }

                foreach (BlacklistItem b in this.Friends.Blacklist)
                {
                    bvalue += b.UserID + b.GroupID;
                }

                return string.Format("{0}{1}{2}{3}{4}{5}", fcount, gcount, bcount, gvalue, fvalue, bvalue);
            }
        }
           

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 密码加密类型
        /// </summary>
        public EncryptFormat PasswordFormat { get; set; }

        /// <summary>
        /// 历史上是否曾经通过头像认证
        /// </summary>
        public bool EverAvatarChecked { get; set; }

        //===============================

        /// <summary>
        /// 用户未读短消息数
        /// </summary>
        public int UnreadMessages { get; set; }

        private SystemNotifyCollection m_SystemNotifys;
        public SystemNotifyCollection SystemNotifys
        {
            get
            {
                SystemNotifyProvider.GetMySystemNotifys(this);
                return m_SystemNotifys;
            }
            set
            {
                m_SystemNotifys = value;
            }
        }

        /// <summary>
        /// 最后获取的系统通知的版本， 用于接收的缓存系统通知数据
        /// </summary>
        public long SystemNotifyVersion { get; set; }

        /// <summary>
        /// 已读的最后一条系统通知ID
        /// </summary>
        public int LastReadSystemNotifyID { get; set; }

        //===============================



        /// <summary>
        /// 获取未读通知的总和
        /// </summary>
        public int TotalUnreadNotifies
        {
            get
            {
                return  UnreadNotify.Count + SystemNotifys.Count;
            }
        }

        private UnreadNotifies m_UnreadNotify;
        public UnreadNotifies UnreadNotify
        {
            get
            {
                if (m_UnreadNotify == null)
                    m_UnreadNotify = new UnreadNotifies();
                return m_UnreadNotify;
            }
            set
            {
                m_UnreadNotify = value;
            }
        }




        //==========================================================================

        /// <summary>
        /// 使用了的相册容量
        /// </summary>
        public long UsedAlbumSize { get; set; }

        /// <summary>
        /// 除了基本拥有的相册容量外,附加上的相册容量,如:用积分兑换加上的容量
        /// </summary>
        public long AddedAlbumSize { get; set; }


        private float m_TimeZone = 9999;
        /// <summary>
        /// 时区
        /// </summary>
        public float TimeZone
        {
            get { return m_TimeZone == 9999 ? AllSettings.Current.DateTimeSettings.ServerTimeZone : m_TimeZone; }
            set { m_TimeZone = value; }
        }

        private string m_SkinID = null;
        public string SkinID
        {
            get {  return m_SkinID; }
            set { m_SkinID = value; }
        }

        ///// <summary>
        ///// 最后更新头像的时间
        ///// </summary>
        //public DateTime LastAvatarUpdateDate { get; set; }

        //========================================================================

        /// <summary>
        /// 网络硬盘文件数
        /// </summary>
        public int TotalDiskFiles { get; set; }

        /// <summary>
        /// 已使用的网络硬盘空间
        /// </summary>
        public long UsedDiskSpaceSize { get; set; }

        /// <summary>
        /// 用户上次保存的在线状态 （不能用此来判断用户是否在线）
        /// </summary>
        public OnlineStatus OnlineStatus { get; set; }

        /// <summary>
        /// 显示侧边栏
        /// </summary>
        public EnableStatus EnableDisplaySidebar { get; set; }

        /// <summary>
        /// 选中的好友分组
        /// </summary>
        public int SelectFriendGroupID { get; set; }

        public bool? ReplyReturnThreadLastPage
        {
            get;
            set;
        }

        ///// <summary>
        ///// 短消息 声音 文件名
        ///// </summary>
        //public string MessageSound { get; set; }

        //===================================

        #region Guest相关的属性

        /// <summary>
        /// 用来标识唯一访客的ID，如果cookie正常，且不丢失，将一直保持不变
        /// </summary>
        public string GuestID
        {
            get
            {
                string guestID;

                if (PageCacheUtil.TryGetValue<string>("GuestID_BX", out guestID) == false)
                {
                    HttpCookie cookie = CookieUtil.Get("bbxmax_guest");

                    if (cookie != null)
                        guestID = cookie.Value;
                    else
                        guestID = string.Empty;

                    //必须是32位长度(GUID)
                    if (guestID != null && guestID.Length == 32)
                        PageCacheUtil.Set("GuestID_BX", guestID);
                    else
                        PageCacheUtil.Set("GuestID_BX", string.Empty);
                }

                if (guestID == string.Empty)
                    return null;

                return guestID;
            }

        }

        public string BuildGuestID()
        {
            string guestID;

            if (PageCacheUtil.TryGetValue<string>("GuestID_BX", out guestID) == false)
            {
                HttpCookie cookie = CookieUtil.Get("bbxmax_guest");

                if (cookie != null)
                    guestID = cookie.Value;
                else
                    guestID = string.Empty;

                //必须是32位长度(GUID)
                if (guestID == null || guestID.Length != 32)
                {
                    guestID = Guid.NewGuid().ToString("N");
                    CookieUtil.Set("bbxmax_guest", guestID, DateTime.MaxValue);
                }

                PageCacheUtil.Set("GuestID_BX", guestID);
            }

            if (guestID == string.Empty)
                return null;

            return guestID;
        }

        ///// <summary>
        ///// 唯一访客的ID是不是本次访问刚刚生成的
        ///// </summary>
        //public bool MachineIDIsNew
        //{
        //    get
        //    {

        //        if (PageCacheUtil.Contains("MachineID_BX") == false)
        //            return true;

        //        bool isNew;

        //        if (PageCacheUtil.TryGetValue<bool>("MachineIDIsNew_BX", out isNew))
        //        {
        //            return isNew;
        //        }

        //        return false;
        //    }
        //}

        #endregion


        /// <summary>
        /// 是否是蜘蛛
        /// </summary>
        public bool IsSpider
        {
            get
            {
                if (UserID > 0)
                    return false;

                return RequestVariable.Current.IsSpider;
            }
        }

        /// <summary>
        /// 当前用户的IP
        /// </summary>
        public string IpAddress
        {
            get { return RequestVariable.Current.IpAddress; }
        }

        private TempDataBox m_TempDataBox = null;
        /// <summary>
        /// 密码保存器,如：保存用户访问加密日志或相册时输入的正确密码
        /// 访问保存器,如：保存用户访问日志的最后时间等
        /// 等等 游客也可以使用
        /// </summary>
        public TempDataBox TempDataBox
        {
            get
            {
                if (m_TempDataBox == null)
                    m_TempDataBox = new TempDataBox(this);

                return m_TempDataBox;
            }
        }

        /// <summary>
        /// 判断这个用户是否需要输入邀请码(该用户未加入)
        /// </summary>
        public bool NeedInputInviteSerial
        {
            get
            {
                InvitationSettings setting = AllSettings.Current.InvitationSettings;

                if (setting.InviteMode == InviteMode.Close)
                    return false;

                //如果系统设置为通过邀请码注册自动加入某个用户组
                if (setting.AddToUserRoleWhenHasInvite != Guid.Empty)
                {
                    //如果用户不在这个用户组中，那么应该提示用户输入邀请码（以加入本用户组）
                    if (Roles.IsInRole(setting.AddToUserRoleWhenHasInvite) == false)
                        return true;
                }

                //如果系统设置为未通过邀请码注册自动加入某个用户组
                if (setting.AddToUserRoleWhenNoInvite != Guid.Empty)
                {
                    //如果用户已经在这个用户组中，那么应该提示用户输入邀请码（以离开本用户组）
                    if (Roles.IsInRole(setting.AddToUserRoleWhenNoInvite))
                        return true;
                }

                return false;
            }
        }

#if !Passport

        #region RSS 相关

        private static string guestRssTicket = null;
        public string GetRssUserTicket(string forumPassword)
        {
            if (UserID == 0)
            {
                if (guestRssTicket == null)
                    guestRssTicket = SecurityUtil.DesEncode("0|guest|");
                return guestRssTicket;
            }
            else
                return SecurityUtil.DesEncode(string.Concat(UserID, "|", Password, "|", forumPassword));
        }

        #endregion

        #region 任务相关

        private int? m_TotalMissions;
        /// <summary>
        /// 正在进行中的任务个数
        /// </summary>
        public int TotalMissions
        {
            get
            {
                if (m_TotalMissions == null)
                    m_TotalMissions = MissionBO.Instance.GetUserMissionCount(UserID, MissionStatus.Underway);

                return m_TotalMissions.Value;
            }
        }

        /// <summary>
        /// 移除用户 正在进行中任务个数 的缓存
        /// </summary>
        public void ClearTotalMissionsCache()
        {
            m_TotalMissions = null;
        }

        #endregion

        #region 论坛相关

        private Dictionary<int, bool> repliedThreads = new Dictionary<int, bool>();
        public Dictionary<int, bool> RepliedThreads
        {
            get { return repliedThreads; }
            set { repliedThreads = value; }
        }

        private Dictionary<int, bool> buyedAttachments = new Dictionary<int, bool>();
        public Dictionary<int, bool> BuyedAttachments
        {
            get { return buyedAttachments; }
            set { buyedAttachments = value; }
        }

        private Dictionary<int, bool> buyedThreads = new Dictionary<int, bool>();
        public Dictionary<int, bool> BuyedThreads
        {
            get { return buyedThreads; }
            set { buyedThreads = value; }
        }

        /// <summary>
        /// 是否验证过密码的版块
        /// </summary>
        /// <param name="forum"></param>
        /// <returns></returns>
        public bool IsValidatedForum(Forum forum)
        {
            string key = string.Format(ForumBO.Key_ForumPassword, forum.ForumID);

            string password = TempDataBox.GetData(key);
            if (password == null)
                password = string.Empty;

            if (StringUtil.EqualsIgnoreCase(forum.Password, password))
            {
                return true;
            }
            return false;
        }

        public void AddValidatedForumID(int forumID, string password)
        {
            string key = string.Format(ForumBO.Key_ForumPassword, forumID);
            TempDataBox.SetData(key, password);
        }

        #endregion

#endif

    }
}