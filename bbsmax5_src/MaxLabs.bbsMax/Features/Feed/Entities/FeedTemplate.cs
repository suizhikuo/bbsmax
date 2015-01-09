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

using MaxLabs.bbsMax.DataAccess;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 通知模板
    /// </summary>
    public class FeedTemplate : IPrimaryKey<string>
    {

        public FeedTemplate()
        { 
        }

        public FeedTemplate(DataReaderWrap readerWrap)
        {
            AppID = readerWrap.Get<Guid>("AppID");

            ActionType = (int)readerWrap.Get<byte>("ActionType");

            Title = readerWrap.Get<string>("Title");
            IconSrc = readerWrap.Get<string>("IconUrl");
            Description = readerWrap.Get<string>("Description");
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        public Guid AppID { get; set; }



        /// <summary>
        /// APP的动作
        /// </summary>
        public int ActionType { get; set; }



        /// <summary>
        /// 通知标题模板  例如：{用户}发表了日志{日志标题} --({自定义变量})
        /// 模板变量格式:{变量名称}
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图标地址
        /// </summary>
        public string IconSrc { get; set; }


        public string IconUrl
        {
            get { return UrlUtil.ResolveUrl(this.IconSrc); }
        }


        /// <summary>
        /// 简介模板  例如： {用户}分享日志 {日志标题}&lt;br />来自：{日志作者} &lt;br />{描述} &lt;br />评论该分享
        /// 模板变量格式:{变量名称}
        /// </summary>
        public string Description { get; set; }

        #region IPrimaryKey<int> 成员

        public string GetKey()
        {
            return AppID.ToString() + "-" + ActionType.ToString();
        }

        #endregion
    }

    /// <summary>
    /// 通知模板对象集合
    /// </summary>
    public class FeedTemplateCollection : EntityCollectionBase<string,FeedTemplate> //Dictionary<Guid,Collection<FeedTemplate>>
    {
        Dictionary<Guid, Collection<int>> innerDictionary = new Dictionary<Guid, Collection<int>>();
        public FeedTemplateCollection()
        {
        }

        public FeedTemplateCollection(DataReaderWrap readerWrap)
        {

            while (readerWrap.Next)
            {
                FeedTemplate feedTemplate = new FeedTemplate(readerWrap);
                this.Add(feedTemplate);
            }
        }


        public override void Add(FeedTemplate item)
        {
            Collection<int> actionTypes;
            if (!innerDictionary.TryGetValue(item.AppID, out actionTypes))
            {
                actionTypes = new Collection<int>();
            }
            if(!actionTypes.Contains(item.ActionType))
                actionTypes.Add(item.ActionType);
            base.Add(item);
        }

        public override void Clear()
        {
            innerDictionary.Clear();
            base.Clear();
        }

        public FeedTemplateCollection GetValues(Guid appID)
        {
            FeedTemplateCollection feedTemplates = new FeedTemplateCollection();
            Collection<int> actionTypes = new Collection<int>();
            if (innerDictionary.TryGetValue(appID, out actionTypes))
            {
                foreach (int actionType in actionTypes)
                {
                    FeedTemplate template = GetValue(appID, actionType);
                    if (template != null)
                        feedTemplates.Add(template);
                }
            }
            return feedTemplates;
        }

        public FeedTemplate GetValue(Guid appID,int actionType)
        {
            return GetValue(appID.ToString() + "-" + actionType.ToString());
        }

    }
}