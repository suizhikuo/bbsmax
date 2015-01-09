//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Web;
using System.Collections.Generic;
using System.Web.UI;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Common;
using MaxLabs.bbsMax;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.FileSystem;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.WebEngine;
using System.Text;
using MaxLabs.bbsMax.ValidateCodes;
using System.Text.RegularExpressions;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Logs;


namespace MaxLabs.bbsMax.Web.max_pages.forums
{
    /// <summary>
    /// Post 的摘要说明
    /// </summary>
    public partial class view_poll : view_thread
    {

        protected new void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void ProcessSubmits()
        {
            if (_Request.IsClick("voteButton"))
            {
                UpdateView = false;
                ProcessVote();
            }
            else
                base.ProcessSubmits();
        }

        protected override BasicThread Thread
        {
            get
            {
                return PollThread;
            }
        }

        private PollThreadV5 m_PollThread;
        protected PollThreadV5 PollThread
        {
            get
            {
                if (m_PollThread == null)
                {
                    GetThread();
                }
                return m_PollThread;
            }
        }

        protected override void GetThread()
        {
            if (IsOnlyLookOneUser)
            {
                Response.Redirect(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, 1, LookUserID, PostBOV5.Instance.GetThreadTypeString(ThreadType.Normal), ForumListPage));
            }

            if (IsUnapprovePosts)
            {
                BbsRouter.JumpToUrl(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(ThreadType.Normal)), "type=" + Type);
            }

            int? total;
            if (string.IsNullOrEmpty(Type))
            {
                ThreadType realThreadType;

                PostBOV5.Instance.GetPollWithReplies(ThreadID, PageNumber, PageSize, true, UpdateView, out m_PollThread, out m_PostList, out realThreadType);

                //如果不是 投票 则跳到相应的页面
                if (realThreadType != ThreadType.Poll)
                {
                    Response.Redirect(BbsUrlHelper.GetThreadUrl(CodeName, ThreadID, PostBOV5.Instance.GetThreadTypeString(realThreadType)));
                }
            }
            else
            {
                BasicThread thread;
                GetPosts(ThreadType.Poll, out m_TotalPosts, out thread, out m_PostList);

                m_PollThread = (PollThreadV5)thread;

            }

            PostBOV5.Instance.ProcessKeyword(m_PostList, ProcessKeywordMode.TryUpdateKeyword);
            //if (_Request.IsSpider == false)
            //{
                //List<int> userIDs = new List<int>();
                //foreach (PostV5 post in m_PostList)
                //{
                //    userIDs.Add(post.UserID);
                //}
                //UserBO.Instance.GetUsers(userIDs, GetUserOption.WithAll);
            UserBO.Instance.GetUsers(m_PostList.GetUserIds(), GetUserOption.WithAll);
            //}
        }

        private int? m_TotalPosts;
        protected override int TotalPosts
        {
            get
            {
                if (m_TotalPosts == null)
                {
                    return base.TotalPosts;
                    //GetThread();
                    //if (m_TotalPosts == null)
                    //    m_TotalPosts = base.TotalPosts;
                }
                return m_TotalPosts.Value;
            }
        }


        private string m_VoteType;
        protected string VoteType
        {
            get
            {
                if (m_VoteType == null)
                {
                    if (PollThread.Multiple < 2)
                        m_VoteType = "单选";
                    else
                    {
                        int count;
                        if (PollThread.PollItems.Count > PollThread.Multiple)
                            count = PollThread.Multiple;
                        else
                            count = PollThread.PollItems.Count;
                        m_VoteType = "多选，最多只能选" + count + "项";
                    }
                }
                return m_VoteType;
            }
        }

        private int? m_VoteTotalCount;
        protected int VoteTotalCount
        {
            get
            {
                if (m_VoteTotalCount == null)
                {
                    int count = 0;
                    foreach (PollItem pollitem in PollThread.PollItems)
                    {
                        count += pollitem.PollItemCount;
                    }
                    m_VoteTotalCount = count;
                }

                return m_VoteTotalCount.Value;
            }
        }


        protected string GetVoteImageWidthPercent(PollItem pollItem, int voteTotalCount, int widthPercent)
        {
            voteTotalCount = voteTotalCount == 0 ? 1 : voteTotalCount;
            return (((float)pollItem.PollItemCount / (float)voteTotalCount) * widthPercent).ToString();
        }


        private bool? m_CanViewPollDetail;
        protected bool CanViewPollDetail
        {
            get
            {
                if (m_CanViewPollDetail == null)
                {
                    if (Can(ForumPermissionSetNode.Action.ViewPollDetail))
                    {
                        if (MyUserID == Thread.PostUserID || AlwaysViewContents || PollThread.IsVoted(MyUserID))
                            m_CanViewPollDetail = true;
                        else
                            m_CanViewPollDetail = false;
                    }
                    else
                        m_CanViewPollDetail = false;
                }

                return m_CanViewPollDetail.Value;
            }
        }

        private bool? m_CanViewPollItemVotedCount;
        protected bool CanViewPollItemVotedCount
        {
            get
            {
                if (m_CanViewPollItemVotedCount == null)
                { 
                    if (MyUserID == Thread.PostUserID || AlwaysViewContents || PollThread.AlwaysEyeable || PollThread.ExpiresDate < DateTimeUtil.Now || PollThread.IsVoted(MyUserID))
                        m_CanViewPollItemVotedCount = true;
                    else
                        m_CanViewPollItemVotedCount = false;
                }
                return m_CanViewPollItemVotedCount.Value;
            }
        }


        private void ProcessVote()
        {
            string validateCodeAction = "Vote";
            MessageDisplay msgDisplay = CreateMessageDisplay("vote");
            if (CheckValidateCode(validateCodeAction, msgDisplay))
            {
                int[] itemIDs = _Request.GetList<int>("pollItem", Method.Post, new int[0] { });

                try
                {
                    bool success = PostBOV5.Instance.Vote(My, itemIDs, ThreadID);
                    if (success == false)
                    {
                        CatchError<ErrorInfo>(delegate(ErrorInfo error)
                        {
                            if (error is UserPointOverMinValueError)
                                msgDisplay.AddError("vote", "您的积分不足，不能进行投票");
                            else
                                msgDisplay.AddError("vote", error.Message);
                        });
                    }
                    else
                    {
                        _Request.Clear();
                        ValidateCodeManager.CreateValidateCodeActionRecode(validateCodeAction);
                        AlertSuccess("投票成功");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogException(ex);
                    msgDisplay.AddError("vote", ex.Message);
                }
            }
        }

    }
}