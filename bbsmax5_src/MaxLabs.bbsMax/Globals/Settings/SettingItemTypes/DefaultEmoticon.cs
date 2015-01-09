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
using System.IO;
using System.Web.Caching;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

using MaxLabs.bbsMax.Entities;

//Emoticon
//DefaultEmoticon
namespace MaxLabs.bbsMax.Settings
{
    public class DefaultEmoticon : SettingBase, IEmoticonBase
    {

        public DefaultEmoticon(DefaultEmoticonGroup group) { this.Group = group; }

        [SettingItem]
        public int SortOrder { get; set; }

        private string m_shortcout;

        [SettingItem]
        public string Shortcut
        {
            get
            {
                if (m_shortcout == null)
                    m_shortcout = this.Group.AllotShortcut(this);
                return m_shortcout;
            }
            set
            {
                m_shortcout = value;
            }
        }

        public DefaultEmoticonGroup Group { get; set; }

        public string FilePath { get { return IOUtil.JoinPath(Group.FilePath, this.FileName); } }

        [SettingItem]
        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string ImageSrc
        {
            get
            {
                return UrlUtil.JoinUrl(this.Group.Url, this.FileName);
            }
        }

        public int GroupID
        {
            get
            {
                return this.Group.GroupID;
            }
        }

        [JsonItem]
        public string ImageUrl
        {
            get
            {
                return UrlUtil.ResolveUrl(this.ImageSrc);
            }
        }

        public string ThunmbnailFilePath { get { return this.Group.PreviewUrl; } }

        private int m_emoticonid = 0;
        public int EmoticonID
        {
            get
            {

                if (m_emoticonid == 0)
                    m_emoticonid = this.FileName.GetHashCode();
                return m_emoticonid;
            }
            set { }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.EmoticonID;
        }

        #endregion
    }

    public class DefaultEmoticonCollection : EntityCollectionBase<int, DefaultEmoticon>, ISettingItem
    {

        public DefaultEmoticonCollection(DefaultEmoticonGroup ownerGroup) { this.OnwerGroup = ownerGroup; }

        public string GetValue()
        {
            StringList list = new StringList();
            foreach (DefaultEmoticon item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public DefaultEmoticonGroup OnwerGroup { get; set; }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    DefaultEmoticon field = new DefaultEmoticon(this.OnwerGroup);
                    field.Parse(item);
                    //field.Group = this.OnwerGroup;
                    this.Add(field);
                }
            }
        }
    }

    public class DefaultEmoticonGroup : EmoticonGroupBase, ISettingItem, IDisposable, IPrimaryKey<int>
    {
        public static readonly string PreviewFileName = "_face_.jpg";
        private string Shortcutfix = "[em:{0}]";
        private string ShortFormat = "{0}/{1}";

        private string m_directoryName;

        /// <summary>
        /// 文件夹是否处于监视状态
        /// </summary>
        public bool IsWatching { get; set; }

        [SettingItem]
        public string DirectoryName
        {
            get { return m_directoryName; }
            set { m_directoryName = value; }
        }

        [SettingItem]
        public bool Disabled { get; set; }

        public DefaultEmoticonGroup()
        {
            this.m_Emoticons = new DefaultEmoticonCollection(this);
        }

        public DefaultEmoticonGroup(string path)
        {
            this.DirectoryName = path;
            this.GroupName = string.IsNullOrEmpty(path) ? "默认分组" : path;
            init();
        }

        public override int TotalEmoticons
        {
            get
            {
                if (m_Emoticons == null) return 0;
                return m_Emoticons.Count;
            }
        }

        private DefaultEmoticonCollection m_Emoticons = null;

        private DefaultEmoticonCollection m_sortedemoticons;

        private static object locker = new object();
        [SettingItem]
        public DefaultEmoticonCollection Emoticons
        {
            get
            {
                if (IsWatching && watcher.HasChanged)//是否在监视文件夹
                {
					lock (locker)
					{
						if (IsWatching && watcher.HasChanged)//是否在监视文件夹
						{
							init();
						}
					}
                }

                if (m_sortedemoticons == null)
                {
                    lock (locker)
                    {
                        if (m_sortedemoticons == null)
                        {
                            int index = 0;
                            m_sortedemoticons = new DefaultEmoticonCollection(this);
                            foreach (DefaultEmoticon emot in this.m_Emoticons)
                            {
                                index = 0;
                                for (int i = 0; i < m_sortedemoticons.Count; i++)
                                {
                                    if (m_sortedemoticons[i].SortOrder > emot.SortOrder)
                                        break;
                                    index++;
                                }
                                m_sortedemoticons.Insert(index, emot);
                            }
                        }
                    }
                }

                return m_sortedemoticons;
            }
        }

        private string m_PreviewUrl;
        public override string PreviewUrl
        {
            get
            {
                if (m_PreviewUrl == null)
                    if (!string.IsNullOrEmpty(this.DirectoryName))
                        m_PreviewUrl = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Assets_Face), this.DirectoryName, PreviewFileName);
                    else
                        m_PreviewUrl = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Assets_Face), PreviewFileName);

                return m_PreviewUrl;
            }
        }

        private string PreviewFilePath
        {
            get
            {
                return IOUtil.JoinPath(this.FilePath, PreviewFileName);
            }
        }

		private void init()
		{
			bool isFirst = false;

			//判断是首次初始化还是文件变动引起的初始化
			if (m_Emoticons == null)
			{
				isFirst = true;
				m_Emoticons = new DefaultEmoticonCollection(this);
			}

			UniteEmotincos(m_Emoticons);
			m_sortedemoticons = null;
			if (isFirst)
			{
				if (!File.Exists(PreviewFilePath))
					BuildPriviewPicture();
			}
			else
			{
				BuildPriviewPicture();
			}

			BeginWach();
		}

        internal string AllotShortcut(DefaultEmoticon emot)
        {
            string file = emot.FileName.Substring(0, emot.FileName.LastIndexOf('.'));

            if (string.IsNullOrEmpty(this.DirectoryName))
            {
                return string.Format(Shortcutfix, file);
            }
            else
            {
                return string.Format(Shortcutfix,
                    string.Format(ShortFormat, this.GroupName, file));
            }
        }

        public List<string> ShortcutList
        {
            get
            {
                List<string> shortcuts = new List<string>();
                foreach (DefaultEmoticon emot in this.m_Emoticons)
                {
                    shortcuts.Add(emot.Shortcut);
                }

                return shortcuts;
            }
        }

        /// <summary>
        /// 开始监控文件夹
        /// </summary>
        public void BeginWach()
        {
            if (watcher != null)
                watcher.Dispose();
            this.watcher = new CacheDependency(this.FilePath);
            IsWatching = true;
        }

        /// <summary>
        /// 取消文件监控
        /// </summary>
        public void EndWach()
        {
            if (watcher != null)
            {
                watcher.Dispose();
                watcher = null;
            }
            IsWatching = false;
        }

        private CacheDependency watcher;

        [JsonItem]
        [SettingItem]
        public int SortOrder { get; set; }

        private string m_groupname;
        [JsonItem]
        [SettingItem]
        public override string GroupName
        {
            get
            {
                if (m_groupname == null)
                {
                    return this.DirectoryName;
                }

                return m_groupname;
            }
            set { m_groupname = value; }
        }

        public string FilePath
        {
            get
            {
                return IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Assets_Face), this.DirectoryName);
            }
        }

        private string m_url;
        public string Url
        {
            get
            {
                if (m_url == null)
                    m_url = UrlUtil.JoinUrl(Globals.GetRelativeUrl(SystemDirecotry.Assets_Face), DirectoryName);
                return m_url;
            }
        }

        private Size m_gridSize = new Size(24, 24);
        public Size GridSize
        {
            get
            {

                return m_gridSize;
            }
        }

        public string GetValue()
        {
            StringList list = new StringList();
            list.Add(this.SortOrder.ToString());
            list.Add(this.GroupName);
            list.Add(this.DirectoryName);
            list.Add(this.Disabled.ToString());
            list.Add(this.Emoticons.GetValue());
            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);
            if (list != null)
            {
                this.SortOrder = int.Parse(list[0]);
                this.GroupName = list[1];
                this.DirectoryName = list[2];
                this.Disabled = bool.Parse(list[3]);
                DefaultEmoticonCollection emots = new DefaultEmoticonCollection(this);
                emots.SetValue(list[4]);
                if (Directory.Exists(this.FilePath))
                    UniteEmotincos(emots);

                Reorder();
            }
        }

        public override bool IsDefault
        {
            get
            {
                return true;
            }
        }

        int m_groupId = 0;
        public override int GroupID
        {
            get
            {
                if (m_groupId == 0)
                    m_groupId = DirectoryName.GetHashCode();
                return m_groupId;
            }
        }

        /// <summary>
        /// 合并原有的表情（数据库有记录的） 和 现在目录底下的实际文件
        /// </summary>
        /// <param name="emoticons"></param>
        public void UniteEmotincos(DefaultEmoticonCollection emoticons)
        {
            DefaultEmoticonCollection diremoticons = new DefaultEmoticonCollection(this);

            List<DefaultEmoticon> Emoticons = new List<DefaultEmoticon>();
            foreach (FileInfo file in IOUtil.GetImagFiles(this.FilePath, SearchOption.TopDirectoryOnly))
            {
                if (file.Name.Equals(PreviewFileName, StringComparison.OrdinalIgnoreCase))
                    continue;

                DefaultEmoticon face = new DefaultEmoticon(this);
                face.FileName = file.Name;
                face.FileSize = file.Length;
                diremoticons.Add(face);
            }

            foreach (DefaultEmoticon em in emoticons)
            {
                if (diremoticons.ContainsKey(em.EmoticonID))
                {
                    DefaultEmoticon temp = diremoticons.GetValue(em.EmoticonID);
                    temp.Shortcut = em.Shortcut;
                    temp.SortOrder = em.SortOrder;
                }
            }
            m_Emoticons = diremoticons;
            Reorder();
        }

        /// <summary>
        /// 序号发生改变时重新排序
        /// </summary>
        public void Reorder()
        {
            this.m_sortedemoticons = null;
            BuildPriviewPicture();
        }

        /// <summary>
        /// 重新生成缩略图
        /// </summary>
        /// <param name="icons"></param>
        public void BuildPriviewPicture()
        {
            if (IsWatching)
                EndWach();

            DefaultEmoticonCollection icons = this.Emoticons;
            if (TotalEmoticons == 0)
            {
                if (File.Exists(this.PreviewFilePath))
                    IOUtil.DeleteFile(PreviewFilePath);
                return;
            }

            Bitmap bmp = new Bitmap(GridSize.Width * TotalEmoticons, GridSize.Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            try
            {
                for (int i = 0; i < this.TotalEmoticons; i++)
                {
                    Image temp = null;
                    try
                    {
                        temp = new Bitmap(icons[i].FilePath);
                    }
                    catch
                    {
                        continue;
                    }
                    g.DrawImage(temp, new Rectangle(i * GridSize.Width, 0
                        , GridSize.Width, GridSize.Height));
                    temp.Dispose();
                }
                bmp.Save(IOUtil.JoinPath(this.FilePath, PreviewFileName), ImageFormat.Png);
            }
            catch
            {

            }

            g.Dispose();
            bmp.Dispose();
            BeginWach();

            //正方形输出
            //if(Count==0)
            //    return ;
            //int borderWidth = 2;


            //int gridX,gridY,previewImageHeight,previewImageWidth;

            //if(Count<4)
            //{
            //    gridX=4;
            //    gridY=1;
            //}
            //else
            //{
            //    gridX =(int)Math.Sqrt((double)Count);
            //    if(Count %  gridX!=0)
            //        gridX++;
            //    gridY =Count / gridX; 
            //}

            //previewImageHeight = (gridSize.Height + borderWidth) * gridY + borderWidth;
            //previewImageWidth = (gridSize.Width + borderWidth) * gridX + borderWidth;

            //Bitmap bmp = new Bitmap(previewImageWidth, previewImageHeight);
            //Graphics g = Graphics.FromImage(bmp);
            //g.Clear(Color.White);

            //int imgIndex=0;
            //for (int i = 0; i < gridX; i++)
            //{
            //    for (int j = 0; j < gridY;j++ )
            //    {
            //        imgIndex=i*gridX + j;
            //        if(imgIndex >this.Count-1)
            //        {
            //            //TODO 跳出循环
            //            break;
            //        }
            //Image temp;
            //try
            //{
            //    temp=new Bitmap( icons[imgIndex].FilePath);
            //}
            //catch
            //{
            //    continue;
            //}
            //       
            //        g.DrawImage(temp, new Rectangle(i * gridSize.Width + borderWidth, j * gridSize.Height + borderWidth
            //            , gridSize.Width, gridSize.Height));
            //        temp.Dispose();
            //    }
            //    if (imgIndex > this.Count - 1)
            //    {
            //        bmp.Save(IOUtil.JoinPath(this.FilePath, PreviewFileName), ImageFormat.Png);
            //        g.Dispose();
            //        bmp.Dispose();
            //        break;
            //    }
            //}
        }

        public override string ToString()
        {
            return this.GetValue();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (watcher != null)
            {
                this.watcher.Dispose();
            }
        }

        #endregion

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.GroupID;
        }

        #endregion
    }

    public class DefaultEmoticonGroupCollection : EntityCollectionBase<int, DefaultEmoticonGroup>, ISettingItem, IDisposable
    {

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (DefaultEmoticonGroup item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        /// <summary>
        /// 通过设置添加的属性
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {

            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    DefaultEmoticonGroup field = new DefaultEmoticonGroup();
                    field.SetValue(item);

                    if (Directory.Exists(field.FilePath))
                    {
                        DefaultEmoticonGroup Group = null;
                        foreach (DefaultEmoticonGroup tempGroup in this)
                        {
                            if (tempGroup.DirectoryName.Equals(field.DirectoryName, StringComparison.OrdinalIgnoreCase))
                            {
                                Group = tempGroup;
                                break;
                            }
                        }

                        if (Group != null)
                        {
                            Group.GroupName = field.GroupName;
                            Group.SortOrder = field.SortOrder;
                            Group.UniteEmotincos(field.Emoticons);
                        }
                        else
                        {
                            this.Add(field);
                            field.BeginWach();
                        }
                    }
                }
            }
        }


        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            foreach (DefaultEmoticonGroup fg in this)
            {
                fg.Dispose();
            }
        }

        #endregion
    }
}