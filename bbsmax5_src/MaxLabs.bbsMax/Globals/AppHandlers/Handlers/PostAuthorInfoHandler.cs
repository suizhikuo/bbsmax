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
using System.Web;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class PostAuthorInfoHandler : IAppHandler
    {
        #region IAppHandler 成员

        public IAppHandler CreateInstance()
        {
            return new PostAuthorInfoHandler();
        }

        public string Name
        {
            get { return "authorinfo"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            int userID = StringUtil.TryParse<int>(request.QueryString["userid"] ,0);

            if (userID == 0)
            {
                return;
            }

            PostAuthorExtendInfo PostUserEntendInfo = PostBOV5.Instance.GetPostAuthorExtendInfo(User.Current, userID);

            StringBuilder builder = new StringBuilder();
           
            builder.Append("{")
                .Append("newarticle:[");

            foreach (BlogArticle art in PostUserEntendInfo.NewArticles)
            {
                builder.Append("{subject:'")
                    .Append(StringUtil.ToJavaScriptString(art.Subject))
                    .Append("'")
                    .Append(",url:'")
                    .Append(BbsRouter.GetUrl("app/blog/view", "id=" + art.ArticleID))
                    .Append("'")
                    .Append(",content:'")
                    .Append(StringUtil.ToJavaScriptString(art.SummaryContent))
                    .Append("'")
                    .Append("},");
            }

            if (PostUserEntendInfo.NewArticles.Count > 0)
                builder.Remove(builder.Length - 1, 1);

            builder.Append("],newphotos:[");

            foreach (Photo p in PostUserEntendInfo.NewPhotos)
            {
                builder.Append("{url:'")
                    .Append(BbsRouter.GetUrl("app/album/photo" ,string.Format("id={0}&uid={1}", p.PhotoID,p.UserID)))
                    .Append("'")
                    .Append(",img:'")
                    .Append(p.ThumbSrc)
                    .Append("'")
                    .Append(",subject:'")
                    .Append( StringUtil.ToJavaScriptString( p.Name))
                    .Append("'")
                    .Append(",description:''},");
            }
            if( PostUserEntendInfo.NewPhotos.Count>0 )
                builder.Remove(builder.Length - 1, 1);

            builder.Append("],impressions:[");

            foreach( Impression im in PostUserEntendInfo.Impressions )
            {
                builder.Append("{text:'").Append(StringUtil.ToJavaScriptString(im.Text)).Append("'")
                    .Append(",count:").Append(im.Count)
                    .Append(",userid:").Append(userID)
                    .Append("},");
            }

            if( PostUserEntendInfo.Impressions.Count>0 )
                builder.Remove(builder.Length - 1, 1);

            builder.Append("]}");


            response.Clear();
            response.Write(builder.ToString());
            response.End();
            //PhotoCollection = AlbumBO.Instance
        }

        #endregion
    }
}