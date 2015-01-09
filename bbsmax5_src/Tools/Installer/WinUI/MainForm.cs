//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.Security;
using System.Threading;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Resources;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.AccessControl;
using Max.Installs;

namespace Max.WinUI
{
    public delegate void InstallBarDelegate();
    public delegate void InstallDelegate(int total,int percent);
    public delegate void ZipPauseDelegate();

    public partial class MainForm : Form
    {
        protected Settings settings;
        InstallWaiter installWaiter;
        public event ZipPauseDelegate PauseEvent;
        Thread InstallThread;
        int total = 0;

        public MainForm()
        {
            InitializeComponent();
            Init();//初始化
        }

        private void Init()
        {
#if SQLSERVER
            DataBase_SqlServer.Visible = true;
            DataBase_Sqlite.Visible = false;
#endif
#if SQLITE
            DataBase_SqlServer.Visible = false;
            DataBase_Sqlite.Visible = true;
#endif
            Settings.Current.RootPath = Globals.RootPath();
            SiteInfo.Current.DomainName = IISManager.GetHostName();
            SiteInfo.Current.IP = IISManager.GetHostIP();
            SiteInfo.Current.IVersion = IISManager.GetIISVersion();
        }

        private void AgreeButton_Click(object sender, EventArgs e)
        {
            AgreeButton.Enabled = false;
            DisagreeButton.Enabled = false;
            ListIIS();
            ip.Text = SiteInfo.Current.IP;
            Step2_Prev.Enabled = true;
            this.tabControl1.SelectedIndex = 1;
        }

        private void DisagreeButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您选择了不同意本协议，确定退出吗？", "bbsMax安装向导", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                this.Close();
        }

        private void Step2_Canel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确实要退出安装吗？", "bbsMax安装向导", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                this.Close();
        }

        private void Step2_Prev_Click(object sender, EventArgs e)
        {
            AgreeButton.Enabled = true;
            DisagreeButton.Enabled = true;
            this.tabControl1.SelectedIndex = 0;
        }

        private void Step2_Next_Click(object sender, EventArgs e)
        {
            Step2_Next.Enabled = false;
            Step2_Prev.Enabled = false;
            step3_Next.Enabled = true;
            step3_Prev.Enabled = true;
            string error = string.Empty;
            string path = string.Empty;
            if (radioButton3.Checked)
            {
                try
                {
                    path = iisList.SelectedNode.Name;
                }
                catch
                {
                    path = null;
                }
                if (!string.IsNullOrEmpty(path) && path != SiteInfo.Current.DomainName)
                    IISManager.SetSiteInfo(path);
                else
                    error = "请选择一个站点或虚拟目录";
            }
            else if (SiteInfo.Current.WebPath == "")
                error = "请先建立一个站点";
            //设置文件夹权限
            if (string.IsNullOrEmpty(error))
            {
                ChangePathPurview(SiteInfo.Current.WebPath, SiteInfo.Current.UserName_iusr, "ReadOnly");
                ChangePathPurview(SiteInfo.Current.WebPath, SiteInfo.Current.UserName_iwam, "FullControl");
                this.tabControl1.SelectedIndex = 2;
                address.Focus();
            }
            else
            {
                MessageBox.Show(error, "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Step2_Prev.Enabled = true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Step2_Next.Enabled = false;
            panel2.Visible = true;
            panel3.Visible = false;
            serverComment.Focus();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Step2_Next.Enabled = false;
            panel2.Visible = false;
            panel3.Visible = true;
        }

        private void browser1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                webPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void browser2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                webPath1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void createVirtual_Click(object sender, EventArgs e)
        {
            if (iisList.SelectedNode == null || iisList.SelectedNode.Text == SiteInfo.Current.DomainName)
            {
                MessageBox.Show("请指定一个站点或虚拟目录", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (virtualName.Text.Trim() == "")
            {
                virtualName.Focus();
                MessageBox.Show("虚拟目录名称不能为空", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (webPath1.Text.Trim() == "")
            {
                webPath1.Focus();
                MessageBox.Show("请选择路径", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                SiteInfo.Current.AppPool = "bbsmax_" + Guid.NewGuid().ToString().Substring(0, 8);
                SiteInfo.Current.VirPath = iisList.SelectedNode.Name;
                try
                {
                    SiteInfo.Current.CurrentSite = new DirectoryEntry(SiteInfo.Current.VirPath);
                    SiteInfo.Current.DefaultDoc = "Default.aspx,Default.html,index.aspx,index.html";
                    SiteInfo.Current.VirtualName = virtualName.Text.Trim();
                    SiteInfo.Current.WebPath = webPath1.Text.Trim();
                    try
                    {
                        UserSet();//IIS帐户设置
                        IISManager.CreateVirtual();
                        virtualName.Text = "";
                        webPath1.Text = "";
                        MessageBox.Show("创建虚拟目录成功。", "IIS管理器", MessageBoxButtons.OK);
                        Step2_Next.Enabled = true;
                        Step2_Next.Focus();
                        ListIIS();//刷新treeView并选中当前目录
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("创建虚拟目录失败:" + ex.Message, "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch
                {
                    MessageBox.Show("当前路径有错，创建失败。", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void creatSite_Click(object sender, EventArgs e)
        {
            if (SiteInfo.Current.IVersion < IISVersion.IIS6)
            {
                MessageBox.Show("你的IIS版本太低，无法创建站点", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (serverComment.Text == "")
            {
                MessageBox.Show("网站描述不能为空", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                serverComment.Focus();
            }
            else if (ip.Text == "")
            {
                MessageBox.Show("无效的IP地址", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ip.Focus();
            }
            else if (serverBindings.Text == "")
            {
                MessageBox.Show("无效的端口号", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                serverBindings.Focus();
            }
            else if (webPath.Text == "")
            {
                MessageBox.Show("请选择路径", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                browser1.Focus();
            }
            else
            {
                SiteInfo.Current.ServerComment = serverComment.Text.Trim();
                SiteInfo.Current.IP = ip.Text.Trim();
                SiteInfo.Current.Port = serverBindings.Text.Trim();
                SiteInfo.Current.Host = host.Text.Trim();
                SiteInfo.Current.WebPath = webPath.Text.Trim();
                SiteInfo.Current.DefaultDoc = "Default.aspx,Default.html,index.aspx,index.html";
                SiteInfo.Current.AppPool = "bbsmax_" + Guid.NewGuid().ToString().Substring(0, 8);
                try
                {
                    UserSet();//IIS帐户设置
                    IISManager.CreateWebsite();
                    serverComment.Text = "";
                    host.Text = "";
                    webPath.Text = "";
                    MessageBox.Show("创建网站成功，直接点下一步进行安装。", "IIS管理器", MessageBoxButtons.OK);
                    ListIIS();
                    Step2_Next.Enabled = true;
                    Step2_Next.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("创建网站失败:" + ex.Message, "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void iisList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Step2_Next.Enabled = true;
        }

        private void deleteVirtual_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除吗?", "IIS管理器", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (iisList.SelectedNode != null)
                {
                    try
                    {
                        IISManager.DeleteSiteVirtual(iisList.SelectedNode.Name);
                        MessageBox.Show("删除成功", "IIS管理器", MessageBoxButtons.OK);
                        ListIIS();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                    MessageBox.Show("请选择要删除的站点或虚拟目录", "IIS管理器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void createDataBase_Click(object sender, EventArgs e)
        {
#if SQLSERVER
            string databaseName = dbName.Text.Trim(' ', '[', ']');
            string connectionString = Settings.ConnectionStringFormat(address.Text.Trim(),
                userName.Text.Trim(),
                dbPwd.Text,
                "master",
                false) + "Pooling=false;";

            string result = SetupManager.CheckCreateDatabase(connectionString, databaseName);
            if (string.IsNullOrEmpty(result))
                MessageBox.Show("数据库" + databaseName + "创建成功", "安装向导", MessageBoxButtons.OK);
            else
                MessageBox.Show(result, "安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif
        }

        private void step3_Canel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确实要退出安装吗？", "bbsMax安装向导", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                this.Close();
        }

        private void step3_prev_Click(object sender, EventArgs e)
        {
            Step2_Next.Enabled = true;
            Step2_Prev.Enabled = true;
            this.tabControl1.SelectedIndex = 1;
        }

        private void step3_Next_Click(object sender, EventArgs e)
        {
            webSite.Focus();
            step3_Next.Enabled = false;
            step3_Prev.Enabled = false;
            step4_Next.Enabled = true;
            step4_Prev.Enabled = true;
            Settings setting = Settings.Current;
#if SQLSERVER
            setting.IServerAddress = address.Text.Trim();
            setting.IDatabase = dbName.Text.Trim(' ', '[', ']');
            setting.IsIWindows = false;
            if (!setting.IsIWindows)
            {
                setting.IUserID = userName.Text.Trim();
                setting.IPassword = dbPwd.Text.Trim();
            }

            string result = ConnectionTester.Check();

            if (string.IsNullOrEmpty(result))
            {
                settings = SetupManager.GetSettings();
                result = ConnectionTester.GetUpgradeOption();
                if (result.StartsWith("~"))//修改安装
                {
                    changeInstall.Text = result.Substring(1);
                    changeInstall.Enabled = true;
                    changeInstall.Checked = true;
                    webSite.Text = settings.SiteName;
                    webSite.Enabled = false;
                    webUrl.Text = settings.SiteUrl;
                    webUrl.Enabled = false;
                    bbsName.Text = settings.BBSName;
                    bbsName.Enabled = false;
                    //bbsUrl.Text = settings.BBSUrl;
                    //bbsUrl.Enabled = false;
                    adminName.Text = settings.AdminName;
                    adminName.Enabled = false;
                    label23.Visible = false;
                    adminPwd.Visible = false;
                    //adminNickName.Text = settings.AdminNickName;
                    //adminNickName.Enabled = false;
                }
                else
                {
                    adminName.Text = "admin";
                    adminNickName.Text = "admin";
                    //ip或主机头..虚拟目录名称？多级虚拟目录..
                    string port = SiteInfo.Current.Port == "80" ? "" : ":" + SiteInfo.Current.Port;
                    string ip = SiteInfo.Current.IP == "" ? "localhost" : SiteInfo.Current.IP;
                    string host = SiteInfo.Current.Host == "" ? ip + port : SiteInfo.Current.Host + port;
                    webUrl.Text = string.Format("http://{0}", host);
                    //bbsUrl.Text = string.Format("http://{0}/{1}", host, SiteInfo.Current.VirtualName);
                    //
                    changeInstall.Text = result;
                    changeInstall.Enabled = false;
                    newInstall.Checked = true;
                }
                this.tabControl1.SelectedIndex = 3;
            }
            else
            {
                step3_Next.Enabled = true;
                step3_Prev.Enabled = true;
                MessageBox.Show("连接数据库失败", "安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
#endif
#if SQLITE
            
            setting.BbsMaxFilePath = bbsMaxFilePath.Text;
            setting.IdMaxFilePath = idMaxFilePath.Text;

            adminName.Text = "admin";
            adminNickName.Text = "admin";
            //ip或主机头..虚拟目录名称？多级虚拟目录..
            string port = SiteInfo.Current.Port == "80" ? "" : ":" + SiteInfo.Current.Port;
            string ip = SiteInfo.Current.IP == "" ? "localhost" : SiteInfo.Current.IP;
            string host = SiteInfo.Current.Host == "" ? ip + port : SiteInfo.Current.Host + port;
            webUrl.Text = string.Format("http://{0}", host);
            bbsUrl.Text = string.Format("http://{0}/{1}", host, SiteInfo.Current.VirtualName);

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
                    changeInstall.Text = result.Substring(1);
                    changeInstall.Enabled = true;
                    changeInstall.Checked = true;
                    webSite.Text = settings.SiteName;
                    webSite.Enabled = false;
                    webUrl.Text = settings.SiteUrl;
                    webUrl.Enabled = false;
                    bbsName.Text = settings.BBSName;
                    bbsName.Enabled = false;
                    bbsUrl.Text = settings.BBSUrl;
                    bbsUrl.Enabled = false;
                    adminName.Text = settings.AdminName;
                    adminName.Enabled = false;
                    label23.Visible = false;
                    adminPwd.Visible = false;
                    adminNickName.Text = settings.AdminNickName;
                    adminNickName.Enabled = false;
                }
                else
                {
                    changeInstall.Text = result;
                    changeInstall.Enabled = false;
                    newInstall.Checked = true;
                }
                this.tabControl1.SelectedIndex = 3;
            }
            else
            {
                changeInstall.Enabled = false;
                newInstall.Checked = true;

                this.tabControl1.SelectedIndex = 3;
            }
#endif
        }

        private void step4_Canel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("您确实要退出安装吗？", "bbsMax安装向导", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                this.Close();
        }

        private void step4_Prev_Click(object sender, EventArgs e)
        {
            step3_Next.Enabled = true;
            step3_Prev.Enabled = true;
            this.tabControl1.SelectedIndex = 2;
        }

        private void step4_Next_Click(object sender, EventArgs e)
        {
            step4_Next.Enabled = false;
            step4_Prev.Enabled = false;
            Finish.Enabled = true;
            Settings setting = Settings.Current;
            setting.SetupMode = newInstall.Checked == true ? SetupMode.New : SetupMode.Update;

            if (webSite.Text.Trim() == "")
            {
                MessageBox.Show("网站名称不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                webSite.Focus();
            }
            else if (webUrl.Text.Trim() == "")
            {
                MessageBox.Show("网站URL不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                webUrl.Focus();
            }
            else if (bbsName.Text.Trim() == "")
            {
                MessageBox.Show("论坛名称不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                bbsName.Focus();
            }
            else if (bbsUrl.Text.Trim() == "")
            {
                MessageBox.Show("论坛URL不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                bbsUrl.Focus();
            }
            else if (adminName.Text.Trim() == "")
            {
                MessageBox.Show("管理员不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                adminName.Focus();
            }
            else if (adminName.Text.Trim() == "")
            {
                MessageBox.Show("管理员帐号不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                adminName.Focus();
            }
            else if (adminNickName.Text.Trim() == "")
            {
                MessageBox.Show("管理员昵称不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                adminNickName.Focus();
            }
            else if (adminPwd.Text.Trim() == "" && newInstall.Checked == true)
            {
                MessageBox.Show("管理员密码不能为空", "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                step4_Next.Enabled = true;
                step4_Prev.Enabled = true;
                adminPwd.Focus();
            }
            else
            {
                setting.BBSName = bbsName.Text.Trim();
                //setting.BBSUrl = bbsUrl.Text.Trim();
                setting.SiteName = webSite.Text.Trim();
                setting.SiteUrl = webUrl.Text.Trim();
                setting.AdminName = adminName.Text.Trim();
                //setting.AdminNickName = adminNickName.Text.Trim();
                setting.AdminPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(adminPwd.Text.Trim(), "MD5");
                if (!setting.IsThreadAlive)
                {
                    installWaiter = new InstallWaiter(this);
                    installWaiter.InstallBarEvent += InstallBar;
                    InstallThread = new Thread(new ThreadStart(Setup));
                    setting.IsThreadAlive = true;
                    InstallThread.IsBackground = true;
                    InstallThread.Start();
                }
                this.tabControl1.SelectedIndex = 4;
            }
        }

        private void newInstall_CheckedChanged(object sender, EventArgs e)
        {
            adminName.Text = "admin";
            adminNickName.Text = "admin";
            //ip或主机头..虚拟目录名称？多级虚拟目录..
            string port = SiteInfo.Current.Port == "80" ? "" : ":" + SiteInfo.Current.Port;
            string ip = SiteInfo.Current.IP == "" ? "localhost" : SiteInfo.Current.IP;
            string host = SiteInfo.Current.Host == "" ? ip + port : SiteInfo.Current.Host + port;
            webUrl.Text = string.Format("http://{0}", host);
            //bbsUrl.Text = string.Format("http://{0}/{1}", host, SiteInfo.Current.VirtualName);

            webSite.Text = "";
            bbsName.Text = "";
            adminPwd.Text = "";

            label23.Visible = true;
            adminPwd.Visible = true;
            webSite.Enabled = true;
            webUrl.Enabled = true;
            bbsName.Enabled = true;
            //bbsUrl.Enabled = true;
            adminName.Enabled = true;
            adminPwd.Enabled = true;
            //adminNickName.Enabled = true;
        }

        private void changeInstall_CheckedChanged(object sender, EventArgs e)
        {
            label23.Visible = false;
            adminPwd.Visible = false;
            webSite.Text = (settings == null) ? "" : settings.SiteName;
            webSite.Enabled = false;
            webUrl.Text = (settings == null) ? "" : settings.SiteUrl;
            webUrl.Enabled = false;
            bbsName.Text = (settings == null) ? "" : settings.BBSName;
            bbsName.Enabled = false;
            //bbsUrl.Text = (settings == null) ? "" : settings.BBSUrl;
            //bbsUrl.Enabled = false;
            adminName.Text = (settings == null) ? "" : settings.AdminName;
            adminName.Enabled = false;
            //adminNickName.Text = (settings == null) ? "" : settings.AdminNickName;
            //adminNickName.Enabled = false;
        }

        private void Finish_Click(object sender, EventArgs e)
        {
            if (Finish.Text == "取  消")
            {
                if (InstallThread != null && InstallThread.ThreadState != ThreadState.Aborted)
                    Pause();
                DialogResult dr = MessageBox.Show("您确实要退出安装吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel)
                {
                    if (InstallThread != null && InstallThread.ThreadState != ThreadState.Aborted)
                        Pause();
                }
                else if (dr == DialogResult.OK)
                {
                    if (InstallThread != null && InstallThread.ThreadState != ThreadState.Aborted)
                        StopThread();
                    this.Close();
                }
            }
            else if (Finish.Text == "完  成")
            {
                StopThread();
                Settings.Current.IsThreadAlive = false;
                this.Close();
                System.Diagnostics.Process.Start(string.Format("http://{0}/{1}", host, SiteInfo.Current.VirtualName));
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Finish.Text != "完  成")
            {
                if (MessageBox.Show("\r\n您确实要退出安装吗？", "bbsMax安装向导", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        StopThread();
                        base.OnFormClosing(e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                    e.Cancel = true;
            }
        }

        protected void StopThread()
        {
            if (InstallThread != null && InstallThread.ThreadState != ThreadState.Aborted)
            {
                try
                {
                    InstallThread.Abort();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// 开始安装
        /// </summary>
        private void Setup()
        {
            int stepIndex = 0;
            Settings setting = Settings.Current;
            setting.RootPath = SiteInfo.Current.WebPath;
            string error = string.Empty;
            try
            {
                //解压文件
                stepIndex++;
                installWaiter.UpZip();

                ResetBar();

                //数据库操作  sql
                stepIndex++;
                SetupManager.InstallDatabase(delegate(int percent, string message)
                {
                    setting.ProgressAccess = new Progress("安装/更新数据库", percent, stepIndex);
                    InstallBarDelegate delegateInstallDb = new InstallBarDelegate(InstallDb);
                    installBars.BeginInvoke(delegateInstallDb);
                });

#if SQLITE
            if (Settings.Current.SetupMode == Max.Installs.SetupMode.New)
                error = SetupManager.SetDatabase();
#endif

                ResetBar();

                //写入配置文件
                stepIndex++;
                ConfigConnectionString();
                SetupManager.AlterGolbals(SiteInfo.Current.WebPath + "\\");
#if SQLSERVER
                if (Settings.Current.SetupMode == Max.Installs.SetupMode.New)
                    SetupManager.CreateAdministrator();//创建管理员
#endif
                setting.ProgressAccess = new Progress("写入配置文件", 100, stepIndex);
                InstallBarDelegate delegateSetConfig = new InstallBarDelegate(SetConfig);
                installBars.BeginInvoke(delegateSetConfig);

                ResetBar();

                //特殊文件夹权限设置
                SetSpecialPurview();
                setting.ProgressAccess = new Progress("设置文件夹权限", 100, stepIndex);
                InstallBarDelegate delegateSetPurview = new InstallBarDelegate(SetPurview);
                installBars.BeginInvoke(delegateSetPurview);

                //完成
                InstallBarDelegate delegateInstallFinish = new InstallBarDelegate(InstallFinish);
                installBars.BeginInvoke(delegateInstallFinish);
            }
            catch (Exception ex)
            {
                Thread.CurrentThread.Abort();
                SetupManager.CreateLog(ex);
                MessageBox.Show(ex.Message, "bbsMax安装向导", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// Reset进度条
        /// </summary>
        private void ResetBar()
        {
            InstallBarDelegate myDelegateResetBar = new InstallBarDelegate(DelegateResetBar);
            installBars.BeginInvoke(myDelegateResetBar);
        }

        public void Pause()
        {
            this.PauseEvent();
        }

        public void InstallBar(int total, int percent)
        {
            this.total = total;
            Settings.Current.ProgressAccess = new Progress("解压文件", percent, 1);
            InstallBarDelegate installBarDelegate = new InstallBarDelegate(Bar);
            installBars.BeginInvoke(installBarDelegate);
        }

        //一系列事件的方法
        void Bar()
        {
            if (installBars.Value < 100 && Settings.Current.ProgressAccess.Percent % (total / 100) == 0)
                installBars.Value++;
        }

        private void InstallDb()
        {
            installStep.Text = Settings.Current.ProgressAccess.Title;
            if (installBars.Value < 100)
                installBars.Value = Settings.Current.ProgressAccess.Percent;
        }

        private void SetConfig()
        {
            installStep.Text = Settings.Current.ProgressAccess.Title;
            if (installBars.Value < 100)
                installBars.Value = Settings.Current.ProgressAccess.Percent;
        }

        private void SetPurview()
        {
            installStep.Text = Settings.Current.ProgressAccess.Title;
            if (installBars.Value < 100)
                installBars.Value = Settings.Current.ProgressAccess.Percent;
        }

        private void DelegateResetBar()
        {
            installBars.Value = 0;
        }

        private void InstallFinish()
        {
            installPanel.Visible = false;
            finishLabel.Text = "完  成";
            Finish.Text = "完  成";
        }

        TreeNode rootNode = null;
        /// <summary>
        /// 递归显示IIS树，做了特殊处理
        /// </summary>
        /// <param name="subDre"></param>
        /// <param name="nodeToAddTo"></param>
        private void GetDirectories(DirectoryEntries subDre, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryEntries subDres;
            foreach (DirectoryEntry dr in subDre)
            {
                if (dr.SchemaClassName.Equals("IIsWebServer") || dr.SchemaClassName.Equals("IIsWebVirtualDir"))
                {
                    if (dr.SchemaClassName.Equals("IIsWebServer"))
                    {
                        aNode = new TreeNode();
                        aNode.Text = dr.Properties["ServerComment"].Value.ToString();
                        aNode.Name = dr.Path;// + "/ROOT"
                        rootNode = aNode;
                    }
                    else
                    {
                        aNode = new TreeNode();
                        aNode.Text = dr.Name;
                        aNode.Name = dr.Path;
                    }
                    subDres = dr.Children;
                    if (subDres != null)
                    {
                        if (string.Compare(dr.Name, "root", true) == 0)
                            GetDirectories(subDres, rootNode);//略过第二结点
                        else
                            GetDirectories(subDres, aNode);
                    }
                    if (aNode.Text != "" && (string.Compare(dr.Name, "root", true) != 0))
                    {
                        nodeToAddTo.Nodes.Add(aNode);
                        rootNode = nodeToAddTo;
                    }
                }
            }
        }
        /// <summary>
        /// ListIIS
        /// </summary>
        private void ListIIS()
        {
            DirectoryEntry root = new DirectoryEntry(string.Format("IIS://{0}/W3SVC", SiteInfo.Current.DomainName));
            TreeNode rootNode;
            iisList.Nodes.Clear();
            rootNode = new TreeNode();
            rootNode.Text = SiteInfo.Current.DomainName;
            rootNode.Name = SiteInfo.Current.DomainName;
            GetDirectories(root.Children, rootNode);
            iisList.Nodes.Add(rootNode);
        }
        /// <summary>
        /// 配置文件
        /// </summary>
        private void ConfigConnectionString()
        {
            try
            {
                string configFile = SiteInfo.Current.WebPath + "\\bbsmax.config";
                SetupManager.CanelReadOnly(configFile);
                string configFileContent = Install_Bin.bbsmax_config;
#if SQLSERVER
                configFileContent = configFileContent.Replace("$(connectionString)", Settings.Current.IConnectionString);
#endif
#if SQLITE
                configFileContent = configFileContent.Replace("$(connectionString_idmax)", Settings.Current.IdMaxFilePath);
                configFileContent = configFileContent.Replace("$(connectionString_bbsmax)", Settings.Current.BbsMaxFilePath);
                configFileContent = configFileContent.Replace("$(providerName_idmax)", "MaxLab.CommonDataProvider.Sqlite3");
                configFileContent = configFileContent.Replace("$(providerName_bbsmax)", "MaxLab.bbsMaxDataProvider.Sqlite3");
#endif
                File.WriteAllText(configFile, configFileContent);
            }
            catch (Exception e)
            {
                SetupManager.CreateLog(e);
            }
        }
        /// <summary>
        /// 设置特殊文件夹权限
        /// </summary>
        private void SetSpecialPurview()
        {
            ChangePathPurview(SiteInfo.Current.WebPath + "/Images", SiteInfo.Current.UserName_iusr, "ReadOnly");
            ChangePathPurview(SiteInfo.Current.WebPath + "/Languages ", SiteInfo.Current.UserName_iusr, "ReadOnly");
            ChangePathPurview(SiteInfo.Current.WebPath + "/Web_Code", SiteInfo.Current.UserName_iusr, "ReadOnly");
            ChangePathPurview(SiteInfo.Current.WebPath + "/MaxTemp", SiteInfo.Current.UserName_iusr, "ReadOnly");
            ChangePathPurview(SiteInfo.Current.WebPath + "/error", SiteInfo.Current.UserName_iusr, "ReadOnly");
#if SQLITE
            ChangePathPurview(SiteInfo.Current.WebPath + "/App_Data", SiteInfo.Current.UserName_iusr, "ReadOnly");
#endif
            ChangePathPurview(SiteInfo.Current.WebPath + "/UserFiles", SiteInfo.Current.UserName_iusr, "ReadOnly");
            ChangePathPurview(SiteInfo.Current.WebPath + "/UserFiles", SiteInfo.Current.UserName_iusr, "Write");

            ChangeTempPurview();
        }
        /// <summary>
        /// IIS用户创建
        /// </summary>
        private void UserSet()
        {
            string pwd = Guid.NewGuid().ToString();
            string IIS_UserGroup = (SiteInfo.Current.IVersion == IISVersion.IIS7) ? "IIS_IUSRS" : "IIS_WPG";
            SiteInfo.Current.UserName_iusr = "bbsMax_iusr_" + pwd.Substring(0, 8);
            pwd = Guid.NewGuid().ToString();
            SiteInfo.Current.UserName_iwam = "bbsMax_iwam_" + pwd.Substring(0, 8);
            pwd = Guid.NewGuid().ToString();
            SiteInfo.Current.PassWord_iusr = "gd5LgDfg_#" + pwd.Substring(0, 8);
            pwd = Guid.NewGuid().ToString();
            SiteInfo.Current.PassWord_iwam = "thi8usHd_#" + pwd.Substring(0, 8);

            CreateWinUser(SiteInfo.Current.UserName_iusr, SiteInfo.Current.PassWord_iusr, "Guests");//IIS来宾 组为Guests
            CreateWinUser(SiteInfo.Current.UserName_iwam, SiteInfo.Current.PassWord_iwam, IIS_UserGroup);//IIS进程启动帐号 组为IIS_WPG IIS_IUSRS
        }
        /// <summary>
        /// 创建系统用户 当前帐号权限如果不够 会创建失败！
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        ///<param name="group">隶属于用户组</param>
        private void CreateWinUser(string userName, string pwd, string group)
        {
            DirectoryEntry dent = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
            DirectoryEntry newUserName;
            DirectoryEntry grp;
            try
            {
                newUserName = dent.Children.Find(userName, "user");
            }
            catch
            {
                newUserName = null;
            }
            try
            {
                grp = dent.Children.Find(group);
            }
            catch
            {
                throw new Exception(string.Format("用户组 {0} 不存在,请检查", group));
            }
            if (newUserName == null)
            {
                try
                {
                    newUserName = dent.Children.Add(userName, "user");
                    newUserName.Properties["UserFlags"].Value = 66049;//启用:512,禁用:514, 密码永不过期:66048
                    newUserName.Invoke("SetPassword", new object[] { pwd });
                    newUserName.Invoke("Put", new object[] { "Description", "IIS帐户" });
                    newUserName.CommitChanges();
                    grp.Invoke("Add", new object[] { newUserName.Path.ToString() });
                }
                catch (Exception ex)
                {
                    throw new Exception("创建系统帐户失败" + ex.Message);
                }
                finally
                {
                    dent.Close();
                    newUserName.Close();
                }
            }
        }
        /// <summary>
        /// 设置网站文件夹权限 1.要有相应权限...2.原来文件夹如果很大 会卡住。。
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="userName">权限用户:NETWORK SERVICE IUSR</param>
        /// <param name="purview">权限</param>
        private void ChangePathPurview(string path, string userName, string purview)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
                dirInfo.Create();
            if ((dirInfo.Attributes & FileAttributes.ReadOnly) != 0)
            {
                dirInfo.Attributes = FileAttributes.Normal;
            }

            //取得访问控制列表
            DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
            switch (purview)
            {
                case "FullControl":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));//InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, 
                    break;
                case "ReadOnly":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.Read, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    break;
                case "Write":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.Write, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    break;
                case "Modify":
                    dirSecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.Modify, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    break;
            }
            try
            {
                dirInfo.SetAccessControl(dirSecurity);
            }
            catch (Exception ex)
            {
                SetupManager.CreateLog("设置网站文件夹权限出错：" + ex.ToString());
            }
        }
        /// <summary>
        /// 设置Temp文件夹权限 用户：IIS_WPG 权限....
        /// </summary>
        private void ChangeTempPurview()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System).ToUpper().Replace("SYSTEM32", "") + @"Temp";
            if ((int)SiteInfo.Current.IVersion >= (int)IISVersion.IIS6)
            {
                ChangePathPurview(path, SiteInfo.Current.UserName_iwam, "FullControl");
            }
            ChangePathPurview(path, SiteInfo.Current.UserName_iusr, "FullControl");
        }
    }
}