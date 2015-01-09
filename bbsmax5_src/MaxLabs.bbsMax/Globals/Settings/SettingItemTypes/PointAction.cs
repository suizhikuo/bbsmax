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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.PointActions;

namespace MaxLabs.bbsMax.Settings
{
    public class PointAction : SettingBase, IPrimaryKey<string>
    {
        public PointAction()
        {
            Type = string.Empty;
            PointActionItems = new PointActionItemCollection();

           
        }

        [SettingItem]
        public int NodeID { get; set; }

        /// <summary>
        /// 类型  比如："Share"
        /// </summary>
        [SettingItem]
        public string Type { get; set; }


        private PointActionItemCollection m_PointActionItems;
        [SettingItem]
        public PointActionItemCollection PointActionItems
        {
            get { return m_PointActionItems; }
            set
            {
                m_PointActionItems = value;
            }
        }

        /// <summary>
        /// 按优先级顺序 RoleID 为 Guid.Empty的 在集合最前面
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public PointActionItemCollection GetPointActionItems(string action)
        {
            PointActionItemCollection tempPointActionItems = new PointActionItemCollection();

            PointActionItem tempItem = null;
            foreach (PointActionItem item in PointActionItems)
            {
                if (string.Compare(item.Action, action, true) == 0)
                {
                    if (item.RoleID == Guid.Empty)
                    {
                        tempItem = item;
                        continue;
                    }
                    int index = tempPointActionItems.Count;
                    for (int i = 0; i < tempPointActionItems.Count; i++)
                    {
                        if (item.RoleSortOrder > tempPointActionItems[i].RoleSortOrder)
                        {
                            index = i;
                            break;
                        }
                    }

                    tempPointActionItems.Insert(index, item);
                }
            }

            PointActionItemCollection results = new PointActionItemCollection();

            if (tempItem != null)
                results.Add(tempItem);

            for (int i = tempPointActionItems.Count - 1; i >= 0; i--)
            {
                results.Add(tempPointActionItems[i]);
            }

            //if (tempItem != null)
            //    tempPointActionItems.Insert(0,tempItem);

            return results;
        }


        /// <summary>
        /// 按用户的用户组  取出对该用户的设置
        /// </summary>
        /// <param name="action"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public PointActionItem GetPointActionItem(string action, int userId)
        {
            PointActionItemCollection items = GetPointActionItems(action);

            if (items.Count == 0)
                return null;

            if (items.Count > 1)
            {
                User user = UserBO.Instance.GetUser(userId);
                if(user == null)
                    return null;
                UserRoleCollection roles = user.Roles;
                for (int i = 1; i < items.Count; i++)
                {
                    foreach (UserRole role in roles)
                    {
                        if (role.RoleID == items[i].RoleID)
                        {
                            return items[i];
                        }
                    }
                }
            }

            return items[0];

        }

        /// <summary>
        /// 对于需要设置值的积分  返回该动作需要操作的积分值  始终返回8个
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public int[] GetPoints(string action, int userId)
        {

            PointActionItem item = GetPointActionItem(action, userId);

            if (item == null)
                return new int[8];
            else
            {
                int[] points = new int[8];
                for (int i = 0; i < item.Points.Length; i++)
                {
                    points[i] = item.Points[i];
                }
                return points;
            }

        }

        /// <summary>
        /// 对于不需要设置值的积分 返回该动作需要操作的积分类型
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public UserPointType GetUserPointType(string action, int userId)
        {
            PointActionItem item = GetPointActionItem(action, userId);
            if (item != null)
            {
                return item.PointType;
            }
            return UserPointType.Point1;
        }

        /// <summary>
        /// 获取前台用户填写积分值的要求（后台不需要设置积分值的动作）
        /// </summary>
        /// <param name="action">动作</param>
        /// <param name="minRemaining">交易后允许剩余的最低余额 不能低于积分下限（null时为积分下限） </param>
        /// <param name="minValue">交易的最小金额</param>
        /// <param name="maxValue">交易的最大金额</param>
        public void GetActionPointValueSetting(string action, int userId, out int? minRemaining, out int minValue, out int? maxValue)
        {

            PointActionItem item = GetPointActionItem(action, userId);
            if (item == null)
            {
                minRemaining = null;

                PointActionItem temp = new PointActionItem();
                minValue = temp.MinValue;
                maxValue = temp.MaxValue;
            }
            else
            {
                if (item.MinRemaining == int.MinValue)
                    minRemaining = null;
                else
                    minRemaining = item.MinRemaining;

                minValue = item.MinValue;

                if (item.MaxValue == int.MaxValue)
                    maxValue = null;
                else
                    maxValue = item.MaxValue;

            }

        }


        ///// <summary>
        ///// 设置前台用户填写积分值的要求（后台不需要设置积分值的动作）
        ///// </summary>
        ///// <param name="action">动作</param>
        ///// <param name="minRemaining">交易后允许剩余的最低余额 不能低于积分下限（null时为积分下限）</param>
        ///// <param name="minValue">交易的最小金额</param>
        ///// <param name="maxValue">交易的最大金额</param>
        //public void SetActionPointValueSetting(string action, int? minRemaining, int minValue, int? maxValue)
        //{
        //    string settingString = (
        //        (minRemaining == null ? string.Empty : minRemaining.Value.ToString())
        //        + "," + minValue.ToString()
        //        + "," + (maxValue == null ? string.Empty : maxValue.Value.ToString())
        //        );
        //    PointValueSettings.Add(action, settingString);
        //}

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return Type.ToLower() + "-" + NodeID;
        }

        #endregion
    }


    public class PointActionCollection : HashedCollectionBase<string, PointAction>, ISettingItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="nodeID"></param>
        /// <returns>如果NodeID为0则始终不返回NULL</returns>
        public PointAction GetPointAction(string type, int nodeID)
        {
            if (this.ContainsKey(type.ToLower() + "-" + nodeID))
            {
                return this.GetValue(type.ToLower() + "-" + nodeID);
            }

            if (nodeID > 0)
            {
                PointActionType pointActionType = PointActionManager.GetPointActionType(type);
                NodeItem item = pointActionType.NodeItemList.GetValue(nodeID);

                if (item == null)
                    return null;
                else
                    return GetPointAction(type, item.ParentID);
            }

            return new PointAction();
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PointAction item in this)
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
                    PointAction pointAction = new PointAction();
                    pointAction.Parse(item);
                    this.Add(pointAction);
                }
            }
        }

        #endregion
    }


}