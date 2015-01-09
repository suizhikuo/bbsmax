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
                <!--[if $PollThread.IsClosed]--><em>(己结束)</em><!--[/if]-->
                <!--[if $thread.IsValued]--><em>[精华帖]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]--><em>[总置顶]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.Sticky]--><em>[置顶]</em><!--[/if]-->
                <!--[if $IsShowThreadUpdateSortOrder]--><em>[自动沉帖]</em><!--[/if]-->
            </h1>
            <!-- #include file="_postalert.aspx" post="$threadContent" -->
        <!--[if $IsShowContent($threadcontent) && $CanSeeContent($threadcontent)]-->
            <div class="postextra">
                <p>统计: 共 $pollThread.VotedUserCount 人参与, 总票数 $voteTotalCount</p>
                <p>截止时间: <!--[if $pollThread.ExpiresDate == DateTime.MaxValue]-->无限期<!--[else]-->$outputDateTime($pollThread.ExpiresDate) <!--[if $pollThread.ExpiresDate < DateTimeUtil.Now]-->(已截止)<!--[/if]--><!--[/if]--></p>
            </div>
            
            <div class="pollentry">
                <p class="polltype">$VoteType</p>
                <!--[error name="vote"]-->
                <div class="errormsg">$Message</div>
                <!--[/error]-->
                <input type="hidden" value="" name="pollItem" />
                <ol class="pollentry-list <!--[if $IsLogin == false]--> inactive<!--[/if]--> <!--[if $pollThread.ExpiresDate <= DateTimeUtil.Now || $pollThread.IsVoted($MyUserID)]--> inactive<!--[/if]-->">
                    <!--[loop $pollItem in $PollThread.pollItems with $loopIndex]-->
                    <li>
                        <!--[if $pollThread.ExpiresDate > DateTimeUtil.Now && $pollThread.IsVoted($MyUserID) == false]-->
                        <!--[if $IsLogin]-->
                        <span class="checkbox">
                            <!--[if $pollThread.Multiple < 2]-->
                            <input type="radio" name="pollItem" id="poll_$loopIndex" value="$pollItem.ItemID" $_form.checked("pollItem",$pollItem.ItemID)>
                            <!--[else]-->
                            <input type="checkbox" name="pollItem" id="poll_$loopIndex" value="$pollItem.ItemID" $_form.checked("pollItem",$pollItem.ItemID)>
                            <!--[/if]-->
                        </span>
                        <!--[/if]-->
                        <!--[/if]-->
                        <!--[if $CanViewPollItemVotedCount]-->
                        <label class="pollchart pollchart-style-{=$loopIndex%5+1}" for="poll_$loopIndex">
                            <span class="figure"><span class="index" style="width:{=$GetPercent($pollItem.PollItemCount,$voteTotalCount)}%;"></span></span>
                        </label>
                        <!--[/if]-->
                        <label class="title" for="poll_$loopIndex">$pollItem.ItemName <!--[if $CanViewPollItemVotedCount]--><span class="count">({=$GetPercent($pollItem.PollItemCount,$voteTotalCount)}% / $pollItem.PollItemCount票)</span><!--[/if]--></label>
                    </li>
                    <!--[/loop]-->
                </ol>
                <!--[if $pollThread.ExpiresDate > DateTimeUtil.Now && $pollThread.IsVoted($MyUserID) == false]-->
                    <!--[if $IsLogin]-->
                <div class="pollsubmit">
                    <input class="button" id="voteButton" name="voteButton" type="submit" onclick="return ajaxSubmit('threadform', 'voteButton','ap_threadcontent',ajaxCallback);" value="投票" <!--[if $type != ""]-->disabled<!--[/if]-->>
                </div>
                    <!--[else]-->
                <div class="polltip">
                    你需要登录才能投票.
                </div>
                    <!--[/if]-->
                <!--[else if $pollThread.ExpiresDate <= DateTimeUtil.Now]-->
                <div class="polltip">
                    此次投票已经结束.
                </div>
                <!--[else if $pollThread.IsVoted($MyUserID)]-->
                <div class="polltip">
                    您已经投过票了, 谢谢您的参与.
                </div>
                <!--[/if]-->
            </div>
            <div class="entry-content">
                $Highlight($threadcontent.ContentText)
            </div>
            <!-- #include file="_postlastmodify.aspx" post="$threadContent" -->
            <!-- #include file="_postattachment.aspx" post="$threadContent" -->
            <!-- #include file="_postscore.aspx" post="$threadContent" -->
        <!--[/if]-->
            <!-- #include file="_postsignature.aspx" post="$threadContent" -->
            <!-- #include file="_postaction.aspx" post="$threadContent" -->
        <!--[/ajaxpanel]-->
        </article>

        <!--[if $postlist.count > 0]-->
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