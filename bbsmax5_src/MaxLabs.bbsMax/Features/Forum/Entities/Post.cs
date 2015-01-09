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
using MaxLabs.bbsMax.DataAccess;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Ubb;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Entities
{
    public class PostV5 : IPrimaryKey<int>, ITextRevertable2, IFillSimpleUsers
    {
        public PostV5()
        {

        }

        public PostV5(DataReaderWrap readerWrap)
        {
            PostID = readerWrap.Get<int>("PostID");
            ParentID = readerWrap.Get<int>("ParentID");
            ThreadID = readerWrap.Get<int>("ThreadID");
            ForumID = readerWrap.Get<int>("ForumID");
            PostType = (PostType)readerWrap.Get<byte>("PostType");
            IconID = readerWrap.Get<int>("IconID");
            Subject = readerWrap.Get<string>("Subject");
            Content = readerWrap.Get<string>("Content");
            int contentFormat = readerWrap.Get<byte>("ContentFormat");
            EnableSignature = readerWrap.Get<bool>("EnableSignature");
            EnableReplyNotice = readerWrap.Get<bool>("EnableReplyNotice");
            IsShielded = readerWrap.Get<bool>("IsShielded");
            UserID = readerWrap.Get<int>("UserID");
            Username = readerWrap.Get<string>("NickName");
            LastEditorID = readerWrap.Get<int>("LastEditorID");
            LastEditor = readerWrap.Get<string>("LastEditor");
            IPAddress = readerWrap.Get<string>("IPAddress");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");
            UpdateDate = readerWrap.Get<DateTime>("UpdateDate");
            SortOrder = readerWrap.Get<long>("SortOrder");
            MarkCount = readerWrap.Get<int>("MarkCount");
            LoveCount = readerWrap.Get<int>("LoveCount");
            HateCount = readerWrap.Get<int>("HateCount");
            KeywordVersion = readerWrap.Get<string>("KeywordVersion");
            HistoryAttachmentIDs = readerWrap.Get<string>("HistoryAttachmentIDs");
            FloorNumber = readerWrap.Get<int>("FloorNumber");

            EnableHTML = (contentFormat & (int)ContentFormat.EnableHTML) == (int)ContentFormat.EnableHTML;
            EnableMaxCode = (contentFormat & (int)ContentFormat.EnableMaxCode) == (int)ContentFormat.EnableMaxCode;
            EnableMaxCode3 = (contentFormat & (int)ContentFormat.EnableMaxCode3) == (int)ContentFormat.EnableMaxCode3;
            IsV5_0 = (contentFormat & (int)ContentFormat.IsV5_0) == (int)ContentFormat.IsV5_0;

            if (SortOrder >= 4000000000000000)
                IsApproved = false;
            else
                IsApproved = true;
        }

        public int PostID { get; set; }

        public int ParentID { get; set; }

        public int ThreadID { get; set; }

        public int ForumID { get; set; }

        public PostType PostType { get; set; }

        public int IconID { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public bool EnableSignature { get; set; }

        public bool EnableReplyNotice { get; set; }

        public bool IsShielded { get; set; }

        public int UserID { get; set; }

        public string Username { get; set; }

        public int LastEditorID { get; set; }

        public string LastEditor { get; set; }

        public string IPAddress { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public int FloorNumber { get; set; }

        /// <summary>
        /// 评分次数
        /// </summary>
        public int MarkCount { get; set; }

        /// <summary>
        /// 支持数
        /// </summary>
        public int LoveCount { get; set; }

        /// <summary>
        /// 反对数
        /// </summary>
        public int HateCount { get; set; }

        public long SortOrder { get; set; }

        public string KeywordVersion { get; set; }

        public string HistoryAttachmentIDs { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return this.PostID;
        }

        #endregion

        public Forum Forum { get { return ForumBO.Instance.GetForum(ForumID, false); } }

        public User User
        {
            get
            {
                return UserBO.Instance.GetUser(UserID, GetUserOption.WithAll);
            }
        }

        /// <summary>
        /// 纯文本格式的标题 过滤过HTML
        /// </summary>
        public string SubjectText
        {
            get
            {
                string key = "post_SubjectText_" + PostID;

                string tempContent;
                if (PageCacheUtil.TryGetValue<string>(key, out tempContent) == false)
                {
                    tempContent = StringUtil.ClearAngleBracket(Subject);
                    PageCacheUtil.Set(key, tempContent);
                }
                return tempContent;
            }
        }

        public string ContentText
        {
            get
            {
                string key = "post_ContentText_" + PostID;

                string tempContent;
                if (PageCacheUtil.TryGetValue<string>(key, out tempContent) == false)
                {
                    string formattedcontent = PostUbbParserV5.ParseWhenDisplay(UserID, ForumID, PostID, Content, EnableHTML, EnableMaxCode, IsV5_0, Attachments);

                    if (PostType != PostType.ThreadContent)
                        tempContent = formattedcontent;
                    else
                        tempContent = PostUbbParserV5.ProcessThreadContentTags(formattedcontent, ThreadID);

                    PageCacheUtil.Set(key, tempContent);
                }
                return tempContent;
            }
        }

        /// <summary>
        /// 用于搜索时候显示内容  （不替换内容中的附件链接）
        /// </summary>
        public string ContentForSearch
        {
            get
            {
                string key = "post_ContentForSearch_" + PostID;

                string tempContent;
                if (PageCacheUtil.TryGetValue<string>(key, out tempContent) == false)
                {
                    string formattedcontent = PostUbbParserV5.ParseWhenDisplay(UserID, ForumID, PostID, Content, EnableHTML, EnableMaxCode, IsV5_0, new AttachmentCollection());
                    if (PostType != PostType.ThreadContent)
                        tempContent = formattedcontent;
                    else
                        tempContent = PostUbbParserV5.ProcessThreadContentTags(formattedcontent, ThreadID);

                    PageCacheUtil.Set(key, tempContent);
                }

                return tempContent;
            }
        }


        ///// <summary>
        ///// 启用表情转换 
        ///// </summary>
        //public bool EnableEmoticons { get; set; }

        /// <summary>
        /// 启用HTML
        /// </summary>
        public bool EnableHTML { get; set; }

        /// <summary>
        /// 启用MaxCode
        /// </summary>
        public bool EnableMaxCode { get; set; }

        public bool EnableMaxCode3 { get; set; }

        /// <summary>
        /// 是否是5.0版本发的帖子
        /// </summary>
        public bool IsV5_0 { get; set; }

        public bool IsApproved { get; set; }

        public int PostIndex { get; set; }

        public string PostIndexAlias
        {
            get
            {
                return AllSettings.Current.PostIndexAliasSettings.GetPostAliasName(PostIndex);
            }
        }

        public AttachmentCollection Attachments { get; set; }

        public PostMarkCollection PostMarks { get; set; }

        
        static System.Text.RegularExpressions.Regex regex_file = new Regex(@"\[attach[imgeda=\d,]*\](\d+)\[/attach[imgeda]*\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private AttachmentCollection m_AttachmentsForView;
        /// <summary>
        /// 附件列表(如果帖子内容里有出现过的附件将排除,需要购买的不排除)
        /// </summary>
        public AttachmentCollection AttachmentsForView
        {
            get
            {
                if (m_AttachmentsForView == null)
                {
                    MatchCollection mc = regex_file.Matches(Content);
                    if (mc.Count > 0)
                    {
                        m_AttachmentsForView = new AttachmentCollection();
                        foreach (Attachment attachment in Attachments)
                        {
                            bool have = false;
                            foreach (Match m in mc)
                            {
                                if (m.Groups[1].Value == attachment.AttachmentID.ToString())
                                {
                                    attachment.IsInContent = true;
                                    if (attachment.Price == 0)
                                    {
                                        have = true;
                                        break;
                                    }
                                }
                            }
                            if (!have)
                                m_AttachmentsForView.Add(attachment);
                        }
                    }
                    else
                        m_AttachmentsForView = Attachments;
                }
                return m_AttachmentsForView;
            }
        }


        public bool IsFreeAttachmentID(int attachmentID)
        {
            if (PostType == PostType.ThreadContent)
            {
                return PostUbbParserV5.IsFreeAttachmentID(attachmentID, Content);
            }
            return false;
        }
        public bool IsFreeDiskFileID(int diskFileID)
        {
            if (PostType == PostType.ThreadContent)
            {
                return PostUbbParserV5.IsFreeDiskFileID(diskFileID, Content);
            }
            return false;
        }



        #region ITextRevertable2 成员

        /// <summary>
        /// 原始标题内容,不经过标题关键字过滤的
        /// </summary>
        public string OriginalSubject { get; set; }

        /// <summary>
        /// 原始正文内容,不经过关键字过滤的
        /// </summary>
        public string OriginalContent { get; set; }

        public string Text1
        {
            get { return Subject; }
        }

        public void SetNewRevertableText1(string text, string textVersion)
        {
            Subject = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText1(string originalText)
        {
            OriginalSubject = originalText;
        }

        public string Text2
        {
            get { return Content; }
        }

        public string TextVersion2
        {
            get { return KeywordVersion; }
        }

        public void SetNewRevertableText2(string text, string textVersion)
        {
            Content = text;
            KeywordVersion = textVersion;
        }

        public void SetOriginalText2(string originalText)
        {
            OriginalContent = originalText;
        }

        #endregion

        #region IFillSimpleUsers 成员

        public int[] GetUserIdsForFill(int actionType)
        {
            switch (actionType)
            {
                case 1:
                    return new int[1] { UserID };
                case 2:
                    return new int[1] { LastEditorID };
                case 3:
                    return new int[2] { UserID, LastEditorID };
                default:
                    return new int[] { };
            }
        }

        #endregion
    }

    public class PostCollectionV5 : EntityCollectionBase<int, PostV5>
    {
        public PostCollectionV5()
        { }

        public PostCollectionV5(PostCollectionV5 posts)
            : base(posts)
        { }

        public PostCollectionV5(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                PostV5 post = new PostV5(readerWrap);

                this.Add(post);
            }
        }

        public List<int> GetUserIds()
        {
            List<int> userIds = new List<int>();

            foreach (PostV5 post in this)
            {
                if (userIds.Contains(post.UserID) == false)
                    userIds.Add(post.UserID);
            }

            return userIds;
        }

    }
}