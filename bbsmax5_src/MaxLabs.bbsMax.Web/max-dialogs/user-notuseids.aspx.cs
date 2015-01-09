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
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_pages.dialogs
{
    public partial class max_dialogs_user_notuseids : AdminDialogPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return BackendPermissions.Action.Manage_User_Add;
            }
        }

        private bool isSearch = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("search"))
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();
                int beginID = _Request.Get<int>("beginid", Method.Post, 1);
                int endID = _Request.Get<int>("endid", Method.Post, 0);

                if (endID == 0)
                {
                    msgDisplay.AddError("请输入要结束的用户ID");
                }
                else if (endID < beginID)
                {
                    msgDisplay.AddError("结束的用户ID必须大于开始的用户ID");
                }
                if (msgDisplay.HasAnyError())
                {
                    m_UserIDs = new List<int>();
                    return;
                }

                HasGet = true;
                m_BeginUserID = beginID.ToString();
                m_EndUserID = endID.ToString();
                m_UserIDs = UserBO.Instance.GetNotUserIDs(beginID, endID, 1, PageSize, out TotalCount);
                isSearch = true;
                SetPager("list", Dialog + "/user-notuseids.aspx?page={0}&beginid=" + beginID + "&endid=" + endID, 1, PageSize, TotalCount);
                //Response.Redirect("user-notuserids.aspx?beginid=" + beginID + "&endid=" + endID);
            }
            else  if (_Request.Get("beginid", Method.Get, null) != null)
            {
                HasGet = true;
            }
        }

        protected bool HasGet = false;

        private string m_BeginUserID;
        protected string BeginUserID
        {
            get
            {
                if (m_BeginUserID == null)
                {
                    int id = _Request.Get<int>("beginid", Method.Get, 0);
                    if (id > 0)
                    {
                        m_BeginUserID = id.ToString();
                    }
                    else
                        m_BeginUserID = "";
                }

                return m_BeginUserID;
            }
        }

        private string m_EndUserID;
        protected string EndUserID
        {
            get
            {
                if (m_EndUserID == null)
                {
                    int id = _Request.Get<int>("endid", Method.Get, 0);
                    if (id > 0)
                    {
                        m_EndUserID = id.ToString();
                    }
                    else
                        m_EndUserID = "";
                }

                return m_EndUserID;
            }
        }

        protected int PageSize = 50;
        protected int TotalCount = 0;

        private List<int> m_UserIDs;
        protected List<int> UserIDs
        {
            get
            {
                if (m_UserIDs == null)
                {
                    if (isSearch == false && _Request.Get("beginid", Method.Get, null) != null)
                    {
                        int beginID = _Request.Get<int>("beginid", Method.Get, 1);
                        int endID = _Request.Get<int>("endid", Method.Get, 100000);
                        int pageNumber = _Request.Get<int>("page", Method.Get, 1);
                        //int total;

                        HasGet = true;
                        m_UserIDs = UserBO.Instance.GetNotUserIDs(beginID, endID, pageNumber, PageSize, out TotalCount);
                        SetPager("list", Dialog + "/user-notuseids.aspx?page={0}&beginid=" + beginID + "&endid=" + endID, pageNumber, PageSize, TotalCount);
                    }
                }
                return m_UserIDs;
            }
        }


        ////private List<Int32Scope> m_NotUsedUserIDs;
        ////protected  List<Int32Scope> NotUsedUserIDs
        ////{
        ////    get
        ////    {
        ////        if (m_NotUsedUserIDs == null)
        ////        {
        ////            int beginID, endID;
        ////            beginID = _Request.Get<int>("beginid", 0);
        ////            endID = _Request.Get<int>("endid", 100000);
        ////          m_NotUsedUserIDs = UserBO.Instance.GetNotUseUserIDs(MyUserID, beginID, endID);
        ////        }
        ////        return m_NotUsedUserIDs;
        ////    }
        ////}
    }
}