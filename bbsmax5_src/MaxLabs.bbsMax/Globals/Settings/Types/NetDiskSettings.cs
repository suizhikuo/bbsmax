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



namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 网络硬盘设置
	/// </summary>
	public class NetDiskSettings : SettingBase
	{
		public NetDiskSettings()
		{
			EnableNetDisk = true;
			NetDiskDefaultView = NetDiskViews.Thumb;
			AllowUserCustomNetDiskView = true;
		}

		/// <summary>
		/// 是否开启网络硬盘功能
		/// </summary>
		[SettingItem]
		public bool EnableNetDisk { get; set; }

		/// <summary>
		/// 网络硬盘默认视图
		/// </summary>
		[SettingItem]
		public NetDiskViews NetDiskDefaultView { get; set; }

		/// <summary>
		/// 是否允许用户自定义网络硬盘实体
		/// </summary>
		[SettingItem]
		public bool AllowUserCustomNetDiskView { get; set; }
	}

	/// <summary>
	/// 网络硬盘实体类型
	/// </summary>
	public enum NetDiskViews
	{
		/// <summary>
		/// 缩略图模式
		/// </summary>
		Thumb,

		/// <summary>
		/// 列表模式
		/// </summary>
		List
	}
}