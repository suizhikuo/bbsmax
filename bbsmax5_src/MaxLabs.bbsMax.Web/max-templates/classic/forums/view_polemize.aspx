<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_forumhtmlhead_.aspx"-->
</head>
<body>
<div class="container">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main nosidebar section-forumview">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix postoperate">
                                <div class="topic-action">
                                    <div class="clearfix action-button">
                                        <a href="$url($codename/post)?threadid=$threadID&action=reply"><span><img src="$skin/images/theme/button_reply.png" alt="" /></span></a>
                                        <a id="action-newtopic" href="$url($codename/post)?action=thread"><span><img src="$skin/images/theme/button_post.png" alt="" /></span></a>
                                        <!--[if $IsShowModeratorManageLink]-->
                                        <a class="action-manage" id="action-manage1" href="#"><span><img src="$skin/images/theme/button_manage.png" alt="" /></span></a>
                                        <!--[/if]-->
                                    </div>
                                </div>
                            </div>
                            
                            <form id="threadform" method="post" enctype="multipart/form-data" action="$_form.action">
                            <input type="hidden" name="threadids" value="$Thread.ThreadID" />
                            <div class="clearfix postcaption">
                                <h3 class="postcaption-title"><span>$thread.SubjectText</span></h3>
                                <div class="poststats">
                                    <!--[ajaxpanel id="ap_threadinfo" idonly="true"]-->
                                    <ul class="clearfix poststats-list">
                                        <li><span class="numeric">$thread.TotalViews</span> <span class="label">阅读</span></li>
                                        <li><span class="numeric">$thread.TotalReplies</span> <span class="label">回复</span></li>
                                    </ul>
                                    <!--[/ajaxpanel]-->
                                </div>
                            </div>
                            <div class="postallentry posttype-debate">
                                <!--[ajaxpanel id="ap_postlist" idonly="true"]-->
                                <div class="clearfix postlayout postmajorentry">
                                    <table class="postlayout-table">
                                        <tr>
                                            <td class="postlayout-content">
                                                <!--[ajaxpanel id="ap_threadcontent" idonly="true"]--> 
                                                <div class="panel extrapost debatepost">
                                                    <div class="panel-head">
                                                        <h3 class="panel-title"><span>辩论 <!--[if $Thread.IsClosed]-->(己结束)<!--[/if]--></span></h3>
                                                    </div>
                                                    <div class="panel-body postwrap">
                                                        <table class="posttable">
                                                            <tr>
                                                                <td class="postmain">
                                                                    <div class="postmain-inner">
                                                                        
                                                                        <div class="clearfix post-head">
                                                                            <!-- #include file="_postshare.aspx" -->
                                                                            <div class="post-heading">
                                                                                <h1>
                                                                                    $GetIcon($threadcontent.IconID)
                                                                                    $thread.SubjectText
                                                                                    <!--[if $thread.IsValued]--><em class="post-profile">[精华帖]</em><!--[/if]-->
                                                                                    <!--[if $thread.ThreadStatus == ThreadStatus.GlobalSticky]--><em class="post-profile">[总置顶]</em><!--[/if]-->
                                                                                    <!--[if $thread.ThreadStatus == ThreadStatus.Sticky]--><em class="post-profile">[置顶]</em><!--[/if]-->
                                                                                    <!--[if $IsShowThreadUpdateSortOrder]--><em class="post-profile">[自动沉帖]</em><!--[/if]-->
                                                                                </h1>
                                                                            </div>
                                                                        </div>
                                                                        
                                                                        <!--[if $thread.Judgement!=null]-->
                                                                        <div class="post-judgement"><img src="$thread.Judgement.LogoUrl" alt="" /></div>
                                                                        <!--[/if]-->
                                                                        
                                                                        <div class="clearfix post-meta">
                                                                            <p class="post-info">
                                                                                发起于 $outputFriendlyDateTime($threadContent.CreateDate)
                                                                                <!--[if $LookUserID > 0]-->
                                                                                - <a href="$url($codename/$thread.ThreadTypeString-$threadid-1)" class="post-filter">查看全部</a>
                                                                                <!--[else if $threadContent.UserID != 0]-->
                                                                                - <a href="$url($codename/thread-$threadid-1)?userid=$threadContent.UserID" class="post-filter">只看楼主</a>
                                                                                <!--[/if]-->
                                                                            </p>
                                                                            <!-- #include file="_postrank.aspx" -->
                                                                            <p class="textresize" onmouseover="this.className+=' textresize-expand';" onmouseout="this.className=this.className.replace('textresize-expand','');">
                                                                                <a class="label" href="javascript:void(0);"><span><em>.</em>字号</span></a>
                                                                                <span class="textresize-list">
                                                                                    <a href="javascript:void(0);" class="small" onclick="PostViewStyle(this, 'fontsize-small')">较小字号</a>
                                                                                    <a href="javascript:void(0);" class="medium" onclick="PostViewStyle(this, 'fontsize-medium')">正常字号</a>
                                                                                    <a href="javascript:void(0);" class="large" onclick="PostViewStyle(this, 'fontsize-large')">较大字号</a>
                                                                                </span>
                                                                            </p>
                                                                            <!--[if $OutputIpPartCount > 0]-->
                                                                            <p class="post-authorip">
                                                                                <!--[if $CanShowIpArea]-->
                                                                                <a onclick="return openDialog(this.href);" href="$dialog/ip.aspx?ip=$OutputIP($threadContent.IpAddress)"><span><em>.</em>IP <code>$OutputIP($threadContent.IpAddress)</code></span></a>
                                                                                <!--[else]-->
                                                                                <a href="javascript:void(0);"><span><em>.</em>IP <code>$OutputIP($threadContent.IpAddress)</code></span></a>
                                                                                <!--[/if]-->
                                                                            </p>
                                                                            <!--[/if]-->
                                                                        </div>
                                                                        
                                                                        <!--[if $HasInPostTopAD(0,false)]-->
                                                                        <div class="ad-text-post">
                                                                            $InPostTopAD(-1,false)
                                                                        </div>
                                                                        <!--[/if]-->

                                                                        <table class="postcontentwrap">
                                                                            <tr>
                                                                                <td class="postcontentwrap-main">
                                                                                
                                                                        <!-- #include file="_postalert.aspx" post="$threadContent" -->
                                                                        
                                                                        <!--[if $IsShowContent($threadcontent) && $CanSeeContent($threadcontent)]-->
                                                                        <div class="debatepost-data">
                                                                            <div class="debatepost-endtime">
                                                                                <span class="label">参与人数</span>
                                                                                <span class="value">$PolemizeUserCount</span>
                                                                            </div>
                                                                            <div class="debatepost-endtime">
                                                                                <!--[if $PolemizeThread.ExpiresDate == DateTime.MaxValue]-->
                                                                                <span class="label">结束时间</span>
                                                                                <span class="value">无限期</span>
                                                                                <!--[else if $PolemizeThread.ExpiresDate < DateTimeUtil.Now]-->
                                                                                <span class="value">已于 $PolemizeThread.ExpiresDate 结束</span>
                                                                                <!--[else]-->
                                                                                <span class="label">结束时间</span>
                                                                                <span class="value">$outputDateTime($PolemizeThread.ExpiresDate)</span>
                                                                                <!--[/if]-->
                                                                            </div>
                                                                        </div>
                                                                        
                                                                        <div class="clearfix post-content">
                                                                            $Highlight($threadcontent.ContentText)
                                                                        </div>
                                                                        
                                                                            <!--[if $threadcontent.LastEditorID > 0]-->
                                                                        <div class="post-lastmodify">
                                                                            本帖最后由 $threadcontent.LastEditor 于 $outputFriendlyDatetime($threadcontent.UpdateDate) 编辑
                                                                        </div>
                                                                            <!--[/if]-->
                                                                        
                                                                        <div class="debateentry">
                                                                            <table class="debateentry-layout">
                                                                                <tr>
                                                                                    <td class="debate-right">
                                                                                        <div class="debate-right-inner">
                                                                                            <h3 class="debate-title">
                                                                                                正方观点
                                                                                                <!--[if $PolemizeThread.AgreeCount > $PolemizeThread.AgainstCount && $Thread.IsClosed == false]-->
                                                                                                (领先)
                                                                                                <!--[else if $PolemizeThread.AgreeCount > $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                                                                                                (获胜)
                                                                                                <!--[else if $PolemizeThread.AgreeCount == $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                                                                                                (平手)
                                                                                                <!--[/if]-->
                                                                                            </h3>
                                                                                            <div class="clearfix dabatechart">
                                                                                                <div class="chart-approve">$PolemizeThread.AgreeCount</div>
                                                                                                <div class="chart-figure">
                                                                                                    <div class="chart-index" style="width:{=$AgreePercent}%;"><div><div>&nbsp;</div></div></div>
                                                                                                    <div class="chart-status" style="margin-left:{=$AgreePercent}%;">{=$AgreePercent}%</div>
                                                                                                </div>
                                                                                                <!--[if $Thread.IsClosed == false && $MyViewPointType == null]-->
                                                                                                <div class="debatesubmit">
                                                                                                    <a class="debatesubmit-button" id="AgreePolemize" href="javascript:;" onclick="return ajaxSubmit('threadform',this.id,'ap_threadcontent',ajaxCallback);" title="支持正方观点"><span>支持</span></a>
                                                                                                </div>
                                                                                                <!--[/if]-->
                                                                                            </div>
                                                                                            <div class="debateviewpoint">
                                                                                                $polemizeThread.AgreeViewPoint
                                                                                            </div>
                                                                                        </div>
                                                                                    </td>
                                                                                    <td class="layout-gap">
                                                                                        <div class="layout-gap-inner">&nbsp;</div>
                                                                                    </td>
                                                                                    <td class="debate-left">
                                                                                        <div class="debate-left-inner">
                                                                                            <h3 class="debate-title">
                                                                                                反方观点
                                                                                                <!--[if $PolemizeThread.AgreeCount < $PolemizeThread.AgainstCount && $Thread.IsClosed == false]-->
                                                                                                (领先)
                                                                                                <!--[else if $PolemizeThread.AgreeCount < $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                                                                                                (获胜)
                                                                                                <!--[else if $PolemizeThread.AgreeCount == $PolemizeThread.AgainstCount && $Thread.IsClosed]-->
                                                                                                (平手)
                                                                                                <!--[/if]-->
                                                                                            </h3>
                                                                                            <div class="clearfix dabatechart">
                                                                                                <div class="chart-approve">$PolemizeThread.AgainstCount</div>
                                                                                                <div class="chart-figure">
                                                                                                    <div class="chart-index" style="width:{=$AgainstPercent}%;"><div><div>&nbsp;</div></div></div>
                                                                                                    <div class="chart-status" style="margin-left:{=$AgainstPercent}%;">{=$AgainstPercent}%</div>
                                                                                                </div>
                                                                                                <!--[if $Thread.IsClosed == false && $MyViewPointType == null]-->
                                                                                                <div class="debatesubmit">
                                                                                                    <a class="debatesubmit-button" href="javascript:;" title="支持反方观点" id="AgainstPolemize" onclick="return ajaxSubmit('threadform',this.id,'ap_threadcontent',ajaxCallback);"><span>支持</span></a>
                                                                                                </div>
                                                                                                <!--[/if]-->
                                                                                            </div>
                                                                                            <div class="debateviewpoint">
                                                                                                $polemizeThread.AgainstViewPoint
                                                                                            </div>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                            </table>
                                                                            
                                                                            <!--[if $Thread.IsClosed]-->
                                                                            <div class="debatetip">
                                                                                此次辩论已经结束.
                                                                            </div>
                                                                            <!--[/if]-->
                                                                        
                                                                            <!--[if $MyViewPointType != null]-->
                                                                            <div class="debatetip">
                                                                                您已经支持过
                                                                                <!--[if $MyViewPointType == ViewPointType.Agree]-->
                                                                                正方
                                                                                <!--[else if $MyViewPointType == ViewPointType.Against]-->
                                                                                反方
                                                                                <!--[else]-->
                                                                                中方
                                                                                <!--[/if]-->
                                                                                观点了, 谢谢您的参与.
                                                                            </div>
                                                                            <!--[/if]-->
                                                                        </div>
                                                                        
                                                                        <!-- #include file="_postattachment.aspx" post="$threadContent" -->
                                                                        <!-- #include file="_postscore.aspx" post="$threadContent" -->
                                                                        
                                                                        <!--[/if]-->
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="postmain-signature">
                                                                    <div class="clearfix postmain-signature-inner">
                                                                        <!--[if $isShowSignature($threadcontent)]-->
                                                                        <div class="post-signature">
                                                                            $threadcontent.User.SignatureFormatted
                                                                        </div>
                                                                        <!--[/if]-->
                                                                        <!--[if $HasInPostBottomAD(0,false)]-->
                                                                        <div class="ad-text-postbottom">
                                                                            $InPostBottomAD(-1,false)
                                                                        </div>
                                                                        <!--[/if]-->
                                                                        <!-- #include file="_postmanage.aspx" post="$threadContent" -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="postmain-tools">
                                                                    <div class="clearfix postmain-tools-inner">
                                                                        <!-- #include file="_postaction.aspx"  post="$threadContent" -->
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                                <!--[/ajaxpanel]-->
                                            </td>
                                            <td class="postlayout-sidebar">
                                                <div class="postauthor">
                                                    <!-- #include file="_postauthorinfo.aspx" post="$threadContent" -->
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <!--[if $postlist.count > 0 || $CurrentPostType != null]-->
                                <div class="clearfix postoperate">
                                    <div class="filtertab debatefilter">
                                        <h3 class="tab-title">按立场筛选</h3>
                                        <ul class="clearfix tab-list">
                                            <li><a $_if($CurrentPostType == null,'class="current"') href="$url($codename/polemize-$threadID-1-1)">全部</a></li>
                                            <li><a $_if($CurrentPostType == PostType.Polemize_Agree,'class="current"') href="$url($codename/polemize-$threadID-1-1)?posttype=2">正方</a></li>
                                            <li><a $_if($CurrentPostType == PostType.Polemize_Against,'class="current"') href="$url($codename/polemize-$threadID-1-1)?posttype=3">反方</a></li>
                                            <li><a $_if($CurrentPostType == PostType.Polemize_Neutral,'class="current"') href="$url($codename/polemize-$threadID-1-1)?posttype=4">中立</a></li>
                                        </ul>
                                    </div>
                                    <div class="pagination">
                                        <div class="pagination-inner">
                                            <!--[ajaxpanel id="ap_pager_up" idonly="true"]-->
                                            <a class="back" href="$ForumUrl">&laquo; 返回列表</a>
                                            <!--[pager name="PostListPager" skin="../_inc/_pager_bbs.aspx"]-->
                                            <!--[/ajaxpanel]-->
                                        </div>
                                    </div>
                                </div>
                                <!--[/if]-->
                                <!--[if $postlist.count > 0]-->
                                <div class="postreplies">
                                <!--[loop $Post in $postlist with $loopindex]-->
                                    <!-- #include file="_post.aspx" post="$Post" -->
                                <!--[/loop]-->
                                </div>
                                <!--[else]-->
                                <!--[if $CurrentPostType != null]-->
                                <div class="debatenopost">
                                    <!--[if $CurrentPostType == PostType.Polemize_Agree]-->
                                    没有没有支持正方观点的回复.
                                    <!--[else if $CurrentPostType == PostType.Polemize_Against]-->
                                    当前没有支持反方观点的回复.
                                    <!--[else if $CurrentPostType == PostType.Polemize_Neutral]-->
                                    当前没有支持中立观点的回复.
                                    <!--[/if]-->
                                </div>
                                <!--[/if]-->
                                <!--[/if]-->
                                <!--[/ajaxpanel]-->
                            </div>
                            </form>
                            <!-- #include file="_threadquickreply.aspx"-->
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
<!--[if $IsShowModeratorManageLink]-->
    <!--#include file="../_inc/_managerusermenu.aspx"-->
<!--[/if]-->
<!--#include file="_threadbottom.aspx"-->
<script type="text/javascript">
new popup('action-newtopic-list',['action-newtopic2','action-newtopic'],true);
<!--[if $IsShowModeratorManageLink]-->
new popup('action-manage-list',['action-manage2','action-manage1'],true);
<!--[/if]-->
</script>
</body>
</html>
