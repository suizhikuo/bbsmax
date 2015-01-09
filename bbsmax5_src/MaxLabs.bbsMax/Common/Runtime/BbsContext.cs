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
using System.Threading;
using System.Collections.Generic;

using MaxLabs.bbsMax.Providers;

namespace MaxLabs.bbsMax
{
	public class BbsContext : IDisposable
	{
		[ThreadStatic]
		private static BbsContext s_Current;

		public static BbsContext Current
		{
			get
			{
				if (s_Current == null)
				{
					BbsContext result = new BbsContext();

					result.m_SafeMode = null;
					result.m_ExecutorID = null;

					return result;
				}

				return s_Current;
			}
		}

		private BbsContext m_Previous;

		public BbsContext()
        {
            CreateBbsContextInner(true, null);
		}

		public BbsContext(bool safeMode)
		{
            CreateBbsContextInner(safeMode, null);
		}

		public BbsContext(int executorID)
		{
            CreateBbsContextInner(null, executorID);
		}

		public BbsContext(bool safeMode, int executorID)
		{
            CreateBbsContextInner(safeMode, executorID);
		}

		private void CreateBbsContextInner(bool? safeMode, int? executorID)
		{
			m_SafeMode = safeMode;
			m_ExecutorID = executorID;

			Thread.BeginThreadAffinity();

			m_Previous = s_Current;
			s_Current = this;
		}

		private bool? m_SafeMode;

		/// <summary>
		/// 获取当前执行过程是否处于非安全模式。
		/// 非安全模式下，业务逻辑将忽略权限判断和设置判断，只维持最低的代码执行需求和最高的权限。
		/// </summary>
        [Obsolete]
		public bool SafeMode
		{
			get
			{
				if (m_SafeMode == null)
				{
					if (m_Previous != null)
						return m_Previous.SafeMode;

					return true;
				}

				return m_SafeMode.Value;
			}
		}

		private int? m_ExecutorID;

		/// <summary>
		/// 获取当前操作的执行者ID
		/// </summary>
        [Obsolete]
		public int ExecutorID
		{
			get
			{
				if (m_ExecutorID == null)
				{
					if (m_Previous != null)
						return m_Previous.ExecutorID;
					
					return UserBO.Instance.GetCurrentUserID();
				}

				return m_ExecutorID.Value;
			}
		}

		private IDbTransaction m_Transaction;

		public IDbTransaction Transaction
		{
			get
            {
                IDbTransaction transaction = m_Transaction;

                if (transaction == null)
                {
                    BbsContext current = this;

                    while (current.m_Previous != null)
                    {
                        if (current.m_Previous.m_Transaction != null)
                        {
                            transaction = current.m_Previous.m_Transaction;
                            break;
                        }
                        current = current.m_Previous;
                    }
                }

                return transaction;
            }
		}

        private bool m_TransactionOpened = false;

        public virtual IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
		{

            IDbTransaction transaction = Transaction;

            if (transaction == null)
            {
                transaction = ProviderManager.Get<IDataProvider>().BeginTransaction(isolationLevel);
                
                m_Transaction = transaction;
            }

            m_TransactionOpened = true;

            return transaction;

		}

		public virtual void CommitTransaction()
		{
            //IDbTransaction transaction = Transaction;

            m_TransactionOpened = false;

            if (m_Transaction == null)
				return;

            m_Transaction.Commit();
            //m_Transaction.Connection.Close();

            //m_Transaction.Connection.Dispose();
            m_Transaction.Dispose();

            m_Transaction = null;

		}

		public virtual void RollbackTransaction()
		{
            m_TransactionOpened = false;

            if (m_Transaction == null)
            {
                BbsContext current = this;

                while (current.m_Previous != null)
                {
                    if (current.m_Previous.m_Transaction != null)
                    {
                        if (current.m_Previous.m_Transaction == null)
                            return;

                        current.m_Previous.m_Transaction.Rollback();

                        current.m_Previous.m_Transaction.Dispose();

                        current.m_Previous.m_Transaction = null;

                        break;
                    }
                    current = current.m_Previous;
                }
            }
            else
            {
                m_Transaction.Rollback();

                m_Transaction.Dispose();

                m_Transaction = null;
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

                if (m_TransactionOpened)
    				RollbackTransaction();

				Thread.EndThreadAffinity();
			}
		}

		#endregion
	}
}