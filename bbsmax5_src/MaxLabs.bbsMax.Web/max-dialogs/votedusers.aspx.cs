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
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class votedusers : DialogPageBase
    {
        protected PollItemDetailsCollectionV5 PollItemDetailList;

        protected int VoteTotalCount = 0;
        protected PollThreadV5 poll;
        protected void Page_Load(object sender, EventArgs e)
        {
            int threadID = _Request.Get<int>("ThreadID", Method.Get, 0);
            if (threadID <= 0)
            {
                ShowError(new InvalidParamError("ThreadID").Message);
            }

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);
            if (thread is PollThreadV5)
            { }
            else
            {
                ShowError("该主题不是投票帖");
            }

            poll = (PollThreadV5)thread;

            //SetPageTitle("投票详细情况");

            if (!AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(poll.ForumID).Can(My, ForumPermissionSetNode.Action.ViewPollDetail))
                ShowError("您所在的用户组没有权限查看详细投票情况！");

            if (thread == null)
                ShowError("主题不存在！");



            if (MyUserID == thread.PostUserID || AllSettings.Current.ForumPermissionSet.Nodes.GetPermission(poll.ForumID).Can(My, ForumPermissionSetNode.Action.AlwaysViewContents) || poll.AlwaysEyeable || poll.IsVoted(MyUserID))
            { }
            else
            {
                ShowError("需要投票后才能查看详细投票情况！");
            }

            PollItemDetailList = PostBOV5.Instance.GetPollItemDetails(threadID);

            foreach (PollItem pi in poll.PollItems)
            {
                VoteTotalCount += pi.PollItemCount;
            }
        }

        protected double GetPercent(int count, int total)
        {
            return MathUtil.GetPercent(count, total);
        }

        private Dictionary<int, PollItemDetailsCollectionV5> pollItemDetails = new Dictionary<int, PollItemDetailsCollectionV5>();
        protected PollItemDetailsCollectionV5 GetPollItemDetails(int itemID)
        {
            PollItemDetailsCollectionV5 details;
            if (pollItemDetails.TryGetValue(itemID, out details) == false)
            {
                details = new PollItemDetailsCollectionV5();
                foreach (PollItemDetailsV5 detail in PollItemDetailList)
                {
                    if (detail.ItemID == itemID)
                    {
                        details.Add(detail);
                    }
                }
                pollItemDetails.Add(itemID, details);
            }

            return details;
        }

    }
}