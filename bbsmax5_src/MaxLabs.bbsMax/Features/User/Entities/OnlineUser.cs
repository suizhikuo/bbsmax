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
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 在线用户对象
    /// </summary>
    public abstract class OnlineUser<TK> : IPrimaryKey<TK>
        where TK : IComparable<TK>
    {
        #region 基本属性

        public TK ID { get; set; }

        /// <summary>
        /// 该用户当前的动作
        /// </summary>
        public OnlineAction Action { get; set; }

        /// <summary>
        /// 使用的操作系统
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// 使用的浏览器
        /// </summary>
        public string Browser { get; set; }

        /// <summary>
        /// 访问者IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 访问者所在地
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 所在的版块ID
        /// </summary>
        public int ForumID { get; set; }

        /// <summary>
        /// 所在的版块名(去除html的)
        /// </summary>
        public string ForumName { get; set; }

        /// <summary>
        /// 所在的主题ID
        /// </summary>
        public int ThreadID { get; set; }

        /// <summary>
        /// 所在的主题标题
        /// </summary>
        public string ThreadSubject { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }



        #endregion

        #region IPrimaryKey<TK> 成员

        public TK GetKey()
        {
            return ID;
        }

        #endregion
    }

    public abstract class OnlineUserCollection<TK, TV> : EntityCollectionBase<TK, TV>
        where TK : IComparable<TK>
        where TV : OnlineUser<TK>
    {
        public OnlineUserCollection()
            : base()
        { }

        public OnlineUserCollection(IEnumerable<TV> list)
            : base(list)
        { }
    }

    #region OnlineMember

    /// <summary>
    /// 在线的已登陆用户
    /// </summary>
    public class OnlineMember : OnlineUser<int> , IFillSimpleUser
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID
        {
            get { return ID; }
            set { ID = value; }
        }

        /// <summary>
        /// 是否隐身
        /// </summary>
        public bool IsInvisible { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户角色的序号  按此取在线角色图标
        /// </summary>
        public int RoleSortOrder { get; set; }

        /// <summary>
        /// 用户角色 逗号分割的RoleIdentityID,从小到大
        /// </summary>
        public string RoleIdentityIDString { get; set; }

        /// <summary>
        /// 显示的名字
        /// </summary>
        public string Display { get; set; }

        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetFilledSimpleUser(UserID);
            }
        }

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return UserID;
        }

        #endregion
    }

    public class OnlineMemberCollection : OnlineUserCollection<int, OnlineMember>
    {
        public OnlineMemberCollection()
            : base()
        { }

        public OnlineMemberCollection(IEnumerable<OnlineMember> list)
            : base(list)
        { }
    }

    #endregion

    #region OnlineGuest

    /// <summary>
    /// 在线的游客
    /// </summary>
    public class OnlineGuest : OnlineUser<string>
    {
        /// <summary>
        /// 游客ID
        /// </summary>
        public string GuestID
        {
            get { return ID; }
            set { ID = value; }
        }

        /// <summary>
        /// 该游客是否蜘蛛
        /// </summary>
        public bool IsSpider { get; set; }

        private StringTable m_TempDataBox = null;

        /// <summary>
        /// 保存游客的临时数据的容器
        /// </summary>
        public StringTable TempDataBox
        {
            get
            {
                if (m_TempDataBox == null)
                    m_TempDataBox = new StringTable();

                return m_TempDataBox;
            }
            set
            {
                m_TempDataBox = value;
            }
        }
    }

    public class OnlineGuestCollection : OnlineUserCollection<string, OnlineGuest>
    {
        public OnlineGuestCollection()
            : base()
        { }

        public OnlineGuestCollection(IEnumerable<OnlineGuest> list)
            : base(list)
        { }
    }

    #endregion

    public class SimpleOnlineMember
    {
        public int UserID { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsInvisible { get; set; }
    }

    public class OnlineGuestsInIp// : IPrimaryKey<string>
    {
        //public string Ip { get; set; }

        public bool IsSpider { get; set; }

        private List<string> m_GuestIds;
        public List<string> GuestIds 
        {
            get 
            {
                if (m_GuestIds == null)
                    m_GuestIds = new List<string>();
                return m_GuestIds;
            }
            set { m_GuestIds = value; } 
        }
    }
}