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

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public enum SqlConditionStart
    {
        None,

        Where,

        And,

        Or
    }

    public class SqlConditionBuilder
    {

        private StringBuilder m_InnerBuilder = new StringBuilder();
        private SqlConditionStart m_StartAs = SqlConditionStart.Where;

        
		public SqlConditionBuilder(SqlConditionStart startAs)
		{
            m_StartAs = startAs;
		}


        public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, bool value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
        public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, byte value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
        public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, char value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, char[] value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, decimal value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, double value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, float value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, int value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, long value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, object value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, sbyte value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, short value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, string value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, uint value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, ulong value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}
		public static SqlConditionBuilder operator +(SqlConditionBuilder buffer, ushort value)
		{
            buffer.m_InnerBuilder.Append(value);

			return buffer;
		}

        public void Append(string condition)
        {
            AppendAnd(condition);
        }

        public void AppendAnd(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return;

            condition = condition.Trim();

            if (condition == string.Empty)
                return;

            m_InnerBuilder.Append(" AND ");
            m_InnerBuilder.Append(condition);
        }

        public void AppendOr(string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return;

            condition = condition.Trim();

            if (condition == string.Empty)
                return;

            m_InnerBuilder.Append(" OR ");
            m_InnerBuilder.Append(condition);
        }

        private bool IsStartWithAnd(int offset)
        {
            if (
                (m_InnerBuilder[offset] == 'A' || m_InnerBuilder[offset] == 'a')
                &&
                (m_InnerBuilder[offset + 1] == 'N' || m_InnerBuilder[offset + 1] == 'n')
                &&
                (m_InnerBuilder[offset + 2] == 'D' || m_InnerBuilder[offset + 2] == 'd')
                &&
                (m_InnerBuilder[offset + 3] == ' ' || m_InnerBuilder[offset + 3] == '\t')
                )
                return true;

            return false;
        }

        private bool IsStartWithOr(int offset)
        {
            if (
                (m_InnerBuilder[offset] == 'O' || m_InnerBuilder[offset] == 'o')
                &&
                (m_InnerBuilder[offset + 1] == 'R' || m_InnerBuilder[offset + 1] == 'r')
                &&
                (m_InnerBuilder[offset + 2] == ' ' || m_InnerBuilder[offset + 2] == '\t')
                )
                return true;

            return false;
        }

        private void TrimLeftAnd(int offset)
        {
            m_InnerBuilder.Remove(offset, 4);
        }

        public void TrimLeftOr(int offset)
        {
            m_InnerBuilder.Remove(offset, 3);
        }

		public override string ToString()
		{
            int realCharOffset = 0;

            for(int i = 0; i < m_InnerBuilder.Length; i++)
            {
                if (m_InnerBuilder[i] == ' ')
                    realCharOffset = i + 1;

                else
                    break;
            }

            if (m_InnerBuilder.Length > realCharOffset)
            {
                switch (m_StartAs)
                {
                    case SqlConditionStart.Where:

                        if (IsStartWithAnd(realCharOffset))
                            TrimLeftAnd(realCharOffset);

                        else if (IsStartWithOr(realCharOffset))
                            TrimLeftOr(realCharOffset);

                        m_InnerBuilder.Insert(0, " WHERE ");

                        break;

                    case SqlConditionStart.And:

                        if (IsStartWithAnd(realCharOffset) == false)
                        {
                            if (IsStartWithOr(realCharOffset))
                                TrimLeftOr(realCharOffset);

                            m_InnerBuilder.Insert(0, " AND ");
                        }

                        break;

                    case SqlConditionStart.Or:

                        if (IsStartWithOr(realCharOffset) == false)
                        {
                            if (IsStartWithAnd(realCharOffset))
                                TrimLeftAnd(realCharOffset);

                            m_InnerBuilder.Insert(0, " OR ");
                        }

                        break;

                    default:

                        if (IsStartWithAnd(realCharOffset))
                            TrimLeftAnd(realCharOffset);

                        else if (IsStartWithOr(realCharOffset))
                            TrimLeftOr(realCharOffset);

                        break;
                }
            }

            return m_InnerBuilder.ToString();
		}
    }
}