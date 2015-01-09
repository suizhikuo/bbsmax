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

namespace MaxLabs.bbsMax.Entities
{
    public class TimeLimitCollectionBase<TK, TV, TCollection> : EntityCollectionBase<TK, TV>
        where TK : IComparable<TK>
        where TV : IPrimaryKey<TK>, ITimeLimit
        where TCollection : TimeLimitCollectionBase<TK, TV, TCollection>, new()
    {
        object locker = new object();
        private DateTime m_NextChangeTime = DateTime.MaxValue;

        private TCollection m_Limited = null;
        private bool m_IsLimitedObject = false;

        public TCollection Limited
        {
            get
            {
                if (m_IsLimitedObject)
                    throw new NotSupportedException("Collection is Limited.");

                TCollection result = m_Limited;

                if (result == null || m_NextChangeTime <= DateTimeUtil.Now)
                {
                    #region 重新生成有效范围内的列表

                    lock (locker)
                    {
                        DateTime nextChangeTime = DateTime.MaxValue;

                        result = new TCollection();


                        DateTime now = DateTimeUtil.Now;

                        foreach (TV item in this)
                        {
                            if (item.BeginDate <= now && now <= item.EndDate)
                                result.Add(item);

                            if (item.BeginDate > now && item.BeginDate < nextChangeTime)
                                nextChangeTime = item.BeginDate;

                            if (item.EndDate > now && item.EndDate < nextChangeTime)
                                nextChangeTime = item.BeginDate;
                        }

                        result.SetReadOnly();
                        result.m_IsLimitedObject = true;

                        m_Limited = result;
                        m_NextChangeTime = nextChangeTime;
                    }

                    #endregion
                }

                return result;
            }
        }

        public TCollection GetListWithoutExpired()
        {
            if (m_IsLimitedObject)
                throw new NotSupportedException("Collection is Limited.");

            TCollection result = new TCollection();

            DateTime now = DateTimeUtil.Now;

            foreach (TV item in this)
            {
                if (now <= item.EndDate)
                    result.Add(item);
            }

            result.SetReadOnly();
            result.m_IsLimitedObject = true;

            return result;
        }

        public override void Add(TV item)
        {
            lock (locker)
            {
                base.Add(item);
                m_Limited = null;
            }
        }

        public override void AddRange(IEnumerable<TV> collection)
        {
            lock (locker)
            {
                base.AddRange(collection);
                m_Limited = null;
            }
        }

        public override void Clear()
        {
            lock (locker)
            {
                base.Clear();
                m_Limited = null;
            }
        }

        public override void Insert(int index, TV item)
        {
            lock (locker)
            {
                base.Insert(index, item);
                m_Limited = null;
            }
        }

        public override bool Remove(TV item)
        {
            lock (locker)
            {
                bool result = base.Remove(item);
                m_Limited = null;
                return result;
            }
        }

        public override void RemoveAt(int index)
        {
            lock (locker)
            {
                base.RemoveAt(index);
                m_Limited = null;
            }
        }

        public override bool RemoveByKey(TK key)
        {
            lock (locker)
            {
                bool result = base.RemoveByKey(key);
                m_Limited = null;
                return result;
            }
        }

        public override void Set(TV item)
        {
            lock (locker)
            {
                base.Set(item);
                m_Limited = null;
            }
        }

        public override void Sort()
        {
            lock (locker)
            {
                base.Sort();
                m_Limited = null;
            }
        }

        public override void Sort(IComparer<TV> comparer)
        {
            lock (locker)
            {
                base.Sort(comparer);
                m_Limited = null;
            }
        }

        public override TV this[int index]
        {
            set
            {
                lock (locker)
                {
                    base[index] = value;
                    m_Limited = null;

                }
            }
        }
    }
}