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


using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
	/// <summary>
	/// 注册限制设置
	/// </summary>
	public class RegisterLimitSettings : SettingBase
	{
		public RegisterLimitSettings()
		{
			UserNameLengthScope = new Int32Scope(3,12);
            AllowUsernames = new UsernameCharRegulation("Chinese,English,Number,Blank,OtherChar");
			TimeSpanForContinuousRegister = 0;
			RegisterIPLimitMode = LimitMode.Free;
            RegisterIPLimitList = new IPMatchRegulation();
            UserNameForbiddenWords = new UsernameKeywordRegulation();
            RegisterEmailLimitMode = LimitMode.Free;
            RegisterEmailLimitList = new TextRegulation(string.Empty);
		}

		/// <summary>
		/// 用户名长度范围
		/// </summary>
		[SettingItem]
		public Int32Scope UserNameLengthScope { get; set; }

		/// <summary>
		/// 单个IP连续注册的时间间隔(分)
		/// </summary>
		[SettingItem]
		public int TimeSpanForContinuousRegister { get; set; }

		/// <summary>
		/// 注册IP限制模式
		/// </summary>
		[SettingItem]
		public LimitMode RegisterIPLimitMode { get; set; }

		/// <summary>
		/// 受限IP匹配规则
		/// </summary>
		[SettingItem]
        public IPMatchRegulation RegisterIPLimitList { get; set; }

        /// <summary>
        /// 禁止关键字
        /// </summary>
        [SettingItem]
        public UsernameKeywordRegulation UserNameForbiddenWords { get; set; }

        /// <summary>
        /// 注册Email限制模式
        /// </summary>
        [SettingItem]
        public LimitMode RegisterEmailLimitMode { get; set; }

        /// <summary>
        /// 受限Email匹配规则
        /// </summary>
        [SettingItem]
        public TextRegulation RegisterEmailLimitList { get; set; }

        /// <summary>
        /// 用户名允许的字符类型
        /// </summary>
        [SettingItem]
        public UsernameCharRegulation AllowUsernames { get; set; }
	}
}