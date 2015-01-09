<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/account.css" />
<script type="text/javascript">
var handler ="$url(handler/RegValidate)?field={0}&value={1}";
var msgObj={};
msgObj.img = '<span class="ajaxloading">加载中...</span>';
msgObj.success = '<span class="form-tip tip-success">可以注册</span>';
msgObj.frustrated='<span class="form-tip tip-error">{0}</span>';
function validate(field){var value = $(field).value; if(value.contains('+')){ showError(field,  '不能包含：+'); return ; } var url = String.format(handler,field, encodeURIComponent (value));var ajax = new ajaxWorker(url,"get",'');    var label= $('err_'+field);label.innerHTML = msgObj.img;ajax.addListener(200,
function(r)
{
    if(r){eval("var result = "+r);
    if(result.state==0)
    {label.innerHTML = msgObj.success;}
    else
    {label.innerHTML = String.format( msgObj.frustrated,result.message);}
    }
    }
    
    );ajax.send();
}
function showError(field,msg)
{
    var label= $('err_'+field);
    label.innerHTML = String.format( msgObj.frustrated,msg);
}
function clear(field){$('err_'+field).innerHTML  = '';}
</script>
</head>
<body>
<div class="container section-account section-account-register">
<!--#include file="_inc/_head.aspx"-->
<div id="main" class="main">
    <div class="clearfix main-inner">
        <div class="content">
            <div class="clearfix content-inner">
                <div class="content-main">
                    <!--#include file="_inc/_round_top.aspx"-->
                    <div class="clearfix content-main-inner">

                    <div class="registerdock-wrap">
                        <div class="registerdock">
                            <div class="registerdock-inner">
            <!--[RegisterForm inviteSerial="$_get.invite"]-->
                <!--[if $IsActiveState]--> <!--判断用户是否正在激活帐号-->
                        <div class="formgroup registerform">
                            <!--[unnamederror]-->
                            <div class="errormsg">$message</div>
                            <!--[/unnamederror]-->
                        <form method="post" action="$_form.action">
                            <div class="registerverify-tip">
                               您的帐号已经注册成功：请点击下面的“<font color="red" size="4">激活帐号</font>”按钮启用您的帐号。
                            </div>
                             <div class="formrow formrow-action">
                             <span class="btn-wrap">
                             <span class="btn">
                            <input class="button"  type="submit" name="activeme" value="激活帐号"/>
                            </span>
                            </span>
                            </div>
                        </form>
                    </div>
                <!--[else]-->
                    <!--[if $CannotRegister]-->
                            <div class="account-message">$CannotRegisterReason</div>
                    <!--[else]-->
                            <form action="$_form.action" method="post">
                <!--[if !IAgree && $CanDisplayAgreement]-->
                        <div class="formcaption">
                            <h3>注册协议</h3>
                            <p>注册前请先阅读以下协议.</p>
                        </div>
                        <div class="agreement-content">
                           <div class="agreement-content-inner">
                              $AgreementContent
                           </div>
                        </div>
                        <div class="clearfix agreement-buttons">
                            <span class="btn-wrap"><span class="btn"><input class="button" type="submit" name="iagree" id="iagree" value="我同意" /></span></span>
                            <span class="btn-wrap"><span class="btn"><input class="button" type="button" value="不同意" onclick="<!--[if $isdialog]-->dialog.close();<!--[else]-->history.go(-1);<!--[/if]-->" /></span></span>
                        </div>
                        <!--[if $AgreementDisplayTime>0]-->
                        <script type="text/javascript">
                            var agreementTime = $AgreementDisplayTime;
                            var iagree = document.getElementById('iagree');
                            function closeAgreementTime() {
                                if (agreementTime == 0) {
                                    iagree.disabled = '';
                                    iagree.value = '我同意';
                                    iagree.parentNode.parentNode.className = 'btn-wrap';
                                } else {
                                    iagree.value = '我同意 (' + agreementTime + ')';
                                    iagree.parentNode.parentNode.className = 'btn-wrap btn-disable';
                                    agreementTime--;
                                    iagree.disabled = 'disabled';
                                    window.setTimeout("closeAgreementTime()", 1000);
                                }
                            }
                            closeAgreementTime();
                        </script>
                        <!--[/if]-->
                <!--[else]-->
                        <div class="formcaption">
                            <!--[if $IsValidateEmail]-->
                            <h3>邮箱验证</h3>
                            <p>您的邮箱还尚未验证.</p>
                            <!--[else if $IsActivingUser]-->
                            <h3>激活用户</h3>
                            <p>您的账号还未激活.</p>
                            <!--[else]-->
                            <h3>用户注册</h3>
                            <p>注册完成后, 该帐号将作为您在本站的通行帐号, 您可以享受本站提供的各种服务.</p>
                            <!--[/if]-->
                        </div>
                   <!--[if !$NeedActive]-->
                        <!--[if $CanDisplayInviter]-->
                        <div class="clearfix register-inviter">
                            <div class="inviter-avatar"><img src="$inviter.AvatarPath" alt="" width="48" height="48" /></div>
                            <div class="inviter-message">
                                <h3>您好</h3>
                                <p>你的朋友<a href="$url(space/$inviter.ID)">$inviter.popupnamelink</a>已经是$BbsName用户.</p>
                                <p><a href="$url(space/$inviter.ID)">$inviter.popupnamelink</a>邀请你注册成为$BbsName用户.</p>
                                <p>注册成功后, 你可以关注<a href="$url(space/$inviter.ID)">$inviter.popupnamelink</a>的动态和享受本站提供的各种服务.</p>
                            </div>
                        </div>
                        <!--[/if]-->
                        <div class="formgroup registerform">
                            <!--[unnamederror]-->
                            <div class="errormsg">$message</div>
                            <!--[/unnamederror]-->
                            <!--[if $CanDisplaySerialInput]-->
                            <div class="formrow">
                                <h3 class="label"><label for="serial">邀请码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" name="invite" id="serial" value="$_form.text('invite',$_get.invite)" tabindex="1" />
                                    <!--[error name="serial"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <!--[if $CanEmptySerial]-->
                                <div class="form-note">如果您没有邀请码可以不填，同样可以注册</div>
                                <!--[else]-->
                                <div class="form-note">需要输入邀请码才能注册</div>
                                <!--[/if]-->
                            </div>
                            <!--[/if]-->
                            <div class="formrow">
                                <h3 class="label"><label for="username">用户名</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" name="username" id="username" onblur="validate('username')" value="$_form.text('username')" tabindex="2" />
                                    <span id="err_username">&nbsp;</span>
                                </div>
                                <div class="form-note">$UsernameTip</div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="password">密码</label></h3>
                                <div class="form-enter">
                                    <input type="password" class="text" name="password" value="$_form.text('password')" id="password" tabindex="3" />
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="confirmpassword">确认密码</label></h3>
                                <div class="form-enter">
                                    <input type="password" class="text" name="password2" value="$_form.text('password2')" id="confirmpassword" tabindex="4" />
                                    <span id="checkpassword2">&nbsp;</span>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="email">Email</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" onblur="validate('email')" name="email" id="email" value="$_form.text('email')" tabindex="5" />
                                    <!--[error name="email"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                    <span id="err_email">&nbsp;</span>
                                </div>
                                <!--[if $EnabledEmailValidate]-->
                                <div class="form-note">账号需要邮箱验证后才能使用, 请填写正确的邮箱地址</div>
                                <!--[else]-->
                                <div class="form-note">请准确填写您的邮箱. 在忘记密码或者您使用邮件通知功能时, 会发送邮件到该邮箱.</div>
                                <!--[/if]-->
                            </div>
                            <!--[ValidateCode actionType="$validateCodeAction"]-->
                            <div class="formrow">
                                <h3 class="label"><label for="validatecode">验证码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text validcode" name="$inputName" value="" tabindex="6" autocomplete="off"  $_if($disableIme,'style="ime-mode:disabled;"') />
                                    <span class="captcha">
                                        <img src="$imageurl" alt="" title="看不清,点击刷新" onclick="javascript:this.src=this.src+'&rnd=' + Math.random();" />
                                    </span>
                                    <!--[error name="$inputName"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <div class="form-note">$tip</div>
                            </div>
                            <!--[/ValidateCode]-->
                            <!--[if $CanDisplayAgreementLink]-->
                            <div class="formrow formrow-agree">
                                <input type="checkbox" name="agree" value="1" checked="checked" id="agree" tabindex="7" />
                                <label for="agree">同意</label>
                                <a href="$dialog/user-agreement.aspx" target="_blank" onclick="return openDialog(this.href)">网站注册协议</a>
                            </div>
                            <!--[/if]-->
                            <div class="formrow formrow-action">
                            <!--[if $CanDisplayAgreementLink == false]-->
                                 <input type="hidden" value="1" name="agree" />
                            <!--[/if]-->
                                <span class="btn-wrap"><span class="btn"><input type="submit" name="register" value="注册新用户" class="button" tabindex="8" /></span></span>
                            </div>
                        </div>
                        
                   <!--[else]-->
                        <!--[if NeedSendMailClick]-->
                            <div class="clearfix registerverify-button">
                                <span class="btn-wrap btn-highlight"><span class="btn"><input type="submit" class="button" name="sendmail"  value="点击发送验证邮件" /></span></span> 
                            </div>
                        <!--[else]-->
                            <div class="registerverify">
                            <input type="hidden" name="userid" value="$_form.text('userid','$_get.userid')" />
                            <input type="hidden" name="code" value="$_form.text('code','$_get.code')" />
                            <!--[unnamederror]-->
                            <div class="errormsg">$message</div>
                            <!--[/unnamederror]-->
                            <!--[if $IsOverrunLimitTimes==false]-->
                            <div class="alertmsg">
                                温馨提示: 我们已经给您发送了验证邮件，邮箱地址为: $Email 
                            </div>
                            <!--[/if]-->
                            <ul class="registerverify-step">
                                <li>
                                    <strong>第一步: 查收您的验证邮件</strong>
                                    请登录到您的邮箱查收验证邮件.
                                    <!--[if $EmailLoginLink!=null]-->
                                    <a class="mail-login" href="$EmailLoginLink" target="_blank">登录邮箱</a>
                                    <!--[/if]-->
                                </li>
                                <li>
                                    <strong>第二步: 点击验证邮件中的验证链接</strong>
                                    请在24小时内激活您的帐号.
                                </li>
                                <li>
                                    <strong>如果一小时内没收到邮件, 建议重新发送验证邮件.</strong>
                                </li>
                            </ul>
                            
                            <div class="clearfix registerverify-button">
                                <!--[if $EmailLoginLink!=null]-->
                                <!--p class="registerverify-buttontext">点击下面的按钮登录您的$EmailName. </p-->
                                <span class="btn-wrap btn-highlight"><span class="btn"><a href="$EmailLoginLink" class="button"  target="_blank">点击登录$EmailName</a></span></span>
                                <!--[/if]-->
                                <span class="btn-wrap btn-highlight"><span class="btn"><input type="submit" class="button" name="sendmail" id="resend" value="点击重新发送验证邮件" /></span></span> 
                            </div>
                            <script type="text/javascript">
                                var agreementTime = 60;
                                var iagree = document.getElementById('resend');
                                function closeAgreementTime() {
                                    if (agreementTime == 0) {
                                        iagree.disabled = '';
                                        iagree.value = '未收到邮件点击此处重发';
                                        //     iagree.parentNode.parentNode.className = 'minbtn-wrap';
                                    } else {
                                    iagree.value = "未收到邮件" + agreementTime + "秒后可重发"; ;
                                        //     iagree.parentNode.parentNode.className = 'minbtn-wrap btn-disable';
                                        agreementTime--;
                                        iagree.disabled = 'disabled';
                                        window.setTimeout("closeAgreementTime()", 1000);
                                    }
                                }
                                closeAgreementTime();
                            </script>
                        </div>
                        <!--[/if]-->
                   <!--[/if]-->
                <!--[/if]-->
                </form>
                    <!--[/if]-->
                <!--[/if]-->
            <!--[/RegisterForm]-->
                            </div>
                        </div>
                    </div>

                    <div class="loginlink-wrap">
                        <div class="loginlink">
                            <div class="loginlink-inner">
                                <div class="loginlink-text">
                                    <h3>已有帐号?</h3>
                                </div>
                                <div class="login-link">
                                    <a href="$url(login)" target="_parent" class="button">登录帐号</a>
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
