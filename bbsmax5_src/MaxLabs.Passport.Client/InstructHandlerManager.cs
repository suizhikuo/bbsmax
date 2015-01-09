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

namespace MaxLabs.Passport.Client
{
    public class InstructHandlerManager
    {
        private static Dictionary<int, List<InstructHandlerBase>> s_Processor = new Dictionary<int, List<InstructHandlerBase>>();

        static object locker = new object();
        public static void RegisterHandler(InstructHandlerBase handler)
        {
            List<InstructHandlerBase> handlers;

            lock (locker)
            {
                if (s_Processor.ContainsKey(handler.InstructType))
                {
                    handlers = s_Processor[handler.InstructType];
                }
                else
                {
                    handlers = new List<InstructHandlerBase>();
                    s_Processor.Add(handler.InstructType, handlers);
                }
                handlers.Add(handler);
            }
        }

        public static void ExecuteInstruct(string key, int instructType, int userID, DateTime instructDateTime, string datas)
        {
            if (s_Processor.ContainsKey(instructType))
            {
                List<InstructHandlerBase> processors = s_Processor[instructType];

                foreach (InstructHandlerBase processor in processors)
                {
                    //   if(processor.Key==key)
                    processor.Execute(userID, instructDateTime, datas);
                }
            }
        }
    }
}