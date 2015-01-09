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
    public class AlbumPointAction : PointActionBase<AlbumPointAction, AlbumPointType, NullEnum>
    {
        public override string Name
        {
            get { return Lang.AlbumPointAction_Name; }
        }

        public override bool Enable
        {
            get
            {
                return AllSettings.Current.AlbumSettings.EnableAlbumFunction;
            }
        }
    }

    public enum AlbumPointType
    {
        [PointActionItem(Lang.AlbumPointType_CreateAlbum, false, true)]
        CreateAlbum,

        [PointActionItem(Lang.AlbumPointType_AlbumWasDeletedBySelf, false, false)]
        AlbumWasDeletedBySelf,

        [PointActionItem(Lang.AlbumPointType_AlbumWasDeletedByAdmin, false, false)]
		AlbumWasDeletedByAdmin,

		[PointActionItem(Lang.AlbumPointType_CreatePhoto, false, true)]
		CreatePhoto,

		[PointActionItem(Lang.AlbumPointType_PhotoWasDeletedBySelf, false, false)]
		PhotoWasDeletedBySelf,

		[PointActionItem(Lang.AlbumPointType_PhotoWasDeletedByAdmin, false, false)]
		PhotoWasDeletedByAdmin,

		[PointActionItem(Lang.AlbumPointType_PhotoWasCommented, false, false)]
		PhotoWasCommented
    }

}