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
using System.Collections.Generic;
using MaxLabs.bbsMax.Entities;
using MaxLabs.bbsMax.Enums;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Settings;
using MaxLabs.bbsMax.PointActions;
using MaxLabs.bbsMax.Errors;


namespace MaxLabs.bbsMax.Web.max_dialogs
{
    public partial class finalquestion : DialogPageBase
    {
        protected const int pageSize = 10;
        protected PostCollectionV5 PostList;
        protected QuestionThread question;
        protected UserPoint TradePoint;
        protected List<int> PostIDs;
        protected void Page_Load(object sender, EventArgs e)
        {
            int threadID = _Request.Get<int>("threadID", Method.Get, 0);

            if (threadID < 1)
                ShowError(new InvalidParamError("threadID").Message);

            int pageNumber = _Request.Get<int>("page",Method.Get,1);

            ThreadType realType;
            PostBOV5.Instance.GetQuestionWithReplies(threadID, pageNumber, pageSize, true, false, out question, out PostList, out realType);

            if(realType!= ThreadType.Question)
                ShowError("该主题不是问题帖");

            if (question == null)
                ShowError("该问题不存在或者已被删除！");

            if (question.IsClosed)
                ShowError("该问题已经结束");

            if (MyUserID != question.PostUserID)
            {
                if (!AllSettings.Current.ManageForumPermissionSet.Nodes.GetPermission(question.ForumID).Can(My, ManageForumPermissionSetNode.ActionWithTarget.FinalQuestion, question.PostUserID))
                    ShowError("您不能结" + UserBO.Instance.GetUser(question.PostUserID, GetUserOption.WithAll).Name + "的帖子");

            }

            TradePoint = ForumPointAction.Instance.GetUserPoint(question.PostUserID, ForumPointValueType.QuestionReward, question.ForumID);

            WaitForFillSimpleUsers<PostV5>(PostList, 1);

            PostIDs = StringUtil.Split2<int>(_Request.Get("postids", Method.Get, string.Empty));

            SetPager("list", null, pageNumber, pageSize, question.TotalReplies + 1);


            if (_Request.IsClick("finalButton"))
            {
                processSubmit();
            }

        }

        protected bool Checked(int postID)
        {
            return PostIDs.Contains(postID);
        }


        protected int BestPostID = 0;
        protected Dictionary<int, string> Rewards = new Dictionary<int, string>();

        protected string GetRewardString(int postID)
        {
            if (Rewards.ContainsKey(postID))
                return Rewards[postID];
            return string.Empty;
        }

        private void processSubmit()
        {
            int threadID = _Request.Get<int>("threadID", Method.Get, 0);
            //int[] postids = _Request.GetList<int>("postid", Method.Post, new int[] { });
            string[] postIDs = _Request.Get("postid", Method.Post, string.Empty).Split(',');

            Dictionary<int, int> rewards = new Dictionary<int, int>();
            foreach (string id in postIDs)
            {
                int tempID;
                if (int.TryParse(id, out tempID))
                {
                    int reward = _Request.Get<int>("reward_" + tempID, Method.Post, 0);
                    if (reward > 0)
                        rewards.Add(tempID, reward);
                    Rewards.Add(tempID, _Request.Get("reward_" + tempID, Method.Post, string.Empty));
                }
            }

            int bestPostID = _Request.Get<int>("bestPostID", Method.Post, 0);
            BestPostID = bestPostID;

            MessageDisplay msgDisplay = CreateMessageDisplay();
            bool success = false;


            using (ErrorScope es = new ErrorScope())
            {
                try
                {
                    success = PostBOV5.Instance.FinalQuestion(My, threadID, bestPostID, rewards);
                }
                catch (Exception ex)
                {
                    msgDisplay.AddException(ex);
                }

                if (success)
                {
                    ShowSuccess("结帖成功！", true);
                }
                else
                {
                    es.CatchError<ErrorInfo>(delegate(ErrorInfo error)
                    {
                        msgDisplay.AddError(error);
                    });

                    PostIDs = new List<int>();
                    foreach (string id in postIDs)
                    {
                        int tempID;
                        if (int.TryParse(id, out tempID))
                            PostIDs.Add(int.Parse(id));
                    }
                }
            }

        }
    }
}