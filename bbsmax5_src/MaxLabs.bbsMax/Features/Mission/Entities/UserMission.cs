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
    public class UserMission:IFillSimpleUser
    {

        public UserMission()
        { }

        public UserMission(DataReaderWrap readerWrap)
        {
            UserID = readerWrap.Get<int>("UserID");
            MissionID = readerWrap.Get<int>("MissionID");

            FinishPercent = readerWrap.Get<double>("FinishPercent");

            Status = (MissionStatus)(int)readerWrap.Get<byte>("Status");

            IsPrized = readerWrap.Get<bool>("IsPrized");

            FinishDate = readerWrap.Get<DateTime>("FinishDate");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
        }

        public int UserID { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public int MissionID { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public MissionStatus Status { get; set; }

        /// <summary>
        /// 完成百分比
        /// </summary>
        public double FinishPercent { get; set; }


        /// <summary>
        /// 是否领取过奖励了
        /// </summary>
        public bool IsPrized { get; set; }


        /// <summary>
        /// 任务完成时间
        /// </summary>
        public DateTime FinishDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        #region 扩展字段        
        
        private Mission mission;
        public Mission Mission
        {
            get
            {
                if (mission == null)
                {
                    mission = MissionBO.Instance.GetMission(this.MissionID);
                }
                return mission;
            }
        }

        public SimpleUser MissionUser
        {
            get
            {
                return UserBO.Instance.GetSimpleUser(UserID);
            }
        }

        #endregion


        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion
    }
    /// <summary>
    /// 任务对象集合
    /// </summary>
    public class UserMissionCollection : Collection<UserMission>
    {

        public UserMissionCollection()
        {
        }



        public UserMissionCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new UserMission(readerWrap));
            }
        }
    }



}