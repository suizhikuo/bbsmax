//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace MaxLabs.WebEngine
{
	/// <summary>
	/// 错误区段，用于捕获一个范围内的代码中通过Context.ThrowError抛出的错误。
	/// </summary>
	public class ErrorScope : IDisposable
	{
		[ThreadStatic]
		private static ErrorScope s_Current;

		public static ErrorScope Current
		{
			get
			{
				return s_Current;
			}
		}

		private ErrorScope m_Previous;

		public ErrorScope()
		{
			if (Context.Current != null)
				m_ErrorStartIndex = Context.Current.Errors.Count;

			Thread.BeginThreadAffinity();

			m_Previous = s_Current;
			s_Current = this;
		}

		private int m_ErrorStartIndex;

		/// <summary>
		/// 获取当前错误区段是否发生过错误
		/// </summary>
		public bool HasError
		{
			get
            {
                if (Context.Current == null)
                    return false;

				return m_ErrorStartIndex < Context.Current.Errors.Count;
			}
		}

        public bool HasUnCatchedError
        {
            get
            {
                if (Context.Current == null)
                    return false;

                if (HasError)
                {
                    for (int i = m_ErrorStartIndex; i < Context.Current.Errors.Count; i++)
                    {
                        if (Context.Current.Errors[i].Catched == false)
                            return true;
                    }
                }

                return false;
            }
        }

		/// <summary>
		/// 忽略调用此方法之前在当前错误区段发生的指定类型的错误。
		/// 这些错误信息不会真正从上下文的Errors集合移除，只是被标示成已捕获。
		/// 和CatchError一样，次方法支持错误信息的继承关系，即清除类型A的错误信息时，属于A的子类型错误信息也将被清除。
		/// </summary>
		/// <typeparam name="TError"></typeparam>
		public void IgnoreError<TError>() where TError : ErrorInfo
        {
            if (Context.Current == null)
                return;

			for (int i = m_ErrorStartIndex; i < Context.Current.Errors.Count; i++)
			{
				ErrorInfo error = Context.Current.Errors[i];

				if (error.Catched == false && error is TError)
				{
					error.Catched = true;
				}
			}
		}

		public delegate void CatchCallback<TError>(TError error);

		/// <summary>
		/// 捕获指定类型的错误信息，和C#的catch类似，所有属于指定类型的子类错误信息也将被捕获。
		/// 比如通过捕获ErrorInfo类型的错误可以捕获所有类型的错误信息。
		/// </summary>
		/// <typeparam name="TError">错误类型</typeparam>
		/// <param name="callback"></param>
		public void CatchError<TError>(CatchCallback<TError> onError) where TError : ErrorInfo
		{
            if (Context.Current == null)
                return;

			for (int i = m_ErrorStartIndex; i < Context.Current.Errors.Count; i++)
			{
				ErrorInfo error = Context.Current.Errors[i];

				if (error.Catched == false && error is TError)
				{
					onError((TError)error);

					error.Catched = true;
				}
			}
		}

		#region IDisposable 成员

		private bool m_Disposed = false;

		public void Dispose()
		{
			if (m_Disposed == false)
			{
				m_Disposed = true;

				s_Current = m_Previous;

				Thread.EndThreadAffinity();
			}
		}

		#endregion
	}
}