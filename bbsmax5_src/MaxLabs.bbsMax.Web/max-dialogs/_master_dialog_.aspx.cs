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

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class _master_dialog_ : DialogPageBase
    {
        protected override bool NeedCheckForumClosed
        {
            get { return false; }
        }

        protected override bool NeedCheckAccess
        {
            get { return false; }
        }

        protected override bool NeedCheckVisit
        {
            get { return false; }
        }

        protected override bool NeedLogin
        {
            get { return false; }
        }

        protected override bool NeedCheckRequiredUserInfo
        {
            get { return false; }
        }

        protected string DialogTitle
        {
            get
            {
                return Parameters!= null && Parameters["title"] != null ?  Parameters["title"].ToString() : "";
            }
        }

        protected bool HasTitle
        {
            get
            {
                return Parameters != null && Parameters["showTitle"] != null ? (bool)Parameters["showTitle"] : string.IsNullOrEmpty(this.DialogTitle)?false:true;
            }
        }

        protected string Width
        {
            get
            {
                return Parameters != null && Parameters["width"] != null ? Parameters["width"].ToString() : "null";
            }
        }

        protected string SubTitle
        {
            get
            {
                if (Parameters != null)
                    if (Parameters["subtitle"] != null)
                        return Parameters["subtitle"].ToString();
                return string.Empty;
            }
        }

        protected bool HasSubTitle
        {
            get
            {
                return Parameters != null && Parameters["subtitle"] != null && !string.IsNullOrEmpty(Parameters["subtitle"].ToString());
            }
        }

        protected string Height
        {
            get
            {
                return Parameters != null && Parameters["height"] != null ? Parameters["height"].ToString() : "null";
            }
        }

        //protected string FromUrl
        //{
        //    get
        //    {
        //        return Request.UrlReferrer.AbsolutePath;
        //    }
        //}
    }
}