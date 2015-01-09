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

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 用户的未读通知数变更
    /// </summary>
    /// <param name="userID">用户ID</param>
    /// <param name="notifyCount">新通知数量</param>
    public delegate void UserNotifyCountChanged(UnreadNotifies unreadnotifies);

    /// <summary>
    /// 创建系统通知
    /// </summary>
    public delegate void SystemNotifyCreate(SystemNotify notify);

    /// <summary>
    /// 编辑系统通知 
    /// </summary>
    /// <param name="notify"></param>
    public delegate void SystemNotifyUpdated(SystemNotify notify);

    /// <summary>
    /// 删除系统通知
    /// </summary>
    /// <param name="notifyID"></param>
    public delegate void SystemnotifyDeleted(IEnumerable<int> notifyIDs);

    /// <summary>
    /// 用户忽略通知
    /// </summary>
    /// <param name="userID"></param>
    /// <param name="systemNotifyID"></param>
    public delegate void UserIgnoreSystemNotify(int userID ,int systemNotifyID);
}