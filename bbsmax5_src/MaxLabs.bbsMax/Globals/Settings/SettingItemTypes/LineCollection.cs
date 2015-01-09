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
using System.Collections.Specialized;

using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Settings
{
    /// <summary>
    /// 用于保存多行文本的容器，可以以列表形式访问、搜索，自动忽略重复项
    /// </summary>
    public class LineCollection : ISettingItem, IEnumerable
    {
        #region ISettingItem Members

        private List<string> m_LoweredCollection = null;
        private string m_Value = string.Empty;
        private bool m_IgnoreRepetition = true;
        private int m_MaxLineLength = 0;
        private object locker = new object();

        /// <summary>
        /// 用于保存多行文本的容器，可以以列表形式访问、搜索，自动忽略重复项
        /// </summary>
        public LineCollection()
        { }

        /// <summary>
        /// 用于保存多行文本的容器，可以以列表形式访问、搜索，并可以设置是否忽略重复的项
        /// </summary>
        /// <param name="ignoreRepetition"></param>
        /// /// <param name="ignoreMaxLength">每行允许的长度 0为不限</param>
        public LineCollection(bool ignoreRepetition, int maxLineLength)
        {
            m_IgnoreRepetition = ignoreRepetition;
            m_MaxLineLength = maxLineLength;
        }

        public string GetValue()
        {
            return m_Value;
        }

        protected List<string> InnerCollection
        {
            get
            {
                if (m_LoweredCollection == null)
                    m_LoweredCollection = new List<string>();

                return m_LoweredCollection;
            }
            private set
            {
                m_LoweredCollection = value;
            }
        }

        public virtual void SetValue(string value)
        {
            string[] lines = StringUtil.GetLines(value);

            List<string> innerCollection = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line) == false)
                {
                    string loweredLine = line.ToLower();

                    if (m_IgnoreRepetition && innerCollection.Contains(loweredLine))//忽略重复的值
                        continue;
                    else if (m_MaxLineLength != 0 && loweredLine.Length > m_MaxLineLength)
                    {
                        WebEngine.Context.ThrowError(new InvalidLineCollectionError(m_MaxLineLength));
                        return;
                    }
                    innerCollection.Add(loweredLine);
                }
            }


            lock (locker)
            {
                InnerCollection = innerCollection;
                m_Value = value;
            }
            return;
        }

        /// <summary>
        /// 检查项是否存在于列表中
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(string value)
        {
            return InnerCollection.Contains(value.ToLower());
        }

        /// <summary>
        /// 得到总项数
        /// </summary>
        public int Count
        {
            get
            {
                return InnerCollection.Count;
            }
        }

        /// <summary>
        /// 得到指定行数的项
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                return this.InnerCollection[index].ToString();
            }
            set
            {
                this.InnerCollection[index] = value;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerCollection.GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return m_Value;
        }
    }
}