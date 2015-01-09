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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public class PostIcon : SettingBase, IPrimaryKey<int>, IComparable<PostIcon>, IBatchSave
    {
        public PostIcon() { }
        internal PostIcon(int id, string iconurl, int order)
        {
            this.IconID = id;
            this.IconUrlSrc = iconurl; this.SortOrder = order;
        }

        [SettingItem]
        public int IconID { get; set; }


        public string IconUrl { get { return UrlUtil.ResolveUrl(this.IconUrlSrc); } }

        [SettingItem]
        public string IconUrlSrc { get; set; }

        [SettingItem]
        public int SortOrder { get; set; }


        public bool IsNew { get; set; }

        public int GetKey()
        {
            return IconID;
        }

        public int CompareTo(PostIcon icon)
        {
            return this.SortOrder.CompareTo(icon.SortOrder);
        }
    }

    public class PostIconCollection : HashedCollectionBase<int, PostIcon>, ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PostIcon item in this)
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
                    PostIcon field = new PostIcon();
                    field.Parse(item);

                    this.Set(field);
                }
            }
        }
    }
}