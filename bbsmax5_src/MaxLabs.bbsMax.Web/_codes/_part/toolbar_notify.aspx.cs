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
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class toolbar_notify : PartPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            string ac = _Request.Get("ac", Method.Get);
            if (ac != null)
            {
                switch (ac)
                {
                    case "ignore":
                        IgnoreNotify();
                        break;
                    case "delete":
                        DeleteNotify();
                        break;
                }
                IsPostBack = 1;
            }
		}

        private void DeleteNotify()
        {
            int[] notifyIDs = _Request.GetList<int>("notifyid", Method.All, new int[0]);
            NotifyBO.Instance.DeleteNotifies(MyUserID, notifyIDs);
        }

        private void IgnoreNotify()
        {
            using (new ErrorScope())
            {
                MessageDisplay msgDisplay = CreateMessageDisplay();

                int[] notifyIDs = _Request.GetList<int>("notifyid", Method.All, new int[0]);
                int notifyID = _Request.Get<int>("notifyid", Method.All, 0);

                if (notifyID < 0)
                {
                    SystemNotifyProvider.IgnoreNotify(MyUserID, notifyID);
                }
                else
                {
                    try
                    {
                        if (!NotifyBO.Instance.IgnoreNotifies(My, notifyIDs))
                        {
                            CatchError<ErrorInfo>(delegate(ErrorInfo error)
                            {
                                msgDisplay.AddError(error);
                            }
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        msgDisplay.AddError(ex.Message);
                    }
                }
            }
        }

        protected int IsPostBack
        {
            get;
            set;
        }

        private NotifyCollection m_TopNotifyList;
        protected NotifyCollection TopNotifyList
        {
            get
            {
                if (m_TopNotifyList == null)
                {
                    if (My.SystemNotifys.Count > 5)
                        m_TopNotifyList = new NotifyCollection();
                    else
                    {
                        NotifyCollection notifies = NotifyBO.Instance.GetTopNotifys(MaxLabs.bbsMax.Entities.User.CurrentID, 5, 0);
                        m_TopNotifyList = new NotifyCollection();
                        m_TopNotifyList.TotalRecords = notifies.TotalRecords;

                        int i = 0;
                        foreach (Notify notify in notifies)
                        {
                            m_TopNotifyList.Add(notify);
                            i++;
                            if (My.SystemNotifys.Count + i >= 5)
                                break;
                        }
                    }
                }
                return m_TopNotifyList;
            }
        }

        protected bool IsShowMore
        {
            get
            {
                return My.TotalUnreadNotifies > (My.SystemNotifys.Count + TopNotifyList.Count);
            }
        }
	}
}