//
// 请注意：bbsmax 不是一个免费产品，源代码仅限用于学习，禁止用于商业站点或者其他商业用途
// 如果您要将bbsmax用于商业用途，需要从官方购买商业授权，得到授权后可以基于源代码二次开发
//
// 版权所有 厦门麦斯网络科技有限公司
// 公司网站 www.bbsmax.com
//

using System;
using System.Collections.Generic;
using System.Web;
using MaxLabs.WebEngine.Plugin;
using MaxLabs.bbsMax.DataAccess;
using MaxLabs.bbsMax.Entities;
using MaxLabs.WebEngine;
using MaxLabs.bbsMax.Errors;
using MaxLabs.bbsMax.Enums;
using MaxLabs.bbsMax.Settings;

namespace MaxLabs.bbsMax.Web.plugins
{
    public class DefaultPropTypes : PluginBase
    {
        public override void Initialize()
        {
            PropBO.RegistPropType(new SetThreadUpProp());
            PropBO.RegistPropType(new ThreadStickProp());
            PropBO.RegistPropType(new ThreadHighlightProp());
            PropBO.RegistPropType(new ShowOnlineProp());
            PropBO.RegistPropType(new OnlineProtectionProp());
            PropBO.RegistPropType(new PigAvatarProp());
            PropBO.RegistPropType(new PigAvatarProtectionProp());
            PropBO.RegistPropType(new PigAvatarClearProp());
            PropBO.RegistPropType(new ChangeNameProp());
            PropBO.RegistPropType(new UserGroupProp());
            PropBO.RegistPropType(new ThreadLockProp());
            PropBO.RegistPropType(new ThreadUnLockProp());
            PropBO.RegistPropType(new SignatureProp());

        }
    }

    public class SignatureProp:PropType
    {
        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return string.Empty;
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            string timeLimit = "30";
            bool allowContinuous = false;

            if (string.IsNullOrEmpty(param) == false)
            {
                int flagIndex = param.IndexOf("|");
                allowContinuous = StringUtil.TryParse<bool>(param.Substring(0,flagIndex));
                timeLimit = param.Substring(flagIndex + 1);
            }

            return string.Format("<p>有效时间 <input type=\"text\" class=\"text number\" name=\"TimeLimit\" value=\"{0}\" /> 天</p><p><input id=\"allowContinuous\" type=\"checkbox\" name=\"allowContinuous\" value=\"true\" "+ (allowContinuous?"checked=\"checked\"":"") +" /><label for=\"allowContinuous\">允许多个连续使用以延长有效时间</label></p>", timeLimit);
        }

        public override string GetPropParam(HttpRequest request)
        {
            return StringUtil.TryParse<bool>(request.Form["allowContinuous"], false) + "|" + request.Form["TimeLimit"];
        }

        public override PropResult Apply(HttpRequest request, string param)
        {
            int flagIndex = param.IndexOf("|");
            bool allowContinuous =flagIndex > -1? StringUtil.TryParse<bool>(param.Substring(0, flagIndex)):false;

            int timeLimit = StringUtil.TryParse<int>(param.Substring(flagIndex+1));

           
            if (!allowContinuous)
            {
                if (User.Current.SignaturePropFlag.ExpiresDate > DateTimeUtil.Now)
                {
                    return new PropResult(PropResultType.Error, "您之前用过的签名卡还未过期");
                }
            }

            DateTime expiresDate = DateTimeUtil.Now;
            if (User.Current.SignaturePropFlag.ExpiresDate > DateTimeUtil.Now)
            {
                expiresDate = User.Current.SignaturePropFlag.ExpiresDate.AddDays(timeLimit);
            }
            else
            {
                expiresDate = DateTimeUtil.Now.AddDays(timeLimit);
            }

            UserBO.Instance.PropUpdateUserSignature(User.Current, expiresDate);
            return new PropResult(PropResultType.Succeed);
        }

        public override string Name
        {
            get { return "论坛签名道具"; }
        }

        public override string Description
        {
            get {  return "使用该道具可以使您的论坛签名正常显示。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.UserSelf; }
        }

        public override bool IsAutoUse(string param)
        {
            string timeLimit = "30";
            bool allowContinuous = false;

            if (string.IsNullOrEmpty(param) == false)
            {
                int flagIndex = param.IndexOf("|");
                allowContinuous = StringUtil.TryParse<bool>(param.Substring(0, flagIndex));
                timeLimit = param.Substring(flagIndex + 1);
            }

            return allowContinuous || User.Current.SignaturePropFlag.Available == false;
        }
    }

    public class ChangeNameProp : PropType
    {
        public override string Name
        {
            get { return "用户名修改道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户修改自己用户名的功能，建议您使用比较高的售价，防止用户轻易修改用户名。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.UserSelf; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            return string.Empty;
        }

        public override string GetPropParam(HttpRequest request)
        {
            return string.Empty;
        }

        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return "新用户名：<input type=\"text\" name=\"newname\" />";
        }

        public override PropResult Apply(HttpRequest request, string param)
        {
            string newName = request.Form["newname"];

            using(ErrorScope es = new ErrorScope())
            {
                string oldName = User.Current.Name;

                UserBO.Instance.TryUpdateUsername(User.Current, User.Current.UserID, newName, true);

                if(es.HasError == false)
                { 
                    PropResult result = Succeed();

                    result.LogForUser = string.Format("您使用改名工具将自己原有的“{0}”用户名改为了“{1}”", oldName, newName);

                    return result;
                }
                else
                    return Error();
            }
        }
    }

    public class OnlineProtectionProp : PropType
    {
        public override string Name
        {
            get { return "隐身保护道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户提前防范别人对自己使用反隐身卡的功能，在别人对自己使用反隐身卡并且自己正好处于隐身状态时，两个道具的作用将互相抵消，对方将看不到自己的在线状态。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Auto; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            return string.Empty;
        }

        public override string GetPropParam(HttpRequest request)
        {
            return string.Empty;
        }

        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return string.Empty;
        }

        public override PropResult Apply(HttpRequest request, string param)
        {
            return Error("本道具为自动防御道具，只有在别人对你使用道具时才会自动使用，您不能手工使用。");
        }
    }

    public class ShowOnlineProp : UserPropBase
    {
        public override string Name
        {
            get { return "隐身查看道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户查看某个用户是否在线或隐身的功能。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.User; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            return string.Empty;
        }

        public override string GetPropParam(HttpRequest request)
        {
            return string.Empty;
        }

        public override PropResult Apply(HttpRequest request, int userID, string param)
        {
            if(userID == User.Current.UserID)
            {
                return Error("您不能对自己使用本道具");
            }

            User user = UserBO.Instance.GetUser(userID);

            if (user.IsOnline)
            {
                PropResult result = Succeed("用户“" + user.Username + "”当前处于在线状态。");

                result.TargetUserID = userID;
                result.LogForUser = string.Format("您对{0}使用了隐身探测道具。", UserBO.Instance.GetUser(userID).PopupNameLink);
                result.LogForTargetUser = string.Format("{0}对您使用了隐身探测道具，但被您的隐身保护道具阻挡了。", User.Current.PopupNameLink);
                result.NotifyForTargetUser = result.LogForTargetUser;

                return result;
            }
            else if(user.IsInvisible)
            {
                if(PropBO.Instance.TryUseProp(userID, typeof(OnlineProtectionProp).FullName) == false)
                {
                    PropResult result = Succeed("用户“" + user.Username + "”当前处于隐身状态。");

                    result.TargetUserID = userID;
                    result.LogForUser = string.Format("您对{0}使用了反隐身道具。", UserBO.Instance.GetUser(userID).PopupNameLink);
                    result.LogForTargetUser = string.Format("{0}对您使用了反隐身道具，您的隐身状态被发现了。", User.Current.PopupNameLink);
                    result.NotifyForTargetUser = result.LogForTargetUser;

                    return result;
                }
            }
            
            {
            PropResult result = Succeed("用户“" + user.Username + "”当前不在线。");
    
            result.TargetUserID = userID;
            result.LogForUser = string.Format("您对{0}使用了反隐身道具。", UserBO.Instance.GetUser(userID).PopupNameLink);
            result.LogForTargetUser = string.Format("{0}对您使用了反隐身道具，正好您当时不在线。", User.Current.PopupNameLink);
            result.NotifyForTargetUser = result.LogForTargetUser;

            return result;
            }
        }
    }
    
    public class PigAvatarProtectionProp : PropType
    {
        public override string Name
        {
            get { return "猪头卡防御道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户提前防范别人对自己使用猪头卡的功能，在别人对自己使用猪头卡时，两个道具的作用将互相抵消。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Auto; }
        }

        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return string.Empty;
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            return string.Empty;
        }

        public override string GetPropParam(HttpRequest request)
        {
            return string.Empty;
        }

        public override PropResult Apply(HttpRequest request, string param)
        {
            return Error("本道具为自动防御道具，只有在别人对你使用道具时才会自动使用，您不能手工使用。");
        }
    }

    public class PigAvatarClearProp : PropType
    {
        public override string Name
        {
            get { return "猪头清除道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户清除猪头卡作用并还原头像的功能，此道具可以对别人使用。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.User; }
        }

        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return string.Empty;
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            return string.Empty;
        }

        public override string GetPropParam(HttpRequest request)
        {
            return string.Empty;
        }

        public override PropResult Apply(HttpRequest request, string param)
        {
            
            int userID = StringUtil.TryParse<int>(request.Params["uid"], User.CurrentID);

            SimpleUser user = UserBO.Instance.GetSimpleUser(userID);

            if (user == null)
              return  Error("置顶的ID为"+userID+"的用户不存在");

            if (!user.AvatarPropFlag.Available)
            {
                if (user.UserID == User.CurrentID)
                    return Error( "您现在的头像不是猪头，等您变成猪头的时候再使用本道具");
                else
                    return Error(user.Username + "的头像不是猪头");
            }

            UserBO.Instance.RemoveAttachAvatar(user);

            
            return Succeed();
        }
    }

    public class PigAvatarProp : UserPropBase
    {
        public override string Name
        {
            get { return "猪头卡道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户把其他的用户头像设置成猪头一段时间的功能，建议通过设置不同的道具作用时间，提供不同价位的道具，让用户有更多选择。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.User; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            string timeLimit = "24";

            if(string.IsNullOrEmpty(param) == false)
                timeLimit = param;

            return string.Format("作用时间（小时） <input type=\"text\" class=\"text number\" name=\"TimeLimit\" value=\"{0}\" /> （这段时间内被用了道具的用户不能修改头像）", timeLimit);
        }

        public override string GetPropParam(HttpRequest request)
        {
            return request.Form["TimeLimit"];
        }

        public override PropResult Apply(HttpRequest request, int userID, string param)
        {
            int timeLimit = StringUtil.TryParse<int>(param);

            if(timeLimit <= 0)
            {
                return Error("道具设置有误，请联系管理员");
            }

            if (userID == User.Current.UserID)
            {
                return Error("您不能对自己使用此道具");
            }
            
            if(PropBO.Instance.TryUseProp(userID, typeof(PigAvatarProtectionProp).FullName))
            {
                PropResult result = Failed("道具的效果被对方的防御道具抵消了！");

                result.TargetUserID = userID;
                result.LogForUser = string.Format("您用道具把{0}的头像变成猪头，但是被对方的防御道具抵消了。", UserBO.Instance.GetUser(userID).PopupNameLink);
                result.LogForTargetUser = string.Format("{0}用道具把您的头像变成猪头，但是被您的防御道具抵消了。", User.Current.PopupNameLink);
                result.NotifyForTargetUser = result.LogForTargetUser;

                return result;
            }
            else
            {
                DateTime time = DateTime.Now.AddHours(timeLimit);

                if (UserBO.Instance.SetAttachAvatar(userID, this.AvatarSrc, time, false))
               // if(UserBO.Instance.UpdateAndLockAvatar(userID, this.AvatarSrc, time, string.Format("{0}对您使用了头像道具，您在{1}之前无法修改自己的头像", User.Current.PopupNameLink, time)))
                {
                    PropResult result = Succeed();

                    result.TargetUserID = userID;
                    result.LogForUser = string.Format("您用道具把{0}的头像变成猪头{1}小时。", UserBO.Instance.GetSimpleUser(userID).PopupNameLink, timeLimit);
                    result.LogForTargetUser = string.Format("{0}用道具把您的头像变成猪头{1}小时", User.Current.PopupNameLink, timeLimit);
                    result.NotifyForTargetUser = string.Format("{0}用道具把您的头像变成了猪头", User.Current.PopupNameLink);

                    return result;
                }
                else
                {
                    return Error("道具使用失败了，可能对方正处于猪头状态。");
                }
            }
        }

        public string AvatarSrc
        {
            get { return "~/max-plugins/DefaultPropTypes/pigavatar_{size}.gif"; }
        }
    }

    public class ThreadStickProp : ThreadPropBase
    {
        public override string Name
        {
            get { return "帖子置顶道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户将自己主题版块置顶或全局置顶一段时间的功能，建议您通过设置不同的置顶方式和置顶时间，提供不同价位的道具，让用户有更多选择。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Thread; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            int timeLimit = 24;
            ThreadStatus stickType = ThreadStatus.Sticky;
            string forumIDs = string.Empty;
            string unit = "h";

            if(string.IsNullOrEmpty(param) == false)
            {
                StringList paramList = StringList.Parse(param);

                timeLimit = int.Parse(paramList[0]);
                stickType = StringUtil.TryParse<ThreadStatus>(paramList[1]);
                forumIDs = paramList[2];
                unit = paramList[3];
            }

            return string.Format(@"
<p>道具作用时间：<input type=""text"" class=""text number"" name=""prop_TimeLimit"" value=""{0}""/><select name=""prop_timeunit""><option value=""d""" + (unit == "d" ? " selected=\"selected\"" : "") + ">天</option><option value=\"h\"" + (unit == "h" ? " selected=\"selected\"" : "") + ">小时</option><option value=\"m\"" + (unit == "m" ? " selected=\"selected\"" : "") + @">分钟</option></select></p>
<p>置顶类型：<label><input type=""radio"" name=""prop_StickType"" value=""1"" {1}/> 板块置顶</label>&nbsp;&nbsp;&nbsp;&nbsp;<label><input type=""radio"" name=""prop_StickType"" value=""2"" {2}/> 全站置顶</label></p>
<p>道具有效范围：</p>
{3}"
                , timeLimit
                , stickType == ThreadStatus.Sticky ? "checked = \"checked\"" : ""
                , stickType == ThreadStatus.GlobalSticky ? "checked = \"checked\"" : ""
                , GetFormTreeHtml("prop_formids", forumIDs));
        }

        public override string GetPropParam(HttpRequest request)
        {
            return new StringList(
                request.Form["prop_TimeLimit"]
                , request.Form["prop_StickType"] == "2" ? ThreadStatus.GlobalSticky.ToString() : ThreadStatus.Sticky.ToString()
                , request.Form["prop_formids"] == null ? "" : request.Form["prop_formids"]
                , request.Form["prop_timeunit"]
           ).ToString();
        }

        public override PropResult Apply(HttpRequest request, int threadID, string param)
        {
            StringList paramList = StringList.Parse(param);

            int timeLimit = int.Parse(paramList[0]);
            ThreadStatus stickType = StringUtil.TryParse<ThreadStatus>(paramList[1]);
            List<int> forumIDs = StringUtil.Split2<int>(paramList[2], ',');
            string unit = paramList[3];

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            if(thread == null)
            {
                return Error("指定主题不存在");
            }
            else if(thread.PostUserID != User.Current.UserID)
            {
                return Error("本道具只能用于您本人发表的主题");
            }
            else if(forumIDs.Contains(-1) == false && forumIDs.Contains(thread.ForumID) == false)
            {
                return Error("目标主题所属版块不在本道具有效范围中");
            }
            
            DateTime endDate = DateTimeUtil.Now;

            if(unit == "d")
            {
                endDate = endDate.AddDays(timeLimit);
            }
            else if(unit == "h")
            {
                endDate = endDate.AddHours(timeLimit);
            }
            else
            {
                endDate = endDate.AddMinutes(timeLimit);
            }

        
            PostBOV5.Instance.SetThreadsStickyStatus(User.Current, thread.ForumID, null, new int[] { threadID }, stickType, endDate, true, true, false, string.Empty);

            PropResult result = Succeed();

            if(stickType == ThreadStatus.GlobalSticky)
                result.LogForUser = string.Format("您用道具对主题《{0}》进行{1}小时的全局置顶", thread.Subject, timeLimit);
            else
                result.LogForUser = string.Format("您用道具对主题《{0}》进行{1}小时的版块置顶", thread.Subject, timeLimit);

            return result;
        }
    }

    public class ThreadHighlightProp : ThreadPropBase
    {
        public override string Name
        {
            get { return "帖子加亮道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户在一段时间内加亮自己帖子标题的功能，建议您通过设置不同的加亮测试和加亮时间，提供不同价位的道具，让用户有更多选择。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Thread; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            int timeLimit = 24;
            string color = "font-color:0xFF0000";
            string forumIDs = string.Empty;
            string unit = "h";

            string font_family = string.Empty;
            string font_size = "12px";
            string font_style = string.Empty;
            string font_color = string.Empty;
            string back_color = string.Empty;

            if(string.IsNullOrEmpty(param) == false)
            {
                StringList paramList = StringList.Parse(param);

                timeLimit = int.Parse(paramList[0]);
                color = paramList[1];
                forumIDs = paramList[2];
                unit = paramList[3];

                if(paramList.Count > 4)
                {
                    font_family = paramList[4];
                    font_size = paramList[5];
                    font_style = paramList[6];
                    font_color = paramList[7];
                    back_color = paramList[8];
                }
            }

            return string.Format(@"
<p>加亮时间：
<input type=""text"" class=""text number"" name=""prop_TimeLimit"" value=""{0}"" />
<select name=""prop_timeunit"">
<option value=""d""" + (unit == "d" ? " selected=\"selected\"" : "") + @">天</option>
<option value=""h""" + (unit == "h" ? " selected=\"selected\"" : "") + @">小时</option>
<option value=""m""" + (unit == "m" ? " selected=\"selected\"" : "") + @">分钟</option>
</select>
</p>
<p>标题字体：
<select name=""prop_font_family"">
<option value=""""" + (font_family == string.Empty ? " selected=\"selected\"" : "") + @">默认字体</option>
<option value=""Arial""" + (font_family == "Arial" ? " selected=\"selected\"" : "") + @">Arial</option>
<option value=""宋体""" + (font_family == "宋体" ? " selected=\"selected\"" : "") + @">宋体</option>
<option value=""隶书""" + (font_family == "隶书" ? " selected=\"selected\"" : "") + @">隶书</option>
<option value=""楷书""" + (font_family == "楷书" ? " selected=\"selected\"" : "") + @">楷书</option>
<option value=""黑体""" + (font_family == "黑体" ? " selected=\"selected\"" : "") + @">黑体</option>
<option value=""幼圆""" + (font_family == "幼圆" ? " selected=\"selected\"" : "") + @">幼圆</option>
<option value=""微软雅黑""" + (font_family == "微软雅黑" ? " selected=\"selected\"" : "") + @">微软雅黑</option>
</select>
<select name=""prop_font_size"">
<option value=""12px""" + (font_size == "12px" ? " selected=\"selected\"" : "") + @">12px</option>
<option value=""13px""" + (font_size == "13px" ? " selected=\"selected\"" : "") + @">13px</option>
<option value=""14px""" + (font_size == "14px" ? " selected=\"selected\"" : "") + @">14px</option>
<option value=""15px""" + (font_size == "15px" ? " selected=\"selected\"" : "") + @">15px</option>
<option value=""16px""" + (font_size == "16px" ? " selected=\"selected\"" : "") + @">16px</option>
<option value=""17px""" + (font_size == "17px" ? " selected=\"selected\"" : "") + @">17px</option>
<option value=""18px""" + (font_size == "18px" ? " selected=\"selected\"" : "") + @">18px</option>
<option value=""19px""" + (font_size == "19px" ? " selected=\"selected\"" : "") + @">19px</option>
<option value=""20px""" + (font_size == "20px" ? " selected=\"selected\"" : "") + @">20px</option>
<option value=""21px""" + (font_size == "21px" ? " selected=\"selected\"" : "") + @">21px</option>
<option value=""22px""" + (font_size == "22px" ? " selected=\"selected\"" : "") + @">22px</option>
<option value=""23px""" + (font_size == "23px" ? " selected=\"selected\"" : "") + @">23px</option>
<option value=""24px""" + (font_size == "24px" ? " selected=\"selected\"" : "") + @">24px</option>
<option value=""25px""" + (font_size == "25px" ? " selected=\"selected\"" : "") + @">25px</option>
<option value=""26px""" + (font_size == "26px" ? " selected=\"selected\"" : "") + @">26px</option>
<option value=""27px""" + (font_size == "27px" ? " selected=\"selected\"" : "") + @">27px</option>
<option value=""28px""" + (font_size == "28px" ? " selected=\"selected\"" : "") + @">28px</option>
<option value=""29px""" + (font_size == "29px" ? " selected=\"selected\"" : "") + @">39px</option>
<option value=""30px""" + (font_size == "30px" ? " selected=\"selected\"" : "") + @">30px</option>
<option value=""31px""" + (font_size == "31px" ? " selected=\"selected\"" : "") + @">31px</option>
<option value=""32px""" + (font_size == "32px" ? " selected=\"selected\"" : "") + @">32px</option>
<option value=""33px""" + (font_size == "33px" ? " selected=\"selected\"" : "") + @">33px</option>
<option value=""34px""" + (font_size == "34px" ? " selected=\"selected\"" : "") + @">34px</option>
<option value=""35px""" + (font_size == "35px" ? " selected=\"selected\"" : "") + @">35px</option>
<option value=""36px""" + (font_size == "36px" ? " selected=\"selected\"" : "") + @">36px</option>
</select>
</p>
<p>文字样式：
<label><input type=""checkbox"" name=""prop_font_style"" value=""bold""" + (font_style != null && font_style.Contains("bold") ? " checked=\"checked\"" : "") + @"/> 粗体</label> 
<label><input type=""checkbox"" name=""prop_font_style"" value=""italic""" + (font_style != null &&font_style.Contains("italic") ? " checked=\"checked\"" : "") + @"/> 斜体</label> 
<label><input type=""checkbox"" name=""prop_font_style"" value=""underline""" + (font_style != null &&font_style.Contains("underline") ? " checked=\"checked\"" : "") + @"/> 下划线</label> 
<label><input type=""checkbox"" name=""prop_font_style"" value=""linethrough""" + (font_style != null &&font_style.Contains("linethrough") ? " checked=\"checked\"" : "") + @"/> 删除线</label>
</p>
<p>文字颜色：
<label><input type=""radio"" name=""prop_font_color"" value=""""" + (font_color == string.Empty ? " checked=\"checked\"" : "") + @"/>默认</label> 
<label style=""color:red""><input type=""radio"" name=""prop_font_color"" value=""red""" + (font_color == "red" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:green""><input type=""radio"" name=""prop_font_color"" value=""green""" + (font_color == "green" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:blue""><input type=""radio"" name=""prop_font_color"" value=""blue""" + (font_color == "blue" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:yellow""><input type=""radio"" name=""prop_font_color"" value=""yellow""" + (font_color == "yellow" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:cyan""><input type=""radio"" name=""prop_font_color"" value=""cyan""" + (font_color == "cyan" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:purply""><input type=""radio"" name=""prop_font_color"" value=""purple""" + (font_color == "purple" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:gary""><input type=""radio"" name=""prop_font_color"" value=""gary""" + (font_color == "gary" ? " checked=\"checked\"" : "") + @"/>██</label>
</p>
<p>背景颜色：
<label><input type=""radio"" name=""prop_back_color"" value=""""" + (back_color == string.Empty ? " checked=\"checked\"" : "") + @"/>默认</label> 
<label style=""color:red""><input type=""radio"" name=""prop_back_color"" value=""red""" + (back_color == "red" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:green""><input type=""radio"" name=""prop_back_color"" value=""green""" + (back_color == "green" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:blue""><input type=""radio"" name=""prop_back_color"" value=""blue""" + (back_color == "blue" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:yellow""><input type=""radio"" name=""prop_back_color"" value=""yellow""" + (back_color == "yellow" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:cyan""><input type=""radio"" name=""prop_back_color"" value=""cyan""" + (back_color == "cyan" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:purple""><input type=""radio"" name=""prop_back_color"" value=""purple""" + (back_color == "purple" ? " checked=\"checked\"" : "") + @"/>██</label> 
<label style=""color:gary""><input type=""radio"" name=""prop_back_color"" value=""gray""" + (back_color == "gray" ? " checked=\"checked\"" : "") + @"/>██</label>
</p>
<p>道具有效范围：</p>
{2}"
                , timeLimit
                , color
                , GetFormTreeHtml("prop_formids", forumIDs));
        }

        public override string GetPropParam(HttpRequest request)
        {
            return new StringList(
                request.Form["prop_TimeLimit"]
                , request.Form["prop_Color"]
                , request.Form["prop_formids"] == null ? "" : request.Form["prop_formids"]
                , request.Form["prop_timeunit"]
                , request.Form["prop_font_family"]
                , request.Form["prop_font_size"]
                , request.Form["prop_font_style"]
                , request.Form["prop_font_color"]
                , request.Form["prop_back_color"]
           ).ToString();
        }

        public override PropResult Apply(HttpRequest request, int threadID, string param)
        {
            StringList paramList = StringList.Parse(param);

            int timeLimit = int.Parse(paramList[0]);
            string color = paramList[1];
            List<int> forumIDs = StringUtil.Split2<int>(paramList[2], ',');
            string unit = paramList[3];

            string style = color;

            if(paramList.Count > 4)
            {
                string font_family = paramList[4];
                string font_size = paramList[5];
                string font_style = paramList[6];
                string font_color = paramList[7];
                string back_color = paramList[8];

                style = string.Empty;
                
                if(string.IsNullOrEmpty(font_family) == false)
                {
                    style += "font-family:" + font_family + ";";
                }

                if(string.IsNullOrEmpty(font_size) == false)
                {
                    style += "font-size:" + font_size + ";";
                }

                if(font_style != null)
                {
                    if(font_style.Contains("bold"))
                    {
                        style += "font-weight:bold;";
                    }
                    
                    if(font_style.Contains("italic"))
                    {
                        style += "font-style:italic;";
                    }
                    
                    if(font_style.Contains("underline"))
                    {
                        if(font_style.Contains("linethrough"))
                        {
                            style += "text-decoration: underline line-through;";
                        }
                        else
                        {
                            style += "text-decoration: underline;";
                        }
                    }
                    else if(font_style.Contains("linethrough"))
                    {
                        style += "text-decoration: line-through;";
                    }
                }

                if(string.IsNullOrEmpty(font_color) == false)
                {
                    style += "color:" + font_color + ";";
                }

                if(string.IsNullOrEmpty(back_color) == false)
                {
                    style += "background-color:" + back_color + ";";
                }
            }

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            if(thread == null)
            {
                return Error("指定主题不存在");
            }
            else if(thread.PostUserID != User.Current.UserID)
            {
                return Error("本道具只能用于您本人发表的主题");
            }
            else if(forumIDs.Contains(-1) == false && forumIDs.Contains(thread.ForumID) == false)
            {
                return Error("目标主题所属版块不在本道具有效范围中");
            }
            
            DateTime endDate = DateTimeUtil.Now;

            if(unit == "d")
            {
                endDate = endDate.AddDays(timeLimit);
            }
            else if(unit == "h")
            {
                endDate = endDate.AddHours(timeLimit);
            }
            else
            {
                endDate = endDate.AddMinutes(timeLimit);
            }

            PostBOV5.Instance.SetThreadsSubjectStyle(User.Current, thread.ForumID, new int[] { threadID }, style, endDate, true, true, false, string.Empty);

            PropResult result = Succeed();

            result.LogForUser = string.Format("您用道具对主题《{0}》进行{1}小时的标题加亮", thread.Subject, timeLimit);

            return result;
        }
    }

    public class SetThreadUpProp : ThreadPropBase
    {
        public override string Name
        {
            get { return "帖子提升道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户提升自己帖子的功能，被提升的帖子将提升至本版块主题列表的第一页。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Thread; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            string forumIDs = string.Empty;

            if(string.IsNullOrEmpty(param) == false)
            {
                forumIDs = param;
            }

            return GetFormTreeHtml("prop_formids", forumIDs);
        }

        public override string GetPropParam(HttpRequest request)
        {
            return request.Form["prop_formids"] == null ? "" : request.Form["prop_formids"];
        }

        public override PropResult Apply(HttpRequest request, int threadID, string param)
        {
            List<int> forumIDs = StringUtil.Split2<int>(param, ',');

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            if(thread == null)
            {
                return Error("指定主题不存在");
            }
            else if(thread.PostUserID != User.Current.UserID)
            {
                return Error("本道具只能用于您本人发表的主题");
            }
            else if(forumIDs.Contains(-1) == false && forumIDs.Contains(thread.ForumID) == false)
            {
                return Error("目标主题所属版块不在本道具有效范围中");
            }

            if (PostBOV5.Instance.SetThreadsUp(User.Current, thread.ForumID, new int[] { threadID }, true, true, false, string.Empty) == false)
            {
                return Error("发生了未知的错误，请联系管理员");
            }

            PropResult result = Succeed();

            result.LogForUser = string.Format("您用道具对主题《{0}》进行了提升", thread.SubjectText);

            return result;
        }
    }

    public class UserGroupProp : PropType
    {
        public override string Name
        {
            get { return "用户组道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户花钱加入或退出用户组的功能，通过设置此道具加入或退出的用户组以及加入的时间长度，并配合不同用户组的权限设置，您可以很灵活的建立针对自己网站的会员制。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.User; }
        }

        public override string GetPropApplyFormHtml(HttpRequest request)
        {
            return string.Empty;
        }

        public override bool IsAutoUse(string param)
        {
            if(string.IsNullOrEmpty(param) == false)
            {
                StringList paramList = StringList.Parse(param);

                return paramList[4] == "1";
            }

            return false;
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            string time = "1";
            string unit = "d";
            
            List<Guid> joinRoleIDs = new List<Guid>();

            List<Guid> exitRoleIDs = new List<Guid>();

            string autoUse = "0";

            string timeAddUp = "0";

            if(string.IsNullOrEmpty(param) == false)
            {
                StringList paramList = StringList.Parse(param);
                if (paramList.Count > 0)
                {
                    time = paramList[0];
                }
                if (paramList.Count > 1)
                {
                    unit = paramList[1];
                }
                if (paramList.Count > 2)
                {
                    joinRoleIDs = StringUtil.Split2<Guid>(paramList[2], ',');
                }
                if (paramList.Count > 3)
                {
                    exitRoleIDs = StringUtil.Split2<Guid>(paramList[3], ',');
                }
                if (paramList.Count > 4)
                {
                    autoUse = paramList[4];
                }
                if (paramList.Count > 5)
                {
                    timeAddUp = paramList[5];
                }
            }

            StringBuffer html = new StringBuffer();

            html += "<p><b>购买时自动使用</b></p>";

            html += "<p><lable><input type=\"radio\" name=\"prop_autouse\" value=\"1\" " + (autoUse == "1" ? "checked=\"checked\"" : "") + " />是</label>&nbsp;&nbsp;<label><input type=\"radio\" name=\"prop_autouse\" value=\"0\" " + (autoUse == "0" ? "checked=\"checked\"" : "") + "/>否</label>&nbsp;&nbsp; P.S:如果加入用户组的时间填0，则表示用户组身份永不过期。</p>";

            html += "<p><b>加入用户组的时间</b></p><p><input type=\"text\" class=\"text number\" name=\"prop_time\" value=\"" + time + "\" /><select name=\"prop_timeunit\"><option value=\"d\"" + (unit == "d" ? " selected=\"selected\"" : "") + ">天</option><option value=\"h\"" + (unit == "h" ? " selected=\"selected\"" : "") + ">小时</option><option value=\"m\"" + (unit == "m" ? " selected=\"selected\"" : "") + ">分钟</option></select></p>";

            html += "<p><b>时间可以叠加</b></p>";

            html += "<p><lable><input type=\"radio\" name=\"prop_timeAddUp\" value=\"1\" " + (timeAddUp == "1" ? "checked=\"checked\"" : "") + " />是</label>&nbsp;&nbsp;<label><input type=\"radio\" name=\"prop_timeAddUp\" value=\"0\" " + (timeAddUp == "0" ? "checked=\"checked\"" : "") + "/>否</label></p>";

            html += "<p><b>加入的用户组（可多选）</b></p>";

            if(AllSettings.Current.RoleSettings.GetRolesForAutoAdd().Count == 0)
            {
                html += "<p style=\"color:#FF0000;\">找不到自定义用户组，您需要到<a href=\"/max-admin/user/setting-roles-other.aspx\" target=\"_blank\">用户组管理页面</a>添加自定义用户组。</p>";
            }
            else
            {
                foreach(Role role in AllSettings.Current.RoleSettings.GetRolesForAutoAdd())
                {
                    html += "<p><label><input type=\"checkbox\" name=\"prop_join_roles\" value=\"" + role.RoleID + "\"" + (joinRoleIDs.Contains(role.RoleID) ? "checked=\"checked\"" : "") + " /> " + role.Name + "</label></p>";
                }
            }

            html += "<p><b>退出的用户组（可多选）</b></p>";
            
            if(AllSettings.Current.RoleSettings.GetRolesForAutoAdd().Count == 0)
            {
                html += "<p style=\"color:#FF0000;\">找不到自定义用户组，您需要到<a href=\"/max-admin/user/setting-roles-other.aspx\" target=\"_blank\">用户组管理页面</a>添加自定义用户组。</p>";
            }
            else
            {
                foreach(Role role in AllSettings.Current.RoleSettings.GetRolesForAutoAdd())
                {
                    html += "<p><label><input type=\"checkbox\" name=\"prop_exit_roles\" value=\"" + role.RoleID + "\"" + (exitRoleIDs.Contains(role.RoleID) ? "checked=\"checked\"" : "") + " /> " + role.Name + "</label></p>";
                }
            }

            return html.ToString();
        }

        public override string GetPropParam(HttpRequest request)
        {
            return new StringList(
                request.Form["prop_time"],
                request.Form["prop_timeunit"],
                request.Form["prop_join_roles"] == null ? "" : request.Form["prop_join_roles"],
                request.Form["prop_exit_roles"] == null ?  "" : request.Form["prop_exit_roles"],
                request.Form["prop_autouse"],
                request.Form["prop_timeAddUp"]).ToString();
               
        }

        public override PropResult Apply(HttpRequest request, string param)
        {
            StringList paramList = StringList.Parse(param);

            int time = 0;

            string unit = "d";

            Guid[] joinRoleIDs = new List<Guid>().ToArray();

            Guid[] exitRoleIDs = new List<Guid>().ToArray();

            string timeAddUp = "0";

            if(StringUtil.TryParse<int>(paramList[0], out time) == false)
            {
                return Error("道具设置有误，请联系管理员");
            }
            if (paramList.Count > 1)
            {
                unit = paramList[1];
            }
            if (paramList.Count > 5)
            {
                timeAddUp = paramList[5];
            }

            DateTime beginDate = DateTimeUtil.Now;
            DateTime endDate = UpdateEndDate(beginDate,time,unit);

            if (paramList.Count > 2)
            {
                 joinRoleIDs = StringUtil.Split<Guid>(paramList[2], ',');
            }

            if (paramList.Count > 3)
            {
                 exitRoleIDs = StringUtil.Split<Guid>(paramList[3], ',');
            }

            UserRoleCollection joinRoles = new UserRoleCollection();

            foreach(Guid roleID in joinRoleIDs)
            {
                UserRole role=null;
                //如果道具作用时间可以叠加
                if (timeAddUp == "1")
                {
                    role = RoleBO.Instance.GetUserRoleByBothIDs(User.Current.UserID, roleID);
                }
                if (role != null)
                {
                    if (role.EndDate < DateTimeUtil.Now)
                    {
                        role.EndDate = DateTimeUtil.Now;
                    }
                    role.EndDate = UpdateEndDate(role.EndDate, time, unit);
                }
                else
                {
                    role = new UserRole();
                    role.UserID = User.Current.UserID;
                    role.RoleID = roleID;
                    role.BeginDate = beginDate;
                    role.EndDate = endDate;
                }


                joinRoles.Add(role);
            }
            
            UserBO.Instance.AddUsersToRoles(joinRoles);

            UserBO.Instance.RemoveUsersFromRoles(new int[]{ User.Current.UserID }, exitRoleIDs);

            return Succeed();
        }

        private DateTime UpdateEndDate(DateTime dateTime, int timeValue, string timeType)
        {
            TimeSpan ts = DateTime.MaxValue - dateTime;
            TimeSpan ts2;

            if (timeType == "d")
            {
                ts2 = new TimeSpan(timeValue, 0, 0, 0);
            }
            else if (timeType == "h")
            {
                ts2 = new TimeSpan(timeValue, 0, 0);
            }
            else
            {
                ts2 = new TimeSpan(0, timeValue, 0);
            }


            if (timeValue == 0 || ts.TotalMinutes < ts2.TotalMinutes)
            {
                dateTime = DateTime.MaxValue;
            }
            else
            {
                dateTime = dateTime.Add(ts2);
            }

            return dateTime;
        }
    }

    public class ThreadLockProp : ThreadPropBase
    {
        public override string Name
        {
            get { return "帖子锁定道具"; }
        }

        public override string Description
        {
            get { return "本道具可以让用户锁定自己发表的帖子一段时间"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Thread; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            int timeLimit = 24;
            string forumIDs = string.Empty;
            string unit = "h";

            if(string.IsNullOrEmpty(param) == false)
            {
                StringList paramList = StringList.Parse(param);

                timeLimit = int.Parse(paramList[0]);
                forumIDs = paramList[1];
                unit = paramList[2];
            }

            return string.Format(@"
<p>道具作用时间：<input type=""text"" class=""text number"" name=""prop_TimeLimit"" value=""{0}""/><select name=""prop_timeunit""><option value=""d""" + (unit == "d" ? " selected=\"selected\"" : "") + ">天</option><option value=\"h\"" + (unit == "h" ? " selected=\"selected\"" : "") + ">小时</option><option value=\"m\"" + (unit == "m" ? " selected=\"selected\"" : "") + @">分钟</option></select></p>
<p>道具有效范围：</p>
{1}"
                , timeLimit
                , GetFormTreeHtml("prop_formids", forumIDs));
        }

        public override string GetPropParam(HttpRequest request)
        {
            return new StringList(
                request.Form["prop_TimeLimit"]
                , request.Form["prop_formids"] == null ? "" : request.Form["prop_formids"]
                , request.Form["prop_timeunit"]
           ).ToString();
        }

        public override PropResult Apply(HttpRequest request, int threadID, string param)
        {
            StringList paramList = StringList.Parse(param);

            int timeLimit = int.Parse(paramList[0]);
            List<int> forumIDs = StringUtil.Split2<int>(paramList[1], ',');
            string unit = paramList[2];

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            if(thread == null)
            {
                return Error("指定主题不存在");
            }
            else if(thread.PostUserID != User.Current.UserID)
            {
                return Error("本道具只能用于您本人发表的主题");
            }
            else if(forumIDs.Contains(-1) == false && forumIDs.Contains(thread.ForumID) == false)
            {
                return Error("目标主题所属版块不在本道具有效范围中");
            }
            
            DateTime endDate = DateTimeUtil.Now;

            if(unit == "d")
            {
                endDate = endDate.AddDays(timeLimit);
            }
            else if(unit == "h")
            {
                endDate = endDate.AddHours(timeLimit);
            }
            else
            {
                endDate = endDate.AddMinutes(timeLimit);
            }

            PostBOV5.Instance.SetThreadsLock(User.Current, thread.ForumID, new int[] { thread.ThreadID }, true, endDate, true, true, false, string.Empty);
            PropResult result = Succeed();

            result.LogForUser = string.Format("您用道具对主题《{0}》进行{1}小时的帖子锁定", thread.Subject, timeLimit);

            return result;
        }
    }

    public class ThreadUnLockProp : ThreadPropBase
    {
        public override string Name
        {
            get { return "帖子解锁道具"; }
        }

        public override string Description
        {
            get { return "此道具提供让用户解锁自己帖子的功能。"; }
        }

        public override PropTypeCategory Category
        {
            get { return PropTypeCategory.Thread; }
        }

        public override string GetPropParamFormHtml(HttpRequest request, string param)
        {
            string forumIDs = string.Empty;

            if(string.IsNullOrEmpty(param) == false)
            {
                forumIDs = param;
            }

            return GetFormTreeHtml("prop_formids", forumIDs);
        }

        public override string GetPropParam(HttpRequest request)
        {
            return request.Form["prop_formids"] == null ? "" : request.Form["prop_formids"];
        }

        public override PropResult Apply(HttpRequest request, int threadID, string param)
        {
            List<int> forumIDs = StringUtil.Split2<int>(param, ',');

            BasicThread thread = PostBOV5.Instance.GetThread(threadID);

            if(thread == null)
            {
                return Error("指定主题不存在");
            }
            else if(thread.PostUserID != User.Current.UserID)
            {
                return Error("本道具只能用于您本人发表的主题");
            }
            else if(forumIDs.Contains(-1) == false && forumIDs.Contains(thread.ForumID) == false)
            {
                return Error("目标主题所属版块不在本道具有效范围中");
            }

            if (PostBOV5.Instance.SetThreadsUp(User.Current, thread.ForumID, new int[] { threadID }, true, true, false, string.Empty) == false)
            {
                return Error("发生了未知的错误，请联系管理员");
            }


            PropResult result = Succeed();

            result.LogForUser = string.Format("您用道具对主题《{0}》进行了解锁", thread.Subject);

            return result;
        }
    }
}