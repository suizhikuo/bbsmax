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

namespace MaxLabs.bbsMax.Logs
{
    [OperationType(Favorite_DeleteFavoriteBySearch.TYPE_NAME)]
    public class Favorite_DeleteFavoriteBySearch : Operation
    {
        public const string TYPE_NAME = "删除收藏";

        public Favorite_DeleteFavoriteBySearch(int operatorID, string operatorName, string operatorIP, Filters.ShareFilter filter, int totalDeleted)
            : base(operatorID, operatorName, operatorIP)
        {
            Filter = filter;
            TotalDeleted = totalDeleted;
        }

        public override string TypeName
        {
            get { return TYPE_NAME; }
        }

        public Filters.ShareFilter Filter
        {
            get;
            private set;
        }

        public int TotalDeleted
        {
            get;
            private set;
        }

        public override string Message
        {
            get
            {
                StringBuffer sb = new StringBuffer();

                if (Filter.ShareID != null)
                    sb += "，并且ID等于" + Filter.ShareID;

                if (Filter.UserID != null)
                    sb += "，并且作者ID等于" + Filter.UserID;

                if (Filter.BeginDate != null && Filter.EndDate != null)
                {
                    sb += "，并且创建时间在" + DateTimeUtil.FormatDateTime(Filter.BeginDate.Value) + "和" + DateTimeUtil.FormatDateTime(Filter.EndDate.Value) + "之间";
                }
                else if (Filter.BeginDate != null)
                {
                    sb += "，并且创建时间晚于或等于" + DateTimeUtil.FormatDateTime(Filter.BeginDate.Value);
                }
                else if (Filter.EndDate != null)
                {
                    sb += "，并且创建时间早于或等于" + DateTimeUtil.FormatDateTime(Filter.EndDate.Value);
                }

                //if (Filter.PrivacyType != null)
                //{
                //    string privacyType = null;

                //    switch (Filter.PrivacyType.Value)
                //    {
                //        case MaxLabs.bbsMax.Enums.PrivacyType.AllVisible:
                //            privacyType = "“所有人都可见”";
                //            break;

                //        case MaxLabs.bbsMax.Enums.PrivacyType.AppointUser:
                //            privacyType = "“仅指定用户可见”";
                //            break;

                //        case MaxLabs.bbsMax.Enums.PrivacyType.FriendVisible:
                //            privacyType = "“仅好友可见”";
                //            break;

                //        case MaxLabs.bbsMax.Enums.PrivacyType.NeedPassword:
                //            privacyType = "“需密码访问”";
                //            break;

                //        case MaxLabs.bbsMax.Enums.PrivacyType.SelfVisible:
                //            privacyType = "“仅作者可见”";
                //            break;
                //    }

                //    sb += "，并且隐私类型是" + privacyType;
                //}

                if (Filter.ShareType != null)
                {
                    switch (Filter.ShareType.Value)
                    {
                        case MaxLabs.bbsMax.Enums.ShareType.Album:
                            sb += "，并且收藏类型为“相册”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Blog:
                            sb += "，并且收藏类型为“博客”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Flash:
                            sb += "，并且收藏类型为“Flash”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Forum:
                            sb += "，并且收藏类型为“群组”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Music:
                            sb += "，并且收藏类型为“音乐”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.News:
                            sb += "，并且收藏类型为“新闻”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Picture:
                            sb += "，并且收藏类型为“图片”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Topic:
                            sb += "，并且收藏类型为“话题”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.URL:
                            sb += "，并且收藏类型为“链接”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.User:
                            sb += "，并且收藏类型为“用户”";
                            break;
                        case MaxLabs.bbsMax.Enums.ShareType.Video:
                            sb += "，并且收藏类型为“视频”";
                            break;
                    }
                }

                if (Filter.Username != null && Filter.Username != string.Empty)
                {
                    sb += "，并且作者名称是" + Filter.Username;
                }

                string con = sb.ToString();

                if (sb.Length > 0)
                    con = con.Substring(3);

                return string.Format(
                    "<a href=\"{0}\">{1}</a> 使用删除搜索结果功能，删除了所有 {2} 的收藏，共计{3}条数据"
                    , BbsRouter.GetUrl("space/" + OperatorID)
                    , OperatorName
                    , con
                    , TotalDeleted
                );
            }
        }
    }
}