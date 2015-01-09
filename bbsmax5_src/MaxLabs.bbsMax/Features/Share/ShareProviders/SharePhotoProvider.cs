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
    public class SharePhotoProvider : IShareProvider
    {
        public ShareType ShareCatagory
        {
            get { return ShareType.Picture; }
        }

        public ShareContent GetShareContent(int targetID, out int userID, out bool isCanShare)
        {
            Photo photo = AlbumBO.Instance.GetPhoto(targetID);
            isCanShare = true;

            userID = photo.UserID;
            User author = UserBO.Instance.GetUser(userID);
            StringBuffer shareContent = new StringBuffer();

            string spaceUrl = UrlHelper.GetSpaceUrlTag(author.UserID);
            string photoUrl = UrlHelper.GetPhotoUrlTag(photo.PhotoID);

            shareContent += "<a target=\"_blank\" href=\"";
            shareContent += photoUrl;
            shareContent += "\" title=\"";
            shareContent += photo.Name;
            shareContent += "\"><img class=\"summaryimg image\" src=\"";
            shareContent += photo.ThumbSrc;
            shareContent += "\" alt=\"";
            shareContent += photo.Name;
            shareContent += "\" onload=\"imageScale(this, 200, 200)\" onerror=\"this.style.display = 'none';\" /></a>";

            shareContent += "<div class=\"detail\">";
            shareContent += "<b><a target=\"_blank\" href=\"";
            shareContent += photoUrl;
            shareContent += "\" title=\"";
            shareContent += photo.Name;
            shareContent += "\">";
            shareContent += photo.Name;
            shareContent += "</a></b><br />";
            shareContent += "<a target=\"_blank\" href=\"";
            shareContent += spaceUrl;
            shareContent += "\" title=\"";
            shareContent += author.Name;
            shareContent += "\">";
            shareContent += author.Name;
            shareContent += "</a><br />";
            shareContent += photo.Description;
            shareContent += "</div>";


            ShareContent content = new ShareContent();
            content.Catagory = ShareType.Picture;
            content.Content = shareContent.ToString();
            content.Title = photo.Name;
            content.URL = photoUrl;

            return content;
        }
    }
}