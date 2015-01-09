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
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public class Link : SettingBase, IPrimaryKey<int>, IComparable<Link>, IBatchSave
    {
        public Link()
        {

        }

        /// <summary>
        /// 名称
        /// </summary>
        [SettingItem]
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [SettingItem]
        public string Url { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        public string ImageUrl { get { return UrlUtil.ResolveUrl(this.ImageUrlSrc); } }

        [SettingItem]
        public string ImageUrlSrc { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SettingItem]
        public int Index { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [SettingItem]
        public int LinkID { get; set; }

        public int ID { get { return LinkID; } set { LinkID = value; } }

        public bool IsImage { get { return !string.IsNullOrEmpty(this.ImageUrl); } }

        /// <summary>
        /// 描述
        /// </summary>
        [SettingItem]
        public string Description { get; set; }

        public int GetKey() { return LinkID; }

        public bool IsNew { get; set; }

        public int CompareTo(Link link)
        {
            if (this.IsImage)
            {
                if (link.IsImage)
                {
                    return this.Index.CompareTo(link.Index);
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (link.IsImage)
                    return 1;
                else
                    return this.Index.CompareTo(link.Index);
            }
        }
    }

    public class LinkCollection : EntityCollectionBase<int, Link>, ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (Link item in this)
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
                    Link field = new Link();
                    field.Parse(item);

                    this.Set(field);
                }

                Sort();
            }
        }
    }
}