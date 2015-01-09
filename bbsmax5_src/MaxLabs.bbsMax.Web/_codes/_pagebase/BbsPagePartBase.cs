//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web
{
    [TemplatePackage]
	public class BbsPagePartBase : WebEngine.PagePartBase
	{
        /// <summary>
        /// 得到自己的用户对象
        /// </summary>
        protected User My
        {
            get { return User.Current; }
        }

        /// <summary>
        /// 得到自己的UserID
        /// </summary>
        protected int MyUserID
        {
            get { return UserBO.Instance.GetCurrentUserID(); }
        }

        /// <summary>
        /// 判断当前用户是否登录
        /// </summary>
        protected bool IsLogin
        {
            get { return UserBO.Instance.GetCurrentUserID() > 0; }
		}

		#region =========↓用于判断功能是否启用的变量↓====================================================================================================


        protected bool EnableChatFunction
        {
            get { return AllSettings.Current.ChatSettings.EnableChatFunction; }
        }

#if !Passport

        protected bool EnablePropFunction
        {
            get { return AllSettings.Current.PropSettings.EnablePropFunction; }
        }

		protected bool EnableAlbumFunction
		{
			get { return AllSettings.Current.AlbumSettings.EnableAlbumFunction; }
		}

		protected bool EnableBlogFunction
		{
			get { return AllSettings.Current.BlogSettings.EnableBlogFunction; }
		}

		protected bool EnableShareFunction
		{
			get { return AllSettings.Current.ShareSettings.EnableShareFunction; }
		}

		protected bool EnableFavoriteFunction
		{
			get { return AllSettings.Current.FavoriteSettings.EnableFavoriteFunction; }
		}

		protected bool EnableDoingFunction
		{
			get { return AllSettings.Current.DoingSettings.EnableDoingFunction; }
		}

        protected bool EnableNetDiskFunction
        {
            get { return DiskBO.Instance.IsEnableDisk; }
        }

        protected bool EnableUserEmoticon
        {
            get { return EmoticonBO.Instance.IsEnableEmotion; }
        }

#endif

		#endregion
	}
}