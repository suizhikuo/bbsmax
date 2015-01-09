//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Rescourses;
using MaxLabs.bbsMax.ValidateCodes;
using System.Collections.Generic;
using MaxLabs.bbsMax.Settings;
using System.Text;
using MaxLabs.bbsMax.Entities;

namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class post_rate : DialogPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Post == null)
            {
                ShowError(new InvalidParamError("postid").Message);
            }
            if (RateSetList.Count == 0)
            {
                ShowError("您所在的用户组没有权限评分!");
            }
            if (Post.UserID == MyUserID)
            {
                ShowError("您不能给自己评分");
            }

            if (_Request.IsClick("ratepost"))
            {
                RatePost();
            }
        }

        private void RatePost()
        {
            MessageDisplay msgDisplay = CreateMessageDisplay("point");

            if (Post == null)
            {
                msgDisplay.AddError(new InvalidParamError("postid").Message);
                return;
            }

            int[] points = new int[8];

            int total = 0;
            foreach (UserPoint userPoint in AllSettings.Current.PointSettings.EnabledUserPoints)
            {
                string valueString = _Request.Get("point_" + userPoint.Type.ToString(), Method.Post, string.Empty);
                int value = 0;

                if (valueString != string.Empty)
                {
                    if (int.TryParse(valueString, out value) == false)
                    {
                        long temp;
                        if (long.TryParse(valueString, out temp))
                        {
                            msgDisplay.AddError("point", "您评的"+ userPoint.Name + "已超出系统允许的最大值");
                            return;
                        }

                        msgDisplay.AddError("point", userPoint.Name + "必须为整数");
                        return;
                    }
                }

                points[(int)userPoint.Type] = value;

                total += value;
            }

            if (total == 0)
            {
                msgDisplay.AddError("point", "请填写积分值");
                return;
            }

            string reason = _Request.Get("actionReasonText",Method.Post);

            bool sendMessage = _Request.Get("cbsendMessage", Method.Post, "0") == "1";


            try
            {
                if (!PostBOV5.Instance.RatePost(My, Post.UserID, Post, points, reason, sendMessage))
                {
                    CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        if (error is UserPointOverMaxValueError)
                        {
                            UserPointOverMaxValueError maxValueError = (UserPointOverMaxValueError)error;

                            string info;
                            if (maxValueError.CanAddValue == 0)
                                info = "您已经不能再给TA评分";
                            else
                                info = "您最多只能给TA加“+" + maxValueError.CanAddValue + "”";


                            msgDisplay.AddError("出错啦！评分后，用户“" + Post.Username + "”的“" + maxValueError.UserPoint.Name + "”已经超过系统允许的最大值，" + info);
                        }
                        else if (error is UserPointOverMinValueError)
                        {
                            UserPointOverMinValueError minValueError = (UserPointOverMinValueError)error;

                            string info;
                            if (minValueError.CanReduceValue == 0)
                                info = "您已经不能再给TA评分";
                            else
                                info = "您最多只能给TA加“-" + minValueError.CanReduceValue + "”";

                            msgDisplay.AddError("出错啦！评分后，用户“" + Post.Username + "”的“" + minValueError.UserPoint.Name + "”已经超过系统允许的最小值，" + info);
                        }
                        else
                            msgDisplay.AddError(error);
                    });
                }
                else
                {
                    Return(true);
                }
            }
            catch (Exception ex)
            {
                msgDisplay.AddError(ex.Message);
            }
        }

        private string m_PostAlias;
        protected string PostAlias
        {
            get
            { 
                if(m_PostAlias == null)
                {
                    m_PostAlias = _Request.Get("PostAlias", Method.Get, "");
                }
                return m_PostAlias;
            }
        }

        private RateSetItemCollection m_RateSetList;
        protected RateSetItemCollection RateSetList
        {
            get
            {
                if (m_RateSetList == null)
                {
                    m_RateSetList = AllSettings.Current.RateSettings.RateSets.GetRateSet(Post.ForumID).GetRateItems(MyUserID);
                }
                return m_RateSetList;
            }
        }

        private PostV5 post;
        protected PostV5 Post
        {
            get
            {
                if (post == null)
                {
                    int postID = _Request.Get<int>("postid", Method.Get, 0);
                    if (postID > 0)
                        post = PostBOV5.Instance.GetPost(postID, false);
                }
                return post;
            }
        }

        private RateSetItemCollection m_TopRateSets;
        protected RateSetItemCollection TopRateSets
        {
            get
            {
                if (m_TopRateSets == null)
                    m_TopRateSets = AllSettings.Current.RateSettings.RateSets.GetRateSet(0).GetRateItems(MyUserID);

                return m_TopRateSets;
            }
        }


        protected int GetTodayValue(RateSetItem item)
        {
            int value = 0;

            foreach (PostMark postMark in PostMarks)
            {
                value += Math.Abs(postMark.Points[(int)item.PointType]);
            }

            int maxValue = 0;

            foreach (RateSetItem tempSet in TopRateSets)
            {
                if (tempSet.PointType == item.PointType)
                {
                    maxValue = item.MaxValueInTime;
                    break;
                }
            }

            int temp = maxValue - value;

            if (temp > 0)
                return temp;
            else
                return 0;
        }

        private PostMarkCollection m_postMarks;
        protected PostMarkCollection PostMarks
        {
            get
            {
                if (m_postMarks == null)
                {
                    m_postMarks = PostBOV5.Instance.GetUserTodayPostMarks(MyUserID);
                }
                return m_postMarks;
            }
        }

    }
}