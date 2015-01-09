//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using System;
using System.Collections.Generic;
using System.Collections.Specialized;

using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax.DataAccess;
using System.Collections;
using System.Text;

namespace MaxLabs.bbsMax.Entities
{
    /// <summary>
    /// 帖子评分信息
    /// </summary>
    public class PostMark : IPrimaryKey<int>
    {
        public PostMark()
        {

        }

        public PostMark(DataReaderWrap readerWrap)
        {
            this.PostMarkID = readerWrap.Get<int>("PostMarkID");
            this.PostID = readerWrap.Get<int>("PostID");
            this.UserID = readerWrap.Get<int>("UserID");
            this.CreateDate = readerWrap.Get<DateTime>("CreateDate");
            this.Reason = readerWrap.Get<string>("Reason");
            this.Username = readerWrap.Get<string>("Username");

            Points = new int[8];
            for (int i = 0; i < 8; i++)
            {
                Points[i] = readerWrap.Get<int>("ExtendedPoints_" + (i + 1));
            }
        }

        public int PostMarkID { get; set; }

        public int PostID { get; set; }

        public int UserID { get; set; }

        public DateTime CreateDate { get; set; }

        public string Reason { get; set; }

        public string Username { get; set; }

        public int[] Points { get; set; }


        /// <summary>
        /// 评分扩展积分
        /// </summary>
        private string m_PostMarkExtentedPoints = null;
        public string PostMarkExtentedPoints
        {
            get
            {

                if (m_PostMarkExtentedPoints == null)
                {
                    StringBuilder builder = new StringBuilder();

                    UserPointCollection userPoints = AllSettings.Current.PointSettings.UserPoints;

                    for (int i = 0; i < 8; i++)
                    {
                        UserPoint ex = userPoints[i];
                        int total = Points[i];
                        if (ex.Enable)
                        {
                            if (total != 0)
                            {
                                builder.Append(ex.Name);
                                builder.Append(":&nbsp;");

                                if (total > 0)
                                    builder.Append("+");

                                builder.Append(total);
                                builder.Append("&nbsp;");
                            }
                        }
                    }
                    m_PostMarkExtentedPoints = builder.ToString();
                }
                return m_PostMarkExtentedPoints;
            }
        }

        public string GetPostMarkPoints(string addPointStyle, string reducePointStyle)
        {
            UserPointCollection userPoints = AllSettings.Current.PointSettings.UserPoints;
            StringBuilder pointBuilder = new StringBuilder();
            for (int i = 0; i < 8; i++)
            {
                UserPoint point = userPoints[i];
                int total = Points[i];
                if (point.Enable)
                {
                    if(total>0)
                    {
                        pointBuilder.Append(string.Format(addPointStyle, point.Name, total));
                    }
                    else if (total < 0)
                    {
                        pointBuilder.Append(string.Format(reducePointStyle, point.Name, total));
                    }
                }
            }

            return pointBuilder.ToString();
        }


        #region IPrimaryKey<int> 成员

        public int GetKey()
        {
            return PostMarkID;
        }

        #endregion
    }

    public class PostMarkCollection : EntityCollectionBase<int, PostMark>
    {
        public PostMarkCollection()
        { }

        public PostMarkCollection(DataReaderWrap readerWrap)
        {
            while (readerWrap.Next)
            {
                PostMark postMark = new PostMark(readerWrap);

                this.Add(postMark);
            }
        }
    }
}