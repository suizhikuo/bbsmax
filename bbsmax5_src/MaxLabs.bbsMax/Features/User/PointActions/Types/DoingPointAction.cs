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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.PointActions
{
	public class DoingPointAction : PointActionBase<DoingPointAction, DoingPointType, NullEnum>
	{
		public override string Name
		{
			get { return Lang.DoingPointTypeName; }
        }

        public override bool Enable
        {
            get
            {
                return AllSettings.Current.DoingSettings.EnableDoingFunction;
            }
        }
	}

	public enum DoingPointType
	{
		[PointActionItem(Lang.DoingPointType_PostDoing, false, true)]
		PostDoing,

		[PointActionItem(Lang.DoingPointType_DoingWasDeletedBySelf, false, false)]
		DoingWasDeletedBySelf,

		[PointActionItem(Lang.DoingPointType_DoingWasDeletedByAdmin, false, false)]
		DoingWasDeletedByAdmin,

		[PointActionItem(Lang.DoingPointType_DoingWasCommented, false, false)]
		DoingWasCommented
	}
}