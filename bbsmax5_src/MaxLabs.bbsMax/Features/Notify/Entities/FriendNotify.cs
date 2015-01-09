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
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    public class FriendNotify : Notify
    {

        public FriendNotify(Notify notify)
            : base(notify)
        {

        }

        public FriendNotify() { }

        public FriendNotify(int RelateUserID, int TargetFriendGroupID, string PostScript)
        {
            this.RelateUserID = RelateUserID;
            this.TargetGroupID = TargetFriendGroupID;
            this.PostScript = PostScript;
        }

        public override int TypeID
        {
            get
            {
                return (int)FixNotifies.FriendNotify;
            }
        }

        private SimpleUser m_RelateUser;
        public SimpleUser RelateUser
        {
            get
            {
                if (m_RelateUser == null)
                    m_RelateUser = UserBO.Instance.GetSimpleUser(RelateUserID);
                return m_RelateUser;
            }
        }

        public override string Content
        {
            get
            {
                return
                    string.Format("{0}希望能加您为好友。{1}", RelateUser.PopupNameLink, string.IsNullOrEmpty(this.PostScript) ? string.Empty : "并说：" + PostScript);
            }
        }

        public int RelateUserID
        {
            get
            {
                if (this.DataTable == null)
                {
                    MaxLabs.bbsMax.Common.LogHelper.CreateErrorLog(null, "FriendNotifyLog:DataTable IS NULL");
                    return 0;
                }
                else
                {
                    if (this.DataTable.ContainsKey("RelateUserID"))
                        return StringUtil.TryParse<int>(DataTable["RelateUserID"], 0);
                    return 0;
                }
            }
            set
            {
                this.DataTable["RelateUserID"] = value.ToString();

            }
        }

        public override List<NotifyAction> Actions
        {
            get
            {

                return new List<NotifyAction>(
                    new NotifyAction[]{
                    new NotifyAction(
                        "通过请求",
                        Globals.GetVirtualPath(SystemDirecotry.Dialogs, "friend-verify.aspx?notifyid={notifyid}")
                        ,true
                        )
                        }
                    );
            }
        }

        public string PostScript
        {
            get
            {
                if (this.DataTable.ContainsKey("PostScript"))
                    return DataTable["PostScript"];
                return string.Empty;
            }
            set { this.DataTable["PostScript"] = value; }
        }

        public int TargetGroupID
        {
            get
            {
                if (this.DataTable.ContainsKey("TargetGroupID"))
                    return StringUtil.TryParse<int>(DataTable["TargetGroupID"], 0);
                return 0;
            }
            set
            {
                this.DataTable["TargetGroupID"] = value.ToString();
            }
        }

        public override string Keyword
        {
            get
            {
                if (!string.IsNullOrEmpty(base.Keyword))
                    return base.Keyword;
                return string.Format("{0}|{1}", UserID, RelateUserID);
            }
        }
    }
}