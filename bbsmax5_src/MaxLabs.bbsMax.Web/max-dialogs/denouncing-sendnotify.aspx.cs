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
using MaxLabs.WebEngine;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class denouncing_sendnotify : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (_Request.IsClick("send"))
            {
                int? uid = _Request.Get<int>("uid");
                int? tid = _Request.Get<int>("tid");
                Enums.DenouncingType? type = _Request.Get<Enums.DenouncingType>("type");

                if(uid == null || tid == null || type == null)
                {
                    ShowError("缺少必要的页面参数");
                    return;
                }

                string name = string.Empty;
                string url = string.Empty;

                switch(type.Value)
                {
                    case MaxLabs.bbsMax.Enums.DenouncingType.Blog:
                        name = "一篇日志";
                        url = BbsRouter.GetUrl("app/blog/view", "id=" + tid);
                        break;

                    case MaxLabs.bbsMax.Enums.DenouncingType.Photo:
                        name = "一张照片";
                        url = BbsRouter.GetUrl("app/album/photo", "id=" + tid);
                        break;

                    case MaxLabs.bbsMax.Enums.DenouncingType.Reply:
                        name= "一条回复";

                        break;

                    case MaxLabs.bbsMax.Enums.DenouncingType.Share:
                        name = "一条分享";
                        url = BbsRouter.GetUrl("app/share/index");
                        break;

                    case MaxLabs.bbsMax.Enums.DenouncingType.Topic:
                        name = "一个主题";
                        break;
                }

                using (ErrorScope es = new ErrorScope())
                {
                    AdminManageNotify notify = new AdminManageNotify(MyUserID, string.Concat("您有", name, "被举报，请您删除此数据，如有疑问请联系管理员。<a href=\"", url ,"\">前往查看</a>"));
                    notify.UserID = uid.Value;
                    if (NotifyBO.Instance.AddNotify( My , notify))
                    {
                        Return(true);
                    }
                    else
                    {
                        es.CatchError<ErrorInfo>(delegate(ErrorInfo error) {
                            ShowError(error);
                        });
                    }
                }
            }
        }
    }
}