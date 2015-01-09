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
using System.Collections;

namespace MaxLabs.bbsMax
{
    public class PageCacheUtil
    {

        public static bool CanUsePageCache
        {
            get
            {
                return HttpContext.Current != null;
            }
        }

        public static bool TryGetValue<T>(string key, out T value)
        {
            if (HttpContext.Current == null || !HttpContext.Current.Items.Contains(key))
            {
                value = default(T);
                return false;
            }
            else
            {
                if (HttpContext.Current.Items[key] is T)
                {
                    value = (T)HttpContext.Current.Items[key];
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        public static bool Contains(string key)
        {
            if (HttpContext.Current == null)
                return false;

            return (HttpContext.Current.Items.Contains(key));
        }

        public static void Add(string key, object value)
        {
            Set(key, value);
        }

        public static void Set(string key, object value)
        {
            if (HttpContext.Current == null)
                return;

            HttpContext.Current.Items[key] = value;
        }

        public static void Remove(string key)
        {
            if (HttpContext.Current == null)
                return;

            HttpContext.Current.Items.Remove(key);
        }


        public static void RemoveBySearch(string keyPrefix)
        {
            if (string.IsNullOrEmpty(keyPrefix))
                return;

            List<string> keys = new List<string>();

            foreach (DictionaryEntry elem in HttpContext.Current.Items)
            {
                string key = elem.Key.ToString();

                if (StringUtil.StartsWithIgnoreCase(key, keyPrefix))
                    keys.Add(key);
            }

            foreach (string key in keys)
            {
                try
                {
                    HttpContext.Current.Items.Remove(key);
                }
                catch { }
            }
        }
    }
}