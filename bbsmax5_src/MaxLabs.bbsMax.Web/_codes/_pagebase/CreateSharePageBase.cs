//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web
{
    //public abstract partial class CreateSharePageBase : BbsPageBase
    //{
    //    protected void Page_Load(object sender, EventArgs e)
    //    {
    //        if (_Request.IsClick(CreateShareButtonName) || _Request.IsClick(CreateCollectionButtonName))
    //        {
    //            Create();
    //        }
    //    }

    //    protected override bool NeedLogin
    //    {
    //        get { return true; }
    //    }

    //    protected abstract string CreateShareButtonName { get; }

    //    protected abstract string CreateCollectionButtonName { get; }

    //    protected abstract string PrivacyTypeFormName { get; }

    //    protected abstract string UrlFormName { get; }

    //    protected abstract string DescriptionFormName { get; }



    //    private void Create()
    //    {
    //        PrivacyType privacyType;
    //        if (_Request.IsClick(CreateShareButtonName))
    //        {
    //            if (_Request.Get(PrivacyTypeFormName, Method.Post, "0") == "0")
    //                privacyType = PrivacyType.AllVisible;
    //            else
    //                privacyType = PrivacyType.FriendVisible;
    //        }
    //        else
    //            privacyType = PrivacyType.SelfVisible;

    //        string validateCodeAction;
            
    //        if(privacyType == PrivacyType.SelfVisible)
    //            validateCodeAction = "CreateCollection";
    //        else
    //            validateCodeAction = "CreateShare";

    //        MessageDisplay msgDisplay = CreateMessageDisplay("url", "description", GetValidateCodeInputName(validateCodeAction));


    //        string url = _Request.Get(UrlFormName, Method.Post);

    //        string description = _Request.Get(DescriptionFormName, Method.Post, string.Empty);

    //        if (!CheckValidateCode(validateCodeAction, msgDisplay))
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            using (new ErrorScope())
    //            {

    //                bool success = ShareBO.Instance.CreateShare(MyUserID, privacyType, url, description);
                        
    //                if (success == false)
    //                {
    //                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
    //                    {
    //                        msgDisplay.AddError(error);
    //                    });
    //                }
    //                else
    //                {
    //                    ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);
    //                    _Request.Clear(Method.Post);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            msgDisplay.AddError(ex.Message);
    //        }
    //    }
    //}
}