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
using MaxLabs.bbsMax.Enums;


namespace MaxLabs.bbsMax.Settings
{

    public sealed class ExceptionItem<T> : SettingBase, IPrimaryKey<Guid> //, ICloneable
	{
        public ExceptionItem()
        {
            //if(typeof(T))
            //Value = value;
        }

        public ExceptionItem(Guid roleID, int sortOrder, T value)
        {
            RoleID = roleID;
            SortOrder = sortOrder;
            Value = value;
        }

        [SettingItem]
        public Guid RoleID { get; set; }

        [SettingItem]
        public int SortOrder { get; set; }

        [SettingItem]
        public T Value { get; set; }

        [SettingItem]
        public LevelStatus LevelStatus { get; set; }

        #region IPrimaryKey<string> 成员

        public Guid GetKey()
        {
            return RoleID;
        }

        #endregion

    }

    public class Exceptable<T> : HashedCollectionBase<Guid, ExceptionItem<T>>, ISettingItem
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">默认值 不能为null</param>
        public Exceptable(T value)
        {
            if (typeof(T).IsValueType || ReflectionUtil.HasInterface(typeof(T), typeof(IExceptableSettingItem)))
            {
                ExceptionItem<T> item = new ExceptionItem<T>(Guid.Empty, 0, value);
                this.Insert(0, item);
            }
            else
                throw new ArgumentException("只支持值类型和IExceptableSettingItem接口");
        }

        public override void Add(ExceptionItem<T> item)
        {
            if (item.RoleID == Guid.Empty)
            {
                if (this[0].RoleID == Guid.Empty)
                    this[0] = item;
                else
                    this.Insert(0, item);
            }
            else if (this.Count == 1 && this[0].RoleID == Guid.Empty)
            {
                this.Insert(1, item);
            }
            else
            {
                int index = 0;
                for (int i = 1; i < this.Count + 1; i++)
                {
                    if (item.SortOrder > this[i - 1].SortOrder)
                    {
                        index = i;
                    }
                    if (item.SortOrder < this[i - 1].SortOrder)
                        break;
                }
                if (index >= this.Count)
                {
                    base.Add(item);
                }
                else
                    base.Insert(index, item);
            }
        }

        [Obsolete("请使用xxx[My]获得更好的性能")]
        public T GetValue(int userID)
        {
            if (this.Count == 1)//没有例外
                return this[0].Value;

            User user = UserBO.Instance.GetUser(userID, GetUserOption.WithAll);
            
            if(user == null)
                return this[0].Value;

            return GetValue(user);
        }

        [Obsolete("请使用xxx[My]获得更好的性能")]
        public T GetValue(User my)
        {
            UserRoleCollection roles = my.Roles;
            Role maxRole = my.MaxRole;

            RoleCollection systemRoles = AllSettings.Current.RoleSettings.Roles;

            foreach (ExceptionItem<T> item in this)
            {
                if (item.LevelStatus == LevelStatus.Above)
                {
                    Role role;
                    if (systemRoles.TryGetValue(item.RoleID, out role) == false)
                        continue;

                    if (maxRole >= role)
                        return item.Value;
                }
                else if (item.LevelStatus == LevelStatus.Below)
                {
                    Role role;
                    if (systemRoles.TryGetValue(item.RoleID, out role) == false)
                        continue;

                    if (maxRole <= role)
                        return item.Value;
                }
                else
                {
                    foreach (UserRole role in roles)
                    {
                        if (role.RoleID == item.RoleID)
                            return item.Value;
                    }
                }
            }

            return this[0].Value;
        }


        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (ExceptionItem<T> item in this)
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
                bool isValueType = typeof(T).IsValueType;

                foreach (string item in list)
                {
                    ExceptionItem<T> exceptionItem = new ExceptionItem<T>();  //new ExceptionItem<T>(Guid.Empty,0);

                    if (isValueType)
                        exceptionItem.Value = this[0].Value;
                    else
                    {
                        exceptionItem.Value = (T)((IExceptableSettingItem)this[0].Value).Clone();//;.Clone
                    }

                    exceptionItem.Parse(item);

                    this.Add(exceptionItem);

                }

                //Sort();
            }
        }

        #endregion

        public T this[AuthUser my]
        {
            get { return GetValue(my); }
        }
    }



    //public enum Role
}