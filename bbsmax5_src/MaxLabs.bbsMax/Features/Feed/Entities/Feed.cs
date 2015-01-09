//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 通知
    /// </summary>
    public sealed class Feed : IPrimaryKey<int>, ICloneable
    {

        public Feed()
        { }

        public Feed(DataReaderWrap readerWrap)
        {
            ID = readerWrap.Get<int>("ID");
            TargetID = readerWrap.Get<int?>("TargetID");
            ActionType = (int)readerWrap.Get<byte>("ActionType");
            PrivacyType = (PrivacyType)(int)readerWrap.Get<byte>("PrivacyType");
            TargetUserID = readerWrap.Get<int>("TargetUserID");

            AppID = readerWrap.Get<Guid>("AppID");
            Title = readerWrap.Get<string>("Title");
            Description = readerWrap.Get<string>("Description");
            TargetNickname = readerWrap.Get<string>("TargetNickname");
            visibleUserIDsString = readerWrap.Get<string>("VisibleUserIDs");
            CommentInfo = readerWrap.Get<string>("CommentInfo");
            CreateDate = readerWrap.Get<DateTime>("CreateDate");

            CommentTargetID = readerWrap.Get<int>("CommentTargetID");
        }

        public int ID { get; set; }

        /// <summary>
        /// 通用的ID（有需要时使用 例如:相册上传图片，就记相册ID 方便处理上传多张图片时只记一个通知）
        /// </summary>
        public int? TargetID { get; set; }

        public int CommentTargetID { get; set; }

        /// <summary>
        /// APP的动作
        /// </summary>
        public int ActionType { get; set; }


        public PrivacyType PrivacyType { get; set; }

        /// <summary>
        /// 相关联的UserID,如aa评论了bb的日志，就是bb的userID;cc和dd成为好友,就是dd的userID
        /// </summary>
        public int TargetUserID { get; set; }



        /// <summary>
        /// 应用ID
        /// </summary>
        public Guid AppID { get; set; }



        /// <summary>
        /// 通知标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 通知简介
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 与TargetUserID对应的昵称
        /// </summary>
        public string TargetNickname { get; set; }

        private string m_CommentInfo;
        public string CommentInfo 
        {
            get { return m_CommentInfo; }
            set 
            { 
                m_CommentInfo = value;
                m_CommentCount = null;
                m_CommentIDs = null;
            }
        }


        private int? m_CommentCount;
        /// <summary>
        /// targetID 对应的内容的评论数
        /// </summary>
        public int CommentCount 
        {
            get
            {
                if (m_CommentCount == null)
                {
                    ProcessCommentInfo();
                }

                return m_CommentCount.Value;
            }
        }

        private List<int> m_CommentIDs;
        /// <summary>
        /// 头尾两个评论ID
        /// </summary>
        public List<int> CommentIDs
        { 
            get
            {
                if (m_CommentIDs == null)
                    ProcessCommentInfo();

                return m_CommentIDs;
            }
        }

        private void ProcessCommentInfo()
        {
            if (string.IsNullOrEmpty(CommentInfo))
            {
                m_CommentCount = 0;
                m_CommentIDs = new List<int>();
                return;
            }
            string[] infos = CommentInfo.Split(',');
            m_CommentIDs = new List<int>();
            if (infos.Length == 3)
            {
                try
                {
                    int firstID = int.Parse(infos[0]);
                    int lastID = int.Parse(infos[1]);
                    if (firstID != 0)
                    {
                        m_CommentIDs.Add(firstID);
                        if (lastID != firstID)
                            m_CommentIDs.Add(lastID);
                        m_CommentCount = int.Parse(infos[2]);
                    }
                }
                catch { }
            }
            if (m_CommentCount == null)
                m_CommentCount = 0;
        }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }


        private UserFeedCollection users = new UserFeedCollection();
        /// <summary>
        /// 与该动态相关联的用户
        /// </summary>
        public UserFeedCollection Users
        {
            get { return users; }
            set { users = value; }
        }


        private List<int> visibleUserIDs;
        public List<int> VisibleUserIDs
        {
            get
            {
                if (visibleUserIDs == null)
                {
                    visibleUserIDs = new List<int>();
                    if (string.IsNullOrEmpty(visibleUserIDsString))
                        return visibleUserIDs;

                    string[] ids = visibleUserIDsString.Split(',');
                    foreach (string id in ids)
                    {
                        int userID;
                        if (int.TryParse(id, out userID))
                            visibleUserIDs.Add(userID);
                    }
                }
                return visibleUserIDs;
            }
            set
            {
                visibleUserIDs = value;
                if (value == null)
                    visibleUserIDsString = null;
            }
        }

        private string visibleUserIDsString;

        private bool isSpecial = false;
        /// <summary>
        /// (定义：A评论了B的日志，A就是动态发起者B就是动态被动者)
        /// 大部分动态 该值是False  表示该条动态只属于发起者 取好友动态时与targetUserID无关
        /// 而类似加好友 该值是True 表示该条动态是属于发起者和被动者双方的 取好友动态时与targetUserID有关 即要判断 UserFeeds 表中与该动态相关的用户是否有好友 也要判断 targetUserID 是否是好友 
        /// </summary>
        public bool IsSpecial { get { return isSpecial; } set { isSpecial = value; } }


        public bool DisplayComments
        {
            get
            {
                if (TargetID != null && TargetID.Value > 0 && AppAction.DisplayComments)
                    return true;
                else
                    return false;
            }
        }

        public AppAction AppAction
        {
            get
            {
                return AppManager.GetApp(AppID).AppActions.GetValue(ActionType);
            }
        }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ID;
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            Feed feed = new Feed();
            feed.AppID = AppID;
            feed.ActionType = ActionType;
            feed.CreateDate = CreateDate;
            feed.Description = Description;
            feed.ID = ID;
            feed.TargetID = TargetID;
            feed.TargetNickname = TargetNickname;
            feed.TargetUserID = TargetUserID;
            feed.Title = Title;
            feed.Users = Users;
            feed.IsSpecial = IsSpecial;
            feed.PrivacyType = PrivacyType;
            feed.VisibleUserIDs = VisibleUserIDs;
            feed.CommentInfo = CommentInfo;
            feed.CommentTargetID = CommentTargetID;
            return feed;
        }

        #endregion
    }
    /// <summary>
    /// 通知对象集合
    /// </summary>
    public class FeedCollection : EntityCollectionBase<int, Feed> //ICollection<Feed> //Dictionary<int, Feed>//Collection<Feed>
    {

        public FeedCollection()
        {
        }



        public FeedCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                this.Add(new Feed(readerWrap));
            }
        }
    }



}