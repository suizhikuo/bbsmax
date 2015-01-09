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
using System.Collections;

namespace MaxLabs.bbsMax.Settings
{
    public class PermissionLimit : ISettingItem
    {
        public PermissionLimit(PermissionLimitType limitType)
        {
            LimitType = limitType;
            ExcludeRoles = new Dictionary<Guid, List<Guid>>(); //new List<Guid>();
        }

        public PermissionLimitType LimitType { get;set; }//private set 被去掉

        public Dictionary<Guid, List<Guid>> ExcludeRoles { get; private set; }



        #region ISettingItem 成员

        public string GetValue()
        {
            StringTable table = new StringTable();

            //table.Add("LimitType", LimitType.ToString());
            table.Add("LimitType", ((byte)LimitType).ToString());


            foreach (KeyValuePair<Guid, List<Guid>> item in ExcludeRoles)
            {

                if (item.Value != null)
                {

                    StringList roles = new StringList();
                    foreach (Guid roleID in item.Value)
                    {
                        roles.Add(roleID.ToString());
                    }

                    table.Add("ExcludeRoles-" + item.Key.ToString("N"), roles.ToString());

                }

            }

            return table.ToString();
        }

        public void SetValue(string value)
        {
            StringTable table = StringTable.Parse(value);

            Dictionary<Guid, List<Guid>> excludeRoles = new Dictionary<Guid, List<Guid>>();

            foreach (DictionaryEntry entry in table)
            {
                string key = entry.Key.ToString();

                if ( key.Equals("LimitType", StringComparison.OrdinalIgnoreCase))
                {
                    LimitType = (PermissionLimitType)Convert.ToByte(entry.Value);
                }

                else if (key.StartsWith("ExcludeRoles-" , StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        Guid tempRoleID = new Guid(key.Substring(13));

                        excludeRoles.Add(tempRoleID, new List<Guid>());

                        StringList roleIds = StringList.Parse(entry.Value.ToString());

                        foreach (string roleID in roleIds)
                            excludeRoles[tempRoleID].Add(new Guid(roleID));

                        ExcludeRoles = excludeRoles;
                    }
                    catch { }

                }
            }
        }

        #endregion
    }


    public enum PermissionLimitType : byte
    {
        Unlimited = 0,

        RoleLevelLowerMe = 1,

        RoleLevelLowerOrSameMe = 2,
        
        ExcludeCustomRoles = 5

    }
}