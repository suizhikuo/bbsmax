//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Text;
using System.Collections.Generic;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.DataAccess;
using System.Data;
using System.Collections.ObjectModel;

namespace MaxLabs.bbsMax.Entities
{
    public class Denouncing : IFillSimpleUser
    {

        public Denouncing()
        { }

        public Denouncing(DataReaderWrap readerWrap)
        {
			this.DenouncingID = readerWrap.Get<int>("DenouncingID");
            this.TargetID = readerWrap.Get<int>("TargetID");
			this.Type = readerWrap.Get<DenouncingType>("Type");
            this.IsIgnore = readerWrap.Get<bool>("IsIgnore");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
			this.TargetUserID = readerWrap.Get<int>("TargetUserID");
			this.ContentList = new DenouncingContentCollection();
        }

		public int DenouncingID { get; set; }

        [Obsolete]
		public int ID { get { return DenouncingID; } }

        /// <summary>
        /// 举报对象ID
        /// </summary>
        public int TargetID { get; set; }

        /// <summary>
        /// 举报对像类型
        /// </summary>
		public DenouncingType Type { get; set; }

        /// <summary>
        /// <summary>
        /// 是否已忽略
        /// </summary>
        public bool IsIgnore { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }

		public int TargetUserID
		{
			get;
			set;
		}

		public DenouncingContentCollection ContentList { get; set; }

        public Photo TargetPhoto
        {
            get;
            set;
        }

        public BlogArticle TargetArticle
        {
            get;
            set;
        }

        public SimpleUser TargetUser
        {
            get { return UserBO.Instance.GetFilledSimpleUser(this.TargetUserID); }
        }

        public Share TargetShare
        {
            get;
            set;
        }

        public BasicThread TargetTopic
        {
            get;
            set;
        }

        public PostV5 TargetReply
        {
            get;
            set;
        }

        #region IFillSimpleUser 成员

        public int GetUserIDForFill()
        {
            return this.TargetUserID;
        }

        #endregion
    }

    public class DenouncingCollection : Collection<Denouncing>
    {
        public DenouncingCollection()
        { }

        public DenouncingCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                Denouncing report = new Denouncing(readerWrap);
                this.Add(report);
            }
        }

		public int TotalRecords { get; set; }

		public int[] GetDenouncingIDs()
		{
			int[] result = new int[this.Count];

			for (int i = 0; i < result.Length; i++)
			{
				result[i] = this[i].DenouncingID;
			}

			return result;
		}

		public Denouncing GetValue(int id)
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (this[i].DenouncingID == id)
					return this[i];
			}

			return null;
		}
	}

	public class DenouncingContent : IFillSimpleUser
	{
		public DenouncingContent(DataReaderWrap readerWrap)
		{
			this.DenouncingID = readerWrap.Get<int>("DenouncingID");
			this.UserID = readerWrap.Get<int>("UserID");
			this.Content = readerWrap.Get<string>("Content");
			this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
		}

		public int DenouncingID { get; set; }

		/// <summary>
		/// 举报内容
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 举报用户ID
		/// </summary>
		public int UserID { get; set; }

		public SimpleUser User { get { return UserBO.Instance.GetFilledSimpleUser(this.UserID); } }
		
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDate { get; set; }

		#region IFillSimpleUser 成员

		public int GetUserIDForFill()
		{
			return this.UserID;
		}

		#endregion
	}

	public class DenouncingContentCollection : Collection<DenouncingContent>
	{
		public DenouncingContentCollection()
		{
		}

		public DenouncingContentCollection(DataReaderWrap readerWrap)
		{
			while (readerWrap.Next)
			{
				DenouncingContent item = new DenouncingContent(readerWrap);
				this.Add(item);
			}
		}
	}
}