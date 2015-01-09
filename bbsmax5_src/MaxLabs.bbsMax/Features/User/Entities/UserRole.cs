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

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class UserRole  : IPrimaryKey<Guid>, IComparable<UserRole>
    {
        public UserRole()
        {
            this.BeginDate = DateTime.MinValue;
            this.EndDate = DateTime.MaxValue;
        }

        public UserRole(DataReaderWrap readerWrap)
        {
            this.RoleID = readerWrap.Get<Guid>("RoleID");
            this.BeginDate = readerWrap.Get<DateTime>("BeginDate");
            this.EndDate = readerWrap.Get<DateTime>("EndDate");
            this.UserID = readerWrap.Get<int>("UserID");
        }

        public int UserID { get; set; }

        public Guid RoleID { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }


        #region 扩展的属性

        public string Username
        {
            get;
            set;
        }

        //private int? m_RoleIdentityID;
        public int RoleIdentityID
        {
            get
            {
  //              if (m_RoleIdentityID == null)
   //                 m_RoleIdentityID = Role.IdentityID;
  //              return m_RoleIdentityID.Value;
                return 1;
            }
        }

        public Role Role
        {
            get
            {
                return AllSettings.Current.RoleSettings.Roles.GetValue(RoleID);
            }
        }

        public string RoleName
        {
            get
            {
                Role role = this.Role;
                if (role == null)
                    return RoleID.ToString();
                else
                    return role.Name;
            }
        }

        public string IconUrl
        {
            get
            {
                Role role = this.Role;
                if (role == null)
                    return string.Empty;
                else
                    return role.IconUrl;
            }
        }

        public string Title
        {
            get
            {
                Role role = this.Role;
                if (role == null)
                    return string.Empty;
                else
                    return role.Title;
            }
        }

        public int StarLevel
        {
            get
            {
                Role role = this.Role;
                if (role == null)
                    return 0;
                else
                    return role.StarLevel;
            }
        }


        public int Level
        {
            get
            {
                Role role = this.Role;
                if (role == null)
                    return int.MinValue;
                else
                    return role.Level;
            }
        }

        #endregion


        #region IPrimaryKey<string> 成员

        public Guid GetKey()
        {
            return RoleID;
        }

        #endregion


        #region IComparable<UserRole> 成员

        public int CompareTo(UserRole other)
        {
            if (other == null)
                return 1;

            Role thisRole = this.Role;
            Role otherRole = other.Role;

            if (thisRole == null && otherRole == null)
                return 0;

            else if (thisRole == null)
                return -1;

            return thisRole.CompareTo(otherRole);
        }

        #endregion
    }

    public class UserRoleCollection : EntityCollectionBase<Guid, UserRole>
    {

        /// <summary>
        /// 是否检查不同的用户
        /// </summary>
        private bool UnCheck = false;

        public UserRoleCollection(bool unCheck)
        {
            this.UnCheck = unCheck;
        }

        public UserRoleCollection() { }
        

        public override void Add(UserRole item)
        {
            if (this.Count > 0)
            {

                if (UnCheck == false)
                {
                    if (this[0].UserID != item.UserID)
                    {
                        throw new ArgumentException("不支持在一个列表中添加不同用户的UserRole实例", "item");
                    }
                }
            }
            base.Add(item);
        }

        public void Add(int userID, Guid roleID)
        {
            if (this.Count > 0)
            {

                if (UnCheck == false)
                {
                    if (this[0].UserID != userID)
                    {
                        throw new ArgumentException("不支持在一个列表中添加不同用户的UserRole实例", "item");
                    }
                }
            }

            UserRole userRole = new UserRole();
            userRole.UserID = userID;
            userRole.RoleID = roleID;
            userRole.BeginDate = DateTime.MinValue;
            userRole.EndDate = DateTime.MaxValue;

            base.Add(userRole);
        }

        public void AddRange( UserRoleCollection userRolos )
        {
            foreach (UserRole ur in userRolos)
            {
                Add(ur);
            }
        }

        public UserRoleCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new UserRole(readerWrap));
            }
        }

        public void Remove( Guid roleid )
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].RoleID == roleid)
                {
                    this.RemoveAt(i);
                    break;
                }
            }
        }

        public UserRoleCollection(int defaultRoleUserID, params Role[] defaultRoles)
        {
            FillDefaultRoles(defaultRoleUserID, defaultRoles);
        }

        public UserRoleCollection(DataReaderWrap readerWrap, int defaultRoleUserID, params Role[] defaultRoles) 
        {
            FillDefaultRoles(defaultRoleUserID, defaultRoles);

            while (readerWrap.Next)
            {
                this.Add(new UserRole(readerWrap));
            }
        }


        private void FillDefaultRoles(int defaultRoleUserID, Role[] defaultRoles)
        {
            if (defaultRoles != null)
            {
                foreach (Role role in defaultRoles)
                {
                    UserRole userRole = new UserRole();
                    userRole.RoleID = role.RoleID;
                    userRole.UserID = defaultRoleUserID;
                    userRole.BeginDate = DateTime.MinValue;
                    userRole.EndDate = DateTime.MaxValue;

                    this.Add(userRole);
                }
            }
        }

        public bool IsInRole(Guid roleID)
        {
            UserRole userRole;

            DateTime now = DateTimeUtil.Now;
            if (this.TryGetValue(roleID, out userRole))
            {
                if (userRole.BeginDate <= now && now <= userRole.EndDate)
                    return true;
            }

            return false;
        }

        public bool IsInRole(Role role)
        {
            UserRole userRole;

            DateTime now = DateTimeUtil.Now;
            if (this.TryGetValue(role.RoleID, out userRole))
            {
                if (userRole.BeginDate <= now && now <= userRole.EndDate)
                    return true;
            }

            return false;

        }

        public List<int> GetUserIds()
        {
            List<int> userIds = new List<int>();

            foreach (UserRole userRole in this)
            {
                if (userIds.Contains(userRole.UserID) == false)
                    userIds.Add(userRole.UserID);
            }

            return userIds;
        }

        //public Role GetMaxRole()
        //{
        //    if (this.Count == 0)
        //        return Role.Everyone;

        //    Role maxRole = Role.Everyone;

        //    DateTime now = DateTimeUtil.Now;
        //    foreach (UserRole userRole in this)
        //    {
        //        if (userRole.BeginDate <= now && now <= userRole.EndDate && userRole.Role > maxRole)
        //            maxRole = userRole.Role;
        //    }

        //    return maxRole;
        //}

        
        private string m_JoinedIds = null;
        public string GetJoinedIds()
        {
            if (m_JoinedIds == null)
            {
                string joinedIds = string.Empty;

                for (int i = 0; i < this.Count; i++)
                {
                    if (i > 0)
                        joinedIds += ",";

                    joinedIds += this[i].RoleID.ToString();
                }

                m_JoinedIds = joinedIds;
            }

            return m_JoinedIds;
        }

        internal bool IsInManagerRole { get; set; }

        internal bool IsInOwnersRole { get; set; }

        internal bool CanLoginConsole { get; set; }

        internal Role RoleForTitle { get; set; }
    }
}