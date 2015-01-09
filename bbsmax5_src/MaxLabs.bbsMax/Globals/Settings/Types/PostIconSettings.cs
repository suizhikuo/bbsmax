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

namespace MaxLabs.bbsMax.Settings
{
    public  class PostIconSettings:SettingBase
    {
        public PostIconSettings()
        {
            int index = 1;
            string iconPath = Globals.GetRelativeUrl(SystemDirecotry.Assets_PostIcon);
            PostIcons = new PostIconCollection();
            PostIcons.Add(new PostIcon(1, UrlUtil.JoinUrl(iconPath, "icon1.gif"), index++));
            PostIcons.Add(new PostIcon(2, UrlUtil.JoinUrl(iconPath, "icon2.gif"), index++));
            PostIcons.Add(new PostIcon(3, UrlUtil.JoinUrl(iconPath, "icon3.gif"), index++));
            PostIcons.Add(new PostIcon(4, UrlUtil.JoinUrl(iconPath, "icon4.gif"), index++));
            PostIcons.Add(new PostIcon(5, UrlUtil.JoinUrl(iconPath, "icon5.gif"), index++));
            PostIcons.Add(new PostIcon(6, UrlUtil.JoinUrl(iconPath, "icon6.gif"), index++));
            PostIcons.Add(new PostIcon(7, UrlUtil.JoinUrl(iconPath, "icon7.gif"), index++));
            PostIcons.Add(new PostIcon(8, UrlUtil.JoinUrl(iconPath, "icon8.gif"), index++));
            PostIcons.Add(new PostIcon(9, UrlUtil.JoinUrl(iconPath, "icon9.gif"), index++));
            this.MaxId = PostIcons.Count;

            EnablePostIcon = true;
        }


        [SettingItem]
        public bool EnablePostIcon
        {
            get;
            set;
        }


        [SettingItem]
        public int MaxId
        {
            get;
            set;
        }
        
        [SettingItem]
        public PostIconCollection PostIcons
        {
            get;
            set;
        }

        public PostIcon GetPostIcon(int posticonId)
        {
            return this.PostIcons.GetValue(posticonId);
        }

        public void DeleteIcon( int iconId )
        {
            this.PostIcons.RemoveByKey(iconId);
            //if (this.PostIcons.ContainKey(iconId))
            //{
            //    this.PostIcons.Remove(PostIcons.GetValue(iconId));
            //}
        }

        private PostIcon[] m_sortedIcons;
        public PostIcon[] SortedIcons
        {
            get
            {
                if (m_sortedIcons == null)
                {
                    PostIcon[] icons = new PostIcon[this.PostIcons.Count];
                    this.PostIcons.CopyTo(icons, 0);
                    Array.Sort<PostIcon>(icons);
                  m_sortedIcons = icons;
                }
                return m_sortedIcons;
            }
        }

        public void DeleteIcons(IEnumerable<int> iconids)
        {
            foreach (int id in iconids)
            {
                DeleteIcon(id);
            }
        }

        public void AddIcon(PostIcon icon )
        {
            this.PostIcons.Add(icon);
        }

        public PostIcon CreatePostIcon()
        {
            PostIcon icon = new PostIcon();
            icon.IconID = ++MaxId;
            icon.IsNew = true;
            return icon;
        }
    }
}