//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;

using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    public class Emoticon : IEmoticonBase
    {
        private bool loadFromDB;
        public Emoticon()
        { }

        public Emoticon(DataReaderWrap readerWrap)
        {
            this.EmoticonID = readerWrap.Get<int>("EmoticonID");
            this.ImageSrc = readerWrap.Get<string>("ImageSrc");
            this.FileSize = readerWrap.Get<int>("FileSize");
            this.MD5 = readerWrap.Get<string>("MD5");
            this.SortOrder = readerWrap.Get<int>("SortOrder");
            this.GroupID = readerWrap.Get<int>("GroupID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.Shortcut = readerWrap.Get<string>("Shortcut");

            if (ImageSrc.IndexOf('/') != -1 || ImageSrc.IndexOf('\\') != -1)
            {
                ImageUrl = UrlUtil.ResolveUrl(ImageSrc);
            }
            else
            {
                ImageUrl = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Upload_Emoticons), ImageSrc.Substring(0, 1), ImageSrc.Substring(1, 1), ImageSrc);
                ImageSrc = UrlUtil.JoinUrl(Globals.GetRelativeUrl(SystemDirecotry.Upload_Emoticons), ImageSrc.Substring(0, 1), ImageSrc.Substring(1, 1), ImageSrc);
            }

            loadFromDB = true;
        }

        public int GroupID { get; set; }

        public int UserID { get; set; }

        /// <summary>
        /// 缩略图的位置
        /// </summary>
        private string m_ThunmbnailFilePath;
        public string ThunmbnailFilePath
        {
            get
            {
                if (m_ThunmbnailFilePath == null)
                {
                    m_ThunmbnailFilePath = EmoticonBO.Instance.GetThumbFilePath(this.ImageUrl, true);
                }
                return m_ThunmbnailFilePath;
            }
        }

        public int EmoticonID { get; private set; }

        private string m_Shortcut;

        public string Shortcut
        {
            get
            {
                if (string.IsNullOrEmpty(m_Shortcut) && loadFromDB)
                {
                    m_Shortcut = string.Format("{{face:{0}}}", this.EmoticonID);
                }
                else if (loadFromDB && (!m_Shortcut.StartsWith("{") || !m_Shortcut.EndsWith("}")))
                {
                    m_Shortcut = string.Format("{{{0}}}", m_Shortcut);
                }
                return m_Shortcut;
            }
            set
            {
                m_Shortcut = value;
            }
        }

        public string ImageSrc { get; set; }

        public string ImageUrl { get; private set; }

        public string MD5 { get; set; }

        public int FileSize { get; set; }

        public int SortOrder { get; private set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.EmoticonID;
        }

        #endregion
    }

    //默认表情不能多重继承， 所以才用接口
    public interface IEmoticonBase : IPrimaryKey<int>
    {
        string Shortcut { get; }
        string ImageUrl { get; }
        string ThunmbnailFilePath { get; }
        string ImageSrc { get; }
        int GroupID { get; }
        int EmoticonID { get; }
    }

    public class EmoticonCollection : EntityCollectionBase<int, Emoticon>
    {
        public EmoticonCollection() { }

        public EmoticonCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                this.Add(new Emoticon(readerWrap));
            }
        }
    }

}