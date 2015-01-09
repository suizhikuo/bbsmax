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
using MaxLabs.bbsMax.AppHandlers;
using MaxLabs.bbsMax.Entities;
using System.Web;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class Js_EmoticonHandler : IAppHandler
    {
        public IAppHandler CreateInstance()
        {
            return new Js_EmoticonHandler();
        }

        public string Name
        {
            get { return "emotelib"; }
        }

        private int MyUserID
        {
            get;
            set;
        }

        private int UserID;
        private string Action;
        private int TagetID;
        



        public void ProcessRequest(System.Web.HttpContext context)
        {
            context.Response.Clear();

            MyUserID = User.CurrentID;

            Action = context.Request["action"];

            TagetID = StringUtil.GetInt(context.Request["targetid"], 0);

            if (context.Request.QueryString["userid"] != null)
            {
                if (!int.TryParse(context.Request.QueryString["userid"], out UserID))
                {
                    UserID = MyUserID;
                }
            }
            else
            {
                UserID = MyUserID;
            }

            if (UserID == 0) UserID = MyUserID;


            string Js = @"
            var myEmoticons={";
            Js += OutputEmoticons();
            Js += @"};
            var imgFormat='<img src=""{0}"" />';
            function emoticon2Ubb(content){ var index=0;for( var key in myEmoticons){do{  var img=myEmoticons[key];index=content.indexOf(img,index);if(index>=0){var ubb='"" emoticon=""'+key;content=content.replace(img,ubb);index+=img.length-(img.length-ubb.length);}}while(index>=0);}content=content.replace(/<img[^>]*\semoticon=\""(.+?)\"".*?>/ig,'$1');return content;}
            function emoticon2Html(content){var index=0;for( var key in myEmoticons){do{  index=content.indexOf(key,index);if(index>=0){var imgHtml=String.format(imgFormat,myEmoticons[key]);content=content.replace(key,imgHtml);index+=key.length+(imgHtml.length-key.length);}}while(index>=0);}return content;}";
            context.Response.Write(Js);
            context.Response.End();
        }

        private string OutputEmoticons()
        {
            List<IEmoticonBase> allEmoticons = new List<IEmoticonBase>();

            if (MyUserID == UserID)
            {
                allEmoticons = EmoticonBO.Instance.GetAllUserEmoticons(UserID, true);
            }
            else
            {
                allEmoticons = EmoticonBO.Instance.GetAllUserEmoticons(UserID, EmoticonBO.Instance.CanListUserEmoticons(User.Current, UserID, TagetID, Action));
            }

            StringBuilder buider = new StringBuilder(string.Empty);
            foreach (IEmoticonBase emote in allEmoticons)
            {
                buider.Append("\"" + StringUtil.ToJavaScriptString(HttpUtility.HtmlAttributeEncode(emote.Shortcut)) + "\"");
                buider.Append(":");
                buider.Append("\"" + StringUtil.ToJavaScriptString(HttpUtility.HtmlAttributeEncode(HttpUtility.UrlPathEncode(emote.ImageUrl))) + "\",");
            }
            if (buider.Length > 0)
                buider = buider.Remove(buider.Length - 1, 1);
            return buider.ToString();
        }
    }
}