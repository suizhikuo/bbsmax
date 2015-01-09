//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Text;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.AppHandlers
{
    public class NotifyHandler:IAppHandler
    {
        public IAppHandler CreateInstance()
        {
            return new NotifyHandler();
        }

        public string Name
        {
            get { return "Notify"; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            string url=null;
            int urlIndex = 0;

            int notifyID =0;

            if (context.Request.QueryString["ui"] != null)
            {
                int.TryParse(context.Request.QueryString["ui"], out urlIndex);
            }


            if (int.TryParse(context.Request.QueryString["notifyID"], out notifyID))
            {
                AuthUser user = User.Current; 
                Notify notify = NotifyBO.Instance.GetNotify(UserBO.Instance.GetCurrentUserID(), notifyID);

                if (notifyID < 0)
                {
                    NotifyBO.Instance.IgnoreSystemNotify(user.UserID, notifyID);
                }
                else
                {
                    if (notify != null && notify.UserID == user.UserID)  //本人，非管理员
                        NotifyBO.Instance.IgnoreNotifies(user, new int[] { notifyID });

                    if (notify != null)
                    {
                        switch (notify.TypeID)
                        {
                            case (int)FixNotifies.CommentNotify:
                                CommentNotify cn = new CommentNotify(notify);
                                url = cn.Url;
                                break;
                            default:
                                if (notify.Urls.Length > 0 && notify.Urls.Length >= urlIndex)
                                {
                                    url = notify.Urls[urlIndex];
                                }
                                break;
                        }
                    }

                    if (notify != null && !string.IsNullOrEmpty(url))
                    {
                        context.Response.Redirect(url);
                    }
                    else
                    {
                        context.Response.Redirect(BbsRouter.GetIndexUrl());
                    }
                }
            }            
        }
    }
}