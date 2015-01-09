<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
<script type="text/javascript" src="$root/max-assets/javascript/max-showflash.js"></script>
<script type="text/javascript">
function textCounter(target, display, maxcount) {
    var a = maxcount - target.value.length;
    if (a < 0) {
        target.value = target.value.substr(0, maxcount);
        a = 0;
    }
    document.getElementById(display).innerHTML = a;
}

function clickProxy(targetid){
    $('proxy_'+targetid).style.display = 'none';
    $('comment_box_'+targetid).style.display = '';
    $('textbox_'+targetid).focus();
}
function CommentBoxLostFocus(targetid) {
    if($('hasvcode_'+targetid))
        return;
    if ($('textbox_' + targetid).value == '') {
        $('proxy_' + targetid).style.display = '';
        $('comment_box_' + targetid).style.display = 'none';
    }
}
function ReplyComment(userid,username,targetid,commentid){
    $('proxy_'+targetid).style.display = 'none';
    $('comment_box_'+targetid).style.display = '';
    var textbox = $('textbox_'+targetid);
    textbox.focus();
    textbox.value='回复'+username+'：';
    $('replyuserid').value = userid;
    $('replycommentid').value = commentid;
}
function expandReply(targetid){
    var commentDiv = $('comment_div_'+targetid);
    var commentBox = $('comment_box_'+targetid);
    var button = $('comment_button_' + targetid);
    if(commentDiv.style.display==''){
        commentDiv.style.display = 'none';
        commentBox.style.display = 'none';
        button.innerHTML = '回复';
    }
    else{
        commentDiv.style.display = '';
        commentBox.style.display = '';
        $('proxy_' + targetid).style.display = 'none';
        button.innerHTML = '收起回复';
    }
}
function submitComment(targetid){
    ajaxSubmit('form_'+targetid, 'addfeedcomment', 'sp_comments_' + targetid + ',sp_vcode_' + targetid, function(result){ 
        if(result == null || result.iserror == null || result.iserror == false){
            $('textbox_'+targetid).value = '';
            CommentBoxLostFocus(targetid); 
            if(result != null && result.iswarning)
                showAlert(result.message);
        }
    }, null, true);
}
</script>
</head>
<body>
<div class="container section-center">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar rightsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            
                            <div class="clearfix quickpublishform">
                                <div class="useravatar">
                                    <a href="$url(my/avatar)" title="更改头像">
                                        <img src="$my.avatarpath" alt="" width="48" height="48" />
                                        <span class="avatar-change">更改头像</span>
                                    </a>
                                </div>
                                <div class="quickpublishform-enter">
                                    <div class="bubble-nw"><div class="bubble-ne"><div class="bubble-n"><div class="bubble-pointer">&nbsp;</div></div></div></div>
                                    <div class="bubble-e"><div class="clearfix bubble-w">
                                    
                                    <div class="quickstatusform" id="doing_form">
                                        <form action="$_form.action" method="post">
                                        <!--[unnamederror]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <div class="statusform-textarea">
                                            <textarea cols="70" rows="2" id="message" name="content" onkeyup="textCounter(this, 'maxlimit', $MaxDoingLength)"></textarea>
                                        </div>
                                        <div class="clearfix statusform-action">
                                            <div class="quickpublishform-change">
                                                <a href="#" onclick="$('share_form').style.display='';  $('doing_form').style.display='none'; return false;" style="background-image:url($root/max-assets/icon/share.gif)">分享</a>
                                            </div>
                                            <div class="statusform-submit">
                                                <span class="statusform-tip">
                                                    还可输入<strong class="numeric" id="maxlimit">$MaxDoingLength</strong>个字符
                                                </span>
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" id="adddoing" name="adddoing" class="button" value="发布" /></span></span>
                                            </div>
                                        </div>
                                        </form>
                                    </div>
                                    
                                    <div class="quickshareform" id="share_form" style="display:none">
                                        <div class="formrow">
                                            <h3 class="label"><label for="shareurl">输入网页, 视频, 音乐的网址</label></h3>
                                            <div class="form-enter">
                                                <input class="text url" id="shareurl" type="text" value="http://" />
                                            </div>
                                        </div>
                                        <div class="clearfix quickshareform-action">
                                            <div class="quickpublishform-change">
                                                <a href="#" onclick="$('share_form').style.display='none';  $('doing_form').style.display=''; return false;" style="background-image:url($root/max-assets/icon/doing.gif)">记录</a>
                                            </div>
                                            <div class="quickshareform-submit">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" id="createshare" name="Publish" value="分享" class="button" onclick="openDialog('$root/max-dialogs/share-create.aspx?sharetype=url&url=' + $('shareurl').value,function(r){ $('shareurl').value='http://'; })" /></span></span>
                                            </div>
                                        </div>
                                    </div>
                                    
                                    </div></div>
                                    <div class="bubble-sw"><div class="bubble-se"><div class="bubble-s">&nbsp;</div></div></div>
                                </div>
                            </div>
                            
                            <div class="panel activities">
                                <div class="panel-head">
                                    <div class="panel-tab">
                                        <ul class="clearfix">
                                        <li><a href="$url(my/default)?feedtype=1" $SelectedFeed(FeedType.FriendFeed,'class="current"')>好友动态</a></li>
                                        <li><a href="$url(my/default)?feedtype=0" $SelectedFeed(FeedType.AllUserFeed,'class="current"')>全站动态</a></li>
                                        <li><a href="$url(my/default)?feedtype=2" $SelectedFeed(FeedType.MyFeed,'class="current"')>自己的动态</a></li>
                                        </ul>
                                    </div>
                                </div>
                                <div class="panel-body">
                                <!--[ajaxpanel id="ap_feeds" idonly="true"]-->  
                                    <!--[if $feedList.Count != 0]-->
                                    <div class="activitieslist">
                                        <ul class="activities-list">
                                            <!--[loop $feed in $feedList]-->
                                            <li class="clearfix activityitem" id="feedid_{=$feed.ID}">
                                                <div class="activity-type">
                                                    <a href="$AttachQueryString("appid="+$Feed.AppID.ToString()+"&actiontype="+$Feed.ActionType)" title="只看此类动态"><img src="$GetAppActionIconUrl($Feed.AppID,$Feed.ActionType)" alt="" /></a>
                                                </div>
                                                <!--#include file="../_inc/_feedcontent.aspx"-->
                                                <!--[if $CanShieldFeed($feed) || $CanDeleteFeed || $CanDeleteAnyFeed]-->
                                                <div class="activity-actions">
                                                    <!--[if $CanShieldFeed($feed)]-->
                                                    <a class="action-banned" href="$dialog/feed-shield.aspx?frienduserid=$GetShieldUserID($feed)&appID=$Feed.AppID.ToString()&actiontype=$Feed.ActionType" onclick="return openDialog(this.href, this,refresh)" title="屏蔽">屏蔽</a>
                                                    <!--[/if]-->
                                                    <!--[if $CanDeleteFeed]-->
                                                    <a class="action-delete" href="$dialog/feed-delete.aspx?feedid=$Feed.ID&uid=$myuserid" onclick="return openDialog(this.href,function(r){ delElement($('feedid_$feed.ID')); });" title="删除">删除</a>
                                                    <!--[/if]-->
                                                    <!--[if $CanDeleteAnyFeed]-->
                                                    <a class="action-delete" href="$dialog/feed-delete.aspx?feedid=$Feed.ID&feedtype=0" onclick="return openDialog(this.href,function(r){ delElement($('feedid_$feed.ID')); });" title="删除">删除</a>
                                                    <!--[/if]-->
                                                </div>
                                                <!--[/if]-->
                                            </li>
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                        <!--[if $haveMoreFeeds]-->
                                        <div class="ajaxloading" style="display:none" id="ajaxsending">
                                            <span>加载中...</span>
                                        </div>
                                        <div class="showallactivities" id="getmorediv">
                                            <a href="#" onclick="$('ajaxsending').style.display='';$('getmorediv').style.display='none';ajaxRender('$AttachQueryString("feedCount="+$NextFeedCount)','ap_feeds',ajaxCallback);return false;">更多动态</a>
                                        </div>
                                        <!--[/if]-->
                                    <!--[else]-->
                                    <div class="nodata">
                                        当前没有任何动态.
                                    </div>
                                    <!--[/if]-->
                                <!--[/ajaxpanel]-->
                                </div>
                            </div>
                            
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                    <div class="content-sub">
                        <div class="content-sub-inner">
                            <!--#include file="../_inc/_round_top.aspx"-->
                            <div class="sidebar">
                                <!--[if $my.TotalUnreadNotifies>0]-->
                                <div class="panel noticelist">
                                    <div class="panel-head">
                                        <h3 class="panel-title"><span>待处理</span></h3>
                                    </div>
                                    <div class="panel-body">
                                        <ul class="notice-list">
                                            <!--[if $my.SystemNotifys.count > 0]-->
                                            <li><img alt="" width="16" height="16" src="$Root/max-assets/icon/megaphone.gif" /><a href="$url(my/notify)?type=all">你有 <strong class="numeric">$my.SystemNotifys.count</strong> 条系统通知未读.</a></li>
                                            <!--[/if]-->
                                            <!--[loop $t in $NotifyTypeList]-->
                                            <!--[if $my.UnreadNotify[$t.TypeID] > 0]-->
                                            <li><img alt="" width="16" height="16" src="$Root/max-assets/icon/friend.gif" /><a href="$url(my/notify)?type=$t.typeid">你有 <strong class="numeric">$my.unreadnotify[$t.typeid]</strong> 个$t.typename未处理</a></li>
                                            <!--[/if]-->
                                            <!--[/loop]-->
                                        </ul>
                                    </div>
                                </div>
                                <!--[/if]-->
                                
                                <!--[if $EnableMissionFunction]-->
                                <!--[if $HaveNewMission == true]-->
                                <div class="panel missiondock">
                                    <div class="panel-head">
                                        <h3 class="panel-title">最新任务</h3>
                                        <%--p class="panel-toggle"><a class="delete" href="#" title="放弃">放弃</a></p--%>
                                    </div>
                                    <div class="panel-body">
                                        <div class="clearfix missionitem">
                                            <div class="mission-icon">
                                                <img src="$NewMission.iconpath" alt="" />
                                            </div>
                                            <h3 class="mission-title">
                                                <a href="$url(mission/detail)?mid=$NewMission.id">$NewMission.Name</a>
                                            </h3>
                                            <div class="mission-award">
                                                $NewMission.PrizeDescription(@"<p><span class=""label"">{0}</span> <span class=""value"">{1}{2}</span></p>","")
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <!--[/if]-->
                                <!--[/if]-->
                                
                                <div class="panel myuserinfo">
                                    <div class="panel-head">
                                        <h3 class="panel-title"><span>我的用户信息</span></h3>
                                    </div>
                                    <div class="panel-body">
                                        <ul class="myuserinfo-list">
                                            <!--[if $My.RoleTitle != ""]-->
                                            <li><span class="label">头衔</span> <span class="value">$My.RoleTitle</span></li>
                                            <!--[/if]-->
                                            <!--[if $My.OnlineLevel > 0]-->
                                            <li class="onlinelevel">
                                                <span class="label">在线等级</span>
                                                $My.GetOnlineLevelIcon(@"<img src=""{0}"" alt="""" title=""{1}"" />","$Root/max-assets/icon-star/star.gif","$Root/max-assets/icon-star/moon.gif","$Root/max-assets/icon-star/sun.gif")
                                            </li>
                                            <!--[/if]-->
                                            <!--[if $My.Birthday.Year > 1900]-->
                                            <li>
                                                <span class="label">生日</span>
                                                <span class="value">$outputdate($My.Birthday)</span>
                                            </li>
                                            <li>
                                                <span class="label">星座</span>
                                                <span class="value"><img src="$My.AtomImg" alt="$My.AtomName" /> $My.AtomName</span>
                                            </li>
                                            <!--[/if]-->
                                            <li>
                                                <span class="label">性别</span>
                                                <span class="value">
                                                <!--[if $My.Gender == Gender.Male]-->
                                                <img class="gander" src="$Root/max-assets/icon/male.gif" alt="" title="男" />
                                                <!--[else if $My.Gender == Gender.Female]-->
                                                <img class="gander" src="$Root/max-assets/icon/female.gif" alt="" title="女" />
                                                <!--[else]-->
                                                保密
                                                <!--[/if]-->
                                                </span>
                                            </li>
                                            <li><span class="label">注册时间</span> <span class="value">$OutputFriendlyDate($my.CreateDate)</span></li>
                                            <li><span class="label">论坛主题数</span> <span class="value">$my.TotalTopics</span><%-- <span class="order">排名 XXX</span>--%> </li>
                                            <li><span class="label">论坛发帖数</span> <span class="value">$my.TotalPosts</span><%--  <span class="order">排名 XXX</span>--%></li>
                                            <li><span class="label">主页访问数</span> <span class="value">$my.TotalViews</span><%--  <span class="order">排名 XXX</span>--%></li>
                                            <li><span class="label">用户组</span> <span class="value">$GetRoleNames($my.Roles,",")</span></li>
                                            <li><span class="label">总积分</span> <span class="value">$my.Points</span> <%--  <span class="order">排名 XXX</span>--%></li>
                                            <!--[loop $point in $PointList]-->
                                            <li><span class="label">$point.name</span> <span class="value">$point.value $point.icon </span></li>
                                            <!--[/loop]-->
                                            <!--[if $IsShowMedal]-->
                                            <li class="medalicon">
                                                $GetMedals(@"<a href=""{0}"" target=""_blank"">{1}</a>",@"<img src=""{0}"" alt=""{1}"" title=""{1}"" /> ",true)
                                            </li>
                                            <!--[/if]-->
                                        </ul>
                                    </div>
                                </div>
                                
                                <div class="panel visitors">
                                    <div class="panel-head">
                                        <h3 class="panel-title"><span>最近访客</span></h3>
                                        <!--[if $VisitorList.Count > 0]-->
                                        <div class="panel-more"><a href="$url(space/$MyUserID/friend)?view=visitor" target="_blank">全部</a></div>
                                        <!--[/if]-->
                                    </div>
                                    <div class="panel-body">
                                        <!--[if $VisitorList.Count == 0]-->
                                        <div class="nodata">
                                            最近一段时间内没有人访问过我的空间.
                                        </div>
                                        <!--[else]-->
                                        <div class="visitorlist">
                                            <ul class="clearfix visitor-list">
                                                <!--[loop $Visitor in $VisitorList]-->
                                                <li class="clearfix visitoritem">
                                                    <%--<div class="action"><a class="action-delete" href="#">删除</a></div>--%>
                                                    <div class="avatar"><a href="$url(space/$Visitor.User.id)"><img src="$Visitor.User.AvatarPath" alt="" width="48" height="48" /></a></div>
                                                    <div class="name"><a class="fn" href="$url(space/$Visitor.User.id)">$Visitor.User.Name</a></div>
                                                    <div class="date">$OutputFriendlyDate($Visitor.CreateDate)</div>
                                                    <div class="operate">
                                                        <span class="chat">
                                                            <!--[if $Visitor.User.IsOnline]-->
                                                            <a class="chat-online" href="$root/max-dialogs/chat.aspx?to=$Visitor.VisitorUserID" onclick="return openDialog(this.href);">对话</a>
                                                            <!--[else]-->
                                                            <a class="chat-offline" href="$root/max-dialogs/chat.aspx?to=$Visitor.VisitorUserID" onclick="return openDialog(this.href);">对话</a>
                                                            <!--[/if]-->
                                                        </span>
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                
                            </div>
                            <!--#include file="../_inc/_round_bottom.aspx"-->
                        </div>
                    </div>
                </div>
            </div>
            <!--#include file="../_inc/_sidebar_app.aspx"-->
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
