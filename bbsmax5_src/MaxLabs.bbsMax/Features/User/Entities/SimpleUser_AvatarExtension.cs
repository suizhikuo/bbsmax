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
using System.Web;
using System.IO;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public partial class SimpleUser
    {

        private static readonly string imageFormat = "<img src=\"{0}\" width=\"{1}\" height=\"{2}\"  alt=\"\" />"; //style=\"visibility:hidden;\" onload=\"AvatarLoaded2(this)\" onerror=\"AvatarError2(this)\" />";
        private static readonly string linkFormat = "<a href=\"{0}\" {1}>{2}</a>";

        /// <summary>
        /// 当前是否系统默认头像
        /// </summary>
        public bool IsDefaultAvatar
        {
            get { return string.IsNullOrEmpty(AvatarSrc) && !AvatarPropFlag.Available; }
        }

        //public bool IsUseAttachAvatar TODO

        private string m_SmallAvatarPath = null;
        private string m_AvatarPath = null;
        private string m_BigAvatarPath = null;

        private string m_SmallAvatar = null;
        private string m_Avatar = null;
        private string m_BigAvatar = null;

        private string m_SmallAvatarLink = null;
        private string m_AvatarLink = null;
        private string m_BigAvatarLink = null;

        private string m_PopupAvatarLink = null;

        internal void ClearAvatarCache()
        {

            m_SmallAvatarPath = null;
            m_AvatarPath = null;
            m_BigAvatarPath = null;

            m_SmallAvatar = null;
            m_Avatar = null;
            m_BigAvatar = null;

            m_SmallAvatarLink = null;
            m_AvatarLink = null;
            m_BigAvatarLink = null;

            m_PopupAvatarLink = null;
        }

        private void CheckPropAvatar()
        {
            if (usePropAvatar && !this.AvatarPropFlag.Available)
            {
                this.ClearAvatarCache();
            }
        }

        #region 返回头像的地址(相对路径)

        /// <summary>
        /// 24px头像路径
        /// </summary>
        public string SmallAvatarPath
        {
            get
            {
                CheckPropAvatar();

                if (m_SmallAvatarPath == null)
                    m_SmallAvatarPath = GetAvatarPath(UserAvatarSize.Small);

                return m_SmallAvatarPath;
            }
            //internal set
            //{
            //    m_SmallAvatarPath = value;
            //}
        }

        /// <summary>
        /// 48px头像路径
        /// </summary>
        public string AvatarPath
        {
            get
            {
                CheckPropAvatar();

                if (m_AvatarPath == null)
                    m_AvatarPath = GetAvatarPath(UserAvatarSize.Default);
                return m_AvatarPath;
            }
            //internal set
            //{
            //    m_AvatarPath = value;
            //}
        }

        /// <summary>
        /// 120px头像路径
        /// </summary>
        public string BigAvatarPath
        {
            get
            {
                CheckPropAvatar();
                if (m_BigAvatarPath == null)
                    m_BigAvatarPath = GetAvatarPath(UserAvatarSize.Big);

                return m_BigAvatarPath;
            }
            //internal set
            //{
            //    m_BigAvatarPath = value;
            //}
        }

        private bool usePropAvatar = false;

        private string GetAvatarPath(UserAvatarSize size)
        {
            if (AvatarPropFlag.Available)
            {
                string s =  UserBO.Instance.GetAvatarSizeDirectoryName(size);
                s = this.AvatarPropFlag.PropData.Replace("{size}",s);
                usePropAvatar = true;
                return UrlUtil.ResolveUrl(s);
               // return Globals.GetVirtualPath(SystemDirecotry.Upload_Avatar,, this.AvatarPropFlag.PropData);
            }
            usePropAvatar = false;
            //判断头像是否存在，如果存在直接返回头像
            string avatarSrc = AvatarSrc;

            if (string.IsNullOrEmpty(avatarSrc) == false)
            {
#if !Passport
                PassportClientConfig settings = Globals.PassportClient;

                if (settings.EnablePassport)
                {
                    string avatarRelativeUrl = Globals.GetRelativeUrl(SystemDirecotry.Upload_Avatar, UserBO.Instance.GetAvatarSizeDirectoryName(size), avatarSrc).Remove(0, 1);
                    return UrlUtil.JoinUrl(settings.PassportRoot, avatarRelativeUrl);
                }
#endif
                return Globals.GetVirtualPath(SystemDirecotry.Upload_Avatar, UserBO.Instance.GetAvatarSizeDirectoryName(size), avatarSrc);
            }

            //当头像和附加头像都为空时，使用默认头像
            string defaultAvatarUrl;

            switch (size)
            {
                case UserAvatarSize.Small:
                    defaultAvatarUrl = Consts.DefaultUserAvatar_Small;
                    break;

                case UserAvatarSize.Big:
                    defaultAvatarUrl = Consts.DefaultUserAvatar_Big;
                    break;

                default:
                    defaultAvatarUrl = Consts.DefaultUserAvatar_Default;
                    break;
            }

#if !Passport
            PassportClientConfig settings2 = Globals.PassportClient;

            if (settings2.EnablePassport)
                return UrlUtil.JoinUrl(settings2.PassportRoot, defaultAvatarUrl);
#endif

            return Globals.GetVirtualPath(SystemDirecotry.Root, defaultAvatarUrl);

        }

        #endregion

        #region 返回头像的整个图片标签（<img />）

        /// <summary>
        /// 小 头像
        /// </summary>
        public string SmallAvatar
        {
            get
            {
                if (m_SmallAvatar == null)
                    m_SmallAvatar = OutputImage(SmallAvatarPath, UserAvatarSize.Small);

                return m_SmallAvatar;
            }
            //internal set
            //{
            //    m_SmallAvatar = value;
            //}
        }

        public string Avatar
        {
            get
            {
                if (m_Avatar == null)
                    m_Avatar = OutputImage(AvatarPath, UserAvatarSize.Default);

                return m_Avatar;
            }
            //internal set
            //{
            //    m_Avatar = value;
            //}
        }

        /// <summary>
        /// 大头像
        /// </summary>
        public string BigAvatar
        {
            get
            {
                if (m_BigAvatar == null)
                    m_BigAvatar = OutputImage(BigAvatarPath, UserAvatarSize.Big);

                return m_BigAvatar;
            }
            //internal set
            //{
            //    m_BigAvatar = value;
            //}
        }

        private string OutputImage(string url, UserAvatarSize size)
        {
            int width, height;
            switch (size)
            {
                case UserAvatarSize.Big:
                    width = 120;
                    height = 120;
                    break;
                case UserAvatarSize.Small:
                    width = 24;
                    height = 24;
                    break;
                default:
                    width = 48;
                    height = 48;
                    break;
            }
            return string.Format(imageFormat, HttpUtility.HtmlAttributeEncode(url), width, height);
        }

        #endregion

        #region 返回带链接的图片标签（<a><img /></a>）

        public string SmallAvatarLink
        {
            get
            {
                if (m_SmallAvatarLink == null)
                    m_SmallAvatarLink = string.Format(linkFormat, BbsRouter.GetUrl("space/" + UserID), "", SmallAvatar);

                return m_SmallAvatarLink;
            }
            //internal set
            //{
            //    m_SmallAvatarLink = value;
            //}
        }

        public string AvatarLink
        {
            get
            {
                if (m_AvatarLink == null)
                    m_AvatarLink = string.Format(linkFormat, BbsRouter.GetUrl("space/" + UserID), "", Avatar);

                return m_AvatarLink;
            }
            //internal set
            //{
            //    m_AvatarLink = value;
            //}
        }

        public string BigAvatarLink
        {
            get
            {
                if (m_BigAvatarLink == null)
                    m_BigAvatarLink = string.Format(linkFormat, BbsRouter.GetUrl("space/" + UserID), "", this.BigAvatar);

                return m_BigAvatarLink;
            }
            //internal set
            //{
            //    m_BigAvatarLink = value;
            //}
        }

        #endregion

        //====

        public string PopupAvatarLink
        {
            get
            {
                if (m_PopupAvatarLink == null)
                    m_PopupAvatarLink = UserID <= 0 ? "" : "<a href=\"" + BbsRouter.GetUrl("space/" + UserID) + "\" target=\"_blank\">" + Avatar + "</a>";

                return m_PopupAvatarLink;
            }
            //internal set
            //{
            //    m_PopupAvatarLink = value;
            //}
        }

    }
}