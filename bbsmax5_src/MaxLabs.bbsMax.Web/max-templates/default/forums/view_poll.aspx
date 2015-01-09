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
                            <div class="postallentry posttype-poll">
                                <!--[ajaxpanel id="ap_postlist" idonly="true"]-->
                                <div class="clearfix postlayout postmajorentry">
                                    <div class="postlayout-content">
                                        <div class="postlayout-content-inner">
                                            <!--[ajaxpanel id="ap_threadcontent" idonly="true"]--> 
                                            <div class="panel extrapost pollpost">
                                                <div class="panel-head">
                                                    <h3 class="panel-title"><span>投票 <!--[if $PollThread.IsClosed]-->(己结束)<!--[/if]--></span></h3>
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
                                                                    
                                                                    <table class="postcontentwrap">
                                                                        <tr>
                                                                            <td class="postcontentwrap-main">
                                                                    
                                                                    <!--[if $HasInPostTopAD(0,false)]-->
                                                                    <div class="ad-text-post">
                                                                        $InPostTopAD(-1,false)
                                                                    </div>
                                                                    <!--[/if]-->
                                                                    
                                                                    <!-- #include file="_postalert.aspx" post="$threadContent" -->
                                                                    
                                                                    
                                                                    <!--[if $IsShowContent($threadcontent) && $CanSeeContent($threadcontent)]-->
                                                                    <div class="pollpost-type">
                                                                        <strong>投票项</strong> ($VoteType)
                                                                    </div>
                                                                    <div class="pollpost-stats">
                                                                        共 $pollThread.VotedUserCount 人参与, 总票数 $voteTotalCount
                                                                        <!--[if $CanViewPollDetail]-->
                                                                        <a class="viewusers" href="$Dialog/votedusers.aspx?threadid=$threadid" onclick="return openDialog(this.href, null)">[查看投票用户]</a>
                                                                        <!--[/if]-->
                                                                    </div>
                                                                    <div class="pollpost-endtime">
                                                                        <!--[if $pollThread.ExpiresDate == DateTime.MaxValue]-->
                                                                        <span class="label">截止时间</span>
                                                                        <span class="date">无限期</span>
                                                                        <!--[else]-->
                                                                        <span class="label">截止时间</span>
                                                                        <span class="date">$outputDateTime($pollThread.ExpiresDate) <!--[if $pollThread.ExpiresDate < DateTimeUtil.Now]-->(已截止)<!--[/if]--></span>
                                                                        <!--[/if]-->
                                                                    </div>
                                                                    
                                                                    <div class="pollentry">
                                                                        <!--[error name="vote"]-->
                                                                        <div class="errormsg">$Message</div>
                                                                        <!--[/error]-->
                                                                        <input type="hidden" value="" name="pollItem" />
                                                                        <div class="pollitemstable">
                                                                            <table class="pollitems-table">
                                                                                <!--[loop $pollItem in $PollThread.pollItems with $loopIndex]-->
                                                                                <tr>
                                                                                    <td class="target">
                                                                                        <!--[if $pollThread.Multiple<2]-->
                                                                                        <input type="radio" name="pollItem" id="poll_$loopIndex" value="$pollItem.ItemID" $_form.checked("pollItem",$pollItem.ItemID) />
                                                                                        <!--[else]-->
                                                                                        <input type="checkbox" name="pollItem" id="poll_$loopIndex" value="$pollItem.ItemID" $_form.checked("pollItem",$pollItem.ItemID) />
                                                                                        <!--[/if]-->
                                                                                    </td>
                                                                                    <td class="entry">
                                                                                        <div class="pollitem">
                                                                                            <!--[if $CanViewPollItemVotedCount]-->
                                                                                            <div class="clearfix pollchart pollchart-style{=$loopIndex%10+1}">
                                                                                                <div class="chart-figure"><div class="chart-index" style="width:{=$GetPercent($pollItem.PollItemCount,$voteTotalCount)}%;"><div>{=$GetPercent($pollItem.PollItemCount,$voteTotalCount)}%</div></div></div>
                                                                                                <div class="chart-status">{=$GetPercent($pollItem.PollItemCount,$voteTotalCount)}% <!--[if $pollItem.PollItemCount > 0]-->($pollItem.PollItemCount票)<!--[/if]--></div>
                                                                                            </div>
                                                                                            <!--[/if]-->
                                                                                            <div class="clearfix poll-title">
                                                                                                <span class="number">{=$loopIndex+1}.</span>
                                                                                                <label class="title" for="poll_$loopIndex">$pollItem.ItemName</label>
                                                                                            </div>
                                                                                        </div>
                                                                                    </td>
                                                                                </tr>
                                                                                <!--[/loop]-->
                                                                            </table>
                                                                        </div>
                                                                        
                                                                        <!--[if $pollThread.ExpiresDate > DateTimeUtil.Now && $pollThread.IsVoted($MyUserID) == false]-->
                                                                        <!--[ValidateCode actionType="Vote"]-->
                                                                        <div class="clearfix pollcaptcha">
                                                                            <h3 class="label"><label for="$inputName">验证码</label> <span class="captcha-tip">$tip (点击输入框显示)</span></h3>
                                                                            <div class="captcha-input">
                                                                                <input type="text" name="$inputName" id="$inputName" class="text" onfocus ="showVCode(this,'$imageurl');" $_if($disableIme,'style="ime-mode:disabled;"') />
                                                                            </div>
                                                                        </div>
                                                                        <!--[/ValidateCode]-->
                                                                        <div class="clearfix pollsubmit">
                                                                            <span class="minbtn-wrap minbtn-highlight"><span class="btn"><input class="button" id="voteButton" name="voteButton" type="submit"  onclick="return ajaxSubmit('threadform', 'voteButton','ap_threadcontent',ajaxCallback);" value="投票" <!--[if $type != ""]-->disabled="disabled"<!--[/if]--> /></span></span>
                                                                        </div>
                                                                        <!--[/if]-->
                                                                        
                                                                        <!--[if $pollThread.ExpiresDate <= DateTimeUtil.Now]-->
                                                                        <div class="polltip">
                                                                            此次投票已经结束.
                                                                        </div>
                                                                        <!--[/if]-->
                                                                        <!--[if $pollThread.IsVoted($MyUserID)]-->
                                                                        <div class="polltip">
                                                                            您已经投过票了, 谢谢您的参与.
                                                                        </div>
                                                                        <!--[/if]-->
                                                                    </div>
                                                                    
                                                                    <div class="clearfix post-content">
                                                                        $Highlight($threadcontent.ContentText)
                                                                    </div>
                                                                    
                                                                        <!--[if $threadcontent.LastEditorID > 0]-->
                                                                    <div class="post-lastmodify">
                                                                        本帖最后由 $threadcontent.LastEditor 于 $outputFriendlyDatetime($threadcontent.UpdateDate) 编辑
                                                                    </div>
                                                                        <!--[/if]-->
                                                                        
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
                                                                    <!-- #include file="_postaction.aspx" post="$threadContent"-->
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </div>
                                            <!--[/ajaxpanel]-->
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
                                <!--[if $postlist.count > 0]-->
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
function postTo( url )
{
    postToDialog({formId:"threadform",url:url,callback:refresh});
}

new popup('action-newtopic-list',['action-newtopic2','action-newtopic'],true);
<!--[if $IsShowModeratorManageLink]-->
new popup('action-manage-list',['action-manage2','action-manage1'],true);
<!--[/if]-->
</script>
</body>
</html>