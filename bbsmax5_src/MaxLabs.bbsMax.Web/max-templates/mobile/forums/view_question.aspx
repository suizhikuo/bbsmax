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
        <a href="$url($forum.CodeName/list-1)">&laquo; 返回$forum.ForumName</a>
    </div>

    <section class="main hfeed viewtopic">
        <div class="publishbutton">
            <a class="post" href="$url($codename/post)?action=thread">发帖</a>
            <a class="reply" href="$url($codename/post)?threadid=$threadID&action=reply">回复</a>
        </div>
        <form id="threadform" method="post" enctype="multipart/form-data" action="$_form.action">
        <input type="hidden" name="threadids" value="$Thread.ThreadID" />
        <!--[ajaxpanel id="ap_postlist" idonly="true"]-->
        <article class="hentry">
        <!--[ajaxpanel id="ap_threadcontent" idonly="true"]-->
            <!-- #include file="_postauthor.aspx" post="$threadContent" -->
            <h1 class="entry-title">
                $thread.SubjectText
                <!--[if $thread.IsClosed]--><em>(已解决)</em><!--[else]--><em>(未解决)</em> <!--[if $QuestionThread.AlwaysEyeable == false]--><em>(需要回复后才可见他人回复)</em><!--[/if]--><!--[/if]-->
                <!--[if $thread.IsValued]--><em>[精华帖]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]--><em>[总置顶]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.Sticky]--><em>[置顶]</em><!--[/if]-->
                <!--[if $IsShowThreadUpdateSortOrder]--><em>[自动沉帖]</em><!--[/if]-->
            </h1>
            <!-- #include file="_postalert.aspx" post="$threadContent" -->
        <!--[if $IsShowContent($threadcontent) && $CanSeeContent($threadcontent)]-->
            <div class="postextra">
                <p>截止时间: <!--[if $questionThread.ExpiresDate == DateTime.MaxValue]-->无限期<!--[else]-->$outputDatetime($questionThread.ExpiresDate)<!--[/if]--></p>
                <p>悬赏: $QuestionPoint.Name $questionThread.Reward $QuestionPoint.UnitName</p>
                <p>最多奖励回复: $questionThread.RewardCount</p>
            </div>
            <div class="entry-content">
                $Highlight($threadcontent.ContentText)
            </div>
            <!-- #include file="_postlastmodify.aspx" post="$threadContent" -->
            <!--[if ($CanReplyThread && $isShowReply) || $IsShowFinalQuestionLink]-->
            <div class="askpostbutton">
                <!--[if $CanReplyThread && $isShowReply]-->
                <a class="button" href="$url($codename/post)?threadid=$threadContent.threadID&action=reply">我来回答</a>
                <!--[/if]-->
                <!--[if $IsShowFinalQuestionLink]-->
                <a class="button" href="$Dialog/finalquestion.aspx?threadid=$threadid" onclick="return openDialog(this.href, refresh)">结帖</a>
                <!--[/if]-->
            </div>
            <!--[/if]-->
            <!-- #include file="_postattachment.aspx" post="$threadContent" -->
            <!-- #include file="_postscore.aspx" post="$threadContent" -->
        <!--[/if]-->
            <!-- #include file="_postsignature.aspx" post="$threadContent" -->
            <!-- #include file="_postaction.aspx" post="$threadContent" -->
        <!--[/ajaxpanel]-->
        </article>

        <!--[if $bestPost != null]-->
        <article class="hentry">
            <!-- #include file="_postauthor.aspx" post="$bestPost" -->
            <p class="askpost-bestanswer"><span>最佳答案</span></p>
            <!--[if $threadcontent.SubjectText != ""]-->
            <h1 class="entry-title">$threadcontent.SubjectText</h1>
            <!--[/if]-->
            <!-- #include file="_postalert.aspx" post="$bestpost" -->
        <!--[if $IsShowContent($bestpost) && $CanSeeContent($bestpost)]-->
            <!--[if $Thread.IsClosed]-->
            <div class="postextra">
                <p>奖励: $GetReward($bestPost.PostID) $QuestionPoint.UnitName</p>
            </div>
            <!--[/if]-->
            <div class="entry-content">
                $Highlight($bestpost.ContentText)
            </div>
            <!-- #include file="_postlastmodify.aspx" post="$bestpost" -->
            <!-- #include file="_postattachment.aspx" post="$bestpost" -->
            <!-- #include file="_postscore.aspx" post="$bestpost" -->
            <div class="askpost-useful">
                <!--[ajaxpanel id="ap_useful" idonly="true"]-->
                <a href="javascript:;" id="UsefulButton" onclick="return ajaxSubmit('threadform',this.id,'ap_useful',ajaxCallback);"><strong>$QuestionThread.UsefulCount</strong> 有用</a>
                <a href="javascript:;" id="UnUsefulButton" onclick="return ajaxSubmit('threadform',this.id,'ap_useful',ajaxCallback);"><strong>$QuestionThread.UnusefulCount</strong> 无用</a>
                <!--[/ajaxpanel]-->
            </div>
        <!--[/if]-->
            <!-- #include file="_postsignature.aspx" post="$bestpost" -->
            <!-- #include file="_postaction.aspx" post="$bestpost" -->
        </article>
        <!--[/if]-->

        <!--[if $postlist.count > 0 && $IsShowPostList]-->
        <!--[loop $Post in $postlist with $loopindex]-->
        <!-- #include file="_post.aspx" post="$Post" -->
        <!--[/loop]-->
        <!--[/if]-->
        
        <!--[/ajaxpanel]-->

        <div class="pagination">
            <!--[pager name="PostListPager" skin="../_inc/_pager.aspx"]-->
        </div>
        </form>
    </section>

    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>