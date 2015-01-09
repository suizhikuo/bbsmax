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

    <section class="main findpassword">
<!--[RecoverPasswordForm serial="$_get.serial"]-->
    <!--[if $RecoverPasswordEnable]-->
        <!--[if $IsRecoverMode]-->
            <!--[if !string.IsNullOrEmpty($RecoverUsername)]-->
        <form action="$_form.action" method="post">
            <!--[unnamederror]-->
            <div class="errormsg">$message</div>
            <!--[/unnamederror]-->
            <!--[error name="newPassword"]-->
            <div class="errormsg">$message</div>
            <!--[/error]-->
            <!--[error name="password2"]-->
            <div class="errormsg">$message</div>
            <!--[/error]-->
            <div class="form">
                <div class="row">
                    <label class="label">用户名</label>
                    <div class="enter">
                        $RecoverUsername
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="password">新密码</label>
                    <div class="enter">
                        <input name="password" id="password" class="text" type="password">
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="confirmpassword">重复密码</label>
                    <div class="enter">
                        <input name="password2" id="password2" class="text" type="password">
                    </div>
                </div>
                <div class="row">
                    <input type="hidden" name="serial" value="$_get.serial">
                    <input type="submit" name="resetRecoverPassword" value="修改密码" class="button">
                </div>
            </div>
        </form>
            <!--[else]-->
        <div class="infomessage message-error">
            <h3 class="title">操作出错</h3>
            <p class="intro">可能输入不正确的序列号, 或者序列号已经过期.</p>
            <p class="action">
                <a href="javascript:history.go(-1)">&laquo; 返回上页</a>
                <a href="$url(default)">前往首页 &raquo;</a>
            </p>
        </div>
            <!--[/if]-->
        <!--[else]-->
        <form action="$_form.action" method="post">
            <div class="form">
                <!--[unnamederror]-->
                <div class="errormsg">$message</div>
                <!--[/unnamederror]-->
                <!--[error name="username"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[error name="email"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[ValidateCode actionType="recoverpassword"]-->
                <!--[error name="$inputName"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[/ValidateCode]-->
                <div class="row">
                    <label class="label" for="username">用户名</label>
                    <div class="enter">
                        <input id="username" name="username" value="$_form.text('username')" type="text" class="text" autocorrect="off" autocapitalize="off">
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="email">邮箱</label>
                    <div class="enter">
                        <input name="email" id="email" value="$_form.text('email')" type="email" class="text" autocorrect="off">
                    </div>
                    <div class="note">填写您在本站注册时使用的邮箱.</div>
                </div>
                <!--[ValidateCode actionType="recoverpassword"]-->
                <div class="row">
                    <label class="label" for="$inputName">验证码</label>
                    <div class="enter">
                        <input type="text" class="text captcha" name="$inputName" id="$inputName" $_if($disableIme,'style="ime-mode:disabled;"') placeholder="$tip">
                    </div>
                    <div class="captchaimg">
                        <img src="$imageurl" alt="" onclick="javascript:this.src=this.src+'&rnd=' + Math.random();">
                        点击图片更换验证码
                    </div>
                </div>
                <!--[/ValidateCode]-->
                <div class="row row-button">
                    <input type="submit" name="RecoverPassword" value="提交" class="button">
                </div>
            </div>
        </form>
        <!--[/if]-->
    <!--[else]-->
        <div class="infomessage message-error">
            <h3 class="title">操作出错</h3>
            <p class="intro">网站已经关闭了密码取回功能, 如果你忘记密码请联网站管理员!</p>
            <p class="action">
                <a href="javascript:history.go(-1)">&laquo; 返回上页</a>
                <a href="$url(default)">前往首页 &raquo;</a>
            </p>
        </div>
    <!--[/if]-->
<!--[/RecoverPasswordForm]-->
    </section>

    <!--#include file="_inc/_footer.aspx"-->
</div>
</body>
</html>
