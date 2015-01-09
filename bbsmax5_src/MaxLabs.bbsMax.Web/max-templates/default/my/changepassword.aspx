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
                                <h3 class="pagecaption-title"><span>修改密码</span></h3>
                            </div>
                            
                            <!--[ChangePasswordForm]-->
                            <form action="$_form.action" method="post">
                            <!--[success form="changePwd"]-->
                            <div class="successmsg">你已成功修改密码.</div>
                            <!--[/success]-->
                            <div class="formgroup">
                                <div class="formrow">
                                    <label class="label" for="password">旧密码</label>
                                    <div class="form-enter">
                                        <input id="password" class="text" name="password" type="password" value="" />
                                    </div>
                                    <!--[error form="changePwd" name="oldpassword"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <label class="label" for="newpasswd">新密码</label>
                                    <div class="form-enter">
                                        <input id="newpasswd" class="text" name="newpassword" type="password" value="" />
                                    </div>
                                    <!--[error form="changePwd" name="newpassword"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow">
                                    <label class="label" for="newpasswd2">确认新密码</label>
                                    <div class="form-enter">
                                        <input id="newpasswd2" class="text" name="newpassword2" type="password" value="" />
                                    </div>
                                    <!--[error form="changePwd" name="newpassword2"]-->
                                    <p class="form-tip tip-error">$message</p>
                                    <!--[/error]-->
                                </div>
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input class="button" name="changepassword" type="submit" value="修改密码" /></span></span>
                                </div>
                            </div>
                            </form>
                            <!--[/ChangePasswordForm]-->
                            
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
