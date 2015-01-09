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

using MaxLabs.bbsMax.Providers;

namespace MaxLabs.bbsMax.DataAccess
{
	public class DaoFactory<T> where T : DaoBase<T>
	{
		private static T s_DaoInstance = null;

		public static T Create()
		{
			if (s_DaoInstance != null)
				return s_DaoInstance;

			s_DaoInstance = ProviderManager.Get<IDataProvider>().Create<T>();

			return s_DaoInstance;
		}
	}
}