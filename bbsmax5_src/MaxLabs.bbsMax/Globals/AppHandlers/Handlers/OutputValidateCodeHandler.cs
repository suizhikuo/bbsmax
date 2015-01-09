//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class OutputValidateCodeHandler : IAppHandler
    {
        public IAppHandler CreateInstance()
        {
            return new OutputValidateCodeHandler();
        }

        public string Name
        {
            get { return "vcode"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            RequestVariable _request = RequestVariable.Current;

            string validateCode;
            string type = _request.Get("type", Method.Get, string.Empty);

            string isStyleType = _request.Get("isstyletype", Method.Get, "0");

            string id = _request.Get("id",Method.Get,string.Empty);

            byte[] image;

            if (isStyleType == "1")
            {
                ValidateCodeType validateCodeType = ValidateCodeManager.GetValidateCodeType(type);

                if (validateCodeType == null)
                    return;

                image = validateCodeType.CreateImage(out validateCode);
            }
            else
            {
                image = MaxLabs.bbsMax.ValidateCodes.ValidateCodeManager.CreateImage(type, out validateCode);
            }

            context.Session[Consts.ValidateCode_SessionKey_Prefix + type.ToLower()+id.ToLower().Trim()] = validateCode;

            context.Response.Clear();
            context.Response.ContentType = "image/gif";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.BinaryWrite(image);
            context.Response.End();
        }

    }
}