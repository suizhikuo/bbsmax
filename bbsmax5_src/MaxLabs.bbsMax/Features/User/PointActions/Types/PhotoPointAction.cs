//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

//using System;
//using System.Collections.Generic;
//using System.Text;

//using MaxLabs.bbsMax.Entities;
//using MaxLabs.bbsMax.Rescourses;
//using MaxLabs.bbsMax.Enums;

//namespace MaxLabs.bbsMax.PointActions
//{
//    public class PhotoPointAction : PointActionBase<PhotoPointAction, PhotoPointType, NullEnum>
//    {

//        public override string Name
//        {
//            get { return Lang.AlbumPointTypeName; }
//        }

//        //public override Dictionary<PhotoPointType, string> Actions
//        //{
//        //    get
//        //    {
//        //        Dictionary<PhotoPointType, string> actions = new Dictionary<PhotoPointType, string>();
//        //        actions.Add(PhotoPointType.AddPhoto, Lang.PhotoPointType_AddPhoto);
//        //        actions.Add(PhotoPointType.PhotoWasDeletedBySelf, Lang.PhotoPointType_PhotoWasDeletedBySelf);
//        //        actions.Add(PhotoPointType.PhotoWasDeletedByAdmin, Lang.PhotoPointType_PhotoWasDeletedByAdmin);
//        //        actions.Add(PhotoPointType.PhotoWasCommented, Lang.PhotoPointType_PhotoWasCommented);
//        //        return actions;
//        //    }
//        //}

//    }

//    public enum PhotoPointType
//    {
//        [PointActionItem(Lang.PhotoPointType_AddPhoto, false, false)]
//        AddPhoto,

//        PhotoWasDeletedBySelf,

//        PhotoWasDeletedByAdmin,

//        PhotoWasCommented
//    }

//}