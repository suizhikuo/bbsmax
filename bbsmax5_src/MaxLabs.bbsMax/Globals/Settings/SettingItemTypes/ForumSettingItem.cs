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
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Settings
{
    public class ForumSettingItem : SettingBase, IPrimaryKey<int>
    {
        public ForumSettingItem()
        {

            AllowGuestVisitForum = true;
            DisplayInListForGuest = true;
            VisitForum = new Exceptable<bool>(true);
            DisplayInList = new Exceptable<bool>(true);

            EnableHiddenTag = true;

            EnableThreadRank = true;

            EnableSellAttachment = new Exceptable<bool>(true);

            EnableSellThread = new Exceptable<bool>(true);

            PostSubjectLengths = new Exceptable<Int32Scope>(new Int32Scope(2, 150));

            PostContentLengths = new Exceptable<Int32Scope>(new Int32Scope(2, 50000));

            CreatePostIntervals = new Exceptable<int>(10);



            UpdateThreadSortOrderIntervals = new Exceptable<int>(60 * 60 * 24 * 30);


            RecycleOwnThreadsIntervals = new Exceptable<int>(60 * 60 * 24);

            UpdateOwnPostIntervals = new Exceptable<int>(0);

            DeleteOwnThreadsIntervals = new Exceptable<int>(60 * 60 * 24);


            QuestionValidDays = new Exceptable<long>(60 * 60 * 24 * 3);

            PolemizeValidDays = new Exceptable<long>(60 * 60 * 24 * 3);

            PollValidDays = new Exceptable<long>(60 * 60 * 24 * 3);

            AllowFileExtensions = new Exceptable<ExtensionList>(ExtensionList.Parse("*"));

            AllowImageAttachment = new Exceptable<bool>(true);

            AllowAttachment = new Exceptable<bool>(true);

            MaxSignleAttachmentSize = new Exceptable<long>(1024 * 1024 * 5);

            MaxTopicAttachmentCount = new Exceptable<int>(10);

            MaxPostAttachmentCount = new Exceptable<int>(10);

            CreatePostAllowEmoticon = new Exceptable<bool>(true);

            CreatePostAllowAudioTag = new Exceptable<bool>(true);

            CreatePostAllowFlashTag = new Exceptable<bool>(true);

            CreatePostAllowHTML = new Exceptable<bool>(false);

            CreatePostAllowImageTag = new Exceptable<bool>(true);

            CreatePostAllowMaxcode = new Exceptable<bool>(true);

            CreatePostAllowTableTag = new Exceptable<bool>(true);

            CreatePostAllowUrlTag = new Exceptable<bool>(true);

            CreatePostAllowVideoTag = new Exceptable<bool>(true);

            ShowSignatureInThread = new Exceptable<bool>(true);

            ShowSignatureInPost = new Exceptable<bool>(true);

            ReplyNeedApprove = new Exceptable<bool>(false);

            CreateThreadNeedApprove = new Exceptable<bool>(false);

            DefaultThreadSortField = ThreadSortField.LastReplyDate;


            ReplyReturnThreadLastPage = true;

        }


        /// <summary>
        /// 版块ID
        /// </summary>
        [SettingItem]
        public int ForumID { get; set; }



        /// <summary>
        /// 允许游客访问
        /// </summary>
        [SettingItem]
        public bool AllowGuestVisitForum { get; set; }

        /// <summary>
        /// 游客可以看到当前版块
        /// </summary>
        [SettingItem]
        public bool DisplayInListForGuest { get; set; }

        /// <summary>
        /// 允许访问
        /// </summary>
        [SettingItem]
        public Exceptable<bool> VisitForum { get; set; }


        /// <summary>
        /// 论坛显示在列表里
        /// </summary>
        [SettingItem]
        public Exceptable<bool> DisplayInList { get; set; }

        /// <summary>
        /// 回帖后默认是否跳转到主题的最后一页
        /// </summary>
        [SettingItem]
        public bool ReplyReturnThreadLastPage { get; set; }

        /// <summary>
        /// 开启hidden标签
        /// </summary>
        [SettingItem]
        public bool EnableHiddenTag { get; set; }

        /// <summary>
        /// 开启出售主题
        /// </summary>
        [SettingItem]
        public Exceptable<bool> EnableSellThread { get; set; }



        /// <summary>
        /// 出售主题有效期(单位秒，0为不限制)
        /// </summary>
        [SettingItem]
        public long SellThreadDays { get; set; }

        /// <summary>
        /// 开启出售附件
        /// </summary>
        [SettingItem]
        public Exceptable<bool> EnableSellAttachment { get; set; }


        /// <summary>
        /// 出售附件有效期(单位秒，0为不限制)
        /// </summary>
        [SettingItem]
        public long SellAttachmentDays { get; set; }


        [SettingItem]
        public ThreadSortField DefaultThreadSortField { get; set; }


        /// <summary>
        /// 开启主题评级
        /// </summary>
        [SettingItem]
        public bool EnableThreadRank { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SettingItem]
        public Exceptable<Int32Scope> PostSubjectLengths { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [SettingItem]
        public Exceptable<Int32Scope> PostContentLengths { get; set; }


        /// <summary>
        /// 发帖时间间隔(单位秒)
        /// </summary>
        [SettingItem]
        public Exceptable<int> CreatePostIntervals { get; set; }

        /// <summary>
        /// 离贴子更新时间多久回复主题 主题不会被顶上去（单位秒）
        /// </summary>
        [SettingItem]
        public Exceptable<int> UpdateThreadSortOrderIntervals { get; set; }



        /// <summary>
        /// 离发发主题时间多久以内可以回收自己的主题
        /// </summary>
        [SettingItem]
        public Exceptable<int> RecycleOwnThreadsIntervals { get; set; }

        /// <summary>
        /// 离发发主题时间多久以内可以删除自己主题
        /// </summary>
        [SettingItem]
        public Exceptable<int> DeleteOwnThreadsIntervals { get; set; }

        /// <summary>
        /// 离发发主题时间多久以内可以编辑自己的帖子
        /// </summary>
        [SettingItem]
        public Exceptable<int> UpdateOwnPostIntervals { get; set; }

        /// <summary>
        /// 悬赏帖有效期(单位秒)
        /// </summary>
        [SettingItem]
        public Exceptable<long> QuestionValidDays { get; set; }

        /// <summary>
        /// 辩论有效期(单位秒)
        /// </summary>
        [SettingItem]
        public Exceptable<long> PolemizeValidDays { get; set; }

        /// <summary>
        /// 投票帖有效期(单位秒)
        /// </summary>
        [SettingItem]
        public Exceptable<long> PollValidDays { get; set; }

        /// <summary>
        /// 允许上传的文件类型 (文件扩展名 不包含点)
        /// </summary>
        [SettingItem]
        public Exceptable<ExtensionList> AllowFileExtensions { get; set; }


        /// <summary>
        /// 是否允许图片类型的附件 在帖子中以图片显示 
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowImageAttachment { get; set; }

        /// <summary>
        /// 是否允许上传附件 
        /// </summary>
        [SettingItem]
        public Exceptable<bool> AllowAttachment { get; set; }

        /// <summary>
        /// 单个附件最大大小 为0则不限制
        /// </summary>
        [SettingItem]
        public Exceptable<long> MaxSignleAttachmentSize { get; set; }

        /// <summary>
        /// 主题最大附件个数 为0则不限制
        /// </summary>
        [SettingItem]
        public Exceptable<int> MaxTopicAttachmentCount { get; set; }

        /// <summary>
        /// 帖子最大附件个数 为0则不限制
        /// </summary>
        [SettingItem]
        public Exceptable<int> MaxPostAttachmentCount { get; set; }

        /// <summary>
        /// 用户表情
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowEmoticon { get; set; }



        /// <summary>
        /// 允许使用HTML
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowHTML { get; set; }


        /// <summary>
        /// 允许使用UBB
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowMaxcode { get; set; }



        /// <summary>
        /// 允许使用img标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowImageTag { get; set; }



        /// <summary>
        /// 允许使用Flash标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowFlashTag { get; set; }



        /// <summary>
        /// 允许使用Audio标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowAudioTag { get; set; }

        /// <summary>
        /// 允许使用Video标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowVideoTag { get; set; }


        /// <summary>
        /// 允许使用Table标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowTableTag { get; set; }


        /// <summary>
        /// 允许使用Url标签
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreatePostAllowUrlTag { get; set; }


        /// <summary>
        /// 主题允许使用签名
        /// </summary>
        [SettingItem]
        public Exceptable<bool> ShowSignatureInThread { get; set; }


        /// <summary>
        /// 回复允许使用签名
        /// </summary>
        [SettingItem]
        public Exceptable<bool> ShowSignatureInPost { get; set; }




        /// <summary>
        /// 发表回复需要审核
        /// </summary>
        [SettingItem]
        public Exceptable<bool> ReplyNeedApprove { get; set; }

        /// <summary>
        /// 发表主题需要审核
        /// </summary>
        [SettingItem]
        public Exceptable<bool> CreateThreadNeedApprove { get; set; }

        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return ForumID;
        }

        #endregion
    }

    public class ForumSettingItemCollection : EntityCollectionBase<int,ForumSettingItem>, ISettingItem
    {
        public ForumSettingItem GetForumSettingItem(int forumID)
        {
            ForumSettingItem item; 
                
            if(this.TryGetValue(forumID,out item))
                return item;

            if (forumID != 0)
            {
                Forum forum = ForumBO.Instance.GetForum(forumID, false);
                if (forum != null)
                    return GetForumSettingItem(forum.ParentID);
            }

            if (forumID == 0)
                return new ForumSettingItem();
            return null;
        }

        public override void Add(ForumSettingItem item)
        {
            if (item.ForumID == 0)
            {
                if (this.Count > 0 && this[0].ForumID == 0)
                    this[0] = item;
                else
                    this.Insert(0, item);
            }
            else
                base.Add(item);
        }

        #region ISettingItem 成员

        public string GetValue()
        {
            StringList list = new StringList();

            foreach (ForumSettingItem item in this)
            {
                list.Add(item.ToString());
            }

            return list.ToString();
        }

        public void SetValue(string value)
        {
            StringList list = StringList.Parse(value);

            if (list != null)
            {
                Clear();

                foreach (string item in list)
                {
                    ForumSettingItem forumSettingItem = new ForumSettingItem();

                    forumSettingItem.Parse(item);

                    this.Add(forumSettingItem);

                }
            }
        }

        #endregion
    }
}