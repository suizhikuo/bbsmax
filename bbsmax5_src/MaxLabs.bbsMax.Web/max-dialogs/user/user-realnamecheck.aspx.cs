//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using System.Collections.Generic;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class user_realnamecheck : AdminDialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ( ! UserBO.Instance.CanRealnameCheck(My))
            {
                ShowError( new NoPermissionRealnameCheckError() );
                return;
            }

            if (AuthenticUser == null)
            {
                ShowError("没有该用户提交的实名认证材料");
                return;
            }

            if (_Request.IsClick("checked"))
            {
                SetRealnameChecked(true);
            }

            else if (_Request.IsClick("autodetect"))
            {
                DetectAuthenticUserInfo();
            }

            else if (_Request.IsClick("unchecked"))
            {
                SetRealnameChecked(false);
            }
        }

        protected List<string> Photos
        {
            get;
            set;
        }

        protected int DetectState
        {
            get;
            set;
        }

        protected bool DetectFromAPI
        {
            get;
            set;
        }

        protected void DetectAuthenticUserInfo()
        {
            DetectFromAPI = true;
            MessageDisplay msgDisplay = CreateMessageDisplay();

            List<string> photos;
            using (ErrorScope es = new ErrorScope())
            {
                DetectState = UserBO.Instance.DetectAuthenticInfo(My, AuthenticUser.UserID, out photos);

                if (DetectState == 0)
                {
                    m_AuthenticUser = UserBO.Instance.GetAuthenticUserInfo(My, this.m_AuthenticUser.UserID);
                }
                else
                {
                    es.CatchError(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }

        protected void SetRealnameChecked(bool isChecked)
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            string remark = _Request.Get("remark", Method.Post);
            bool sendNotify = _Request.Get<bool>("sendnotify", Method.Post, false);

            using (ErrorScope es = new ErrorScope())
            {
                UserBO.Instance.AdminSetRealnameChaecked(My, AuthenticUser.UserID, isChecked, remark,sendNotify);

                if (es.HasUnCatchedError)
                {
                    es.CatchError(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    ShowSuccess(isChecked ? AuthenticUser.Realname + " 已成功通过身份验证。" : "操作成功",1);
                }
            }
        }

        private AuthenticUser m_AuthenticUser;
        protected AuthenticUser AuthenticUser
        {
            get
            {
                if (m_AuthenticUser == null)
                {
                    m_AuthenticUser = UserBO.Instance.GetAuthenticUserInfo(My,
                        _Request.Get<int>("userid", Method.Get, 0));
                }

                return m_AuthenticUser;
            }
        }

        /// <summary>
        /// 是否有自动检测接口
        /// </summary>
        protected bool CanAutoDetect
        {
            get
            {
return false;
            }
        }
    }
}