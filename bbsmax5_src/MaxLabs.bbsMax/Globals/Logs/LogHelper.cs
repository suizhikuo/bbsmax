//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using System.IO;
using System.Configuration;
using System.Diagnostics;
//using MaxLabs.bbsMax.Settings;
//using zzbird.Common.Permissions;
//using zzbird.Common.Users;

namespace MaxLabs.bbsMax.Common
{
    public class LogHelper
    {
        /// <summary>
        /// 创建错误日志（有可能抛出日志文件操作异常）
        /// </summary>
        /// <param name="ex">发生的异常</param>
        /// <returns>返回错误ID</returns>
        public static string CreateErrorLog(Exception ex)
        {
            return CreateErrorLog(ex, string.Empty);
        }

        public static string CreateErrorLog(Exception ex, string errorMessage)
        {
            string logPath = string.Format(
                "error-{0}-{1}-{2}-{3}.log",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                Globals.FileNamePart
            );

            return CreateLog(ex, errorMessage, logPath);
        }

        public static string CreateDebugLog(string message)
        {
            string logPath = string.Format(
                "debug-{0}-{1}-{2}-{3}.log",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                Globals.FileNamePart
            );

            return CreateLog(null, message, logPath);
        }

        public static string CreateRecodeTodayPostsLog(int forumID, string message)
        {
            string logPath = string.Format(
                "todaypost-{4}-{0}-{1}-{2}-{3}.log",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                Globals.FileNamePart,
                forumID
            );

            return CreateLog(null, message, logPath);
        }

        public static string CreatePostFloorLog(string message)
        {
            string logPath = string.Format(
                "PostFloor-{0}-{1}-{2}-{3}.log",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                Globals.FileNamePart
            );

            return CreateLog(null, message, logPath);
        }

        public static string CreateErrorLog2(string fileNameStart, string message)
        {
            string logPath = string.Format(
                fileNameStart + "-{0}-{1}-{2}-{3}.log",
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                Globals.FileNamePart
            );

            return CreateLog(null, message, logPath);
        }

        /// <summary>
        /// 创建错误日志（有可能抛出日志文件操作异常）
        /// </summary>
        /// <param name="ex">发生的异常</param>
        /// <param name="errorMessage">自定义错误消息</param>
        /// <returns>返回错误ID</returns>
        public static string CreateLog(Exception ex, string errorMessage, string logPath)
        {
            string errorPath = Globals.GetPath(SystemDirecotry.Error); //UrlUtil.JoinUrl(Globals.ApplicationPath, "error");
            
            string errorID = Guid.NewGuid().ToString("N").ToUpper();
            
            logPath = UrlUtil.JoinUrl(errorPath, logPath);

            if (!Directory.Exists(errorPath))
            {
                try
                {
                    Directory.CreateDirectory(errorPath);
                }
                catch { return "error"; }
            }

            StreamWriter streamWriter = null;

            try
            {
                streamWriter = new StreamWriter(logPath, true, Encoding.UTF8);
            }
            catch
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                return "error";
            }


            streamWriter.WriteLine("ErrorID:" + errorID);
            if (!string.IsNullOrEmpty(errorMessage))
                streamWriter.WriteLine("Message:" + errorMessage);
            else if (ex != null)
                streamWriter.WriteLine("Message:" + ex.Message);
            else
                return errorID;

            try
            {
                if (System.Web.HttpContext.Current != null)
                {
                    System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;

                    string userVar = request.RawUrl;
                    //if (ex != null)
                    //{
                        streamWriter.WriteLine("Path:" + userVar);
                    //}

                    if (request.UrlReferrer != null)
                    {
                        userVar = request.UrlReferrer.ToString();
                        streamWriter.WriteLine("Referrer:" + userVar);
                    }

                    userVar = System.Web.HttpContext.Current.Request.UserAgent;
                    streamWriter.WriteLine("UserAgent:" + userVar);

                    userVar = request.UserHostAddress;
                    streamWriter.WriteLine("Address:" + userVar);

                    int userID = UserBO.Instance.GetCurrentUserID();
                    if (userID > 0)
                    {
                        streamWriter.WriteLine("UserID:" + userID);
                        streamWriter.WriteLine("Username:" + UserBO.Instance.GetUser(userID).Username);
                    }

                    if (ex != null)
                    {
                        //foreach (System.Web.HttpCookie cookie in request.Cookies)
                        if (request.Cookies.Count > 0)
                            streamWriter.WriteLine("Cookie:");
                        for (int i = 0; i < request.Cookies.Count; i++)
                        {
                            System.Web.HttpCookie cookie = request.Cookies[i];
                            streamWriter.WriteLine("    name:" + cookie.Name + "    value:" + cookie.Value);
                        }

                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < System.Web.HttpContext.Current.Request.Form.Count; i++)
                        {
                            if (i > 0)
                                sb.Append("&");
                            sb.Append(request.Form.GetKey(i));
                            sb.Append("=");
                            sb.Append(request.Form[i].ToString());

                        }
                        streamWriter.WriteLine("PostData:" + sb.ToString());
                    }
                }
            }
            catch
            {
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.WriteLine("Time:" + DateTime.Now.ToString());
                        streamWriter.WriteLine("----------------------------------------------------------------------------------");
                    }
                    catch { }
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
                return "error";
            }

            if (ex != null)
            {
                streamWriter.WriteLine("Type:" + ex.GetType());
                streamWriter.WriteLine("Source:" + ex.Source);
                streamWriter.WriteLine("StackTrace:");
                streamWriter.WriteLine(ex.StackTrace);
                streamWriter.WriteLine("Method:" + ex.TargetSite.Name);
                streamWriter.WriteLine("Class:" + ex.TargetSite.DeclaringType.FullName);
            }
            streamWriter.WriteLine("Time:" + DateTime.Now.ToString());
            streamWriter.WriteLine("----------------------------------------------------------------------------------");
            try
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            catch { return "error"; }

            return errorID;
        }

        static string file = "error-language-information-" + Globals.FileNamePart + ".log";

        [Conditional("DEBUG")]
        public static void SetText(string status)
        {
            LogHelper.CreateLog(null, status, file);
        }

        /// <summary>
        /// 清理超过一个月的日志文件
        /// </summary>
        public static void ClearExpiresLogs()
        {
            DirectoryInfo dir = new DirectoryInfo(Globals.GetPath(SystemDirecotry.Error));

            if (dir.Exists)
            {
                FileInfo[] fileInfos = dir.GetFiles("*", SearchOption.AllDirectories);

                DateTime time = DateTime.UtcNow.AddDays(-30);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (fileInfo.CreationTimeUtc < time && fileInfo.LastWriteTimeUtc < time)
                    {
                        try
                        {
                            fileInfo.Delete();
                        }
                        catch { }
                    }
                }
            }
        }
    }
}