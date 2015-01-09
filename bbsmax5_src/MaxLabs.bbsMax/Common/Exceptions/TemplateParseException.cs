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

namespace MaxLabs.bbsMax
{
    public class TemplateParseException : Exception
    {
        private string m_FilePath;
        private string m_Template;
        private int m_Index;

        public TemplateParseException(string message, string filePath, string template, int index) : base(message)
        {
            m_FilePath = filePath;
            m_Template = template;
            m_Index = index;
        }

        public string FilePath { get { return m_FilePath; } }

        public string Template { get { return m_Template; } }

        public int Index { get { return m_Index; } }

    }
}