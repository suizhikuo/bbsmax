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
    <div id="main" class="main section-forumview">
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
                            <div class="postallentry posttype-ask">
                                <!--[ajaxpanel id="ap_postlist" idonly="true"]-->
                                <div class="clearfix postlayout postmajorentry">
                                    <div class="postlayout-content">
                                        <div class="postlayout-content-inner" id="post_$threadContent.PostID">
                                            <!--[ajaxpanel id="ap_threadcontent" idonly="true"]-->
                                            <div class="panel extrapost questionpost" <!--[if $threadContent.PostID == $LastPostID]-->id="last"<!--[/if]-->>
                                                <div class="panel-head">
                                                    <!--[if $thread.IsClosed]-->
                                                    <h3 class="panel-title ask-solved"><span>已解决</span></h3>
                                                    <!--[else]-->
                                                    <h3 class="panel-title ask-unsolved"><span>未解决 <!--[if $QuestionThread.AlwaysEyeable == false]-->(需要回复后才可见他人回复)<!--[/if]--></span></h3>
                                                    <!--[/if]-->
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
                                                                            发表于 $outputFriendlyDateTime($threadContent.CreateDate)
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
                                                                    <div class="askpost-endtime">
                                                                        <span class="label">截止时间:</span>
                                                                        <span class="date">
                                                                        <!--[if $questionThread.ExpiresDate == DateTime.MaxValue]-->
                                                                        无限期
                                                                        <!--[else]-->
                                                                        $outputDatetime($questionThread.ExpiresDate)
                                                                        <!--[/if]-->
                                                                        </span>
                                                                    </div>
                                                                    
                                                                    <div class="askpost-reward">
                                                                        <p>
                                                                            悬赏$QuestionPoint.Name: <strong class="value">$questionThread.Reward</strong> $QuestionPoint.UnitName
                                                                        </p>
                                                                        <p>
                                                                            最多奖励给<strong class="value">$questionThread.RewardCount</strong>个回复
                                                                        </p>
                                                                    </div>
                                                                    
                                                                    <div class="clearfix post-content">
                                                                        $Highlight($ThreadContent.ContentText)
                                                                    </div>
                                                                    
                                                                        <!--[if $threadcontent.LastEditorID > 0]-->
                                                                    <div class="post-lastmodify">
                                                                        本帖最后由 $threadcontent.LastEditor 于 $outputFriendlyDatetime($threadcontent.UpdateDate) 编辑
                                                                    </div>
                                                                        <!--[/if]-->
                                                                    
                                                                    <div class="clearfix askpost-button">
                                                                        <!--[if $CanReplyThread && $isShowReply]-->
                                                                        <a class="askpost-answerbutton" href="$url($codename/post)?threadid=$threadContent.threadID&action=reply"><span>我来回答</span></a>
                                                                        <!--[/if]-->
                                                                        <!--[if $IsShowFinalQuestionLink]-->
                                                                        <a class="askpost-finishbutton" href="$Dialog/finalquestion.aspx?threadid=$threadid" onclick="return openDialog(this.href, refresh)"><span>结帖</span></a>
                                                                        <!--[/if]-->
                                                                    </div>
                                                                    
                                                                    <!-- #include file="_postattachment.aspx" post="$threadContent" -->
                                                                    <!-- #include file="_postscore.aspx" post="$threadContent" -->
                                                                    
                                                                    <!--[/if]-->
                                                                            </td>
                                                                            <!--[if $HasInPostRightAD(0,false)]-->
                                                                            <td class="postcontentwrap-ad">
                                                                    <div class="ad-verticalbanner-post">
                                                                        $InPostRightAD(-1,false)
                                                                    </div>
                                                                            </td>
                                                                            <!--[/if]-->
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
                                                                    <!--[if $HasInPostBottomAD(0,false) -->
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
                                            <!--[if $bestPost != null]-->
                                            <div class="panel extrapost answerpost" id="$bestPost.PostID">
                                                <div class="panel-head">
                                                    <h3 class="panel-title"><span>最佳答案
                                                    </span></h3>
                                                </div>
                                                <div class="panel-body postwrap">
                                                    <table class="posttable">
                                                        <tr>
                                                            <td class="postauthor" rowspan="2">
                                                                <!-- #include file="_postauthorinfo.aspx" post="$bestPost" -->
                                                            </td>
                                                            <td class="postmain">
                                                                <div class="postmain-inner">
                                                                    <div class="clearfix post-head">
                                                                        <div class="post-heading">
                                                                            <h1>
                                                                                $GetIcon($bestPost.IconID)
                                                                                $Highlight($bestPost.SubjectText)
                                                                            </h1>
                                                                        </div>
                                                                    </div>
                                                                    
                                                                    <div class="clearfix post-meta">
                                                                        <p class="post-info">
                                                                            发表于 $outputFriendlyDateTime($bestPost.CreateDate)
                                                                            <!--[if $LookUserID > 0]-->
                                                                            - <a href="$url($codename/$thread.ThreadTypeString-$threadid-1)" class="post-filter">查看全部</a>
                                                                            <!--[else if $bestPost.UserID != 0]-->
                                                                            - <a href="$url($codename/thread-$threadid-1)?userid=$bestPost.UserID" class="post-filter">只看该用户</a>
                                                                            <!--[/if]-->
                                                                        </p>
                                                                        <!--[if $IsShowPostIndexAlias]-->
                                                                        <p class="post-number">$bestPost.PostIndexAlias</p>
                                                                        <!--[/if]-->
                                                                        <!--[if $OutputIpPartCount > 0]-->
                                                                        <p class="post-authorip">
                                                                            <!--[if $CanShowIpArea]-->
                                                                            <a onclick="return openDialog(this.href);" href="$dialog/ip.aspx?ip=$OutputIP($bestPost.IpAddress)"><span><em>.</em>IP <code>$OutputIP($bestPost.IpAddress)</code></span></a>
                                                                            <!--[else]-->
                                                                            <a href="javascript:void(0);"><span><em>.</em>IP <code>$OutputIP($bestPost.IpAddress)</code></span></a>
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
                                                                    
                                                                    <!-- #include file="_postalert.aspx" post="$bestpost" -->
                                                                    
                                                                    <!--[if $IsShowContent($bestpost) && $CanSeeContent($bestpost)]-->
                                                                    <div class="clearfix post-content">
                                                                        $Highlight($bestpost.ContentText)
                                                                    </div>
                                                                    
                                                                        <!--[if $bestpost.LastEditorID > 0]-->
                                                                    <div class="post-lastmodify">
                                                                        本帖最后由 $bestpost.LastEditor 于 $outputFriendlyDatetime($bestpost.UpdateDate) 编辑
                                                                    </div>
                                                                        <!--[/if]-->
                                                                        
                                                                        <!--[if $Thread.IsClosed]-->
                                                                    <div class="askpost-reward">
                                                                        <p>
                                                                            奖励: <strong class="value">$GetReward($bestPost.PostID)</strong> $QuestionPoint.UnitName
                                                                        </p>
                                                                    </div>
                                                                        <!--[/if]-->
                                                                        
                                                                    <!-- #include file="_postattachment.aspx" post="$bestpost" -->
                                                                    <!-- #include file="_postscore.aspx" post="$bestpost" -->
                                                                    
                                                                        <!--[ajaxpanel id="ap_useful" idonly="true"]-->
                                                                    <div class="askpost-useful">
                                                                        <a href="javascript:;" id="UsefulButton"  onclick="return ajaxSubmit('threadform',this.id,'ap_useful',ajaxCallback);"><b>$QuestionThread.UsefulCount</b>有用</a>
                                                                        <a href="javascript:;" id="UnUsefulButton"  onclick="return ajaxSubmit('threadform',this.id,'ap_useful',ajaxCallback);"><b>$QuestionThread.UnusefulCount</b>无用</a>
                                                                    </div>
                                                                        <!--[/ajaxpanel]-->
                                                                        
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
                                                                    <!--[if $isShowSignature($bestpost)]-->
                                                                    <div class="post-signature">
                                                                        $bestpost.User.SignatureFormatted
                                                                    </div>
                                                                    <!--[/if]-->
                                                                    <!--[if $HasInPostBottomAD(0,false)]-->
                                                                    <div class="ad-text-postbottom">
                                                                        $InPostBottomAD(-1,false)
                                                                    </div>
                                                                    <!--[/if]-->
                                                                    <!-- #include file="_postmanage.aspx"  post="$bestpost" -->
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="postauthor-tools">
                                                                <!--[if $my.ismanager]-->
                                                                <div class="clearfix postauthor-tools-inner">
                                                                    <div class="post-tools">
                                                                        <ul class="post-tools-list">
                                                                            <li><a class="action-usermanage" id="manageUserLink_$bestpost.UserID" href="javascript:void(0);" onclick="return openUserMenu(this,$bestpost.UserID)"><span><em>管理用户</em></span></a></li>
                                                                        </ul>
                                                                    </div>
                                                                </div>
                                                                <!--[/if]-->
                                                            </td>
                                                            <td class="postmain-tools">
                                                                <div class="clearfix postmain-tools-inner">
                                                                    <!-- #include file="_postaction.aspx" post="$bestpost" -->
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                            <!--[/if]-->
                                        </div>
                                    </div>
                                    <div class="postlayout-sidebar">
                                        <div class="postlayout-sidebar-inner">
                                            <div class="postauthor">
                                                <!-- #include file="_postauthorinfo.aspx" post="$threadContent" -->
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <!--[if $postlist.count > 0 && $IsShowPostList]-->
                                <div class="postgapwrap postgapwrap-bottom">
                                    <table class="postgaptable">
                                        <tr>
                                            <td class="col-left">
                                                <!--[ajaxpanel id="ap_threadinfo" idonly="true"]-->
                                                <div class="poststats">
                                                    <ul class="clearfix poststats-list">
                                                        <li><span class="numeric">$thread.TotalViews</span> <span class="label">阅读</span></li>
                                                        <li><span class="numeric">$thread.TotalReplies</span> <span class="label">回复</span></li>
                                                    </ul>
                                                </div>
                                                <!--[/ajaxpanel]-->
                                            </td>
                                            <td class="col-right">
                                                <div class="clearfix postoperate">
                                                    <div class="pagination">
                                                        <div class="pagination-inner">
                                                            <!--[ajaxpanel id="ap_pager_up" idonly="true"]-->
                                                            <a class="back" href="$ForumUrl">&laquo; 返回列表</a>
                                                            <!--[pager name="PostListPager" skin="../_inc/_pager_bbs.aspx"]-->
                                                            <!--[/ajaxpanel]-->
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                
                                <div class="postreplies">
                                    <!--[loop $Post in $postlist with $loopindex]-->
                                        <!-- #include file="_post.aspx" post="$Post" -->
                                    <!--[/loop]-->
                                </div>
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