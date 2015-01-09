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
using System.Collections.Generic;

namespace MaxLabs.bbsMax.DataAccess
{
	/// <summary>
	/// 对DataReader进行包装，提供更简便的数据读取接口
	/// </summary>
	public abstract class DataReaderWrap
	{
		/// <summary>
		/// 构造DataReader包装
		/// </summary>
		/// <param name="reader">原DataReader</param>
		protected DataReaderWrap(IDataReader reader)
		{
			m_InnerReader = reader;
		}

		private IDataReader m_InnerReader;

		/// <summary>
		/// 原DataReader
		/// </summary>
		public IDataReader InnerReader
		{
			get
			{
				return m_InnerReader;
			}
		}

		/// <summary>
		/// 等效于DataReader.Read()方法
		/// </summary>
		public virtual bool Next
		{
			get
			{
				return m_InnerReader.Read();
			}
		}

        /// <summary>
        /// 等效于DataReader.NextResult()方法
        /// </summary>
        public virtual bool NextResult()
        {
            return m_InnerReader.NextResult();
        }

		public bool ContainsField(string name)
		{
			for (int i = 0; i < m_InnerReader.FieldCount; i++)
			{
				if (m_InnerReader.GetName(i) == name)
					return true;
			}

			return false;
		}

		/// <summary>
		/// 获取字段的索引位置
		/// </summary>
		/// <param name="name">字段名</param>
		/// <returns>字段的索引位置或者-1</returns>
		public int IndexOf(string name)
		{
            return m_InnerReader.GetOrdinal(name);
		}

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <returns>字段的值</returns>
        public object Get(string name)
        {
            return Get(name, null);
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <param name="index">字段的索引位置</param>
        /// <returns></returns>
        public object Get(int index)
        {
            return Get(index, null);
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <param name="name">字段名称</param>
        /// <param name="defaultValue">在找不到字段或者字段值为DBNull时的默认返回值</param>
        /// <returns>字段的值</returns>
        public object Get(string name, object defaultValue)
        {
            int index = IndexOf(name);

            return Get(index, defaultValue);
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <param name="index">字段的索引位置</param>
        /// <param name="defaultValue">在找不到字段或者字段值为DBNull时的默认返回值</param>
        /// <returns>字段的值</returns>
        public object Get(int index, object defaultValue)
        {

            if (m_InnerReader.IsDBNull(index) == false)
                return m_InnerReader[index];
            else
                return defaultValue;
        }

		public object Get(string name, Type resultType)
		{
			return Get(name, null, resultType);
		}

        public object Get(int index, Type resultType)
        {
            return Get(index, null, resultType);
        }

		public virtual object Get(string name, object defaultValue, Type resultType)
		{
			int index = IndexOf(name);

            return Get(index, defaultValue, resultType);
		}

        public virtual object Get(int index, object defaultValue, Type resultType)
        {

            if (m_InnerReader.IsDBNull(index))
                return defaultValue;

            TypeCode typeCode = Type.GetTypeCode(resultType);

            object result = null;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    result = m_InnerReader.GetBoolean(index);
                    break;
                case TypeCode.Byte:
                    result = m_InnerReader.GetByte(index);
                    break;
                case TypeCode.Char:
                    result = m_InnerReader.GetChar(index);
                    break;
                case TypeCode.DateTime:
                    result = m_InnerReader.GetDateTime(index);
                    break;
                case TypeCode.Decimal:
                    result = m_InnerReader.GetDecimal(index);
                    break;
                case TypeCode.Double:
                    result = m_InnerReader.GetDouble(index);
                    break;
                case TypeCode.Single:
                    result = m_InnerReader.GetFloat(index);
                    break;
                case TypeCode.Int16:
                    result = m_InnerReader.GetInt16(index);
                    break;
                case TypeCode.Int32:
                    result = m_InnerReader.GetInt32(index);
                    break;
                case TypeCode.Int64:
                    result = m_InnerReader.GetInt64(index);
                    break;
                case TypeCode.String:
                    result = m_InnerReader.GetString(index);
                    break;
                case TypeCode.Object:
                    result = m_InnerReader.GetValue(index);
                    break;
            }

            return result;
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">返回类型，无法进行类型转换时将产生异常</typeparam>
        /// <param name="name">字段名称</param>
        /// <returns>字段的值</returns>
        public T Get<T>(string name)
        {
            return Get<T>(name, default(T));
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">返回类型，无法进行类型转换时将产生异常</typeparam>
        /// <param name="index">字段的索引位置</param>
        /// <returns>字段的值</returns>
        public T Get<T>(int index)
        {
            return Get<T>(index, default(T));
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">返回类型，无法进行类型转换时将产生异常</typeparam>
        /// <param name="name">字段名称</param>
        /// <param name="defaultValue">在找不到字段或者字段值为DBNull时的默认返回值</param>
        /// <returns>字段的值</returns>
        public T Get<T>(string name, T defaultValue)
        {
            return (T)Get(name, defaultValue, typeof(T));
        }

        /// <summary>
        /// 获取字段的值
        /// </summary>
        /// <typeparam name="T">返回类型，无法进行类型转换时将产生异常</typeparam>
        /// <param name="index">字段的索引位置</param>
        /// <param name="defaultValue">在找不到字段或者字段值为DBNull时的默认返回值</param>
        /// <returns>字段的值</returns>
        public T Get<T>(int index, T defaultValue)
        {
			return (T)Get(index, defaultValue, typeof(T));
        }

        /// <summary>
        /// 以Nullable形式获取字段的值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="name">字段名称</param>
        /// <returns>字段的值或者T类型的默认值</returns>
        public Nullable<T> GetNullable<T>(string name) where T : struct
        {
            return Get<Nullable<T>>(name);
        }

        /// <summary>
        /// 以Nullable形式获取字段的值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="index">字段的索引位置</param>
        /// <returns>字段的值或者T类型的默认值</returns>
        public Nullable<T> GetNullable<T>(int index) where T : struct
        {
            return Get<Nullable<T>>(index);
        }
	}
}