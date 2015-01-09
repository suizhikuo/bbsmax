//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class Medal : SettingBase, IPrimaryKey<int>//, ICloneable<Medal>
	{
        public Medal()
        {
            Name = string.Empty;
            Condition = string.Empty;
            Levels = new MedalLevelCollection();
        }
        [SettingItem]
        public int ID { get; set; }

        [SettingItem]
        public string Name { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [SettingItem]
        public bool Enable { get; set; }

        [SettingItem]
        public bool IsHidden { get; set; }

        [SettingItem]
        public int SortOrder { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SettingItem]
        public string Condition { get; set; }

        [SettingItem]
        public bool IsCustom { get; set; }


        private MedalLevelCollection m_Levels;
        /// <summary>
        /// 等级  等级低的在前面
        /// </summary>
        [SettingItem]
        public MedalLevelCollection Levels 
        {
            get 
            {
                return m_Levels;
            }
            set 
            {
                m_Levels = value;
            } 
        }

        [SettingItem]
        public int MaxLevelID { get; set; }


        public MedalLevel GetMedalLevel(User user)
        {
            return GetMedalLevel(user, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="onlyGetAuto">为true 则不包括管理员手动颁发的</param>
        /// <returns></returns>
        public MedalLevel GetMedalLevel(User user, bool onlyGetAuto)
        {
            MedalLevel tempMedalLevel = null;

            if (onlyGetAuto == false)
            {
                foreach (UserMedal userMedal in user.UserMedals)
                {
                    if (userMedal.MedalID == ID)
                    {
                        int i = 0;
                        foreach (MedalLevel level in Levels)
                        {
                            if (level.ID == userMedal.MedalLeveID)
                            {
                                tempMedalLevel = level;
                                if (i == Levels.Count - 1)//是最大等级的勋章 直接返回
                                    return tempMedalLevel;

                                break;
                            }
                            i++;
                        }
                    }
                }
            }

            if (IsCustom)
                return tempMedalLevel;

            int value = 0;
            switch (Condition.ToLower())
            {
                case "point_0": value = user.Points; break;
                case "point_1": value = user.ExtendedPoints[0]; break;
                case "point_2": value = user.ExtendedPoints[1]; break;
                case "point_3": value = user.ExtendedPoints[2]; break;
                case "point_4": value = user.ExtendedPoints[3]; break;
                case "point_5": value = user.ExtendedPoints[4]; break;
                case "point_6": value = user.ExtendedPoints[5]; break;
                case "point_7": value = user.ExtendedPoints[6]; break;
                case "point_8": value = user.ExtendedPoints[7]; break;
#if !Passport
                case "totalonlinetime": value = user.TotalOnlineTime; break;
#endif
                case "totaltopics": value = user.TotalTopics; break;
                case "totalposts": value = user.TotalPosts; break;
                case "deletedtopics": value = user.DeletedTopics; break;
                case "deletedreplies": value = user.DeletedReplies; break;
                case "valuedtopics": value = user.ValuedTopics; break;
                default: break;
            }

            for (int i = Levels.Count - 1; i > -1; i--)
            {
                MedalLevel medalLevel = Levels[i];
                if (value >= medalLevel.Value)
                {
                    if (tempMedalLevel == null)
                        return medalLevel;
                    else if (medalLevel.Value > tempMedalLevel.Value)//与管理员颁发的 比较  返回等级大的
                        return medalLevel;
                    else
                        return tempMedalLevel;
                }
            }

            return tempMedalLevel;
        }


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ID;
        }

        #endregion
    }

    public class MedalCollection : HashedCollectionBase<int, Medal>, ISettingItem
	{

        public override void Add(Medal item)
        {
            if (this.ContainsKey(item.ID))
                return;

            int index = 0;
            for (int i = 1; i < this.Count + 1; i++)
            {
                if (item.SortOrder > this[i - 1].SortOrder)
                {
                    index = i;
                }
                if (item.SortOrder < this[i - 1].SortOrder)
                    break;
            }
            if (index >= this.Count)
            {
                base.Add(item);
            }
            else
                base.Insert(index, item);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (Medal item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    Medal medalItem = new Medal();

                    medalItem.Parse(item);
                    this.Add(medalItem);

                }
            }
        }

        #endregion
    }
}