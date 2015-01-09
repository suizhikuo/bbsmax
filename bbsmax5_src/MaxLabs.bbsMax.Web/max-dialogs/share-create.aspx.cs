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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.ValidateCodes;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class share_create : DialogPageBase
    {

        private string tempReplace_rn = "bbsmax_share_replace_rn";
        private string tempReplace_n = "bbsmax_share_replace_n";
        protected bool IsCollection;
		protected void Page_Load(object sender, EventArgs e)
		{

            if (My.Roles.IsInRole(Role.FullSiteBannedUsers))
                ShowError("您已经被整站屏蔽不能使用分享或收藏");

            if (CanUseCollection == false && CanUseShare == false)
            {
                ShowError("您所在的用户组没有分享或收藏的权限");
                return;
            }

            IsCollection = _Request.Get("type", Method.Get, "") == "collection";

            if (_Request.IsClick("shareforeveryone"))//分享对话筐
			{
                Create(PrivacyType.AllVisible);
			}
            else if (_Request.IsClick("shareforfriend"))//分享对话筐
            {
                Create(PrivacyType.FriendVisible);
            }
            else if (_Request.IsClick("shareforselfe"))//分享对话筐
            {
                Create(PrivacyType.SelfVisible);
            }


			shareType = _Request.Get<ShareType>("sharetype", Method.Get);
			int? targetID = _Request.Get<int>("targetID", Method.Get);
            string url = _Request.Get("url", Method.Get, string.Empty);

            if (url.ToLower() == "http://")
                ShowError("请填写要分享的地址");

            m_Url = url;

            if (shareType.HasValue)
            {
                if (targetID.HasValue)
                {
                    ShareContent content = ShareBO.Instance.GetShareContent(shareType.Value, targetID.Value, out m_UserID, out m_IsCanShare);

                    if (content != null)
                    {
                        m_Content = content.Content;
                        m_Title = content.Title;

                        //m_HideTitle = StringUtil.HtmlEncode(m_Title);
                        m_HideContent = StringUtil.HtmlEncode(ProcessHidenContent(m_Content));
                        m_HideUrl = StringUtil.HtmlEncode(content.URL);

                        m_Url = BbsRouter.ReplaceUrlTag(content.URL);
                        m_Content = BbsRouter.ReplaceUrlTag(m_Content);
                        m_Title = BbsRouter.ReplaceUrlTag(m_Title);
                        ImageUrl = content.ImgUrl;

                    }
                }
                else if (url != null)
                {
                    ShareContent content = ShareBO.Instance.GetShareContent(url, true);

                    if (content == null)
                    {
                        ShowError("无发找到您入的URL地址或者您的输入不正确");
                    }

                    shareType = content.Catagory;

                    m_Title = content.Title;

                    m_Content = content.Content;

                    //m_HideTitle = StringUtil.HtmlEncode(m_Title);

                    m_HideContent = StringUtil.HtmlEncode(ProcessHidenContent(m_Content));

                    m_IsCanShare = true;

                    m_Url = content.URL;

                    m_HideUrl = StringUtil.HtmlEncode(m_Url);
                    ImageUrl = content.ImgUrl;
                }
            }
            else
            {
                int? refShareID = _Request.Get<int>("refshareid", Method.Get);

                if (refShareID.HasValue)
                {
                    Share share = ShareBO.Instance.GetShare(refShareID.Value);

                    if (share != null)
                    {
                        shareType = share.Type;

                        m_Title = share.Subject;
                        m_Content = share.Content;

                        //m_HideTitle = StringUtil.HtmlEncode(m_Title);

                        m_HideContent = StringUtil.HtmlEncode(ProcessHidenContent(m_Content));

                        m_RefShareID = refShareID.Value;

                        m_IsCanShare = true;
                    }
                }
            }
		}

        private string ProcessHidenContent(string content)
        {
            return content.Replace("\r\n", tempReplace_rn).Replace("\n", tempReplace_n);
        }

        protected string ImageUrl;

        private string m_Url;

        public string Url
        {
            get { return m_Url; }
        }

        private int m_RefShareID;

        public int RefShareID
        {
            get { return m_RefShareID; }
        }

        ShareType? shareType = null;

        public ShareType ShareType
        {
            get { return shareType.GetValueOrDefault(); }
        }

        public string FormatUrl(string content)
        {
            return BbsRouter.ReplaceUrlTag(content);
        }

        public string TypeName
        {
            get { return ShareBO.Instance.GetShareTypeName(shareType.GetValueOrDefault()); }
        }

        private string m_Title;

        public string Subject
        {
            get { return m_Title; }
        }

		private string m_Content;

		public string Content
		{
            get
            {
                //if (Request.HttpMethod == "POST")
                //    return _Request.Get("content", Method.Post);
                //else 
                    return m_Content;
            }
		}

        //private string m_HideTitle;

        //public string HideTitle
        //{
        //    get { return m_HideTitle; }
        //}

        private string m_HideContent;

        public string HideContent
        {
            get { return m_HideContent; }
        }

        private string m_HideUrl;
        protected string HideUrl
        {
            get
            {
                return m_HideUrl;
            }
        }


		private int m_UserID;

		public int UserID
		{
			get { return m_UserID; }
		}

		private bool m_IsCanShare;

		public bool IsCanShare
		{
			get { return m_IsCanShare; }
		}


        protected bool IsCheckSelfVisible
        {
            get
            {
                if (_Request.Get("isfav", Method.Get, "").ToLower() == "true")
                {
                    if (CanUseCollection)
                        return true;
                }

                return false;
            }
        }



        private void Create(PrivacyType privacyType)
        {
            string validateCodeAction = "CreateShare";
            MessageDisplay msgDisplay = CreateMessageDisplay("description", "subject", GetValidateCodeInputName(validateCodeAction));

            if (CheckValidateCode(validateCodeAction, msgDisplay) == false)
            {
                return;
            }


            string description = _Request.Get("description", Method.Post, string.Empty);
            string content = _Request.Get("content", Method.Post, string.Empty, false).Replace(tempReplace_rn, "\r\n").Replace(tempReplace_n, "\n");
            string title = _Request.Get("title", Method.Post, string.Empty, false);
            string url = _Request.Get("url", Method.Post, string.Empty, false);
            string securityCode = _Request.Get("securityCode", Method.Post, string.Empty);
            //string securityCode2 = _Request.Get("securityCode2", Method.Post, string.Empty);
            string urlSecurityCode = _Request.Get("urlSecurityCode", Method.Post, string.Empty);
            int targetUserID = _Request.Get<int>("userid", Method.Post, 0);

            //int ptype = _Request.Get<int>("privacytype", Method.Post, 0);
            //if (ptype > 2 || ptype < 0)
            //    ptype = 0;

            //PrivacyType privacyType = (PrivacyType)ptype;

            ShareType? shareCatagory = _Request.Get<ShareType>("ShareType", Method.Post);
            
            if(shareCatagory == null)
            {
                ShowError(new InvalidShareContentError("ShareType").Message);
                return;
            }

            if (GetShareContentSafeSerial(BbsRouter.ReplaceUrlTag(content), targetUserID) != securityCode)
            {
                ShowError(new InvalidShareContentError("ShareContent").Message);
                return;
            }
            
            if (string.IsNullOrEmpty(title))
            {
                msgDisplay.AddError("subject", "标题不能为空");
                return;
            }

            if (GetShareContentSafeSerial(BbsRouter.ReplaceUrlTag(url), targetUserID) != urlSecurityCode)
            {
                ShowError(new InvalidShareContentError("ShareUrl").Message);
                return;
            }


            int refShareID = _Request.Get<int>("refshareid", 0);

            try
            {
                using (new ErrorScope())
                {
                    bool success;

                    if (refShareID == 0)
                    {
                        int targetID = _Request.Get<int>("targetID", Method.Get, 0);
                        int shareID;

                        if (shareCatagory.Value == ShareType.Video
                            || shareCatagory.Value == ShareType.URL
                            || shareCatagory.Value == ShareType.Music
                            || shareCatagory.Value == ShareType.Flash)
                        {
                            success = ShareBO.Instance.CreateShare(MyUserID, privacyType, url, title, description);
                        }
                        else
                        {
                            success = ShareBO.Instance.CreateShare(MyUserID, targetUserID, shareCatagory.Value, privacyType, url, title, content, description, targetID, out shareID);
                        }
                    }
                    else
                    {
                        success = ShareBO.Instance.ReShare(My, refShareID, privacyType, title, description);
                    }

                    if (!success)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            msgDisplay.AddError(error);
                        });
                    }
                    else
                    {
                        ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);
                        //msgDisplay.ShowInfo(this);
                        ShowSuccess(privacyType == PrivacyType.SelfVisible ? "收藏成功" : "分享成功", new object());
                    }
                }
            }
            catch(Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }


        //protected bool CanUseCollection
        //{
        //    get
        //    {
        //        return ShareBO.Instance.Permission.Can(My, SpacePermissionSet.Action.UseCollection);
        //    }
        //}

        //protected bool CanUseShare
        //{
        //    get
        //    {
        //        return ShareBO.Instance.Permission.Can(My, SpacePermissionSet.Action.UseShare);
        //    }
        //}

        [TemplateFunction]
        public string GetShareContentSafeSerial(string content,int userID)
        {
            return SecurityUtil.GetContentSecurityCode(content+userID.ToString());
        }
    }
}