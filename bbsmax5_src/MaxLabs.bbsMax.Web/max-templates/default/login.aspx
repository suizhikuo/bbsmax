<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/account.css" />
</head>
<body>
<div class="container section-account section-account-login">
<!--#include file="_inc/_head.aspx"-->
<div id="main" class="main">
    <div class="clearfix main-inner">
        <div class="content">
            <div class="clearfix content-inner">
                <div class="content-main">
                    <!--#include file="_inc/_round_top.aspx"-->
                    <div class="clearfix content-main-inner">
                        <div class="loginform-wrap">
                            <div class="formcaption">
                                <h3>用户登录</h3>
                                <p><!--[if $IsLogin]-->您的账号 $My.Username 已经登录.<!--[else]-->请输入用户名和密码.<!--[/if]--></p>
                            </div>
                            <div class="formgroup loginform">
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
                                        <select id="logintype" name="logintype" tabindex="1">
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
                                        <input type="text" class="text validcode" name="$inputName" id="validatecode" $_if($disableIme,'style="ime-mode:disabled;"') tabindex="4" autocomplete="off" />
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
                                    <input type="hidden" name="returnurl" value="$_form.text('returnurl', $returnurl)" />
                                    <span class="btn-wrap"><span class="btn"><input type="submit" name="login" value="登录" class="button" tabindex="7" /></span></span>
                                    <a href="$url(recoverpassword)">忘记密码?</a>
                                </div>
                                <!--[if $LoginType == UserLoginType.Email]-->
                                <div class="bindemail">
                                    <a href="loginemailbind.aspx">您已经拥有本站的用户名？请点击此处绑定email后再登陆</a>
                                </div>
                                <!--[/if]-->
                            </form>
                            </div>
                        </div>
                        <div class="reglink-wrap">
                            <div class="reglink">
                                <div class="reglink-inner">
                                    <div class="reglink-text">
                                        <h3>还没有注册吗?</h3>
                                        <p>如果还没有本站的帐号, 请先注册一个属于自己的帐号吧.</p>
                                    </div>
                                    <div class="register-link">
                                        <a href="$url(register)" target="_parent" class="button">立即注册</a>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <!--#include file="_inc/_round_bottom.aspx"-->
                </div>
            </div>
        </div>
    </div>
</div>
<!--#include file="_inc/_foot.aspx"-->
</div>
</body>
</html>
