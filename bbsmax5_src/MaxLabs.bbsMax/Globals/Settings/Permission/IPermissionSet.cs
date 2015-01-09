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
using System.Collections.Specialized;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public interface IPermissionSet
    {
        string Name { get; }

        string TypeName { get; }


        bool IsManagement { get; }

        bool CanSetDeny { get; }

        /// <summary>
        /// 继承的NodeID
        /// </summary>
        int NodeID { get; set; }

        /// <summary>
        /// 实际的nodeID
        /// </summary>
        int RealNodeID { get; set; }

        bool HasNodeList { get; }

        NodeItemCollection NodeItemList { get; }

        //PermissionSetWithTargetType PermissionSetWithTargetType { get; }

        StringCollection GetPermissionItemNames();

        StringCollection GetPermissionItemNamesWithTarget();

        List<PermissionItem> GetPermissionItems(Role role);

        List<PermissionItem> GetPermissionItemsWithTarget(Role role);

        Dictionary<string, PermissionItem> GetPermissionItemTable(Role role);

        void SetAllPermissionItems(Role role, List<PermissionItem> items, List<PermissionItem> itemsWithTarget);

        Dictionary<string, PermissionItem> GetPermissionItemWithTargetTable(Role role);

    }
}