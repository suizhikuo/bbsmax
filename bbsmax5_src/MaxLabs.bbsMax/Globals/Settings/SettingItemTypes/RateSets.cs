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

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using System.Collections.ObjectModel;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class RateSet : SettingBase, IPrimaryKey<int>//, ICloneable
    {
        public RateSet()
        {
            RateItems = new RateSetItemCollection();
        }

        [SettingItem]
        public int NodeID { get; set; }

        [SettingItem]
        public RateSetItemCollection RateItems { get; set; }

        /// <summary>
        /// 按优先级顺序 RoleID 为 Guid.Empty的 在集合最前面
        /// </summary>
        /// <param name="pointType"></param>
        /// <returns></returns>
        public RateSetItemCollection GetRateItems(UserPointType pointType)
        {
            RateSetItemCollection tempRateItems = new RateSetItemCollection();

            RateSetItem tempItem = null;
            foreach (RateSetItem item in RateItems)
            {
                if (item.PointType == pointType)
                {
                    if (item.RoleID == Guid.Empty)
                    {
                        tempItem = item;
                        continue;
                    }
                    int index = tempRateItems.Count;
                    for (int i = 0; i < tempRateItems.Count; i++)
                    {
                        if (item.RoleSortOrder > tempRateItems[i].RoleSortOrder)
                        {
                            index = i;
                            break;
                        }
                    }

                    tempRateItems.Insert(index, item);
                }
            }
            if (tempItem == null)
            {
                tempItem = new RateSetItem();
                tempItem.PointType = pointType;
                tempItem.RoleID = Guid.Empty;
            }

            RateSetItemCollection results = new RateSetItemCollection();

            results.Add(tempItem);

            for (int i = tempRateItems.Count - 1; i >= 0; i--)
            {
                results.Add(tempRateItems[i]);
            }
           //tempRateItems.Insert(0, tempItem);

           return results;
        }

        public RateSetItem GetRateItem(int userID,UserPointType pointType)
        {
            User user = UserBO.Instance.GetUser(userID);

            RateSetItemCollection items = GetRateItems(pointType);

            RateSetItem tempRankItem = null;
            foreach (RateSetItem rankItem in items)
            {
                if (rankItem.PointType == pointType)
                {
                    if (user.Roles.IsInRole(rankItem.RoleID))
                        return rankItem;

                    if (rankItem.RoleID == Guid.Empty)
                        tempRankItem = rankItem;
                }
            }

            if (tempRankItem == null)
            {
                tempRankItem = new RateSetItem();
                tempRankItem.PointType = pointType;
            }

            return tempRankItem;
        }

        /// <summary>
        /// 获取某个用户 可以评分的设置 （按顺序：积分1 积分2 ...，不包括未启用的积分，不包括不能评分的设置） 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public RateSetItemCollection GetRateItems(int userID)
        {
            RateSetItemCollection rateItems = new RateSetItemCollection();

            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.UserPoints)
            {
                if (userPoint.Enable)
                {
                    RateSetItem item = GetRateItem(userID, userPoint.Type);

                    if (item.MinValue != 0 || item.MaxValue != 0)
                        rateItems.Add(item);
                }
            }

            return rateItems;
        }

        //#region ICloneable 成员

        //public object Clone()
        //{
        //    RateSettings setting = new RateSettings();

        //    setting.RateItems = RateItems;

        //    return setting;
        //}

        //#endregion


        public void ClearExperiesData()
        {
            /*
            RateSetItemCollection rateItems = new RateSetItemCollection();

            foreach (RateSetItem item in RateItems)
            {
                if (item.RoleID == Guid.Empty || AllSettings.Current.RoleSettings.Roles.GetValue(item.RoleID) != null)
                {
                    rateItems.Add(item);
                }
            }

            RateSettings setting = new RateSettings();

            setting.RateItems = rateItems;

            try
            {
                SettingManager.SaveSettings(setting);
            }
            catch { }
             */
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return NodeID;
        }

        #endregion
    }
    public class RateSetCollection : EntityCollectionBase<int,RateSet>, ISettingItem
    {
        public RateSet GetRateSet(int nodeID)
        {
            RateSet rateSet = this.GetValue(nodeID);
            if (rateSet != null)
            {
                return rateSet;
            }
            if (nodeID != 0)
            {
                Forum forum = ForumBO.Instance.GetForum(nodeID, false);
                if (forum != null)
                {
                    return GetRateSet(forum.ParentID);
                }
            }

            if (nodeID == 0)
                return new RateSet();

            return null;
        }

        public override void Add(RateSet item)
        {
            if (item.NodeID == 0)
            {
                if (this.Count > 0 && this[0].NodeID == 0)
                    this[0] = item;
                else
                    this.Insert(0, item);
            }
            else
                base.Add(item);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (RateSet item in this)
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
                    RateSet rankItem = new RateSet();

                    rankItem.Parse(item);
                    this.Add(rankItem);

                }
            }
        }

        #endregion
    }
}