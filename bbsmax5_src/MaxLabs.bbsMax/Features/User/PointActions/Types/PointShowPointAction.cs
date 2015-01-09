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

//using MaxLabs.bbsMax.Enums;
//using MaxLabs.bbsMax.Entities;

//namespace MaxLabs.bbsMax.PointActions
//{

//    public class PointShowPointAction : PointActionBase<PointShowPointAction, NullEnum, PointShowType>
//    {
//        public override string Name
//        {
//            get { return "竞价上榜"; }
//        }

//        //public override Dictionary<PointShowType, string> NeedValueActions
//        //{
//        //    get
//        //    {
//        //        Dictionary<PointShowType, string> actions = new Dictionary<PointShowType, string>();
//        //        actions.Add(PointShowType.AddPointShow, "自己竞价上榜");
//        //        actions.Add(PointShowType.AddFriendPointShow, "帮好友竞价上榜");
//        //        return actions;
//        //    }
//        //}
//    }

//    public enum PointShowType
//    {
//        /// <summary>
//        /// 自己竞价上榜
//        /// </summary>
//        [PointActionItem("自己竞价上榜", false, true, true, true)]
//        AddPointShow,

//        /// <summary>
//        /// 帮好友竞价上榜
//        /// </summary>
//        [PointActionItem("帮好友竞价上榜", false, true, true, true)]
//        AddFriendPointShow,
//    }
//}