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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.PointActions
{
    public abstract class PointActionType
    {
        public virtual bool Enable
        {
            get { return true; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get { return this.GetType().Name; } }

        /// <summary>
        /// 名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 是否有类似版快的子节点
        /// </summary>
        public virtual bool HasNodeList { get { return false; } }

        public virtual NodeItemCollection NodeItemList { get { return null; } }


        /// <summary>
        /// 未开启的动作
        /// </summary>
        public abstract List<string> DisableActions { get; }
        ///// <summary>
        ///// 获取某个动作 会更新的积分类型和值
        ///// </summary>
        ///// <param name="actionType"></param>
        ///// <returns></returns>
        //public Dictionary<UserPoint, int> GetActionUserPointValue(int userId,string actionType)
        //{
        //    return UserBO.Instance.GetActionUserPointValue<string>(userId, Type, actionType);
        //}

        /// <summary>
        /// 获取 某个动作 会更新的积分类型 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="minRemainingValue">交易后积分剩余最低值 null为不限制 但不能低于积分下限</param>
        /// <param name="minValue">本次最小交易值</param>
        /// <param name="maxValue">本次最大交易值 null为不限制</param>
        /// <returns></returns>
        public UserPoint GetUserPoint(int userId, string actionType, int nodeID, out int? minRemainingValue, out int minValue, out int? maxValue)
        {
            PointAction pointAction;
            UserPoint userPoint = PointActionManager.GetUserPoint<string>(userId, Type, actionType, nodeID, out pointAction);

            pointAction.GetActionPointValueSetting(actionType.ToString(), userId, out minRemainingValue, out minValue, out maxValue);

            return userPoint;
        }

        public abstract Dictionary<string, PointActionItemAttribute> ActionAttributes { get; }

        public abstract Dictionary<string, PointActionItemAttribute> NeedValueActionAttributes { get; }

    }
}