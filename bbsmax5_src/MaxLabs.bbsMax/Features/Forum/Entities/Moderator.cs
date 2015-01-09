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
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class Moderator : IComparable<Moderator>, IBatchSave, IFillSimpleUser, IPrimaryKey<string>, ITimeLimit
    {
        public int UserID { get; set; }
        public int ForumID { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public ModeratorType ModeratorType { get; set; }
        public int AppointorID { get; set; }
        public int SortOrder { get; set; }

        public Moderator()
        { }

        public Moderator(DataReaderWrap readerWrap)
        {
            this.ForumID = readerWrap.Get<int>("ForumID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.BeginDate = readerWrap.Get<DateTime>("BeginDate");
            this.EndDate = readerWrap.Get<DateTime>("EndDate");
            this.ModeratorType = readerWrap.Get<ModeratorType>("ModeratorType");
            this.SortOrder = readerWrap.Get<int>("SortOrder");
        }

        #region 扩展属性
        public SimpleUser User
        {
            get
            {
                return UserBO.Instance.GetSimpleUser(this.UserID);
            }
        }

        public SimpleUser Appointor
        {
            get
            {
                return UserBO.Instance.GetSimpleUser(this.AppointorID);
            }
        }

        public string Name
        {
            get
            {
                switch (ModeratorType)
                {
                    case ModeratorType.CategoryModerators:
                        return "分类版主";

                    case ModeratorType.JackarooModerators:
                        return "实习版主";

                    default:
                        return "版主";
                }
            }
        }

        #endregion

        public int CompareTo(Moderator m)
        {
            if (this.ModeratorType == ModeratorType.CategoryModerators  )
            {
                if (m.ModeratorType != ModeratorType.CategoryModerators)
                    return -1;
            }

            if (m.ModeratorType == ModeratorType.CategoryModerators)
                if (this.ModeratorType != ModeratorType.CategoryModerators)
                    return 1;

            return this.SortOrder.CompareTo(m.SortOrder);
        }

        #region IBatchSave 成员

        public bool IsNew
        {
            get;
            set;
        }

        #endregion

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.UserID;
        }

        #endregion

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return this.ForumID + "-" + this.UserID;
        }

        #endregion
    }

    public class ModeratorCollection : HashedTimeLimitCollectionBase<string, Moderator, ModeratorCollection>
    {
        private object locker = new object();
        private Dictionary<int, ModeratorType> m_CachedUserModeratorsTypes = null;

        public ModeratorCollection()
        { }

        public ModeratorCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new Moderator(readerWrap));
            }
        }

        public ModeratorType GetUserModeratorType(int userID)
        {
            Dictionary<int, ModeratorType> userModeratorsTypes = m_CachedUserModeratorsTypes;

            if (userModeratorsTypes == null)
            {
                #region 重建用户在本列表中的版主类型

                lock (locker)
                {
                    userModeratorsTypes = new Dictionary<int, ModeratorType>();

                    foreach (Moderator moderator in this)
                    {
                        if (userModeratorsTypes.ContainsKey(moderator.UserID))
                            userModeratorsTypes[moderator.UserID] |= moderator.ModeratorType;

                        else
                            userModeratorsTypes.Add(moderator.UserID, moderator.ModeratorType);
                    }

                    m_CachedUserModeratorsTypes = userModeratorsTypes;
                }

                #endregion
            }

            ModeratorType result;

            if (userModeratorsTypes.TryGetValue(userID, out result))
                return result;

            return ModeratorType.None;
        }


        #region 重写基类中的方法，给所有的写入操作加锁，目的是为了避免重建缓存时冲突

        public override void Add(Moderator item)
        {
            lock (locker)
            {
                base.Add(item);
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override void AddRange(IEnumerable<Moderator> collection)
        {
            lock (locker)
            {
                base.AddRange(collection);
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override void Clear()
        {
            lock (locker)
            {
                base.Clear();
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override void Insert(int index, Moderator item)
        {
            lock (locker)
            {
                base.Insert(index, item);
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override bool Remove(Moderator item)
        {
            lock (locker)
            {
                m_CachedUserModeratorsTypes = null;
                return base.Remove(item);
            }
        }

        public override void RemoveAt(int index)
        {
            lock (locker)
            {
                base.RemoveAt(index);
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override bool RemoveByKey(string key)
        {
            lock (locker)
            {
                m_CachedUserModeratorsTypes = null;
                return base.RemoveByKey(key);
            }
        }

        public override void Set(Moderator item)
        {
            lock (locker)
            {
                base.Set(item);
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override void Sort()
        {
            lock (locker)
            {
                base.Sort();
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override void Sort(IComparer<Moderator> comparer)
        {
            lock (locker)
            {
                base.Sort(comparer);
                m_CachedUserModeratorsTypes = null;
            }
        }

        public override Moderator this[int index]
        {
            set
            {
                lock (locker)
                {
                    base[index] = value;
                    m_CachedUserModeratorsTypes = null;
                }
            }
        }

        #endregion

    }

}