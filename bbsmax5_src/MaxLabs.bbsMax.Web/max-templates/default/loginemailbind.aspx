<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/account.css" />
</head>
<body>
<div class="container">
    <!--#include file="_inc/_head.aspx"-->
    <div id="main" class="main section-account section-account-login">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            <div class="formcaption">
                                <h3>绑定邮箱</h3>
                                <p>请输入的用户名和密码, 将邮箱与账号绑定.</p>
                            </div>
                            <!--[success form="updateEmail"]-->
                            <div class="successmsg">你已成功修改邮箱.</div>
                            <!--[/success]-->
                            <form id="bindmailform" action="$_form.action" method="post">
                            <div class="formgroup loginform">
                                <!--[unnamederror]-->
                                <div class="errormsg">$message</div>
                                <!--[/unnamederror]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="username">用户名</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="username" id="username" value="$_form.text('username',$_post.username)" tabindex="1" />
                                    </div>
                                    <!--[error name="username"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <h3 class="label"><label for="password">密码</label></h3>
                                    <div class="form-enter">
                                        <input type="password" class="text" name="password" id="password" value="$_form.text('password')"  tabindex="2" />
                                    </div>
                                    <!--[error name="password"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                </div>
                            <!--[if $LoginUser == null]-->
                                <div class="formrow formrow-action">
                                    <span class="btn-wrap"><span class="btn"><input type="submit" name="checkuser" value="下一步" class="button" tabindex="3" /></span></span>
                                </div>
                            <!--[else]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="email">您当前的邮箱</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="email" id="email" value="$_form.text('email',$LoginUser.Email)" tabindex="3" />
                                    </div>
                                    <!--[error name="email" form="updateEmail"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                    <!--[if $LoginUser.EmailValidated]-->
                                    <div class="form-note">
                                        该邮箱已经通过验证,无需再次绑定,您可以直接使用该邮箱登录论坛.
                                        <a href="login.aspx?email=$LoginUser.Email">点击立刻登录</a>
                                    </div>
                                    <!--[/if]-->
                                </div>
                                <!--[if !$LoginUser.EmailValidated]-->
                                <div class="formrow formrow-action">
                                    <span class="btn-wrap"><span class="btn"><input type="submit" name="bindemail" value="绑定邮箱" class="button" tabindex="4" /></span></span>
                                </div>
                                <!--[/if]-->
                            <!--[/if]-->
                            </div>
                            </form>
                        </div>
                    </div>
                </div>
             </div>
        </div>
    </div>
    <!--#include file="_inc/_foot.aspx"-->
</div>   
</body>
</html>
