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
using System.Collections.Specialized;

namespace MaxLabs.bbsMax
{
    public class StringCollectionIgnoreCase : IEnumerable<string>
    {
        private List<string> m_InnerList;
        private object m_Locker = new object();

        public StringCollectionIgnoreCase()
        {
            m_InnerList = new List<string>();
        }

        public void Add(string item)
        {
            lock (m_Locker)
            {
                m_InnerList.Add(item);
            }
        }

        public void Insert(int index, string item)
        {
            lock (m_Locker)
            {
                m_InnerList.Insert(index, item);
            }
        }

        public void Remove(string item)
        {
            lock (m_Locker)
            {
                int index = IndexOf(item);
                m_InnerList.RemoveAt(index);
            }
        }

        public int IndexOf(string item)
        {
            lock (m_Locker)
            {
                int i = 0;
                foreach (string line in m_InnerList)
                {
                    if (string.Compare(item, line, true) == 0)
                        return i;

                    i++;
                }
            }
            return -1;
        }

        public bool Contains(string item)
        {
            return IndexOf(item) != -1;
        }

        #region IEnumerable<string> 成员

        public IEnumerator<string> GetEnumerator()
        {
            return m_InnerList.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_InnerList.GetEnumerator();
        }

        #endregion
    }
}