<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlmeta.aspx"-->
</head>
<body>
<div class="container">
    <!--#include file="../_inc/_header.aspx"-->
    
    <div class="crumbnav">
        <!--[if $IsCreateThread == false]-->
        <a href="$url($forum.codename/$Thread.ThreadTypeString-$Thread.ThreadID-1-1)">&laquo; 返回$Thread.Subject</a>
        <!--[else]-->
        <a href="$url($forum.CodeName/list-1)">&laquo; 返回$forum.ForumName</a>
        <!--[/if]-->
    </div>

    <section class="main publish">
        <form id="formpost" method="post" enctype="multipart/form-data" action="$_form.action">
            <!--[if $IsCreateThread]-->
            <ul class="topictypes">
                <!--[if $CanCreateThread]-->
                <li <!--[if $action == "thread"]-->class="current"<!--[/if]-->><a class="type-normal" href="$url($codename/post)?action=thread" onclick="return ajaxRender(this.href, '*');">主题</a></li>
                <!--[/if]-->
                <!--[if $CanCreatePoll]-->
                <li <!--[if $action == "poll"]-->class="current"<!--[/if]-->><a class="type-poll" href="$url($codename/post)?action=poll" onclick="return ajaxRender(this.href, '*');">投票</a></li>
                <!--[/if]-->
                <!--[if $CanCreateQuestion]-->
                <li <!--[if $action == "question"]-->class="current"<!--[/if]-->><a class="type-question" href="$url($codename/post)?action=question" onclick="return ajaxRender(this.href, '*');">提问</a></li>
                <!--[/if]-->
                <!--[if $CanCreatePolemize]-->
                <li <!--[if $action == "polemize"]-->class="current"<!--[/if]-->><a class="type-polemize" href="$url($codename/post)?action=polemize" onclick="return ajaxRender(this.href, '*');">辩论</a></li>
                <!--[/if]-->
            </ul>
            <!--[/if]-->
            <input type="hidden" name="postaction" value="$action">
            <div class="form">
                <!--[unnamederror]-->
                <div class="errormsg">$Message</div>
                <!--[/unnamederror]-->
                <!--[if $IsShowNoUpdateSortOrder]-->
                <div class="alertmsg">由于该主题太长时间未被回复, 您的回复不会将该主题顶上去.</div>
                <!--[/if]-->
                <!--[if $IsLogin==false && $EnableGuestNickName]-->
                <div class="row">
                    <label class="label" for="guestNickName">昵称</label>
                    <div class="enter">
                        <input type="text" class="text" name="guestNickName" id="guestNickName" value="$_form.text('guestNickName')" autocorrect="off" autocapitalize="off">
                    </div>
                </div>
                <!--[/if]-->
                <div class="row">
                    <div class="enter">
                        <input class="text" type="text" name="subject" id="subject" value="$_form.text("subject","$Subject")" placeholder="标题">
                    </div>
                </div>
                <!--[if $IsReply && $Thread.ThreadType == ThreadType.Polemize]-->
                <div class="row">
                    <label class="label">选择您的立场</label>
                    <div class="enter">
                        <input name="viewPointType" id="viewPointType1" type="radio" value="2" $_form.checked("viewPointType","2")>
                        <label for="viewPointType1">支持正方</label>
                        <input name="viewPointType" id="viewPointType3" type="radio" value="4" $_form.checked("viewPointType","4")>
                        <label for="viewPointType3">支持反方</label>
                        <input name="viewPointType" id="viewPointType2" type="radio" value="3" checked="checked" $_form.checked("viewPointType","3")>
                        <label for="viewPointType2">支持中立</label>
                    </div>
                </div>
                <!--[/if]-->
                <!--[if $isShowPollOptions]-->
                <div class="row row-pollitem">
                    <label class="label" for="vote">投票项 <span class="note">一行一个选项, 最多可使用 $MaxPollItemCount 项</span></label>
                    <div class="enter">
                        <input type="hidden" name="pollItemCount" id="pollItemCount" value="2">
                        <textarea cols="40" rows="4" id="vote" name="vote" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->>$_form.text("vote",$PollItemString)</textarea>
                    </div>
                </div>
                <!--[else if $isShowPolemizeOptions]-->
                <div class="row row-debateitem">
                    <label class="label" for="agreeViewPoint">正方观点</label>
                    <div class="enter">
                        <textarea cols="40" rows="5" id="agreeViewPoint" name="agreeViewPoint" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->><!--[if $polemize == null]-->$_form.text("agreeViewPoint")<!--[else]-->$polemize.AgreeViewPoint<!--[/if]--></textarea>
                    </div>
                </div>
                <div class="row row-debateitem">
                    <label class="label" for="againstViewPoint">反方观点</label>
                    <div class="enter">
                        <textarea cols="40" rows="5" id="againstViewPoint" name="againstViewPoint" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->><!--[if $polemize == null]-->$_form.text("againstViewPoint")<!--[else]-->$polemize.AgainstViewPoint<!--[/if]--></textarea>
                    </div>
                </div>
                <!--[/if]-->
                <div class="row">
                    <div class="enter">
                        <textarea cols="50" rows="10" name="editor_content" id="editor_content" placeholder="内容">$_form.text("editor_content","$Content")</textarea>
                    </div>
                </div>
                <!--[if $IsShowSellThread]-->
                <div class="row">
                    <label class="label" for="price">出售帖子 <span class="note">$SellPostPointScope</span></label>
                    <div class="enter">
                        <input type="number" class="text number" name="price" id="price" <!--[if $IsEditThread]-->value="$_form.text('price','$thread.Price')"<!--[else]--> value="$_form.text('price')"<!--[/if]-->>
                        $SellPostPoint.UnitName $SellPostPoint.Name
                    </div>
                </div>
                <!--[else if $isShowPollOptions]-->
                <div class="row">
                    <label class="label" for="voteMultiple">最多可选数</label>
                    <div class="enter">
                        <input type="number" class="text number" id="voteMultiple" name="voteMultiple" value="$PollMultiple" <!--[if $IsEditThread]-->disabled="disabled"<!--[/if]-->>
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="expiresDays">有效时间 <span class="note">最多$time$timeUnit, 0或空为允许的最大值</span></label>
                    <div class="enter">
                    <!--[if $IsShowPollExpiresDate]-->
                        <!--[if $Poll.ExpiresDate == DateTime.MaxValue]-->
                        无限期
                        <!--[else]-->
                        $Poll.ExpiresDate
                        <!--[/if]-->
                    <!--[else]-->
                        <input type="number" class="text number" id="expiresDays" name="expiresDays" value="$time">
                        $timeUnit
                    <!--[/if]-->
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="cbNoEyeable">投票结果显示</label>
                    <div class="enter">
                        <!--[if $IsEditThread]-->
                        <input type="checkbox" disabled="disabled" name="cbNoEyeable" id="cbNoEyeable" <!--[if $poll.AlwaysEyeable==false]-->checked="checked"<!--[/if]-->>
                        <!--[else]-->
                        <input type="checkbox" name="cbNoEyeable" id="cbNoEyeable">
                        <!--[/if]-->
                        <label for="cbNoEyeable">投票后结果可见</label>
                    </div>
                </div>
                <!--[else if $isShowQuestionOptions]-->
                <div class="row">
                    <label class="label" for="askaward">悬赏设置 <span class="note">$QuestionRewardScope</span></label>
                    <div class="enter">
                        <input type="number" class="text number" name="reward" id="reward" <!--[if $IsEditThread]-->value="$question.Reward" disabled="disabled"<!--[else]-->value="$_form.text("reward")"<!--[/if]-->>
                        $QuestionRewardPoint.UnitName $QuestionRewardPoint.Name, 
                        您还有: $My.GetPoint($QuestionRewardPoint.Type)
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="expiresDays">有效时间 <span class="note">最多$time$timeUnit, 0或空为允许的最大值</span></label>
                    <div class="enter">
                    <!--[if $IsEditThread]-->
                        <!--[if $question.ExpiresDate == DateTime.MaxValue]-->
                        无限期
                        <!--[else]-->
                        $outputDateTime($question.ExpiresDate)
                        <!--[/if]-->
                    <!--[else]-->
                        <input type="number" class="text number" name="expiresDays" id="expiresDays" value="$time">
                        $timeUnit
                    <!--[/if]-->
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="rewardCount">奖励回复数</label>
                    <div class="enter">
                        <input type="number" class="text number" name="rewardCount" id="rewardCount" <!--[if $IsEditThread]--> value="$question.RewardCount" disabled="disabled" <!--[else]--> value="1" <!--[/if]-->>
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="notEyeable">是否允许他人查看回复</label>
                    <div class="enter">
                        <input name="notEyeable" id="notEyeable" type="checkbox" <!--[if $IsEditThread && $question.AlwaysEyeable == false]--> checked="checked" disabled="disabled" <!--[/if]-->>
                        <label for="notEyeable">回复可见</label>
                    </div>
                </div>
                <!--[else if $isShowPolemizeOptions]-->
                <div class="row">
                    <label class="label" for="expiresDays">有效时间 <span class="note">最多$time$timeUnit, 0或空为允许的最大值</span></label>
                    <div class="enter">
                    <!--[if $IsEditThread]-->
                        <!--[if $polemize.ExpiresDate == DateTime.MaxValue]-->
                        无限期
                        <!--[else]-->
                        $outputDateTime($polemize.ExpiresDate)
                        <!--[/if]-->
                    <!--[else]-->
                        <input type="number" class="text number" id="expiresDays" name="expiresDays" value="$time">
                        $timeUnit
                    <!--[/if]-->
                    </div>
                </div>
                <!--[/if]-->
                <!--[if $IsCreateThread || $IsEditThread]-->
                    <!--[ValidateCode actionType="CreateTopic"]-->
                <div class="row">
                    <label class="label" for="$inputName">验证码</label>
                    <div class="enter">
                        <input type="text" class="text validcode" name="$inputName" id="$inputName" placeholder="$tip">
                    </div>
                    <div class="captchaimg">
                        <img src="$imageurl" alt="" onclick="this.src=this.src+'&rnd='+Math.random();">
                        点击图片更换验证码
                    </div>
                </div>
                    <!--[/ValidateCode]-->
                <!--[else if $IsReply || $IsEditPost]-->
                    <!--[ValidateCode actionType="ReplyTopic"]-->
                <div class="formrow">
                    <h3 class="label"><label for="$inputName">验证码</label>
                    <img src="$imageurl" alt="" />
                     <span class="form-note">$tip</span></h3>
                    <div class="form-enter">
                        <input type="text" class="text validcode" name="$inputName" id="$inputName" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                    </div>
                </div>
                    <!--[/ValidateCode]-->
                <!--[/if]-->

                <div class="row row-button">
                    <input type="submit" class="button" name="postButton" value="发布">
                </div>
            </div>
        </form>
    </section>

    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>