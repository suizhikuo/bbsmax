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

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web
{
    public abstract class CenterPageBase : BbsPageBase
    {
        //protected override void OnInit(EventArgs e)
        //{
        //    base.OnInit(e);
        //}

#if !Passport
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            OnlineUserPool.Instance.Update(My, _Request, OnlineAction.OtherAction, 0, 0, "");
        }
#endif

        /// <summary>
        /// 本页面需要登陆
        /// </summary>
        protected override bool NeedLogin
        {
            get { return true; }
        }

        protected override string PageTitleAttach
        {
            get { return string.Empty; }
        }

        protected override bool IncludeBase64Js
        {
            get
            {
                return false;
            }
        }
        


#if !Passport
        protected string DoingFunctionName
        {
            get { return AllSettings.Current.DoingSettings.FunctionName; }
        }
#endif
    }
}