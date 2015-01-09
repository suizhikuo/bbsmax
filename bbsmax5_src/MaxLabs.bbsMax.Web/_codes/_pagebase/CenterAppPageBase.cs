//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.Security;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web
{
	public abstract class CenterAppPageBase : AppPageBase
	{
        protected override void OnInit(EventArgs e)
		{
            base.OnInit(e);
 

            if (IsSpace)
            {
                if (AllSettings.Current.SpaceSettings.AllowGuestAccess == false)
                {
                    if (IsLogin == false)
                    {
                        ShowError(new Errors.CustomError("", "您必须登录后才能访问用户空间"));
                    }
                }
            }


			if (EnableFunction == false)
				BbsRouter.JumpTo("my/default");
		}

        

		protected abstract bool EnableFunction
		{
			get;
		}

        protected abstract string FunctionName
        {
            get;
        }


        protected override bool NeedLogin
        {
            get
            {
                if (IsSpace)
                {
                    return false;
                }
                else
                    return true;//base.NeedLogin;
            }
        }




        //===============================================

        private bool? m_IsSpace;
        /// <summary>
        /// 是否是空间的页面
        /// </summary>
        protected virtual bool IsSpace
        {
            get
            {
                if (m_IsSpace == null)
                    m_IsSpace = AppOwnerUserID != MyUserID; //|| _Request.Get("uid", Method.Get) != null;

                return m_IsSpace.Value;
            }
        }

        protected bool SpaceCanAccess
        {
            get
            {
                return true;
            }
        }

	}
}