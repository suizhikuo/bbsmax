//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Principal;
using System.Diagnostics;
using Max.Installs;

#if SQLSERVER
using System.Data.SqlClient;
using MaxLabs.bbsMax.Enums;
#endif


namespace Max.WebUI
{
    //操作步骤
    public enum Step
    {
        None,
        First,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth
    }

    public partial class Install : System.Web.UI.Page
    {
        public static List<Progress> Progress = new List<Progress>();
        protected Settings settings;
        protected string Self = null;
        private const string UseOldPassword = "(继续使用原密码)";
        private static string oldPassword = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            Settings.Current.RootPath = Globals.RootPath();
            Self = Request.Url.AbsolutePath;

            //处理图片请求
            string name = Context.Request.QueryString["fileName"];
            if (!string.IsNullOrEmpty(name))
            {
                processImageRequest(name);
            }

            Response.ContentEncoding = Encoding.UTF8;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            AjaxJSON();//ajax请求

            if (Settings.Current.ProgressAccess != Max.Installs.Progress.Notset)
            {
                SetVisible(Step.Sixth);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Progress_Refresh", "setTimeout(getContent, 1500);", true);
                return;
            }


            SetVisible(Step.First);
            if (SetupManager.StepChecker < 0)
            {
                this.WelcomeTitle.InnerHtml = "无法正常运行安装向导";
                this.WelcomeBody.InnerHtml = "安装程序不完整,可能您已经成功安装了bbsmax。<br /><br />如果您要再次安装bbsMax，请重新上传 Install.aspx、Global.asax、bin\\MaxInstall.dll 文件，然后重新运行本安装向导；否则请删除本文件以及Bin\\MaxInstall.dll文件。";
                this.ToSecondNext.Enabled = false;
                this.ToOut.Disabled = true;
                return;
            }


            EventsRegister();//加载事件
            if (siteUrl.Value == "")
            {
                string host = Request.Url.Scheme + "://" + Request.Url.Authority;
                string url = host + Request.Url.AbsolutePath;
                url = url.Remove(url.LastIndexOf("/"));

                siteUrl.Value = host;
                //bbsUrl.Value = url;
            }


#if SQLSERVER
            DataBase_Sqlite.Visible = false;
            DataBase_SqlServer.Visible = true;
#endif
#if SQLITE
            DataBase_Sqlite.Visible = true;
            DataBase_SqlServer.Visible = false;
#endif

        }

        protected string GetUrlFormat(string urlFormat)
        {
            switch (urlFormat.ToLower())
            {
                case "aspx": return "ASP.NET模式";
                case "html": return "静态页模式";
                case "query": return "参数模式";
                case "folder": return "无后缀模式";
                default: return "";
            }
        }

        private void processImageRequest(string name)
        {
            switch (name.ToLower())
            {
                case "alert.gif":
                    writeImage(Install_Bin.alert);
                    break;
                case "ok.gif":
                    writeImage(Install_Bin.ok);
                    break;
                case "no.gif":
                    writeImage(Install_Bin.no);
                    break;
                case "maxinstall.gif":
                    writeImage(Install_Bin.maxInstall);
                    break;
                case "loader.gif":
                    writeImage(Install_Bin.loader);
                    break;
                case "checkconnect.gif":
                    writeImage(Install_Bin.checkconnect);
                    break;
                case "max_install.css":
                    Context.Response.ContentEncoding = Encoding.UTF8;
                    Context.Response.ContentType = "text/css";
                    Context.Response.Write(Regex.Replace(Install_Bin.max_install_css, "Install\\.aspx", Path.GetFileName(Context.Request.PhysicalPath), RegexOptions.IgnoreCase));
                    break;
                default:
                    break;
            }
            Context.Response.End();
        }

        void EventsRegister()
        {

            ToFirstPrev.Click += new EventHandler(ToFirstPrev_Click);
            ToSecondNext.Click += new EventHandler(ToSecond_Click);
            ToSecondPrev.Click += new EventHandler(ToSecond_Click);
            ToThirdNext.Click += new EventHandler(ToThird_Click);
            ToThirdPrev.Click += new EventHandler(ToThird_Click);
            ToFourthNext.Click += new EventHandler(ToFourthNext_Click);
            ToFifthNext.Click += new EventHandler(ToFifthNext_Click);
#if SQLSERVER
            CreateDatabase.Click += new EventHandler(CreateDatabase_Click);
#endif
        }

        void CreateDatabase_Click(object sender, EventArgs e)
        {
#if SQLSERVER
            string databaseName = IdMaxDatabase.Text.Trim(' ', '[', ']');

            string connectionString = Settings.ConnectionStringFormat(GetSqlServerAddress(),
                IdMaxUserID.Text.Trim(),
                IdMaxPassword.Text,
                "master",
                this.ThirdIsWindows.SelectedValue == "0") + "Pooling=false;";

            string result = SetupManager.CheckCreateDatabase(connectionString, databaseName);
            if (string.IsNullOrEmpty(result))
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CreateDatabase_Result", "alert('数据库 " + Globals.SafeJS(databaseName) + " 创建成功');", true);
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "CreateDatabase_Result", "alert('" + Globals.SafeJS(result) + "');", true);

            SetVisible(Step.Third);
#endif
        }

        private string GetSqlServerAddress()
        {
            string port = sqlServerPort.Text.Trim();
            string ip = IdMaxServer.Text.Trim();
            if (ip.ToLower().Contains("local") || ip.ToLower().Contains("127.0.0.1") || port == "1433" || port == string.Empty)
            {
            }
            else
                ip = ip + "," + port;

            return ip;
        }

        void ToFirstPrev_Click(object sender, EventArgs e)
        {
            SetVisible(Step.First);
        }

        bool checkDirectory(string directoryName, bool checkSubDirectory, List<string> resultList)
        {
            string result = PermissionTester.CheckDirectory(directoryName, checkSubDirectory);
            if (string.IsNullOrEmpty(result))
            {
                //icon.Attributes["class"] = "ok";
                //message.Text = "权限检查：" + directoryName + " 目录 (通过)";
                resultList.Add(@"<span class=""ok"">.</span>权限检查：" + directoryName + " 目录 (通过)");
                return true;
            }
            else
            {
                //icon.Attributes["class"] = "no";
                //message.Text = "权限检查：" + directoryName + " 目录 (" + result + ")";
                resultList.Add(@"<span class=""no"">.</span>权限检查：" + directoryName + " 目录 (" + result + ")");
                return false;
            }
        }

        bool checkFile(string fileName, List<string> resultList)
        {
            string result = PermissionTester.CheckFile(fileName);
            if (string.IsNullOrEmpty(result))
            {
                //icon.Attributes["class"] = "ok";
                //message.Text = "权限检查：" + fileName + " 文件 (通过)";
                resultList.Add(@"<span class=""ok"">.</span>权限检查：" + fileName + " 文件 (通过)");
                return true;
            }
            else
            {
                //icon.Attributes["class"] = "no";
                //message.Text = "权限检查：" + fileName + " 文件 (" + result + ")";
                resultList.Add(@"<span class=""no"">.</span>权限检查：" + fileName + " 文件 (" + result + ")");
                return false;
            }
        }

        protected List<string> CheckPermissionResult = null;

        //检查目录(读 写 改 删) 程序版本判断
        void ToSecond_Click(object sender, EventArgs e)
        {

            string result = LastestApplicationTester.CheckVersion();
            if (string.IsNullOrEmpty(result))
            {
                Request2.Text = "提醒：请到官方网站检查是否有新版本";
                SecondLastestApplicationImage.Attributes["class"] = "wa";
            }
            else if (result.StartsWith("~"))
            {
                Request2.Text = result.Substring(1);
                SecondLastestApplicationImage.Attributes["class"] = "ok";
            }
            else
            {
                int index = result.ToLower().IndexOf("<!--true-->");
                if (index > 0)
                {
                    Request2.Text = result.Substring(0, index);
                    //ToThirdNext.Enabled = false;
                    thirdNextInfo.Visible = true;
                    thirdNextInfo.Text = Request2.Text;
                    ToThirdNext.Visible = false;
                }
                else
                {
                    Request2.Text = result;
                }


                SecondLastestApplicationImage.Attributes["class"] = "wa";
            }

            bool passed = true;

            List<string> builder = new List<string>();

            //检查目录
            passed = checkDirectory("max-pages", true, builder) && passed;
            passed = checkDirectory("max-admin", true, builder) && passed;
            passed = checkDirectory("max-dialogs", true, builder) && passed;
            passed = checkDirectory("max-plugins", true, builder) && passed;
            passed = checkDirectory("max-spacestyles", true, builder) && passed;
            passed = checkDirectory("max-assets", true, builder) && passed;
            passed = checkDirectory("max-temp", true, builder) && passed;
            passed = checkDirectory("UserFiles", true, builder) && passed;

            //检查文件
            passed = checkFile("web.config", builder) && passed;
            passed = checkFile("bbsmax.config", builder) && passed;
            passed = checkFile("global.asax", builder) && passed;
            passed = checkFile("install.aspx", builder) && passed;
            passed = checkFile("bin\\MaxInstall.dll", builder) && passed;
            passed = checkFile("bin\\MaxLabs.bbsMax.dll", builder) && passed;
            passed = checkFile("bin\\MaxLabs.bbsMax.DataAccess.SqlServer.dll", builder) && passed;
            passed = checkFile("bin\\MaxLabs.bbsMax.RegExp.dll", builder) && passed;
            passed = checkFile("bin\\MaxLabs.bbsMax.Rescourses.dll", builder) && passed;

            CheckPermissionResult = builder;

#if SQLITE
            PermissionSpan.Visible = true;
            passed = checkDirectory("APP_Data", true, PermissionTestIcon_App_Data, PermissionTestMsg_App_Data) && passed;
            passed = checkFile("APP_Data\\bbsmax.config", PermissionTestIcon_App_Data_bbsmax_config, PermissionTestMsg_App_Data_bbsmax_config) && passed;
            passed = checkFile("APP_Data\\idmax.config", PermissionTestIcon_App_Data_idmax_config, PermissionTestMsg_App_Data_idmax_config) && passed;
#endif
            SetVisible(Step.Second);

            ToThirdNext.Enabled = passed;
        }

        //配置安装模式 
        void ToThird_Click(object sender, EventArgs e)
        {
            SetVisible(Step.Third);
            GetConfig();
        }

        //测试 安装模式
        void ToFourthNext_Click(object sender, EventArgs e)
        {
            SetupManager.StepChecker = 1;
            Settings setting = Settings.Current;
#if SQLSERVER


            setting.IServerAddress = TextGetter(GetSqlServerAddress());
            setting.IDatabase = TextGetter(IdMaxDatabase.Text.Trim(' ', '[', ']'));
            setting.IsIWindows = ThirdIsWindows.SelectedValue == "0" ? true : false;
            if (!setting.IsIWindows)
            {
                setting.IUserID = TextGetter(IdMaxUserID.Text);

                if (IdMaxPassword.Text != UseOldPassword)
                    setting.IPassword = TextGetter(IdMaxPassword.Text);
                else
                {
                    setting.IPassword = oldPassword;
                }
            }

            setting.DynamicCompress = DynamicCompress.Checked;

            setting.StaticCompress = StaticCompress.Checked;

            setting.Licence = Licence.Text;

            string result = ConnectionTester.Check();

            if (string.IsNullOrEmpty(result))
            {
                settings = SetupManager.GetSettings();
                result = ConnectionTester.GetUpgradeOption();
                if (result.IndexOf(" 3.") > 0 || result.IndexOf(" 2.") > 0)
                {
                    RabSetupMode.Items[0].Enabled = false;
                    RabSetupMode.Items[1].Enabled = false;
                    if (result.StartsWith("~"))
                    {
                        RabSetupMode.Items[1].Text = " " + result.Substring(1);
                        //RabSetupMode.SelectedIndex = 1;
                    }
                    else
                    {
                        RabSetupMode.Items[1].Text = " " + result;
                        //RabSetupMode.SelectedIndex = 0;
                    }
                    ToFifthNext.Enabled = false;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToFourth_ClickError", "alert('请先升级到4.2.3版本');", true);
                    SetVisible(Step.Fifth);
                }
                else
                {
                    if (result.StartsWith("~"))
                    {
                        RabSetupMode.Items[1].Text = " " + result.Substring(1);
                        RabSetupMode.Items[1].Enabled = true;
                        RabSetupMode.SelectedIndex = 1;
                    }
                    else
                    {
                        RabSetupMode.Items[1].Text = " " + result;
                        RabSetupMode.Items[1].Enabled = false;
                        RabSetupMode.SelectedIndex = 0;
                    }
                    SetVisible(Step.Fifth);
                }
            }
            else
            {
                SetVisible(Step.Third);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ToFourth_ClickError", "alert('" + Globals.SafeJS(result) + "');", true);
            }
#endif
#if SQLITE
            
            setting.BbsMaxFilePath = bbsMaxFilePath.Text;
            setting.IdMaxFilePath = idMaxFilePath.Text;
            RabSetupMode.Items[1].Enabled = false;

            Regex reg = new Regex(@"\\.*?", RegexOptions.IgnoreCase);// /d/bbsmax/bbsmax.config
            string[] array = null;
            if (reg.IsMatch(bbsMaxFilePath.Text))
                array = reg.Split(bbsMaxFilePath.Text);
            setting.BbsMaxDatabase = array[array.Length - 1];

            if (reg.IsMatch(idMaxFilePath.Text))
                array = reg.Split(idMaxFilePath.Text);
            setting.IdMaxDatabase = array[array.Length - 1];
            string result = string.Empty;

            if (ConnectionTester.IsSqlLiteMaxExists())//该数据库是否存在
            {
                settings = SetupManager.GetSettings();
                result = ConnectionTester.GetUpgradeOption();
                if (result.StartsWith("~"))
                {
                    RabSetupMode.Items[1].Text = " " + result.Substring(1);
                    RabSetupMode.Items[1].Enabled = true;
                    RabSetupMode.SelectedIndex = 1;
                }
                else
                {
                    RabSetupMode.Items[1].Text = " " + result;
                    RabSetupMode.Items[1].Enabled = false;
                    RabSetupMode.SelectedIndex = 0;
                }
                SetVisible(Step.Fifth);
            }
            else
            {
                RabSetupMode.SelectedIndex = 0;
                SetVisible(Step.Fifth);
            }
#endif
        }

        void beginSetupError()
        {
            settings = SetupManager.GetSettings();
            string result = ConnectionTester.GetUpgradeOption();
            if (result.StartsWith("~"))
            {
                RabSetupMode.Items[1].Text = " " + result.Substring(1);
                RabSetupMode.Items[1].Enabled = true;
            }
            else
            {
                RabSetupMode.Items[1].Text = " " + result;
                RabSetupMode.Items[1].Enabled = false;
            }
            RabSetupMode.SelectedIndex = 0;
            SetVisible(Step.Fifth);
        }

        //网站配置信息设置
        void ToFifthNext_Click(object sender, EventArgs e)
        {
            if (SetupManager.StepChecker < 1)
            {
                SetVisible(Step.First);
                return;
            }

            Settings setting = Settings.Current;

            setting.SetupMode = (SetupMode)Globals.ToInt32(RabSetupMode.SelectedValue);

            if (setting.SetupMode == SetupMode.New)
            {
                if (string.IsNullOrEmpty(TextGetter(siteName.Value)))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToSixthNext_Error", "alert('网站名称不能为空！！');", true);
                    beginSetupError();
                    return;
                }
                if (string.IsNullOrEmpty(TextGetter(siteUrl.Value)))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToSixthNext_Error", "alert('网站URL不能为空！！');", true);
                    beginSetupError();
                    return;
                }
                if (string.IsNullOrEmpty(TextGetter(bbsName.Value)))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToSixthNext_Error", "alert('论坛名称不能为空！！');", true);
                    beginSetupError();
                    return;
                }
                //if (string.IsNullOrEmpty(TextGetter(bbsUrl.Value)))
                //{
                //    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToSixthNext_Error", "alert('论坛URL不能为空！！');", true);
                //    beginSetupError();
                //    return;
                //}
                //Regex reg = new Regex("[^\x00-\xff]", RegexOptions.IgnoreCase);
                //if (reg.Replace(AdminName.Value, "**").Trim().Length > 12 || reg.Replace(AdminNickName.Value, "**").Trim().Length > 12)
                //{
                //    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToSixthNext_Error", "alert('管理员帐号或者昵称长度过长！！');", true);
                //    beginSetupError();
                //    return;
                //}

                if (string.IsNullOrEmpty(TextGetter(AdminName.Value)) || string.IsNullOrEmpty(TextGetter(AdminPassword.Value)))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ToSixthNext_Error", "alert('用户名或密码不能为空！！');", true);
                    beginSetupError();
                    return;
                }

                if (UrlFormat1.Checked)
                    setting.UrlFormat = UrlFormat.Aspx;
                else if (UrlFormat2.Checked)
                    setting.UrlFormat = UrlFormat.Html;
                else if (UrlFormat3.Checked)
                    setting.UrlFormat = UrlFormat.Query;
                else
                    setting.UrlFormat = UrlFormat.Folder;

                setting.SiteName = TextGetter(siteName.Value);
                setting.SiteUrl = TextGetter(siteUrl.Value);
                setting.BBSName = TextGetter(bbsName.Value);
                //setting.BBSUrl = TextGetter(bbsUrl.Value);
                setting.AdminName = TextGetter(AdminName.Value);
                //setting.AdminNickName = TextGetter(AdminNickName.Value);
                setting.AdminPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(TextGetter(AdminPassword.Value), "MD5");
            }
            if (!setting.IsThreadAlive)
            {
                BeginSetup();
                setting.IsThreadAlive = true;
            }
            SetVisible(Step.Sixth);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "Progress_Refresh", "setTimeout(getContent, 1500);", true);
        }

        //自动获取配置信息
        void GetConfig()
        {
#if SQLSERVER
            string connectionString = string.Empty;
            bool? dynamicCompress = null;
            bool? staticCompress = null;
            string license = string.Empty;

            Settings setting = Settings.Current;
            if (!string.IsNullOrEmpty(setting.IServerAddress) && !string.IsNullOrEmpty(setting.IDatabase) && setting.DynamicCompress != null && setting.StaticCompress != null)
            {
                connectionString = setting.IConnectionString;
                dynamicCompress = setting.DynamicCompress;
                staticCompress = setting.StaticCompress;
                license = setting.Licence;
            }
            else
            {
                try
                {
                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                    fileMap.ExeConfigFilename = Globals.MapPath("bbsmax.config");
                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    connectionString = config.ConnectionStrings.ConnectionStrings["bbsmax"].ConnectionString;

                    if (config.AppSettings.Settings["bbsMax.DynamicCompress"] != null)
                        dynamicCompress = config.AppSettings.Settings["bbsMax.DynamicCompress"].Value == "ON";

                    if (config.AppSettings.Settings["bbsMax.StaticCompress"] != null)
                        staticCompress = config.AppSettings.Settings["bbsMax.StaticCompress"].Value == "ON";

                    if (config.AppSettings.Settings["bbsMax.Licence"] != null)
                        license = config.AppSettings.Settings["bbsMax.Licence"].Value;
                }
                catch
                {
                    string path = Globals.MapPath("zzbird.config");
                    if (File.Exists(path))
                    {

                        XmlDocument xml = new XmlDocument();
                        xml.Load(path);
                        XmlNode node = xml.SelectSingleNode("Configurations/DataProviders");
                        if (node != null)
                        {
                            string provider = (node.Attributes["DefaultDataProvider"].Value);

                            foreach (XmlNode tempNode in node.ChildNodes)
                            {
                                if (tempNode.Attributes["Name"].Value.ToLower() == provider.ToLower())
                                {
                                    connectionString = tempNode.InnerText;
                                }
                            }
                        }
                        else
                        {
                            node = xml.SelectSingleNode("Configurations/DataProvider");
                            if (node != null)
                            {
                                connectionString = node.InnerText;
                            }
                        }
                    }
                }
            }

            ThirdIsWindows.SelectedIndex = 0;
            thirdsql.Visible = true;

            if (string.IsNullOrEmpty(license) == false)
                Licence.Text = license;

            if (string.IsNullOrEmpty(connectionString))
                return;

            Regex regServer = new Regex(@"(?:Data\sSource|server)=(.*?);", RegexOptions.IgnoreCase);
            Regex regUid = new Regex(@"(?:uid|User\sID)=(.*?);", RegexOptions.IgnoreCase);
            Regex regPwd = new Regex(@"(?:Pwd|Password)=(.*?);", RegexOptions.IgnoreCase);
            Regex regDataBase = new Regex(@"(?:Initial\sCatalog|DataBase)=(.*?);", RegexOptions.IgnoreCase);
            Regex regWin = new Regex(@"Integrated\sSecurity=\w+;", RegexOptions.IgnoreCase);

            if (dynamicCompress != null)
                DynamicCompress.Checked = dynamicCompress.Value;

            if (staticCompress != null)
                StaticCompress.Checked = staticCompress.Value;

            if (regServer.IsMatch(connectionString) && regDataBase.IsMatch(connectionString))
            {

                string sqlAddress = regServer.Match(connectionString).Groups[1].Value;
                int index = sqlAddress.IndexOf(',');

                if (index > -1)
                {
                    sqlServerPort.Text = sqlAddress.Substring(index + 1);
                    IdMaxServer.Text = sqlAddress.Substring(0, index);
                }
                else
                {
                    sqlServerPort.Text = "1433";
                    IdMaxServer.Text = regServer.Match(connectionString).Groups[1].Value;
                }
                IdMaxDatabase.Text = regDataBase.Match(connectionString).Groups[1].Value;
            }
            if (regWin.IsMatch(connectionString))
            {
                ThirdIsWindows.SelectedIndex = 1;
                thirdsql.Visible = false;
            }
            else if (regUid.IsMatch(connectionString) && regPwd.IsMatch(connectionString))
            {
                IdMaxUserID.Text = regUid.Match(connectionString).Groups[1].Value;
                string password = regPwd.Match(connectionString).Groups[1].Value;
                if (password != string.Empty)
                {
                    IdMaxPassword.Text = UseOldPassword;
                    oldPassword = password;
                }
            }
#endif

#if SQLITE
            //一般是没用到 因为bbsmax.config是安装时释放...
            string idMaxConnStr = string.Empty;
            string bbsMaxConnStr = string.Empty;

            Settings setting = Settings.Current;
            if (!string.IsNullOrEmpty(setting.BbsMaxDatabase) && !string.IsNullOrEmpty(setting.IdMaxDatabase))
            {
                bbsMaxConnStr = setting.bbsMaxConnectionString;
                idMaxConnStr = setting.idMaxConnectionString;
            }
            else
            {
                try
                {
                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                    fileMap.ExeConfigFilename = Globals.MapPath("bbsmax.config");
                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    bbsMaxConnStr = config.ConnectionStrings.ConnectionStrings["bbsmax"].ConnectionString;
                    idMaxConnStr = config.ConnectionStrings.ConnectionStrings["idmax"].ConnectionString;
                }
                catch//读取旧版本配置文件
                {
                    //idmax
                    string path = Globals.MapPath("zzbird.config");
                    if (File.Exists(path))
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(path);
                        XmlNode node = xml.SelectSingleNode("Configurations/DataProviders");
                        if (node != null)
                        {
                            string provider = (node.Attributes["DefaultDataProvider"].Value);
                            foreach (XmlNode tempNode in node.ChildNodes)
                            {
                                if (tempNode.Attributes["Name"].Value.ToLower() == provider.ToLower() && provider.ToLower() == "sqlite")
                                {
                                    idMaxConnStr = tempNode.InnerText;
                                }
                            }
                        }
                        else
                        {
                            node = xml.SelectSingleNode("Configurations/DataProvider");
                            if (node != null && node.Name.ToLower() == "sqlite")
                            {
                                idMaxConnStr = node.InnerText;
                            }
                        }
                    }
                    //bbsmax
                    path = Globals.MapPath("zzbird.bbsmax.config");
                    if (File.Exists(path))
                    {
                        XmlDocument xml = new XmlDocument();
                        xml.Load(path);
                        XmlNode node = xml.SelectSingleNode("Configurations/DataProviders");
                        if (node != null)
                        {
                            string provider = (node.Attributes["DefaultDataProvider"].Value);
                            foreach (XmlNode tempNode in node.ChildNodes)
                            {
                                if (tempNode.Attributes["Name"].Value.ToLower() == provider.ToLower() && provider.ToLower() == "sqlite")
                                {
                                    bbsMaxConnStr = tempNode.InnerText;
                                }
                            }
                        }
                        else
                        {
                            node = xml.SelectSingleNode("Configurations/DataProvider");
                            if (node != null && node.Name.ToLower() == "sqlite")
                            {
                                bbsMaxConnStr = node.InnerText;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(bbsMaxConnStr) || string.IsNullOrEmpty(idMaxConnStr) || idMaxConnStr == "$(connectionString_idmax)" || bbsMaxConnStr == "$(connectionString_bbsmax)")
                return;

            bbsMaxFilePath.Text = bbsMaxConnStr;
            idMaxFilePath.Text = idMaxConnStr;
#endif
        }

        //操作步骤
        public void SetVisible(Step step)
        {
            StepFirst.Visible = false;
            StepSecond.Visible = false;
            StepThird.Visible = false;
            StepFifth.Visible = false;
            StepSixth.Visible = false;

            switch (step)
            {
                case Step.First:
                    StepFirst.Visible = true;
                    break;
                case Step.Second:
                    StepSecond.Visible = true;
                    break;
                case Step.Third:
                    StepThird.Visible = true;
#if SQLSERVER
                    ThirdIsWindows.Items[0].Attributes["onclick"] = "isShowAccountTextbox()";
                    ThirdIsWindows.Items[1].Attributes["onclick"] = "isShowAccountTextbox()";
#endif
                    break;
                case Step.Fifth:
                    StepFifth.Visible = true;
                    RabSetupMode.Items[0].Attributes["onclick"] = "switchSetupMode()";
                    RabSetupMode.Items[1].Attributes["onclick"] = "switchSetupMode()";
                    break;
                case Step.Sixth:
                    StepSixth.Visible = true;
                    break;
            }
        }

        //开始安装
        private void BeginSetup()
        {
            Thread SetupThread = new Thread(Setup_Database);
            SetupThread.IsBackground = true;
            SetupThread.Start();
        }

        //安装、更新数据库
        private void Setup_Database()
        {
            Settings setting = Settings.Current;
            setting.Error = -1;
            int stepIndex = 0;
            try
            {
                stepIndex = 1;
                setting.ProgressAccess = new Progress("写入配置信息", 0, stepIndex);
                //SetupManager.ConfigConnectionString();
                setting.ProgressAccess = new Progress("写入配置信息", 100, stepIndex);

                //==============================================

                stepIndex = 2;
                setting.ProgressAccess = new Progress("建立数据库", 0, stepIndex);
                SetupManager.InstallDatabase(delegate(int percent, string message)
                {
                    setting.ProgressAccess = new Progress("建立数据库", percent, stepIndex);
                });
#if SQLSERVER
                if (Settings.Current.SetupMode == Max.Installs.SetupMode.New)
                    SetupManager.CreateAdministrator();
#endif
                setting.ProgressAccess = new Progress("建立数据库", 100, stepIndex);


                //==============================================


                if (Settings.Current.SetupMode != Max.Installs.SetupMode.New)
                {

                    stepIndex = 3;
                    setting.ProgressAccess = new Progress("处理用户组和勋章数据", 0, stepIndex);

                    SetupManager.ConvertRoles();
                    SetupManager.ConvertMedals();
                    //处理用户扩展信息  一定要放在 ConvertRoles 和 ConvertMedals 后面执行
                    SetupManager.ProcessUserExtendData();

                    SetupManager.ConvertLinks();
                    SetupManager.ConvertPoints();
                    SetupManager.ConvertForumLogos();
                    SetupManager.ConvertJudgements();
                    SetupManager.ConvertEmailSettingsAndKeywords();

                    SetupManager.ProcessSetting();

                    //SetupManager.ProcessUsernames();
                    setting.ProgressAccess = new Progress("处理用户组和勋章数据", 100, stepIndex);
                    //==============================================

                    stepIndex = 4;
                    setting.ProgressAccess = new Progress("处理头像数据", 0, stepIndex);

                    SetupManager.ConvertAvatars(delegate(int percent, string message)
                    {
                        setting.ProgressAccess = new Progress("处理头像数据", percent, stepIndex);
                    });

                    SetupManager.ProcessAvatars();

                    setting.ProgressAccess = new Progress("处理头像数据", 100, stepIndex);

                    //==============================================

                    stepIndex = 5;
                    setting.ProgressAccess = new Progress("正在准备数据库校对", 0, stepIndex);

                    string[] prefixs = new string[] { "bx_", "bbsMax_", "Max_", "System_bbsMax_", "System_Max_" };
                    SetupManager.AutoDeployDatabase(delegate(int percent, string message)
                    {
                        setting.ProgressAccess = new Progress(message, percent, stepIndex);
                    }, prefixs);


                    setting.ProgressAccess = new Progress("升级积分公式", 95, stepIndex);
                    SetupManager.ProcessPointsExpression();

                    setting.ProgressAccess = new Progress("升级用户信息", 97, stepIndex);
                    SetupManager.UpdateUserInfos();

                    setting.ProgressAccess = new Progress("数据库校对完成", 100, stepIndex);

                    //==============================================
                }

                //==============================================

                stepIndex = 6;
                setting.ProgressAccess = new Progress("完成安装", 0, stepIndex);
                setting.IsCompleted = true;
                setting.ProgressAccess = new Progress("完成安装", 100, stepIndex);
                setting.ProgressAccess = new Progress("Completed", 100, 10000);
            }
            catch (Exception e)
            {
                setting.Error = 1;
                SetupManager.CreateLog(e);
                setting.ProgressAccess = new Progress("安装出错", 0, stepIndex, string.Format("安装出错:{0}", e.Message));
                setting.Error = stepIndex + 1;
                setting.IsThreadAlive = false;
            }
            finally
            {
                Thread.CurrentThread.Abort();
            }
            stepIndex += 1;
        }

        private string TextGetter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            else
                return text.Trim();
        }

        //ajax定时请求
        private void AjaxJSON()
        {
            string ajaxQS = Request["ajaxstr"];
            if (ajaxQS == "getstate")
            {
                Max.Installs.Progress p = Settings.Current.ProgressAccess;
                int isCompleted = Settings.Current.IsCompleted ? 1 : 0;
                string isError = string.IsNullOrEmpty(p.Error) ? "false" : "true";
                string error = p.Error;
                string step = p.Step.ToString();
                string percent = p.Percent.ToString();
                string indx = Settings.Current.Error.ToString();

                string message = Settings.Current.Message;

                if (string.IsNullOrEmpty(message))
                    message = string.Empty;

                if (Settings.Current.IsCompleted)
                {
                    //删除、修改文件



                    SetupManager.ConvertDefaultEmotions();
#if SQLITE
                    if (Settings.Current.SetupMode == Max.Installs.SetupMode.New)
                        error = SetupManager.SetDatabase();
#endif
                    error += SetupManager.ConfigConnectionString();

                    //try
                    //{
                    //    tryDeleteDirectory("/UserFiles/Avatar");
                    //}
                    //catch (Exception ex)
                    //{
                    //    CreateLog(ex.Message);
                    //}

                    if (string.IsNullOrEmpty(error))
                        error = SetupManager.AlterGolbals();

                    if (string.IsNullOrEmpty(error))
                    {
                        SetupManager.DeleteOldFiles();

                        bool deleteImages = (SetupManager.ErrorMessages.Count == 0);
                        if (string.IsNullOrEmpty(message))
                            message += SetupManager.DeleteSetupFiles(deleteImages);
                        else
                            message += "<br />" + SetupManager.DeleteSetupFiles(deleteImages);


                        message += "<br />提示：任何页面首次访问将需要3-5秒钟才能打开，这是为了加速之后的访问，不是程序有问题（每次重启服务器都会进行加速）";

                        if (SetupManager.ErrorMessages.Count > 0)
                        {

                            message = message + "<br />升级过程中发生了" + SetupManager.ErrorMessages.Count + "个错误,需要您手动修正，具体请参见安装目录下的InstallLog.txt文件";

                            string errorMessages = "===================================================================\r\n";
                            errorMessages += "升级过程中发生了" + SetupManager.ErrorMessages.Count + "个错误,需要您手动修正以下错误\r\n";

                            int i = 1;
                            foreach (string m in SetupManager.ErrorMessages)
                            {
                                errorMessages += i + "." + m + "\r\n";
                                i++;
                            }
                            errorMessages += "\r\n最后请删除目录“/Images”\r\n";
                            errorMessages += "===================================================================\r\n";

                            SetupManager.CreateLog(errorMessages);
                        }
                    }
                    else
                    {
                        isError = "true";
                        isCompleted = 0;
                        //只要出错，就中止过程
                        Settings.Current.ProgressAccess = Max.Installs.Progress.Notset;
                    }


                }
                else if (!string.IsNullOrEmpty(p.Error))
                {
                    isError = "true";
                    isCompleted = 0;
                    Settings.Current.ProgressAccess = Max.Installs.Progress.Notset;
                }

                string json = "p = {'IsError':" + isError + ",'Error':'" + Globals.SafeJS(error) + "','message':'" + Globals.SafeJS(message) + "','Indx':" + indx + ",'Step':" + step + ",'Percent':" + percent + ",'IsCompleted':" + isCompleted + ",'title':'" + Globals.SafeJS(Settings.Current.ProgressAccess.Title) + "'};";
                Response.Write(json);
                Response.End();
            }
        }

        //图片输出
        private void writeImage(System.Drawing.Image img)
        {
            HttpResponse response = Page.Response;
            MemoryStream fs = new MemoryStream();
            img.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
            byte[] buffer = fs.GetBuffer();
            response.ContentType = "image/gif";
            response.BinaryWrite(buffer);
            response.Flush();
        }
    }
}