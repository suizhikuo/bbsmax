<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_spacehtmlhead.aspx"-->
</head>
<body>
<div class="container section-space section-space-visitor">
<!--#include file="_spacethemetip.aspx"-->
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <!--#include file="../_inc/_round_top.aspx"-->
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title">
                                    <span style="background-image:url($Root/max-assets/icon/friend.gif);">
                                    <!--[if $IsViewAll]-->
                                    $SpaceOwner.Name的好友
                                    <!--[else if $IsViewVisitor]-->
                                    $SpaceOwner.Name的最近访客
                                    <!--[else]-->
                                    $SpaceOwner.Name最近访问过的
                                    <!--[/if]-->
                                    </span>
                                </h3>
                            </div>

                            <div class="filtertab">
                                <ul class="clearfix tab-list">
	                                <li><a $_if($IsViewAll, 'class="current"') href="$url(space/friend-list)?uid=$SpaceOwnerID">$SpaceOwner.Name的好友</a></li>
	                                <li><a $_if($IsViewVisitor, 'class="current"') href="$url(space/friend-list)?uid=$SpaceOwnerID&view=visitor">$SpaceOwner.Name的访客</a></li>
	                                <li><a $_if($IsViewTrace, 'class="current"') href="$url(space/friend-list)?uid=$SpaceOwnerID&view=trace">$SpaceOwner.Name的足迹</a></li>
                                </ul>
                            </div>
     
                        <!--[if $IsViewAll]-->
                            <!--[if $FriendList.Count == 0]-->
                            <div class="nodata">
                                $SpaceOwner.Name暂时没有好友
                            </div>
                            <!--[else]-->
                            <div class="spacevisitorlist">
                                <ul class="clearfix spacevisitor-list">
                                    <!--[loop $Friend in $FriendList]-->
                                    <li class="clearfix visitoritem">
                                        <%--<div class="action"><a class="action-delete" href="#">删除</a></div>--%>
                                        <div class="avatar"><a href="$url(space/$Friend.User.id)"><img src="$Friend.User.AvatarPath" alt="" width="48" height="48" /></a></div>
                                        <div class="name"><a class="fn" href="$url(space/$friend.User.id)">$friend.User.Name</a></div>
                                        <div class="date">$OutputFriendlyDate($friend.CreateDate)</div>
                                        <div class="operate">
                                            <!--[if $friend.user.UserID != $myUserID]-->
                                            <span class="chat">
                                                <!--[if $friend.user.isonline]-->
                                                <a class="chat-online" href="$root/max-dialogs/chat.aspx?to=$friend.User.UserID" onclick="return openDialog(this.href);">对话</a>
                                                <!--[else]-->
                                                <a class="chat-offline" href="$root/max-dialogs/chat.aspx?to=$friend.User.UserID" onclick="return openDialog(this.href);">对话</a>
                                                <!--[/if]-->
                                            </span>
                                            <!--[/if]-->
                                            <!--[if $IsShowAddFriendLink($friend.User.UserID)]-->
                                            <a class="addfriend" href="$dialog/friend-tryadd.aspx?uid=$friend.User.UserID" onclick="return openDialog(this.href, function(result){})">
                                                加为好友
                                            </a>
                                            <!--[/if]-->
                                        </div>
                                    </li>
                                    <!--[/loop]-->
                                </ul>
                            </div>
                            <!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
                            <!--[/if]-->
                        <!--[else if IsViewVisitor]-->
                            <!--[if $VisitorList.Count == 0]-->
                            <div class="nodata">
                                最近一段时间内没有人访问过$SpaceOwner.Name的空间。
                            </div>
                            <!--[else]-->
                            <div class="spacevisitorlist">
                                <ul class="clearfix spacevisitor-list">
                                    <!--[loop $Visitor in $VisitorList]-->
                                    <li class="clearfix visitoritem">
                                        <%--<div class="action"><a class="action-delete" href="#">删除</a></div>--%>
                                        <div class="avatar"><a href="$url(space/$Visitor.User.id)"><img src="$Visitor.User.AvatarPath" alt="" width="48" height="48" /></a></div>
                                        <div class="name"><a class="fn" href="$url(space/$Visitor.User.id)">$Visitor.User.Name</a></div>
                                        <div class="date">$OutputFriendlyDate($Visitor.CreateDate)</div>
                                        <div class="operate">
                                            <!--[if $Visitor.user.UserID != $myUserID]-->
                                            <span class="chat">
                                                <!--[if $Visitor.user.isonline]-->
                                                <a class="chat-online" href="$root/max-dialogs/chat.aspx?to=$Visitor.User.UserID" onclick="return openDialog(this.href);">对话</a>
                                                <!--[else]-->
                                                <a class="chat-offline" href="$root/max-dialogs/chat.aspx?to=$Visitor.User.UserID" onclick="return openDialog(this.href);">对话</a>
                                                <!--[/if]-->
                                            </span>
                                            <!--[/if]-->
                                            <!--[if $IsShowAddFriendLink($Visitor.User.UserID)]-->
                                            <a class="addfriend" href="$dialog/friend-tryadd.aspx?uid=$Visitor.User.UserID" onclick="return openDialog(this.href, function(result){})">
                                                加为好友
                                            </a>
                                            <!--[/if]-->
                                        </div>
                                    </li>
                                    <!--[/loop]-->
                                </ul>
                            </div>
                            <!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
                            <!--[/if]-->
                        <!--[else if $IsViewTrace]-->
                            <!--[if $traceList.Count == 0]-->
                            <div class="nodata">
                                $SpaceOwner.Name还没有访问过其他用户的空间.
                            </div>
                            <!--[else]-->
                            <div class="spacevisitorlist">
                                <ul class="clearfix spacevisitor-list">
                                    <!--[loop $Visitor in $traceList]-->
                                    <li class="clearfix visitoritem">
                                        <%--<div class="action"><a class="action-delete" href="#">删除</a></div>--%>
                                        <div class="avatar"><a href="$url(space/$Visitor.TargetUser.id)"><img src="$Visitor.TargetUser.AvatarPath" alt="" width="48" height="48" /></a></div>
                                        <div class="name"><a class="fn" href="$url(space/$Visitor.TargetUser.id)">$Visitor.TargetUser.Name</a></div>
                                        <div class="date">$OutputFriendlyDate($Visitor.CreateDate)</div>
                                        <div class="operate">
                                            <!--[if $Visitor.TargetUser.UserID != $myUserID]-->
                                            <span class="chat">
                                                <!--[if $Visitor.TargetUser.isonline]-->
                                                <a class="chat-online" href="$root/max-dialogs/chat.aspx?to=$Visitor.TargetUser.UserID" onclick="return openDialog(this.href);">对话</a>
                                                <!--[else]-->
                                                <a class="chat-offline" href="$root/max-dialogs/chat.aspx?to=$Visitor.TargetUser.UserID" onclick="return openDialog(this.href);">对话</a>
                                                <!--[/if]-->
                                            </span>
                                            <!--[/if]-->
                                            <!--[if $IsShowAddFriendLink($Visitor.TargetUser.UserID)]-->
                                            <a class="addfriend" href="$dialog/friend-tryadd.aspx?uid=$Visitor.TargetUser.UserID" onclick="return openDialog(this.href, function(result){})">
                                                加为好友
                                            </a>
                                            <!--[/if]-->
                                        </div>
                                    </li>
                                    <!--[/loop]-->
                                </ul>
                            </div>
                            <!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
                            <!--[/if]-->
                        <!--[/if]-->
                            
                        </div>
                    </div>
                    
                </div>
            </div>
            <!--#include file="_spacesidebar.aspx"-->
        </div>
        <!--#include file="../_inc/_round_bottom.aspx"-->
    </div>
    <!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>

