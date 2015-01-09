//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Entities
{

	public abstract class EntityCollectionBase<TK, TV> : ICollection<TV>
		where TK : IComparable<TK>
		where TV : IPrimaryKey<TK>
	{
        private bool m_IsReadOnly = false;
        private IEqualityComparer<TK> m_Comparer;

		List<TV> innerList;
        private int m_TotalRecords = -1;
        private object locker = new object();

        public EntityCollectionBase()
            : this(null, null)
        { }

        public EntityCollectionBase(IEnumerable<TV> list)
            : this(list, null)
        { }

        public EntityCollectionBase(IEqualityComparer<TK> comparer)
            : this(null, comparer)
        { }

        public EntityCollectionBase(IEnumerable<TV> list, IEqualityComparer<TK> comparer)
        {
            if (list == null)
                innerList = new List<TV>();
            else
                innerList = new List<TV>(list);

            if (comparer == null)
                comparer = EqualityComparer<TK>.Default;

            m_Comparer = comparer;
        }

        public void SetReadOnly()
        {
            this.m_IsReadOnly = true;

        }

		#region ICollection<EntityBase<TK>> 成员

		public virtual void Add(TV item)
		{
            if (item == null)
                throw new ArgumentNullException("item","不允许为null");

            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

			innerList.Add(item);
		}

		public virtual void Clear()
		{
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

			innerList.Clear();
		}

		public virtual bool Contains(TV item)
		{
			return innerList.Contains(item);
		}

		public virtual bool ContainsKey(TK key)
		{
			for (int i = 0; i < innerList.Count; i ++)
			{
                TV v = innerList[i];

                if (m_Comparer.Equals(v.GetKey(), key))
                    return true;
			}

			return false;
		}

		public virtual void CopyTo(TV[] array, int arrayIndex)
		{
			innerList.CopyTo(array, arrayIndex);
		}

		public virtual int Count
		{
			get { return innerList.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return m_IsReadOnly; }
		}

		public virtual bool Remove(TV item)
		{
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

			bool result = innerList.Remove(item);

            return result;
		}

		#endregion

		#region IEnumerable<TV> 成员

		public virtual IEnumerator<TV> GetEnumerator()
		{
			return innerList.GetEnumerator();
		}

		#endregion

		#region IEnumerable 成员

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

        public virtual bool RemoveByKey(TK key)
        {
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            int deleteIndex = -1;

            int i = 0;

            lock (locker)
            {
                foreach (TV item in innerList)
                {
                    if (m_Comparer.Equals(item.GetKey(), key))
                    {
                        deleteIndex = i;
                        break;
                    }

                    i++;
                }

                if (deleteIndex != -1)
                {
                    RemoveAt(deleteIndex);
                    return true;
                }
            }

            return false;
        }

        public virtual void Sort()
        {
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            innerList.Sort();
        }

        public virtual void Sort(IComparer<TV> comparer)
        {
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            innerList.Sort(comparer);
        }

		/// <summary>
		/// 分页查询时的总记录数
		/// </summary>
        public virtual int TotalRecords
        {
            get { return m_TotalRecords; }
            set { m_TotalRecords = value; }
        }

        public virtual TK[] GetKeys()
        {
            TK[] keys = new TK[this.Count];

            for (int i = 0; i < this.Count; i++)
            {
                keys[i] = this[i].GetKey();
            }

            return keys;
        }

        /// <summary>
        /// 合并两个集合， 这里没有做重复数据的排除检查
        /// </summary>
        /// <param name="collection"></param>
        public virtual void AddRange(IEnumerable<TV> collection)
        {
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            foreach (TV value in collection)
            {
                Add(value);
            }
        }

		public virtual TV GetValue(TK key)
		{
			for (int i = 0; i < innerList.Count; i ++)
			{
                TV item = innerList[i];

                if (m_Comparer.Equals(item.GetKey(), key))
                    return item;
			}

			return default(TV);
		}

        public virtual bool TryGetValue(TK key, out TV value)
        {

            for (int i = 0; i < innerList.Count; i++)
            {
                TV item = innerList[i];

                if (m_Comparer.Equals(item.GetKey(), key))
                {
                    value = item;
                    return true;
                }
            }

            value = default(TV);

            return false;
        }

        /// <summary>
        /// 修改某一项的值，如果集合中已经存在同一个key的项，将直接将其覆盖，否则会添加到集合尾部
        /// </summary>
        /// <param name="item"></param>
		public virtual void Set(TV item)
		{
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

			TK key = item.GetKey();
			for (int i = 0; i < innerList.Count; i++)
			{
                if (m_Comparer.Equals(innerList[i].GetKey(), key))
				{
					innerList[i] = item;
					return;
				}
			}

            innerList.Add(item);
			//throw new ArgumentException("集合中并不存在键", "key");
		}

		public virtual void Insert(int index, TV item)
		{
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

			innerList.Insert(index, item);
		}

        public void Reverse() {
            this.innerList.Reverse();
        }

		public virtual void RemoveAt(int index)
		{
            if (m_IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

			innerList.RemoveAt(index);
		}

		public virtual TV this[int index]
		{
			get
			{
				return innerList[index];
			}
			set
			{
                if (m_IsReadOnly)
                    throw new NotSupportedException("Collection is readonly.");

				innerList[index] = value;
			}
		}
	}
}