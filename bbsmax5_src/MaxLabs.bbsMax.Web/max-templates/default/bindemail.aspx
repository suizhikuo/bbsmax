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
    <div id="main" class="main section-account">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            <div class="formcaption">
                                <h3>解决邮箱冲突</h3>
                                <p>我们发现有多个用户使用了这个邮箱。如果您希望使用这个邮箱登陆，我们需要确认您是该邮箱的所有者。</p>
                            </div>
                            <form id="bindmailform" action="$_form.action" method="post">
                            <div class="formgroup">
                                <!--[unnamederror]-->
                                <div class="errormsg">$message</div>
                                <!--[/unnamederror]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="email">邮箱</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="email" id="email" disabled="disabled" value="$_form.text('email',$_get.email)" tabindex="1" />
                                    </div>
                                    <!--[error name="email"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <h3 class="label"><label for="username">网站账号</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="username" id="username" value="$_form.text('username',$_post.username)" tabindex="2" />
                                    </div>
                                    <!--[error name="username"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <h3 class="label"><label for="password">网站密码</label></h3>
                                    <div class="form-enter">
                                        <input type="password" class="text" name="password" id="password" tabindex="3" />
                                    </div>
                                    <!--[error name="password"]-->
                                    <div class="form-tip tip-error">$message</div>
                                    <!--[/error]-->
                                </div>
                                <input type="hidden" name="postemail" id="postemail" value="$_form.text('email',$_get.email)" />
                                <div class="formrow formrow-action">
                                    <span class="btn-wrap"><span class="btn"><input type="submit" name="bindemail" value="绑定邮箱" class="button" tabindex="4" /></span></span>
                                </div>
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
