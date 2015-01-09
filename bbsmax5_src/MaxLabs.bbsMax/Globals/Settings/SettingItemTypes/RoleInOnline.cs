//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MaxLabs.bbsMax.Entities;


namespace MaxLabs.bbsMax.Settings
{
    public sealed class RoleInOnline : SettingBase, IComparable<RoleInOnline>, IPrimaryKey<Guid>, IBatchSave
    {
        private Guid roleID;
        private int sortOrder;

        public RoleInOnline()
        {
            LogoUrlSrc = "";

        }

        public RoleInOnline(Guid roleID, int sortOrder, string name, string logoUrl)
        {
            this.roleID = roleID;
            this.sortOrder = sortOrder;
            this.LogoUrlSrc = logoUrl;
            this.RoleName = name;
        }

        public bool IsNew { get; set; }

        private int id;

        [JsonItem]
        [SettingItem]
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        [JsonItem]
        [SettingItem]
        public Guid RoleID
        {
            get { return roleID; }
            set { roleID = value; }
        }

        [JsonItem]
        public string LogoUrl
        {
            get { return UrlUtil.ResolveUrl(this.LogoUrlSrc); }
        }

        [JsonItem]
        [SettingItem]
        public string LogoUrlSrc { get; set; }

        [JsonItem]
        [SettingItem]
        public int SortOrder
        {
            get { return sortOrder; }
            set { sortOrder = value; }
        }

        [JsonItem]
        [SettingItem]
        public string RoleName { get; set; }


        public Guid GetKey()
        {
            return roleID;
        }

        public bool HasLogo
        {
            get { return !string.IsNullOrEmpty(LogoUrl); }
        }

        public int CompareTo(RoleInOnline roleInOnline)
        {
            return SortOrder.CompareTo(roleInOnline.SortOrder);
        }
    }

    public class RoleInOnlineCollection : HashedCollectionBase<Guid, RoleInOnline>, ISettingItem
    {

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (RoleInOnline item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    RoleInOnline field = new RoleInOnline();
                    field.Parse(item);

                    this.Set(field);
                }

                Sort();
            }
        }

    }
}