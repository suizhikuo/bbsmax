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

using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class hail : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("addhail"))
            {
                Hail();
            }
        }

        private void Hail()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay(GetValidateCodeInputName(validateActionName));

            int HailID = _Request.Get<int>("HailID", Method.Post,0);
            string note = _Request.Get("Note");
            string IP = _Request.IpAddress;
            bool success = false;

            using (ErrorScope es = new ErrorScope())
            {
                ValidateCodeManager.CreateValidateCodeActionRecode(validateActionName);

                if (this.TheNotify ==null && !CheckValidateCode(validateActionName, msgDisplay))
                {
                    return;
                }
                else
                {
                    try
                    {
                        Notify notify = new HailNotify(MyUserID, HailID, note);
                        notify.UserID = UserID;
                        NotifyBO.Instance.AddNotify(My, notify);
                        NotifyBO.Instance.DeleteNotify(My,  NotifyID);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddException(ex);
                        return;
                    }

                }

                if (es.HasUnCatchedError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });
                }
            }

            if (success)
            {
                ShowSuccess("已成功送出了您的问候！", 1);
                return;
            }
        }

        protected HailNotify m_TheNotify;
        protected HailNotify TheNotify
        {
            get
            {
                if (NotifyID == 0)
                    return null;
                if (m_TheNotify == null)
                {
                    Notify notify = NotifyBO.Instance.GetNotify(MyUserID, NotifyID);
                    m_TheNotify = new HailNotify(notify);
                }
                return m_TheNotify;
            }
        }

        int? m_NotifyID = null;
        private int NotifyID
        {
            get
            {
                if (m_NotifyID == null)
                {
                    m_NotifyID = _Request.Get<int>("notifyid", Method.Get, 0);
                }

                return m_NotifyID.Value;
            }
        }

        private bool _hailOk;
        protected bool HailOk
        {
            get { return _hailOk; }
            set { _hailOk = value; }
        }

        protected int UserID
        {
            get
            {
             
                int hailUserID = _Request.Get<int>("uid", 0);
                int notifyID = _Request.Get<int>("notifyid", Method.Get, 0);
                if (hailUserID <= 0)
                {
                    Notify temp = NotifyBO.Instance.GetNotify(MyUserID, notifyID);
                    if (temp == null)
                    {
                        ShowError("参数错误！");
                    }
                    HailNotify notify = new HailNotify(temp);
                    if (notify != null)
                    {
                        hailUserID = notify.RelateUserID;
                    }
                }

                return hailUserID;
            }
        }
        
        protected User ForUser
        {
            get
            {
                return UserBO.Instance.GetUser(UserID,true);
            }
        }

        protected override bool EnableClientBuffer
        {
            get
            {
               return false;
            }
        }

        string _result;
        protected string Result
        {
            get
            {
                return _result ; 
            }
            set
            {
                _result = value;
            }
        }

        protected string validateActionName
        {
            get { return "hail"; }
        }
    }
}