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
    /// 任务申请条件
    /// </summary>
    public class ApplyMissionCondition :  IStringConverter<ApplyMissionCondition>
    {

        public ApplyMissionCondition()
        { }

        public ApplyMissionCondition(string valueString)
        {
            this.ConvertFromString(valueString);
        }

        private List<Guid> userGroupIDs = new List<Guid>();
        /// <summary>
        /// 用户组，如果Count为0 则表示所有用户组的用户都能申请
        /// </summary>
        public List<Guid> UserGroupIDs 
        {
            get { return userGroupIDs; }
            set { userGroupIDs = value; } 
        }

        private int totalPoint;
        public int TotalPoint
        {
            get { return totalPoint; }
            set { totalPoint = value; }
        }


        private int[] points = new int[8]{0,0,0,0,0,0,0,0};
        /// <summary>
        /// 积分 8个
        /// </summary>
        public int[] Points
        {
            get { return points; }
            set 
            {
                if (value.Length == 8)
                    points = value;
                else
                {
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 8)
                            break;
                        points[i] = value[i];
                    }
                }
            } 
        }

        private int totalPosts;
        /// <summary>
        /// 发帖子总数
        /// </summary>
        public int TotalPosts
        {
            get { return totalPosts; }
            set { totalPosts = value; }
        }

        private int onlineTime;
        /// <summary>
        /// 在线时长 单位小时
        /// </summary>
        public int OnlineTime
        {
            get { return onlineTime; }
            set { onlineTime = value; }
        }


        private List<int> otherMissionIDs = new List<int>();
        /// <summary>
        /// 必须先完成的任务
        /// </summary>
        public List<int> OtherMissionIDs
        {
            get { return otherMissionIDs; }
            set { otherMissionIDs = value; }
        }

        private int maxApplyCount;
        /// <summary>
        /// 申请人数上限
        /// </summary>
        public int MaxApplyCount
        {
            get { return maxApplyCount; }
            set { maxApplyCount = value; }
        }


        #region IStringConverter<ApplyMissionCondition> 成员

        public string ConvertToString()
        {
            StringTable table = new StringTable();
            table.Add("UserGroupIDs", StringUtil.Join(UserGroupIDs));
            table.Add("TotalPoint", TotalPoint.ToString());
            table.Add("Points", StringUtil.Join(Points));
            table.Add("TotalPosts", TotalPosts.ToString());
            table.Add("OnlineTime", OnlineTime.ToString());
            table.Add("OtherMissionIDs", StringUtil.Join(OtherMissionIDs));
            table.Add("MaxApplyCount", MaxApplyCount.ToString());

            return table.ToString();
        }

        public void ConvertFromString(string valueString)
        {
            StringTable table = StringTable.Parse(valueString);
            if (table.ContainsKey("UserGroupIDs"))
                UserGroupIDs = StringUtil.Split2<Guid>(table["UserGroupIDs"]);

            if (table.ContainsKey("TotalPoint"))
                int.TryParse(table["TotalPoint"], out totalPoint);

            if (table.ContainsKey("Points"))
                Points = StringUtil.Split<int>(table["Points"]);

            if (table.ContainsKey("TotalPosts"))
                int.TryParse(table["TotalPosts"],out totalPosts);

            if (table.ContainsKey("OnlineTime"))
                int.TryParse(table["OnlineTime"], out onlineTime);

            if (table.ContainsKey("OtherMissionIDs"))
                OtherMissionIDs = StringUtil.Split2<int>(table["OtherMissionIDs"]);

            if (table.ContainsKey("MaxApplyCount"))
                int.TryParse(table["MaxApplyCount"], out maxApplyCount);

        }

        #endregion


        public string OtherMissionIDString
        {
            get
            {
                return StringUtil.Join(OtherMissionIDs);
            }
        }
    }
    /// <summary>
    /// 集合
    /// </summary>
    public class ApplyMissionConditionCollection : Collection<ApplyMissionCondition>
    {

        public ApplyMissionConditionCollection()
        {
        }



        //public ApplyConditionCollection(IDataReader reader)
        //{
            
        //    DataReaderWrap readerWrap = new DataReaderWrap(reader);

        //    while (readerWrap.Next)
        //    {
        //        this.Add(new ApplyCondition(readerWrap));
        //    }
        //}
    }



}