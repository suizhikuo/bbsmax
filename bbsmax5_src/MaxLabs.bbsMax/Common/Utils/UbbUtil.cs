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
using System.Reflection;

using MaxLabs.bbsMax.Ubb;

namespace MaxLabs.bbsMax
{
    public static class UbbUtil
    {
        private static Dictionary<RuntimeTypeHandle, UbbParser> s_UbbParsers = new Dictionary<RuntimeTypeHandle, UbbParser>();
        private static object locker = new object();

        /// <summary>
        /// 解析，
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"> 提供三种标签集合， 全部支持， 大部分支持， 最少 </param>
        /// <returns></returns>
        public static string ConvertUbbToHtml(string text, ParserType type)
        {
            UbbParser parser;
            parser = GetUbbParser(type);
            return parser.UbbToHtml(text);
        }

        public static string ConvertUbbToHtml<T>(string text) where T : UbbParser, new()
        {
            UbbParser parser = new T();

            return parser.UbbToHtml(text);
        }

        /// <summary>
        /// 自定义要解析的标签
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string ConvertUbbToHtml(string text, string[] tags)
        {
            List<UbbTagHandler> handlers = CreateHandlers(tags);
            UbbParser parser = new UbbParser(handlers);
            return parser.UbbToHtml(text);
        }

        public static string ConvertHtmlToUbb(string html)
        {
            return GetUbbParser(ParserType.Full).HtmlToUbb(html);
        }

        private static UbbParser fullParser = null;
        private static UbbParser simpleParser = null;
        private static UbbParser normalParser = null;

        private static UbbParser GetUbbParser(ParserType type)
        {
            switch (type)
            {
                case ParserType.Full:
                    if (fullParser == null)
                    {
                        fullParser = new UbbParser(CreateHandlers(
                               "ALIGN", "B", "U", "I", "S", "FONT", "COLOR", "EMAIL", "IMG"
                             , "FLASH", "WMA", "MP3", "WMV", "SIZE", "RM", "QQ", "TABLE", "URL"
                             , "SUB", "SUP", "BGCOLOR", "INDENT", "LIST", "MSN", "QUOTE", "BR"
                             ));
                    }
                    return fullParser;
                case ParserType.Normal:
                    if (normalParser == null)
                    {
                        normalParser = new UbbParser(CreateHandlers("B", "I", "U", "SIZE", "IMG", "COLOR", "S", "BGCOLOR", "URL", "FONT", "EMAIL", "FLASH"));
                    }
                    return normalParser;
                case ParserType.Simple:
                    if (simpleParser == null)
                    {
                        simpleParser = new UbbParser(CreateHandlers("B", "I", "U", "SIZE", "IMG", "COLOR", "S", "BGCOLOR"));
                    }
                    return simpleParser;
            }

            return new UbbParser();
        }

        private static List<UbbTagHandler> CreateHandlers(params string[] tagNames)
        {
            List<UbbTagHandler> handlers = new List<UbbTagHandler>();
            UbbTagHandler tagHandler;
            foreach (string s in tagNames)
            {
                tagHandler = CreateHandler(s);
                if (tagHandler != null)
                    handlers.Add(tagHandler);
            }
            return handlers;
        }

        private static UbbTagHandler CreateHandler(string tagName)
        {
            Type handlerType = Type.GetType("MaxLabs.bbsMax.UBB.Tags." + tagName);
            UbbTagHandler handler = null;

            if (handlerType == null) return null;

            try
            {
                handler = (UbbTagHandler)Activator.CreateInstance(handlerType);
            }
            catch
            {
            }

            return handler;
        }
    }

    public enum ParserType
    {
        /// <summary>
        /// 支持系统所有的UBB
        /// </summary>
        Full,
        /// <summary>
        /// 支持一般的UBB
        /// B,I,U,SIZE,IMG,COLOR,S,BGCOLOR,URL,FONT,EMAIL,FLASH
        /// </summary>
        Normal,

        /// <summary>
        /// 谨支持B,I,U,SIZE,IMG,COLOR,S,BGCOLOR
        /// </summary>
        Simple
    }
}