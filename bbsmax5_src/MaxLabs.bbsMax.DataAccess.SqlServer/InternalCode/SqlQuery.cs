//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Xml;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.XCmd;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
	public class SqlQuery : IDisposable
	{
        private const int COMMAND_TIMEOUT = 20;
        private List<DataTable> m_XCmdDataTables = null;

		public SqlQuery() : this(QueryMode.Single)
		{

		}

		public SqlQuery(QueryMode queryMode) : this(queryMode, 10)
		{
		}

		public SqlQuery(QueryMode queryMode, int batchSubmitSize)
		{
			m_QueryMode = queryMode;

			m_BatchSubmitSize = batchSubmitSize;
		}

        static SqlQuery()
        {

            using (SqlSession session = new SqlSession())
            {
                try
                {
                    session.Connection.Open();
                    SqlVersion = int.Parse(session.Connection.ServerVersion.Substring(0, 2));
                }
                catch {
                    SqlVersion = 8;
                }
            }
        }

        internal void AddToXCmdDataTables(IDataReader reader)
        {
            if (reader.GetName(0) == "XCMD")
            {
                DataTable dataTable = null;

                while (reader.Read())
                {
                    if (dataTable == null)
                    {
                        dataTable = new DataTable();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dataTable.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }
                    }

                    DataRow dataRow = dataTable.NewRow();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dataRow[i] = reader[i];
                    }

                    dataTable.Rows.Add(dataRow);

                }

                if (dataTable != null)
                {
                    if (m_XCmdDataTables == null)
                        m_XCmdDataTables = new List<DataTable>();

                    m_XCmdDataTables.Add(dataTable);
                }
            }


        }

        public static int SqlVersion;

        public static int QueryTimes
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    object result = HttpContext.Current.Items["MaxLabs.bbsMax.DataAccess.QueryTimes"];

                    if (result == null)
                        result = 0;

                    return (int)result;
                }

                return 0;
            }

            set
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Items["MaxLabs.bbsMax.DataAccess.QueryTimes"] = value;
            }
        }

		private bool m_OwnSession;
		private SqlSession m_Session;

		public SqlSession Session
		{
			get 
			{
				if (m_Session == null)
				{
					m_Session = new SqlSession();
					m_OwnSession = true;
				}
				return m_Session; 
			}
			set 
			{
				if (m_Session != null)
					throw new Exception("不能重复设置SqlSession");

				m_Session = value;
			}
		}

		private QueryMode m_QueryMode;

		public QueryMode QueryMode
		{
			get { return m_QueryMode; }
			set { m_QueryMode = value; }
		}

		private int m_BatchSubmitSize;

		public int BatchSubmitSize
		{
			get { return m_BatchSubmitSize; }
			set { m_BatchSubmitSize = value; }
		}

        private SqlCommand m_Command;

        private SqlCommand Command
        {
            get
            {
                if (m_Command == null)
                    m_Command = new SqlCommand();

                return m_Command;
            }
        }

        public SqlParameterCollection Parameters
        {
            get { return Command.Parameters; }
        }

        private string m_CommandText;

		public string CommandText
		{
            get
            {
                if (m_CommandText == null)
                    return string.Empty;
                return m_CommandText;
            }
            set
            {
                m_Pager = null;
                m_CommandText = value;
            }
		}

        private int m_CommandTimeout = COMMAND_TIMEOUT;

		public int CommandTimeout
		{
            get { return m_CommandTimeout; }
            set { m_CommandTimeout = value; }
		}

        private CommandType m_CommandType = CommandType.Text;

		public CommandType CommandType
		{
            get { return m_CommandType; }
            set { m_CommandType = value; }
		}

        private MagicParameterCollection m_MagicParameters = null;

        public MagicParameterCollection MagicParameters
        {
            get
            {
                if (m_MagicParameters == null)
                    m_MagicParameters = new MagicParameterCollection();

                return m_MagicParameters;
            }
        }

		private PagerInfo m_Pager;

		public PagerInfo Pager
		{
			get 
			{
				if (m_Pager == null)
					m_Pager = new PagerInfo();

				return m_Pager;
			}

			set
			{
				m_Pager = value;
			}
		}

        public void Prepare()
        {
            Command.Prepare();
        }

		public SqlParameter CreateParameter<T>(string name, T value, SqlDbType type)
		{
			return CreateParameter<T>(name, value, type, null, null);
		}

		public SqlParameter CreateParameter<T>(string name, T value, SqlDbType type, int size)
		{
			return CreateParameter<T>(name, value, type, size, null);
		}

		public SqlParameter CreateParameter<T>(string name, SqlDbType type, ParameterDirection direction)
		{
            return CreateParameter<T>(name, default(T), type, null, direction);
		}

		public SqlParameter CreateParameter<T>(string name, SqlDbType type, int size, ParameterDirection direction)
		{
            return CreateParameter<T>(name, default(T), type, size, direction);
		}

		private SqlParameter CreateParameter<T>(string name, T value, SqlDbType? type, int? size, ParameterDirection? direction)
		{
            if (type == null)
                throw new ArgumentNullException("type", "必须指定SqlDbType");

            switch (type.Value)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarBinary:
                    if (size == null)
                        throw new ArgumentException("size", string.Format("{0}类型的参数必须指定size", type));
                    break;

                default:
                    if (size != null)
                        throw new ArgumentException("size", string.Format("没有必要为{0}类型的参数指定size", type));

                    switch (type.Value)
                    {
                        //设置几种固定数据类型的长度
                        case SqlDbType.Text:
                        case SqlDbType.Image:
                            size = 2147483647;
                            break;

                        case SqlDbType.NText:
                            size = 1073741823;
                            break;
                    }
                    break;
            }

			SqlParameter result;

            if (Command.Parameters.Contains(name))
            {
                result = Command.Parameters[name];
            }
            else
            {
                result = new SqlParameter();
                result.ParameterName = name;
                Command.Parameters.Add(result);
            }

			if (type != null)
				result.SqlDbType = type.Value;

            if (value == null)
            {
                result.Value = DBNull.Value;
            }
            else if (value is DateTime || value is Nullable<DateTime>)
            {
                if (Convert.ToDateTime(value) < InternalConsts.SqlMinDateTime)
                    result.Value = InternalConsts.SqlMinDateTime;

                else if (Convert.ToDateTime(value) > InternalConsts.SqlMaxDateTime)
                    result.Value = InternalConsts.SqlMaxDateTime;

                else
                    result.Value = value;
            }
            else
                result.Value = value;

            if (size != null)
                result.Size = size.Value;

			if (direction != null)
				result.Direction = direction.Value;

			return result;
		}

        /// <summary>
        /// 创建一个 WHERE ID IN (@param) 类型的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public MagicParameter CreateInParameter<T>(string name, IEnumerable<T> values, SqlDbType dbType, int? size)
        {
            switch (dbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarBinary:
                    if (size == null)
                        throw new ArgumentException("size", string.Format("{0}类型的参数必须指定size", dbType));
                    break;

                default:
                    if (size != null)
                        throw new ArgumentException("size", string.Format("没有必要为{0}类型的参数指定size", dbType));

                    switch (dbType)
                    {
                        //设置几种固定数据类型的长度
                        case SqlDbType.Text:
                        case SqlDbType.Image:
                            size = 2147483647;
                            break;

                        case SqlDbType.NText:
                            size = 1073741823;
                            break;
                    }
                    break;
            }

            MagicParameter result;

            if (values == null)
            {
                result = new MagicParameter(name, null, MagicParameter.MagicParameterType.SelectInParameter, 0);

                MagicParameters.Add(result);

                return result;
            }

            StringBuilder sb = new StringBuilder();
            int i = 0;

            foreach (T value in values)
            {
                string paramName = string.Concat(name, "_P", i.ToString());

                sb.Append(paramName).Append(",");

                SqlParameter param = new SqlParameter(paramName, dbType);
                
                if (size != null)
                    param.Size = size.Value;

                param.Value = value;
                this.Command.Parameters.Add(param);

                i++;
            }

            if (i > 0)
                sb.Remove(sb.Length - 1, 1);

            result = new MagicParameter(name, sb.ToString(), MagicParameter.MagicParameterType.SelectInParameter, i);

            MagicParameters.Add(result);

            return result;

        }

        /// <summary>
        /// 创建一个 WHERE ID IN (@param) 类型的参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
		public MagicParameter CreateInParameter<T>(string name, IEnumerable<T> values)
		{
            MagicParameter result;

            if (values == null)
            {
                result = new MagicParameter(name, null, MagicParameter.MagicParameterType.SelectInParameter);

                MagicParameters.Add(result);

                return result;
            }

			StringBuilder sb = new StringBuilder();
            int i = 0;

            TypeCode typeCode = Type.GetTypeCode(typeof(T));

			foreach (T value in values)
			{
				switch (typeCode)
				{
					case TypeCode.String:
						sb.Append("'");

						sb.Append(((string)(object)value).Replace("'", "''"));

						sb.Append("'");
						break;

					case TypeCode.Char:
						sb.Append("'");

						if (((char)(object)value) == '\'')
							sb.Append("''");
						else
							sb.Append(value);

						sb.Append("'");
						break;

					case TypeCode.Boolean:
						if ((bool)(object)value)
							sb.Append(1);
						else
							sb.Append(0);
						break;

                    case TypeCode.Int32:

                        string int32ParamName = string.Concat(name, "_P", i.ToString());
                        sb.Append(int32ParamName);

                        SqlParameter int32Param = new SqlParameter(int32ParamName, SqlDbType.Int);
                        int32Param.Value = value;
                        this.Command.Parameters.Add(int32Param);

                        break;

                    case TypeCode.Int64:

                        string int64ParamName = string.Concat(name, "_P", i.ToString());
                        sb.Append(int64ParamName);

                        SqlParameter int64Param = new SqlParameter(int64ParamName, SqlDbType.Int);
                        int64Param.Value = value;
                        this.Command.Parameters.Add(int64Param);

                        break;

					case TypeCode.Byte:
					case TypeCode.SByte:
					case TypeCode.Int16:
					//case TypeCode.Int32:
					//case TypeCode.Int64:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
					case TypeCode.UInt64:
					case TypeCode.Double:
					case TypeCode.Single:
					case TypeCode.Decimal:
						sb.Append(value);
						break;

					default:
						sb.Append("'");

						sb.Append(value.ToString().Replace("'", "''"));

						sb.Append("'");
						break;
				}

				sb.Append(",");

                i++;
			}

            if (i > 0)
    			sb.Remove(sb.Length - 1, 1);

			result = new MagicParameter(name, sb.ToString(), MagicParameter.MagicParameterType.SelectInParameter, i);

            MagicParameters.Add(result);

            return result;
		}

        /// <summary>
        /// 创建一个 SELECT TOP (@param) 类型的参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="top"></param>
        /// <returns></returns>
		public MagicParameter CreateTopParameter(string name, int top)
		{
			MagicParameter result = new MagicParameter(name, top, MagicParameter.MagicParameterType.SelectTopParameter);

            MagicParameters.Add(result);

			return result;
		}

        private bool prepared = false;

#if !Publish
        private System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        private StringBuilder runInfo = new StringBuilder();

        internal void RunInfoStart(SqlCommand command)
        {

            StringBuilder builder = runInfo;
            //builder.AppendLine();
            if (command.Parameters != null && command.Parameters.Count > 0)
            {
                //builder.AppendLine("参数：====>>>>");

                foreach (SqlParameter param in command.Parameters)
                {
                    builder.Append("DECLARE ");
                    builder.Append(param.ParameterName);
                    builder.Append(" ");
                    switch (param.SqlDbType)
                    {
                        case SqlDbType.Char:
                        case SqlDbType.NChar:
                        case SqlDbType.VarChar:
                        case SqlDbType.NVarChar:
                        case SqlDbType.VarBinary:
                            builder.Append(param.SqlDbType.ToString().ToLower());
                            builder.Append("(");
                            builder.Append(param.Size);
                            builder.AppendLine(");");
                            break;

                        default:
                            builder.Append(param.SqlDbType.ToString().ToLower());
                            builder.AppendLine(";");
                            break;
                    }

                    if (param.Value != null && param.Value != DBNull.Value)
                    {
                        builder.Append("SET ");
                        builder.Append(param.ParameterName);
                        builder.Append(" = ");
                        switch (param.DbType)
                        {
                            case DbType.String:
                            case DbType.AnsiString:
                            case DbType.Object:
                            case DbType.Xml:
                            case DbType.Guid:
                            case DbType.DateTime:
                            case DbType.DateTime2:
                            case DbType.Date:
                            case DbType.Time:
                                builder.Append("'");
                                builder.Append(param.Value.ToString());
                                builder.Append("'");
                                break;

                            case DbType.Boolean:
                                if (Convert.ToBoolean(param.Value))
                                    builder.Append("1");
                                else
                                    builder.Append("0");
                                break;

                            default:
                                builder.Append(param.Value.ToString());
                                break;
                        }
                        builder.AppendLine(";");
                    }
                }
            }

            builder.AppendLine("/* 开始执行 */");

            if (command.CommandType == CommandType.StoredProcedure)
            {
                builder.Append("exec ");
                builder.Append(command.CommandText);

                bool isfirst = true;
                foreach (SqlParameter param in command.Parameters)
                {
                    if (isfirst)
                    {
                        isfirst = false;
                        builder.Append(" ");
                    }
                    else
                        builder.Append(",");

                    builder.Append(param.ParameterName);
                    builder.Append("=");
                    builder.Append(param.ParameterName);
                }
                builder.AppendLine(";");
            }
            else
                builder.AppendLine(command.CommandText);

            builder.AppendLine("GO");
            builder.AppendLine("/* ======== */");

            ended = false;
            timer.Start();
        }

        internal string TempInfo = null;

        private bool ended = false;
        internal void RunInfoEnd()
        {
            if (ended)
                return;

            ended = true;
            timer.Stop();

            runInfo.AppendLine("/* timer:" + timer.Elapsed.TotalSeconds + " */\r\n");

            if (TempInfo != null)
            {
                runInfo.Append(TempInfo);
                runInfo.AppendLine();
            }

            if (HttpContext.Current != null)
            {
                runInfo.Append("url:");
                runInfo.AppendLine(HttpContext.Current.Request.RawUrl);

                string agent = HttpContext.Current.Request.UserAgent;
                if (agent != null)
                {
                    runInfo.Append("agent:");
                    runInfo.AppendLine(agent);
                }
                runInfo.AppendLine();
            }

            string info = runInfo.ToString().Replace(">", "＞").Replace("--", "——");

            timer.Reset();
            runInfo.Remove(0, runInfo.Length);

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items["max_runinfo"] == null)
                    HttpContext.Current.Items["max_runinfo"] = info;
                else
                    HttpContext.Current.Items["max_runinfo"] += info;
            }

#if DEBUG
            Globals.LogRunInfo(info);
#endif

            DateTime now = DateTime.Now;
            string path = Globals.GetPath(SystemDirecotry.Root, string.Concat("debug\\", now.Year, "_", now.Month, "_", now.Day, "\\", now.Hour));
            if (System.IO.Directory.Exists(path) == false)
                System.IO.Directory.CreateDirectory(path);

            path = IOUtil.JoinPath(path, now.Minute.ToString() + ".txt");
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true, Encoding.UTF8))
                {
                    writer.WriteLine(info);
                    writer.Close();
                }
            }
            catch { }
        }

#endif

        private SqlCommand CreateCommand()
        {

            SqlCommand command = this.Command;

            if (m_Pager != null)
            {
                CommandType = m_Pager.GetCommandType();
                CommandText = m_Pager.CreateCommandText(command.Parameters);
            }
            
            command.CommandText = CommandText;
            command.CommandType = CommandType;
            command.CommandTimeout = CommandTimeout;


            if (m_MagicParameters != null)
            {

                foreach (MagicParameter param in m_MagicParameters)
                {
                    // TOP (@Param) 类型的参数处理
                    if (param.Type == MagicParameter.MagicParameterType.SelectTopParameter)
                    {
                        if (SqlVersion > 8)
                        {
							SqlParameter topParam = new SqlParameter(param.ParameterName, SqlDbType.Int);
							topParam.Value = param.Value;

                            command.Parameters.Add(topParam);
                        }
                        else
                        {
                            Regex topParamFixRegex = new Regex(@"\s*top\s*\(\s*" + param.ParameterName + @"\s*\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

							command.CommandText = topParamFixRegex.Replace(command.CommandText, delegate(Match match) 
							{
								return " TOP " + param.Value;
							});
                        }
                    }
                    // IN (@Param) 类型的参数处理
                    else if (param.Type == MagicParameter.MagicParameterType.SelectInParameter)
                    {
                        Regex topParamFixRegex = new Regex(@"\s*(not)?\s*in\s*\(\s*" + param.ParameterName + @"\s*\)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                        command.CommandText = topParamFixRegex.Replace(command.CommandText, delegate(Match match)
                        {
                            if (string.IsNullOrEmpty(match.Groups[1].Value))
                            {
                                if (param.SelectInCount <= 0)
                                    return " IS NULL ";
                                else if (param.SelectInCount == 1)
                                {
                                    if (StringUtil.EqualsIgnoreCase(param.Value.ToString(), "NULL"))
                                        return " IS NULL ";
                                    else
                                        return " = " + param.Value;
                                }
                                else
                                    return string.Concat(" IN (", param.Value, ") ");
                            }
                            else
                            {
                                if (param.SelectInCount <= 0)
                                    return " IS NOT NULL ";
                                else if (param.SelectInCount == 1)
                                {
                                    if (StringUtil.EqualsIgnoreCase(param.Value.ToString(), "NULL"))
                                        return " IS NOT NULL ";
                                    else
                                        return " <> " + param.Value;
                                }
                                else
                                    return string.Concat(" NOT IN (", param.Value, ") ");
                            }
                        });
                    }
                    else
                    {
                        //                        string regex = string.Format(
                        //    @"'(?:''|[^'])*'
                        //|
                        //(?<isnull>{0}\s*is\s*null)
                        //|
                        //(?<isnotnull>{0}\s*is\s*not\s*null)
                        //|
                        //(?<param>{0})(?![_\w])", param.ParameterName);
                        string regex = string.Format(
@"'(?:''|[^'])*'  '('')0~+
|
(?<param>{0})(?![_\w])", param.ParameterName);

                        command.CommandText = Regex.Replace(command.CommandText, regex, delegate(Match match)
                        {

                            if (match.Groups["param"].Success)
                            {
                                if (param.Type == MagicParameter.MagicParameterType.SelectInParameter && param.Value == null)
                                    return "NULL";

                                return param.Value.ToString();
                            }

                            //if (match.Groups["isnull"].Success)
                            //    return param.Value == null ? "(1=1)" : "(1=2)";

                            //if (match.Groups["isnotnull"].Success)
                            //    return param.Value == null ? "(1=2)" : "(1=1)";

                            return match.Value;
                        }, RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                    }
                }
            }

#if !Publish
            RunInfoStart(command);
#endif

            command.Connection = Session.Connection;

            if (command.Connection.State == ConnectionState.Closed)
                command.Connection.Open();

            if (Session.Transaction != null)
                command.Transaction = Session.Transaction;

            if (prepared == false && this.QueryMode == QueryMode.Prepare)
            {
                prepared = true;
                command.Prepare();
            }

            return command;
        }

        public int ExecuteNonQuery()
        {
            QueryTimes++;

            int result = -1;

            using (SqlCommand command = CreateCommand())
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                    do
                    {
                        bool isXCommand = (reader.FieldCount > 0 && reader.GetName(0) == "XCMD");

                        if (isXCommand)
                            AddToXCmdDataTables(reader);

                        else if (result == -1)
                            result = reader.RecordsAffected;

                    }
                    while (reader.NextResult());
                }

#if !Publish
                RunInfoEnd();
#endif
            }

            return result;
        }

        public XSqlDataReader ExecuteReader()
		{
            QueryTimes++;

			using (SqlCommand command = CreateCommand())
			{
				return new XSqlDataReader(this, command.ExecuteReader());
			}
		}

        public XSqlDataReader ExecuteReader(CommandBehavior behavior)
		{
            QueryTimes++;

			using (SqlCommand command = CreateCommand())
			{
				return new XSqlDataReader(this, command.ExecuteReader(behavior));
			}
		}

		public object ExecuteScalar()
		{
            QueryTimes++;

            bool hasResult = false;
            object result = null;

			using (SqlCommand command = CreateCommand())
			{

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    do
                    {
                        bool isXCommand = (reader.FieldCount > 0 && reader.GetName(0) == "XCMD");

                        if (isXCommand)
                            AddToXCmdDataTables(reader);

                        else if (hasResult == false && reader.FieldCount > 0)
                        {
                            if (reader.Read())
                                result = reader[0];
                            else
                                result = null;

                            hasResult = true;
                        }

                    }
                    while (reader.NextResult());
                }

#if !Publish
                RunInfoEnd();
#endif
			}

            return result;
		}

		public T ExecuteScalar<T>()
		{
            QueryTimes++;

			object result = ExecuteScalar();

			if (result == null || result == DBNull.Value)
				return default(T);

			TypeCode type = Type.GetTypeCode(typeof(T));

			switch (type)
			{
				case TypeCode.Boolean:
					return (T)(object)Convert.ToBoolean(result);
				case TypeCode.Byte:
					return (T)(object)Convert.ToByte(result);
				case TypeCode.Char:
					return (T)(object)Convert.ToChar(result);
				case TypeCode.DateTime:
					return (T)(object)Convert.ToDateTime(result);
				case TypeCode.Decimal:
					return (T)(object)Convert.ToDecimal(result);
				case TypeCode.Double:
					return (T)(object)Convert.ToDouble(result);
				case TypeCode.Int16:
					return (T)(object)Convert.ToInt16(result);
				case TypeCode.Int32:
					return (T)(object)Convert.ToInt32(result);
				case TypeCode.Int64:
					return (T)(object)Convert.ToInt64(result);
				case TypeCode.SByte:
					return (T)(object)Convert.ToSByte(result);
				case TypeCode.Single:
					return (T)(object)Convert.ToSingle(result);
				case TypeCode.String:
					return (T)(object)Convert.ToString(result);
				case TypeCode.UInt16:
					return (T)(object)Convert.ToUInt16(result);
				case TypeCode.UInt32:
					return (T)(object)Convert.ToUInt32(result);
				case TypeCode.UInt64:
					return (T)(object)Convert.ToUInt64(result);
				default:
					return (T)result;
			}
		}

		public void Dispose()
		{

#if !Publish
            RunInfoEnd();
#endif

			if (m_OwnSession)
				m_Session.Dispose();

            if (m_XCmdDataTables != null)
            {
                foreach (DataTable dataTable in m_XCmdDataTables)
                {
                    XCmdManager.ProcessXCmd(dataTable);
                }
            }
		}

		public class MagicParameter
		{
			internal MagicParameter(string name, object value, MagicParameterType type)
			{
				m_ParameterName = name;
				m_Value = value;
				m_Type = type;
			}

            internal MagicParameter(string name, object value, MagicParameterType type, int selectInCount)
            {
                m_ParameterName = name;
                m_Value = value;
                m_Type = type;
                m_SelectInCount = selectInCount;
            }

			private string m_ParameterName;

			public string ParameterName
			{
				get { return m_ParameterName; }
			}

			private object m_Value;

			public object Value
			{
				get { return m_Value; }
			}

			private MagicParameterType m_Type;

			public MagicParameterType Type
			{
				get { return m_Type; }
			}

            private int m_SelectInCount;

            public int SelectInCount
            {
                get { return m_SelectInCount; }
            }

			public enum MagicParameterType
			{
				ReplaceParameter,
				SelectInParameter,
				SelectTopParameter
			}
		}

        public class MagicParameterCollection : Collection<MagicParameter>
        {
            public new void Add(MagicParameter parameter)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].ParameterName == parameter.ParameterName)
                    {
                        this[i] = parameter;
                        return;
                    }
                }

                base.Add(parameter);
            }
        }

		public class PagerInfo
		{
			private int m_PageSize;

            /// <summary>
            /// 每页显示的记录数
            /// </summary>
			public int PageSize
			{
				get { return m_PageSize; }
				set { m_PageSize = value; }
			}

			private int m_PageNumber = 1;

            /// <summary>
            /// 页码
            /// </summary>
			public int PageNumber
			{
                get { return m_PageNumber <= 0 ? 1 : m_PageNumber; }
				set { m_PageNumber = value; }
			}

			private string m_TableName;

            /// <summary>
            /// 要查询的表名或视图名
            /// </summary>
			public string TableName
			{
				get { return m_TableName; }
				set { m_TableName = value; }
			}

            private bool m_WithNolock = true;

            /// <summary>
            /// 查询时是否加入 WITH (NOLOCK) 语句
            /// </summary>
            public bool WithNolock
            {
                get { return m_WithNolock; }
                set { m_WithNolock = value; }
            }

			private string m_ResultFields = "*";

            /// <summary>
            /// 获取或设置要查询的字段
            /// </summary>
			public string ResultFields
			{
				get { return m_ResultFields; }
				set { m_ResultFields = value; }
			}

            private string m_PrimaryKey;

            /// <summary>
            /// 获取或设置分页表的主键
            /// 请注意：仅当SortField属性指定的字段不是唯一的情况下，才一定需要设置本属性
            /// 设置了本属性后，将使用全TOP模式处理分页，可以处理SortField不是唯一字段的分页
            /// </summary>
            public string PrimaryKey
            {
                get { return m_PrimaryKey; }
                set { m_PrimaryKey = value; }
            }

			private string m_SortField;

            /// <summary>
            /// 获取或设置分页的排序依据。
            /// 请注意：如果此字段不是唯一的，必须同时指定PrimaryKey（主键）属性，否则将无法得到正确的分页数据
            /// </summary>
			public string SortField
			{
				get { return m_SortField; }
				set { m_SortField = value; }
			}

			private bool m_IsDesc = true;

            /// <summary>
            /// 获取或设置分页查询是否倒序。不指定本属性默认为倒序
            /// </summary>
			public bool IsDesc
			{
				get { return m_IsDesc; }
				set { m_IsDesc = value; }
			}

			private string m_Condition;

            /// <summary>
            /// 获取或设置分页查询的条件。不指定本属性默认为取所有数据
            /// </summary>
			public string Condition
			{
				get { return m_Condition; }
				set { m_Condition = value; }
			}

			private int? m_TotalRecords;

            /// <summary>
            /// 预先设置分页查询的结果的总记录数。此属性并不是必须的，因为大部分情况并不知道结果的总记录数。
            /// 提供此属性是为了以下情况提高性能：
            /// 查询某一页的数据的时候，得到了总记录数，此时把总记录数缓存起来，
            /// 下次再查询任意一页，因为缓存中已经存在，那只需要直接将缓存中的值赋值到这个属性即可，可以极大提高性能
            /// 注意：不能与TotalRecordsVariable一起使用
            /// </summary>
			public int? TotalRecords
			{
				private get { return m_TotalRecords; }
				set { m_TotalRecords = value;}
			}

            private string m_TotalRecordsVariable;
            /// <summary>
            /// 预先设置分页查询的结果的总记录数由此变量提供。此属性并不是必须的，因为大部分情况并不知道结果的总记录数。
            /// 提供此属性是为了以下情况提高性能：
            /// 已经有另外的表专门记录了记录条数，在BeforeExecute里面已经从这个表查询到了正确的总结果数并保存在一个临时变量中，
            /// 那只需要直接将这个临时变量名赋值到这个属性即可，可以极大提高性能
            /// 注意：不能与TotalRecords一起使用
            /// </summary>
            public string TotalRecordsVariable
            {
                private get { return m_TotalRecordsVariable; }
                set { m_TotalRecordsVariable = value; }
            }

            private DeclareVariableCollection m_BeforeExecuteDealcre;

            public DeclareVariableCollection BeforeExecuteDealcre
            {
                get
                {
                    DeclareVariableCollection declare = m_BeforeExecuteDealcre;
                    if (declare == null)
                    {
                        declare = new DeclareVariableCollection();
                        m_BeforeExecuteDealcre = declare;
                    }
                    return declare;
                }
                set { m_BeforeExecuteDealcre = value; }
            }

			private string m_BeforeExecute;

            /// <summary>
            /// 在分页查询执行之前需要执行的sql
            /// </summary>
			public string BeforeExecute
			{
				get { return m_BeforeExecute; }
				set
                {
                    if (StringUtil.ContainsIgnoreCase(value, "declare ") || StringUtil.ContainsIgnoreCase(value, "declare\t"))
                        throw new ArgumentException("请不要在BeforeExecute中用SQL代码声明变量，如需声明，请使用BeforeExecuteDeclare属性来控制");

                    m_BeforeExecute = value;
                }
			}

			private string m_AfterExecute;

            /// <summary>
            /// 在分页查询执行之后需要执行的sql
            /// </summary>
			public string AfterExecute
			{
				get { return m_AfterExecute; }
				set { m_AfterExecute = value; }
			}

			private bool m_SelectCount;

            /// <summary>
            /// 获取或设置是否在返回数据结果集之后，再返回一个单行、单列的结果集标识总记录数。如果已经指定了TotalRecords属性，那么返回的总记录数将直接使用TotalRecords，而不会重新到数据库查询
            /// </summary>
			public bool SelectCount
			{
				get { return m_SelectCount; }
				set { m_SelectCount = value; }
			}

            private int m_Offset = 0;

            /// <summary>
            /// 偏移量
            /// </summary>
            public int Offset
            {
                get { return m_Offset; }
                set { m_Offset = value; }
            }

			internal string CreateCommandText(SqlParameterCollection parameters)
			{
				StringBuilder sb = new StringBuilder();

                if (BeforeExecuteDealcre != null)
                {
                    BeforeExecuteDealcre.BuildDeclareVariableSql(sb);
                }

				sb.AppendLine(BeforeExecute);

                sb.AppendLine(CreatePagedSelect(ResultFields, TableName, WithNolock, PrimaryKey, Condition, parameters, BeforeExecuteDealcre, SortField, IsDesc, PageNumber, PageSize, Offset, SelectCount, TotalRecords));

                sb.AppendLine(AfterExecute);

				return sb.ToString();
			}

			internal CommandType GetCommandType()
			{
				return CommandType.Text;
			}

            private string GetSafeDBObjectName(string dbObjectName)
            {
                if (string.IsNullOrEmpty(dbObjectName))
                    return string.Empty;

                bool b1 = StringUtil.StartsWith(dbObjectName, '[');
                bool b2 = StringUtil.EndsWith(dbObjectName, ']');

                if (b1 && b2)
                    return dbObjectName;

                else if (b1)
                    return dbObjectName + "]";

                else if (b2)
                    return "[" + dbObjectName;

                else
                    return string.Concat("[", dbObjectName, "]");
            }

            internal string CreatePagedSelect(string selectFields, string tableName, bool withNolock, string primaryKey, string condition, SqlParameterCollection parameters, DeclareVariableCollection beforeExecuteDeclare, string sortField, bool isDesc, int pageNumber, int pageSize, int offset, bool returnTotalRecords, int? totalRecords)
            {
                if (string.IsNullOrEmpty(selectFields))
                    throw new ArgumentNullException("selectFields");

                if (string.IsNullOrEmpty(tableName))
                    throw new ArgumentNullException("tableName");

                if (string.IsNullOrEmpty(sortField))
                    throw new ArgumentNullException("sortField");

                if (pageSize < 1)
                    throw new ArgumentException("每页显示的记录数不能为0", "pageSize");

                string _tableName = GetSafeDBObjectName(tableName);
                string _primaryKey = GetSafeDBObjectName(primaryKey);
                string _sortField = GetSafeDBObjectName(sortField);
                string _tableNameWithNolock;

                if (_primaryKey != string.Empty && string.Compare(_primaryKey, _sortField, true) == 0)
                    _primaryKey = string.Empty;

                if (withNolock)
                    _tableNameWithNolock = _tableName + " WITH (NOLOCK) ";
                else
                    _tableNameWithNolock = _tableName;

                string desc = isDesc ? " DESC" : " ASC";
                string primaryKeyDesc;

                if (string.IsNullOrEmpty(_primaryKey))
                    primaryKeyDesc = string.Empty;
                else
                    primaryKeyDesc = isDesc ? string.Concat(",", _primaryKey, " DESC") : string.Concat(",", _primaryKey, " ASC");

                if (pageNumber < 1)
                    pageNumber = 1;

                //是否知道总记录数
                bool knowTotalRecords = (totalRecords != null && totalRecords.Value > -1);

                bool hasWhere = string.IsNullOrEmpty(condition) == false;
                string whereString = hasWhere ? " WHERE " + condition : " ";

                //是否在sql中已经声明了TotalRecords变量
                bool declareTotalRecords = false;

                StringBuffer sqlBuilder = new StringBuffer();

                //在已知总记录数的情况下，以下情况直接返回0条数据  --  passed
                if (
                    (
                        knowTotalRecords
                        &&
                        (
                    //已知没有数据
                            totalRecords.Value == 0
                            ||
                    //页码已经超过最后一页
                            (
                            pageNumber > 1
                            &&
                            (pageSize * (pageNumber - 1)) >= (totalRecords.Value + offset)
                            )
                        )
                    )
                    ||
                    //Offset已经大到本页面已经不需要取任何数据
                    (
                        offset > 0
                        &&
                        (pageSize * pageNumber) <= offset
                    )
                   )
                {
                    sqlBuilder += string.Concat(@"SELECT TOP 0 ", selectFields, " FROM ", _tableName, " WITH (NOLOCK);");
                }

                //取第一页，使用最简单高效的方式：top查询  --  passed
                else if (offset / pageSize == pageNumber - 1)
                {
                    #region TOP方式取第一页数据

                    //sql server 2005 及更高版本，top参数化
                    if (SqlVersion > 8)
                    {
                        sqlBuilder += string.Concat("SELECT TOP (@_Top) ", selectFields, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, desc, primaryKeyDesc, ";");

                        SqlParameter topParam = new SqlParameter("@_Top", SqlDbType.Int);
                        topParam.Value = pageSize - offset % pageSize;
                        parameters.Add(topParam);

                    }
                    //sql server 2000 ，top没有优化
                    else
                    {
                        sqlBuilder += string.Concat("SELECT TOP ", (pageSize - offset % pageSize).ToString(), " ", selectFields, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, desc, primaryKeyDesc, ";");
                    }

                    #endregion
                }


                //最后一页，直接top查询，并颠倒结果  --  passed
                else if (
                    knowTotalRecords
                    &&
                    (pageSize * pageNumber) >= (totalRecords.Value + offset)
                    )
                {
                    #region TOP方式取最后一页数据

                    string tempDesc = isDesc ? " ASC" : " DESC";
                    string tempPrimaryKeyDesc;

                    if (string.IsNullOrEmpty(_primaryKey))
                        tempPrimaryKeyDesc = string.Empty;
                    else
                        tempPrimaryKeyDesc = isDesc ? string.Concat(",", _primaryKey, " ASC") : string.Concat(",", _primaryKey, " DESC");

                    int top = (totalRecords.Value + offset) % pageSize;
                    if (top == 0)
                        top = pageSize - offset % pageSize;

                    //sql server 2005 及更高版本，top参数化
                    if (SqlVersion > 8)
                    {

                        sqlBuilder += string.Concat(@"
SELECT * FROM (
    SELECT TOP (@_Top) ", selectFields, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc, tempPrimaryKeyDesc, @"
) AS t1 ORDER BY ", _sortField, desc, primaryKeyDesc, ";");

                        SqlParameter topParam = new SqlParameter("@_Top", SqlDbType.Int);
                        topParam.Value = top;
                        parameters.Add(topParam);

                    }
                    //sql server 2000 ，top没有优化
                    else
                    {
                        sqlBuilder += string.Concat(@"
SELECT * FROM (
    SELECT TOP ", top.ToString(), " ", selectFields, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc, tempPrimaryKeyDesc, @"
) AS t1 ORDER BY ", _sortField, desc, primaryKeyDesc, ";");

                    }

                    #endregion
                }

                //没有指定主键，使用MAX/MIN方式来分页，SortField必须是唯一字段  --  doing
                else if (string.IsNullOrEmpty(_primaryKey))
                {

                    #region MIN/MAX分页模式

                    //如果是sql server 2005及更高版本，可以使用 top @参数 来优化查询，提高查询计划的复用程度，改善性能和内存占用
                    if (SqlVersion > 8)
                    {

                        #region sql server 2005以上版本的处理

                        //如果已知总记录数，则可以马上推算出到底是否需要逆向查询以提高性能
                        //(逆向查询指：如果要查询的页数超过总页数的一般，则倒过来查询数据。例如：最后一页的数据应该使用 top 直接查询)
                        if (totalRecords != null && totalRecords.Value > -1)
                        {
                            #region 复杂逻辑...

                            string minOrMax, tempDesc, op;
                            int topMax = pageSize * (pageNumber - 1);

                            if (topMax - offset > totalRecords.Value / 2)
                            {
                                minOrMax = isDesc ? "MAX" : "MIN";
                                op = isDesc ? "<=" : ">=";
                                tempDesc = isDesc ? " ASC" : " DESC";
                                topMax = totalRecords.Value - topMax + offset;
                            }
                            else
                            {
                                minOrMax = isDesc ? "MIN" : "MAX";
                                op = isDesc ? "<" : ">";
                                tempDesc = isDesc ? " DESC" : " ASC";
                                topMax -= offset;
                            }

                            sqlBuilder += string.Concat(@"
SELECT TOP (@_Size) ", selectFields, @" FROM ", _tableNameWithNolock, whereString, (hasWhere ? " AND " : " WHERE "), _sortField, op, @"
    (SELECT ", minOrMax, "(", _sortField, @") FROM (SELECT TOP (@_TopMax) ", _sortField, @" FROM ", _tableNameWithNolock, whereString, @" ORDER BY ", _sortField, tempDesc, @") AS t1)
ORDER BY ", _sortField, desc, @"
");

                            SqlParameter pageSizeParam = new SqlParameter("@_Size", SqlDbType.Int);
                            pageSizeParam.Value = pageSize;
                            parameters.Add(pageSizeParam);

                            SqlParameter topMaxParam = new SqlParameter("@_TopMax", SqlDbType.Int);
                            topMaxParam.Value = topMax;
                            parameters.Add(topMaxParam);

                            #endregion
                        }

                        //不知道总记录数，则需要在sql内部计算到底是否需要逆向查询来提高性能
                        else
                        {
                            #region 更复杂逻辑...

                            declareTotalRecords = true;

                            string tempDesc = isDesc ? " ASC" : " DESC";

                            sqlBuilder += @"
DECLARE @_Total int;
DECLARE @_TopMax int;
";

                            if (string.IsNullOrEmpty(TotalRecordsVariable) == false)
                            {
                                sqlBuilder += "SELECT @_Total = ";
                                sqlBuilder += TotalRecordsVariable;
                                sqlBuilder += ";";
                            }
                            else
                            {
                                sqlBuilder += "SELECT @_Total = COUNT(*) FROM ";
                                sqlBuilder += _tableName;
                                sqlBuilder += " WITH (NOLOCK) ";
                                sqlBuilder += whereString;
                                sqlBuilder += ";";
                            }

                            sqlBuilder += string.Concat(@"
SET @_TopMax = @_Size * (@_Number - 1);

--out page
IF (
    (@_Total = 0)
    OR
    (@_Number > 1 AND @_TopMax >= (@_Total + @_Offset))
    )
    SELECT TOP 0 ", selectFields, " FROM ", _tableName, @" WITH (NOLOCK);

--last page
ELSE IF (@_Size * @_Number) >= (@_Total + @_Offset) BEGIN
    SET @_TopMax = (@_Total + @_Offset) % @_Size;
    IF @_TopMax = 0
        SET @_TopMax = @_Size - (@_Offset % @_Size);

    SELECT * FROM (
        SELECT TOP (@_TopMax) ", selectFields, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc, @"
    ) AS t1 ORDER BY ", _sortField, desc, @";
END
");

                            //查询页数超过10页的时候才自动优化，否则提高不了太多性能，却反而使sql语句变得更长
                            if (pageNumber - (offset / pageSize) > 10)
                            {
                                sqlBuilder += string.Concat(@"
ELSE IF (@_TopMax - @_Offset) > (@_Total / 2) BEGIN
    SET @_TopMax = @_Total - @_TopMax + @_Offset;

    SELECT TOP (@_Size) ", selectFields, " FROM ", _tableNameWithNolock, whereString, (hasWhere ? " AND " : " WHERE "), _sortField, (isDesc ? "<" : ">"), @"=
        (SELECT ", (isDesc ? "MAX" : "MIN"), "(", _sortField, ") FROM (SELECT TOP (@_TopMax) ", _sortField, @" FROM ", _tableNameWithNolock, whereString, @" ORDER BY ", _sortField, tempDesc, @") AS t1)
    ORDER BY ", _sortField, desc, @";
END
");
                            }

                            sqlBuilder += string.Concat(@"
ELSE BEGIN
    SET @_TopMax = @_TopMax - @_Offset;

    SELECT TOP (@_Size) ", selectFields, " FROM ", _tableNameWithNolock, whereString, (hasWhere ? " AND " : " WHERE "), _sortField, (isDesc ? "<" : ">"), @"
        (SELECT ", (isDesc ? "MIN" : "MAX"), "(", _sortField, ") FROM (SELECT TOP (@_TopMax) ", _sortField, @" FROM ", _tableNameWithNolock, whereString, @" ORDER BY ", _sortField, desc, @") AS t1)
    ORDER BY ", _sortField, desc, @";

END
");

                            SqlParameter pageSizeParam = new SqlParameter("@_Size", SqlDbType.Int);
                            pageSizeParam.Value = pageSize;
                            parameters.Add(pageSizeParam);

                            SqlParameter pageNumberParam = new SqlParameter("@_Number", SqlDbType.Int);
                            pageNumberParam.Value = pageNumber;
                            parameters.Add(pageNumberParam);

                            SqlParameter offsetParam = new SqlParameter("@_Offset", SqlDbType.Int);
                            offsetParam.Value = offset;
                            parameters.Add(offsetParam);

                            #endregion
                        }

                        #endregion

                    }
                    else
                    {
                        #region sql server 2000版本的处理

                        //如果已知总记录数，则可以马上推算出到底是否需要逆向查询以提高性能
                        //(逆向查询指：如果要查询的页数超过总页数的一般，则倒过来查询数据。例如：最后一页的数据应该使用 top 直接查询)
                        if (totalRecords != null && totalRecords.Value > -1)
                        {

                            #region 复杂逻辑...

                            string minOrMax, tempDesc, op;
                            int topMax = pageSize * (pageNumber - 1);

                            if (topMax - offset > totalRecords.Value / 2)
                            {
                                minOrMax = isDesc ? "MAX" : "MIN";
                                op = isDesc ? "<=" : ">=";
                                tempDesc = isDesc ? " ASC" : " DESC";
                                topMax = totalRecords.Value - topMax + offset;
                            }
                            else
                            {
                                minOrMax = isDesc ? "MIN" : "MAX";
                                op = isDesc ? "<" : ">";
                                tempDesc = isDesc ? " DESC" : " ASC";
                                topMax -= offset;
                            }

                            sqlBuilder += string.Concat(@"
SELECT TOP ", pageSize.ToString(), " ", selectFields, @" FROM ", _tableNameWithNolock, whereString, (hasWhere ? " AND " : " WHERE "), _sortField, op, @"
    (SELECT ", minOrMax, "(", _sortField, @") FROM (SELECT TOP ", topMax.ToString(), " ", _sortField, @" FROM ", _tableNameWithNolock, whereString, @" ORDER BY ", _sortField, tempDesc, @") AS t1)
ORDER BY ", _sortField, desc, @"
");

                            #endregion

                        }

                        //不知道总记录数，则需要在sql内部计算到底是否需要逆向查询来提高性能
                        else
                        {

                            #region 更复杂逻辑...

                            int topMax = pageSize * (pageNumber - 1);

                            declareTotalRecords = true;

                            string tempDesc = isDesc ? " ASC" : " DESC";

                            string paramString = buildParamStringForExecuteSql(beforeExecuteDeclare, condition, parameters);

                            sqlBuilder += @"
DECLARE @_Total int;
DECLARE @_TopMax int;
DECLARE @_Sql nvarchar(4000);
";

                            if (string.IsNullOrEmpty(TotalRecordsVariable) == false)
                            {
                                sqlBuilder += "SELECT @_Total = ";
                                sqlBuilder += TotalRecordsVariable;
                                sqlBuilder += ";";
                            }
                            else
                            {
                                sqlBuilder += "SELECT @_Total = COUNT(*) FROM ";
                                sqlBuilder += _tableName;
                                sqlBuilder += " WITH (NOLOCK) ";
                                sqlBuilder += whereString;
                                sqlBuilder += ";";
                            }

                            sqlBuilder += string.Concat(@"
SET @_TopMax = @_Size * (@_Number - 1);

--out page
IF (
    (@_Total = 0)
    OR
    (@_Number > 1 AND @_TopMax >= (@_Total + @_Offset))
    )
    SELECT TOP 0 ", selectFields, " FROM ", _tableName, @" WITH (NOLOCK);

--last page
ELSE IF (@_Size * @_Number) >= (@_Total + @_Offset) BEGIN
    SET @_TopMax = (@_Total + @_Offset) % @_Size;
    IF @_TopMax = 0
        SET @_TopMax = @_Size - (@_Offset % @_Size);

    SET @_Sql = N'SELECT * FROM (
        SELECT TOP ' + CAST(@_TopMax AS varchar(9)) + N' ", selectFields, " FROM ", _tableNameWithNolock, whereString.Replace("'", "''"), " ORDER BY ", _sortField, tempDesc, @"
    ) AS t1 ORDER BY ", _sortField, desc, @"';

    EXEC sp_executesql @_Sql", paramString, @";
END
");

                            //查询页数超过10页的时候才自动优化，否则提高不了太多性能，却反而使sql语句变得更长
                            if (pageNumber - (offset / pageSize) > 10)
                            {
                                sqlBuilder += string.Concat(@"
ELSE IF (@_TopMax - @_Offset) > (@_Total / 2) BEGIN
    SET @_TopMax = @_Total - @_TopMax + @_Offset;

    SET @_Sql = N'SELECT TOP ", pageSize.ToString(), " ", selectFields, " FROM ", _tableNameWithNolock, whereString.Replace("'", "''"), (hasWhere ? " AND " : " WHERE "), _sortField, (isDesc ? "<" : ">"), @"=
        (SELECT ", (isDesc ? "MAX" : "MIN"), "(", _sortField, ") FROM (SELECT TOP ' + CAST(@_TopMax AS varchar(9)) + N' ", _sortField, @" FROM ", _tableNameWithNolock, whereString.Replace("'", "''"), @" ORDER BY ", _sortField, tempDesc, @") AS t1)
    ORDER BY ", _sortField, desc, @"';

    EXEC sp_executesql @_Sql", paramString, @";
END
");
                            }

                            sqlBuilder += string.Concat(@"
ELSE BEGIN
    --SET @_TopMax = @_TopMax - @_Offset;

    SELECT TOP ", pageSize.ToString(), " ", selectFields, " FROM ", _tableNameWithNolock, whereString, (hasWhere ? " AND " : " WHERE "), _sortField, (isDesc ? "<" : ">"), @"
        (SELECT ", (isDesc ? "MIN" : "MAX"), "(", _sortField, ") FROM (SELECT TOP ", (topMax - offset).ToString(), " ", _sortField, @" FROM ", _tableNameWithNolock, whereString, @" ORDER BY ", _sortField, desc, @") AS t1)
    ORDER BY ", _sortField, desc, @";

END
");

                            SqlParameter pageSizeParam = new SqlParameter("@_Size", SqlDbType.Int);
                            pageSizeParam.Value = pageSize;
                            parameters.Add(pageSizeParam);

                            SqlParameter pageNumberParam = new SqlParameter("@_Number", SqlDbType.Int);
                            pageNumberParam.Value = pageNumber;
                            parameters.Add(pageNumberParam);

                            SqlParameter offsetParam = new SqlParameter("@_Offset", SqlDbType.Int);
                            offsetParam.Value = offset;
                            parameters.Add(offsetParam);

                            #endregion

                        }

                        #endregion
                    }

                    #endregion

                }
                //指定了主键，使用全TOP方式来分页，并且可以处理SortField并不是唯一字段的情况
                else
                {

                    //if (offset != 0)
                    //    throw new NotSupportedException("指定了主键的分页方式不支持偏移分页(Offset必需为0)");

                    #region TOP分页模式

                    //如果是sql server 2005及更高版本，可以使用 top @参数 来优化查询，提高查询计划的复用程度，改善性能和内存占用
                    if (SqlVersion > 8)
                    {
                        #region sql server 2005以上版本的处理


                        //如果已知总记录数，则可以马上推算出到底是否需要逆向查询以提高性能
                        //(逆向查询指：如果要查询的页数超过总页数的一般，则倒过来查询数据。例如：最后一页的数据应该使用 top 直接查询)
                        if (totalRecords != null && totalRecords.Value > -1)
                        {

                            #region 复杂逻辑...

                            string tempDesc1, tempDesc2;
                            int topMax = pageSize * pageNumber;

                            if (topMax - offset > totalRecords.Value / 2)
                            {
                                tempDesc1 = isDesc ? " ASC" : " DESC";
                                tempDesc2 = isDesc ? " DESC" : " ASC";
                                topMax = totalRecords.Value - topMax + pageSize + offset;
                            }
                            else
                            {
                                tempDesc1 = isDesc ? " DESC" : " ASC";
                                tempDesc2 = isDesc ? " ASC" : " DESC";
                                topMax -= offset;
                            }

                            sqlBuilder += string.Concat(@"

SELECT ", selectFields, " FROM ", _tableNameWithNolock, " WHERE ", _primaryKey, @" IN (
    SELECT TOP (@_Size) ", _primaryKey, @" FROM (
        SELECT TOP (@_TopMax) ", _primaryKey, ",", _sortField, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc1, ",", _primaryKey, tempDesc1, @"
    ) AS t1 ORDER BY ", _sortField, tempDesc2, ",", _primaryKey, tempDesc2, @"
) ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @";
");

                            SqlParameter pageSizeParam = new SqlParameter("@_Size", SqlDbType.Int);
                            pageSizeParam.Value = pageSize;
                            parameters.Add(pageSizeParam);

                            SqlParameter topMaxParam = new SqlParameter("@_TopMax", SqlDbType.Int);
                            topMaxParam.Value = topMax;
                            parameters.Add(topMaxParam);


                            #endregion

                        }

                        //不知道总记录数，则需要在sql内部计算到底是否需要逆向查询来提高性能
                        else
                        {

                            #region 更复杂逻辑...

                            declareTotalRecords = true;

                            string tempDesc = isDesc ? " ASC" : " DESC";

                            sqlBuilder += @"
DECLARE @_Total int;
DECLARE @_TopMax int;
";

                            if (string.IsNullOrEmpty(TotalRecordsVariable) == false)
                            {
                                sqlBuilder += "SELECT @_Total = ";
                                sqlBuilder += TotalRecordsVariable;
                                sqlBuilder += ";";
                            }
                            else
                            {
                                sqlBuilder += "SELECT @_Total = COUNT(*) FROM ";
                                sqlBuilder += _tableName;
                                sqlBuilder += " WITH (NOLOCK) ";
                                sqlBuilder += whereString;
                                sqlBuilder += ";";
                            }

                            sqlBuilder += string.Concat(@"
SET @_TopMax = @_Size * @_Number;

--out page
IF (
    (@_Total = 0)
    OR
    (@_Number > 1 AND (@_Size * (@_Number - 1)) >= (@_Total + @_Offset))
    )
    SELECT TOP 0 ", selectFields, " FROM ", _tableName, @" WITH (NOLOCK);

--last page
ELSE IF @_TopMax >= (@_Total + @_Offset) BEGIN
    SET @_TopMax = (@_Total + @_Offset) % @_Size;
    IF @_TopMax = 0
        SET @_TopMax = @_Size - (@_Offset % @_Size);

    SELECT * FROM (
        SELECT TOP (@_TopMax) ", selectFields, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc, ",", _primaryKey, tempDesc, @"
    ) AS t1 ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @";
END
");

                            //查询页数超过10页的时候才自动优化，否则提高不了太多性能，却反而使sql语句变得更长
                            if (pageNumber - (offset / pageSize) > 10)
                            {
                                sqlBuilder += string.Concat(@"
ELSE IF (@_TopMax - @_Offset) > (@_Total / 2) BEGIN
    SET @_TopMax = @_Total - @_TopMax + @_Size + @_Offset;

    SELECT ", selectFields, " FROM ", _tableNameWithNolock, " WHERE ", _primaryKey, @" IN (
	    SELECT TOP (@_Size) ", _primaryKey, @" FROM (
		    SELECT TOP (@_TopMax) ", _primaryKey, ",", _sortField, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc, ",", _primaryKey, tempDesc, @"
	    ) AS t1 ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @"
    ) ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @";
END
");
                            }

                            sqlBuilder += string.Concat(@"
ELSE BEGIN
    SET @_TopMax = @_TopMax - @_Offset;

    SELECT ", selectFields, " FROM ", _tableNameWithNolock, " WHERE ", _primaryKey, @" IN (
	    SELECT TOP (@_Size) ", _primaryKey, @" FROM (
		    SELECT TOP (@_TopMax) ", _primaryKey, ",", _sortField, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @"
	    ) AS t1 ORDER BY ", _sortField, tempDesc, ",", _primaryKey, tempDesc, @"
    ) ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @";
END
");

                            SqlParameter pageSizeParam = new SqlParameter("@_Size", SqlDbType.Int);
                            pageSizeParam.Value = pageSize;
                            parameters.Add(pageSizeParam);

                            SqlParameter pageNumberParam = new SqlParameter("@_Number", SqlDbType.Int);
                            pageNumberParam.Value = pageNumber;
                            parameters.Add(pageNumberParam);

                            SqlParameter offsetParam = new SqlParameter("@_Offset", SqlDbType.Int);
                            offsetParam.Value = offset;
                            parameters.Add(offsetParam);

                            #endregion

                        }

                        #endregion
                    }
                    else
                    {
                        #region sql server 2000的处理

                        //如果已知总记录数，则可以马上推算出到底是否需要逆向查询以提高性能
                        //(逆向查询指：如果要查询的页数超过总页数的一般，则倒过来查询数据。例如：最后一页的数据应该使用 top 直接查询)
                        if (totalRecords != null && totalRecords.Value > -1)
                        {
                            #region 复杂逻辑...

                            string tempDesc1, tempDesc2;
                            int topMax = pageSize * pageNumber;

                            if (topMax - offset > totalRecords.Value / 2)
                            {
                                tempDesc1 = isDesc ? " ASC" : " DESC";
                                tempDesc2 = isDesc ? " DESC" : " ASC";
                                topMax = totalRecords.Value - topMax + pageSize + offset;
                            }
                            else
                            {
                                tempDesc1 = isDesc ? " DESC" : " ASC";
                                tempDesc2 = isDesc ? " ASC" : " DESC";
                                topMax -= offset;
                            }

                            sqlBuilder += string.Concat(@"

SELECT ", selectFields, " FROM ", _tableNameWithNolock, " WHERE ", _primaryKey, @" IN (
    SELECT TOP ", pageSize.ToString(), " ", _primaryKey, @" FROM (
        SELECT TOP ", topMax.ToString(), " ", _primaryKey, ",", _sortField, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, tempDesc1, ",", _primaryKey, tempDesc1, @"
    ) AS t1 ORDER BY ", _sortField, tempDesc2, ",", _primaryKey, tempDesc2, @"
) ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @";
");

                            #endregion
                        }
                        //不知道总记录数，则需要在sql内部计算到底是否需要逆向查询来提高性能
                        else
                        {
                            #region 更复杂逻辑...

                            declareTotalRecords = true;

                            int topMax = pageSize * pageNumber;
                            string tempDesc = isDesc ? " ASC" : " DESC";

                            string paramString = buildParamStringForExecuteSql(beforeExecuteDeclare, condition, parameters);

                            sqlBuilder += @"
DECLARE @_Total int;
DECLARE @_TopMax int;
DECLARE @_Sql nvarchar(4000);
";

                            if (string.IsNullOrEmpty(TotalRecordsVariable) == false)
                            {
                                sqlBuilder += "SELECT @_Total = ";
                                sqlBuilder += TotalRecordsVariable;
                                sqlBuilder += ";";
                            }
                            else
                            {
                                sqlBuilder += "SELECT @_Total = COUNT(*) FROM ";
                                sqlBuilder += _tableName;
                                sqlBuilder += " WITH (NOLOCK) ";
                                sqlBuilder += whereString;
                                sqlBuilder += ";";
                            }

                            sqlBuilder += string.Concat(@"
SET @_TopMax = @_Size * @_Number;

--out page
IF (
    (@_Total = 0)
    OR
    (@_Number > 1 AND (@_Size * (@_Number - 1)) >= (@_Total + @_Offset))
    )
    SELECT TOP 0 ", selectFields, " FROM ", _tableName, @" WITH (NOLOCK);

--last page
ELSE IF @_TopMax >= (@_Total + @_Offset) BEGIN
    SET @_TopMax = (@_Total + @_Offset) % @_Size;
    IF @_TopMax = 0
        SET @_TopMax = @_Size - (@_Offset % @_Size);

    SET @_Sql = N'SELECT * FROM (
        SELECT TOP ' + CAST(@_TopMax AS varchar(9)) + N' ", selectFields, " FROM ", _tableNameWithNolock, whereString.Replace("'", "''"), " ORDER BY ", _sortField, tempDesc, ",", _primaryKey, tempDesc, @"
    ) AS t1 ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @"';

    EXEC sp_executesql @_Sql", paramString, @";
END
");
                            //查询页数超过10页的时候才自动优化，否则提高不了太多性能，却反而使sql语句变得更长
                            if (pageNumber - (offset / pageSize) > 10)
                            {
                                sqlBuilder += string.Concat(@"
ELSE IF (@_TopMax - @_Offset) > (@_Total / 2) BEGIN
    SET @_TopMax = @_Total - @_TopMax + @_Size + @_Offset;

    SET @_Sql = N'SELECT ", selectFields, " FROM ", _tableNameWithNolock, " WHERE ", _primaryKey, @" IN (
	    SELECT TOP ", pageSize.ToString(), " ", _primaryKey, @" FROM (
		    SELECT TOP ' + CAST(@_TopMax AS varchar(9)) + N' ", _primaryKey, ",", _sortField, " FROM ", _tableNameWithNolock, whereString.Replace("'", "''"), " ORDER BY ", _sortField, tempDesc, ",", _primaryKey, tempDesc, @"
	    ) AS t1 ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @"
    ) ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @"';

    EXEC sp_executesql @_Sql", paramString, @";
END
");
                            }

                            sqlBuilder += string.Concat(@"
ELSE BEGIN
    --SET @_TopMax = @_TopMax - @_Offset;

    SELECT ", selectFields, " FROM ", _tableNameWithNolock, " WHERE ", _primaryKey, @" IN (
	    SELECT TOP ", pageSize.ToString(), " ", _primaryKey, @" FROM (
		    SELECT TOP ", (topMax - offset).ToString(), " ", _primaryKey, ",", _sortField, " FROM ", _tableNameWithNolock, whereString, " ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @"
	    ) AS t1 ORDER BY ", _sortField, tempDesc, ",", _primaryKey, tempDesc, @"
    ) ORDER BY ", _sortField, desc, ",", _primaryKey, desc, @";
END
");

                            SqlParameter pageSizeParam = new SqlParameter("@_Size", SqlDbType.Int);
                            pageSizeParam.Value = pageSize;
                            parameters.Add(pageSizeParam);

                            SqlParameter pageNumberParam = new SqlParameter("@_Number", SqlDbType.Int);
                            pageNumberParam.Value = pageNumber;
                            parameters.Add(pageNumberParam);

                            SqlParameter offsetParam = new SqlParameter("@_Offset", SqlDbType.Int);
                            offsetParam.Value = offset;
                            parameters.Add(offsetParam);

                            #endregion
                        }

                        #endregion
                    }

                    #endregion

                }

                //如果需要返回总记录数，返回。如果已经知道总记录数，此处无需再取
                if (returnTotalRecords)
                {
                    if (string.IsNullOrEmpty(this.TotalRecordsVariable) == false)
                    {
                        sqlBuilder += "\r\nSELECT ";
                        sqlBuilder += this.TotalRecordsVariable;
                        sqlBuilder += ";";
                    }

                    else if (knowTotalRecords || declareTotalRecords)
                    {
                        sqlBuilder += "\r\nSELECT @_Total;";

                        if (knowTotalRecords)
                        {
                            SqlParameter totalRecordsParam = new SqlParameter("@_Total", SqlDbType.Int);
                            totalRecordsParam.Value = totalRecords.Value;
                            parameters.Add(totalRecordsParam);
                        }
                    }

                    else
                    {
                        sqlBuilder += "SELECT COUNT(*) FROM ";
                        sqlBuilder += _tableName;
                        sqlBuilder += " WITH (NOLOCK) ";
                        sqlBuilder += whereString;
                        sqlBuilder += ";";
                    }
                }


                return sqlBuilder.ToString();
            }

            /// <summary>
            /// 生成sp_executesql专用的参数字符串
            /// </summary>
            /// <param name="beforeExecuteDeclare"></param>
            /// <param name="condition"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            private string buildParamStringForExecuteSql(DeclareVariableCollection beforeExecuteDeclare, string condition, SqlParameterCollection parameters)
            {
                if (string.IsNullOrEmpty(condition))// || parameters == null || parameters.Count == 0)
                    return string.Empty;

                if (
                    (beforeExecuteDeclare == null || beforeExecuteDeclare.Count == 0)
                    &&
                    (parameters == null || parameters.Count == 0)
                    )
                {
                    return string.Empty;
                }

                StringBuilder paramDeclare = new StringBuilder();
                StringBuilder paramValues = new StringBuilder();

                if (parameters != null && parameters.Count > 0)
                {
                    #region 根据参数列表生成sp_executesql专用的参数字符串

                    foreach (SqlParameter param in parameters)
                    {
                        if (condition.IndexOf(param.ParameterName) == -1)
                            continue;

                        paramDeclare.Append(",");

                        DeclareVariable.BuildDeclareVariableSql(paramDeclare, param.ParameterName, param.SqlDbType, param.Size);

                        paramValues.Append(",");
                        paramValues.Append(param.ParameterName);
                        paramValues.Append("=");
                        paramValues.Append(param.ParameterName);
                    }

                    #endregion
                }

                if (beforeExecuteDeclare != null && beforeExecuteDeclare.Count > 0)
                {
                    #region 根据BeforeExecute的变量声明列表生成sp_executesql专用的参数字符串

                    foreach (DeclareVariable declare in beforeExecuteDeclare)
                    {
                        if (condition.IndexOf(declare.Name) == -1)
                            continue;

                        paramDeclare.Append(",");

                        declare.BuildDeclareVariableSql(paramDeclare);

                        paramValues.Append(",");
                        paramValues.Append(declare.Name);
                        paramValues.Append("=");
                        paramValues.Append(declare.Name);
                    }

                    #endregion
                }

                if (paramDeclare.Length > 0)
                {
                    paramDeclare.Insert(1, "N'");
                    paramDeclare.Append("'");
                    paramDeclare.Append(paramValues.ToString());
                }

                return paramDeclare.ToString();
            }
		}
	}

	public enum QueryMode
	{
		Batch,
		Single,
		Prepare
	}
}