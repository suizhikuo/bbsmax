//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//


using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine.Plugin;
using System.Collections;
using System;
using System.Text;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Common;

namespace MaxLabs.bbsMax.Web.plugins
{
    public class DefaultMissions : PluginBase
    {
        public override string DisplayName
        {
            get
            {
                return "默认用户任务类型包";
            }
        }

        public override string Description
        {
            get
            {
                return "此插件用于提供系统默认的用户任务类型";
            }
        }

        public override void Initialize()
        {
            MissionBO.Instance.RegisterMission(new ActiveEmailMission());
            MissionBO.Instance.RegisterMission(new AlbumMission());
            MissionBO.Instance.RegisterMission(new AvatarMission());
            MissionBO.Instance.RegisterMission(new BlogMission());
            MissionBO.Instance.RegisterMission(new DoingMission());
            MissionBO.Instance.RegisterMission(new FriendMission());
            MissionBO.Instance.RegisterMission(new InviteMission());
            MissionBO.Instance.RegisterMission(new LoginMission());
            //MissionBO.Instance.RegisterMission(new ProfileMission());
            MissionBO.Instance.RegisterMission(new ShareMission());
            MissionBO.Instance.RegisterMission(new TopicMission());
            MissionBO.Instance.RegisterMission(new MissionGroup());
        }
    }

    public class ActiveEmailMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.ActiveEmailMission"; }
        }

        public override string Name
        {
            get { return "邮箱类"; }
        }


        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                return null;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                System.Collections.Generic.List<string> procedures = new System.Collections.Generic.List<string>();
                procedures.Add("<a href=\"" + BbsRouter.GetUrl("my/account") + "\" target=\"_blank\">新窗口打开账号设置页面</a>；");
                procedures.Add("在新打开的设置页面中，将自己的邮箱真实填写，并点击“验证邮箱”按钮；");
                procedures.Add("几分钟后，系统会给你发送一封邮件，收到邮件后，请按照邮件的说明，访问邮件中的验证链接就可以了。");
                return procedures;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/ActiveEmail.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        public override Hashtable CheckValues(StringTable values)
        {
            return null;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;

            if (UserBO.Instance.GetUser(userID).EmailValidated)
                return 1;
            else
                return 0;
        }

    }

    public class AlbumMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.AlbumMission"; }
        }

        public override string Name
        {
            get { return "相册类"; }
        }


        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string input_Action = prefix + "action";
        private const string input_Count = prefix + "count";
        private const string input_Timeout = prefix + "timeout";

        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();

                inputNames.Add(input_Action);
                inputNames.Add(input_Count);
                inputNames.Add(input_Timeout);

                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Album.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            StringBuffer description = new StringBuffer();

            string value_Timeout = values[input_Timeout];
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];

            if (string.IsNullOrEmpty(value_Timeout) == false && value_Timeout != "0")
                description += "从申请任务开始计时，" + value_Timeout + " 小时内，";

            if (value_Action == "1")
            {
                description += "上传图片";
            }

            description += "达到" + value_Count + "张。";

            return description.ToString();
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count;

            if (int.TryParse(value_Count, out count))
            {
                if (count < 1)
                    result.Add(input_Count, "数量必须大于0");
            }
            else
                result.Add(input_Count, "数量必须为整数");

            int timeout;

            if (string.IsNullOrEmpty(value_Timeout) == false)
            {
                if (int.TryParse(value_Timeout, out timeout))
                {
                    if (count < 0)
                        result.Add(input_Timeout, "时间限制必须大于或等于0");
                }
                else
                    result.Add(input_Timeout, "时间限制必须为整数");
            }
            return result;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count = int.Parse(value_Count);
            int currentCount = 0;

            DateTime endDate;

            if (value_Timeout != string.Empty)
            {
                double time = double.Parse(value_Timeout);

                if (time > 0)
                    endDate = beginDate.AddHours(time);
                else
                    endDate = DateTime.MaxValue;
            }
            else
                endDate = DateTime.MaxValue;

            if (value_Action == "1")
            {
                currentCount = AlbumBO.Instance.GetUploadPhotoCount(userID, beginDate, endDate);
            }

            if (currentCount >= count)
            {
                isFail = false;
                return 1;
            }

            isFail = endDate <= DateTimeUtil.Now;

            return (double)currentCount / (double)count;
        }
    }

    public class AvatarMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.AvatarMission"; }
        }

        public override string Name
        {
            get { return "头像类"; }
        }


        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                return null;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                System.Collections.Generic.List<string> procedures = new System.Collections.Generic.List<string>();
                procedures.Add("<a href=\"" + BbsRouter.GetUrl("my/avatar") + "\" target=\"_blank\">新窗口打开头像设置页面</a>；");
                procedures.Add("在新打开的头像设置页面上传您的新头像就可以了；");
                return procedures;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Avatar.aspx"; }
        }

        public override string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        public override Hashtable CheckValues(StringTable values)
        {
            return null;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;
            User user = UserBO.Instance.GetUser(userID);

            if (user.IsDefaultAvatar)
                return 0;
            else
                return 1;

        }


    }

    public class BlogMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.BlogMission"; }
        }

        public override string Name
        {
            get { return "日志类"; }
        }


        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string input_Action = prefix + "action";
        private const string input_ArticleID = prefix + "articleID";
        private const string input_UserID = prefix + "userID";
        private const string input_Count = prefix + "count";
        private const string input_Timeout = prefix + "timeout";

        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();

                inputNames.Add(input_Action);
                inputNames.Add(input_ArticleID);
                inputNames.Add(input_UserID);
                inputNames.Add(input_Count);
                inputNames.Add(input_Timeout);

                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Blog.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            StringBuffer description = new StringBuffer();

            string value_Timeout = values[input_Timeout];
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];

            if (string.IsNullOrEmpty(value_Timeout) == false && value_Timeout != "0")
                description += "从申请任务开始计时，" + value_Timeout + " 小时内，";

            if (value_Action == "1")
            {
                description += "发表文章";
            }
            else if (value_Action == "2")
            {
                string value_ArticleID = values[input_ArticleID];

                if (string.IsNullOrEmpty(value_ArticleID) == false)
                {
                    BlogArticle article = BlogBO.Instance.GetBlogArticle(int.Parse(value_ArticleID));


                    description += "评论博客文章：<a href=\"" + UrlHelper.GetBlogArticleUrl(article.ArticleID) + "\">" + article.Subject + "</a>，";
                }
                else
                {
                    string value_UserID = values[input_UserID];

                    User user = UserBO.Instance.GetUser(int.Parse(value_UserID));

                    description += "评论用户：" + user.PopupNameLink + " 的博客文章，";
                }
            }

            description += "达到" + value_Count + "次。";

            return description.ToString();
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            string value_ArticleID = values[input_ArticleID];
            string value_UserID = values[input_UserID];
            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            if (value_ArticleID != string.Empty)
            {
                int articleID;

                if (int.TryParse(value_ArticleID, out articleID))
                {
                    if (BlogBO.Instance.ExistsBlogArticle(articleID) == false)
                    {
                        result.Add(input_ArticleID, "不存在ID为“" + articleID + "”的日志");
                    }
                }
                else
                    result.Add(input_ArticleID, "正确的日志ID应该是大于零的整数");
            }

            if (value_UserID != string.Empty)
            {
                int userID;

                if (int.TryParse(value_UserID, out userID))
                {
                    if (UserBO.Instance.GetUser(userID) == null)
                    {
                        result.Add(input_UserID, "不存在ID为“" + userID + "”的用户");
                    }
                }
                else
                    result.Add(input_UserID, "正确的用户ID应该是大于零的整数");
            }

            int count;

            if (int.TryParse(value_Count, out count))
            {
                if (count < 1)
                    result.Add(input_Count, "数量必须大于0");
            }
            else
                result.Add(input_Count, "数量必须为整数");

            int timeout;

            if (value_Timeout != string.Empty)
            {
                if (int.TryParse(value_Timeout, out timeout))
                {
                    if (count < 0)
                        result.Add(input_Timeout, "时间限制必须大于或等于0");
                }
                else
                    result.Add(input_Timeout, "时间限制必须为整数");
            }
            return result;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count = int.Parse(value_Count);
            int currentCount = 0;

            DateTime endDate;

            if (value_Timeout != string.Empty)
            {
                double time = double.Parse(value_Timeout);

                if (time > 0)
                    endDate = beginDate.AddHours(time);
                else
                    endDate = DateTime.MaxValue;
            }
            else
                endDate = DateTime.MaxValue;

            if (value_Action == "1")
            {
                currentCount = BlogBO.Instance.GetPostBlogArticleCount(userID, beginDate, endDate);
            }
            else if (value_Action == "2")
            {
                string value_ArticleID = values[input_ArticleID];

                if (value_ArticleID != string.Empty)
                {
                    int articleID = int.Parse(value_ArticleID);

                    currentCount = BlogBO.Instance.GetCommentCountForArticle(userID, articleID, beginDate, endDate);
                }
                else
                {
                    string value_UserID = values[input_UserID];

                    int targetUserID = int.Parse(value_UserID);

                    currentCount = BlogBO.Instance.GetCommentCountForUser(userID, targetUserID, beginDate, endDate);
                }
            }

            if (currentCount >= count)
            {
                isFail = false;
                return 1;
            }

            isFail = endDate <= DateTimeUtil.Now;

            return (double)currentCount / (double)count;
        }
    }

    public class DoingMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.DoingMission"; }
        }

        public override string Name
        {
            get { return "记录类"; }
        }


        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string input_Action = prefix + "action";
        private const string input_Count = prefix + "count";
        private const string input_Timeout = prefix + "timeout";

        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();

                inputNames.Add(input_Action);
                inputNames.Add(input_Count);
                inputNames.Add(input_Timeout);

                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Doing.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            StringBuffer description = new StringBuffer();

            string value_Timeout = values[input_Timeout];
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];

            if (string.IsNullOrEmpty(value_Timeout) == false && value_Timeout != "0")
                description += "从申请任务开始计时，" + value_Timeout + " 小时内，";

            if (value_Action == "1")
            {
                description += "记录心情";
            }

            description += "达到" + value_Count + "次。";

            return description.ToString();
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count;

            if (int.TryParse(value_Count, out count))
            {
                if (count < 1)
                    result.Add(input_Count, "数量必须大于0");
            }
            else
                result.Add(input_Count, "数量必须为整数");

            int timeout;

            if (string.IsNullOrEmpty(value_Timeout) == false)
            {
                if (int.TryParse(value_Timeout, out timeout))
                {
                    if (count < 0)
                        result.Add(input_Timeout, "时间限制必须大于或等于0");
                }
                else
                    result.Add(input_Timeout, "时间限制必须为整数");
            }
            return result;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count = int.Parse(value_Count);
            int currentCount = 0;

            DateTime endDate;

            if (value_Timeout != string.Empty)
            {
                double time = double.Parse(value_Timeout);

                if (time > 0)
                    endDate = beginDate.AddHours(time);
                else
                    endDate = DateTime.MaxValue;
            }
            else
                endDate = DateTime.MaxValue;

            if (value_Action == "1")
            {
                currentCount = DoingBO.Instance.GetPostDoingCount(userID, beginDate, endDate);
            }

            if (currentCount >= count)
            {
                isFail = false;
                return 1;
            }

            isFail = endDate <= DateTimeUtil.Now;

            return (double)currentCount / (double)count;
        }
    }

    public class FriendMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.FriendMission"; }
        }

        public override string Name
        {
            get { return "好友类"; }
        }

        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string countName = prefix + "count";

        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();
                inputNames.Add(countName);
                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                System.Collections.Generic.List<string> stepDescriptions = new System.Collections.Generic.List<string>();
                stepDescriptions.Add("在新窗口中打开<a href=\"" + BbsRouter.GetUrl("members") + "\" target=\"_blank\">会员页面</a>；");
                stepDescriptions.Add("在新打开的页面中，将某些用户加为好友，也可以自己设置条件寻找并添加为好友；");
                stepDescriptions.Add("接下来，您还需要等待对方批准您的好友申请。");
                return stepDescriptions;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Friend.aspx"; }
        }

        public override string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;

            int friendCount = MaxLabs.bbsMax.Entities.User.Current.TotalFriends;
            int count;
            int.TryParse(values[countName], out count);

            if (friendCount >= count)
                return 1;
            else
                return (double)friendCount / (double)count;
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            if (values[countName] != string.Empty)
            {
                int count;
                if (int.TryParse(values[countName], out count))
                {
                    if (count <= 0)
                        result.Add(countName, "数量必须是大于0的整数");
                }
                else
                    result.Add(countName, "数量必须是大于0的整数");
            }
            else
            {
                result.Add(countName, "数量必须是大于0的整数");
            }
            return result;
        }
    }

    public class InviteMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Entities.Missions.InviteMission"; }
        }

        public override string Name
        {
            get { return "邀请类"; }
        }


        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string countName = prefix + "count";
        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();
                inputNames.Add(countName);
                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                System.Collections.Generic.List<string> procedures = new System.Collections.Generic.List<string>();
                procedures.Add("在新窗口中打开<a href=\"" + BbsRouter.GetUrl("my/friends-invite") + "\">邀请页面</a>；");
                procedures.Add("通过QQ、MSN等IM工具，或者发送邮件，把邀请链接告诉你的好友，邀请他们加入进来；");
                procedures.Add("您需要邀请指定好友个数才算完成。");
                return procedures;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Invite.aspx"; }
        }

        public override string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            if (values[countName] != string.Empty)
            {
                int count;
                if (int.TryParse(values[countName], out count))
                {
                    if (count <= 0)
                        result.Add(countName, "数量必须是大于0的整数");
                }
                else
                    result.Add(countName, "数量必须是大于0的整数");
            }
            else
            {
                result.Add(countName, "数量必须是大于0的整数");
            }
            return result;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;

            int count;
            int.TryParse(values[countName], out count);

            int inviteCount = UserBO.Instance.GetUser(userID).TotalInvite;
            if (inviteCount >= count)
                return 1;
            else
                return (double)inviteCount / (double)count;
        }

    }

    public class LoginMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.LoginMission"; }
        }

        public override string Name
        {
            get { return "登陆类"; }
        }


        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                return null;
            }
        }



        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Login.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        public override Hashtable CheckValues(StringTable values)
        {
            return null;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;
            return 1;
        }


    }

    public class ProfileMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.ProfileMission"; }
        }

        public override string Name
        {
            get { return "用户资料类"; }
        }


        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                return null;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Profile.aspx"; }
        }

        public override string GetFinishConditionDescription(StringTable values)
        {
            return null;
        }

        public override Hashtable CheckValues(StringTable values)
        {
            return null;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;

            //

            return 1;
        }


    }

    public class ShareMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.ShareMission"; }
        }

        public override string Name
        {
            get { return "分享类"; }
        }


        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string input_Action = prefix + "action";
        private const string input_Count = prefix + "count";
        private const string input_Timeout = prefix + "timeout";

        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();

                inputNames.Add(input_Action);
                inputNames.Add(input_Count);
                inputNames.Add(input_Timeout);

                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Share.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            StringBuffer description = new StringBuffer();

            string value_Timeout = values[input_Timeout];
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];

            if (string.IsNullOrEmpty(value_Timeout) == false && value_Timeout != "0")
                description += "从申请任务开始计时，" + value_Timeout + " 小时内，";

            if (value_Action == "1")
            {
                description += "发表分享";
            }

            description += "达到" + value_Count + "条。";

            return description.ToString();
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count;

            if (int.TryParse(value_Count, out count))
            {
                if (count < 1)
                    result.Add(input_Count, "数量必须大于0");
            }
            else
                result.Add(input_Count, "数量必须为整数");

            int timeout;
            if (string.IsNullOrEmpty(value_Timeout) == false)
            {
                if (int.TryParse(value_Timeout, out timeout))
                {
                    if (count < 0)
                        result.Add(input_Timeout, "时间限制必须大于或等于0");
                }
                else
                    result.Add(input_Timeout, "时间限制必须为整数");
            }
            return result;
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            string value_Action = values[input_Action];
            string value_Count = values[input_Count];
            string value_Timeout = values[input_Timeout];

            int count = int.Parse(value_Count);
            int currentCount = 0;

            DateTime endDate;

            if (value_Timeout != string.Empty)
            {
                double time = double.Parse(value_Timeout);

                if (time > 0)
                    endDate = beginDate.AddHours(time);
                else
                    endDate = DateTime.MaxValue;
            }
            else
                endDate = DateTime.MaxValue;

            if (value_Action == "1")
            {
                currentCount = ShareBO.Instance.GetPostShareCount(userID, beginDate, endDate);
            }

            if (currentCount >= count)
            {
                isFail = false;
                return 1;
            }

            isFail = endDate <= DateTimeUtil.Now;

            return (double)currentCount / (double)count;
        }
    }

    public class TopicMission : MissionBase
    {
        public override string Type
        {
            get { return "MaxLabs.bbsMax.Missions.TopicMission"; }
        }

        public override string Name
        {
            get { return "帖子类"; }
        }

        private const string prefix = Consts.Mission_FinishConditionPrefix;

        private const string actionName = prefix + "action";
        private const string replyTopicName = prefix + "replytopic";
        private const string replyUserName = prefix + "replyuser";
        private const string topicCountName = prefix + "topiccount";
        private const string timeOutName = prefix + "timeout";
        private const string forumIDsName = prefix + "forumIDs";

        public override System.Collections.Generic.List<string> InputNames
        {
            get
            {
                System.Collections.Generic.List<string> inputNames = new System.Collections.Generic.List<string>();
                inputNames.Add(actionName);
                inputNames.Add(replyTopicName);
                inputNames.Add(replyUserName);
                inputNames.Add(topicCountName);
                inputNames.Add(timeOutName);
                inputNames.Add(forumIDsName);

                return inputNames;
            }
        }

        public override System.Collections.Generic.List<string> StepDescriptions
        {
            get
            {
                return null;
            }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/Topic.aspx"; }
        }


        public override string GetFinishConditionDescription(StringTable values)
        {
            StringBuilder description = new StringBuilder();
            if (!string.IsNullOrEmpty(values[timeOutName]) && values[timeOutName] != "0")
                description.AppendFormat("从申请任务开始计时, {0} 小时内,", values[timeOutName]);
            if (!string.IsNullOrEmpty(values[forumIDsName]))
            {
                int[] forumIDs = StringUtil.Split<int>(values[forumIDsName]);

                StringBuilder forumNames = new StringBuilder();
                foreach (int forumID in forumIDs)
                {
                    Forum forum = ForumBO.Instance.GetForum(forumID);
                    if (forum != null)
                    {
                        forumNames.Append(forum.ForumName).Append("，");
                    }
                }
                if (forumNames.Length > 0)
                {
                    description.AppendFormat("在以下版块“{0}”,", forumNames.ToString(0, forumNames.Length - ("，").Length));
                }
            }

            if (values[replyTopicName] != string.Empty)
            {
                BasicThread thread = PostBOV5.Instance.GetThread(int.Parse(values[replyTopicName]));
                string title;
                if (thread == null)
                {
                    title = "指定主题不存在或已被删除";
                }
                else
                {
                    Forum forum = ForumBO.Instance.GetForum(thread.ForumID);
                    title = "<a href=\"" + MaxLabs.bbsMax.Common.BbsUrlHelper.GetThreadUrl(forum.CodeName, thread.ThreadID, thread.ThreadTypeString, 1) + "\">" + thread.SubjectText + "</a>";
                }
                description.AppendFormat("回复主题:{0} {1}次", title, values[topicCountName]);
            }
            else if (values[replyUserName] != string.Empty)
            {
                string nickName;
                User user = UserBO.Instance.GetUser(int.Parse(values[replyUserName]));
                if (user != null)
                    nickName = user.Name;
                else
                    nickName = "用户不存在";
                description.AppendFormat("回复 {0} 发表的主题 {1}次", nickName, values[topicCountName]);
            }
            else
            {
                if (values[actionName] == "0")//发主题
                {
                    description.AppendFormat("发表主题 {0} 次", values[topicCountName]);
                }
                else if (values[actionName] == "1")//发回复
                    description.AppendFormat("发表回复 {0} 次", values[topicCountName]);
                else
                    description.AppendFormat("发表主题或者回复 {0} 次", values[topicCountName]);
            }
            return description.ToString();
        }

        public override Hashtable CheckValues(StringTable values)
        {
            Hashtable result = new Hashtable();

            if (values[replyTopicName] != string.Empty)
            {
                int topicID;
                if (int.TryParse(values[replyTopicName], out topicID))
                {
                    if (PostBOV5.Instance.GetThread(topicID) == null)
                    {
                        result.Add(replyTopicName, "不存在ID为“" + topicID + "”的主题");
                    }
                }
                else
                    result.Add(replyTopicName, "主题ID必须为整数");
            }

            if (values[replyUserName] != string.Empty)
            {
                int userID;
                if (int.TryParse(values[replyUserName], out userID))
                {
                    if (UserBO.Instance.GetUser(userID) == null)
                    {
                        result.Add(replyTopicName, "不存在ID为“" + userID + "”的用户");
                    }
                }
                else
                    result.Add(replyUserName, "用户ID必须为整数");
            }

            int count;
            if (int.TryParse(values[topicCountName], out count))
            {
                if (count < 1)
                    result.Add(topicCountName, "发帖数量必须大于0");
            }
            else
                result.Add(topicCountName, "发帖数量必须为整数");

            if (values[timeOutName] != string.Empty)
            {
                if (int.TryParse(values[timeOutName], out count))
                {
                }
                else
                    result.Add(timeOutName, "时间限制必须为整数");
            }
            return result;
        }


        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;

            System.Collections.Generic.List<int> forumIDs = new System.Collections.Generic.List<int>();
            if (!string.IsNullOrEmpty(values[forumIDsName]))
            {
                int[] tempforumIDs = StringUtil.Split<int>(values[forumIDsName]);

                StringBuilder forumNames = new StringBuilder();
                foreach (int forumID in tempforumIDs)
                {
                    Forum forum = ForumBO.Instance.GetForum(forumID);
                    if (forum != null)
                    {
                        forumIDs.Add(forumID);
                    }
                }
            }

            DateTime endDate = DateTime.MaxValue;

            if (false == string.IsNullOrEmpty(values[timeOutName]))
            {
                int hour = int.Parse(values[timeOutName]);
                if (hour > 0)
                {
                    endDate = beginDate.AddHours(hour);
                }
                else if (cycleTimes == 0)
                {
                    beginDate = DateTime.MinValue;
                }
            }
            else if (cycleTimes == 0)
            {
                beginDate = DateTime.MinValue;
            }
            else if (cycleTimes > 0)
            {
                endDate = beginDate.AddSeconds(cycleTimes);
            }

            int postCount = int.Parse(values[topicCountName]);

            if (values[actionName] == "0")//发主题
            {
                int total;

                ThreadCollectionV5 threads = PostBOV5.Instance.GetMyThreads(userID, true, 1, int.MaxValue, out total);

                int finishCount = 0;
                foreach (BasicThread thread in threads)
                {
                    if (thread.ThreadStatus != ThreadStatus.Recycled && thread.ThreadStatus != ThreadStatus.UnApproved)
                    {
                        if (forumIDs.Count == 0 || forumIDs.Contains(thread.ForumID))
                        {
                            if (thread.CreateDate >= beginDate && thread.CreateDate <= endDate)
                                finishCount++;
                        }
                    }

                    if (finishCount >= postCount)
                    {
                        return 1;
                    }
                }

                if (endDate <= DateTimeUtil.Now)
                    isFail = true;

                return (double)finishCount / (double)postCount;

            }
            else if (values[actionName] == "1")//发回复
            {

                PostCollectionV5 posts = PostBOV5.Instance.GetUserPosts(userID, beginDate, endDate);

                int targetUserID = 0;
                int.TryParse(values[replyUserName], out targetUserID);

                ThreadCollectionV5 threads = null;

                if (targetUserID > 0)
                {
                    System.Collections.Generic.List<int> threadIDs = new System.Collections.Generic.List<int>();
                    foreach (PostV5 post in posts)
                    {
                        if (post.IsApproved && !threadIDs.Contains(post.ThreadID))
                        {
                            threadIDs.Add(post.ThreadID);
                        }
                    }
                    if (threadIDs.Count > 0)
                        threads = PostBOV5.Instance.GetThreads(threadIDs);
                }

                int targetThreadID = 0;
                int.TryParse(values[replyTopicName], out targetThreadID);

                int finishCount = 0;

                foreach (PostV5 post in posts)
                {
                    if (post.IsApproved)
                    {
                        if (forumIDs.Count == 0 || forumIDs.Contains(post.ForumID))
                        {
                            if (targetThreadID == 0 || post.ThreadID == targetThreadID)//回复指定主题
                            {
                                if (targetUserID == 0)
                                {
                                    finishCount++;
                                }
                                else
                                {
                                    foreach (BasicThread thread in threads)
                                    {
                                        if (thread.ThreadID == post.ThreadID && thread.PostUserID == targetUserID)//回复指定用户
                                        {
                                            finishCount++;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (finishCount >= postCount)
                        return 1;
                }


                if (endDate <= DateTimeUtil.Now)
                    isFail = true;

                return (double)finishCount / (double)postCount;

            }
            else //发主题或者回复
            {
                int total;

                ThreadCollectionV5 threads = PostBOV5.Instance.GetMyThreads(userID, true, 1, int.MaxValue, out total);

                int finishCount = 0;
                foreach (BasicThread thread in threads)
                {
                    if (thread.ThreadStatus != ThreadStatus.Recycled && thread.ThreadStatus != ThreadStatus.UnApproved)
                    {
                        if (forumIDs.Count == 0 || forumIDs.Contains(thread.ForumID))
                        {
                            if (thread.CreateDate >= beginDate && thread.CreateDate <= endDate)
                                finishCount++;
                        }
                    }

                    if (finishCount >= postCount)
                        return 1;
                }

                PostCollectionV5 posts = PostBOV5.Instance.GetUserPosts(userID, beginDate, endDate);

                foreach (PostV5 post in posts)
                {
                    if (post.IsApproved)
                    {
                        if (forumIDs.Count == 0 || forumIDs.Contains(post.ForumID))
                        {
                            finishCount++;
                        }
                    }

                    if (finishCount >= postCount)
                        return 1;
                }


                if (endDate <= DateTimeUtil.Now)
                    isFail = true;

                return (double)finishCount / (double)postCount;

            }
        }

    }

    public class MissionGroup : MissionBase
    {
        public override string Type
        {
            get { return "group"; }
        }

        public override string Name
        {
            get { return "顺序任务组"; }
        }

        public override string HtmlPath
        {
            get { return "DefaultMissions/MissionGroup.aspx"; }
        }

        public override Hashtable CheckValues(StringTable values)
        {
            return new Hashtable();
        }

        public override double GetFinishPercent(Mission mission, int userID, int cycleTimes, DateTime beginDate, StringTable values, out bool isFail)
        {
            isFail = false;

            double sum = 0;

            foreach (Mission child in mission.ChildMissions)
            {
                sum += child.MissionBase.GetFinishPercent(child, userID, cycleTimes, beginDate, child.FinishCondition, out isFail);
            }

            sum = sum / mission.ChildMissions.Count;

            return sum;
        }
    }

}