//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Text;
using System.Collections.Generic;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine.Template;

namespace MaxLabs.WebEngine
{
	/// <summary>
	/// WebEngine的请求上下文信息
	/// </summary>
	public class Context
	{
		public Context()
		{
			//RawAbsolutePath = HttpContext.Current.Request.Url.AbsolutePath;
		}

		public static void Init()
        {
            System.Threading.Thread.BeginThreadAffinity();

            s_Current = new Context();
			//HttpContext.Current.Items[Consts.CurrentContext] = new Context();
		}

        [ThreadStatic]
        private static Context s_Current;

		/// <summary>
		/// 获取当前的请求上下文
		/// </summary>
		public static Context Current
		{
			get
			{
                return s_Current;// (Context)HttpContext.Current.Items[Consts.CurrentContext];
			}
		}

		private ErrorInfoCollection m_Errors;

		/// <summary>
		/// 获取当前请求上下文中的所有错误信息
		/// </summary>
		public ErrorInfoCollection Errors
		{
			get
			{
				if (m_Errors == null)
					m_Errors = new ErrorInfoCollection();

				return m_Errors;
			}
		}

		private void ThrowErrorInner<TError>(TError error) where TError : ErrorInfo
		{
			error.Catched = false;
			this.Errors.Add(error);
		}

		/// <summary>
		/// 抛出错误信息（不会中止当前线程继续执行）
		/// </summary>
		/// <typeparam name="TError">错误信息类型</typeparam>
		/// <param name="error">错误信息</param>
		public static void ThrowError<TError>(TError error) where TError : ErrorInfo
		{
			if (Current != null)
				Current.ThrowErrorInner(error);
		}

        private Skin m_Skin;

        public Skin Skin
        {
            get
            {
                if (m_Skin == null)
                {

                    AuthUser user = User.Current;

                    string skinName = null;

                    if (user != null && user.UserID > 0)
                    {
                        skinName = user.SkinID;
                    }
                    else 
                    {
                        skinName = UserBO.Instance.GetCookieSkinID();
                    }

                    m_Skin = TemplateManager.GetSkin(skinName);

                    if(m_Skin == null)
                    {
                        skinName = AllSettings.Current.SkinSettings.DefaultSkin;

                        m_Skin = TemplateManager.GetSkin(skinName);
                    }

                    if (m_Skin == null)
                    {
                        m_Skin = TemplateManager.GetSkin(bbsMax.Consts.DefaultSkinID);
                    }
//#endif
                }

                return m_Skin;
            }
        }
	}
}