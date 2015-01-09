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

namespace MaxLabs.WebEngine
{
	/// <summary>
	/// 错误信息的基类
	/// </summary>
	public abstract class ErrorInfo
	{
        private string m_Target;
        private int m_TargetLine = -1;

        public ErrorInfo()
        { }

        public ErrorInfo(string target)
        {
            m_Target = target;
        }

        public ErrorInfo(string target, int targetLine)
        {
            m_Target = target;
            m_TargetLine = targetLine;
        }

		/// <summary>
		/// 获取错误目标行号，如集合中的序号等
		/// </summary>
        public int TargetLine { get { return m_TargetLine; } }

		/// <summary>
		/// 获取错误目标名称，如参数名等
		/// </summary>
        public string TatgetName { get { return m_Target; } }

		/// <summary>
		/// 获取错误信息
		/// </summary>
		public abstract string Message { get; }

		/// <summary>
		/// 获取当前错误是否被Catch过（通过ErrorScope的Catch方法）
		/// </summary>
		public bool Catched { get; internal set; }

        /// <summary>
        /// 是否对Message进行HTML编码 默认为false，即不编码
        /// </summary>
        public virtual bool HtmlEncodeMessage { get { return false; } }
	}

	public class ErrorInfoCollection : Collection<ErrorInfo>
	{
		/// <summary>
		/// 错误信息集合只能添加不能移除！
		/// </summary>
		/// <param name="index"></param>
		protected override void RemoveItem(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 错误信息集合只能添加不能移除！
		/// </summary>
		protected override void ClearItems()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 错误信息集合只能添加不能插入！
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		protected override void InsertItem(int index, ErrorInfo item)
		{
			if (index != this.Count)
				throw new NotImplementedException();

			if (this.Contains(item))
				return;

			base.InsertItem(index, item);
		}

		/// <summary>
		/// 获取所有未被捕获过的错误
		/// </summary>
		/// <returns></returns>
		public ErrorInfo[] GetUnCatchedErrors()
		{
			List<ErrorInfo> result = new List<ErrorInfo>();

			for (int i = 0; i < Count; i++)
			{
				if (this[i].Catched == false)
					result.Add(this[i]);
			}

			return result.ToArray();
		}

		/// <summary>
		/// 获取是否存在未被捕获过的错误
		/// </summary>
		public bool HasUnCatchedError
		{
			get
			{
				for (int i = 0; i < Count; i++)
				{
					if (this[i].Catched == false)
						return true;
				}

				return false;
			}
		}
	}
}