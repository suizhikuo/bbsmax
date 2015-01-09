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
    public sealed class FeedSendItem : SettingBase, ICloneable
	{
        public FeedSendItem()
        {
            AppID = Guid.Empty;
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        [SettingItem]
        public Guid AppID { get; set; }

        /// <summary>
        /// 动作类型
        /// </summary>
        [SettingItem]
        public int ActionType { get; set; }

        /// <summary>
        /// 发送类型
        /// </summary>
        [SettingItem]
        public SendType DefaultSendType { get; set; }


        #region ICloneable 成员

        public object Clone()
        {
            FeedSendItem item = new FeedSendItem();
            item.ActionType = ActionType;
            item.AppID = AppID;
            item.DefaultSendType = SendType.Send;

            return item;
        }

        #endregion


        public enum SendType
        {
            /// <summary>
            /// 默认发送
            /// </summary>
            Send = 0,

            /// <summary>
            /// 默认不发送
            /// </summary>
            NotSend = 1,

            /// <summary>
            /// 强制发送
            /// </summary>
            ForceSend = 2,

            /// <summary>
            /// 强制不发送
            /// </summary>
            ForceNotSend = 3,

        }
    }

    public class FeedSendItemCollection : Collection<FeedSendItem>, ISettingItem
	{
        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (FeedSendItem item in this)
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
                    FeedSendItem feedSendItem = new FeedSendItem();

                    feedSendItem.Parse(item);
                    this.Add(feedSendItem);

                }
            }
        }

        #endregion
    }
}