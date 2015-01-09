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
using System.Web;
using System.Collections.ObjectModel;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Templates
{

    [TemplatePackage]
    public class TemplateExtensions
    {
        #region IPrintUser 扩展

        public class TemplateUserInfo
        {
            private int m_UserID;
            private User m_user;

            public TemplateUserInfo(int userID)
            {
                m_UserID = userID;
                if (m_UserID > 0)
                {
                    m_user = UserBO.Instance.GetUser(m_UserID);
                }
                else
                {
                    m_user = new User();
                    m_user.Username = "";
                    m_user.Realname = "";
                }
            }

            public string OnlineIcon { get { return "<img src=\"在线?不在线.gif\" alt=\"\" />"; } }
        }

        [TemplateExtensionProperty("Print")]
        public static string InviteSerialStatus_Print(InviteSerialStatus status, string name)
        {
            switch (status)
            {
                case InviteSerialStatus.Used:
                    return Lang.Common_Used;
                case InviteSerialStatus.Expires:
                    return Lang.Common_Expires;
                case InviteSerialStatus.Unused:
                    return Lang.Common_Unused;
            }
            return Lang.Common_Unused;
        }

        //[TemplateExtensionProperty("User")]
        //public static TemplateUserInfo Message_User(Message value, string name)
        //{
        //    return new TemplateUserInfo(value.UserID);
        //}


        #endregion


        #region DateTime

        [TemplateExtensionProperty("Friendly")]
        public static string DateTime_Friendly(DateTime value, string name)
        {
            float timeDiffrence;
            timeDiffrence = UserBO.Instance.GetUserTimeDiffrence(User.Current);
            return DateTimeUtil.GetFriendlyDate(value);
        }

        [TemplateExtensionProperty("ForEdit")]
        public static string DateTime_ForInput(DateTime value, string name)
        {
            if (value == DateTime.MaxValue || value == DateTime.MinValue)
                return string.Empty;
            return value.ToString();
        }

        #endregion


    }
}