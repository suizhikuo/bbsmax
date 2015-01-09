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
    public sealed class MedalLevel : SettingBase
	{
        public MedalLevel()
        {
            Name = string.Empty;
            IconSrc = string.Empty;
        }

        [SettingItem]
        public int ID { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        [SettingItem]
        public string Name { get; set; }

        /// <summary>
        /// 等级图标 
        /// </summary>
        [SettingItem]
        public string IconSrc { get; set; }

        /// <summary>
        /// 此等级必须达到的值
        /// </summary>
        [SettingItem]
        public int Value { get; set; }

        /// <summary>
        /// 如果是手动点亮  说明点亮条件
        /// </summary>
        [SettingItem]
        public string Condition { get; set; }

        public string LogoUrl
        {
            get
            {
                return UrlUtil.ResolveUrl(IconSrc);
            }
        }

    }

    public class MedalLevelCollection : Collection<MedalLevel>, ISettingItem
	{
        public new void Add(MedalLevel item)
        {
            Add(item,true);
        }

        public void Add(MedalLevel item,bool sort)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].ID == item.ID)
                {
                    return;
                }
            }
            if (false == sort)
            {
                base.Add(item);
                return;
            }

            int index = 0;
            for (int i = 1; i < this.Count + 1; i++)
            {
                if (item.Value > this[i - 1].Value)
                {
                    index = i;
                }
                if (item.Value < this[i - 1].Value)
                    break;
            }
            if (index >= this.Count)
            {
                base.Add(item);
            }
            else
                base.Insert(index, item);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (MedalLevel item in this)
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
                    MedalLevel medalLevelItem = new MedalLevel();

                    medalLevelItem.Parse(item);
                    this.Add(medalLevelItem);

                }
            }
        }

        #endregion
    }
}