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

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Filters;


namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class TagTemplateMembers
    {


        #region 标签委托及委托参数

        public class TagListHeadFootParams
        {
            private int? m_TotalTags = null;
            private AdminTagFilter m_Filter = null;
            private int m_PageSize = Consts.DefaultPageSize;

            public TagListHeadFootParams() { }

            public TagListHeadFootParams(int? totalTags, int pageSize)
            {
                m_TotalTags = totalTags;
                m_PageSize = pageSize;
            }

            public TagListHeadFootParams(int? totalTags, AdminTagFilter filter, int pageSize)
            {
                m_TotalTags = totalTags;
                m_Filter = filter;
                m_PageSize = pageSize;
            }

            public bool HasItems
            {
                get { return m_TotalTags != null && m_TotalTags > 0; }
            }

            public int TotalTags
            {
                get { return m_TotalTags != null ? m_TotalTags.Value : 0; }
            }

            public int PageSize
            {
                get { return m_PageSize; }
            }

            public AdminTagFilter AdminForm
            {
                get { return m_Filter; }
            }
        }

        public class TagListItemParams
        {
            private Tag m_Tag = null;
            private TagType m_TagType = TagType.Blog;

            public TagListItemParams() { }

            public TagListItemParams(Tag tag)
            {
                m_Tag = tag;
            }

            public TagListItemParams(Tag tag, TagType type)
            {
                m_Tag = tag;
                m_TagType = type;
            }

            public Tag Tag
            {
                get { return m_Tag; }
            }

            public bool CanDisplayBlogArticle
            {
                get { return m_TagType == TagType.Blog; }
            }

        }

        public delegate void TagListHeadFootTemplate(TagListHeadFootParams _this);
        public delegate void TagListItemTemplate(TagListItemParams _this, int i);

        #endregion

        #region 列表

        /// <summary>
        /// 标签列表
        /// </summary>
        [TemplateTag]
        public void TagList(
             int pageNumber
            , TagListHeadFootTemplate head
            , TagListHeadFootTemplate foot
            , TagListItemTemplate item)
        {
            #region 标签列表

            int pageSize = Consts.DefaultTagListPageSize;
            int? count = null;
            TagCollection tags = TagBO.Instance.GetUnlockTags(pageNumber, pageSize, ref count);
            TagListHeadFootParams headFootParams = new TagListHeadFootParams(count, pageSize);
            head(headFootParams);

            if (tags != null && tags.Count > 0)
            {
                int i = 0;
                foreach (Tag tag in tags)
                {
                    TagListItemParams itemParams = new TagListItemParams(tag);
                    item(itemParams, i++);
                }
            }

            foot(headFootParams);

            #endregion
        }

        /// <summary>
        /// 标签搜索列表
        /// </summary>
        [TemplateTag]
        public void TagSearchList(
              string filter
            , int pageNumber
            , TagListHeadFootTemplate head
            , TagListHeadFootTemplate foot
            , TagListItemTemplate item)
        {
            #region 标签搜索列表

            int? count = null;
            TagCollection tags = null;
            int pageSize = Consts.DefaultPageSize;

            TagListHeadFootParams headFootParams;
            AdminTagFilter tagFilter = AdminTagFilter.GetFromFilter(filter);
            if (tagFilter != null)
            {
                tags = TagBO.Instance.GetTagsBySearch(tagFilter, pageNumber, ref count);
                pageSize = tagFilter.PageSize;
                headFootParams = new TagListHeadFootParams(count, tagFilter, pageSize);
            }
            else
            {
                tags = TagBO.Instance.GetAllTags(pageNumber, pageSize, ref count);

                headFootParams = new TagListHeadFootParams(count, new AdminTagFilter(), pageSize);
            }

            head(headFootParams);

            if (tags != null && tags.Count > 0)
            {
                int i = 0;
                foreach (Tag tag in tags)
                {
                    TagListItemParams itemParams = new TagListItemParams(tag);
                    item(itemParams, i++);
                }
            }

            foot(headFootParams);

            #endregion
        }

        #endregion

        #region 单条数据

        [TemplateTag]
        public void TagView(
              int tagID
            , string type
            , TagListItemTemplate item
        )
        {
            #region 单条数据

            Tag tag = TagBO.Instance.GetTag(tagID);

            TagType tagType = StringUtil.TryParse<TagType>(type, TagType.Blog);

            TagListItemParams listItemParams = new TagListItemParams(tag, tagType);

            item(listItemParams, 0);

            #endregion
        }

        #endregion
    }
}