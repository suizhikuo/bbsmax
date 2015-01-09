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
        <a href="$url(default)">&laquo; 返回首页</a>
    </div>

    <section class="main forumtopics">
    <!--[if $IsNormalThreads && $forum.SubForumsForList.Count > 0]-->
        <h3><span>$forum.ForumName子板块</span></h3>
        <ul class="forumcates">
        <!--[loop $subforum in $forum.SubForumsForList]-->
            <li>
                <a href="$url($subforum.CodeName/list-1)">
                    <span class="inner">
                        <span class="title">$subforum.ForumName</span>
                        <!--[if $subforum.TodayPostsWithSubForums > 0]-->
                        <em class="count">$subforum.TodayPostsWithSubForums</em>
                        <!--[/if]-->
                        <span class="status">
                            $subforum.TotalThreadsWithSubForums / $subforum.TotalPostsWithSubForums
                            <!--[if $CanSeeLastUpdate($subforum) && $subforum.LastThread != null]-->
                            - $outputFriendlyDateTime($subforum.LastThread.UpdateDate)更新
                            <!--[/if]-->
                        </span>
                    </span>
                </a>
            </li>
        <!--[/loop]-->
        </ul>
    <!--[/if]-->
    <!--[if $forum.ForumType == ForumType.Normal && $IsCatalogForum == false]-->
    <!--[ajaxpanel id="ap_threads" idonly="true"]-->
        <!--[if $IsNormalThreads]-->
        <div class="publishbutton">
            <a class="post" href="$url($codename/post)?action=thread">发帖</a>
        </div>
        <!--[/if]-->
        <h3><span>$forum.ForumName</span></h3>
        <!--[if $StickThreads.Count > 0]-->
        <ul class="topiclist topiclist-sticky">
            <!--[loop $thread in $StickThreads]-->
            <li>
                <a href="$url($codename/$Thread.ThreadTypeString-$Thread.ThreadID-1)" class="<!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]-->topic-pinned<!--[else]-->topic-sticky<!--[/if]-->">
                    <span class="inner">
                        <span class="title <!--[if $thread.ThreadType == ThreadType.Poll]-->figure figure-poll<!--[else if $thread.ThreadType == ThreadType.Question]-->figure figure-question<!--[else if $thread.ThreadType == ThreadType.Polemize]-->figure figure-polemize<!--[/if]--> <!--[if $thread.ThreadType == ThreadType.Normal]--><!--[if $thread.IsValued]-->figure figure-best<!--[/if]--><!--[if $thread.AttachmentType == ThreadAttachType.Image]-->figure figure-image<!--[else if $thread.AttachmentType == ThreadAttachType.Normal]-->figure figure-attachment<!--[/if]--><!--[/if]-->">
                            <strong>$GetThreadCatalogName($thread.ThreadCatalogID, @"[{0}]") $GetThreadLink($thread, 60, @"{1}")</strong>
                        </span>
                        <span class="author"><!--[if $thread.PostUserID != 0 || $thread.PostUsername != ""]-->$thread.PostUsername<!--[else]-->匿名游客<!--[/if]--></span>
                        <time>$outputDate($thread.CreateDate)</time>
                        <!--[if $thread.TotalReplies > 0]--><span class="stats">$thread.TotalReplies回复</span><!--[/if]-->
                    </span>
                </a>
            </li>
            <!--[/loop]-->
        </ul>
        <!--[/if]-->
        <!--[if $NormalThreads.Count > 0]-->
        <ul class="topiclist">
            <!--[loop $thread in $NormalThreads]-->
            <li>
                <a href="$url($codename/$Thread.ThreadTypeString-$Thread.ThreadID-1)" class="<!--[if $thread.IsLocked]-->topic-lock<!--[else if $thread.TotalReplies >= $HotThreadRequireReplies]-->topic-hot<!--[else]-->topic-normal<!--[/if]-->">
                    <span class="inner">
                        <span class="title <!--[if $thread.ThreadType == ThreadType.Poll]-->figure figure-poll<!--[else if $thread.ThreadType == ThreadType.Question]-->figure figure-question<!--[else if $thread.ThreadType == ThreadType.Polemize]-->figure figure-polemize<!--[/if]--> <!--[if $thread.ThreadType == ThreadType.Normal]--><!--[if $thread.IsValued]-->figure figure-best<!--[/if]--><!--[if $thread.AttachmentType == ThreadAttachType.Image]-->figure figure-image<!--[else if $thread.AttachmentType == ThreadAttachType.Normal]-->figure figure-attachment<!--[/if]--><!--[/if]-->">
                            <strong>$GetThreadCatalogName($thread.ThreadCatalogID, @"[{0}]") $GetThreadLink($thread, 60, @"{1}")</strong>
                        </span>
                        <span class="author"><!--[if $thread.PostUserID != 0 || $thread.PostUsername != ""]-->$thread.PostUsername<!--[else]-->匿名游客<!--[/if]--></span>
                        <time>$outputDate($thread.CreateDate)</time>
                        <!--[if $thread.TotalReplies > 0]--><span class="stats">$thread.TotalReplies回复</span><!--[/if]-->
                    </span>
                </a>
            </li>
            <!--[/loop]-->
        </ul>
        <div class="pagination">
            <!--[pager name="ThreadListPager" skin="../_inc/_pager.aspx"]-->
        </div>
        <!--[else]-->
        <div class="nodata">
            当前版块暂时没有主题.
        </div>
        <!--[/if]-->
    <!--[/ajaxpanel]-->
    <!--[/if]-->
    </section>

    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>