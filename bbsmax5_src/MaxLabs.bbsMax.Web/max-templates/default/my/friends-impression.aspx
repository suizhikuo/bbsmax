<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
</head>
<body>
<div class="container section-friend section-friend-impression">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/friend.gif);">好友</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a href="$url(my/friends)"><span>好友</span></a></li>
                                        <li><a class="current" href="$url(my/friends-impression)"><span>好友印象</span></a></li>
                                        <!--[if $EnableInvitation]-->
                                        <li><a href="$url(my/friends-invite)"><span>邀请好友</span></a></li>
                                        <!--[/if]-->
                                        <li><a href="$url(my/blacklist)"><span>黑名单</span></a></li>
                                    </ul>
                                </div>
                            </div>
                            
                            <div class="clearfix workspace">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <!--[if $friendlist.count != 0]-->
                                        <div class="impressionlist">
                                            <ul class="impression-list">
                                                <!--[loop $friend in $friendlist]-->
                                                <li class="clearfix vcard impressionitem">
                                                    <div class="avatar">
                                                        $friend.User.AvatarLink
                                                    </div>
                                                    <div class="name">
                                                        <a class="fn url" href="$url(space/$friend.user.userid)" target="_blank">$friend.user.Name</a>
                                                        <span class="chat">
                                                            <a class="<!--[if $friend.user.IsOnline]-->chat-online<!--[else]-->chat-offline<!--[/if]-->" href="$dialog/chat.aspx?to=$friend.UserID" onclick="return openDialog(this.href)">发消息</a>
                                                        </span>
                                                        <a class="writeimpression" href="$dialog/user-impressions.aspx?uid=$friend.user.userid"  onclick="openDialog(this.href);return false;">撰写好友印象</a>
                                                    </div>
                                                    <div class="impression-word">
                                                        <!--[if $GetImpressionList($friend.User.UserID) != ""]-->
                                                        $GetImpressionList($friend.User.UserID)
                                                        <!--[else]-->
                                                        还没有人描述过 $friend.user.Name.
                                                        <!--[/if]-->
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        
                                        <!--[pager name="pager1" skin="../_inc/_pager_app.aspx"]-->
                                        
                                        <!--[else]-->
                                        <div class="nodata">
                                            你现在还没有任何好友, 可以到<a href="$url(members)?view=search" target="_blank">"搜索好友"</a>找到感兴趣的朋友, 或者<a href="$url(my/friends-invite)">"邀请"</a>认识的人加入.
                                        </div>
                                        <!--[/if]-->
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        &nbsp;
                                    </div>
                                </div>
                            </div>

                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
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
