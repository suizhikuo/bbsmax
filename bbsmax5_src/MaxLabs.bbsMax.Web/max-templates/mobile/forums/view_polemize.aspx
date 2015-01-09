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
                <!--[if $Thread.IsClosed]--><em>(己结束)</em><!--[/if]-->
                <!--[if $thread.IsValued]--><em>[精华帖]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]--><em>[总置顶]</em><!--[/if]-->
                <!--[if $thread.ThreadStatus == ThreadStatus.Sticky]--><em>[置顶]</em><!--[/if]-->
                <!--[if $IsShowThreadUpdateSortOrder]--><em>[自动沉帖]</em><!--[/if]-->
            </h1>
            <!-- #include file="_postalert.aspx" post="$threadContent" -->
        <!--[if $IsShowContent($threadcontent) && $CanSeeContent($threadcontent)]-->
            <div class="postextra">
                <p>参与人数: $PolemizeUserCount</p>
                <p>结束时间: <!--[if $PolemizeThread.ExpiresDate == DateTime.MaxValue]-->无限期<!--[else if $PolemizeThread.ExpiresDate < DateTimeUtil.Now]-->已于 $PolemizeThread.ExpiresDate 结束<!--[else]-->$outputDateTime($PolemizeThread.ExpiresDate)<!--[/if]-->
            </div>
            <div class="debateentry">
                <div class="debate-right">
                    <h3 class="debate-title">
                        正方观点
                        <span class="count">$PolemizeThread.AgreeCount</span>
                        <!--[if $PolemizeThread.AgreeCount > $PolemizeThread.AgainstCount && $Thread.IsClosed == false]-->
                        <em>(领先)</em>
                        <!--[else if $PolemizeThread.AgreeCount > $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                        <em>(获胜)</em>
                        <!--[else if $PolemizeThread.AgreeCount == $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                        <em>(平手)</em>
                        <!--[/if]-->
                    </h3>
                    <div class="dabatechart">
                        <div class="figure">
                            <div class="index" style="width:{=$AgreePercent}%;">&nbsp;</div>
                        </div>
                        <div class="status">{=$AgreePercent}%</div>
                    </div>
                    <!--[if $Thread.IsClosed == false && $MyViewPointType == null]-->
                    <div class="debatesubmit">
                        <a class="button" id="AgreePolemize" href="javascript:;" onclick="return ajaxSubmit('threadform',this.id,'ap_threadcontent',ajaxCallback);">支持</a>
                    </div>
                    <!--[/if]-->
                    <div class="debateviewpoint">
                        $polemizeThread.AgreeViewPoint
                    </div>
                </div>
                <div class="debate-left">
                    <h3 class="debate-title">
                        反方观点
                        <span class="count">$PolemizeThread.AgainstCount</span>
                        <!--[if $PolemizeThread.AgreeCount < $PolemizeThread.AgainstCount && $Thread.IsClosed == false]-->
                        <em>(领先)</em>
                        <!--[else if $PolemizeThread.AgreeCount < $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                        <em>(获胜)</em>
                        <!--[else if $PolemizeThread.AgreeCount == $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                        <em>(平手)</em>
                        <!--[/if]-->
                    </h3>
                    <div class="dabatechart">
                        <div class="figure">
                            <div class="index" style="width:{=$AgainstPercent}%;">&nbsp;</div>
                        </div>
                        <div class="status">{=$AgainstPercent}%</div>
                    </div>
                    <!--[if $Thread.IsClosed == false && $MyViewPointType == null]-->
                    <div class="debatesubmit">
                        <a class="button" href="javascript:;" id="AgainstPolemize" onclick="return ajaxSubmit('threadform',this.id,'ap_threadcontent',ajaxCallback);">支持</a>
                    </div>
                    <!--[/if]-->
                    <div class="debateviewpoint">
                        $polemizeThread.AgainstViewPoint
                    </div>
                </div>
            </div>
            <!--[if $Thread.IsClosed]-->
            <div class="debatetip">此次辩论已经结束.</div>
            <!--[/if]-->
            <!--[if $MyViewPointType != null]-->
            <div class="debatetip">您已经支持过<!--[if $MyViewPointType == ViewPointType.Agree]-->正方<!--[else if $MyViewPointType == ViewPointType.Against]-->反方<!--[else]-->中方<!--[/if]-->观点了, 谢谢您的参与.</div>
            <!--[/if]-->
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