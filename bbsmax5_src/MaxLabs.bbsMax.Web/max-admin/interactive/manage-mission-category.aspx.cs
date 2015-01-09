//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_admin.interactive
{
    public partial class manage_mission_category : AdminPageBase
    {
        protected override MaxLabs.bbsMax.Settings.BackendPermissions.Action BackedPermissionAction
        {
            get
            {
                return MaxLabs.bbsMax.Settings.BackendPermissions.Action.Manage_Mission_Category;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_CategoryList = MissionBO.Instance.GetMissionCategories();

            if (_Request.IsClick("delete"))
            {
                int[] ids = StringUtil.Split<int>(_Request.Get("CategoryIDs", MaxLabs.WebEngine.Method.Post, string.Empty));

                if (ids.Length > 0)
                {
                    MissionBO.Instance.DeleteMissionCategories(My, ids);

                    JumpTo("interactive/manage-mission-category.aspx?delsucceed=1");
                }
            }
        }

        private MissionCategoryCollection m_CategoryList;

        public MissionCategoryCollection CategoryList
        {
            get
            {
                return m_CategoryList;
            }
        }
    }
}