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
    /// <summary>
    /// 帖子评分通知
    /// </summary>
    public class RatePostNotify : CommentNotify
    {
        public RatePostNotify() 
        {

        }

        public RatePostNotify(int relateUserID, int ThreadID, string threadSubject, string ForumCode)
        {
            this.RelateUserID = relateUserID;
            this.ForumCode = ForumCode;
            this.TopicSubject = threadSubject;
            this.TargetID = ThreadID;

            this.Url =   BbsUrlHelper.GetThreadUrl(ForumCode, TargetID);
        }

        public string ForumCode
        {
            get { return DataTable["ForumCode"]; }
            set { DataTable["ForumCode"] = value; }
        }

        public string TopicSubject
        {
            get { return DataTable["TopicSubject"]; }
            set { DataTable["TopicSubject"] = value; }
        }


        public override string Content
        {
            get
            {
                if (m_Content == null)
                {
                    m_Content = string.Format(@"有人给您的帖子“<a href=""{0}""  target=""_top"">{1}</a>”评分了，去看看吧！", HandlerUrl, TopicSubject);
                }
                return m_Content;
            }
        }

    }

    /// <summary>
    /// 撤销评分的通知
    /// </summary>
    public class CancelRateNotify : AdminManageNotify
    {
        public CancelRateNotify()
        {

        }


        public CancelRateNotify(int relateUserID, string forumCode, int postID, string reson)
            : base(relateUserID, string.Empty)
        {
            this.ForumCode = forumCode;
            this.Reson = reson;

           
            Url = BbsUrlHelper.GetThreadUrl(ForumCode, TargetID);
        }

        public string ForumCode
        {
            get;
            set;
        }

        public string Reson
        {
            get { return DataTable["Reson"]; }
            set { DataTable["Reson"] = value; }
        }

        public int TargetID
        {
            get 
            {
                return DataTable.ContainsKey("TargetID") ?
                    StringUtil.TryParse<int>(DataTable["TargetID"]) : 0;
            }
            set
            {
                DataTable["TargetID"] = value.ToString();
            }
        }

        public override string Content
        {
            get
            {
                if (m_Content == null)
                {
                    m_Content = string.Format(@"您的帖子被管理员撤消评分<br />被撤消评分原因：<br />{0}<br /><a href=""{1}"">详情查看</a>", Reson, HandlerUrl);

                }
                return m_Content;
            }
        }
    }
}