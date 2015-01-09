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
using System.Data;
using System.Data.SqlClient;
using MaxLabs.bbsMax.XCmd;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class XSqlDataReader : DataReaderWrap, IDataReader
    {
        //private SqlDataReader InnerReader;

        private SqlQuery m_SqlQuery = null;

        public XSqlDataReader(SqlQuery query, SqlDataReader reader) : base(reader)
        {
            m_SqlQuery = query;
            //InnerReader = reader;
        }

        public SqlQuery SqlQuery
        {
            get
            {
                return m_SqlQuery;
            }
        }

        #region IDataReader 成员

        public void Close()
        {
            try
            {
                do
                {
                    if (InnerReader.FieldCount > 0 && InnerReader.GetName(0) == "XCMD")
                    {
                        m_SqlQuery.AddToXCmdDataTables(InnerReader);
                    }
                }
                while (InnerReader.NextResult());
            }
            finally
            {
#if !Publish
                m_SqlQuery.RunInfoEnd();
#endif
                InnerReader.Close();
            }
        }

        public int Depth
        {
            get { return InnerReader.Depth; }
        }

        public DataTable GetSchemaTable()
        {
            return InnerReader.GetSchemaTable();
        }

        public bool IsClosed
        {
            get { return InnerReader.IsClosed; }
        }

        public override bool NextResult()
        {

            if (InnerReader.NextResult())
            {

                if (InnerReader.FieldCount > 0 && InnerReader.GetName(0) == "XCMD")
                {

                    m_SqlQuery.AddToXCmdDataTables(InnerReader);

                    //bool result = InnerReader.NextResult();

                    //if (result == false)
                    //    return false;

                    return this.NextResult();
                }

                return true;
            }
            else

                return false;

        }

        public bool Read()
        {
            //如果可以Read，则判断当前结果集是否是一句XCMD命令
            if (InnerReader.FieldCount > 0 && InnerReader.GetName(0) == "XCMD")
            {
                m_SqlQuery.AddToXCmdDataTables(InnerReader);

                bool result = InnerReader.NextResult();

                if (result == false)
                    return false;

                return this.Read();
            }

            return InnerReader.Read();
        }

        public int RecordsAffected
        {
            get { return InnerReader.RecordsAffected; }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (IsClosed == false)
                Close();

            InnerReader.Dispose();
        }

        #endregion

        #region IDataRecord 成员

        public int FieldCount
        {
            get { return InnerReader.FieldCount; }
        }

        public bool GetBoolean(int i)
        {
            return InnerReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return InnerReader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return InnerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return InnerReader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return InnerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return InnerReader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return InnerReader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            DateTime value = InnerReader.GetDateTime(i);

            if (value.Year == InternalConsts.SqlMinDateTime.Year)
                value = DateTime.MinValue;

            else if (value.Year == InternalConsts.SqlMaxDateTime.Year)
                value = DateTime.MaxValue;

            return value;
        }

        public decimal GetDecimal(int i)
        {
            return InnerReader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return InnerReader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return InnerReader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return InnerReader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return InnerReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return InnerReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return InnerReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return InnerReader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return InnerReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return InnerReader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return InnerReader.GetString(i);
        }

        public object GetValue(int i)
        {
            return InnerReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return InnerReader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return InnerReader.IsDBNull(i);
        }

        public object this[string name]
        {
            get { return InnerReader[name]; }
        }

        public object this[int i]
        {
            get { return InnerReader[i]; }
        }

        #endregion


        //public override object Get(string name, object defaultValue, Type resultType)
        //{
        //    object value = base.Get(name, defaultValue, resultType);

        //    if (value != null)
        //    {
        //        TypeCode typeCode = Type.GetTypeCode(resultType);

        //        if (typeCode == TypeCode.DateTime)
        //        {
        //            if (((DateTime)value).Year == InternalConsts.SqlMinDateTime.Year)
        //                value = DateTime.MinValue;
        //            else if (((DateTime)value).Year == InternalConsts.SqlMaxDateTime.Year)
        //                value = DateTime.MaxValue;
        //        }
        //    }

        //    return value;
        //}

        public override bool Next
        {
            get
            {
                return this.Read();
            }
        }

    }
}