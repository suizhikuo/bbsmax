//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax
{
    /// <summary>
    /// 实现分享的接口IShareProvider，以实现“分享用户”功能
    /// </summary>
    class ShareUserProvider : IShareProvider
    {

        public ShareType ShareCatagory
        {
            get { return ShareType.User; }
        }

        public ShareContent GetShareContent(int targetID, out int userID, out bool canShare)
        {
            canShare = true;
            userID = targetID;
            User user = UserBO.Instance.GetUser(targetID);

            string spaceUrl = UrlHelper.GetSpaceUrlTag(user.UserID);
            string content = user.AvatarLink + "<div class=\"detail\">" + user.PopupNameLink + "</div>";

            ShareContent shareContent = new ShareContent();
            shareContent.Catagory = ShareType.User;
            shareContent.Content = content;
            shareContent.Title = user.Name;
            shareContent.URL = spaceUrl;

            return shareContent;
        }
    }
}