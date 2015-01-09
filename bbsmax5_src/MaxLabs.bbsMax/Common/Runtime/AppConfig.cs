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
using System.Xml;
using MaxLabs.bbsMax.Settings;
using System.IO;
using System.Reflection;

namespace MaxLabs.bbsMax
{
    public class AppConfig : ICloneable
    {
        #region 生成bbsmax.config的XML内容

        private const string bbsmaxConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <connectionStrings>
    <add name=""bbsmax"" connectionString=""{connectionstring}"" providerName=""{dataprovidername}"" />
  </connectionStrings>

  <appSettings>
    <add key=""bbsMax.Licence"" value=""{licence}""/>
    <add key=""bbsMax.InstallPath"" value=""~/""/>
    <add key=""bbsMax.DynamicCompress"" value=""{dynamiccompress}"" />
    <!-- 是否启用动态内容压缩，如果启用后出错请把value改为OFF -->
    <add key=""bbsMax.StaticCompress"" value=""{staticcompress}"" />
    <!-- 是否启用静态内容压缩，如果启用后出错请把value改为OFF -->
  </appSettings>
{owners}
{passport}
{maxPlugins}
</configuration>
";

        private const string bbsbaxConfig_Owners_Tag = @"
  <owners>
{lines}
  </owners>
";
        private const string bbsbaxConfig_Owners_Line = @"    <add username=""{username}"" />";

        private const string bbsbaxConfig_Passport_Tag = @"
  <passportConnectionSetting enable=""{enable}"" server=""{server}"" clientid=""{clientid}"" accesskey=""{accesskey}"" timeout=""{timeout}"" />
";

        #endregion

        private const string DefaultInstallDirectory = "~/";

        public string ConnectionString;

        public string DataProviderName;

        public bool DynamicCompress;

        public bool StaticCompress;

        public string Licence;

        public string[] LicencedServers;

        public DateTime LicenceExpireDate;

        public string SafeString;

        public string FileNamePart;

        public string InstallDirectory = DefaultInstallDirectory;

        public PassportClientConfig PassportClient = new PassportClientConfig();

        public AppMaxPlugins MaxPlugins =new AppMaxPlugins();

        public List<string> OwnerUsernames = new List<string>();

        public void Read(string configFilePath)
        {
            #region 读取配置

            if (File.Exists(configFilePath) == false)
                throw new FileNotFoundException("未在bbsmax所在的根目录找到bbsmax.config配置文件", "bbsmax.config");

            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(configFilePath);
            }
            catch (XmlException)
            {
                throw new Exception("配置文件 bbsmax.config 格式不正确。您是否在数据库名、数据库帐号或密码中使用了 & 或者其他特殊符号？如果是，您需要在配置文件中将其进行HTML编码。例如 & 应该写为 &amp;");
            }
            catch
            {
                throw;
            }

            foreach (XmlNode configNode in doc.DocumentElement.ChildNodes)
            {
                if (configNode.NodeType != XmlNodeType.Comment)
                {
                    switch (configNode.Name.ToLower())
                    {
                        case "connectionstrings":

                            #region 读取连接字符串

                            foreach (XmlNode connectionNode in configNode.ChildNodes)
                            {
                                if (connectionNode.NodeType != XmlNodeType.Comment)
                                {
                                    if (string.Compare(connectionNode.Name, "clear", true) == 0)
                                    {
                                        ConnectionString = null;
                                        DataProviderName = null;
                                    }

                                    else
                                    {
                                        bool isAdd = string.Compare(connectionNode.Name, "add", true) == 0;
                                        bool isRemove = string.Compare(connectionNode.Name, "remove", true) == 0;

                                        if ((isAdd || isRemove) && connectionNode.Attributes["name"] != null)
                                        {
                                            switch (connectionNode.Attributes["name"].Value.ToLower())
                                            {
                                                case "bbsmax":
                                                    if (isAdd)
                                                    {
                                                        ConnectionString = connectionNode.Attributes["connectionString"].Value.Trim();
                                                        DataProviderName = connectionNode.Attributes["providerName"].Value.Trim();
                                                    }
                                                    else if (isRemove)
                                                    {
                                                        ConnectionString = null;
                                                        DataProviderName = null;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                            #endregion

                        case "appsettings":

                            #region 读取基本设置

                            foreach (XmlNode appSettingNode in configNode.ChildNodes)
                            {
                                if (appSettingNode.NodeType != XmlNodeType.Comment)
                                {
                                    if (string.Compare(appSettingNode.Name, "clear", true) == 0)
                                    {
                                        InstallDirectory = DefaultInstallDirectory;
                                        DynamicCompress = false;
                                        StaticCompress = false;
                                    }

                                    else
                                    {
                                        bool isAdd = string.Compare(appSettingNode.Name, "add", true) == 0;
                                        bool isRemove = string.Compare(appSettingNode.Name, "remove", true) == 0;

                                        if ((isAdd || isRemove) && appSettingNode.Attributes["key"] != null)
                                        {
                                            switch (appSettingNode.Attributes["key"].Value.ToLower())
                                            {
                                                case "bbsmax.installpath":

                                                    if (isAdd)
                                                        InstallDirectory = appSettingNode.Attributes["value"].Value.Trim();
                                                    else if (isRemove)
                                                        InstallDirectory = null;

                                                    break;

                                                case "bbsmax.dynamiccompress":

                                                    if (isAdd)
                                                        DynamicCompress = string.Compare(appSettingNode.Attributes["value"].Value.Trim(), "ON", true) == 0;
                                                    else if (isRemove)
                                                        DynamicCompress = false;

                                                    break;

                                                case "bbsmax.staticcompress":

                                                    if (isAdd)
                                                        StaticCompress = string.Compare(appSettingNode.Attributes["value"].Value.Trim(), "ON", true) == 0;
                                                    else if (isRemove)
                                                        StaticCompress = false;

                                                    break;

                                                case "bbsmax.licence":

                                                    if (isAdd)
                                                        Licence = appSettingNode.Attributes["value"].Value.Trim();
                                                    else if (isRemove)
                                                        Licence = null;

                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;

                            #endregion

                        case "owners":

                            #region 读取创始人

                            foreach (XmlNode ownersNode in configNode.ChildNodes)
                            {
                                if (ownersNode.NodeType != XmlNodeType.Comment)
                                {
                                    if (string.Compare(ownersNode.Name, "clear", true) == 0)
                                    {
                                        OwnerUsernames.Clear();
                                    }

                                    else
                                    {
                                        bool isAdd = string.Compare(ownersNode.Name, "add", true) == 0;
                                        bool isRemove = string.Compare(ownersNode.Name, "remove", true) == 0;

                                        if ((isAdd || isRemove) && ownersNode.Attributes["username"] != null)
                                        {
                                            string username = ownersNode.Attributes["username"].Value.Trim().ToLower();

                                            if (isAdd)
                                            {
                                                OwnerUsernames.Add(username);
                                            }
                                            else if (isRemove)
                                            {
                                                OwnerUsernames.Remove(username);
                                            }
                                            break;

                                        }
                                    }
                                }
                            }
                            break;

                            #endregion

                        case "passportconnectionsetting":

                            #region 读取通行证客户端设置

                            bool enablePassport = false;

                            if (configNode.Attributes["enable"] != null)
                            {
                                enablePassport = StringUtil.EqualsIgnoreCase(configNode.Attributes["enable"].Value.Trim(), "true");
                                PassportClient.EnablePassport = enablePassport;
                            }

                            if (enablePassport)
                            {
                                bool serverRead = false;
                                bool clientidRead = false;
                                bool accesskeyRead = false;

                                if (configNode.Attributes["server"] != null)
                                {
                                    PassportClient.PassportRoot = configNode.Attributes["server"].Value.Trim();
                                 
                                    if(PassportClient.PassportRoot != string.Empty)
                                        serverRead = true;
                                }

                                if (serverRead == false)
                                    throw new ArgumentException("通行证配置中缺少server属性");

                                if (configNode.Attributes["clientid"] != null)
                                {
                                    int clientID;
                                    if (int.TryParse(configNode.Attributes["clientid"].Value.Trim(), out clientID))
                                    {
                                        PassportClient.ClientID = clientID;
                                        clientidRead = true;
                                    }
                                }

                                if (clientidRead == false)
                                    throw new ArgumentException("通行证配置中缺少clientid属性");

                                if (configNode.Attributes["accesskey"] != null)
                                {
                                    PassportClient.AccessKey = configNode.Attributes["accesskey"].Value.Trim();

                                    if (PassportClient.PassportRoot != string.Empty)
                                        accesskeyRead = true;
                                }

                                if (accesskeyRead == false)
                                    throw new ArgumentException("通行证配置中缺少accesskey属性");

                                if (configNode.Attributes["timeout"] != null)
                                {
                                    int timeout;
                                    if (int.TryParse(configNode.Attributes["timeout"].Value.Trim(), out timeout))
                                    {
                                        PassportClient.PassportTimeout = timeout;
                                        clientidRead = true;
                                    }
                                }
                            }

                            #endregion
                            break;
                        case "maxplugins":
                            #region 外部插件配置
                            XmlNodeList childNodes = configNode.ChildNodes;
                            int ln=1;
                            foreach (XmlNode n in childNodes)
                            {
                                bool readname = false;
                                bool readtype = false;
                                bool readassembly = false;
                                if (n.Name.Equals("add", StringComparison.OrdinalIgnoreCase))
                                {
                                    MaxPluginItem item = new MaxPluginItem();

                                    if (n.Attributes["name"] != null)
                                    {
                                        item.Name = n.Attributes["name"].Value.Trim();
                                        readname = true;
                                    }
                                    if (n.Attributes["type"] != null)
                                    {
                                        item.Type = n.Attributes["type"].Value.Trim();
                                        readtype = true;
                                    }

                                    if (n.Attributes["assembly"] != null)
                                    {
                                        item.Assembly = n.Attributes["assembly"].Value;
                                        readassembly = true;
                                    }

                                    if (readname == false)
                                    {
                                        throw new ArgumentException("外部插件配置项目第" + ln + "行缺少name属性");
                                    }
                                    if (readtype == false)
                                    {
                                        throw new ArgumentException("外部插件配置项目第" + ln + "行缺少type属性");
                                    }
                                    if (readassembly == false)
                                    {
                                        throw new ArgumentException("外部插件配置项目第" + ln + "行缺少readassembly属性");
                                    }

                                    this.MaxPlugins.Add(item.Name, item);
                                }
                                ln++;
                            }
                            #endregion
                            break;
                    }
                }
            }

            if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(DataProviderName))
                throw new Exception("配置文件 bbsmax.config 不正确，请重新上传Install.aspx、Globals.asax、bin\\MaxInstall.dll文件然后运行Install.aspx来启动安装向导");

            string md5 = SecurityUtil.MD5(ConnectionString);

            SafeString = SecurityUtil.MD5(md5).Substring(0, 10);

            FileNamePart = md5.Substring(0, 6);

            LicenceExpireDate = DateTime.MinValue;

            LicencedServers = null;

            if (string.IsNullOrEmpty(Licence) == false)
            {
                string decodedInfo = SecurityUtil.DesDecode(Licence);

                if (decodedInfo == null)
                    return;

                string[] infos = decodedInfo.Split('|');

                if (infos.Length == 2)
                {
                    LicenceExpireDate = DateTime.Parse(infos[0]);
                    LicencedServers = infos[1].Split(',');
                }
            }

            #endregion
        }

        public void Write(string configFilePath)
        {
            StringBuilder xmlBuilder = new StringBuilder(bbsmaxConfig);

            xmlBuilder.Replace("{connectionstring}", StringUtil.HtmlEncode(this.ConnectionString));
            xmlBuilder.Replace("{dataprovidername}", this.DataProviderName);

            xmlBuilder.Replace("{licence}", this.Licence);
            xmlBuilder.Replace("{dynamiccompress}", this.DynamicCompress ? "ON" : "OFF");
            xmlBuilder.Replace("{staticcompress}", this.StaticCompress ? "ON" : "OFF");

            string ownersXml;
            if (this.OwnerUsernames.Count > 0)
            {
                StringBuilder ownerLinesBuilder = new StringBuilder();

                foreach (string username in OwnerUsernames)
                {
                    ownerLinesBuilder.AppendLine(bbsbaxConfig_Owners_Line.Replace("{username}", username));
                }

                ownersXml = bbsbaxConfig_Owners_Tag.Replace("{lines}", ownerLinesBuilder.ToString());
            }
            else
            {
                ownersXml = string.Empty;
            }

            xmlBuilder.Replace("{owners}", ownersXml);

            string passportXml;
            if (this.PassportClient.EnablePassport)
            {
                StringBuilder passportXmlBuilder = new StringBuilder(bbsbaxConfig_Passport_Tag);

                passportXmlBuilder.Replace("{enable}", PassportClient.EnablePassport ? "true" : "false");
                passportXmlBuilder.Replace("{server}", PassportClient.PassportRoot);
                passportXmlBuilder.Replace("{clientid}", PassportClient.ClientID.ToString());
                passportXmlBuilder.Replace("{accesskey}", PassportClient.AccessKey);
                passportXmlBuilder.Replace("{timeout}", PassportClient.PassportTimeout.ToString());

                passportXml = passportXmlBuilder.ToString();
            }
            else
            {
                passportXml = string.Empty;
            }

            xmlBuilder.Replace("{passport}", passportXml);
            xmlBuilder.Replace("{maxPlugins}", MaxPlugins.GetAppConfigString());

            File.WriteAllText(configFilePath, xmlBuilder.ToString(), Encoding.UTF8);
        }

        #region ICloneable 成员

        public object Clone()
        {
            AppConfig config = new AppConfig();
            config.ConnectionString = this.ConnectionString;
            config.DataProviderName = this.DataProviderName;
            config.DynamicCompress = this.DynamicCompress;
            config.FileNamePart = this.FileNamePart;
            config.InstallDirectory = this.InstallDirectory;
            config.Licence = this.Licence;
            config.LicencedServers = this.LicencedServers;
            config.LicenceExpireDate = this.LicenceExpireDate;
            config.OwnerUsernames = new List<string>(this.OwnerUsernames);
            config.PassportClient = (PassportClientConfig)this.PassportClient.Clone();
            config.SafeString = this.SafeString;
            config.StaticCompress = this.StaticCompress;
            config.MaxPlugins = this.MaxPlugins.Clone();
            return config;
        }

        #endregion
    }

    public class AppMaxPlugins:Dictionary<string,MaxPluginItem>,ICloneable<AppMaxPlugins>
    {
        public AppMaxPlugins() 
        {

        }
        public new void Add(string name, MaxPluginItem item)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            name = name.ToLower();
            if (this.ContainsKey(name))
            {
                base[name] = item;
            }
            else
            {
                base.Add(name, item);
            }
        }

        public string GetAppConfigString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<maxPlugins>\r\n");
            foreach (MaxPluginItem item in this.Values)
            {
                sb.AppendFormat("\t<add name=\"{0}\" type=\"{1}\" assembly=\"{2}\" />\r\n", item.Name, item.Type, item.Assembly);
            }
            sb.Append("</maxPlugins>");

            return sb.ToString();
        }

        public new object this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                name = name.ToLower();
                if (this.ContainsKey(name))
                {
                    return base[name].Instance;
                }
                else
                {
                    return null;
                }
            }
        }

        #region ICloneable<AppMaxPlugins> 成员

        public AppMaxPlugins Clone()
        {
            AppMaxPlugins plugins = new AppMaxPlugins();

            foreach (KeyValuePair<string, MaxPluginItem> item in this)
            {
                plugins.Add(item.Key,item.Value);
            }

            return plugins;
        }

        #endregion
    }

    public class MaxPluginItem
    {
        public string Type
        {
            get;
            set;
        }

        public string Assembly
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 实例（动态创建）
        /// </summary>
        private bool tryCreateInstance = false;
        private object m_instance;
        public object Instance
        {
            get
            {
                if (tryCreateInstance == false && m_instance==null )
                {
                    tryCreateInstance = true;
                    string fullTypename = this.Type;
                    if (!string.IsNullOrEmpty(this.Assembly))
                        fullTypename += "," + this.Assembly;
                    Type type = System.Type.GetType(fullTypename);
                    m_instance = Activator.CreateInstance(type);
                }

                return m_instance;
            }
        }
    }
}