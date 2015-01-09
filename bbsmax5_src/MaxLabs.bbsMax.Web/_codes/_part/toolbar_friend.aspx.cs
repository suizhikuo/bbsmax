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
using System.Text;
using MaxLabs.bbsMax.Common;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Web.max_pages
{
    public partial class toolbar_friend : PartPageBase
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (_Request.IsClick("adddoing"))
            {
                string content = _Request.Get("content", Method.Post);
                bool success;


                {
                    success = DoingBO.Instance.CreateDoing(AuthUser.CurrentID, IPUtil.GetCurrentIP(), content);
                }
                Response.Clear();
                Response.Write(My.Doing);
                Response.End();
                return;
            }
            this.FriendList = new FriendCollection();
            ProcessRequest();
		}

        protected FriendCollection FriendList
        {
            get;
            set;
        }

        protected FriendGroupCollection GroupList
        {
            get
            {
                return My.FriendGroups;
            }
        }

        public void ProcessRequest()
        {
            AuthUser my = My;
            int friendGroupID;
            bool useDefault = _Request.Get("default", Method.Get, "").ToLower() == "true";
            if (useDefault)
                friendGroupID = my.SelectFriendGroupID;
            else
                friendGroupID = _Request.Get<int>("groupid", Method.Get, -1);

            if (my.SelectFriendGroupID != friendGroupID)
            {
                my.SelectFriendGroupID = friendGroupID;
                try
                {
                    UserBO.Instance.UpdateUserSelectFriendGroupID(my, friendGroupID);
                }
                catch (Exception ex)
                {
                    LogHelper.CreateErrorLog(ex);
                }
            }

            List<int> currentGroupUserIDs = new List<int>();
            FriendCollection friends = FriendBO.Instance.GetFriends(my.UserID);

            Dictionary<int, int> onlineCount = new Dictionary<int, int>();
            foreach (Friend friend in friends)
            {
                if (friend.GroupID == friendGroupID)
                {
                    currentGroupUserIDs.Add(friend.UserID);
                    if (friend.GroupID == my.SelectFriendGroupID)
                        FriendList.Add(friend);
                }

#if !Passport
                if (friend.IsOnline)
                {
                    if (onlineCount.ContainsKey(friend.GroupID))
                    {
                        onlineCount[friend.GroupID] += 1;
                    }
                    else
                        onlineCount.Add(friend.GroupID, 1);
                }
#endif
            }

            foreach (FriendGroup group in my.FriendGroups)
            {
                int count;
                if (
                onlineCount.TryGetValue(group.GroupID, out count))
                {
                    group.OnlineCount = count;
                }
                group.IsSelect = group.GroupID == my.SelectFriendGroupID;
            }

            UserBO.Instance.FillSimpleUsers<Friend>(this.FriendList);

            //SimpleUserCollection users = UserBO.Instance.GetSimpleUsers(currentGroupUserIDs, GetUserOption.Default);
        }
	}
}