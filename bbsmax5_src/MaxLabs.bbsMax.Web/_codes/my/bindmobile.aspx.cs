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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class bindmobile : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return string.Concat("绑定手机 - ", base.PageTitle); }
        }

        protected override string PageName
        {
            get { return "bindmobile"; }
        }
        protected override string NavigationKey
        {
            get { return "bindmobile"; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("手机认证");
            FirstVisit();

            if (EnableMobileBind == false)
            {
                ShowError("当前手机绑定功能已关闭!");
            }


            if (_Request.IsClick("next"))
            {
                NotFirstVisit();
                PhoneValidate();
            }

            if (_Request.IsClick("finish"))
            {
                NotFirstVisit();
                Finish();
            }

            if (_Request.IsClick("changemobile"))
            {
                NotFirstVisit();
                ChangeMobileFirst = true;
            }

            if (_Request.IsClick("changemobilenext"))
            {
                NotFirstVisit();
                ChangeMobile();
            }

            if (_Request.IsClick("changemobilefinish"))
            {
                NotFirstVisit();
                ChangeMobileFinish();
            }

            if (_Request.IsClick("unbound"))
            {
                NotFirstVisit();
                UnBound();
            }

            if (_Request.IsClick("unboundfinish"))
            {
                NotFirstVisit();
                UnBoundFinish();
            }

        }

        private void ChangeMobile()
        {
            MessageDisplay messageDisplay = CreateMessageDisplay("mobilePhone");
            string newMobilePhone = _Request.Get("mobilePhone", Method.Post, string.Empty);

            using (ErrorScope es = new ErrorScope())
            {
                ChangeMobileSecond = UserBO.Instance.SendChangePhoneSms(My, newMobilePhone);

                es.CatchError<PhoneRepeatError>(delegate(PhoneRepeatError error)
                {
                    ChangeMobileFirst = true;
                    ChangeMobileSecond = false;
                    messageDisplay.AddError(error);
                });
                es.CatchError<PhoneValidateLimitError>(delegate(PhoneValidateLimitError error)
                {
                    //如果抛出一天限制三次的异常,则跳转到输入短信验证码页面.
                    ChangeMobileSecond = true;
                    IsOverrunLimitTryNum = true;
                    messageDisplay.AddError(error);
                });
                es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                {
                    ChangeMobileFirst = true;
                    ChangeMobileSecond = false;
                    messageDisplay.AddError(err.TatgetName, err.Message);

                });
            }
        }

        private void ChangeMobileFinish()
        {
            MessageDisplay messageDisplay = CreateMessageDisplay("oldsmscode", "newsmscode");
            string oldSMSCode = _Request.Get("oldsmscode", Method.Post, string.Empty, false);
            string newSMSCode = _Request.Get("newsmscode", Method.Post, string.Empty, false);
            string newMobilePhone = _Request.Get("mobilePhone", Method.Post, string.Empty);

            using (ErrorScope es = new ErrorScope())
            {
                ChangeMobileThird = UserBO.Instance.ChangePhoneBySmsCode(My, newMobilePhone, oldSMSCode, newSMSCode);
                if (ChangeMobileThird==false)
                {
                    ChangeMobileSecond = true;
                }
                es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                {
                    messageDisplay.AddError(err.TatgetName, err.Message);
                });

            }
        }

        private void UnBound()
        {
            MessageDisplay messageDisplay = CreateMessageDisplay();
            using (ErrorScope es = new ErrorScope())
            {
                UnBoundMobileFirst = UserBO.Instance.SendUnbindPhoneSms(My);
                es.CatchError<PhoneValidateLimitError>(delegate(PhoneValidateLimitError error)
                {
                    //如果抛出一天限制三次的异常,则跳转到输入短信验证码页面.
                    UnBoundMobileFirst = true;
                    IsOverrunLimitTryNum = true;
                    messageDisplay.AddError(error);
                });
                es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                {
                    messageDisplay.AddError(err.TatgetName, err.Message);
                });
            }
        }

        private void UnBoundFinish()
        {
            MessageDisplay messageDisplay = CreateMessageDisplay("smscode");
            string phoneCode = _Request.Get("smscode", Method.Post, string.Empty, false);
            using (ErrorScope es = new ErrorScope())
            {
                UnBoundMobileSecond = UserBO.Instance.UnbindPhoneBySmsCode(My, phoneCode);
                if (UnBoundMobileSecond==false)
                {
                    UnBoundMobileFirst = true;
                }
                es.CatchError<ErrorInfo>(delegate(ErrorInfo err)
                {
                    messageDisplay.AddError(err.TatgetName, err.Message);
                });
            }
        }


        private void PhoneValidate()
        {
            MessageDisplay messageDisplay = CreateMessageDisplay("mobilePhone");
            string mobilePhone = _Request.Get("mobilePhone", Method.Post, string.Empty);

            using (ErrorScope es = new ErrorScope())
            {
                BindingMobileSecond = UserBO.Instance.SendBindPhoneSms(My, mobilePhone);


                es.CatchError<PhoneRepeatError>(delegate(PhoneRepeatError error)
                {
                    //如果抛出手机号码重复的异常,则跳转到输入手机号码页面.
                    BindingMobileFirst = true;
                    BindingMobileSecond = false;
                    messageDisplay.AddError(error);
                });
                    

                es.CatchError<PhoneValidateLimitError>(delegate(PhoneValidateLimitError error)
                {
                    //如果抛出一天限制三次的异常,则跳转到输入短信验证码页面.
                    BindingMobileFirst = false;
                    BindingMobileSecond = true;
                    //用来判断是否显示重新发送短信按钮
                    IsOverrunLimitTryNum = true;
                    messageDisplay.AddError(error);
                });
                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    BindingMobileFirst = true;
                    BindingMobileSecond = false;
                    messageDisplay.AddError(error);
                });
            }
        }

        private void Finish()
        {
            MessageDisplay messageDisplay = CreateMessageDisplay("smscode", "mobilePhone");
            string phoneCode = _Request.Get("smscode", Method.Post, string.Empty, false);
            string mobilePhone = _Request.Get("mobilePhone", Method.Post, string.Empty);

            using (ErrorScope es = new ErrorScope())
            {
                BindingMobileThird = UserBO.Instance.BindPhoneBySmsCode(My, mobilePhone, phoneCode);
                if (BindingMobileThird == false)
                {
                    BindingMobileSecond = true;   
                }
                es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                {
                    messageDisplay.AddError(error);
                });
            }

        }

        public bool IsBound
        {
            get
            {
                return My.MobilePhone != 0;
            }
        }

        private string m_NewInputMobilePhone;
        public string NewInputMobilePhone
        {
            get
            {
                if (m_NewInputMobilePhone == null)
                    m_NewInputMobilePhone = _Request.Get("mobilePhone", Method.Post, string.Empty);
                return m_NewInputMobilePhone;
            }

        }

        public string UserMobilePhoneWithStar
        {
            get
            {
                if (IsBound)
                {
                    string mobilePhone = GetStarFormatMobilePhone(My.MobilePhone);
                    return mobilePhone;
                }
                return string.Empty;
            }
        }

        private string GetStarFormatMobilePhone(long mobilePhone)
        {
            string phoneNumStr = mobilePhone.ToString();

            phoneNumStr = string.Format("{0}****{1}", phoneNumStr.Substring(0, 3), phoneNumStr.Substring(7, 4));

            return phoneNumStr;
        }

        private void FirstVisit()
        {
            if (IsBound)
            {
                IsBoundFirst = true;
            }
            else
            {
                BindingMobileFirst = true;
            }
        }

        private void NotFirstVisit()
        {
            IsBoundFirst = false;
            BindingMobileFirst = false;
        }

        private string m_SubTitle;
        public string SubTitle
        {
            get 
            {
                if (m_SubTitle == null)
                    GetSubTitle();
                return m_SubTitle;
            }
        }

        private void  GetSubTitle()
        {
            if (ChangeMobileFirst || ChangeMobileSecond || ChangeMobileThird)
            {
                m_SubTitle = "更改手机绑定";
            }
            else if (UnBoundMobileFirst || BindingMobileSecond)
            {
                m_SubTitle = "解除手机绑定";
            }
            else
            {
                m_SubTitle = "手机绑定";
            }
        }

        protected bool IsOverrunLimitTryNum { get; set; }

        public bool IsBoundFirst { get; set; }

        public bool ChangeMobileFirst { get; set; }

        public bool ChangeMobileSecond { get; set; }

        public bool ChangeMobileThird { get; set; }

        public bool UnBoundMobileFirst { get; set; }

        public bool UnBoundMobileSecond { get; set; }

        public bool BindingMobileFirst { get; set; }

        public bool BindingMobileSecond { get; set; }

        public bool BindingMobileThird { get; set; }

    }

}