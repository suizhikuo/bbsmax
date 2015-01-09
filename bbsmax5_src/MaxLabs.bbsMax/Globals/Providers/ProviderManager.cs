//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Providers
{
	public class ProviderManager
	{
		public static void Set<T>(T provider)
		{
			if (ProviderCache<T>.ProviderInstance == null)
				ProviderCache<T>.ProviderInstance = provider;
		}

		public static T Get<T>()
		{
			return ProviderCache<T>.ProviderInstance;
		}

        public static void Add<T>(T provider)
        {
            if (!ProviderCache<T>.ProviderInstances.Contains(provider))
            {
                ProviderCache<T>.ProviderInstances.Add(provider);
            }
        }

        public static List<T> GetManay<T>()
        {
            return ProviderCache<T>.ProviderInstances;
        }

		private class ProviderCache<T>
		{
			public static T ProviderInstance;
            public static List<T> ProviderInstances = new List<T>();
		}
	}
}