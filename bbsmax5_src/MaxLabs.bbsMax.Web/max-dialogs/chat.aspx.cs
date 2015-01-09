//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Web;


using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.ValidateCodes;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class chat : DialogPageBase
    {
        const int c_PageSize = 20;

        private int m_ToUserID;
        private ChatMessageCollection m_ChatMessageList;
        private SimpleUser m_ChatUser;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!EnableChatFunction)
            {
                ShowError("管理员已关闭对话功能！");
                return;
            }

            //验证码频率计数（页面调用次数计数）
            ValidateCodeManager.CreateValidateCodeActionRecode(validateActionName);

            m_ToUserID = _Request.Get<int>("to", Method.Get, 0);

            if (m_ToUserID <= 0)
                ShowError(new UserNotExistsError("to", m_ToUserID));

            m_ChatUser = UserBO.Instance.GetSimpleUser(m_ToUserID,true);

            if (m_ChatUser == null)
                ShowError(new UserNotExistsError("to", m_ToUserID));

            m_ChatMessageList = ChatBO.Instance.GetLastChatMessages(MyUserID, m_ToUserID, 0, 20);
        }


        protected bool HasSound
        {
            get
            {
                return AllSettings.Current.ChatSettings.HasMessageSound;
            }
        }

        protected string BgSound
        {
            get
            {
                return AllSettings.Current.ChatSettings.MessageSound;
            }
        }

        protected int ToUserID
        {
            get
            {
                return this.m_ToUserID;
            }
        }

        protected bool CanUseDefaultEmotcion
        {
            get
            {
              //   ,AllSettings.Current.ChatSettings.EnableUserEmoticon
                return AllSettings.Current.ChatSettings.EnableDefaultEmoticon;
            }
        }

        protected bool CanUseUserEmoticon
        {
            get
            {
                return AllSettings.Current.ChatSettings.EnableUserEmoticon;
            }
        }

        protected int MaxMessageID
        {
            get
            {
                if (this.ChatMessageList.Count > 0)
                {
                    return this.ChatMessageList[ChatMessageList.Count - 1].MessageID;
                }
                return 0;
            }
        }

        /// <summary>
        /// 对话列表
        /// </summary>
        protected ChatMessageCollection ChatMessageList
        {
            get { return m_ChatMessageList; }
        }

        protected SimpleUser ChatUser
        {
            get { return m_ChatUser; }
        }

        protected bool CanShowAddFriendLink
        {
            get { return FriendBO.Instance.IsFriend(MyUserID, m_ToUserID); }
        }

        private string lastDay = ""; 
        protected bool NewDay( DateTime datetime )
        {
            string day = datetime.ToString("yyyyMMdd");
            if (lastDay != day)
            {
                lastDay = day;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 每页显示多少条数据
        /// </summary>
        protected int PageSize
        {
            get { return c_PageSize; }
        }

        protected string validateActionName
        {
            get { return "sendmessage"; }
        }
    }    
}