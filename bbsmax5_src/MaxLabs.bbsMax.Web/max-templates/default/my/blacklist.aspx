<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
</head>
<body>
<div class="container section-friend section-friend-blacklist">
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
                                        <li><a href="$url(my/friends-impression)"><span>好友印象</span></a></li>                                        
                                        <!--[if $EnableInvitation]-->
                                        <li><a href="$url(my/friends-invite)"><span>邀请好友</span></a></li>
                                        <!--[/if]-->
                                        <li><a class="current" href="$url(my/blacklist)"><span>黑名单</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <!--[if $blacklist.count > 0]-->
                                        <div class="panel friendlist">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>黑名单 <em class="counts">($blacklist.count)</em></span></h3>
                                                <!--[if $blacklist.count > 1]-->
                                                <div class="friendfind">
                                                    <div class="friendfind-input">
                                                        <input type="text" class="text" value="搜索黑名单..." onfocus="this.value='';" />
                                                        <input type="button" class="button button-search" value="" title="" />
                                                        <input type="button" class="button button-reset" value="" title="" style="display:none;" />
                                                    </div>
                                                    <%--
                                                    <div class="friendfind-suggest" style="display:none;">
                                                        <ul class="suggest-list">
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                        </ul>
                                                    </div>
                                                    --%>
                                                </div>
                                                <!--[/if]-->
                                            </div>
                                            <div class="panel-body">
                                                
                                                <div class="friend-list">
                                                    <ul class="clearfix friend-list-inner">
                                                        <!--[loop $blacklistItem in $Blacklist]-->
                                                        <li class="frienditem" id="friendid_$blacklistItem.UserID">
                                                            <div class="vcard clearfix friend-entry">
                                                                <div class="avatar">
                                                                    <a href="$url(space/$blacklistItem.UserID)" target="_blank"><img class="pohto" src="$blacklistItem.User.AvatarPath" alt="" width="48" height="48" /></a>
                                                                </div>
                                                                <div class="friend-content">
                                                                    <div class="name">
                                                                        <a class="fn url" href="$url(space/$blacklistItem.UserID)" target="_blank">$blacklistItem.User.Name</a>
                                                                    </div>
                                                                    <div class="operate">
                                                                        <a href="$dialog/blacklist-remove.aspx?uid=$blacklistItem.UserID" onclick="return openDialog(this.href,this, refresh)">解除黑名单</a>
                                                                    </div>
                                                                    <div class="friend-status">$blacklistItem.User.doing</div>
                                                                </div>
                                                                <div class="entry-action">
                                                                    <a class="action-delete" href="$dialog/blacklist-remove.aspx?uid=$blacklistItem.UserID" onclick="return openDialog(this.href,this, refresh)" title="解除黑名单">解除黑名单</a>
                                                                </div>
                                                            </div>
                                                        </li>
                                                        <!--[/loop]-->
                                                    </ul>
                                                </div>
                                                <!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
                                                
                                            </div>
                                        </div>
                                        <!--[else]-->
                                        <div class="nodata">
                                            你现在没有将任何用户加入黑名单.
                                        </div>
                                        <!--[/if]-->
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        
                                        <form method="post" name="blackform" action="$_form.action">
                                        <div class="panel round-tl addblacklist">
                                            <div class="panel-body round-tr">
                                                <div class="round-bl"><div class="clearfix round-br">
                                                
                                                    <!--[success]-->
                                                    <div class="successmsg">你已成功将 "$UsernameToAdd" 加入黑名单.</div>
                                                    <!--[/success]-->
                                                    <!--[unnamederror]-->
                                                    <div class="errormsg">$message</div>
                                                    <!--[/unnamederror]-->
                                                    <div class="formgroup appform">
                                                        <div class="formrow">
                                                            <h3 class="label"><label for="username">输入用户名</label></h3>
                                                            <div class="form-enter">
                                                                <input type="text" class="text" name="username" id="username" value="" />
                                                            </div>
                                                            <p class="form-note">
                                                                被加入黑名单的用户将会从你的好友中删除。
                                                                同时, 该黑名单用户将被禁止某些与你相关的互动行为。
                                                            </p>
                                                        </div>
                                                        <div class="formrow formrow-action">
                                                            <span class="minbtn-wrap"><span class="btn"><input type="submit" name="add" id="moodsubmit_btn" value="添加" class="button" /></span></span>
                                                        </div>
                                                    </div>
                                                
                                                </div></div>
                                            </div>
                                        </div>
                                        </form>
                                        
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
