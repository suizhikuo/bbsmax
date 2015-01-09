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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Settings
{
    public class NavigationItem : SettingBase, IPrimaryKey<int>, IComparable<NavigationItem>, IBatchSave
    {
        public NavigationItem()
        {
            IsTopLink = false;
        }


        /// <summary>
        /// 名称
        /// </summary>
        [SettingItem]
        public string Name { get; set; }


        [SettingItem]
        public NavigationType Type { get; set; }


        private string m_UrlInfo;
        /// <summary>
        /// 地址
        /// </summary>
        [SettingItem]
        public string UrlInfo
        {
            get { return m_UrlInfo; }
            set 
            { 
                m_UrlInfo = value;
                m_ForumID = null;
            }
        }


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
        public int ID { get; set; }

        [SettingItem]
        public int ParentID { get; set; }


        [SettingItem]
        public bool IsTopLink { get; set; }

        public int GetKey() { return ID; }


        private NavigationItemCollection m_ChildItems;
        public NavigationItemCollection ChildItems
        {
            get
            {
                if (m_ChildItems == null)
                {
                    if (IsTopLink)
                        m_ChildItems = AllSettings.Current.TopLinkSettings2.GetChildItems(ID);
                    else
                        m_ChildItems = AllSettings.Current.NavigationSettings.GetChildItems(ID);
                }
                return m_ChildItems;
            }

        }

        private NavigationItemCollection m_GuestChildItems;
        public NavigationItemCollection GuestChildItems
        {
            get
            {
                if (m_GuestChildItems == null)
                {
                    m_GuestChildItems = new NavigationItemCollection();
                    foreach (NavigationItem item in ChildItems)
                    {
                        if (item.OnlyLoginCanSee == false)
                            m_GuestChildItems.Add(item);
                    }
                }
                return m_GuestChildItems;
            }
        }

        public void ClearCache()
        {
            m_GuestChildItems = null;
            m_ChildItems = null;
            m_ForumID = null;
            m_ForumCodeName = null;
        }

        private int? m_ForumID;
        public int ForumID 
        {
            get
            {
                if (Type == NavigationType.Forum)
                {
                    if (m_ForumID == null)
                    {
                        int id;
                        if (int.TryParse(UrlInfo, out id))
                            m_ForumID = id;

                        else
                            m_ForumID = 0;
                    }

                    return m_ForumID.Value;
                }
                else
                    return 0;
                
            }
        }

        private string m_ForumCodeName;
        public string Url
        {
            get
            {
                if (Type == NavigationType.Custom)
                    return UrlInfo;
                else if (Type == NavigationType.Internal)
                    return NavigationSettings.GetInternalUrl(UrlInfo);
                else
                {
                    if (string.IsNullOrEmpty(m_ForumCodeName))
                    {
                        m_ForumCodeName = ForumBO.Instance.GetCodeName(ForumID);
                    }

                    if (m_ForumCodeName == string.Empty)
                    {
                        return MaxLabs.bbsMax.Common.BbsUrlHelper.GetForumUrl("ForumID-" + ForumID);
                        //return "ID为" + ForumID + "的版块不存在";
                    }

                    return MaxLabs.bbsMax.Common.BbsUrlHelper.GetForumUrl(m_ForumCodeName);
                }
            }
        }

        public int CompareTo(NavigationItem item)
        {
            return this.SortOrder.CompareTo(item.SortOrder);
        }

        #region IBatchSave 成员

        public bool IsNew
        {
            get;
            set;
        }

        #endregion

    }

    public class NavigationItemCollection : EntityCollectionBase<int, NavigationItem>, ISettingItem
    {
        public string GetValue()
        {
            StringList list = new StringList();

            foreach (NavigationItem item in this)
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
                    NavigationItem field = new NavigationItem();
                    field.Parse(item);

                    this.Set(field);
                }

                Sort();
            }
        }
    }
}