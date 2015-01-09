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
    public class Mission
    {

        public Mission()
        {

            ChildMissions = new MissionCollection();
        }

        public Mission(DataReaderWrap readerWrap)
        {
            ID = readerWrap.Get<int>("ID");
            CycleTime = readerWrap.Get<int>("CycleTime");
            SortOrder = readerWrap.Get<int>("SortOrder");
            TotalUsers = readerWrap.Get<int>("TotalUsers");

            IsEnable = readerWrap.Get<bool>("IsEnable");

            Type = readerWrap.Get<string>("Type");
            Name = readerWrap.Get<string>("Name");
            IconUrl = readerWrap.Get<string>("IconUrl");
            DeductPoint = StringUtil.Split<int>(readerWrap.Get<string>("DeductPoint"));

            Prize = new MissionPrize(readerWrap.Get<string>("Prize"));

            Description = readerWrap.Get<string>("Description");

            ApplyCondition = new ApplyMissionCondition(readerWrap.Get<string>("ApplyCondition"));
            
            FinishCondition = StringTable.Parse(readerWrap.Get<string>("FinishCondition"));

            EndDate = readerWrap.Get<DateTime>("EndDate");
            BeginDate = readerWrap.Get<DateTime>("BeginDate");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");

            CategoryID = readerWrap.GetNullable<int>("CategoryID");
            ParentID = readerWrap.GetNullable<int>("ParentID");
        }

        public int ID { get; set; }

        /// <summary>
        /// 周期 单位秒  0为不是周期任务
        /// </summary>
        public int CycleTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 申请用户总数
        /// </summary>
        public int TotalUsers { get; set; }

        public bool IsEnable { get; set; }

        /// <summary>
        /// 任务对象类名 如maxLabs.bbsMax.TopicMission 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 任务图标
        /// </summary>
        public string IconUrl { get; set; }

        private int[] deductPoint;
        /// <summary>
        /// 用户申请任务后扣除积分(格式: pointID:值;pointID:值)
        /// </summary>
        public int[] DeductPoint 
        {
            get { return deductPoint; }
            set 
            {
                if (value.Length == 8)
                    deductPoint = value;
                else
                {
                    deductPoint = new int[8];
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 8)
                            break;
                        deductPoint[i] = value[i];
                    }
                }
            } 
        }

        /// <summary>
        /// 奖励 
        /// </summary>
        public MissionPrize Prize { get; set; }


        /// <summary>
        /// 任务说明
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 申请条件
        /// </summary>
        public ApplyMissionCondition ApplyCondition
        {
            get;
            set;
        }

        /// <summary>
        /// 完成条件
        /// </summary>
        public StringTable FinishCondition { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime EndDate { get; set; }


        /// <summary>
        /// 下线时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        public int? CategoryID { get; set; }

        public int? ParentID { get; set; }

        #region 扩展字段        
        
        public UserMission StepUserMission
        {
            get
            {
                UserMission result = null;

                if (PageCacheUtil.TryGetValue<UserMission>("StepUserMission/" + this.ID, out result) == false)
                {
                    if (ChildMissions != null && ChildMissions.Count > 0)
                    {
                        int[] missionIDs = new int[ChildMissions.Count];

                        for (int i = 0; i < missionIDs.Length; i++)
                        {
                            missionIDs[i] = ChildMissions[i].ID;
                        }

                        result = MissionBO.Instance.GetStepUserMission(User.Current.UserID, missionIDs);
                    }

                    PageCacheUtil.Set("StepUserMission/" + this.ID, result);
                }

                return result;
            }
        }

        private MissionBase missionBase;
        public MissionBase MissionBase
        {
            get
            {
                if (missionBase == null)
                {
                    missionBase = MissionBO.Instance.GetMissionBase(Type);
                }
                return missionBase;
            }
        }

        /// <summary>
        /// 图表的完整地址
        /// </summary>
        public string IconPath
        {
            get
            {
                return  UrlUtil.ResolveUrl(IconUrl);
            }
        }

        #endregion

        public MissionCollection ChildMissions
        {
            get;
            set;
        }

        public bool CanApply
        {
            get
            {
                return MissionBO.Instance.CanApplyMission(User.Current, this);
            }
        }

        private string prizeDescriptionKey = "prizeDescriptionKey_{0}_{1}_{2}";
        public string PrizeDescription(string lineFormat, string separator)
        {
            string key = string.Format(prizeDescriptionKey, lineFormat, separator, this.Prize.ConvertToString());
            string description;
            if (PageCacheUtil.TryGetValue<string>(key, out description) == false)
            {
                description = MissionBO.Instance.GetMissionPrizeDescription(this.Prize, lineFormat, separator);
                PageCacheUtil.Set(key, description);
            }

            return description;
        }

        private string m_FinishConditionDescription;
        public string FinishConditionDescription
        {
            get
            {
                if (m_FinishConditionDescription == null)
                {
                    m_FinishConditionDescription = this.MissionBase.GetFinishConditionDescription(this.FinishCondition);
                    if (m_FinishConditionDescription == null)
                        m_FinishConditionDescription = string.Empty;
                }
                return m_FinishConditionDescription;
            }
        }

        private string m_ApplyConditionDescription;
        public string ApplyConditionDescription
        {
            get
            {
                if (m_ApplyConditionDescription == null)
                {
                    m_ApplyConditionDescription = MissionBO.Instance.GetApplyMissionConditionDescription(this.ApplyCondition);
                    if (m_ApplyConditionDescription == null)
                        m_ApplyConditionDescription = string.Empty;
                }
                return m_ApplyConditionDescription;
            }
        }

        private string m_DeductPointDescription;
        public string DeductPointDescription
        {
            get
            {
                if (m_DeductPointDescription == null)
                {
                    string lineFormat = "{0}{1}{2}";
                    m_DeductPointDescription = MissionBO.Instance.GetPointDescription(this.DeductPoint, lineFormat, "<br />", "扣除");
                }
                return m_DeductPointDescription;
            }
        }

        public bool IsDisplayDeductPointDescription
        {
            get
            {
                return DeductPointDescription != string.Empty;
            }
        }

        public bool IsDisplayFinishCondition
        {
            get
            {
                return FinishConditionDescription != string.Empty;
            }
        }
        public bool IsDisplayApplyCondition
        {
            get
            {
                return ApplyConditionDescription != string.Empty;
            }
        }
        public bool HavePrize
        {
            get
            {
                return this.Prize.PrizeTypes.Count > 0;
            }
        }
    }
    /// <summary>
    /// 任务对象集合
    /// </summary>
    public class MissionCollection : Collection<Mission>
    {

        public MissionCollection()
        {
        }



        public MissionCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new Mission(readerWrap));
            }
        }
    }


}