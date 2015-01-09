//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Xml.Serialization;
using MaxLabs.bbsMax.Common;
using System.Text;
using System.Net;
using System.Web;

namespace MaxLabs.bbsMax.PassportServerInterface
{

    /// <summary>
    /// 服务器端API请求监控部分
    /// </summary>
    public partial class Service
    {
        private static readonly string Item_Key = "PassportInterfaceCall";

        /// <summary>
        /// 当前HTTP请求过程调用Web Service次数计数
        /// </summary>
        public static int ServiceCallCount
        {
            get
            {
                if(HttpContext.Current==null) return 0;
                if(HttpContext.Current.Items[Item_Key]==null) return 0;
                return (int)HttpContext.Current.Items[Item_Key];
            }
        }

        protected override WebResponse GetWebResponse(System.Net.WebRequest request)
        {
            StringBuilder sb = new StringBuilder();
            string action = request.Headers["SOAPAction"].Trim('"');
            sb.AppendLine(string.Concat("Method=", action.Substring(action.LastIndexOf("/") + 1)));
            Stopwatch s = new Stopwatch();


            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[Item_Key] == null)
                {
                    HttpContext.Current.Items[Item_Key] = 1;
                }
                else
                {
                    HttpContext.Current.Items[Item_Key] = (int)HttpContext.Current.Items[Item_Key] + 1;
                }
            }

            s.Start();
            WebResponse response =  base.GetWebResponse(request);
            s.Stop();
#if DEBUG
            if (s.ElapsedMilliseconds > 300)
            {
                sb.AppendLine(string.Concat("time=", s.Elapsed.TotalSeconds));
                LogHelper.CreateLog(null, sb.ToString(), string.Format("RemoteCall_{0}.txt", DateTime.Now.ToString("yyyyMMddHH")));
            }
#endif
            return response;
        }
    }
}