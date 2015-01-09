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
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;

using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Ubb;

namespace MaxLabs.bbsMax.Web.max_dialogs.user
{
    public partial class user_edit : UserDialogPageBase
    {
        private AuthUser m_User;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!UserBO.Instance.CanEditUserProfile(My, UserID))
            {
                ShowError(new NoPermissionEditUserProfileError());
                return;
            }

            m_User = UserBO.Instance.GetAuthUser(UserID);

            if (m_User == null || m_User == Entities.User.Guest)
            {
                ShowError(new UserNotExistsError("id", UserID));
                return;
            }

            if (_Request.IsClick("save"))
            {
                SaveUserinfo();
            }

            else if (_Request.IsClick("clearavatar"))
            {
                ClearAvatar();
            }
        }

        protected AuthUser user
        {
            get { return m_User; }
        }

        private void ClearAvatar()
        {
            UserBO.Instance.RemoveAvatar(My, UserID);
        }

        #region 签名
        protected string ParsedSignature
        {
            get
            {
                return SignatureParser.ParseForEdit(My, UserID);
            }
        }

        protected bool ImportEditor
        {
            get
            {
                SignatureFormat format = UserBO.Instance.GetSignatureFormat(My);
                return format == SignatureFormat.Html || format == SignatureFormat.Ubb;
            }
        }

        protected bool ImportEmoticonLib
        {
            get
            {
                return AllSettings.Current.UserSettings.AllowDefaultEmoticon[My] || AllSettings.Current.UserSettings.AllowUserEmoticon[My];
            }
        }

        private UserBO.UserSignatureTagSettings m_tagSetttings;
        protected UserBO.UserSignatureTagSettings TagSettings
        {
            get
            {
                if (m_tagSetttings == null)
                    m_tagSetttings = new UserBO.UserSignatureTagSettings(MyUserID);
                return m_tagSetttings;
            }
        }

        protected bool SignatureAllowHtml
        {
            get
            {
                return AllSettings.Current.UserSettings.SignatureFormat[My] == SignatureFormat.Html;
            }
        }

        #endregion

        private void SaveUserinfo()
        {
            using (ErrorScope es = new ErrorScope())
            {

                MessageDisplay msgDisplay = CreateMessageDisplay();
                string realname, email, signature, avatar;// username
                int genderValue;
                Gender gender;
                bool nameChecked, emailValidated, isActive;
                int birthYear, birthMonth, birthday;
                UserExtendedValueCollection extendedFields = UserBO.Instance.LoadExtendedFieldValues();

                nameChecked = _Request.Get<bool>("namechecked", Method.Post, false);
                emailValidated = _Request.Get<bool>("emailValidated", Method.Post, false);
                realname = _Request.Get("realname", Method.Post);
                email = _Request.Get("email", Method.Post);
                avatar = _Request.Get("avatar", Method.Post);
                genderValue = _Request.Get<int>("gender", Method.Post, 0);
                gender = (Gender)genderValue;
                birthYear = _Request.Get<int>("birthyear", Method.Post, 0);
                birthMonth = _Request.Get<int>("birthmonth", Method.Post, 0);
                birthday = _Request.Get<int>("birthday", Method.Post, 0);
                isActive = _Request.Get<bool>("isActive", Method.Post, false);
                signature = _Request.Get("signature", Method.Post, string.Empty, false);

                //User user = UserBO.Instance.GetUser(userID);

                DateTime UserBirthday = DateTimeUtil.CheckDateTime(birthYear, birthMonth, birthday);
                if (user != null)
                {
                    UserBO.Instance.AdminUpdateUserProfile(My, UserID, realname, email, gender, UserBirthday, signature, nameChecked, isActive, emailValidated, extendedFields);
                }
                else
                {

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

        protected bool NameChecked
        {
            get
            {
                return   user.RealnameChecked;
            }
        }

        protected bool ShowRealnameCheck
        {
            get { return UserBO.Instance.CanEditRole(My, UserID); }
        }

        protected bool ShowEmailCheck
        {
            get { return UserBO.Instance.CanEditRole(My, UserID); }
        }
    }
}