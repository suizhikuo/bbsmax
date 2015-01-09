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

    <section class="main forumnav">
<!--[if $ForumCatalogs.Count > 0]-->
    <!--[loop $forumCatalog in $ForumCatalogs with $LoopIndex]-->
        <h3><a href="$url($forumCatalog.CodeName/list-1)">$forumCatalog.ForumName</a></h3>
        <!--[if $forumCatalog.SubForumsForList.count > 0]-->
        <ul class="forumcates">
        <!--[loop $forum in $forumCatalog.SubForumsForList]-->
            <li>
                <a href="$url($forum.CodeName/list-1)" class="<!--[if $forum.ForumType!=ForumType.Link && $forum.Password != ""]-->forum-lock<!--[else if $forum.CanVisit($my)]--><!--[if $forum.ForumType == ForumType.Link]-->forum-link<!--[else]-->forum-normal<!--[/if]--><!--[else]-->forum-banned<!--[/if]-->">
                    <span class="inner">
                        <span class="title">$forum.ForumName</span>
                        <!--[if $forum.TodayPostsWithSubForums > 0]-->
                        <em class="count">$forum.TodayPostsWithSubForums</em>
                        <!--[/if]-->
                        <span class="status">
                            $forum.TotalThreadsWithSubForums / $forum.TotalPostsWithSubForums
                            <!--[if $CanSeeLastUpdate($forum) && $forum.LastThread != null]-->
                            - $outputFriendlyDateTime($forum.LastThread.UpdateDate)更新
                            <!--[/if]-->
                        </span>
                    </span>
                </a>
            </li>
        <!--[/loop]-->
        </ul>
        <!--[else]-->
        <div class="nodata">
            该版块没有任何子版块.
        </div>
        <!--[/if]-->
    <!--[/loop]-->
<!--[else]-->
        <div class="nodata">
            论坛暂时未建立任何版块.
        </div>
<!--[/if]-->
    </section>

    <!--#include file="../_inc/_footer.aspx"-->
</div>
</body>
</html>