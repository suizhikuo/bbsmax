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

using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 关于设置表的数据库操作
	/// </summary>
	public abstract class SettingDao : DaoBase<SettingDao>
	{
		/// <summary>
		/// 保存设置
		/// </summary>
		/// <param name="settings">设置对象</param>
		public abstract void SaveSettings(SettingBase settings);

		/// <summary>
		/// 加载设置表中的设置到一个AllSettings对象中，并返回
		/// </summary>
		/// <returns>包含当前系统中所有设置的AllSettings对象</returns>
		public abstract AllSettings LoadAllSettings();
	}
}