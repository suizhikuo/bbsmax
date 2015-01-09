//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities 
{
    /// <summary>
    /// 任务
    /// </summary>
    public class MissionPrize :  IStringConverter<ApplyMissionCondition>
    {

        public MissionPrize()
        { }

        public MissionPrize(string valueString)
        {
            this.ConvertFromString(valueString);
        }

        private List<MissionPrizeType> prizeTypes = new List<MissionPrizeType>();
        /// <summary>
        /// 奖励类型
        /// </summary>
        public List<MissionPrizeType> PrizeTypes 
        {
            get { return prizeTypes; }
            set { prizeTypes = value; } 
        }

        private int[] points = new int[8]{0,0,0,0,0,0,0,0};
        /// <summary>
        /// 积分
        /// </summary>
        public int[] Points
        {
            get {  return points; }
            set 
            {
                if (value.Length == 8)
                    points = value;
                else
                {
                    points = new int[8];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 8)
                            break;
                        points[i] = value[i];
                    }
                }
            } 
        }

        private Dictionary<Guid, long> userGroups = new Dictionary<Guid, long>();
        /// <summary>
        /// 用户组 （Guid:用户组ID int:有效时间 单位秒）
        /// </summary>
        public Dictionary<Guid, long> UserGroups
        {
            get { return userGroups; }
            set { userGroups = value; }
        }


        private PrizeMedalCollection m_Medals = new PrizeMedalCollection();
        public PrizeMedalCollection Medals
        {
            get { return m_Medals; }
            set { m_Medals = value; }
        }

        //private Dictionary<int, long> medals = new Dictionary<int, long>();
        ///// <summary>
        ///// 用户组 （第一个int:勋章ID 第二个int:有效时间 单位秒）
        ///// </summary>
        //public Dictionary<int, long> Medals
        //{
        //    get { return medals; }
        //    set { medals = value; }
        //}

        private int inviteSerialCount;
        /// <summary>
        /// 邀请码个数
        /// </summary>
        public int InviteSerialCount 
        {
            get { return inviteSerialCount; }
            set { inviteSerialCount = value; } 
        }

        ///// <summary>
        ///// 用户组有效时间 单位秒
        ///// </summary>
        //public List<int> UserGroupActiveTime
        //{ 
        //}

        private Hashtable props = new Hashtable();

        public Hashtable Props
        {
            get { return props; }
            set { props = value; }
        }


        #region IStringConverter<ApplyMissionCondition> 成员

        public string ConvertToString()
        {
            StringTable table = new StringTable();
            table.Add("PrizeTypes", StringUtil.Join(PrizeTypes));
            table.Add("Points", StringUtil.Join(Points));
            table.Add("UserGroupIDs", StringUtil.Join(UserGroups.Keys));
            table.Add("UserGroupActiveTimes", StringUtil.Join(UserGroups.Values));

            StringBuilder medalIDs = new StringBuilder();
            StringBuilder levelIDs = new StringBuilder();
            StringBuilder seconds = new StringBuilder();
            foreach (PrizeMedal medal in Medals)
            {
                medalIDs.Append(medal.MedalID).Append(",");
                levelIDs.Append(medal.MedalLevelID).Append(",");
                seconds.Append(medal.Seconds).Append(",");
            }

            if (medalIDs.Length > 0)
            {
                table.Add("MedalIDs", medalIDs.ToString(0, medalIDs.Length - 1));
                table.Add("MedalLevelIDs", levelIDs.ToString(0, levelIDs.Length - 1));
                table.Add("MedalActiveTimes", seconds.ToString(0, seconds.Length - 1));
            }
            else
            {
                table.Add("MedalIDs", string.Empty);
                table.Add("MedalLevelIDs", string.Empty);
                table.Add("MedalActiveTimes", string.Empty);
            }

            table.Add("InviteSerialCount", InviteSerialCount.ToString());


            StringTable propCounts = new StringTable();

            foreach(int key in props.Keys)
            {
                propCounts.Add(key.ToString(), props[key].ToString());
            }

            table.Add("Props", propCounts.ToString());

            return table.ToString();
        }

        public void ConvertFromString(string valueString)
        {
            StringTable table = StringTable.Parse(valueString);
            if (table.ContainsKey("PrizeTypes"))
                PrizeTypes = StringUtil.Split2<MissionPrizeType>(table["PrizeTypes"]);

            if (table.ContainsKey("Points"))
                Points = StringUtil.Split<int>(table["Points"]);

            Guid[] userGroupIDs = null;
            if (table.ContainsKey("UserGroupIDs"))
                userGroupIDs = StringUtil.Split<Guid>(table["UserGroupIDs"]);

            if (userGroupIDs != null && userGroupIDs.Length > 0 && table.ContainsKey("UserGroupActiveTimes"))
            {
                long[] times = StringUtil.Split<long>(table["UserGroupActiveTimes"]);
                for (int i = 0; i < userGroupIDs.Length; i++)
                {
                    long time;
                    if (times.Length > i)
                        time = times[i];
                    else
                        time = 0;
                    UserGroups.Add(userGroupIDs[i],time);
                }
            }

            int[] medalIDs = null, medalLevelIDs = null;
            if (table.ContainsKey("MedalIDs"))
                medalIDs = StringUtil.Split<int>(table["MedalIDs"]);
            if (table.ContainsKey("MedalLevelIDs"))
                medalLevelIDs = StringUtil.Split<int>(table["MedalLevelIDs"]);

            if (medalIDs != null && medalIDs.Length > 0 && medalLevelIDs != null && medalLevelIDs.Length > 0 && table.ContainsKey("MedalActiveTimes"))
            {
                long[] times = StringUtil.Split<long>(table["MedalActiveTimes"]);

                for (int i = 0; i < medalIDs.Length; i++)
                {
                    long time;
                    if (times.Length > i)
                        time = times[i];
                    else
                        time = 0;

                    int medalLevelID;
                    if (medalLevelIDs.Length > i)
                        medalLevelID = medalLevelIDs[i];
                    else
                        break;

                    PrizeMedal medal = new PrizeMedal();
                    medal.MedalID = medalIDs[i];
                    medal.MedalLevelID = medalLevelID;
                    medal.Seconds = time;

                    Medals.Add(medal);
                }
            }

            if (table.ContainsKey("InviteSerialCount"))
                int.TryParse(table["InviteSerialCount"], out inviteSerialCount);

            if (table.ContainsKey("Props"))
            {
                StringTable propCounts = StringTable.Parse(table["Props"]);

                foreach (string key in propCounts.Keys)
                {
                    props.Add(int.Parse(key), int.Parse(propCounts[key]));
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// 任务对象集合
    /// </summary>
    public class MissionPrizeCollection : Collection<MissionPrize>
    {

        public MissionPrizeCollection()
        {
        }

    }


    public class PrizeMedal
    {
        public PrizeMedal()
        {}

        public int MedalID { get; set; }

        public int MedalLevelID { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long Seconds { get; set; }
    }

    public class PrizeMedalCollection : Collection<PrizeMedal>
    {
        public PrizeMedalCollection()
        {
        }
    }



}