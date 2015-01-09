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

	public abstract class HashedCollectionBase<TK, TV> : EntityCollectionBase<TK, TV>
		where TK : IComparable<TK>
		where TV : IPrimaryKey<TK>
	{
        Dictionary<TK, TV> innerDictionary = null;
        object locker = new object();

        public HashedCollectionBase()
            : this(null, null)
        { }

        public HashedCollectionBase(IEnumerable<TV> list)
            : this(list, null)
        { }

        public HashedCollectionBase(IEqualityComparer<TK> comparer)
            : this(null, comparer)
        { }

        public HashedCollectionBase(IEnumerable<TV> list, IEqualityComparer<TK> comparer) : base(list, comparer)
        {
            if (comparer == null)
                comparer = EqualityComparer<TK>.Default;

            innerDictionary = new Dictionary<TK, TV>(comparer);

            if (list != null)
            {
                foreach (TV item in list)
                {
                    innerDictionary.Add(item.GetKey(), item);
                }
            }
        }

        public override void Add(TV item)
        {
            if (item == null)
                throw new ArgumentNullException("item", "不允许为null");

            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                innerDictionary.Add(item.GetKey(), item);
                base.Add(item);
            }
        }

        public override void AddRange(IEnumerable<TV> collection)
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                foreach (TV item in collection)
                {
                    innerDictionary.Add(item.GetKey(), item);
                    base.Add(item);
                }
            }
        }

        public override void Clear()
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                innerDictionary.Clear();
                base.Clear();
            }
        }

        public override bool ContainsKey(TK key)
        {
            return innerDictionary.ContainsKey(key);
        }

        public override TV GetValue(TK key)
        {
            TV value;

            if (innerDictionary.TryGetValue(key, out value) == false)
                return default(TV);

            return value;
        }

        public override void Set(TV item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                innerDictionary[item.GetKey()] = item;
                base.Set(item);
            }
        }

        public override void Insert(int index, TV item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                innerDictionary.Add(item.GetKey(), item);
                try
                {
                    base.Insert(index, item);
                }
                catch
                {
                    innerDictionary.Remove(item.GetKey());
                    throw;
                }
            }
        }

        public override bool Remove(TV item)
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                TV value;
                if (innerDictionary.TryGetValue(item.GetKey(), out value))
                {
                    if (value.Equals(item))
                    {
                        bool result = innerDictionary.Remove(item.GetKey());

                        if (result)
                            base.Remove(item);

                        return result;
                    }
                }

                return false;
            }
        }

        public override void RemoveAt(int index)
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                innerDictionary.Remove(base[index].GetKey());
                base.RemoveAt(index);
            }
        }

        public override bool RemoveByKey(TK key)
        {
            if (IsReadOnly)
                throw new NotSupportedException("Collection is readonly.");

            lock (locker)
            {
                bool result = innerDictionary.Remove(key);
                if (result)
                    base.RemoveByKey(key);

                return result;
            }
        }

        public override bool TryGetValue(TK key, out TV value)
        {
            return innerDictionary.TryGetValue(key, out value);
        }

        public override TV this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                if (IsReadOnly)
                    throw new NotSupportedException("Collection is readonly.");

                lock (locker)
                {
                    innerDictionary[base[index].GetKey()] = value;
                    base[index] = value;
                }
            }
        }

	}
}