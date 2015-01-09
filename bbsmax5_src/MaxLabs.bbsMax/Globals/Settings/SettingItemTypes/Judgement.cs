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
    /// <summary>
    /// 主题鉴定实体类
    /// </summary>
    public class Judgement : SettingBase, IPrimaryKey<int>, IBatchSave
    {
        public Judgement()
        {

        }

        internal Judgement(int id, string description, string logourl)
        {
            this.ID = id;
            this.Description = description;
            this.LogoUrlSrc = logourl;
        }

        /// <summary>
        /// 鉴定ID
        /// </summary>
        [SettingItem]
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// 鉴定描述
        /// </summary>
        [SettingItem]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string LogoUrl
        {
            get { return UrlUtil.ResolveUrl(this.LogoUrlSrc); }
        }

        [SettingItem]
        public string LogoUrlSrc
        {
            get;
            set;
        }


        public bool IsNew
        {
            get;
            set;
        }

        public int GetKey()
        {
            return ID;
        }
    }

    public class JudgementCollection : HashedCollectionBase<int, Judgement>, ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (Judgement item in this)
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
                    Judgement field = new Judgement();
                    field.Parse(item);

                    this.Set(field);
                }
            }
        }
    }
}