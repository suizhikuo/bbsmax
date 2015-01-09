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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax
{
    public class VarsManager
    {

        private static Vars stat;
        public static Vars Stat
        {
            get
            {
                if (stat == null)
                    stat = VarDao.Instance.GetVars();

                return stat;
            }
        }

        public static void ResetVars()
        {
            stat = null;
        }

        ///// <summary>
        ///// 更新最新用户的统计
        ///// </summary>
        //public static void UpdateNewUserStat()
        //{
        //    try
        //    {
        //        Vars newStat = VarDao.Instance.UpdateNewUserStat();
        //        stat = newStat;
        //    }
        //    catch { }
        //}


        public static void UpdateMaxOnline(int count,DateTime date)
        {
            Vars stat = (Vars)Stat.Clone();

            if (stat.MaxOnlineCount < count)
            {
                stat.MaxOnlineCount = count;
                stat.MaxOnlineDate = date;
                UpdateVars(stat);
            }
        }

        public static bool UpdateYestodayPostAndMaxPost(int postCount, int topicCount, DateTime postDate)
        {
            DateTime datetime = DateTimeUtil.Now;


            if (datetime.Year == Stat.LastResetDate.Year && datetime.Month == Stat.LastResetDate.Month
                && datetime.Day == Stat.LastResetDate.Day)
                return false;

            Vars stat = (Vars)Stat.Clone();

            if (Stat.MaxPosts < postCount)
            {
                stat.MaxPosts = postCount;
                stat.MaxPostDate = postDate;
            }
            stat.YestodayPosts = postCount;
            stat.YestodayTopics = topicCount;
            stat.LastResetDate = datetime;

            UpdateVars(stat);

            return true;
        }

        /// <summary>
        /// 重新统计用户数  只用于后台 的重新统计
        /// </summary>
        public static void UpdateUserCount()
        {
            Vars newStat = VarDao.Instance.UpdateNewUserStat();
            stat = newStat;
        }

        public static void UpdateVars(Vars vars)
        {
            try
            {
                VarDao.Instance.UpdateVars(vars);
                VarsManager.stat = vars;
            }
            catch(Exception ex)
            {
                LogHelper.CreateErrorLog(ex);
            }
        }

    }
}