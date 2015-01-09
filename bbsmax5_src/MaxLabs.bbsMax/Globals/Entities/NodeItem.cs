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

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Entities
{
    public class NodeItem : IPrimaryKey<int>
    {

        public NodeItem()
        { }

        public int NodeID { get; set; }

        public int ParentID { get; set; }

        public string Name { get; set; }



        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return NodeID;
        }

        #endregion
    }

    public class NodeItemCollection : EntityCollectionBase<int, NodeItem>
    {


        /// <summary>
        /// 输出树形结构
        /// </summary>
        public string GetTreeHtml(string style, string itemStyle, string linkClassName, string currentLinkClassName, int currentNodeID, string url)
        {
            string tree = GetTree(style, itemStyle, 0, currentNodeID, linkClassName, currentLinkClassName, url);
            //if (tree.StartsWith("<ul>"))
            //    tree = tree.Substring(4);
            //if (tree.EndsWith("</ul>"))
            //    tree = tree.Substring(0, tree.Length - 5);
            return tree;
        }

        private string GetTree(string style, string itemStyle, int parentID, int currentNodeID, string linkClassName, string currentLinkClassName, string url)
        {
            StringBuilder sb = new StringBuilder();
            foreach (NodeItem item in this)
            {
                if (item.ParentID != parentID)
                    continue;

                string linkClass = "";

                string subItemString = string.Empty;
                subItemString = GetTree(style, itemStyle, item.NodeID, currentNodeID, linkClassName, currentLinkClassName, url);

                if (item.NodeID == currentNodeID)
                    linkClass = currentLinkClassName;
                else
                    linkClass = linkClassName;

                sb.Append(string.Format(itemStyle, item.NodeID, item.Name, subItemString, linkClass, BbsRouter.GetUrl(url)));
            }
            if (sb.Length > 0)
                return string.Format(style, sb.ToString());
            else
                return string.Empty;
        }
    }
}