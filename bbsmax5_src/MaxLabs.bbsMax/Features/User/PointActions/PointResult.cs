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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.PointActions
{
    public class PointResult<T> where T : struct
    {
        public int UserID { get; set; }

        public T actionType { get; set; }

        public int Count { get; set; }

        public int NodeID { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">动作枚举</typeparam>
    public class PointResultCollection<T> : Collection<PointResult<T>> where T:struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="actionType">动作枚举</param>
        /// <param name="count">更新倍数</param>
        public void Add(int userID,T actionType,int count)
        {
            Add(userID, actionType, count, 0);
        }

        public void Add(int userID, T actionType, int count,int nodeID)
        {
            PointResult<T> result = new PointResult<T>();
            result.UserID = userID;
            result.actionType = actionType;
            result.Count = count;
            result.NodeID = nodeID;
            this.Add(result);
        }
    }
}