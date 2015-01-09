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



namespace MaxLabs.bbsMax.Entities
{
	public abstract class ExtendedFieldType
	{
		public string TypeName
		{
			get { return this.GetType().Name; }
		}

		/// <summary>
		/// 显示名称如：Email输入框、电话输入框等
		/// </summary>
		public abstract string DisplayName
		{
			get;
		}

		/// <summary>
		/// 是否在后台设置时可用
		/// </summary>
		public virtual bool Settingable 
		{ 
			get { return false; } 
		}

		/// <summary>
		/// 后台设置用的控件包含路径
		/// </summary>
		public virtual string BackendControlSrc
		{ 
			get { return string.Empty; } 
		}

		/// <summary>
		/// 从后台表单提交中读取对扩展字段的设置信息
		/// </summary>
		/// <returns></returns>
		public virtual StringTable LoadSettingsFromRequest()
		{
			return new StringTable();
		}

		/// <summary>
		/// 前台控件的包含路径
		/// </summary>
		public abstract string FrontendControlSrc { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fieldInfo"></param>
		/// <returns></returns>
		public abstract string LoadValueFromRequest(ExtendedField fieldInfo);

		/// <summary>
		/// 获取搜索该类型的信息时是否使用完整匹配的方式，完整匹配性能好于不完全匹配
		/// </summary>
		public virtual bool NeedExactMatch { get { return true; } }

		/// <summary>
		/// 获取前台显示信息时所需的HTML
		/// </summary>
		/// <param name="filedInfo"></param>
		/// <returns></returns>
		public abstract string GetHtmlForDisplay(string value);
	}
}