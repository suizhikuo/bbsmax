//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System.Web.Services;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using System.Collections.Generic;
using System;
using MaxLabs.WebEngine;
using MaxLabs.Passport.Proxy;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.Passport.Server
{
    public partial class Service : ServiceBase
    {
        [WebMethod(Description="返回Passport服务器的一些配置信息", CacheDuration=5)]
        public PassportInfoProxy Passport_GetInfo()
        {
            string urlStart, urlEnd;

            switch (AllSettings.Current.FriendlyUrlSettings.UrlFormat)
            {
                case UrlFormat.Folder:
                    urlStart = string.Empty;
                    urlEnd = string.Empty;
                    break;

                case UrlFormat.Aspx:
                    urlStart = string.Empty;
                    urlEnd = ".aspx";
                    break;

                case UrlFormat.Html:
                    urlStart = string.Empty;
                    urlEnd = ".html";
                    break;

                default:
                    urlStart = "?";
                    urlEnd = string.Empty;
                    break;
            }

            PassportInfoProxy passportInfo = new PassportInfoProxy();

            passportInfo.RegisterUrl = string.Concat(urlStart, "register", urlEnd);
            passportInfo.RecoverPasswordUrl = string.Concat(urlStart, "recoverpassword", urlEnd); //BbsRouter.GetUrl("recoverpassword");
            passportInfo.LogoutUrl = string.Concat(urlStart, "logout", urlEnd); //BbsRouter.GetUrl("logout");
            passportInfo.LoginUrl = string.Concat(urlStart, "login", urlEnd); //BbsRouter.GetUrl("login");

            passportInfo.SettingAvatarUrl = string.Concat(urlStart, "my/avatar", urlEnd);
            passportInfo.SettingPrlofileUrl = string.Concat(urlStart, "my/setting", urlEnd);
            passportInfo.SettingPasswordUrl = string.Concat(urlStart, "my/changepassword", urlEnd);
            passportInfo.SettingChangeEmailUrl = string.Concat(urlStart, "my/changeemail", urlEnd);
            passportInfo.SettingNotifyUrl = string.Concat(urlStart, "my/notify-setting", urlEnd);

            passportInfo.CenterUrl = string.Concat(urlStart, "my/default", urlEnd);
            passportInfo.CenterNotifyUrl = string.Concat(urlStart, "my/notify", urlEnd);
            passportInfo.CenterChatUrl = string.Concat(urlStart, "my/chat", urlEnd);
            passportInfo.CenterFriendUrl = string.Concat(urlStart, "my/friends", urlEnd);

            passportInfo.AvatarGeneratorUrl = string.Concat(urlStart, "my/avatar",urlEnd);
            passportInfo.EnablePhoneValidate = AllSettings.Current.PhoneValidateSettings.Open;
            passportInfo.EnableRealnameCheck = AllSettings.Current.NameCheckSettings.EnableRealnameCheck;

            passportInfo.ExtendedFields = new List<ExtendedFieldProxy>();

            foreach (ExtendedField field in AllSettings.Current.ExtendedFieldSettings.Fields)
            {
                passportInfo.ExtendedFields.Add(ProxyConverter.GetExtendedFieldProxy(field));
            }

            
            passportInfo.CookieName = UserBO.cookieKey_User;

            return passportInfo;
        } 

        private static List<KeyValuePair<string,int>> clientTryRegCounter = new List<KeyValuePair<string,int>>();

        [WebMethod]
        public APIResult Passport_RegiserClient( string ownerUsername,string ownerPassword, string clientName,string url,string apiFilePath,string accessKey,int[] instructList,out int clientID)
        {
            APIResult result = new APIResult();

            string ip = Context.Request.UserHostAddress; //获取客户端ip
            int count=0;
            KeyValuePair<string, int> tryer = new KeyValuePair<string, int>();
            foreach ( KeyValuePair<string,int> c in clientTryRegCounter)
            {
                if (c.Key == ip)
                {
                    tryer = c;
                    break;
                }
            }

            count = tryer.Value;
            

            if (count > 10) //最多一个IP10次尝试
            {
                result.IsSuccess = false;
                result.Messages.Add("超过允许的尝试注册次数！请联系Passport管理员");
                clientID = 0;
                return result;
            }

            if (clientTryRegCounter.Count > 100)  //每次超过100个客户端要注册， 认为是不正常的
            {
                throw new Exception("接口出现异常，已关闭注册。 请联系服务器管理员");
            }

            clientID = 0;
      
            AuthUser user = UserBO.Instance.GetAuthUser(ownerUsername);
            if (user == null
                || user == MaxLabs.bbsMax.Entities.User.Guest
                || !user.IsOwner
                || !SecurityUtil.ComparePassword( user.PasswordFormat, ownerPassword, user.Password))
            {
                result.IsSuccess = false;
                result.Messages .Add( "Passport创始人信息无效！");

                if (string.IsNullOrEmpty(tryer.Key))
                {
                    tryer = new KeyValuePair<string, int>(ip, 1);
                    clientTryRegCounter.Add(tryer);
                }
                else
                {
                    clientTryRegCounter.Remove(tryer);
                    clientTryRegCounter.Add(new KeyValuePair<string, int>(ip, count + 1));
                }

                return result;
            }

            using (ErrorScope es = new ErrorScope())
            {
                InstructType[] instrcuts = new InstructType[instructList == null ? 0 : instructList.Length];

                for (int i = 0; i < instrcuts.Length; i++)
                    instrcuts[i] = (InstructType) instructList[i];

                PassportClient client = PassportBO.Instance.CreatePassportClient(clientName, url, apiFilePath, accessKey, instrcuts);
                if (client == null)
                {
                    result.IsSuccess = false;

                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        result.AddError(error.TatgetName,error.Message);
                    }
                    );
                }
                else
                {
                    clientID = client.ClientID;
                    result.IsSuccess = true;
                }
            }

            return result;
        }

        [WebMethod(Description = "此接口用于测试Passport服务器是否工作正常，比如有机房有没有被外星人攻击，或者服务器有没有被偷走。没有特殊的含义。")]
        public string Hello(string yourName)
        {
            if(AllSettings.Current.PassportServerSettings.EnablePassportService)
                return "Hello " + yourName+"";
            return "API not is open!";
        }
    }
}