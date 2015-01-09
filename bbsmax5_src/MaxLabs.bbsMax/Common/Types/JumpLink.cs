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
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax
{
    public class JumpLink
    {
        private string m_Link;

        public string Link
        {
            get { return m_Link; }
            set { m_Link = StringUtil.GetSafeFormText(value); }
        }

        public string Text { get; set; }

        public bool IsSystem { get; set; }
    }

    public class JumpLinkCollection : Collection<JumpLink>
    {

        public void Add(string text, string link)
        {
            JumpLink jumpLink = new JumpLink();
            jumpLink.Text = text;
            jumpLink.Link = link;
            base.Add(jumpLink);
        }

        public JumpLinkCollection() { }
        public JumpLinkCollection(string text,string url) { Add(text,url); }

        public void Add(string text, string link, bool isSystem)
        {
            JumpLink jumpLink = new JumpLink();
            jumpLink.Text = text;
            jumpLink.Link = link;
            jumpLink.IsSystem = true;
            base.Add(jumpLink);
        }
    }
}