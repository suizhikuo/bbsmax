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


using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;
using System.Web;


namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 只有最基本信息的用户实体
    /// </summary>
    public partial class SimpleUser : IPrimaryKey<int>
    {

        public SimpleUser()
        {

        }

        public SimpleUser(DataReaderWrap readerWrap)
        {
            this.UserID = readerWrap.Get<int>("UserID");
            this.Username = HttpUtility.HtmlEncode(readerWrap.Get<string>("Username"));
            this.Realname = readerWrap.Get<string>("Realname");
            this.Doing = readerWrap.Get<string>("Doing");
            this.m_AvatarPropFlag = new PropFlag(readerWrap.Get<string>("AvatarSrc"));
            this.Gender = readerWrap.Get<Gender>("Gender");
        }



        static SimpleUser()
        {
            SimpleUser guestUser = new SimpleUser();
            guestUser.UserID = 0;
            guestUser.Username = "Guest";
            Guest = guestUser;
        }


        public bool IsDeleted { get; set; }

        public int UserID { get; set; }

        [Obsolete]
        public int ID { get { return UserID; } }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 是否通过实名认证
        /// </summary>
        public bool RealnameChecked { get { return !string.IsNullOrEmpty(this.Realname); } }

        /// <summary>
        ///真实姓名
        /// </summary>
        public string Realname { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Doing { get; set; }

#if !Passport

        /// <summary>
        /// 当前用户是否在线（不包括隐身）
        /// </summary>
        public bool IsOnline
        {
            get { return OnlineUserPool.Instance.IsOnline(UserID); }
        }

        /// <summary>
        /// 当前用户是否隐身
        /// </summary>
        public bool IsInvisible
        {
            get { return OnlineUserPool.Instance.IsInvisible(UserID); }
        }

        /// <summary>
        /// 当前用户是否在线或隐身
        /// </summary>
        public bool IsOnlineOrInvisible
        {
            get { return OnlineUserPool.Instance.IsOnlineOrInvisible(UserID); }
        }

#endif

        #region 头像相关

        /// <summary>
        /// 用户的头像
        /// </summary>
        public string AvatarSrc 
        { 
            get
            {
                return AvatarPropFlag.OriginalData;
            }
            set
            {
                AvatarPropFlag.OriginalData = value;
                ClearAvatarCache();
            }
        }

        private PropFlag m_AvatarPropFlag;
        public PropFlag AvatarPropFlag
        {
            get
            {
                if (m_AvatarPropFlag == null)
                    m_AvatarPropFlag = new PropFlag();
                return m_AvatarPropFlag;
            }
        }

        #endregion

        #region 显示的名称

        /// <summary>
        /// 用户显示的名称
        /// </summary>
        public string Name
        {
            get
            {
                //if (string.IsNullOrEmpty(Realname))
                    return this.Username;

                //return Realname;
            }
        }

        string m_NameLink = null;
        /// <summary>
        /// 用户显示的名称（含链接）
        /// </summary>
        public string NameLink
        {
            get
            {
                if (m_NameLink == null)
                    m_NameLink = UserID <= 0 ? string.Empty : string.Concat("<a href=\"", BbsRouter.GetUrl("space/" + UserID), "\">", Name, "</a>");

                return m_NameLink;
            }
        }

        private string m_PopupNameLink = null;
        /// <summary>
        /// 用户显示的名称（含在新窗口打开的链接）
        /// </summary>
        public string PopupNameLink
        {
            get
            {
                if (m_PopupNameLink == null)
                    m_PopupNameLink = UserID <= 0 ? string.Empty : string.Concat("<a href=\"", BbsRouter.GetUrl("space/" + UserID), "\" target=\"_blank\">", Name, "</a>");

                return m_PopupNameLink;
            }
        }

        #endregion

        #region 性别名称

        /// <summary>
        /// 性格名称
        /// </summary>
        public string GenderName
        {
            get
            {
                switch (Gender)
                {
                    case Gender.Female:
                        return Lang.Gender_Female;
                    case Gender.Male:
                        return Lang.Gender_Male;
                    case Gender.NotSet:
                        return Lang.Gender_NotSet;
                }
                return string.Empty;
            }
        }
        #endregion

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return UserID;
        }

        #endregion

        public static SimpleUser BuildDeletedUser(int userID)
        {
            SimpleUser deletedUser = new SimpleUser();
            deletedUser.UserID = userID;
            deletedUser.Username = "Deleted User";
            deletedUser.IsDeleted = true;
            return deletedUser;
        }

        public static SimpleUser Guest { get; private set; }
    }

    /// <summary>
    /// 只有最基本信息的用户实体的集合
    /// </summary>
    public class SimpleUserCollection : EntityCollectionBase<int, SimpleUser>, IStringConverter<SimpleUser>
    {
        public SimpleUserCollection()
        { }

        public SimpleUserCollection(DataReaderWrap wrap)
        {
            while (wrap.Next)
            {
                this.Add(new SimpleUser(wrap));
            }
        }

        /// <summary>
        /// 解析用户ID集字符串为一个SimpleUserCollection
        /// </summary>
        public static SimpleUserCollection Parse(string userIDsString)
        {
            SimpleUserCollection simpleUsers = new SimpleUserCollection();
            simpleUsers.ConvertFromString(userIDsString);
            return simpleUsers;
        }

        #region IStringConverter<SimpleUser> 成员

        /// <summary>
        /// 将当前SimpleUserCollection转换为字符串
        /// </summary>
        /// <returns>ID集字符串</returns>
        public string ConvertToString()
        {
            StringBuffer simpleUserIDs = new StringBuffer();

            for (int i = 0; i < this.Count; i++)
            {
                simpleUserIDs += this[i].UserID.ToString();
                if (i < this.Count - 1)
                {
                    simpleUserIDs += ",";
                }
            }

            return simpleUserIDs.ToString();
        }

        /// <summary>
        /// 根据传入的ID字符串,往当前SimpleUserCollection添加SimpleUser
        /// </summary>
        public void ConvertFromString(string valueString)
        {
            if (string.IsNullOrEmpty(valueString))
            {
                return;
            }
            int[] userIDsArray = StringUtil.Split<int>(valueString);

            foreach (int userID in userIDsArray)
            {
                SimpleUser simpleUser = new SimpleUser();
                simpleUser.UserID = userID;
                this.Add(simpleUser);
            }
        }

        #endregion
    }
}