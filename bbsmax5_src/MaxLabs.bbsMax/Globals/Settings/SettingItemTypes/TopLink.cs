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
    public class TopLink : SettingBase, IPrimaryKey<int>, IComparable<TopLink>, IBatchSave
    {
        public TopLink()
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
        /// 排序
        /// </summary>
        [SettingItem]
        public int SortOrder { get; set; }


        /// <summary>
        /// 仅登陆可见
        /// </summary>
        [SettingItem]
        public bool OnlyLoginCanSee { get; set; }


        /// <summary>
        /// 新窗口打开
        /// </summary>
        [SettingItem]
        public bool NewWindow { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [SettingItem]
        public int LinkID { get; set; }

        public int GetKey() { return LinkID; }


        public int CompareTo(TopLink link)
        {
            return this.SortOrder.CompareTo(link.SortOrder);
        }

        #region IBatchSave 成员

        public bool IsNew
        {
            get;
            set;
        }

        #endregion

    }

    public class TopLinkCollection : EntityCollectionBase<int, TopLink>, ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (TopLink item in this)
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
                    TopLink field = new TopLink();
                    field.Parse(item);

                    this.Set(field);
                }

                if (this.ContainsKey(-1) == false)
                {
                    TopLink link = new TopLink();
                    link.LinkID = -1;
                    link.Name = "版块导航";
                    link.OnlyLoginCanSee = false;
                    link.SortOrder = -2;
                    link.Url = "/default.aspx";

                    this.Add(link);
                }
                if (this.ContainsKey(-2) == false)
                {
                    TopLink link = new TopLink();
                    link.LinkID = -2;
                    link.Name = "个人空间";
                    link.OnlyLoginCanSee = true;
                    link.SortOrder = -1;
                    link.Url = MaxLabs.bbsMax.Common.UrlHelper.GetSpaceUrlTag("$MyUserID");
                    this.Add(link);
                }

                Sort();
            }
        }
    }
}