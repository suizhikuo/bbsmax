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
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Generic;

using MaxLabs.bbsMax.Settings;



namespace MaxLabs.bbsMax.DataAccess.SqlServer
{
    class SettingDao : Settings.SettingDao
    {
        public override void SaveSettings(SettingBase settings)
        {
            if (settings.Serializable)
                SaveSerializableSettings(settings);
            else
                SaveUnSerializableSettings(settings);
        }

        private void SaveSerializableSettings(SettingBase settings)
        {
            using (SqlQuery query = new SqlQuery())
            {
                query.CommandText = @"
IF EXISTS(SELECT * FROM [bx_Settings] WHERE [TypeName] = @TypeName)
	UPDATE [bx_Settings] SET [Value] = @Value WHERE [TypeName] = @TypeName;
ELSE
	INSERT [bx_Settings] ([TypeName], [Value]) VALUES (@TypeName, @Value);";

                query.CreateParameter<string>("@Value", settings.ToString(), SqlDbType.NText);
                query.CreateParameter<string>("@TypeName", settings.GetType().FullName, SqlDbType.NVarChar, 200);

                query.ExecuteNonQuery();
            }
        }

        private void SaveUnSerializableSettings(SettingBase settings)
        {
			using(SqlQuery query = new SqlQuery(QueryMode.Prepare))
			{
				query.CommandText = @"

				IF EXISTS (SELECT * FROM [bx_Settings] WHERE [TypeName] = @TypeName AND [Key] = @Key)
					UPDATE [bx_Settings] SET [Value] = @Value WHERE [TypeName] = @TypeName AND [Key] = @Key;
				ELSE
					INSERT [bx_Settings] ([TypeName], [Key], [Value]) VALUES (@TypeName, @Key, @Value);";

                foreach (PropertyInfo property in settings.GetType().GetProperties())
                {
                    if (property.IsDefined(typeof(SettingItemAttribute), true))
                    {
                        query.CreateParameter<string>("@TypeName", settings.GetType().FullName, SqlDbType.NVarChar, 200);
                        query.CreateParameter<string>("@Key", property.Name, SqlDbType.NVarChar, 100);
                        query.CreateParameter<string>("@Value", settings.GetPropertyValue(property), SqlDbType.NText);

                        query.ExecuteNonQuery();
                    }
                }
			}
        }

        [StoredProcedure(Name = "bx_LoadAllSettings", Script = @"
CREATE PROCEDURE {name}
AS
BEGIN

    SET NOCOUNT ON;

    SELECT * FROM [bx_Settings] ORDER BY [TypeName];

END
")]
        public override AllSettings LoadAllSettings()
        {
            AllSettings result = new AllSettings();

            using (SqlQuery db = new SqlQuery())
            {
                db.CommandType = CommandType.StoredProcedure;
                db.CommandText = "bx_LoadAllSettings";
                db.CommandTimeout = 180;

                using (XSqlDataReader reader = db.ExecuteReader())
                {

                    string typeName = null;

                    //while (dataReader != null && true)
                    while (true)
                    {
                        #region 获取类型名
                        if (typeName == null)
                        {
                            if (reader.Read())
                                typeName = reader.Get<string>("TypeName");
                            else
                                break;
                        }
                        #endregion

                        #region 获取对应AllSettings类的属性名
                        string propertyName = null;

                        if (typeName.StartsWith("MaxLabs.bbsMax.Settings."))
                            propertyName = typeName.Substring(24);
                        else
                        {
                            if (reader.Read())
                            {
                                typeName = reader.Get<string>("TypeName");
                                continue;
                            }
                            else
                                break;
                        }
                        #endregion

                        #region 获取对应属性名的属性
                        FieldInfo settingField = result.GetType().GetField(propertyName);

                        if (settingField == null)
                        {
                            if (reader.Read())
                            {
                                typeName = reader.Get<string>("TypeName");
                                continue;
                            }
                            else
                                break;
                        }
                        #endregion

                        SettingBase propertyValue = result.GetSettingFieldValue(settingField);

                        if (propertyValue.Serializable)
                        {
                            propertyValue.Parse(reader.Get<string>("Value"));

                            #region 读下一条记录
                            if (reader.Read())
                                typeName = reader.Get<string>("TypeName");
                            else
                                break;
                            #endregion
                        }
                        else
                        {
                            bool end = false;

                            while (true)
                            {
                                #region 通过Key获取对应属性并赋值
                                string key = reader.Get<string>("Key");

                                PropertyInfo settingItemProperty = propertyValue.GetType().GetProperty(key);

                                if (settingItemProperty != null)
                                {
                                    propertyValue.SetPropertyValue(settingItemProperty, reader.Get<string>("Value"), true);
                                }
                                #endregion

                                #region 读下一条记录
                                if (reader.Read())
                                {
                                    string nextTypeName = reader.Get<string>("TypeName");

                                    if (typeName != nextTypeName)
                                    {
                                        typeName = nextTypeName;
                                        break;
                                    }
                                }
                                else
                                {
                                    end = true;
                                    break;
                                }
                                #endregion
                            }

                            if (end)
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}