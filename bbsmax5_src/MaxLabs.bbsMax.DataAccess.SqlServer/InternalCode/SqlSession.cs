//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
	public class SqlSession : IDisposable
	{
		public SqlSession()
		{
			if (BbsContext.Current.Transaction != null)
			{
				m_TransactionFromBbsContext = true;
				m_Transaction = (SqlTransaction)BbsContext.Current.Transaction;
				m_Connection = m_Transaction.Connection;
			}
			else
			{
				m_Connection = new SqlConnection();
				m_Connection.ConnectionString = Globals.ConnectionString;
			}
		}

		private bool m_TransactionFromBbsContext = false;

		private SqlConnection m_Connection;

		public SqlConnection Connection
		{
			get { return m_Connection; }
		}

		public SqlQuery CreateQuery()
		{
			SqlQuery q = new SqlQuery();

			q.Session = this;

			return q;
		}

		public SqlQuery CreateQuery(QueryMode queryMode)
		{
			SqlQuery q = new SqlQuery(queryMode);

			q.Session = this;

			return q;
		}

		public SqlQuery CreateQuery(int batchSumitSize)
		{
			SqlQuery q = new SqlQuery(QueryMode.Batch, batchSumitSize);

			q.Session = this;

			return q;
		}

        private SqlTransaction m_Transaction;

		/// <summary>
		/// 获取当前的数据库事务
		/// </summary>
		public SqlTransaction Transaction
		{
			get { return m_Transaction; }
		}

        public SqlTransaction BeginTransaction()
        {
            if (m_Transaction != null)
                return m_Transaction;

            if (m_Connection.State == System.Data.ConnectionState.Closed)
                m_Connection.Open();

            m_Transaction = m_Connection.BeginTransaction();

            return m_Transaction;

        }

        /// <summary>
        /// 开始一个新的事务。如果原来的事务没有提交，将自动提交原来的事务
        /// </summary>
        public SqlTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
			if (m_Transaction != null)
				return m_Transaction;

            if (m_Connection.State == System.Data.ConnectionState.Closed)
                m_Connection.Open();

            m_Transaction = m_Connection.BeginTransaction(isolationLevel);

            return m_Transaction;

        }

        /// <summary>
        /// 提交事务
        /// </summary>
		public void CommitTransaction()
		{
			if (m_Transaction == null || m_TransactionFromBbsContext)
				return;

			m_Transaction.Commit();
			m_Transaction.Dispose();
			m_Transaction = null;
		}

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            if (m_Transaction == null || m_TransactionFromBbsContext)
                return;

            m_Transaction.Rollback();
            m_Transaction.Dispose();
            m_Transaction = null;
        }

		public void Dispose()
		{
			if (m_Connection != null && m_TransactionFromBbsContext == false)
			{
				if (m_Transaction != null)
				{
                    m_Transaction.Rollback();
					m_Transaction.Dispose();
				}

				if (m_Connection.State != System.Data.ConnectionState.Closed)
				{
					m_Connection.Close();
					m_Connection.Dispose();
				}
			}
		}
	}
}