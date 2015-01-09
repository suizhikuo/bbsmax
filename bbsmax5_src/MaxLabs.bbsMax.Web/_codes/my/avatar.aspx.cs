//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class avatar : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return string.Concat("设置头像 - ", base.PageTitle); }
        }

        protected override string PageName
        {
            get { return "avatar"; }
        }

        protected override string NavigationKey
        {
            get { return "avatar"; }
        }

        private string m_AvatarBuilderVars;
        protected string AvatarBuilderVars
        {
            get
            {
                if (m_AvatarBuilderVars == null)
                    m_AvatarBuilderVars =  string.Concat(IsShotMode ? "camera=true" : string.Empty, "&url=",BbsRouter.GetUrl("handler/avatar","authcookie="+ HttpUtility.UrlEncode( UserAuthCookie))
                        );
                return m_AvatarBuilderVars;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("设置头像");

            if (_Request.Get<bool>("kachashow", Method.Get) == true)
            {
                if (!string.IsNullOrEmpty(_Request.Get("file", Method.All)))
                {
                    ProcessUploadAvatar();
                }
                else
                {
                    ProcessFlash();
                }
                return;
            }

            uncheckedAvatar = UserTempDataBO.Instance.GetTempData(MyUserID, UserTempDataType.Avatar);
            
            if (_Request.IsClick("clearavatar"))
            {
                ClearAvatar();
            }
            else if (_Request.IsClick("cancelavatarcheck"))
            {
                CancelAvatarCheck();
            }
        }


        //protected bool AvatarLocked
        //{
        //    get
        //    {
        //        return AvatarLockReason != null;
        //    }
        //}

        //private bool m_GetAvatarLockReason = true;
        //private string m_AvatarLockReason;

        //protected string AvatarLockReason
        //{
        //    get
        //    {
        //        if(m_AvatarLockReason == null && m_GetAvatarLockReason)
        //        {
        //            m_AvatarLockReason = UserBO.Instance.GetAvatarLockReason(MyUserID);
        //            m_GetAvatarLockReason = false;
        //        }

        //        return m_AvatarLockReason;
        //    }
        //}

        protected bool CanCheckAvatar
        {
            get { return UserBO.Instance.CanAvatarCheck(My); }
        }


        protected void CancelAvatarCheck()
        {
            if (uncheckedAvatar != null)
                UserTempDataBO.Instance.Delete(uncheckedAvatar.UserID, UserTempDataType.Avatar);

            uncheckedAvatar = null;
        }

        protected bool IsMakingMode
        {
            get
            {
                string m_Mode = _Request.Get("mode");
                return string.Compare(m_Mode, "making", true) == 0 || string.IsNullOrEmpty(m_Mode);
            }
        }

        protected bool IsShotMode
        {
            get
            {
                string m_Mode = _Request.Get("mode");
                return string.Compare(m_Mode, "shot", true) == 0;
            }
        }

        private void ProcessUploadAvatar()
        {
#if !Passport
            if (Globals.PassportClient.EnablePassport)
            {
                PassportClientConfig settings = Globals.PassportClient;

                string passportAvatarUrl = string.Concat(settings.AvatarGeneratorUrl, "?kachashow=true&file=aaa.jpg");

                KeyValuePair<string,string>[] heads= new KeyValuePair<string,string>[1];
                heads[0]=new KeyValuePair<string,string>("size",Request.Headers["size"]);

                HttpWebResponse response = NetUtil.PostToRemoteUrl(Request, passportAvatarUrl, heads);

                Thread.Sleep(500);
            }
            else
#endif
            {
                UserBO.Instance.SaveAvatar(My, MyUserID, Request);
            }
        }

        public void ProcessFlash()
        {
            if (_Request.Get<bool>("kachashow", Method.Get) == true)
            {
                if (Request.Files.Count > 0)
                {
                    string phyFile;
                    Response.Write(UserBO.Instance.SaveTempAvatar(My, Request, out phyFile));
                    Response.End();
                }
            }
        }

        protected bool EnableAvatarCheck
        {
            get { return AllSettings.Current.AvatarSettings.EnableAvatarCheck; }
        }

        private void ClearAvatar()
        {
            UserBO.Instance.RemoveAvatar(My);
        }

        private UserTempData uncheckedAvatar;

        protected string UncheckedAvatarUrl
        {
            get
            {
                return string.Format(uncheckedAvatar.Data, "B");
            }
        }


        protected bool HasUncheckAvatar
        {
            get { return EnableAvatarCheck && uncheckedAvatar != null; }
        }
    }
}