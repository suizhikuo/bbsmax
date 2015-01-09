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
using MaxLabs.bbsMax.Enums;

namespace MaxLabs.bbsMax.Common
{
    public class UrlHelper
    {

        public static string GetIndexUrl()
        {
            return BbsRouter.GetIndexUrl();
        }

        public static string GetLoginUrl()
        {
            return BbsRouter.GetUrl("login");
        }

        public static string GetRegisterUrl()
        {
            return BbsRouter.GetUrl("register");
        }

        public static string GetRecoverPasswordUrl()
        {
            return BbsRouter.GetUrl("recoverpassword");
        }

        public static string GetSpaceUrlTag(int userID)
        {
            return GetSpaceUrlTag(userID.ToString());
        }

        public static string GetSpaceUrl(int userID)
        {
            return BbsRouter.GetUrl("space/"+userID);
        }

        public static string GetSpaceUrlTag(string userID)
        {
            return string.Concat("$url(space/", userID, ")");
        }

        #region BLOG

        public static string GetBlogUrl()
        {
            return BbsRouter.GetUrl("app/blog/index");
        }

        public static string GetBlogIndexUrl(int userID)
        {
            return BbsRouter.GetUrl("app/blog/index", "uid=" + userID);
        }

        public static string GetBlogArticleUrl(int articleID)
        {
            return BbsRouter.GetUrl("app/blog/view", "id=" + articleID);
        }

        public static string GetBlogArticleUrlTag(int articleID)
        {
            return string.Concat("$url(app/blog/view)?id=", articleID);
        }

        #endregion


        #region Share

        public static string GetShareUrl()
        {
            return BbsRouter.GetUrl("app/share/index");
        }

        public static string GetShareViewUrl(int shareID)
        {
            return BbsRouter.GetUrl("app/share/view", "id=" + shareID);
        }

        public static string GetFavoriteUrl()
        {
            return BbsRouter.GetUrl("app/share/index", "mode=fav");
        }

        public static string GetShareViewUrlTag(int shareID)
        {
            return string.Concat("$url(app/share/view)?id=", shareID);
        }
        #endregion

        #region album
        public static string GetAlbumUrl()
        {
            return BbsRouter.GetUrl("app/album/index");
        }

        public static string GetAlbumViewUrl(int albumID)
        {
            return BbsRouter.GetUrl("app/album/list", "id=" + albumID);
        }

        public static string GetPhotoUrlForPager(int photoID)
        {
            return BbsRouter.GetUrl("app/album/photo", "id=" + photoID + "&page={0}");
        }

        public static string GetPhotoIndexUrl(int userID)
        {
            return BbsRouter.GetUrl("app/album/index", "uid=" + userID);
        }

        public static string GetUploadPhotoUrl()
        {
            return BbsRouter.GetUrl("app/album/upload");
        }

        public static string GetAblumUrlTag(int ablumID)
        {
            return string.Concat("$url(app/album/list)?id=", ablumID);
        }

        public static string GetPhotoUrlTag(int photoID)
        {
            return string.Concat("$url(app/album/photo)?id=", photoID);
        }

        #endregion


        #region disk

        public static string GetDiskUrl()
        {
            return BbsRouter.GetUrl("app/disk/index");
        }

        #endregion

        #region doing

        public static string GetDoingUrl()
        {
            return BbsRouter.GetUrl("app/doing/index");
        }

        #endregion

        #region emoticon

        public static string GetEmoticonUrl()
        {
            return BbsRouter.GetUrl("app/emoticon/index");
        }

        #endregion

        #region friend

        public static string GetFriendsInviteUrl()
        {
            return BbsRouter.GetUrl("my/friends-invite");
        }

        public static string GetFriendsImpressionUrl()
        {
            return BbsRouter.GetUrl("my/friends-impression");
        }


        public static string GetBlackListUrl()
        {
            return BbsRouter.GetUrl("my/blacklist");
        }
        #endregion

        #region mission

        public static string GetMissionUrl()
        {
            return BbsRouter.GetUrl("mission/index");
        }

        public static string GetMyMissionUrl()
        {
            return BbsRouter.GetUrl("mission/current");
        }

        public static string GetMissionDetailUrlTag(int missionID)
        {
            return string.Concat("$url(mission/detail)?mid=", missionID);
        }
        #endregion

        #region prop

        public static string GetPropUrl()
        {
            return BbsRouter.GetUrl("prop/index");
        }

        public static string GetMyPropUrl()
        {
            return BbsRouter.GetUrl("prop/my");
        }
        public static string GetSellingPropUrl()
        {
            return BbsRouter.GetUrl("prop/selling");
        }
        public static string GetPropLogUrl()
        {
            return BbsRouter.GetUrl("prop/log");
        }
        #endregion

        #region my

        public static string GetMyAvatarUrl()
        {
            return BbsRouter.GetUrl("my/avatar");
        }

        public static string GetMySettingUrl()
        {
            return BbsRouter.GetUrl("my/setting");
        }

        public static string GetMyChagePasswordUrl()
        {
            return BbsRouter.GetUrl("my/changepassword");
        }
        public static string GetMyPrivacyUrl()
        {
            return BbsRouter.GetUrl("my/privacy");
        }
        public static string GetMyNotifySettingUrl()
        {
            return BbsRouter.GetUrl("my/notify-setting");
        }
        public static string GetMyFeedFilterUrl()
        {
            return BbsRouter.GetUrl("my/feedfilter");
        }
        public static string GetMyPointUrl()
        {
            return BbsRouter.GetUrl("my/point");
        }
        public static string GetMyPointExchangeUrl()
        {
            return BbsRouter.GetUrl("my/point-exchange");
        }
        public static string GetMyPointTransferUrl()
        {
            return BbsRouter.GetUrl("my/point-transfer");
        }
        public static string GetMySpaceStyleUrl()
        {
            return BbsRouter.GetUrl("my/spacestyle");
        }
        public static string GetMyDefaultUrl()
        {
            return BbsRouter.GetUrl("my/default");
        }
        public static string GetMyNotifyUrl()
        {
            return BbsRouter.GetUrl("my/notify");
        }
        public static string GetMyChatUrl()
        {
            return BbsRouter.GetUrl("my/chat");
        }

        public static string GetMyFriendUrl()
        {
            return BbsRouter.GetUrl("my/friends");
        }
        #endregion

        #region thread

        public static string GetMyThreadUrl()
        {
            return BbsRouter.GetUrl("my/mythreads");
        }

#if!Passport
        public static string GetMyThreadsUrl(MyThreadType myThreadType)
        {
            return GetMyThreadsUrl(myThreadType.ToString());
        }
        public static string GetMyThreadsUrlForPager(MyThreadType myThreadType)
        {
            return BbsRouter.GetUrl("my/mythreads", "type=" + myThreadType.ToString() + "&page={0}");
        }
#endif

        public static string GetMyThreadsUrl(string myThreadType)
        {
            return BbsRouter.GetUrl("my/mythreads", "type=" + myThreadType.ToString());
        }
        public static string GetThreadUrlTag(string forumCodeName, int threadID)
        {
            return string.Concat("$url(", forumCodeName, "/thread-", threadID, "-1-1)");
        }


        public static string GetUnapprovedThreadsUrl(string codeName)
        {
            return BbsRouter.GetUrl(codeName + "/unapproved-1");
        }

        public static string GetUnapprovedPostsUrl(string codeName)
        {
            return BbsRouter.GetUrl(codeName + "/unapprovedpost-1");
        }

        public static string GetRecycledThreadsUrl(string codeName)
        {
            return BbsRouter.GetUrl(codeName + "/recycled-1");
        }

        #endregion


        public static string GetMemberUrlTag(string viewType)
        {
            return string.Concat("$url(members)?view=" + viewType);
        }

        public static string GetSearchUrl()
        {
            return BbsRouter.GetUrl("search");
        }

        public static string GetMembersUrl()
        {
            return BbsRouter.GetUrl("members");
        }

        public static string GetAnnouncementsUrl()
        {
            return BbsRouter.GetUrl("announcements");
        }

        public static string GetNewThreadUrl()
        {
            return BbsRouter.GetUrl("new");
        }

        public static string GetForumUrl(string codename)
        {
            return BbsRouter.GetUrl(string.Format("{0}/list-1", codename));
        }

        public static string GetArchiverThreadUrl(string codeName, int threadID)
        {
            return BbsRouter.GetUrl(string.Format("archiver/{0}/thread-{1}-{2}", codeName, threadID, 1));
        }

        #region 发送验证邮箱地址

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">发送验证email邮件的类型</param>
        /// <param name="userID">用户ID</param>
        /// <param name="password">已经入库的、加密后的密码</param>
        /// <param name="isSent">是否已经发出了邮件，如果是，则不需要进入提示用户发送邮件的界面</param>
        /// <returns></returns>
        public static string GetSendEmailUrl(ValidateEmailAction type,int userID,string encodedPassword, bool isSent)
        {
            string code = SecurityUtil.SHA1(encodedPassword + Globals.SafeString);
            return BbsRouter.GetUrl("register", string.Format("type={0}&userid={1}&code={2}&issent={3}", (int)type, userID, code, isSent ? 1 : 0));
        }

	    #endregion
    }
}