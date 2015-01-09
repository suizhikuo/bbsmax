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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Filters;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_namecheck : AdminPageBase
	{
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_NameCheck; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            WaitForFillSimpleUsers<AuthenticUser>(UncheckUsers);

            if (_Request.IsClick("namecheck"))
            {
                NameCheck();
            }
            else if (_Request.IsClick("searchusers"))
            {
                AuthenticUserFilter filter = AuthenticUserFilter.GetFromForm();
                filter.Apply("filter", "page");
            }
            else if (_Request.IsClick("unchecked"))
            {
                RealnameUnchecked();
            }
        }

        private void NameCheck()
        {
            //MessageDisplay msgDisplay = CreateMessageDisplay();
            //string selectedIDs;
            //int userID = _Request.Get<int>("userid", Method.Post,0);

            //UserBO.Instance.AdminSetRealnameChaecked(My, userID, true,);
            //if (HasUnCatchedError)
            //{
            //    CatchError<ErrorInfo>(delegate(ErrorInfo error)
            //    {
            //        msgDisplay.AddError(error);
            //    });
            //}
        }

        protected AuthenticUserFilter Filter
        {
            get
            {
                AuthenticUserFilter filter = AuthenticUserFilter.GetFromFilter("filter");
                if (filter == null)
                {
                    filter = new AuthenticUserFilter();
                }
                if(filter.PageSize==0)filter.PageSize = Consts.DefaultPageSize;
                return filter;
            }
        }

        protected bool HasData
        {
            get
            {
                return UncheckUsers.Count > 0;
            }
        }

        private AuthenticUserCollection _uncheckUsers = null;

        public  AuthenticUserCollection UncheckUsers
        {
            get
            {
                if (_uncheckUsers == null)
                {
                    int pageNumber = _Request.Get<int>("page",  Method.Get, 1);
                    _uncheckUsers = UserBO.Instance.GetAuthenticUsers(My, Filter, pageNumber);
                }
                return _uncheckUsers;
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

        private void RealnameUnchecked()
        {
            //string selectedIDs;
            //int[] userIDs;
            //selectedIDs = _Request.Get("userids", Method.Post);
            //userIDs = StringUtil.Split<int>(selectedIDs);
            //UserBO.Instance.AdminSetRealnameChaecked(My, userIDs, false);
        }

        protected int TotalCount
        {
            get
            {
                return UncheckUsers.TotalRecords;
            }
        }
    }
}