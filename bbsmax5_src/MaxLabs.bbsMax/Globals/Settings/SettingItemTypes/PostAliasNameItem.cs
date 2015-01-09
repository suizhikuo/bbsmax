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

    public class PostAliasNameItem : SettingBase, IPrimaryKey<int>, IBatchSave
    {
        public PostAliasNameItem() { }
        public PostAliasNameItem(int postIndex, string name)
        {
            this.PostIndex = postIndex;
            this.AliasName = name;
        }

        [SettingItem]
        public int PostIndex
        {
            get;
            set;
        }

        public bool IsNew { get; set; }

        [SettingItem]
        public string AliasName
        {
            get;
            set;
        }

        public int GetKey()
        {
            return this.PostIndex;
        }
    }

    public class PostAliasNameCollection : HashedCollectionBase<int, PostAliasNameItem>,ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (PostAliasNameItem item in this)
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
                    PostAliasNameItem field = new PostAliasNameItem();
                    field.Parse(item);
                    this.Set(field);
                }
            }
        }
    }
}