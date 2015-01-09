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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web
{
	public partial class realname :CenterPageBase
	{
        protected override string PageTitle
        {
            get { return "实名认证"; }
        }

        protected override string PageName
        {
            get { return "realname"; }
        }

        protected override string NavigationKey
        {
            get { return "realname"; }
        }

		protected void Page_Load(object sender, EventArgs e)
		{
            AddNavigationItem("实名认证");

            if (!EnableRealnameCheck)
            {
                ShowError("未开启实名认证功能");
            }

            if (_Request.IsClick("submit"))
            {
                SaveRealnameData();
            }
		}

        protected bool UploadCardImage
        {
            get
            {
                return AllSettings.Current.NameCheckSettings.NeedIDCardFile;
            }
        }

        private string m_IDCardNumber;
        protected string IDCardNumber
        {
            get
            {
                if (m_IDCardNumber == null)
                {
                    m_IDCardNumber = AuthenticUser.IDNumber.Substring(0, 6) + "************";
                }
                return m_IDCardNumber;
            }
        }

        private string m_IDCardFileSize;
        protected string IDCardFileSize
        {

            get
            {
                if (m_IDCardFileSize == null)
                {
                    m_IDCardFileSize = ConvertUtil.FormatSize(AllSettings.Current.NameCheckSettings.MaxIDCardFileSize);
                }

                return m_IDCardFileSize;
            }
        }

        protected bool CanInputAuthenticInfo
        {
            get
            {
                //if (My.RealnameChecked)
                //    return false;

                return this.HasAuthenticInfo==false || this.AuthenticUser.Processed;
            }
        }

        protected bool HasAuthenticInfo
        {
            get
            {
                return AuthenticUser != null; 
            }
        }

        private bool authenticUserFlag=false;
        private AuthenticUser m_AuthenticUser = null;
        protected AuthenticUser AuthenticUser
        {
            get
            {
                if (authenticUserFlag == false && m_AuthenticUser == null)
                {
                    m_AuthenticUser = UserBO.Instance.GetAuthenticUserInfo(My, MyUserID);
                    authenticUserFlag = true;
                }
                return m_AuthenticUser;
            }

        }

        protected string AllowFileSize
        {
            get
            {
                return OutputFileSize( AllSettings.Current.NameCheckSettings.MaxIDCardFileSize);
            }
        }

        private void SaveRealnameData()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("realname", "idnumber", "idcardfileface", "idcardfileback");

            string realname = _Request.Get("realname",Method.Post);
            string idNumber = _Request.Get("idnumber", Method.Post);

            HttpPostedFile idCardFileFace = null;
            HttpPostedFile idCardFileBack = null;
            if (UploadCardImage)
            {
                if (Request.Files.Count > 0)
                    idCardFileFace = Request.Files[0].FileName != string.Empty ? Request.Files[0] : null;
                if (Request.Files.Count > 1)
                    idCardFileBack = Request.Files[1].FileName != string.Empty ? Request.Files[1] : null;
            }

            using (ErrorScope es = new ErrorScope())
            {
                UserBO.Instance.SaveUserRealnameData(My, idNumber, realname, idCardFileFace,idCardFileBack);

                if (es.HasError)
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo err){
                        msgDisplay.AddError(err.TatgetName, err.Message);
                    });
                }
            }
        }
	}
}