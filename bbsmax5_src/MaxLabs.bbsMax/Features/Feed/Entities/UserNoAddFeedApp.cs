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

using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 用户不加入通知的应用
    /// </summary>
    public class UserNoAddFeedApp
    {
        public UserNoAddFeedApp()
        { 
        }

        public UserNoAddFeedApp(DataReaderWrap readerWrap)
        {
            AppID = readerWrap.Get<Guid>("AppID");

            UserID = readerWrap.Get<int>("UserID");
            ActionType = (int)readerWrap.Get<byte>("ActionType");
            Send = readerWrap.Get<bool>("Send");
        }
        /// <summary>
        /// 应用ID
        /// </summary>
        public Guid AppID { get; set; }



        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// APP动作,如"评论日志" "发表日志"
        /// </summary>
        public int ActionType { get; set; }

        /// <summary>
        /// 是否发送
        /// </summary>
        public bool Send { get; set; }
    }

    /// <summary>
    /// 用户不加入通知的应用 对象集合
    /// </summary>
    public class UserNoAddFeedAppCollection : Collection<UserNoAddFeedApp>
    {
        public UserNoAddFeedAppCollection()
        {
        }

        public UserNoAddFeedAppCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new UserNoAddFeedApp(readerWrap));
            }
        }

        /// <summary>
        /// 指定类型的动作是否加入动态
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public bool IsAddFeedAction(int userID, Guid appID, int actionType)
        {
            foreach (UserNoAddFeedApp userNoAddFeedApp in this)
            {
                if (userNoAddFeedApp.UserID == userID && userNoAddFeedApp.AppID == appID && userNoAddFeedApp.ActionType == actionType)
                    return userNoAddFeedApp.Send;
            }
            foreach (FeedSendItem item in AllSettings.Current.PrivacySettings.FeedSendItems)
            {
                if (appID == item.AppID && actionType == item.ActionType)
                {
                    return (item.DefaultSendType == FeedSendItem.SendType.Send) || (item.DefaultSendType == FeedSendItem.SendType.ForceSend);
                }
            }
            return true;
        }
    }
}