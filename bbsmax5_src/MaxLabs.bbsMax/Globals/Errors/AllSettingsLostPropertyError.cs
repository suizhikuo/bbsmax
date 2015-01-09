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


using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Errors
{
	public class AllSettingsLostPropertyError : ErrorInfo
	{
        public AllSettingsLostPropertyError(Type settingType)
		{
			m_SettingType = settingType;
		}

		private Type m_SettingType;

		public Type SettingType
		{
			get { return m_SettingType; }
		}

		public override string Message
		{
			get { return string.Format("{0}设置类型在AllSettings类中没有与其对应的{1}属性", SettingType.FullName, SettingType.Name); }
		}
	}
}