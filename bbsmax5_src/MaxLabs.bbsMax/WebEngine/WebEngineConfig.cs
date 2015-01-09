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

namespace MaxLabs.bbsMax
{
	public sealed class WebEngineConfig : Config
	{
		protected override string[] GetTemplateImports()
		{
            return new string[] { "MaxLabs.bbsMax", "MaxLabs.bbsMax.Enums", "MaxLabs.bbsMax.Settings", "MaxLabs.bbsMax.Filters" };
		}

		protected override string[] GetTemplatePackages()
		{
			return new string[] { "MaxLabs.bbsMax" };
		}

		protected override string GetTemplateOutputPath()
		{
			return "~/max-temp/parsed-template/";
		}

        //protected override string[] GetTemplateDirectories()
        //{
        //    return new string[] { "~/max-templates/default/", "~/max-plugins/", "~/max-dialogs/", "~/max-admin/" };
        //}
	}
}