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
    public class LinkSettings:SettingBase
    {
        public LinkSettings()
        {
            Links = new LinkCollection();
            Links.Add(new Link());
            Links[0].ImageUrlSrc = UrlUtil.JoinUrl(Globals.GetRelativeUrl(SystemDirecotry.Assets_LinkIcon), "bbsmax.gif");
            Links[0].Name = "bbsMax";
            Links[0].Url = "http://www.bbsmax.com";
            Links[0].Index = 0;
            Links[0].ID = 1;
            MaxID = 100;
        }

        [SettingItem]
        public LinkCollection Links
        {
            get; set;
        }

        private LinkCollection m_ImageLinks; 
        public LinkCollection ImageLinks
        {
            get
            {
                LinkCollection imageLinks = m_ImageLinks;

                if (imageLinks == null)
                {
                    imageLinks = new LinkCollection();

                    foreach (Link link in Links)
                    {
                        if (link.IsImage)
                            imageLinks.Add(link);
                    }

                    m_ImageLinks = imageLinks;
                }

                return imageLinks;
            }
        }

        private LinkCollection m_TextLinks;
        public LinkCollection TextLinks
        {
            get
            {
                LinkCollection textLinks = m_TextLinks;

                if (textLinks == null)
                {
                    textLinks = new LinkCollection();

                    foreach (Link link in Links)
                    {
                        if (link.IsImage == false)
                            textLinks.Add(link);
                    }

                    m_TextLinks = textLinks;
                }

                return textLinks;
            }
        }

        /*
        /// <summary>
        /// 排序好的友情链接列表
        /// </summary>
        public LinkCollection SortedLinks
        {
            get
            {
                int position=0;
                LinkCollection linkCollection = new LinkCollection();
                foreach( Link link in Links )
                {
                    position = 0;
                    for (int i = 0; i < linkCollection.Count; i++)
                    {
                        if (linkCollection[i].Index > link.Index)
                        {
                            break;
                        }
                        position ++; 
                    }
                    linkCollection.Insert(position, link);
                }

                return linkCollection;
            }
        }
        */

        [SettingItem]
        public int MaxID
        {
            get;
            //改成public了  升级程序要用  --sek
            set;
        }

        public Link CreateLink()
        {
            Link l = new Link();
            l.ID = ++MaxID;
            l.IsNew = true;
            return l;
        }

        public void AddLink( Link link)
        {
            this.Links.Add(link);
        }

        public void RemoveLink(int linkID)
        {
            RemoveLink( new int[]{ linkID});
        }

        public void RemoveLink(int[] linkIds)
        {
            bool needSave = false;
            foreach(int g in linkIds)
            {
                for(int i=0;i<Links.Count;i++)
                {
                    if( Links[i].LinkID==g )
                    {
                        Links.RemoveAt(i);
                        needSave = true;
                        break;
                    }
                }
            }
            if(needSave)  SettingManager.SaveSettings(this);
        }
    }
}