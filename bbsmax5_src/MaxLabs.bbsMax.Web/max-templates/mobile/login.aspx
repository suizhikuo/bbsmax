<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>$PageTitle</title>
<!--#include file="_inc/_htmlmeta.aspx"-->
</head>
<body>
<div class="container">
    <!--#include file="_inc/_header.aspx"-->
    
    <div class="crumbnav">
        <a href="$url(default)">&laquo; 返回首页</a>
    </div>

    <section class="main login">
        <form id="loginform" action="$_form.action" method="post">
            <div class="form">
                <!--[unnamederror]-->
                <div class="errormsg">$message</div>
                <!--[/unnamederror]-->
                <!--[error name="username"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[error name="password"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[validateCode actionType="$validateActionName"]-->
                <!--[error name="$inputName"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[/validateCode]-->
                <div class="row">
                    <label class="label" for="account"><!--[if $LoginType == UserLoginType.Email]-->邮箱<!--[else]-->账号<!--[/if]--></label>
                    <div class="enter">
                        <!--[if $LoginType == UserLoginType.Username]-->
                        <input type="text" class="text" name="username" id="account" value="$_form.text('username')" autocorrect="off" autocapitalize="off" placeholder="输入您的用户名">
                        <!--[else if $LoginType == UserLoginType.Email]-->
                        <input type="email" class="text" name="username" id="account" value="$_form.text('username')" autocorrect="off" placeholder="输入您的邮箱">
                        <!--[else]-->
                        <input type="text" class="text" name="username" id="account" value="$_form.text('username')" autocorrect="off" autocapitalize="off" placeholder="输入您的用户名或者邮箱">
                        <!--[/if]-->
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="password">密码</label>
                    <div class="enter">
                        <input type="password" class="text" name="password" id="password" placeholder="输入您的密码">
                    </div>
                </div>
                <!--[validateCode actionType="$validateActionName"]-->
                <div class="row">
                    <label class="label" for="captcha">验证码</label>
                    <div class="enter">
                        <input type="text" class="text captcha" name="$inputName" id="captcha" placeholder="$tip">
                    </div>
                    <div class="captchaimg">
                        <img src="$imageurl" alt="" onclick="this.src=this.src+'&rnd='+Math.random();">
                        点击图片更换验证码
                    </div>
                </div>
                <!--[/validateCode]-->
                <div class="row">
                    <div class="enter">
                        <input type="checkbox" id="autologin" name="autologin" value="autologin" $_form.checked('autologin', 'autologin')>
                        <label for="autologin">记住我的登录状态</label>
                    </div>
                </div>
                <div class="row">
                    <div class="enter">
                        <input type="checkbox" id="invisible" name="invisible" value="true" $_form.checked('invisible', 'true')>
                        <label for="invisible">隐身登陆</label>
                    </div>
                </div>
                <div class="row row-button">
                    <input type="hidden" name="returnurl" value="$_form.text('returnurl', $returnurl)">
                    <input class="button" type="submit" name="login" value="登录">
                    <a class="forget" href="$url(recoverpassword)">忘记密码</a>
                </div>
            </div>
        </form>
    </section>

    <!--#include file="_inc/_footer.aspx"-->
</div>
</body>
</html>
