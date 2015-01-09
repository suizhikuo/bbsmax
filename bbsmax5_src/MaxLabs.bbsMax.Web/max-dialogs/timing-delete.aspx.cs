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
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class timing_delete : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(_Request.IsClick("delete"))
            {
                onClickDelete();
            }
        }

        protected  void onClickDelete()
        {
            bool success=false;
            int type = _Request.Get<int>("type",Method.Post,0);
            string scopeid = _Request.Get("scopeid", Method.Post, null, false);

            success = DeleteScopeBaseFromList(scopeid, SelectScopeList(type));

            if (success)
            {
                ShowSuccess("删除成功!", true);
            }
            else
            {
                ShowError("操作出错!");
            }
        }

        protected override void CheckForumClosed()
        {
            if (My.IsManager == false)
            {
                base.CheckForumClosed();
            }
        }

        /// <summary>
        /// 根据传入参数确定是哪个时间列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ScopeBaseCollection SelectScopeList(int type)
        {
            if (type == 0)
            {
                return AllSettings.Current.SiteSettings.ScopeList;
            }
            else
            {
                return AllSettings.Current.RegisterSettings.ScopeList;
            }
        }

        private bool DeleteScopeBaseFromList(string scopeid,ScopeBaseCollection scopelist)
        {
            if (scopeid != null)
            {
                foreach (ScopeBase scope in scopelist)
                {
                    if (scope.ID.ToString() == scopeid)
                    {
                        scopelist.Remove(scope);
                        return true;
                    }
                }
            }

            return false;
        }

    }
}