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
    class ShareAlbumProvider : IShareProvider
    {
        public ShareContent GetShareContent(int targetID, out int userID, out bool isCanShare)
        {
            userID = 0;
            isCanShare = true;

            Album album = AlbumBO.Instance.GetAlbum(targetID);
            
            if (album == null)
            {
                return null;
            }
            
            isCanShare = album.PrivacyType == PrivacyType.AllVisible;
            
            if (!isCanShare)
            {
                userID = 0;
                return null;
            }
            
            userID = album.UserID;
            
            User author = UserBO.Instance.GetUser(userID);
            StringBuffer shareContent = new StringBuffer();

            string spaceUrl = UrlHelper.GetSpaceUrlTag(album.UserID);
            string albumUrl = UrlHelper.GetAblumUrlTag(targetID);

            shareContent += "<a target=\"_blank\" href=\"";
            shareContent += albumUrl;
            shareContent += "\" title=\"";
            shareContent += album.Name;
            shareContent += "\"><img class=\"summaryimg image\" src=\"";
            shareContent += album.CoverSrc;
            shareContent += "\" alt=\"";
            shareContent += album.Name;
            shareContent += "\" onload=\"imageScale(this, 100, 100)\" onerror=\"this.style.display = 'none';\" /></a>";

            shareContent += "<div class=\"detail\">";
            shareContent += "<b><a target=\"_blank\" href=\"";
            shareContent += albumUrl;
            shareContent += "\" title=\"";
            shareContent += album.Name;
            shareContent += "\">";
            shareContent += album.Name;
            shareContent += "</a></b><br />";
            shareContent += "<a target=\"_blank\" href=\"";
            shareContent += spaceUrl;
            shareContent += "\" title=\"";
            shareContent += author.Name;
            shareContent += "\">";
            shareContent += author.Name;
            shareContent += "</a><br />";
            shareContent += "</div>";


            ShareContent content = new ShareContent();
            content.Catagory = ShareType.Album;
            content.Content = shareContent.ToString();
            content.Title = album.Name;
            content.URL = albumUrl;

            return content;
        }

        public ShareType ShareCatagory
        {
            get { return ShareType.Album; }
        }
    }
}