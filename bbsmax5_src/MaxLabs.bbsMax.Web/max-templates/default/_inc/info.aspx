<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
$AutoJumpHtml
<!--#include file="_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/account.css" />
</head>
<body>
<div class="container section-info">
<!--#include file="_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="_round_top.aspx"-->
                        <div class="clearfix content-main-inner">
                            <div class="infopanel">
                                <div class="operatemessage">
                                    <div class="operatemessage-inner message-$mode">
                                        <h3 class="message-title">$message</h3>
                                        <!--[loop $jump in $JumpLinkList]-->
                                        <p class="message-intro">
                                            <!--[if $jump.link == ""]-->
                                            $jump.Text
                                            <!--[else]-->
                                            <a href="$jump.Link">$jump.Text</a>
                                            <!--[/if]-->
                                        </p>
                                        <!--[/loop]-->
                                        <p class="message-action">
                                            <a href="javascript:history.go(-1)">&laquo;后退到上一页</a> -
                                            <a href="$url(default)">前往首页&raquo;</a>
                                        </p>
                                    </div>
                                </div>
                                
                                <!--[if $IsLogin == false]-->
                                    <!--[if $mode == "error" && $AutoJump==false ]-->
                                <div class="extramessage-wrap">
                                    <div class="extramessage">
                                        <div class="extramessage-inner">
                                            <p>你可能尚未登录或者已经安全退出.</p>
                                        </div>
                                    </div>
                                    
                                    <div class="fastloginform-wrap">
                                        <div class="formgroup fastloginform">
                                        <form id="loginform" action="$_form.action" method="post">
                                            <!--[unnamederror]-->
                                            <div class="errormsg">$message</div>
                                            <!--[/unnamederror]-->
                                            <div class="formrow">
                                                <div class="label">
                                                    <!--[if $LoginType == UserLoginType.Username]-->
                                                    <label for="username">用户名</label>
                                                    <!--[else if $LoginType == UserLoginType.Email]-->
                                                    <label for="username">邮箱</label>
                                                    <!--[else]-->
                                                    <select id="logintype" name="logintype"  tabindex="1">
                                                        <option value="username" $_form.selected("logintype","username")>账号</option>
                                                        <option value="email" $_form.selected("logintype","email")>邮箱</option>
                                                    </select>
                                                    <!--[/if]-->
                                                </div>
                                                <div class="form-enter">
                                                    <input type="text" class="text" name="username" id="all" value="$_form.text('username')" tabindex="2" />
                                                    <!--[error name="username"]-->
                                                    <span class="form-tip tip-error">$message</span>
                                                    <!--[/error]-->
                                                </div>
                                            </div>
                                            <div class="formrow formrow-password">
                                                <h3 class="label"><label for="password">密码</label></h3>
                                                <div class="form-enter">
                                                    <input type="password" class="text" name="password" id="password" tabindex="3" />
                                                    <!--[error name="password"]-->
                                                    <span class="form-tip tip-error">$message</span>
                                                    <!--[/error]-->
                                                </div>
                                            </div>
                                            <!--[validateCode actionType="$validateActionName"]-->
                                            <div class="formrow">
                                                <h3 class="label"><label for="validatecode">验证码</label></h3>
                                                <div class="form-enter">
                                                    <input type="text" class="text validcode" name="$inputName" $_if($disableIme,'style="ime-mode:disabled;"') id="validatecode" tabindex="4" autocomplete="off" />
                                                    <span class="captcha">
                                                        <img alt="" src="$imageurl" title="看不清,点击刷新" onclick="this.src=this.src+'&rnd=' + Math.random();" />
                                                    </span>
                                                    <!--[error name="$inputName"]-->
                                                    <span class="form-tip tip-error">$message</span>
                                                    <!--[/error]-->
                                                </div>
                                                <div class="form-note">$tip</div>
                                            </div>
                                            <!--[/validateCode]-->
                                            <div class="formrow">
                                                <div class="form-enter">
                                                    <p class="remember">
                                                        <input type="checkbox" id="autologin" name="autologin" value="autologin" $_form.checked('autologin', 'autologin') tabindex="5" />
                                                        <label for="autologin">记住我的登录状态</label>
                                                    </p>
                                                    <p class="invisible">
                                                        <input type="checkbox" id="invisible" name="invisible" value="true" $_form.checked('invisible', 'true') tabindex="6" />
                                                        <label for="invisible">隐身登陆</label>
                                                    </p>
                                                </div>
                                            </div>
                                            <div class="formrow formrow-action">
                                                <input type="hidden" name="returnurl" value="___form.text('returnurl', __returnurl)" />
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="login" value="登录" class="button" tabindex="7" /></span></span>
                                                <a href="$url(recoverpassword)">忘记密码?</a>
                                            </div>
                                        </form>
                                        </div>
                                    </div>
                                </div>
                                    <!--[/if]-->
                                <!--[else if $RoleDescription != ""]-->
                                <div class="extramessage-wrap">
                                    <div class="extramessage">
                                        <div class="extramessage-inner">
                                            <p>$RoleDescription</p>
                                        </div>
                                    </div>
                                </div>
                                <!--[/if]-->

                            </div>
                        </div>
                        <!--#include file="_round_bottom.aspx"-->
                    </div>

                </div>
            </div>
        </div>
    </div>
<!--#include file="_foot.aspx"-->
</div>
</body>
</html>
