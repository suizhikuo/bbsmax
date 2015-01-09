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
                                                        <a class="action-reply" href="$url($codename/post)?threadid=$threadID&action=reply&page=$PageNumber"><span><img src="$skin/images/theme/button_reply.png" alt="" /></span></a>
                                                        <a class="action-newtopic" id="action-newtopic" href="$url($codename/post)?action=thread"><span><img src="$skin/images/theme/button_post.png" alt="" /></span></a>
                                                        <!--[if $IsShowModeratorManageLink]-->
                                                        <a class="action-manage" id="action-manage1" href="#"><span><img src="$skin/images/theme/button_manage.png" alt="" /></span></a>
                                                        <!--[/if]-->
                                                    </div>
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

                            <form id="threadform" method="post" enctype="multipart/form-data" action="$_form.action">
                            <input type="hidden" name="threadids" value="$Thread.ThreadID" />
                            <div class="clearfix postcaption">
                                <h3 class="postcaption-title"><span>$thread.SubjectText</span></h3>
                                <div class="poststats">
                                    <!--[ajaxpanel id="ap_threadinfo" idonly="true"]-->
                                    <ul class="clearfix poststats-list">
                                        <li><span class="label">阅读</span> <span class="numeric">$thread.TotalViews</span></li>
                                        <li><span class="label">回复</span> <span class="numeric">$thread.TotalReplies</span></li>
                                    </ul>
                                    <!--[/ajaxpanel]-->
                                </div>
                            </div>
                            <div class="postallentry posttype-normal">
                                <!--[ajaxpanel id="ap_postlist" idonly="true"]-->
                                <!--[if $IsShowThreadContent && $ThreadContent!=null]-->
                                <div class="postmajorentry">
                                <!--[ajaxpanel id="ap_threadcontent" idonly="true"]-->
                                    <div class="postwrap" id="postid_$threadContent.PostID">
                                        <table class="posttable" <!--[if $threadContent.PostID == $LastPostID]-->id="last"<!--[/if]-->>
                                            <tr>
                                                <td class="postauthor" rowspan="2">
                                                    <!-- #include file="_postauthorinfo.aspx" post="$threadContent" -->
                                                </td>
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
                                                            <!--[if $IsShowPostIndexAlias]-->
                                                            <p class="post-number">$threadContent.PostIndexAlias</p>
                                                            <!--[/if]-->
                                                            <p class="textresize" onmouseover="this.className+=' textresize-expand';" onmouseout="this.className=this.className.replace('textresize-expand','');">
                                                                <a class="label" href="javascript:void(0);"><span><em>.</em>字号</span></a>
                                                                <span class="textresize-list">
                                                                    <a href="javascript:void(0);" class="small" onclick="PostViewStyle(this, 'fontsize-small');return false;">较小字号</a>
                                                                    <a href="javascript:void(0);" class="medium" onclick="PostViewStyle(this, 'fontsize-medium');return false;">正常字号</a>
                                                                    <a href="javascript:void(0);" class="large" onclick="PostViewStyle(this, 'fontsize-large');return false;">较大字号</a>
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
                                                        <div class="clearfix post-content">
                                                            $Highlight($threadcontent.ContentText)
                                                        </div>
                                                        
                                                        <!--[if $HasBuyed]-->
                                                        <!--[if $threadcontent.LastEditorID > 0]-->
                                                        <div class="post-lastmodify">
                                                            本帖最后由 $threadcontent.LastEditor 于 $outputFriendlyDatetime($threadcontent.UpdateDate) 编辑
                                                        </div>
                                                        <!--[/if]-->
                                                        
                                                        <%--
                                                        <div class="clearfix post-topicnav">
                                                            <p class="topicnav-previous">上一主题: <a href="">上一主题...</a></p>
                                                            <p class="topicnav-next">下一主题: <a href="">下一主题...</a></p>
                                                        </div>
                                                        --%>
                                                         
                                                        <!-- #include file="_postattachment.aspx" post="$threadContent" -->
                                                        <!-- #include file="_postscore.aspx" post="$threadContent" -->
                                                        
                                                        <!--[/if]-->
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
                                                        <!--[if $HasSignatureAd]-->
                                                        <div class="clearfix ad-banner-postsignature">
                                                            $SignatureAd
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
                                                <td class="postauthor-tools">
                                                    <!--[if $my.ismanager]-->
                                                    <div class="clearfix postauthor-tools-inner">
                                                        <div class="post-tools">
                                                            <ul class="post-tools-list">
                                                                <li><a class="action-usermanage" id="manageUserLink_$threadcontent.UserID" href="javascript:void(0);" onclick="return openUserMenu(this,$threadcontent.UserID)"><span><em>.</em>管理用户</span></a></li>
                                                            </ul>
                                                        </div>
                                                    </div>
                                                    <!--[else]-->
                                                    &nbsp;
                                                    <!--[/if]-->
                                                </td>
                                                <td class="postmain-tools">
                                                    <div class="clearfix postmain-tools-inner">
                                                        <!-- #include file="_postaction.aspx" post="$threadContent" -->
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                 <!--[/ajaxpanel]-->
                                    <!--[if $HasPostLeaderboardAD]-->
                                    <div class="ad-banner-postdivide">
                                        $PostLeaderboardAD
                                    </div>
                                    <!--[/if]-->
                                    
                                </div>
                                <!--[/if]-->
                                
                                <!--[if $postlist.count > 0]-->
                                <div class="postreplies">
                                    <!--[if !$IsShowThreadContent]-->
                                    
                                    <!--[/if]-->
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
