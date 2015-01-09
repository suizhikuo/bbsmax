<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
<script type="text/javascript">
function delFriend(result) {
    if (result) refresh(); //delElement($('f_' + result.targetUserID));
}
function shieldFriend (result) {
    if (result) { $('f_shield_' + result.targetUserID).innerHTML = (result.isShield ? '<span class="red">解除屏蔽</span>' : '屏蔽动态'); }
}
function moveFriend(result) {
    if (result) refresh();
}
function addBlacklist(result) {
    if (result) refresh();
}
function addGroup(result) {
    if (result) refresh();
}
function shieldGroup(result) {
    if (result) refresh();
}
function deleteGroup(result){
    if (result) refresh();
}
function renameGroup(result){
    if (result) {
        $('g_'+result.groupID).innerHTML = result.groupName;
    }
}
function OnOver(id) {
    $('f_' + id).className = 'hover';
}
function OnOut(id) {
    $('f_' + id).className = '';
}
</script>
</head>
<body>
<div class="container section-friend">
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
                                        <li><a class="current" href="$url(my/friends)"><span>好友</span></a></li>
                                        <li><a href="$url(my/friends-impression)"><span>好友印象</span></a></li>
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
                                        
                                        <form id="form1" method="post" action="$_form.action">
                                        <div class="panel friendlist">
                                            <%--
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>$GroupTitle</span></h3>
                                                <div class="friendfind">
                                                    <div class="friendfind-input">
                                                        <input type="text" class="text" value="搜索好友..." onfocus="this.value='';" />
                                                        <input type="button" class="button button-search" value="" />
                                                        <input type="button" class="button button-cancel value="" style="display:none;" />
                                                    </div>
                                                    <div class="friendfind-suggest" style="display:none;">
                                                        <ul class="suggest-list">
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                            <li><a href="#">ssss</a></li>
                                                        </ul>
                                                    </div>
                                                </div>
                                            </div>
                                            --%>
                                            <div class="panel-body">

                                                <!--[if $IsGroupShield]-->
                                                <div class="alertmsg">你已经屏蔽了本分组内所有好友的动态, <a href="$dialog/friendgroup-shield.aspx?groupid=$groupid" onclick="return openDialog(this.href)">点击此处解除</a></div>
                                                <!--[/if]-->
                                                <!--[if $friendlist.count > 0]-->
                                                <div class="friend-list">
                                                    <ul class="clearfix friend-list-inner">
                                                        <!--[loop $friend in $friendlist]-->
                                                        <li class="frienditem" id="friendid_$friend.UserID">
                                                            <div class="vcard clearfix friend-entry">
                                                                <div class="avatar">
                                                                    <a href="$url(space/$friend.UserID)" target="_blank"><img class="pohto" src="$friend.User.AvatarPath" alt="" width="48" height="48" /></a>
                                                                </div>
                                                                <div class="friend-content">
                                                                    <div class="name">
                                                                        <a class="fn url" href="$url(space/$friend.UserID)" target="_blank">$friend.user.Name</a>
                                                                        <span class="chat">
                                                                            <a class="<!--[if $friend.user.IsOnline]-->chat-online<!--[else]-->chat-offline<!--[/if]-->" href="$dialog/chat.aspx?to=$friend.UserID" onclick="return openDialog(this.href)"><!--[if $friend.user.IsOnline]-->会话<!--[else]-->发消息<!--[/if]--></a>
                                                                        </span>
                                                                    </div>
                                                                    <div class="clearfix operate">
                                                                        <a class="changegroup" href="$dialog/friend-move.aspx?uid=$friend.user.id" onclick="return openDialog(this.href, refresh)"><span>更改分组</span></a>
                                                                    </div>
                                                                    <div class="friend-status">$friend.user.doing</div>
                                                                </div>
                                                                <p class="entry-action">
                                                                    <a class="action-delete" href="$dialog/friend-delete.aspx?uid=$friend.UserID" onclick="return openDialog(this.href, refresh)" title="解除好友关系">解除好友</a>
                                                                    <!--[if $friend.IsShield]-->
                                                                    <a class="action-banned" href="$dialog/friend-shield.aspx?uid=$friend.UserID" onclick="return openDialog(this.href, refresh)" title="解除屏蔽">解除屏蔽</a>
                                                                    <!--[else]-->
                                                                    <a class="action-banned" href="$dialog/friend-shield.aspx?uid=$friend.UserID" onclick="return openDialog(this.href, refresh)" title="屏蔽动态">屏蔽动态</a>
                                                                    <!--[/if]-->
                                                                    <a class="action-backlist" href="$dialog/blacklist-add.aspx?uid=$friend.UserID" onclick="return openDialog(this.href, refresh)" title="拉入黑名单">拉入黑名单</a>
                                                                </p>
                                                            </div>
                                                        </li>
                                                        <!--[/loop]-->
                                                    </ul>
                                                </div>
                                                <!--[pager name="pager1" skin="../_inc/_pager_app.aspx"]-->
                                                <!--[else]-->
                                                <div class="nodata">
                                                    <!--[if $groupid == null]-->
                                                    你现在还没有任何好友, 可以到<a href="$url(members)?view=search" target="_blank">"搜索好友"</a>找到感兴趣的朋友, 或者<a href="$url(my/friends-invite)">"邀请"</a>认识的人加入.
                                                    <!--[else]-->
                                                    此分组还没有任何好友.
                                                    <!--[/if]-->
                                                </div>
                                                <!--[/if]-->
                                                
                                            </div>
                                        </div>
                                        </form>
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        <form id="form2" action="$_form.action" method="post">
                                        <div class="panel categorylist friendgroup">
                                            <div class="panel-head">
                                                <h3 class="panel-title"><span>好友分组</span></h3>
                                            </div>
                                            <div class="panel-body">
                                                <!--[ajaxpanel id="catlist"]-->
                                                <ul class="category-list actioncount-3">
                                                    <li class="$_if($GroupID == null,"current")">
                                                        <div class="name">
                                                            <a href="$url(my/friends)">
                                                                全部好友
                                                                <span class="counts">[$my.totalfriends]</span>
                                                            </a>
                                                        </div>
                                                    </li>
                                                    <!--[loop $Group in $FriendGroupList]-->
                                                    <li class="clearfix $_if($GroupID == $Group.ID,"current")">
                                                        <div class="name">
                                                            <a id="g_$Group.ID" href="$url(my/friends)?groupid=$Group.ID">
                                                                $Group.Name
                                                                <em class="counts">[$group.totalFriends]</em>
                                                                <!--[if $Group.IsShield]--><em class="banned">[屏蔽]</em><!--[/if]-->
                                                            </a>
                                                        </div>
                                                        <!--[if $Group.CanManage]-->
                                                        <div class="clearfix entry-action">
                                                            <a class="action-rename" href="$dialog/friendgroup-rename.aspx?groupid=$Group.ID" onclick="return openDialog(this.href, this,renameGroup)" title="重命名">重命名</a>
                                                            <a class="action-banned" href="$dialog/friendgroup-shield.aspx?groupid=$Group.ID" onclick="return openDialog(this.href,this,shieldGroup)" title="$_if($Group.IsShield,'解除屏蔽','屏蔽组动态')">$_if($Group.IsShield,'解除屏蔽','屏蔽组动态')</a>
                                                            <a class="action-delete" href="$dialog/friendgroup-delete.aspx?groupid=$Group.ID" onclick="return openDialog(this.href,deleteGroup)" title="删除分组">删除分组</a>
                                                        </div>
                                                        <!--[/if]-->
                                                    </li>
                                                    <!--[/loop]-->
                                                </ul>
                                                <!--[/ajaxpanel]-->
                                                <div class="addcategory addfriendgroup" id="addcat-box" style="display:none">
                                                <!--[ajaxpanel id="ap_error"]-->
                                                <!--[unnamederror]-->
                                                <div id="errormsg" class="errormsg">$Message</div>
                                                <script type="text/javascript">
                                                    $('addcat-box').style.display = '';
                                                </script>
                                                <!--[/unnamederror]-->
                                                <!--[/ajaxpanel]-->
                                                    <div class="formgroup">
                                                        <div class="formrow">
                                                            <div class="form-enter">
                                                                <input type="text" class="text" name="groupName" id="groupName" value="分组名称..." onfocus="if(this.value=='分组名称...'){this.value='';}this.style.color='#000';" />
                                                            </div>
                                                        </div>
                                                        <div class="formrow formrow-action">
                                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="button" name="addgroup" value="确认"onclick="ajaxSubmit('form2','addgroup','catlist,ap_error',function(r){ if(r!=null){}else{ $('groupName').value = '';$('addcat-btn').style.display=''; $('addcat-box').style.display='';}},null,true);" /></span></span>
                                                            <a href="#" onclick="$('addcat-btn').style.display = '';$('addcat-box').style.display='none'; return false;">取消</a>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="addcategory-button">
                                                    <a href="#" id="addcat-btn" onclick="$('addcat-box').style.display = '';this.style.display='none'; return false;">新建好友分组</a>
                                                </div>
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
