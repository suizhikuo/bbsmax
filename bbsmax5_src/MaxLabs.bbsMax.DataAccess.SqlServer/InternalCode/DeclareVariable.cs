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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    public class DeclareVariable
    {
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set
            {
                if (StringUtil.StartsWith(value, '@'))
                    m_Name = value;
                else
                    m_Name = "@" + value;
            }
        }

        public SqlDbType DbType { get; set; }

        public int Size { get; set; }

        public void BuildDeclareVariableSql(StringBuilder builder)
        {
            BuildDeclareVariableSql(builder, this.Name, this.DbType, this.Size);
        }

        /// <summary>
        /// 生成类似 "@ParamName varchar(50)" 这样的声明字符串
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        public static void BuildDeclareVariableSql(StringBuilder builder, string name, SqlDbType dbType, int size)
        {
            builder.Append(name);
            builder.Append(" ");
            builder.Append(dbType.ToString().ToLower());

            switch (dbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.VarChar:
                case SqlDbType.NVarChar:
                case SqlDbType.VarBinary:
                    builder.Append("(");
                    builder.Append(size);
                    builder.Append(")");
                    break;
            }
        }
    }

    public class DeclareVariableCollection : Collection<DeclareVariable>
    {
        private void TryAdd(DeclareVariable variable)
        {
            foreach (DeclareVariable item in this)
            {
                if (StringUtil.EqualsIgnoreCase(item.Name, variable.Name))
                    throw new ArgumentException("在BeforeExecuteDeclare中声明了重复的参数");
            }

            this.Add(variable);
        }

        public void Add(string name, SqlDbType dbType, int size)
        {
            DeclareVariable variable = new DeclareVariable();
            variable.Name = name;
            variable.DbType = dbType;
            variable.Size = size;
            this.TryAdd(variable);
        }

        public void Add(string name, SqlDbType dbType)
        {
            DeclareVariable variable = new DeclareVariable();
            variable.Name = name;
            variable.DbType = dbType;
            variable.Size = 0;
            this.TryAdd(variable);
        }

        public void AddRange(IEnumerable<DeclareVariable> list)
        {
            foreach (DeclareVariable item in list)
            {
                this.TryAdd(item);
            }
        }

        /// <summary>
        /// 生成类似
        /// DECLARE @ParamName varchar(50);
        /// DECLARE @ParamName2 int;
        /// 这样的声明字符串
        /// </summary>
        /// <returns></returns>
        public string GetDeclareVariableSql()
        {
            StringBuilder builder = new StringBuilder();
            BuildDeclareVariableSql(builder);
            return builder.ToString();
        }
        
        public void BuildDeclareVariableSql(StringBuilder builder)
        {
            if (this.Count > 0)
            {
                foreach (DeclareVariable variable in this)
                {
                    builder.Append("DECLARE ");
                    variable.BuildDeclareVariableSql(builder);
                    builder.AppendLine(";");
                }
            }
        }
    }
}