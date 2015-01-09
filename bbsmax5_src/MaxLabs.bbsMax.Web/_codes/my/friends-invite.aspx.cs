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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Email;
using MaxLabs.bbsMax.Logs;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class friends_invite : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "邀请好友 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "friends"; }
        }

        protected override string NavigationKey
        {
            get { return "friends-invite"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddNavigationItem("邀请好友");

            if (this.EnableInvitation == false)
            {
                ShowError("当前系统未启用邀请功能");
                return;
            }

            if (_Request.IsClick("emailinvite"))
            {
                SendInviteMail();
                return;
            }
            else if (_Request.IsClick("buySerial"))
            {
                BuySerial();
                return;
            }
            else if (_Request.IsClick("searchSerial"))
            {
                SearchInviteSerial();
                return;
            }
            else if (_Request.IsClick("setSerial"))
            {
                SetMyInviteSerial();
                return;
            }

            if (AllSettings.Current.InvitationSettings.InviteMode == InviteMode.InviteSerialOptional
                || AllSettings.Current.InvitationSettings.InviteMode == InviteMode.InviteSerialRequire)
            {
                UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();
                scheme.AttachQuery("page", "{0}");
                SetPager("pager1", scheme.ToString(false), _Request.Get<int>("page", Method.Get, 1), Filter.Pagesize, this.TotalCount);
                scheme.AttachQuery("ipage", "{0}");
                SetPager("pager2", scheme.ToString(false), _Request.Get<int>("ipage", Method.Get, 1), InviteesPageSize, Invitees.TotalRecords);
            }

        }

        private void SetMyInviteSerial()
        {
            string myInviteSerial;
            myInviteSerial = _Request.Get("MyInviteSerial", Method.Post);

            using (ErrorScope es = new ErrorScope())
            {

                MessageDisplay msgDisplay = CreateMessageDisplayForForm("setSerial");

                if (!UserBO.Instance.SetUserInviteCode(MyUserID, myInviteSerial))
                {
                    ThrowError(new InviteSerialError("MyInviteSerial", myInviteSerial));
                }

                if (es.HasUnCatchedError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
                else
                {
                    //msgDisplay.ShowInfo(this);
                    ShowSuccess();
                    //BbsRouter.JumpToCurrentUrl("success=1");
                }
            }
        }

        private void SearchInviteSerial()
        {
            InviteSerialFilter filter = InviteSerialFilter.GetFromForm();
            filter.Apply("filter", "page");
        }

        private int _totalCount;

        public int TotalCount
        {
            get
            {
               return  SerialList.TotalRecords;
            }
        }

        private InviteSerialCollection _serialList;


        /// <summary>
        /// 够买邀请码
        /// </summary>
        private void BuySerial()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            int buyCount;
            buyCount = _Request.Get<int>("buyCount", MaxLabs.WebEngine.Method.Post, 0);
            try
            {
                InviteBO.Instance.BuyInviteSerial(My, buyCount);

                if (HasUnCatchedError)
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        if (error is UserPointOverMinValueError || error is UserPointOverMaxValueError)
                        {
                            UserPointOverMinValueError tempError = (UserPointOverMinValueError)error;
                            ErrorInfo pointError = new BuyInviteSerialPointError(tempError.UserPoint.Name, tempError.GetType());
                            msgDisplay.AddError(pointError.Message);
                        }
                        else
                        {
                            msgDisplay.AddError(error);
                        }
                    });
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddException(ex);
            }
        }

        protected InviteSerialStatus Status
        {
            get
            {
                InviteSerialStatus status = _Request.Get<InviteSerialStatus>("status", Method.Get, InviteSerialStatus.Unused);
                return status;
            }
        }

        protected InviteSerialCollection SerialList
        {
            get
            {
                if (_serialList == null)
                {
                    InviteSerialFilter filter = InviteSerialFilter.GetFromFilter("filter");
                    int pageNumber = _Request.Get<int>("page", Method.Get, 1);

                    _serialList = InviteBO.Instance.GetInviteSerials(My, Status, filter.SearchKey, pageNumber, out _totalCount);

                    FillSimpleUsers<InviteSerial>(_serialList, 0);
                }
                return _serialList;
            }
        }

        private InviteSerialFilter _filter;

        protected InviteSerialFilter Filter
        {
            get
            {
                if (_filter == null)
                {
                    _filter = InviteSerialFilter.GetFromFilter("filter");
                }
                return _filter;
            }
        }

        private void SendInviteMail()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay();
            string obiterText;
            string emailString;
            string[] emails;

            obiterText = _Request.Get("message", Method.Post);
            emailString = _Request.Get("emails", Method.Post);

            using (ErrorScope es = new ErrorScope())
            {

                if (string.IsNullOrEmpty(emailString))
                {
                    ThrowError(new EmptyEmailError("emails"));
                }
                else
                {
                    emails = StringUtil.GetLines(emailString);
                    UserBO.Instance.MassEmailingInvite(My, emails, InviteBO.Instance.BuildFixInviteCode(MyUserID), obiterText);
                }

                if (es.HasUnCatchedError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }
        }

        protected bool ShowInvitePoint
        {
            get { return InvitePointAction.Instance.HasActionUserPointValue(InvitePointType.InviteNewUser, MyUserID); }

        }

        private User _inviter;

        protected string SiteRoot
        {
            get { return Globals.SiteRoot; }
        }

        protected User Inviter
        {
            get
            {
                if (My.InviterID == 0) return null;
                if (_inviter == null)
                {
                    _inviter = UserBO.Instance.GetUser(My.InviterID);
                }

                return _inviter;
            }
        }

        protected string InviteEmailContent
        {
            get
            {
                InviteEmail mail;
                mail = new InviteEmail(null,
                 "<span id=\"obiter\">&nbsp;</span>",
                  InviteBO.Instance.BuildFixInviteCode(MyUserID), My.Name, MyUserID);
                return mail.Content;
            }
        }

        protected string GetStatusUrl(string status)
        {
            //NameValueCollection param = new NameValueCollection();
            //param.Add("status", status);
            //return BbsRouter.GetUrl("friend/invite", ) + UrlUtil.AttachQueryString(param, true, "page", "filter");

            UrlScheme scheme = BbsRouter.GetCurrentUrlScheme();

            scheme.RemoveQuery("page");
            scheme.RemoveQuery("filter");
            scheme.AttachQuery("status", status);

            return scheme.ToString();
        }

        protected string InviteEmailTitle
        {
            get
            {
                return AllSettings.Current.InvitationSettings.InviteEmailTitle;
            }
        }

        protected string MyInviteCode
        {
            get
            {
                return InviteBO.Instance.BuildFixInviteCode(MyUserID);
            }
        }

        /// <summary>
        /// 检查邮件系统是否关闭
        /// </summary>
        public bool CanSendEmail
        {
            get
            {
                return AllSettings.Current.EmailSettings.EnableSendEmail;
            }
        }

        protected bool InviteSerialMode
        {
            get
            {
                InvitationSettings settings = AllSettings.Current.InvitationSettings;

                if (settings.InviteMode == InviteMode.InviteSerialOptional || settings.InviteMode == InviteMode.InviteSerialRequire)
                    return true;

                return false;
            }
        }

        //权限判断
        protected bool CanBuySerial
        {
            get
            {
                InvitationSettings settings = AllSettings.Current.InvitationSettings;
                return (settings.InviteMode == InviteMode.InviteSerialOptional || settings.InviteMode == InviteMode.InviteSerialRequire)
                    && InviteBO.Instance.CanBuyInviteSerial(My);
            }
        }

        protected string InvitePointValue
        {
            get
            {
                return InvitePointAction.Instance.GetActionUserPointValue(InvitePointType.InviteNewUser, MyUserID, "{0}:{1}(剩余:{2})", "； ");
            }
        }

        protected string SerialPrice
        {
            get
            {
                UserPointType type= AllSettings.Current.InvitationSettings.PointFieldIndex;
                int price = AllSettings.Current.InvitationSettings.IntiveSerialPoint;
                UserPoint point = AllSettings.Current.PointSettings.GetUserPoint(type);
                
                return string.Format("{0}:{1}(剩余:{2})",point.Name,price,My.ExtendedPoints[(int)type]);
            }
        }

        private int? m_canbuyCount = null;
        protected int CanBuyCount
        {
            get
            {
                if (m_canbuyCount == null)
                    m_canbuyCount = InviteBO.Instance.GetCanBuyCount(MyUserID);
                return m_canbuyCount.Value;
            }
        }

        protected string BuyInterval
        {
            get
            {
                if (AllSettings.Current.InvitationSettings.Interval == InviteBuyInterval.Disable)
                    return "无限制";
                string u = string.Empty;
                switch (AllSettings.Current.InvitationSettings.Interval)
                {
                    case InviteBuyInterval.ByDay:
                        u = "天";
                        break;
                    case InviteBuyInterval.ByHour:
                        u = "小时";
                        break;
                    case InviteBuyInterval.ByMonth:
                        u = "月";
                        break;
                    case InviteBuyInterval.ByYear:
                        u = "年";
                        break;
                }

                return string.Concat("每", u, AllSettings.Current.InvitationSettings.InviteSerialBuyCount.ToString(), "个");

            }
        }

        #region  邀请人列表

        protected int InviteesPageNumber
        {
            get
            {
                return _Request.Get<int>("ipage", Method.Get, 1);
            }
        }

        protected int InviteesPageSize
        {
            get
            {
                return 15;
            }
        }

        UserCollection _invitees = null;

        int _inviteesCount;

        public int InviteesCount
        {
            get { return _inviteesCount; }
        }

        public UserCollection Invitees
        {
            get
            {
                if (_invitees == null) _invitees = UserBO.Instance.GetInvitees(MyUserID, InviteesPageSize, InviteesPageNumber, out _inviteesCount);
                return _invitees;
            }
        }
        #endregion
    }
}