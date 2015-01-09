<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/account.css" />
</head>
<body>
<div class="container section-account section-account-findpassword">
<!--#include file="_inc/_head.aspx"-->
<div id="main" class="main">
    <div class="clearfix main-inner">
        <div class="content">
            <div class="clearfix content-inner">
                <div class="content-main">
                    <!--#include file="_inc/_round_top.aspx"-->
                    <div class="clearfix content-main-inner">

                    <div class="findpassworddock-wrap">
                        <div class="findpassworddock">
                            <div class="findpassworddock-inner">
            <!--[RecoverPasswordForm serial="$_get.serial"]-->
                <!--[if $RecoverPasswordEnable]-->
                    <!--[if $IsRecoverMode]-->
                        <!--[if !string.IsNullOrEmpty($RecoverUsername)]-->
                        <div class="formcaption">
                            <h3>找回密码</h3>
                            <p>请输入你的新密码.</p>
                        </div>
                        <!--[unnamederror]-->
                        <div class="errormsg">$message</div>
                        <!--[/unnamederror]-->
                        <form action="$_form.action" method="post">
                        <div class="formgroup findpasswordform">
                            <div class="formrow">
                                <h3 class="label">用户名</h3>
                                <div class="form-enter">
                                    <strong>$RecoverUsername</strong>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="password">新密码</label></h3>
                                <div class="form-enter">
                                    <input name="password" id="password" class="text" type="password" tabindex="1" />
                                    <!--[error name="newPassword"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="confirmpassword">重复密码</label></h3>
                                <div class="form-enter">
                                    <input name="password2" id="password2" class="text" type="password" tabindex="2" />
                                    <!--[error name="password2"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <input type="hidden" name="serial" value="$_get.serial" />
                                <span class="btn-wrap"><span class="btn"><input type="submit" name="resetRecoverPassword" value="修改密码" class="button" tabindex="3" /></span></span>
                            </div>
                        </div>
                        </form>
                        <!--[else]-->
                        <div class="account-message">不正确的序列号，或者序列号已经过期.</div>
                        <!--[/if]-->
                    <!--[else]-->
                        <div class="formcaption">
                            <h3>找回密码</h3>
                            <p>请输入你注册时使用的用户名和邮箱地址.</p>
                        </div>
                        <!--[unnamederror]-->
                        <div class="errormsg">$message</div>
                        <!--[/unnamederror]-->
                        <form action="$_form.action" method="post">
                        <div class="formgroup findpasswordform">
                            <div class="formrow">
                                <h3 class="label"><label for="username">用户名</label></h3>
                                <div class="form-enter">
                                    <input id="username" name="username" value="$_form.text('username')" type="text" class="text" tabindex="1" />
                                    <!--[error name="username"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="email">邮箱</label></h3>
                                <div class="form-enter">
                                    <input name="email" id="email" value="$_form.text('email')" type="text" class="text" tabindex="2" />
                                    <!--[error name="email"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <div class="form-note">填写您在本站注册时使用的Email.</div>
                            </div>
                            <!--[ValidateCode actionType="recoverpassword"]-->
                            <div class="formrow">
                                <h3 class="label"><label for="$inputName">验证码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text validcode" name="$inputName" id="$inputName" tabindex="3" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                    <span class="captcha">
                                        <img alt="" src="$imageurl" title="看不清,点击刷新" onclick="javascript:this.src=this.src+'&rnd=' + Math.random();" />
                                    </span>
                                    <!--[error name="$inputName"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <div class="form-note">$tip</div>
                            </div>
                            <!--[/ValidateCode]-->
                            <div class="formrow formrow-action">
                                <span class="btn-wrap"><span class="btn"><input type="submit" name="RecoverPassword" value="提交" class="button" tabindex="4" /></span></span>
                            </div>
                        </div>
                        </form>
                    <!--[/if]-->
                <!--[else]-->
                        <div class="account-message">网站已经关闭了密码取回功能, 如果你忘记密码请联网站管理员!</div>
                <!--[/if]-->
            <!--[/RecoverPasswordForm]-->
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
