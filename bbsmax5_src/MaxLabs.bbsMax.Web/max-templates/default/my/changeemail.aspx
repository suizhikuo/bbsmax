<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/setting.css" />
</head>
<body>
<div class="container section-setting">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="../_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span>设置邮箱</span></h3>
                            </div>
                            <div class="formcaption">
                                <h3>更改邮箱</h3>
                                <p>你填写的邮箱是保密的, 网站不会向第三方透露你的邮箱信息. 取回密码、邮件通知等邮件都将发送到该邮箱。</p>
                            </div>
                            
                            <form action="$_form.action" method="post">
                            <!--[EmailUpdateForm]-->
                            <!--[if $OpenEmailValidate]-->
                            <!--[success form="updateEmail"]-->
                            <div class="successmsg">已成功发送验证邮件到您的邮箱, 请注意查收.</div>
                            <!--[/success]-->
                            <!--[/if]-->

                            <!--[if $MyEmailPassed]-->
                            <div class="verifyemail-success">你的邮箱 "$my.email" 已通过验证.</div>
                            <!--[else if $OpenEmailValidate]-->
                            <div class="verifyemail-fail">
                                您的邮箱还没有通过验证。
                                为了您的账号安全，请输入您的登录密码和真实的邮箱地址，进行邮箱验证。
                            </div>
                            <!--[/if]-->
                            
                            <!--[if $OpenEmailValidate]-->
                            <div class="formgroup">
                                <div class="formrow">
                                    <label class="label" for="password">本站密码</label>
                                    <div class="form-enter">
                                        <input id="password" class="text" name="password" type="password" value="" />
                                    </div>
                                    <!--[error form="updateEmail" name="password"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <label class="label" for="email">常用邮箱</label>
                                    <div class="form-enter">
                                        <!--[if $UnValidateEmail!=""]-->
                                        <input type="text" id="email" class="text" name="email" value="$_form.text('email',$UnValidateEmail)" />
                                            待验证
                                            <!--[if $My.EmailValidated]-->
                                            , 已验证过的邮箱:$My.Email 
                                            <!--[/if]-->
                                        <!--[else]-->
                                        <input type="text" id="email" class="text" name="email" value="$_form.text('email',$my.email)" />
                                        <!--[/if]-->
                                    </div>
                                    <!--[error form="updateEmail" name="email"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input class="button" name="emailcheck" type="submit" value="$_if($myemailPassed,'重新')验证邮箱" /></span></span>
                                </div>
                            </div>
                            <!--[else]-->
                            <!--[success form="updateEmail"]-->
                            <div class="successmsg">你已成功修改邮箱.</div>
                            <!--[/success]-->
                            <div class="formgroup">
                                <div class="formrow">
                                    <label class="label" for="email">常用邮箱</label>
                                    <div class="form-enter">
                                        <input type="text" class="text" id="email" name="email" value="$_form.text('email',$my.email)" />
                                    </div>
                                    <!--[error form="updateEmail" name="email"]-->
                                    <p class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input type="submit" name="updateEmail" class="button" value="修改" /></span></span>
                                </div>
                            </div>
                            <!--[/if]-->
                            
                            <!--[/EmailUpdateForm]-->
                            </form>

                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
