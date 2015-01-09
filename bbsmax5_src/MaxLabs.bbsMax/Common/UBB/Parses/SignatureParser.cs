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
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Ubb
{
    public class SignatureParser : UbbParser
    {
        private UserBO.UserSignatureTagSettings TagSettings;

        int UserID;
        public SignatureParser(int userID)
        {
            this.UserID = userID;

            AddTagHandler(new B());
            AddTagHandler(new I());
            AddTagHandler(new S());
            AddTagHandler(new U());
            AddTagHandler(new ALIGN());
            AddTagHandler(new FONT());
            AddTagHandler(new SIZE());
            AddTagHandler(new COLOR());
            AddTagHandler(new BGCOLOR());
            AddTagHandler(new EMAIL());
            AddTagHandler(new SUB());
            AddTagHandler(new SUP());
            AddTagHandler(new INDENT());
            AddTagHandler(new LIST());
            AddTagHandler(new BR());

            TagSettings = new UserBO.UserSignatureTagSettings(userID);

            bool allowURL = TagSettings.AllowUrl;

            //if (TagSettings.AllowImage)
            AddTagHandler(new IMG(TagSettings.AllowImage, allowURL));

            //if(allowURL)
            AddTagHandler(new URL(allowURL));

            //if (TagSettings.AllowFlash)
            AddTagHandler(new FLASH(TagSettings.AllowFlash, allowURL));

            if (TagSettings.AllowTable)
                AddTagHandler(new TABLE());

            bool allowAudio = TagSettings.AllowAudio;

            AddTagHandler(new WMA(allowAudio, allowURL));
            AddTagHandler(new MP3(allowAudio, allowURL));
            AddTagHandler(new RA(allowAudio, allowURL));

            bool allowVideo = TagSettings.AllowVideo;

            AddTagHandler(new WMV(allowVideo, allowURL));
            AddTagHandler(new RM(allowVideo, allowURL));

            //使用HTML则不编码
            EncodeHtml = true;
        }

        public static string ParseForSave(int userID, string Signature)
        {
            SignatureParser parser = new SignatureParser(userID);
            
            Signature = parser.UbbToHtml(Signature);

            Signature = EmoticonParser.ParseToHtml( userID, Signature,parser.TagSettings.AllowUserEmoticon,parser.TagSettings.AllowDefaultEmoticon);

            return Signature;
        }

        public static string ParseForEdit(AuthUser operatorUser, int userID)
        {
            User user = UserBO.Instance.GetUser(userID);
            if (string.IsNullOrEmpty(user.Signature))
                return string.Empty;

            switch (UserBO.Instance.GetSignatureFormat(operatorUser))
            {
                case SignatureFormat.Html:
                    return user.Signature;
                case SignatureFormat.Ubb:
                    return HtmlToUbbParser.Html2Ubb(userID,  user.Signature);
                case SignatureFormat.Text:
                    return StringUtil.ClearAngleBracket(user.Signature);
                default:
                    return string.Empty;
            }
        }
    }
}