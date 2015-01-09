//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Diagnostics;
using System.Web;
using System.Web.Hosting;

using MaxLabs.WebEngine;
using MaxLabs.WebEngine.Template;
using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.Jobs;
using MaxLabs.bbsMax.Common;
using System.Reflection;
using MaxLabs.bbsMax.Logs;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web
{
    public class _Global : System.Web.HttpApplication
	{
        private static bool inited = false;
        private static object initLocker = new object();

        public _Global()
        {
            if (Globals.UseDynamicCompress)
            {
                ReleaseRequestState += new EventHandler(CompressDynamicContent);
                PreSendRequestHeaders += new EventHandler(CompressDynamicContent);
                ReleaseRequestState += new EventHandler(ProcessJsContent);
                PreSendRequestHeaders += new EventHandler(ProcessJsContent);
            }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (inited == false)
            {
                #region 初始化

                lock (initLocker)
                {
                    if (inited == false)
                    {
                        LogHelper.CreateDebugLog("bbsmax 开始初始化");

                        Config.Current = new WebEngineConfig();
                        TemplateManager.Init(false); //TODO:重复执行会出错

                        //初始化系统并载入设置
                        Globals.Init();
                        Booter.Init();  //TODO:重复执行会出错

                        //初始化插件
                        PluginManager.Init();
                        
#if !Passport
                        //将最后一次备份的在线信息恢复至内存
                        OnlineUserPool.Instance.Restore();
#endif
           
                        //初始化路由
                        BbsRouter.Init();

                        inited = true;
                    }
                }

                #endregion
            }

            if (Request.RequestType == "POST")
            {
                #region 处理大文件上传

                if (StringUtil.StartsWithIgnoreCase(Request.RawUrl, Globals.AppRoot + "/default.aspx?uploadtempfile.aspx"))
                {
                    string query = Request.RawUrl.Substring(Globals.AppRoot.Length + 33);

                    new MaxLabs.bbsMax.AppHandlers.UploadTempFileHandler().ProcessRequest(HttpContext.Current, HttpUtility.ParseQueryString(query));

                    Response.End();
                }

                #endregion
            }

            if (RequestUtil.CompressStaticContent(Context))
                return;

            MaxLabs.WebEngine.Context.Init();

            if (BbsRouter.Route())
            {
                Context.Items.Add("need-compress", true);

                JobManager.ExecuteBeforeRequestJobs();

                MaxLabs.bbsMax.Entities.User user = MaxLabs.bbsMax.Entities.User.Current;

                if (user != null && user.UserID > 0)
                {
                    string ip = IPUtil.GetCurrentIP();

                    if (ip != user.LastVisitIP)
                    {
                        LogManager.LogUserIPChanged(new UserIPLog(user.UserID, user.Username, ip,user.LastVisitIP,Request.RawUrl));                      

//                        MaxLabs.bbsMax.Logs.LogManager.LogOperation(new Logs.User_IPChange(user.UserID, user.Username, user.LastVisitIP == null ? string.Empty : user.LastVisitIP, ip));

                        UserBO.Instance.UpdateLastVisitIP(user.UserID, ip);
                    }
                }

                Stopwatch processTimer = new Stopwatch();

                processTimer.Start();

                HttpContext.Current.Items["MaxLabs.bbsMax.ProcessTimer"] = processTimer;
            }
            else
            {

                #region 根据路径来决定是否允许请求。例如某些文件夹只允许请求图片而某些文件夹什么都不允许请求

                string path = Request.Url.AbsolutePath.Substring(Globals.AppRoot.Length).Trim('/', '\\');
                string file = Request.Url.LocalPath;

                //max-templates目录、max-spacestyles目录和max-assets目录禁止可执行文件
                if (StringUtil.StartsWithIgnoreCase(path, "max-templates/") || StringUtil.StartsWithIgnoreCase(path, "max-spacestyles/") || StringUtil.StartsWithIgnoreCase(path, "max-assets/"))
                {
                    if (IsExecuteableFile(file))
                    {
                        Response.Redirect("~/");
                        return;
                    }
                }

                //max-temp和UserFiles目录只允许访问图片
                else if (StringUtil.StartsWithIgnoreCase(path, "max-temp/") || StringUtil.StartsWithIgnoreCase(path, "UserFiles/"))
                {
                    if (IsImageFile(file) == false)
                    {
                        Response.Redirect("~/");
                        return;
                    }
                }

                else if (StringUtil.EndsWithIgnoreCase(file, ".aspx"))
                {
                    Context.Items.Add("need-compress", true);
                }

                #endregion

            }
        }

        private bool IsExecuteableFile(string path)
        {
            return 
                StringUtil.EndsWithIgnoreCase(path, ".aspx") ||
                StringUtil.EndsWithIgnoreCase(path, ".ashx") ||
                StringUtil.EndsWithIgnoreCase(path, ".asmx");
        }

        private bool IsImageFile(string path)
        {
            return
                    StringUtil.EndsWithIgnoreCase(path, ".gif") ||
                    StringUtil.EndsWithIgnoreCase(path, ".jpg") ||
                    StringUtil.EndsWithIgnoreCase(path, ".jpeg") ||
                    StringUtil.EndsWithIgnoreCase(path, ".png") ||
                    StringUtil.EndsWithIgnoreCase(path, ".bmp");
        }

        //[Conditional("DEBUG")]
        protected void Application_End(object sender, EventArgs e)
        {
#if !Passport
            //备份在线列表
            OnlineUserPool.Instance.Backup();
#endif

            #region 记录关机原因

            try
            {
                HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember(
                    "_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null
                );

                if (runtime == null)
                    return;

                string shutDownMessage = (string)runtime.GetType().InvokeMember(
                    "_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null
                );

                string shutDownStack = (string)runtime.GetType().InvokeMember(
                    "_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null
                );

                LogHelper.CreateDebugLog("ASP.NET应用程序关闭。\r\n关闭消息：\r\n" + shutDownMessage + "堆栈跟踪：\r\n" + shutDownStack);
            }
            catch
            {
                LogHelper.CreateDebugLog("ASP.NET应用程序关闭。\r\n关闭原因：\r\n" + HostingEnvironment.ShutdownReason);
            }

            #endregion
        }

        [Conditional("DEBUG")]
		protected void Application_Error(Object sender, EventArgs e)
        {
            if (Globals.DebugVersion)
                return;

            #region 记录错误日志

            Exception lastError = Server.GetLastError();//.GetBaseException();

			if (lastError == null)
				return;

			Exception ex = lastError.GetBaseException();

			if (ex is HttpException)
			{
                if (((HttpException)ex).GetHttpCode() == 404)
                {
                    //if (Request.CurrentExecutionFilePath.IndexOf("checkurlmode") > -1)
                    if (Request.QueryString["checkurlmode"] != null)
                    {
                        Response.Write("success");
                        Server.ClearError();
                    }
                    return;
                }

                else if (ex.TargetSite.Name == "RaiseCommunicationError")
                {
                    return;
                }

                else if (ex.Message.IndexOf("Max.WebUI.Install") != -1)
                {
                    ShowErrorHtml(false, "安装程序不完整,可能您之前已经成功安装了bbsmax。如果您希望修复安装，请重新上传 Install.aspx、Global.asax、bin\\MaxInstall.dll 文件，然后重新运行本安装向导；否则请删除本文件。", null);
                    return;
                }
			}

			//专门用于处理 Windows Vista 上 IIS 7.0 的一个BUG导致的异常
			else if (ex is NullReferenceException)
			{
				if (((NullReferenceException)ex).TargetSite.DeclaringType.FullName == "System.Web.Hosting.IIS7WorkerRequest")
					return;
			}

            //else if (ex is )

			string errorID = Logs.LogManager.LogException(ex);

			Server.ClearError();

			if (System.Web.HttpContext.Current != null)
			{
                bool isAjax = Request.Form["_AjaxPanelID"] != null || Request.QueryString["_AjaxPanelID"] != null;
				ShowErrorHtml(isAjax, ex.Message, errorID);
            }

            #endregion
        }

        private string GetSafeErrorMessage(string message)
        {
            if (Globals.ApplicationPath.EndsWith("\\"))
                return message.Replace(Globals.ApplicationPath, "~\\");
            else
                return message.Replace(Globals.ApplicationPath, "~");
        }

		private void ShowErrorHtml(bool isAjax, string message, string errorID)
		{
            if (string.IsNullOrEmpty(errorID))
                errorID = "error";

			Response.ClearContent();

			if (isAjax)
				Response.Write("[error]发生了系统错误, 错误信息已经被记录, 请联系管理员<br /><br />错误编号: <span class=\"red\">" + (errorID != "error" ? errorID : "无法得到错误编号") + "</span>");
			else
            {
                #region 系统错误要显示的HTML

                string errorImagePath = Globals.GetVirtualPath(SystemDirecotry.Assets_Images, "error_bg.png");

				Response.Write(
string.Concat(@"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
<title>系统错误 - Powered by bbsMax</title>
<style type=""text/css"">
body{margin:0;padding:0 100px;font:12px ""Segoe UI"",Helvetica,Arial,sans-serif;background:url(", errorImagePath, @") repeat-x 0 -70px;}
h1{padding-left:70px;height:70px;line-height:70px;margin:0 0 0 -70px;font-size:130%;white-space:nowrap;background:url(", errorImagePath, @") no-repeat;}
ul{margin:40px 0;padding:0 0 0 1.2em;font-size:120%;}
li{margin-bottom:0.2em;}
pre{margin:5px 10px 0 0;padding:10px;background:#ffe;white-space:pre;}
code{font-size:90%;font-family:Consolas,""Courier New"",monospace;}
a:link, a:visited{color:#06C;text-decoration:none;}
.red{color:#c00;}
</style>
</head>
<body>
<h1>发生了系统错误, 错误信息已经被记录, 请联系管理员</h1>
<ul>
<li>错误提示: <code class=""red"">", GetSafeErrorMessage(message), @"</code></li>", (errorID == "error" ? "" : string.Concat(@"
<li>错误编号: <code class=""red"">", errorID, @"</code></li>")), @"
</ul>
<p>管理员根据错误编号查找日志文件以得到详细错误信息. 更多技术支持请前往 <a href=""http://www.bbsmax.com/"" target=""_target"">bbsmax技术支持网站</a></p>
</body>
</html>
"));

                #endregion
            }
			Response.End();
		}

        private void CompressDynamicContent(object sender, EventArgs e)
        {
            if (Globals.UseDynamicCompress && !Common.CommonUtil.ContainsInstalledKey(Context) && Context.Items.Contains("need-compress"))
            {
                if (string.IsNullOrEmpty(Context.Request.Form["_ajaxpanelid"]) && string.IsNullOrEmpty(Context.Request.QueryString["_ajaxpanelid"]))
                {
                    Common.CommonUtil.SetInstalledKey(Context);

                    string realPath = Request.Path.Remove(0, Request.ApplicationPath.Length + 1);

                    Response.Cache.VaryByHeaders["Accept-Encoding"] = true;

                    CompressingType compressingType = RequestUtil.GetCompressingType(Context);

                    if (compressingType == CompressingType.GZip)
                        Response.Filter = new GZipFilter(Response.Filter);
                    else if (compressingType == CompressingType.Deflate)
                        Response.Filter = new DeflateFilter(Response.Filter);
                }
            }
        }

        private void ProcessJsContent(object sender, EventArgs e)
        {
            if (Request.CurrentExecutionFilePath.IndexOf("/max-js/") > -1)
            {
                Response.Filter = new JsFilter(Response.Filter);
            }
        }
    }
}