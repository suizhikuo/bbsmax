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
using MaxLabs.Passport.ClientKit.PassportServerInterface;

namespace MaxLabs.Passport.ClientKit
{
    public static class NotifyUtil
    {
        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="targetUserID">接收通知的用户ID</param>
        /// <param name="notifyType">通知类型（名称）</param>
        /// <param name="content">通知内容
        /// 内容里面的链接可以是带goto:http://xxxxx 这样的URL，如果带这种URL的在用户点击这个URL的时候，通知会变成已读状态，否则需要手动忽略
        /// </param>
        /// <param name="actions">actions指通知中出现的几个链接，比如加好友的时候有“通过请求”和打招呼时候的“回敬一个”就是由actions参数指定</param>
        /// <returns></returns>
        public static bool SendNotify(int targetUserID,string notifyType,string content,NotifyActionProxy[] actions)
        {
            string keyword = notifyType.GetHashCode() + "_" + content.GetHashCode();
            return  AsmxAccess.API.Notify_Send(targetUserID,notifyType,content,string.Empty,actions,keyword);
        }

        /// <summary>
        /// 删除指定的通知
        /// </summary>
        /// <param name="targetUserID">用户ID</param>
        /// <param name="notifyID">通知ID</param>
        /// <returns></returns>
        public static APIResult DeleteNotify( int targetUserID , int notifyID )
        {
            return  AsmxAccess.API.Notify_DeleteNotify(targetUserID, notifyID);
        }
    }
}