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

namespace PanGu.HighLight
{
    public class SimpleHTMLFormatter : Formatter   
    {
        string _PreTag = "<font color=\"red\">";
        string _PostTag = "</font>";

        #region Public properties

        /// <summary>
        /// Get or set prefix gag.
        /// Default: "<font color=\"red\">"
        /// </summary>
        public string PreTag
        {
            get
            {
                return _PreTag;
            }

            set
            {
                _PreTag = value;
            }
        }

        /// <summary>
        /// Get or set postfix tag.
        /// Default:"</font>"
        /// </summary>
        public string PostTag
        {
            get
            {
                return _PostTag;
            }

            set
            {
                _PostTag = value;
            }
        }

        #endregion

        public SimpleHTMLFormatter()
        {
           
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="preTag">prefix tag</param>
        /// <param name="postTag">postfix tag</param>
        public SimpleHTMLFormatter(string preTag, string postTag)
        {
            _PreTag = preTag;
            _PostTag = postTag;
        }

        #region Formatter Members

        public string HighlightTerm(string originalText)
        {
            return PreTag + originalText + PostTag;
        }

        #endregion
    }
}