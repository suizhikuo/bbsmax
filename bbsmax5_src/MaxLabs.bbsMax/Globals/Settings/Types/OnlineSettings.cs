//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;


using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 
    /// </summary>
    public class OnlineSettings : SettingBase
    {
        public OnlineSettings()
        {
            StarsUpgradeValve = 4;
            DefaultOpen = true;
            ShowOnlineCount = 200;

            string iconPath = Globals.GetRelativeUrl(SystemDirecotry.Assets_OnlineIcon);

            RolesInOnline = new RoleInOnlineCollection();
            RolesInOnline.Add(new RoleInOnline(Role.Administrators.RoleID, 1, Role.Administrators.Name, UrlUtil.JoinUrl(iconPath, "admin.gif")));
            RolesInOnline.Add(new RoleInOnline(Role.SuperModerators.RoleID, 2, Role.SuperModerators.Name, UrlUtil.JoinUrl(iconPath, "supermod.gif")));//超级版主
            RolesInOnline.Add(new RoleInOnline(Role.CategoryModerators.RoleID, 3, Role.CategoryModerators.Name, UrlUtil.JoinUrl(iconPath, "sbm.gif")));//分类版主
            RolesInOnline.Add(new RoleInOnline(Role.Moderators.RoleID, 4, Role.Moderators.Name, UrlUtil.JoinUrl(iconPath, "mod.gif")));//版主
            RolesInOnline.Add(new RoleInOnline(Role.Users.RoleID, 5, Role.Users.Name, UrlUtil.JoinUrl(iconPath, "user.gif")));
            RolesInOnline.Add(new RoleInOnline(Role.Guests.RoleID, 6, Role.Guests.Name, UrlUtil.JoinUrl(iconPath, "guest.gif")));
        }

        private int overTime = 20;

        private OnlineShowType onlineMemberShow = OnlineShowType.ShowAll;
        //private OnlineShowType onlineGuestShow = OnlineShowType.NotShow;

        /// <summary>
        /// 同一IP的游客显示个数
        /// </summary>
        private int showSameIPCount = 3;

        /// <summary>
        /// 同一IP的蜘蛛是否全部显示
        /// </summary>
        //private bool isShowAllSpidersInSameIP = false;

        private int onlineMinLevelHours = 20;

        private int onlineUpgradeValue = 10;

        /// <summary>
        /// 超时时间单位分钟
        /// </summary>
        [SettingItem]
        public int OverTime
        {
            get { return overTime; }
            set { overTime = value; }
        }

        /// <summary>
        /// 星星升级阀值
        /// </summary>
        [SettingItem]
        public int StarsUpgradeValve { get; set; }

        [SettingItem]
        public OnlineShowType OnlineMemberShow
        {
            get { return onlineMemberShow; }
            set { onlineMemberShow = value; }
        }

        /// <summary>
        /// 默认打开还是关闭列表
        /// </summary>
        [SettingItem]
        public bool DefaultOpen
        {
            get;
            set;
        }
        //[SettingItem]
        //public OnlineShowType OnlineGuestShow
        //{
        //    get { return onlineGuestShow; }
        //    set { onlineGuestShow = value; }
        //}

        [SettingItem]
        public int ShowSameIPCount
        {
            get { return showSameIPCount; }
            set { showSameIPCount = value; }
        }

        //[SettingItem]
        public bool IsShowAllSpidersInSameIP
        {
            get
            {
                return false;
            }
            //get { return isShowAllSpidersInSameIP; }
            //set { isShowAllSpidersInSameIP = value; }
        }

        /// <summary>
        /// 在线时间等级一需要小时数
        /// </summary>
        [SettingItem]
        public int OnlineMinLevelHours
        {
            get { return onlineMinLevelHours; }
            set { onlineMinLevelHours = value; }
        }

        /// <summary>
        /// 每提高一个等级所需的小时数比上一等级提高的小时数多   ？？小时  
        /// </summary>
        [SettingItem]
        public int OnlineUpgradeValue
        {
            get
            {
                if (onlineUpgradeValue <= 0)
                    onlineUpgradeValue = 10;
                return onlineUpgradeValue;
            }
            set { onlineUpgradeValue = value; }
        }

        private int showOnlineMemberNum = 0;

        /// <summary>
        /// 在线列表最多显示多少（游客数加会员数 超过这个值， 在线列表将关闭）
        /// </summary>
        [SettingItem]
        public int ShowOnlineMemberNum
        {
            get { return showOnlineMemberNum; }
            set { showOnlineMemberNum = value; }
        }

        //private int showOnlineGuestNum = 0;

        //[SettingItem]
        //public int ShowOnlineGuestNum
        //{
        //    get { return showOnlineGuestNum; }
        //    set { showOnlineGuestNum = value; }
        //}

        [SettingItem]
        public RoleInOnlineCollection RolesInOnline
        {
            get;
            set;
        }

        public void AddRoleInOnline(RoleInOnline r)
        {
            this.RolesInOnline.Add(r);
        }

        ///// <summary>
        ///// 已经排序过
        ///// </summary>
        //public RoleInOnline[] SortedRoleInOnline
        //{
        //    get
        //    {
        //        RoleInOnline[] roles = new RoleInOnline[this.RolesInOnline.Count];
        //        RolesInOnline.CopyTo(roles, 0);
        //        Array.Sort<RoleInOnline>(roles);
        //        return roles;
        //    }
        //}

        public bool RoleInOnlineList(Guid roleID)
        {
            return this.RolesInOnline.ContainsKey(roleID);
        }

        /// <summary>
        /// 最多显示几个在线
        /// </summary>
        [SettingItem]
        public int ShowOnlineCount
        {
            get;
            set;
        }


        //==========================================================================


        public long SettingVersion
        {
            get { return 0; }
        }

        /// <summary>
        /// 获取等级图标
        /// </summary>
        /// <param name="stars">星星数（最小等级）</param>
        /// <param name="imgStyle"></param>
        /// <param name="iconUrl">各等级图标，按等级从小到大顺序</param>
        /// <returns></returns>
        public string GetLevelIcon(int stars, string imgStyle, params string[] iconUrl)
        {
            if (string.IsNullOrEmpty(imgStyle))
                imgStyle = "<img src=\"{0}\" alt=\"\" />";

            StringBuilder sb = new StringBuilder();
            for (int i = iconUrl.Length - 1; i > -1; i--)
            {
                if (i == 0)
                    break;
                int pow = (int)Math.Pow(StarsUpgradeValve, i);
                int iconCount = stars / pow;
                for (int j = 0; j < iconCount; j++)
                {
                    sb.AppendFormat(imgStyle, iconUrl[i]);
                }
                stars = stars % pow;
            }
            for (int i = 0; i < stars; i++)
            {
                sb.AppendFormat(imgStyle, iconUrl[0]);
            }
            return sb.ToString();
        }

        public string GetLevelIcon(int stars)
        {

            StringBuilder sb = new StringBuilder();
            int crownCount = stars / (StarsUpgradeValve * StarsUpgradeValve * StarsUpgradeValve);
            for (int i = 0; i < crownCount; i++)
            {
                sb.Append("<img style=\"margin-left:2px\" src=\"");
                sb.Append(Globals.GetVirtualPath(SystemDirecotry.Assets_IconStar, "crown.gif"));
                sb.Append("\" alt=\"\" />");
            }
            int remainder = stars % (StarsUpgradeValve * StarsUpgradeValve * StarsUpgradeValve);

            int sunCount = remainder / (StarsUpgradeValve * StarsUpgradeValve);
            for (int i = 0; i < sunCount; i++)
            {
                sb.Append("<img style=\"margin-left:2px;margin-top:1px\" src=\"");
                sb.Append(Globals.GetVirtualPath(SystemDirecotry.Assets_IconStar, "sun.gif"));
                sb.Append("\" alt=\"\" />");
            }

            remainder = remainder % (StarsUpgradeValve * StarsUpgradeValve);

            int moonCount = remainder / StarsUpgradeValve;
            for (int i = 0; i < moonCount; i++)
            {
                sb.Append("<img style=\"margin-left:2px;margin-top:1px\" src=\"");
                sb.Append(Globals.GetVirtualPath(SystemDirecotry.Assets_IconStar, "moon.gif"));
                sb.Append("\" alt=\"\" />");
            }

            remainder = remainder % StarsUpgradeValve;
            for (int i = 0; i < remainder; i++)
            {
                sb.Append("<img style=\"margin-left:2px;;margin-top:2px\" src=\"");
                sb.Append(Globals.GetVirtualPath(SystemDirecotry.Assets_IconStar, "star.gif"));
                sb.Append("\" alt=\"\" />");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获得在线时长等级
        /// </summary>
        /// <returns></returns>
        public int GetOnlineLevelNumber(int minutes)
        {
            int hours = minutes / 60;
            int gradeIncrease = OnlineUpgradeValue;
            int gradeStartTime = OnlineMinLevelHours;

            int gradeMax = 100;
            int gradeMin = 1;
            int lastGradeMid = 0;

            if (hours < gradeStartTime)
                return 0;

            if (hours > GetHoursByOnlineLevelNumber(gradeMax))
                return gradeMax;

            while (true)
            {
                int gradeMid = (gradeMin + gradeMax) / 2;
                int gradeHours = GetHoursByOnlineLevelNumber(gradeMid);
                if (hours < gradeHours)
                {
                    gradeMax = gradeMid;
                    if (Math.Abs(lastGradeMid - gradeMid) <= 1)
                        return lastGradeMid;
                }
                else
                {
                    gradeMin = gradeMid;
                    if (Math.Abs(lastGradeMid - gradeMid) < 1)
                        return gradeMid;
                }
                lastGradeMid = gradeMid;
            }
        }

        /// <summary>
        /// 获取在线时长剩余升级时间，单位分钟
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public int GetOnlineUpgradeRemainderMinutes(int minutes)
        {
            return GetHoursByOnlineLevelNumber(GetOnlineLevelNumber(minutes) + 1) * 60 - minutes;
        }

        /// <summary>
        /// 获取在线时长的某个等级需要的小时数
        /// </summary>
        /// <returns></returns>
        public int GetHoursByOnlineLevelNumber(int levelNumber)
        {
            //20X+10{X的平方－1/2（1+X)X}
            int gradeIncrease = OnlineUpgradeValue;
            int gradeStartTime = OnlineMinLevelHours;
            if (levelNumber == 1)
                return gradeStartTime;
            else
                return Convert.ToInt32(gradeStartTime * levelNumber + gradeIncrease * (Math.Pow(levelNumber, 2) / 2 - 0.5 * levelNumber));

        }


    }
}