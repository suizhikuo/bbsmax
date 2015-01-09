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
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public class ForumPermissionSet : SettingBase, IPermissionSetWithNode
    {
        public ForumPermissionSet()
        {
            Nodes = new ForumPermissionSetNodeCollection();
            ForumPermissionSetNode topNode = new ForumPermissionSetNode();
            Nodes.Add(topNode);
        }

        [SettingItem]
        public ForumPermissionSetNodeCollection Nodes { get; set; }


        #region IPermissionSetWithNode 成员

        private string m_TypeName;
        public string TypeName
        {
            get
            {
                if (m_TypeName == null)
                    m_TypeName = this.GetType().Name;
                return m_TypeName;
            }
        }

        public string Name { get { return "版块使用权限"; } }

        public IPermissionSet GetNode(int nodeID)
        {
            ForumPermissionSetNode node = Nodes.GetPermission(nodeID);

            return node;
        }

        public bool SaveSetting(IPermissionSet setting)
        {
            ForumPermissionSetNodeCollection tempNodes = new ForumPermissionSetNodeCollection();

            bool haveAdd = false;
            foreach (ForumPermissionSetNode node in Nodes)
            {
                if (node.NodeID == setting.NodeID)
                {
                    tempNodes.Add((ForumPermissionSetNode)setting);
                    haveAdd = true;
                }
                else
                    tempNodes.Add(node);
            }

            if (haveAdd == false)
            {
                tempNodes.Add((ForumPermissionSetNode)setting);
            }

            ForumPermissionSet permissionSet = new ForumPermissionSet();
            permissionSet.Nodes = tempNodes;

            return SettingManager.SaveSettings(permissionSet);
        }
        public bool DeleteSetting(int nodeID)
        {
            if (nodeID == 0)
                return true;

            ForumPermissionSetNodeCollection tempNodes = new ForumPermissionSetNodeCollection();

            foreach (ForumPermissionSetNode node in Nodes)
            {
                if (node.NodeID != nodeID)
                {
                    tempNodes.Add(node);
                }
            }

            ForumPermissionSet permissionSet = new ForumPermissionSet();
            permissionSet.Nodes = tempNodes;

            return SettingManager.SaveSettings(permissionSet);
        }

        #endregion
    }

}