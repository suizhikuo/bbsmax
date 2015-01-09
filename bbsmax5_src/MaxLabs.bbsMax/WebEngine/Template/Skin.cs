//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.WebEngine.Template
{
    public class Skin : IPrimaryKey<string>
    {
        /// <summary>
        /// 皮肤的ID，这也是皮肤所在文件夹的名称
        /// </summary>
        public string SkinID { get; internal set; }

        /// <summary>
        /// 皮肤的名称，从_skin.config文件中读取得到
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 皮肤的版本，从_skin.config文件中读取得到
        /// </summary>
        public string Version { get; internal set; }

        /// <summary>
        /// 皮肤的作者，从_skin.config文件中读取得到
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// 皮肤发布者的网站，从_skin.config文件中读取得到
        /// </summary>
        public string WebSite { get; internal set; }

        /// <summary>
        /// 皮肤的发布时间，从_skin.config文件中读取得到
        /// </summary>
        public DateTime PublishDate { get; internal set; }

        public string Description { get; internal set; }

        public string SkinBase { get; internal set; }

        private string m_RootUrl;

        public string RootUrl 
        {
            get
            {
                if (m_RootUrl == null)
                    m_RootUrl = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Skins), this.SkinID);

                return m_RootUrl;
            }
        }

        private Skin m_SkinBase;

        public Skin GetSkinBase()
        {
            if (m_SkinBase == null && SkinBase != null)
            {
                m_SkinBase = TemplateManager.GetSkin(SkinBase);
            }

            return m_SkinBase;
        }

        private string m_Path = null;
        private string m_VirtualPath = null;

        /// <summary>
        /// 皮肤存放目录的物理路径
        /// </summary>
        public string Path
        {
            get
            {
                if (m_Path == null)
                {
                    m_Path = IOUtil.JoinPath(Globals.GetPath(SystemDirecotry.Skins), SkinID);
                }
                return m_Path;
            }
        }

        /// <summary>
        /// 皮肤存放目录的虚拟路径
        /// </summary>
        public string VirtualPath
        {
            get
            {
                if (m_VirtualPath == null)
                {
                    m_VirtualPath = UrlUtil.JoinUrl(Globals.GetVirtualPath(SystemDirecotry.Skins), SkinID);
                }
                return m_VirtualPath;
            }
        }

        /// <summary>
        /// 模板是否启用
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (AllSettings.Current == null)
                    return false;

                return AllSettings.Current.SkinSettings.DisabledSkins.Contains(SkinID) == false;
            }
        }

        public bool IsDefaultSkin
        {
            get
            {
                if (AllSettings.Current == null)
                    return false;

                return AllSettings.Current.SkinSettings.DefaultSkin == this.SkinID;
            }

            //set
            //{
            //    if (value)
            //    {
            //        AllSettings.Current.SkinSettings.DefaultSkin = this.SkinID;
            //        SettingManager.SaveSettings(AllSettings.Current.SkinSettings);
            //    }
            //}
        }

        #region IPrimaryKey<string> 成员

        public string GetKey()
        {
            return SkinID;
        }

        #endregion
    }

    public class SkinCollection : HashedCollectionBase<string, Skin>
    {
        
    }
}