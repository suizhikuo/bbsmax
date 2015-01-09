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
using System.Web;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class AppHandlerManager
    {
        private static Dictionary<string, IAppHandler> s_Handlers = new Dictionary<string, IAppHandler>(StringComparer.OrdinalIgnoreCase);

        public static void RegisterAppHandler(IAppHandler handler)
        {
            if (s_Handlers.ContainsKey(handler.Name))
                s_Handlers[handler.Name] = handler;
            else
                s_Handlers.Add(handler.Name, handler);
        }

        public static void ExecuteHandler(string name, HttpContext context)
        {
            IAppHandler handler;

            if (s_Handlers.TryGetValue(name, out handler))
                handler.CreateInstance().ProcessRequest(context);
        }
    }
}