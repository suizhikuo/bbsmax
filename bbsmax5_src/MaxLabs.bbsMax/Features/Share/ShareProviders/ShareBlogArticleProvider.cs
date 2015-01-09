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
    class ShareBlogArticleProvider : IShareProvider
    {
        public ShareContent GetShareContent(int targetID, out int userID, out bool isCanShare)
        {
            userID = 0;
            isCanShare = false;

            BlogArticle article = BlogBO.Instance.GetBlogArticle(targetID);

            if (article == null)
                return null;

            isCanShare = article.PrivacyType == PrivacyType.AllVisible;

            if (!isCanShare)
            {
                userID = 0;
                return null;
            }

            userID = article.UserID;

            User author = UserBO.Instance.GetUser(userID);

            StringBuffer shareContent = new StringBuffer();

            string spaceUrl = UrlHelper.GetSpaceUrlTag(author.UserID);
            string articleUrl = UrlHelper.GetBlogArticleUrlTag(article.ArticleID); //"$url(" + "space/" + article.UserID + "/blog/article-" + targetID + ")";

            shareContent += "<div class=\"detail\">";
            shareContent += "<b><a target=\"_blank\" href=\"";
            shareContent += articleUrl;
            shareContent += "\" title=\"";
            shareContent += article.Subject;
            shareContent += "\">";
            shareContent += article.Subject;
            shareContent += "</a></b><br />";
            shareContent += "<a target=\"_blank\" href=\"";
            shareContent += spaceUrl;
            shareContent += "\" title=\"";
            shareContent += author.Username;
            shareContent += "\">";
            shareContent += author.Username;
            shareContent += "</a><br />";
            shareContent += StringUtil.CutString(StringUtil.ClearAngleBracket(article.Content), Consts.Share_ReviewContentLength);
            shareContent += "</div>";

            if (string.IsNullOrEmpty(article.Thumb) == false)
            {
                shareContent += "<a target=\"_blank\" href=\"" + articleUrl;
                shareContent += "\" title=\"";
                shareContent += article.Subject;
                shareContent += "\"><img class=\"summaryimg image\" src=\"";
                shareContent += article.Thumb;
                shareContent += "\" alt=\"";
                shareContent += article.Subject;
                shareContent += "\" onload=\"imageScale(this, 100, 100)\" onerror=\"this.style.display = 'none';\"  /></a>";
            }


            ShareContent content = new ShareContent();
            content.Catagory = ShareType.Blog;
            content.Content = shareContent.ToString();
            content.Title = article.Subject;
            content.URL = articleUrl;

            return content;
        }

        public ShareType ShareCatagory
        {
            get { return ShareType.Blog; }
        }
    }
}