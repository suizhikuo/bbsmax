//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Extensions;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Ubb;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class setting : CenterPageBase
    {
        protected override string PageTitle
        {
            get { return "个人资料 - " + base.PageTitle; }
        }

        protected override string PageName
        {
            get { return "setting"; }
        }

        protected override string NavigationKey
        {
            get { return "setting"; }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //AddNavigationItem("设置中心", BbsRouter.GetUrl("my/setting"));
            AddNavigationItem("个人资料");

            if (_Request.IsClick("save"))
            {
                UpdateUserProfile();
            }

            m_EmptyRequiredFieldList = AllSettings.Current.ExtendedFieldSettings.GetNeedCompleteInfoNames(My);

        }

        private List<string> m_EmptyRequiredFieldList;

        protected List<string> EmptyRequiredFieldList
        {
            get { return m_EmptyRequiredFieldList; }
        }


        private string _editorfunctions = null;
        protected string EditorFunctions
        {
            get
            {
                if (_editorfunctions != null)
                    return _editorfunctions;
                _editorfunctions = "";
                if (TagSettings.AllowUrl) _editorfunctions += ",link,unlink";
                if (TagSettings.AllowTable) _editorfunctions += ",table";
                if (ImportEmoticonLib) _editorfunctions += ",emoticons";
                if (TagSettings.AllowImage) _editorfunctions += ",image";
                if (TagSettings.AllowFlash) _editorfunctions += ",flash";
                if (TagSettings.AllowVideo || TagSettings.AllowAudio) _editorfunctions += ",media";
                return _editorfunctions;
            }
        }

        /// <summary>
        /// 不检查必填项
        /// </summary>
        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected void UpdateUserProfile()
        {
            MessageDisplay msgDisplay;

            string signature;
            short birthYear;
            short birthMonth, birthday;
            int genderValue;
            Gender gender;
            UserExtendedValueCollection extendedFileds;
            float timeZone;
            genderValue = _Request.Get<int>("gender", Method.Post, 0);
            birthYear = _Request.Get<short>("birthYear", Method.Post, 0);
            birthMonth = _Request.Get<short>("birthMonth", Method.Post, 0);
            birthday = _Request.Get<short>("birthday", Method.Post, 0);
            gender = (Gender)genderValue;
            timeZone = _Request.Get<float>("timezone", Method.Post, 8.0f);
            signature = _Request.Get("signature", Method.Post, string.Empty, false);

            List<string> fieldNames = new List<string>();

            ExtendedField[] fields = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport.ToArray();

            foreach (ExtendedField field in fields)
            {
                fieldNames.Add(field.Key);
            }

            msgDisplay = CreateMessageDisplay(fieldNames.ToArray());

            extendedFileds = UserBO.Instance.LoadExtendedFieldValues();

            CatchError<ErrorInfo>(delegate(ErrorInfo error)
            {
                msgDisplay.AddError(error);
            });

            ExtendedFieldCollection settingFields = AllSettings.Current.ExtendedFieldSettings.FieldsWithPassport;

            bool success = false;

            if (!msgDisplay.HasAnyError())
            {
                foreach (ExtendedField field in settingFields)
                {
                    if (field.IsRequired)
                    {
                        UserExtendedValue temp = extendedFileds.GetValue(field.Key);
                        if (temp == null || string.IsNullOrEmpty(temp.Value))
                        {
                            msgDisplay.AddError(field.Key, "必须填写此项");
                            break;
                        }
                    }
                }

                if (!msgDisplay.HasAnyError())
                {
                    success = UserBO.Instance.UpdateUserProfile(
                          My
                        , gender
                        , birthYear
                        , birthMonth
                        , birthday
                        , signature
                        , timeZone
                        , extendedFileds);

                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                        BbsRouter.JumpToCurrentUrl("success=1");
                }
            }


        }

        protected string ParsedSignature
        {
            get
            {
                return SignatureParser.ParseForEdit(My, MyUserID);
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
                return AllSettings.Current.UserSettings.AllowDefaultEmoticon.GetValue(My) || AllSettings.Current.UserSettings.AllowUserEmoticon.GetValue(My);
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
                return AllSettings.Current.UserSettings.SignatureFormat.GetValue(MyUserID) == SignatureFormat.Html;
            }
        }
    

    }
}