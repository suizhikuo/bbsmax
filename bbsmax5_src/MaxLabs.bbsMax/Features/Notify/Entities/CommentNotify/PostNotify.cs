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
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Entities
{
    public class PostNotify : CommentNotify
    {
        public PostNotify(string TopicSubject, int RelateUserID, int ThreadID, string forumCode)
        {
            this.ForumAliasName = forumCode; 
            this.TopicSubject = TopicSubject;
            this.RelateUserID = RelateUserID;
            this.TargetID = ThreadID;

            Url = BbsUrlHelper.GetThreadUrl(ForumAliasName, TargetID);
        }

        public override int TypeID
        {
            get
            {
                return (int)FixNotifies.CommentNotify;
            }
        }

        public override string ObjectName
        {
            get
            {
                return "主题";
            }
        }

        public override string Content
        {
            get
            {
                if (m_Content == null)
                {
                    m_Content = string.Format("有人回复了您的帖子“<a href=\"{0}\" target=\"_top\">{1}</a>”，快去看看吧！ ",  HandlerUrl, TopicSubject);
                }
                return m_Content;
            }
        }

        public string TopicSubject
        {
            get { return DataTable["TopicSubject"]; }
            set { DataTable["TopicSubject"] = value; }
        }

        public string ForumAliasName
        {
            get { return DataTable["ForumAliasName"]; }
            set { DataTable["ForumAliasName"] = value; }
        }
    }
}