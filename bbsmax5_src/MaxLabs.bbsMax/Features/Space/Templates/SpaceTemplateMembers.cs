//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Templates
{
    [TemplatePackage]
    public class SpaceTemplateMembers
    {
        //public delegate void SpaceViewTemplate(Space space, bool isCanVisitor, bool isFriend, bool isSelf);
        //public delegate void SpacePrivacyViewTemplate(Space space);

        public delegate void LastVisitorListItemTemplate(Visitor visitor);
        public delegate void LastVisitorHeadFootTemplate(int totalCount, bool hasItems);
        public delegate void SpaceTabTemplate(SpaceTabParam _this, bool isSelf);

        public class SpaceTabParam
        {
            private string m_Selected;
            private int m_UserID;
            public SpaceTabParam(int userID, string selected)
            {
                this.m_Selected = selected;
                this.m_UserID = userID;
            }

            public bool SelectedHome
            {
                get
                {
                    if (string.Compare(m_Selected, "home", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowHome
            {
                get
                {
                    return true;
                }
            }

            public bool SelectedDoing
            {
                get
                {
                    if (string.Compare(m_Selected, "doing", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowDoing
            {
                get
                {
                    if (SpaceBO.Instance.IsSpaceElementShow(SpaceElement.Doing, m_UserID))
                        return true;

                    return false;
                }
            }

            public bool SelectedBlog
            {
                get
                {
                    if (string.Compare(m_Selected, "blog", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowBlog
            {
                get
                {
                    if (SpaceBO.Instance.IsSpaceElementShow(SpaceElement.Blog, m_UserID))
                        return true;

                    return false;
                }
            }

            public bool SelectedAlbum
            {
                get
                {
                    if (string.Compare(m_Selected, "album", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowAlbum
            {
                get
                {
                    if (SpaceBO.Instance.IsSpaceElementShow(SpaceElement.Album, m_UserID))
                        return true;

                    return false;
                }
            }

            public bool SelectedShare
            {
                get
                {
                    if (string.Compare(m_Selected, "share", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowShare
            {
                get
                {
                    if (SpaceBO.Instance.IsSpaceElementShow(SpaceElement.Share, m_UserID))
                        return true;

                    return false;
                }
            }

            public bool SelectedBoard
            {
                get
                {
                    if (string.Compare(m_Selected, "board", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowBoard
            {
                get
                {
                    if (SpaceBO.Instance.IsSpaceElementShow(SpaceElement.Board, m_UserID))
                        return true;

                    return false;
                }
            }

            public bool SelectedFriend
            {
                get
                {
                    if (string.Compare(m_Selected, "friend", true) == 0)
                        return true;

                    return false;
                }
            }

            public bool ShowFriend
            {
                get
                {
                    if (SpaceBO.Instance.IsSpaceElementShow(SpaceElement.Friend, m_UserID))
                        return true;

                    return false;
                }
            }
        }


        [TemplateTag]
        public void LastVisitorList(int userID, int pageNumber, int pageSize, LastVisitorHeadFootTemplate head, LastVisitorHeadFootTemplate foot, LastVisitorListItemTemplate item)
        {
            int totalCount;
            bool hasItems = true;
            VisitorCollection visitors = SpaceBO.Instance.GetVisitors(userID, pageNumber, pageSize, out totalCount);
            if (totalCount == 0)
                hasItems = false;

            head(totalCount, hasItems);

            foreach (Visitor visitor in visitors)
            {
                item(visitor);
            }

            foot(totalCount, hasItems);
        }

        [TemplateTag]
        public void LastVisitorList(int pageNumber, int pageSize, LastVisitorHeadFootTemplate head, LastVisitorHeadFootTemplate foot, LastVisitorListItemTemplate item)
        {
            int totalCount;
            bool hasItems = true;
            int userID = UserBO.Instance.GetCurrentUserID();
            VisitorCollection visitors = SpaceBO.Instance.GetVisitors(userID, pageNumber, pageSize, out totalCount);
            if (totalCount == 0)
                hasItems = false;
            head(totalCount, hasItems);

            foreach (Visitor visitor in visitors)
            {
                item(visitor);
            }

            foot(totalCount, hasItems);
        }

        [TemplateTag]
        public void LastVisitorList(int userID, int count, LastVisitorHeadFootTemplate head, LastVisitorListItemTemplate item)
        {
            bool hasItems = true;

            VisitorCollection visitors = SpaceBO.Instance.GetVisitors(userID, count);
            if (visitors.Count == 0)
                hasItems = false;

            head(0, hasItems);

            foreach (Visitor visitor in visitors)
            {
                item(visitor);
            }
        }

        [TemplateTag]
        public void LastVisitorList(int count, LastVisitorHeadFootTemplate head, LastVisitorListItemTemplate item)
        {
            int userID = UserBO.Instance.GetCurrentUserID();

            LastVisitorList(userID, count, head, item);
        }

        //[TemplateTag]
        //public void UpdateLastVisitor(int userID)
        //{
        //    SpaceBO.Instance.ModifyVisitor(userID, IPUtil.GetCurrentIP());
        //}

        [TemplateTag]
        public void SpaceTab(int userID, string selected, SpaceTabTemplate template)
        {
            bool isSelf = false;
            if (userID == UserBO.Instance.GetCurrentUserID())
                isSelf = true;
            template(new SpaceTabParam(userID, selected), isSelf);
        }

        public delegate void ThemeListItemTemplate(SpaceTheme theme, int i);

        [TemplateTag]
        public void ThemeList(ThemeListItemTemplate item)
        {
            int i = 0;

            foreach (SpaceTheme theme in SpaceBO.Instance.GetSpaceThemes())
            {
                i++;
                item(theme, i);
            }
        }

    }
}