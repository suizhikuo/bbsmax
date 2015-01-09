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
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_pages.admin
{
    public partial class manage_inviteserial : AdminPageBase
    {
        protected override BackendPermissions.Action BackedPermissionAction
        {
            get { return BackendPermissions.Action.Manage_InviteSerial; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (_Request.IsClick("delete"))
            {
                DeleteInviteSerial();
            }
            else if (_Request.IsClick("searchSerial"))
            {
                SearchInviteSerial();
            }
            else if (_Request.IsClick("deleteAll"))
            {
                DeleteAll();
            }
        }

        private void SearchInviteSerial()
        {
            InviteSerialFilter filter = InviteSerialFilter.GetFromForm();

            filter.Apply("filter", "page");
        }

        private InviteSerialFilter _filter;

        protected InviteSerialFilter Filter
        {
            get
            {
                if (_filter == null)
                {
                    _filter = InviteSerialFilter.GetFromFilter("filter");
                    if (_filter == null) _filter = new InviteSerialFilter();
                }
                return _filter;
            }
        }

        private void DeleteInviteSerial()
        {
            //TODO 删除邀请码权限判断
            //if (InviteSerialPO.GetInstance(MyUserID).CanDeleteInviteSerial())
            //{
            int[] serialIDs = StringUtil.Split<int>(_Request.Get("serials", Method.Post), ',');
            if (serialIDs.Length > 0)
                InviteBO.Instance.DeleteInviteSerials(My, serialIDs);
            //}
            //else
            //{

            //}
        }

        private void DeleteAll()
        {
            InviteSerialFilter filter = InviteSerialFilter.GetFromFilter("filter");
            InviteBO.Instance.DeleteByFilter(My, filter);
        }

        protected bool CanDelete
        {
            get
            {
                return InviteBO.Instance.CanDeleteInviteSerial(My);
            }

        }

        protected bool CanAddSerial
        {
            get
            {
                return InviteBO.Instance.CanAddInviteSerial(My);
            }
        }
    }
}