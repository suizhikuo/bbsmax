<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
</head>
<body>
<div class="container section-friend section-friend-invite">
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
                                        <li><a class="current" href="$url(my/friends-invite)"><span>邀请好友</span></a></li>
                                        <li><a href="$url(my/blacklist)"><span>黑名单</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace">
                                <div class="workspace-content">
                                    <div class="workspace-content-inner">

                                    <!--[if $My.NeedInputInviteSerial]-->
                                        <div class="page-message">
                                            您注册时没有输入邀请码, 可能会有一些功能上的限制.<br />
                                            请在下面输入您的邀请码, 以取消这些限制.
                                        </div>
                                        <form action="$_form.action" method="post">
                                        <div class="formgroup inviteform">
                                            <div class="formrow">
                                                <h3 class="label"><label for="myInviteSerial">输入邀请码</label></h3>
                                                <div class="form-enter">
                                                    <input id="myInviteSerial" name="MyInviteSerial" type="text" class="text" value="$_form.text('MyInviteSerial','')" />
                                                </div>
                                                <!--[unnamederror form="setSerial"]-->
                                                <div class="form-tip tip-error">$message</div>
                                                <!--[/unnamederror]-->
                                            </div>
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input value="确定" type="submit" name="setSerial" class="button" /></span></span>
                                            </div>
                                        </div>
                                        </form>
                                    <!--[/if]-->

                                    <!--[if $InviteSerialMode]-->
                                        <!--[if $CanBuySerial]-->
                                        <!--[unnamederror]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <!--[success]-->
                                        <div class="successmsg">邀请码购买成功</div>
                                        <!--[/success]-->
                                        <form action="$_Form.Action" method="post">
                                        <div class="formgroup inviteform">
                                            <div class="formrow">
                                                <h3 class="label"><label for="">购买邀请码</label></h3>
                                                <div class="form-enter">
                                                    <input type="text" name="buyCount" class="text number" value="$_form.text('buyCount','')" /> 个
                                                </div>
                                                <div class="form-note">每个邀请码所需积分: $SerialPrice</div>
                                                <div class="form-note">邀请码购买数量限制: $BuyInterval<!--[if $CanBuyCount>0]-->, 当前时间段您还可以购买 $CanBuyCount 个邀请码!<!--[/if]--></div>
                                            </div>
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" name="buySerial" value="购买" /></span></span>
                                            </div>
                                        </div>
                                        </form>
                                        <!--[/if]-->
                                        
                                        <div class="infomsg">
                                            <p>复制未使用的邀请码给你的朋友, 或者通过Email发给他们, 邀请他们加入该网站.</p>
                                            <!--[if $ShowInvitePoint]-->
                                            <p>每次成功邀请一个好友, $InvitePointValue!</p>
                                            <!--[/if]-->
                                        </div>
                                        <form action="$_form.Action" method="post">
                                        <div class="invitecodetable">
                                            <table>
                                                <thead>
                                                    <tr>
                                                        <td>
                                                            <select onchange="location.replace('$url(my/friends-invite)?status='+this.value)">
                                                                <option value="used" $_if(""+$Status=="Used", "selected=\"selected\"")>已使用</option>
                                                                <option value="unused" $_if(""+$Status=="Unused", "selected=\"selected\"")>未使用</option>
                                                                <option value="expires" $_if(""+$Status=="Expires", "selected=\"selected\"")>已过期</option>
                                                                <option value="all" $_if(""+$Status=="All", "selected=\"selected\"")>全部</option>
                                                            </select>
                                                        </td>
                                                        <td class="code">邀请码</td>
                                                        <!--[if $_get.status == "all" || $_get.status == null || $_get.status == "used"]-->
                                                        <td class="user">受邀请用户</td>
                                                        <!--[/if]-->
                                                        <td class="date">生成时间</td>
                                                        <td class="date">过期时间</td>
                                                        <td class="send">&nbsp;</td>
                                                    </tr>
                                                </thead>
                                                <!--[if $TotalCount > 0]-->
                                                <tbody>
                                                    <!--[loop $sl in $SerialList]-->
                                                    <tr>
                                                        <td><!--[if $sl.IsExpires]-->已过期<!--[else if $sl.Used]-->已使用<!--[else if $sl.Canuse]-->未使用<!--[/if]--></td>
                                                        <td class="code"><code>$sl.Serial</code></td>
                                                        <!--[if $_get.status == "all" || $_get.status == null || $_get.status == "used"]-->
                                                        <td class="user"><!--[if $sl.Used]-->$sl.Touser.namelink<!--[/if]--></td>
                                                        <!--[/if]-->
                                                        <td class="date">$outputdate($sl.CreateDate)</td>
                                                        <td class="date">$outputdate($sl.ExpiresDate)</td>
                                                        <td class="send"><!--[if $sl.CanUse]--><a onclick="return openDialog(this.href)" href="$dialog/user-sendinviteemail.aspx?s=$sl.Serial">通过Email发送</a><!--[/if]--></td>
                                                    </tr>
                                                    <!--[/loop]-->
                                                </tbody>
                                                <!--[else]-->
                                                <tbody>
                                                    <tr>
                                                        <td <!--[if $_get.status == "all" || $_get.status == null || $_get.status == "used"]-->colspan="6"<!--[else]-->colspan="5"<!--[/if]-->>
                                                            <div class="nodata">没有$_if(""+$Status=="Used", "已使用的")$_if(""+$Status=="Unused", "未使用的")$_if(""+$Status=="Expires", "已过期的")邀请码.</div>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                                <!--[/if]-->
                                            </table>
                                        </div>
                                        <!-- 分页 -->
                                        <!--[pager name="pager1" skin="../_inc/_pager_app.aspx"]-->
                                        
                                        </form>
                                    <!--[else]-->
                                        <!--[unnamederror]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <!--[success]-->
                                        <div class="successmsg">恭喜您, 邀请邮件全部发送成功!</div>
                                        <!--[/success]-->
                                        <div class="formgroup inviteform">
                                            <div class="formrow">
                                                <h3 class="label">您的邀请码</h3>
                                                <div class="form-enter">
                                                    <code>$MyInviteCode</code>
                                                </div>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label">邀请地址</h3>
                                                <div class="form-enter">
                                                    <a href="$SiteRoot$url(Register/$MyInviteCode)" target="_blank">$SiteRoot$url(Register/$MyInviteCode)</a>
                                                </div>
                                                <div class="form-note">
                                                    您可以通过QQ, MSN等IM工具, 或者发送邮件, 把链接告诉你的好友, 邀请他们加入.
                                                </div>
                                                <!--[if $ShowInvitePoint]-->
                                                <div class="form-note">
                                                    每次成功邀请一个好友, $InvitePointValue!
                                                </div>
                                                <!--[/if]-->
                                            </div>
                                            <!--[if $CanSendEmail]-->
                                            <form method="post" action="$_form.action">
                                            <div class="formrow">
                                                <h3 class="label"><label for="emails">好友Email</label></h3>
                                                <div class="form-enter">
                                                    <textarea name="emails" id="emails" cols="50" rows="5"></textarea>
                                                </div>
                                                <div class="form-note">每行填写一个Email地址.</div>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label"><label for="message">附言</label></h3>
                                                <div class="form-enter">
                                                    <textarea id="message" name="message" cols="50" rows="5" onkeyup="$('obiter').innerText=$('message').value">$_form.text("message","")</textarea>
                                                </div>
                                            </div>
                                            <div class="formrow">
                                                <h3 class="label">预览</h3>
                                                <div class="form-enter invitepreview">
                                                    <div class="invitepreview-inner">$InviteEmailContent</div>
                                                </div>
                                            </div>
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="emailinvite" value="发送邀请" class="button" /></span></span>
                                            </div>
                                            </form>
                                            <!--[/if]-->
                                        </div>
                                    <!--[/if]-->
                                        
                                        <!--[if $Inviter != null]-->
                                        <div class="panel inviteuser">
                                            <div class="panel-head">
                                                <h3 class="panel-title">我的邀请人</h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="inviteuserlist">
                                                    <ul class="clearfix inviteuser-list">
                                                        <li>
                                                            <a class="avatar" href="$url(space/$Inviter.id)"><img src="$Inviter.avatarpath" alt="" width="24" height="24" /></a>
                                                            <a class="fn" href="$url(space/$Inviter.id)">$Inviter.name</a>
                                                        </li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                        <!--[if $Invitees.Count > 0]-->
                                        <div class="panel inviteuser">
                                            <div class="panel-head">
                                                <h3 class="panel-title">我邀请的人</h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="inviteuserlist">
                                                    <ul class="clearfix inviteuser-list">
                                                        <!--[loop $i in $Invitees]-->
                                                        <li>
                                                            <a class="avatar" href="$url(space/$I.id)"><img src="$I.avatarpath" alt="" width="24" height="24" /></a>
                                                            <a class="fn" href="$url(space/$I.id)">$i.name</a>
                                                        </li>
                                                        <!--[/loop]-->
                                                    </ul>
                                                </div>
                                                <!-- 分页 -->
                                                  <!--[pager name="pager2" skin="../_inc/_pager_app.aspx"]-->
                                            </div>
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
