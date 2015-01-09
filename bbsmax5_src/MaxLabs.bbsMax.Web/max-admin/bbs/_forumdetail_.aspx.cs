//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Data;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.ExceptableSetting;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class _forumdetail_ : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Manage_Forum;
            }
        }

        protected string Encode(string text)
        {
            return Server.HtmlEncode(text);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsEdit == true && Forum == null)
            {
                ShowError(new InvalidParamError("forumid"));
                return;
            }
            if (IsEdit)
            {
                if (Forum.ForumStatus == ForumStatus.Deleted || Forum.ForumStatus == ForumStatus.JoinTo || Forum.ForumStatus == ForumStatus.Joined)
                {
                    ShowError("当前版块处于不正常状态，不能进行编辑操作");
                    return;
                }
            }

            if (_Request.IsClick("saveforum"))
                SaveForum();

        }

        private void SaveForum()
        {
            m_IsSuccess = false;
            MessageDisplay message = CreateMessageDisplay("codeName", "forumName", "parentID", "forumType", "logoSrc", "themeID", "password"
                , "DisplayInList", "new_DisplayInList", "VisitForum", "new_VisitForum", "SigninForumWithoutPassword", "new_SigninForumWithoutPassword");

            string codeName = _Request.Get("codeName", Method.Post, string.Empty);
            string forumName = _Request.Get("forumName", Method.Post, string.Empty, false);
            int sortOrder = _Request.Get<int>("sortorder", Method.Post, 0);
            ForumType forumType = _Request.Get<ForumType>("forumType", Method.Post, ForumType.Normal);

            string password;
            if (forumType == ForumType.Link)
            {
                password = _Request.Get("forumLink", Method.Post, string.Empty, false);
                if (password.Trim() == string.Empty)
                    message.AddError("password", "请填写链接地址");
            }
            else
            {
                password = _Request.Get("password", Method.Post, string.Empty);
            }

            int columnSpan = _Request.Get<int>("colSpan", Method.Post, 0);
            int parentID;
            if (forumType == ForumType.Catalog)
            {
                parentID = 0;
            }
            else
            {
                if (IsEdit)
                    parentID = Forum.ParentID;
                else
                    parentID = _Request.Get<int>("parentForum", Method.Post, 0);
            }
            string description = StringUtil.Trim(_Request.Get("Description", Method.Post, string.Empty, false));
            string readme = StringUtil.Trim(_Request.Get("readme", Method.Post, string.Empty, false));
            string logoSrc = _Request.Get("logo", Method.Post, string.Empty);
            string themeID = _Request.Get("theme", Method.Post, string.Empty);

            ThreadCatalogStatus threadCatalogStatus;
            if (IsEdit)
                threadCatalogStatus = Forum.ThreadCatalogStatus;
            else
                threadCatalogStatus = ThreadCatalogStatus.DisEnable;

            ForumExtendedAttribute extendedDatas = new ForumExtendedAttribute();

            extendedDatas.LinkOpenByNewWidow = _Request.Get<bool>("linktype", Method.Post, false);

            //extendedDatas.AllowGuestVisitForum = _Request.Get<bool>("AllowGuestVisitForum", Method.Post, true);
            //extendedDatas.DisplayInListForGuest = _Request.Get<bool>("DisplayInListForGuest", Method.Post, true);

            //extendedDatas.DisplayInList = new ExceptableSetting.ExceptableItem_bool().GetExceptable("DisplayInList", message);
            //extendedDatas.VisitForum = new ExceptableSetting.ExceptableItem_bool().GetExceptable("VisitForum", message);
            extendedDatas.SigninForumWithoutPassword = new ExceptableSetting.ExceptableItem_bool().GetExceptable("SigninForumWithoutPassword", message);


            extendedDatas.MetaDescription = _Request.Get("MetaDescription", Method.Post, "");
            extendedDatas.MetaKeywords = _Request.Get("MetaKeywords", Method.Post, "");
            extendedDatas.TitleAttach = _Request.Get("TitleAttach", Method.Post, "");

            if (message.HasAnyError())
            {
                return;
            }

            try
            {
                using (new ErrorScope())
                {
                    bool success;
                    int forumID;
                    if (IsEdit)
                    {
                        forumID = Forum.ForumID;
                        success = ForumBO.Instance.UpdateForum(My, forumID, codeName, forumName, forumType, password, logoSrc
                            , readme, description, themeID, columnSpan, sortOrder, extendedDatas);
                    }
                    else
                    {
                        success = ForumBO.Instance.CreateForum(My, codeName, forumName, parentID, forumType, password, logoSrc
                            , themeID, readme, description, threadCatalogStatus, columnSpan, sortOrder, extendedDatas, out forumID);
                    }
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            message.AddError(error);
                        });
                        m_IsSuccess = false;
                    }
                    else
                    {

                        if (new ExceptableItem_bool().AplyAllNode("SigninForumWithoutPassword"))//应用到所有下级版块
                        {
                            Dictionary<int, ForumExtendedAttribute> extendedAttributes = new Dictionary<int, ForumExtendedAttribute>();
                            ForumCollection childForums = ForumBO.Instance.GetAllSubForums(forumID);
                            foreach (Forum tempForum in childForums)
                            {
                                tempForum.ExtendedAttribute.SigninForumWithoutPassword = forum.ExtendedAttribute.SigninForumWithoutPassword;
                                extendedAttributes.Add(tempForum.ForumID, tempForum.ExtendedAttribute);
                            }

                            ForumBO.Instance.UpdateChildForumsExtendedAttributes(My, extendedAttributes);
                        }

                        if (IsEdit == false)
                            JumpTo("bbs/manage-forum.aspx");
                        else
                            BbsRouter.JumpToCurrentUrl("success=1");
                    }
                }
            }
            catch (Exception ex)
            {
                message.AddError(ex.Message);
            }
        }



        private bool GetIntValue(string formName, out int value, out string valueString)
        {
            valueString = _Request.Get(formName, Method.Post, string.Empty);
            if (string.IsNullOrEmpty(valueString))
            {
                value = 0;
                return true;
            }
            else
            {
                return int.TryParse(valueString, out value);
            }
        }


        private bool? m_IsEdit;
        protected bool IsEdit
        {
            get
            {
                if (m_IsEdit == null)
                {
                    string action = Parameters["IsEdit"].ToString();//_Request.Get("action",Method.Get,string.Empty);
                    if (string.Compare(action, "true", true) == 0)
                    {
                        m_IsEdit = true;
                    }
                    else
                    {
                        m_IsEdit = false;
                    }
                }
                return m_IsEdit.Value;
            }
        }

        

        private ForumCollection m_Forums;
        protected ForumCollection Forums
        {
            get
            {
                if (m_Forums == null)
                {
                    GetForums();
                }
                return m_Forums;
            }
        }

        private List<string> m_ForumSeparators;
        protected List<string> ForumSeparators
        {
            get
            {
                if (m_ForumSeparators == null)
                {
                    GetForums();
                }
                return m_ForumSeparators;
            }
        }

        private void GetForums()
        {
            ForumBO.Instance.GetTreeForums("&nbsp;&nbsp;&nbsp;&nbsp;", delegate(Forum forum)
            {
                if (IsEdit)
                {
                    if (forum.ForumID == Forum.ForumID)
                        return false;
                    else
                        return true;
                }
                else
                    return true;
            }, out m_Forums, out m_ForumSeparators);
        }

        private Forum forum;
        protected Forum Forum
        {
            get
            {
                if (IsEdit)
                {
                    if (forum == null)
                    {
                        int forumID = int.Parse(Parameters["ForumID"].ToString());//_Request.Get<int>("forumID", Method.Get, 0);
                        forum = ForumBO.Instance.GetForum(forumID);
                    }
                    return forum;
                }
                return null;
            }
        }

       

        protected string ForumPassword
        {
            get
            {
                if (IsEdit && Forum.ForumType == ForumType.Normal)
                {
                    return Forum.Password;
                }
                return string.Empty;
            }
        }
        protected string ForumLink
        {
            get
            {
                if (IsEdit && Forum.ForumType == ForumType.Link)
                {
                    return Forum.Password;
                }
                return string.Empty;
            }
        }


        protected int ParentID
        {
            get
            {
                if (IsEdit)
                    return Forum.ParentID;
                else
                    return _Request.Get<int>("parentid", Method.Get, 0);
            }
        }

        protected bool DisabledCatalogItem
        {
            get
            {
                if (ParentID != 0)
                    return true;
                else
                    return false;
            }
        }
        protected bool DisabledForumType
        {
            get
            {
                if (IsEdit && ParentID == 0)
                    return true;
                else
                    return false;
            }
        }

        protected ForumType ForumType
        {
            get
            {
                if (IsEdit)
                    return Forum.ForumType;
                else if (IsCreateForumCatalog)
                    return ForumType.Catalog;
                else
                    return ForumType.Normal;
            }
        }

        protected bool IsCreateForumCatalog
        {
            get
            {
                if (ForumBO.Instance.GetAllForums().Count == 0)
                    return true;

                return _Request.Get("parentid", Method.Get) == "0";
            }
        }

        private ForumExtendedAttribute m_ForumExtendedAttribute;
        protected ForumExtendedAttribute ForumExtendedAttribute
        {
            get
            {
                if (m_ForumExtendedAttribute == null)
                {
                    if (Forum != null)
                        m_ForumExtendedAttribute = Forum.ExtendedAttribute;
                    else
                        m_ForumExtendedAttribute = new ForumExtendedAttribute();
                }

                return m_ForumExtendedAttribute;
            }
        }

        private bool? m_IsSuccess;
        protected bool IsSuccess
        {
            get
            {
                if (m_IsSuccess == null)
                {
                    m_IsSuccess = _Request.Get<int>("success", Method.Get, 0) == 1;
                }
                return m_IsSuccess.Value;
            }
        }

    }
}