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
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax
{
    class ShareThreadProvider : IShareProvider
    {

        public ShareType ShareCatagory
        {
            get { return ShareType.Topic; }
        }

        public ShareContent GetShareContent(int targetID, out int userID, out bool canShare)
        {
            userID = 0;
            canShare = false;

            BasicThread thread = PostBOV5.Instance.GetThread(targetID);

            if (thread == null)
            {
                return null;
            }
            if (thread.ThreadStatus == ThreadStatus.Recycled || thread.ThreadStatus == ThreadStatus.UnApproved)
            {
                return null;
            }

            if (thread.Price > 0)
                return null;

            PostV5 post;
            if (thread.ThreadContent != null)
            {
                post = thread.ThreadContent;
            }
            else
            {
                post = PostBOV5.Instance.GetPost(thread.ContentID, false);
            }

            if (post == null)
                return null;

            canShare = true;
            userID = thread.PostUserID;

            Forum forum = thread.Forum;

            string content = @"
<div class=""detail"">
{0}
</div>
";


            string threadUrl = UrlHelper.GetThreadUrlTag(forum.CodeName,thread.ThreadID);

            content = string.Format(content
                //  threadUrl
                //, thread.SubjectText
                //, UrlHelper.GetSpaceUrlTag(thread.PostUserID)
                //, thread.PostUsername
                , StringUtil.CutString(StringUtil.ClearAngleBracket(PostBOV5.Instance.GetPublicContent(thread, post)), Consts.Share_ReviewContentLength));

            ShareContent shareContent = new ShareContent();
            shareContent.Catagory = ShareType.Topic;
            shareContent.Content = content;
            shareContent.Title = thread.Subject;
            shareContent.URL = threadUrl;

            return shareContent;
        }
    }
}