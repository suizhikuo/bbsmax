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

namespace MaxLabs.bbsMax.Entities
{
    public class DeleteResultItem
    {
        public int NodeID { get; set; }

        public int UserID { get; set; }

        public int Count { get; set; }

        //public int Count2 { get; set; }
    }
    public class DeleteResult : Collection<DeleteResultItem>
    {

        public void Add(int userID, int deletedCount)
        {
            Add(userID, deletedCount, 0);
		}

        public void Add(int userID, int deletedCount,int nodeID)
        {
            DeleteResultItem item = new DeleteResultItem();
            item.UserID = userID;
            item.Count = deletedCount;
            item.NodeID = nodeID;
            this.Add(item);
        }

        //public void Add(int userID, int deletedCount, int deleteCount2)
        //{
        //    DeleteResultItem item = new DeleteResultItem();
        //    item.UserID = userID;
        //    item.Count = deletedCount;
        //    item.Count2 = deleteCount2;
        //    this.Add(item);
        //}

        /// <summary>
        /// 获取某用户的删除个数 如果不存该用户 则返回0
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public new int this[int userID] 
        {
            get
            {
                foreach(DeleteResultItem item in this)
                {
                    if (item.UserID == userID)
                        return item.Count;
                }
                return 0;
            }
        }
    }
}