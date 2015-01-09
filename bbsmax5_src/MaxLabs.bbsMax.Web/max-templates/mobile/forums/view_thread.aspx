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
            <a class="reply" href="$url($codename/post)?threadid=$threadID&action=reply&page=$PageNumber">回复</a>
        </div>
    <form id="threadform" method="post" enctype="multipart/form-data" action="$_form.action">
    <input type="hidden" name="threadids" value="$Thread.ThreadID" />
        <!--[ajaxpanel id="ap_postlist" idonly="true"]-->
        <!--[if $IsShowThreadContent && $threadContent != null]-->
        <article class="hentry">
        <!--[ajaxpanel id="ap_threadcontent" idonly="true"]-->
            <!-- #include file="_postauthor.aspx" post="$threadContent" -->
            <h1 class="entry-title">
                $thread.SubjectText
                <!--[if $thread.IsValued]--><em>[精华帖]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]--><em>[总置顶]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.Sticky]--><em>[置顶]</em><!--[/if]-->
                <!--[if $IsShowThreadUpdateSortOrder]--><em>[自动沉帖]</em><!--[/if]-->
            </h1>
            <!-- #include file="_postalert.aspx" post="$threadContent" -->
        <!--[if $IsShowContent($threadcontent) && $CanSeeContent($threadcontent)]-->
            <div class="entry-content">
                $Highlight($threadcontent.ContentText)
            </div>
                <!--[if $HasBuyed]-->
            <!-- #include file="_postlastmodify.aspx" post="$threadContent" -->
            <!-- #include file="_postattachment.aspx" post="$threadContent" -->
            <!-- #include file="_postscore.aspx" post="$threadContent" -->
                <!--[/if]-->
        <!--[/if]-->
            <!-- #include file="_postsignature.aspx" post="$threadContent" -->
            <!-- #include file="_postaction.aspx" post="$threadContent" -->
        <!--[/ajaxpanel]-->
        </article>
        <!--[/if]-->

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