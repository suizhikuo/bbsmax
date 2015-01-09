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
    public  class CommentNotify : Notify
    {
        public CommentNotify(Notify notify)
            :base(notify) { }


        public CommentNotify() { }

        public CommentNotify(int commentID, bool isReply) 
        {
            CommentID = commentID;
            IsReply = isReply;
        }




        public int TargetID {
            get
            {
                if (DataTable.ContainsKey("TargetID"))
                {
                    return StringUtil.TryParse<int>(DataTable["TargetID"], 0);
                }
                return 0;
            }
            set
            {
                DataTable["TargetID"] = value.ToString();
            }
        }


        public int RelateUserID
        {
            get
            {
                if (DataTable.ContainsKey("RelateUserID"))
                {
                    return StringUtil.TryParse<int>(DataTable["RelateUserID"], 0);
                }
                return 0;
            }
            set
            {
                DataTable["RelateUserID"] = value.ToString();
            }
        }

        public override int TypeID
        {
            get
            {
                return (int)FixNotifies.CommentNotify;
            }
        }

        public virtual string Title
        {
            get;
            set;
        }

        public override string Content
        {
            get
            {
                if (m_Content == null)
                {
                    if (!IsReply)
                    {
                        if (!string.IsNullOrEmpty(Title))
                        {
                            m_Content = string.Format(@"评论了您的{0}：<a href=""{1}"" target=""_top"">{2}</a>", ObjectName, HandlerUrl, Title);
                        }
                        else
                        {
                            m_Content = string.Format(@"有用户评论了您的{0}，<a href=""{1}"" target=""_top"">{2}</a>", ObjectName, HandlerUrl, "点击这里查看");
                        }
                    }
                    else
                    {
                        m_Content = string.Format(@"有用户回复了您说的话，<a href=""{0}"" target=""_top"">{1}</a>", HandlerUrl, "点击这里查看");
                    }
                }
                return m_Content;
            }
        }

        public bool IsReply
        {
            get;
            protected set;
        }

        public int CommentID
        {
            get
            {
                if (DataTable.ContainsKey("CommentID"))
                {
                    return StringUtil.TryParse<int>(DataTable["CommentID"], 0);
                }
                return 0;
            }
            protected  set
            {
                DataTable["CommentID"] = value.ToString();
            }
        }

        public virtual string ObjectName
        {
            get;
            private set;
        }

        public override string Keyword
        {
            get
            {
                if (string.IsNullOrEmpty(base.Keyword))
                    return string.Format( "{0}|{1}|{2}",TargetID, ObjectName,IsReply);
                return base.Keyword;
            }
        }
    }
}