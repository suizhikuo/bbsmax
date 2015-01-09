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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.PointActions
{
    /// <summary>
    ///  对于T1，T2 需要保证没有相同的枚举值
    /// </summary>
    /// <typeparam name="T1">不需要传积分值的动作枚举 如没有使用NullPointAction</typeparam>
    /// <typeparam name="T2">需要传积分值的动作枚举 如没有使用NullPointAction</typeparam>
    public abstract class PointActionBase<T, T1, T2>
        : PointActionType
        where T : PointActionBase<T, T1, T2>, new()
        where T1 : struct
        where T2 : struct
    {
        private static T s_Instance = null;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new T();
                return s_Instance;
            }
        }

        /// <summary>
        /// 更新用户积分 (仅当外面还有一层检查积分时  才使用此方法 或者使用带有委托的方法)
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="actionType">动作枚举</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUserPoint(int userID, T1 actionType)
        {
            return UpdateUserPoint(userID, actionType, 1, null);
        }

        /// <summary>
        /// 更新用户积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="actionType">动作枚举</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUserPoint(int userID, T1 actionType, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return UpdateUserPoint(userID, actionType, 1, beforeUpdate);
        }

        /// <summary>
        /// 批量更新用户的积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoint(T1 actionType, PointActionManager.TryUpdateUserPointCallback2 beforeUpdate)
        {
            return PointActionManager.UpdateUserPoint<T1>(Type, actionType, true, beforeUpdate);
        }


        /// <summary>
        /// 更新多个用户的多个动作的积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoints(PointActionManager.TryUpdateUserPointCallback3<T1> beforeUpdate)
        {
            return PointActionManager.UpdateUsersPoints<T1>(Type, true, 0, beforeUpdate);
        }

        /// <summary>
        /// 更新多个用户的多个动作的积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="isNormal">通常为true，如果为false则取积分的相反值（通常发生在类似帖子由正常变成未审核，此时为false）</param>
        /// <param name="nodeId">没有则为0</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoints(bool isNormal, int nodeId, PointActionManager.TryUpdateUserPointCallback3<T1> beforeUpdate)
        {
            return PointActionManager.UpdateUsersPoints<T1>(Type, isNormal, nodeId, beforeUpdate);
        }

        /// <summary>
        /// 批量更新用户的积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="isNormal">如果为false则取积分的相反值（通常发生在类似帖子由正常变成未审核，此时为false）</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoint(T1 actionType, bool isNormal, PointActionManager.TryUpdateUserPointCallback2 beforeUpdate)
        {
            return PointActionManager.UpdateUserPoint<T1>(Type, actionType,true, beforeUpdate);
        }

        /// <summary>
        /// 批量更新用户的积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="userIDs">key:用户ID；value:更新倍数</param>
        /// <param name="actionType"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoint(Dictionary<int, int> userIDs, T1 actionType, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUserPoint<T1>(userIDs, Type, actionType, true, 0, beforeUpdate);
        }

        /// <summary>
        /// 批量更新用户的积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="userIDs">key:用户ID；value:更新倍数</param>
        /// <param name="actionType"></param>
        /// <param name="isNormal">如果为false则取积分的相反值（通常发生在类似帖子由正常变成未审核，此时为false）</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoint(Dictionary<int, int> userIDs, T1 actionType, bool isNormal,int nodeId, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUserPoint<T1>(userIDs, Type, actionType, isNormal, nodeId, beforeUpdate);
        }

        /// <summary>
        /// 更新用户积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="actionType">动作枚举</param>
        /// <param name="count">更新次数 如：批量删除了3篇日志，这里应该为3</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUserPoint(int userID, T1 actionType, int count, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUserPoint<T1>(userID, Type, actionType, count, true, 0, beforeUpdate);
        }

        /// <summary>
        /// 更新用户积分
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="actionType">动作枚举</param>
        /// <param name="count">更新次数 如：批量删除了3篇日志，这里应该为3</param>
        /// <param name="isNormal">如果为false则取积分的相反值（通常发生在类似帖子由正常变成未审核，此时为false）</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        internal bool UpdateUserPoint(int userID, T1 actionType, int count, bool isNormal, int nodeID, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUserPoint<T1>(userID, Type, actionType, count, isNormal, nodeID, beforeUpdate);
        }

        /// <summary>
        /// 对同一个用户的不同动作 操作积分
        /// </summary>
        /// <param name="userID">操作该用户的积分</param>
        /// <param name="noNeedValueActions">(key:动作类型 value:操作倍数) 不需要用户填值的积分动作类型 如果没有用null</param>
        /// <param name="needValueActions">(key:动作类型 value:积分值) 需要用户填值的积分动作类型 如果没有用null</param>
        /// <param name="nodeID"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUserPoints(int userID, Dictionary<T1, int> noNeedValueActions, Dictionary<T2, int> needValueActions, int nodeID, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUserPoints<T1, T2>(userID, noNeedValueActions, needValueActions, Type, nodeID, beforeUpdate);
        }

        /// <summary>
        /// 更新多个用户的多个动作 积分  （针对不同的用户）
        /// </summary>
        /// <param name="userActions">key:userID value:动作</param>
        /// <param name="count">更新倍数</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPoint(Dictionary<int, T1> userActions, int count, int nodeID, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUsersPoint<T1>(userActions, Type, count, nodeID, beforeUpdate);
        }

        /// <summary>
        /// 更新多个用户的多个动作 积分  （针对不同的用户）
        /// </summary>
        /// <param name="userActions">key:userID value:动作</param>
        /// <param name="count">更新倍数</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPointWithNoTrans(Dictionary<int, T1> userActions, int count, int nodeID, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUsersPoint<T1>(userActions, Type, count, nodeID, beforeUpdate);
        }


        /// <summary>
        /// 获取某个动作 会更新的积分类型和值
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="userId"></param>
        /// <param name="template">显示格式如：{0}:{1}  其中{0}表示积分名称 {1}表示值</param>
        /// <param name="separator">多个积分之间的分隔符</param>
        /// <returns></returns>
        public string GetActionUserPointValue(T1 actionType, int userId,int nodeID, string template, string separator)
        {
            Dictionary<UserPoint, int> points = PointActionManager.GetActionUserPointValue<T1>(userId, Type, actionType, nodeID);

            StringBuilder valueString = new StringBuilder();

            MaxLabs.bbsMax.Entities.User user = UserBO.Instance.GetUser(userId);
            if (user == null)
                return string.Empty;

            foreach (KeyValuePair<UserPoint, int> pair in points)
            {
                string value;

                if (pair.Value >= 0)
                    value = "+" + pair.Value;
                else
                    value = pair.Value.ToString();

                valueString.Append(string.Format(template, pair.Key.Name, value, user.ExtendedPoints[(int)pair.Key.Type])).Append(separator);
            }

            if (valueString.Length > 0)
                return valueString.ToString(0, valueString.Length - separator.Length);

            return string.Empty;
        }

        public string GetActionUserPointValue(T1 actionType, int userId, string template, string separator)
        {
            return GetActionUserPointValue(actionType,userId,0,template,separator);
        }

        /// <summary>
        /// 该动作 是否有要更新的积分
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasActionUserPointValue(T1 actionType, int userId,int nodeID)
        {
            return GetActionUserPointValue(actionType, userId, nodeID).Count > 0;
        }
        public bool HasActionUserPointValue(T1 actionType, int userId)
        {
            return HasActionUserPointValue(actionType, userId, 0);
        }

        /// <summary>
        /// 获取某个动作 会更新的积分类型和值
        /// </summary>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public Dictionary<UserPoint, int> GetActionUserPointValue(T1 actionType,int userId,int nodeID)
        {
            return PointActionManager.GetActionUserPointValue<T1>(userId, Type, actionType, nodeID);
        }

        public Dictionary<UserPoint, int> GetActionUserPointValue(T1 actionType, int userId)
        {
            return GetActionUserPointValue(actionType,userId,0);
        }


        /// <summary>
        /// 更新用户积分 
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// <para>UserPointTradeMinValueError 积分超出本次交易允许的最小值</para>
        /// <para>UserPointTradeMaxValueError 积分超出本次交易允许的最大值</para>
        /// <para>UserPointTradeRemainingError 交易后积分剩余值小于该积分允许的最小值</para>
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="actionType">动作枚举</param>
        /// <param name="value">要更新的值 正数为加积分 负数为减积分</param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUserPointValue(int userID, T2 actionType, int value, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return UpdateUserPointValue(userID,actionType,0,value,beforeUpdate);
        }
        public bool UpdateUserPointValue(int userID, T2 actionType, int nodeID, int value, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUserPointValue<T2>(userID, Type, actionType, nodeID, value, beforeUpdate);
        }

        /*
        /// <summary>
        /// 更新用户积分 
        /// <para>可能抛出的错误:</para>
        /// <para>UserPointOverMaxValueError 积分超出上限</para>
        /// <para>UserPointOverMinValueError 积分超出下限</para>
        /// <para>UserPointTradeMinValueError 积分超出本次交易允许的最小值</para>
        /// <para>UserPointTradeMaxValueError 积分超出本次交易允许的最大值</para>
        /// <para>UserPointTradeRemainingError 交易后积分剩余值小于该积分允许的最小值</para>
        /// </summary>
        /// <param name="userPoints">key:userID value:积分值</param>
        /// <param name="actionType"></param>
        /// <param name="beforeUpdate"></param>
        /// <returns></returns>
        public bool UpdateUsersPointValue(Dictionary<int, int> userPoints, T2 actionType, PointActionManager.TryUpdateUserPointCallback beforeUpdate)
        {
            return PointActionManager.UpdateUsersPointValue<T2>(userPoints, Type, actionType, beforeUpdate, null);
        }

        public bool UpdateUsersPointValue(T2 actionType, PointActionManager.TryUpdateUserPointCallback2 beforeUpdate)
        {
            return PointActionManager.UpdateUsersPointValue<T2>(null, Type, actionType, null, beforeUpdate);
        }
        */

        /// <summary>
        /// 获取 某个动作 会更新的积分类型 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public UserPoint GetUserPoint(int userId, T2 actionType)
        {
            return GetUserPoint(userId,actionType,0);
        }
        public UserPoint GetUserPoint(int userId, T2 actionType,int nodeID)
        {
            int? minRemainingValue;
            int minValue;
            int? maxValue;
            return GetUserPoint(userId, actionType, nodeID, out minRemainingValue, out minValue, out maxValue);
        }
        /// <summary>
        /// 获取 某个动作 会更新的积分类型 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="minRemainingValue">交易后积分剩余最低值 null为不限制 但不能低于积分下限</param>
        /// <param name="minValue">本次最小交易值</param>
        /// <param name="maxValue">本次最大交易值 null为不限制</param>
        /// <returns></returns>
        public UserPoint GetUserPoint(int userId,T2 actionType,int nodeID,out int? minRemainingValue,out int minValue,out int? maxValue)
        {
            PointAction pointAction;
            UserPoint userPoint = PointActionManager.GetUserPoint<T2>(userId, Type, actionType,nodeID, out pointAction);

            pointAction.GetActionPointValueSetting(actionType.ToString(), userId, out minRemainingValue, out minValue, out maxValue);

            return userPoint;
        }



        private Dictionary<string, PointActionItemAttribute> m_ActionAttributes;
        public override Dictionary<string, PointActionItemAttribute> ActionAttributes
        {
            get 
            {
                if (m_ActionAttributes == null)
                {
                    T1 t1 = new T1();
                    m_ActionAttributes = GetPointActionItemAttributes<T1>(t1);
                }
                return m_ActionAttributes;
            }
        }

        private Dictionary<string, PointActionItemAttribute> m_NeedValueActionAttributes;
        public override Dictionary<string, PointActionItemAttribute> NeedValueActionAttributes
        {
            get
            {
                if (m_NeedValueActionAttributes == null)
                {
                    T2 t2 = new T2();

                    m_NeedValueActionAttributes = GetPointActionItemAttributes<T2>(t2);

                }

                return m_NeedValueActionAttributes;
            }
        }

        private Dictionary<string, PointActionItemAttribute> GetPointActionItemAttributes<ItemT>(ItemT t) where ItemT : struct
        {
            Dictionary<string, PointActionItemAttribute>  actionAttributes = new Dictionary<string, PointActionItemAttribute>(StringComparer.CurrentCultureIgnoreCase);

            if (t is NullEnum)
            { }
            else
            {
                System.Reflection.FieldInfo[] fields = t.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);

                foreach (System.Reflection.FieldInfo field in fields)
                {
                    object[] objects = field.GetCustomAttributes(typeof(PointActionItemAttribute), true);
                    if (objects.Length > 0)
                    {
                        PointActionItemAttribute item = (PointActionItemAttribute)objects[0];
                        item.Action = field.Name;
                        actionAttributes.Add(field.Name, item);
                    }
                    else
                    {
                        PointActionItemAttribute item = new PointActionItemAttribute(field.Name, false, false);
                        item.Action = field.Name;
                        actionAttributes.Add(field.Name, item);
                    }
                }
            }

            return actionAttributes;
        }

        /// <summary>
        /// 未开启的动作
        /// </summary>
        public virtual List<T1> DisableActionItems
        {
            get
            {
                return new List<T1>();
            }
        }

        public override List<string> DisableActions
        {
            get 
            {
                List<string> items = new List<string>();
                foreach (T1 t in DisableActionItems)
                {
                    items.Add(t.ToString());
                }
                return items;
            }
        }
    }
}