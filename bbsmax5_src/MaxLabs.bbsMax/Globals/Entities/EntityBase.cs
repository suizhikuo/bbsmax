//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Entities
{
	/// <summary>
	/// IMaxObject 的摘要说明。
	/// </summary>
    [Obsolete("请完全避免使用，这是3.0中的类")]
	public class EntityBase
	{


		protected int id = 0;
		public virtual int ID
		{
			get { return id; }
			set { id = value; }
		}

		protected string key = null;
		public virtual string Key
		{
			get
			{
				if (key == null)
					return id.ToString();
				return key;
			}
			set { key = value; }
		}

		protected string name = null;
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}

		//private DateTime lastUsed = DateTimeUtil.Now;
		//public DateTime LastUsed
		//{
		//    get { return lastUsed; }
		//    set { lastUsed = value; }
		//}

		public override string ToString()
		{
			return this.name;
		}
	}
}