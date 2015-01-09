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

    <section class="main register">
<!--[RegisterForm inviteSerial="$_get.invite"]-->
<!--[if $IsActiveState]-->
        <div class="infomessage message-success">
            <p class="title">您的帐号已经注册成功</p>
            <p>请点击下面的 "激活帐号" 按钮启用您的帐号.</p>
        </div>
        <form method="post" action="$_form.action">
            <div class="form regverify">
                <!--[unnamederror]-->
                <div class="errormsg">$message</div>
                <!--[/unnamederror]-->
                <div class="button">
                    <input class="button" type="submit" name="activeme" value="激活帐号">
                </div>
            </div>
        </form>
<!--[else]-->
    <!--[if $CannotRegister]-->
        <div class="infomessage message-alert">
            <h3 class="title">暂时无法注册</h3>
            <p class="intro">$CannotRegisterReason</p>
            <p class="action">
                <a href="javascript:history.go(-1)">&laquo; 返回上页</a>
                <a href="$url(default)">前往首页 &raquo;</a>
            </p>
        </div>
    <!--[else]-->
        <form action="$_form.action" method="post">
        <!--[if !IAgree && $CanDisplayAgreement]-->
            <div class="agreement">
                <div class="content">
                    $AgreementContent
                </div>
                <div class="action">
                    <input class="button" type="submit" name="iagree" value="同意协议, 并继续注册">
                </div>
            </div>
        <!--[else]-->
        <!--[if !$NeedActive]-->
            <!--[if $CanDisplayInviter]-->
            <div class="reginviter">
                <p class="avatar"><img src="$inviter.AvatarPath" alt="" width="48" height="48"></p>
                <div class="message">
                    <p>你的朋友 $inviter.popupnamelink 已经是$BbsName用户.</p>
                    <p>$inviter.popupnamelink 邀请你注册成为$BbsName用户.</p>
                    <p>注册成功后, 你可以关注 $inviter.popupnamelink 的动态和享受本站提供的各种服务.</p>
                </div>
            </div>
            <!--[/if]-->
            <div class="form">
                <!--[unnamederror]-->
                <div class="errormsg">$message</div>
                <!--[/unnamederror]-->
                <!--[if $CanDisplaySerialInput]-->
                <!--[error name="serial"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[/if]-->
                <!--[error name="email"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[ValidateCode actionType="$validateCodeAction"]-->
                <!--[error name="$inputName"]-->
                <div class="errormsg">$message</div>
                <!--[/error]-->
                <!--[/ValidateCode]-->
                <!--[if $CanDisplaySerialInput]-->
                <div class="row">
                    <label class="label" for="serial">邀请码</label>
                    <div class="enter">
                        <input type="text" class="text" name="invite" id="serial" value="$_form.text('invite',$_get.invite)" autocorrect="off" autocapitalize="off">
                    </div>
                    <!--[if $CanEmptySerial]-->
                    <div class="note">如果您没有邀请码可以不填，同样可以注册</div>
                    <!--[else]-->
                    <div class="note">需要输入邀请码才能注册</div>
                    <!--[/if]-->
                </div>
                <!--[/if]-->
                <div class="row">
                    <label class="label" for="username">用户名</label>
                    <div class="enter">
                        <input type="text" class="text" name="username" id="username" value="$_form.text('username')" autocorrect="off" autocapitalize="off">
                    </div>
                    <div class="note">$UsernameTip</div>
                </div>
                <div class="row">
                    <label class="label" for="email">邮箱</label>
                    <div class="enter">
                        <input type="email" class="text" name="email" id="email" value="$_form.text('email')">
                    </div>
                    <!--[if $EnabledEmailValidate]-->
                    <div class="note">账号需要邮箱验证后才能使用, 请填写正确的邮箱地址</div>
                    <!--[else]-->
                    <div class="note">请准确填写您的邮箱. 在忘记密码或者您使用邮件通知功能时, 会发送邮件到该邮箱.</div>
                    <!--[/if]-->
                </div>
                <div class="row">
                    <label class="label" for="password">密码</label>
                    <div class="enter">
                        <input type="password" class="text" name="password" value="$_form.text('password')" id="password">
                    </div>
                </div>
                <div class="row">
                    <label class="label" for="confirmpassword">确认密码</label>
                    <div class="enter">
                        <input type="password" class="text" name="password2" value="$_form.text('password2')" id="confirmpassword">
                    </div>
                </div>
                <!--[ValidateCode actionType="$validateCodeAction"]-->
                <div class="row">
                    <label class="label" for="validatecode">验证码</label>
                    <div class="enter">
                        <input type="text" class="text captcha" name="$inputName" value="" $_if($disableIme,'style="ime-mode:disabled;"') placeholder="$tip">
                    </div>
                    <div class="captchaimg">
                        <img src="$imageurl" alt="" onclick="javascript:this.src=this.src+'&rnd=' + Math.random();">
                        点击图片更换验证码
                    </div>
                </div>
                <!--[/ValidateCode]-->
                <!--[if $CanDisplayAgreementLink]-->
                <div class="row">
                    <input type="checkbox" name="agree" value="1" checked="checked" id="agree">
                    <label for="agree">我已经认真阅读, 并同意接受注册协议中的各项规定.</label>
                    <a href="$dialog/user-agreement.aspx">网站注册协议</a>
                </div>
                <!--[/if]-->
                <div class="row row-button">
                    <!--[if $CanDisplayAgreementLink == false]-->
                    <input type="hidden" value="1" name="agree">
                    <!--[/if]-->
                    <input type="submit" name="register" value="注册新用户" class="button">
                </div>
            </div>
        <!--[else]-->
            <!--[if NeedSendMailClick]-->
            <div class="verifybutton">
                <input type="submit" class="button" name="sendmail" value="发送验证邮件">
            </div>
            <!--[else]-->
            <div class="regverify">
                <input type="hidden" name="userid" value="$_form.text('userid','$_get.userid')">
                <input type="hidden" name="code" value="$_form.text('code','$_get.code')">
                <!--[unnamederror]-->
                <div class="errormsg">$message</div>
                <!--[/unnamederror]-->
                <!--[if $IsOverrunLimitTimes==false]-->
                <div class="alertmsg">系统已向"$Email"发送验证邮件.</div>
                <!--[/if]-->
                <div class="action">
                    <input type="submit" class="button" name="sendmail" value="重新发送验证邮件">
                </div>
            </div>
            <!--[/if]-->
        <!--[/if]-->
        <!--[/if]-->
        </form>
    <!--[/if]-->
<!--[/if]-->
<!--[/RegisterForm]-->
    </section>

    <!--#include file="_inc/_footer.aspx"-->
</div>
</body>
</html>
