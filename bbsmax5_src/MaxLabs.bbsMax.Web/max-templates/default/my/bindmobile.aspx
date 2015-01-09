<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/setting.css" />
</head>
<body>
<div class="container section-setting section-bindmobile">
<!--#include file="../_inc/_head.aspx"-->
<div id="main" class="main">
    <div class="clearfix main-inner">
        <div class="content">
            <!--#include file="../_inc/_round_top.aspx"-->
            <div class="clearfix content-inner">
                <div class="content-main">
                    <div class="content-main-inner">
                        
                        <div class="clearfix pagecaption">
                            <h3 class="pagecaption-title"><span>$SubTitle</span></h3>
                        </div>
                        
                        <!--[unnamederror]-->
                        <div class="errormsg">$message</div>
                        <!--[/unnamederror]-->
                        <form id="form1" action="$_form.action" method="post">
            <!--[if $IsBoundFirst]-->
                        <div class="formgroup bindmobile-form">
                            <div class="formrow">
                                <h3 class="label">绑定状态</h3>
                                <div class="form-enter">
                                    <span class="phonebind-yes">已绑定</span>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label">绑定手机</h3>
                                <div class="form-enter">
                                    <span class="phone">$UserMobilePhoneWithStar</span>
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="changemobile" value="更换号码" class="button" /></span></span>
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="unbound" onclick="return confirm('确定要解除绑定吗?')" value="解除绑定" class="button" /></span></span>
                            </div>
                        </div>
                        <div class="formhelper">
                            <h3>注意</h3>
                            <ul>
                                <li>点击“解除绑定”后，您将收到一条来自站长之家的短信，其中包含了解除绑定所需的验证码。</li>
                            </ul>
                        </div>
            <!--[else if $BindingMobileFirst ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tipdock">
                                <div class="bindmobile-tip bindmobile-tip-no">您还未对帐号进行手机绑定.</div>
                                <div class="bindmobile-note">申请手机绑定后您可以享用$SiteName的所有便捷服务.</div>
                            </div>
                            
                            <div class="formrow">
                                <h3 class="label"><label for="mobilePhone">请输入手机号码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" name="mobilePhone" id="mobilePhone" value="$_form.text('mobilePhone')" />
                                    <!--[error name="mobilePhone"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <div class="form-note">
                                    <p>请输入您要绑定的手机号码，绑定手机不会向你收取任何服务费用.</p>
                                    <p>稍后我们会将验证码发送到你的手机, 请填写真实的号码.</p>
                                </div>
                            </div>
                            
                            <div class="formrow formrow-action">
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="next" value="下一步" class="button" /></span></span>
                            </div>
                        </div>
                        <div class="formhelper">
                            <h3>注意</h3>
                            <ul>
                                <li>点击“下一步”后，您将收到一条来$SiteName的短信，其中包含了手机验证码。</li>
                            </ul>
                        </div>
            <!--[else if $BindingMobileSecond ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tip">我们已将六位 "验证码" 发送到您的手机上, 请注意查收.</div>
                            <div class="formrow">
                                <h3 class="label"><label for="smscode">输入验证码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" name="smscode" id="smscode" value="$_form.text('smscode')" />
                                    <!--[error name="smscode"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                                <div class="form-note">您当前申请绑定的手机号码为: <span class="phone">$NewInputMobilePhone</span> <a href="javascript:void(clickButton('changeNum'))">[更改手机号码]</a></div>
                            </div>
                            <div class="formrow formrow-action">
                                <input type="hidden" name="mobilePhone" value="$_form.text('mobilePhone')" />
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="finish" value="确认" class="button" /></span></span>
                                <!--[if $IsOverrunLimitTryNum==false]-->
                                <span class="minbtn-wrap minbtn-disable"><span class="btn"><input type="submit" id="resend" name="next" value="(60) 重新发送验证码" class="button" /></span></span>
                                <!--[/if]-->
                            </div>
                        </div>
                        <div class="formhelper">
                            <h3>注意</h3>
                            <ul>
                                <li>如果1分钟后还未收到验证码, 请确认手机号码是否有误。号码无误，请点击 "重新发送验证码" 按钮.</li>
                                <li>输入错误的手机号码，号码边上 "更改手机号码" 的链接重新输入手机号码.</li>
                            </ul>
                        </div>
            <!--[else if $BindingMobileThird ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tipdock">
                                <div class="bindmobile-tip bindmobile-tip-yes">您已经成功绑定了手机，号码为: $My.MobilePhone</div>
                                <div class="bindmobile-note">提示: 绑定手机后你可以随时更改绑定号码，或者解除绑定。</div>
                            </div>
                        </div>
            <!--[else if $ChangeMobileFirst ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="formrow">
                                <h3 class="label">您的原手机号码</h3>
                                <div class="form-enter">
                                    <span class="phone">$UserMobilePhoneWithStar</span>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label">您的新手机号码</h3>
                                <div class="form-enter">
                                    <input type="text"  class="text" name="mobilePhone"  value="$_form.text('mobilePhone')" />
                                    <!--[error name="mobilePhone"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="changemobilenext" value="下一步" class="button" /></span></span>
                            </div>
                        </div>
                        <div class="formhelper">
                            <h3>注意</h3>
                            <ul>
                                <li>点击“下一步”后，站长之家将会向你提供的2个手机分别发送一条短信，其中包含了手机验证码。</li>
                            </ul>
                        </div>
            <!--[else if $ChangeMobileSecond ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tip">我们已将六位 "验证码" 分别发送到您的手机上, 请注意查收.</div>
                            <div class="formrow">
                                <h3 class="label">您的原手机号码</h3>
                                <div class="form-enter">
                                    <span class="phone">$UserMobilePhoneWithStar</span>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="oldsmscode">输入验证码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" name="oldsmscode" id="oldsmscode" value="$_form.text('oldsmscode')" />
                                    <!--[error name="oldsmscode"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label">您的新手机号码</h3>
                                <div class="form-enter">
                                    <span class="phone">$NewInputMobilePhone</span>
                                </div>
                            </div>
                            <div class="formrow">
                                <h3 class="label"><label for="newsmscode">输入验证码</label></h3>
                                <div class="form-enter">
                                    <input type="text" class="text" name="newsmscode" id="newsmscode" value="$_form.text('newsmscode')" />
                                    <!--[error name="newsmscode"]-->
                                    <span class="form-tip tip-error">$message</span>
                                    <!--[/error]-->
                                </div>
                            </div>
                            <div class="formrow formrow-action">
                                <input type="hidden" name="ischangemobile" value="$_post.ischangemobile" />
                                <input type="hidden" name="mobilePhone" value="$_form.text('mobilePhone')" />
                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="changemobilefinish" value="确认" class="button" /></span></span>
                                <!--[if $IsOverrunLimitTryNum==false]-->
                                <span class="minbtn-wrap minbtn-disable"><span class="btn"><input type="submit" name="changemobilenext" id="resend" value="重新发送验证码" class="button" /></span></span>
                                <!--[/if]-->
                            </div>
                        </div>
                        <div class="formhelper">
                            <h3>注意</h3>
                            <ul>
                                <li>如果1分钟后还未收到验证码, 请确认手机号码是否有误。号码无误，请点击 "重新发送验证码" 按钮.</li>
                                <li>输入错误的手机号码，号码边上 "更改手机号码" 的链接重新输入手机号码.</li>
                            </ul>
                        </div>
            <!--[else if $ChangeMobileThird ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tipdock">
                                <div class="bindmobile-tip bindmobile-tip-yes">您已经成功更改绑定，更改后的号码为: $My.MobilePhone</div>
                                <div class="bindmobile-note">提示: 绑定手机后你可以随时更改绑定号码，或者解除绑定。</div>
                            </div>
                        </div>
            <!--[else if $UnBoundMobileFirst ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tip">我们已将六位 "验证码" 发送到您的手机上, 请注意查收.</div>
                                    <div class="formrow">
                                            <h3 class="label"><label for="">手机号</label></h3>
                                            <div class="form-enter">
                                                <span>$UserMobilePhoneWithStar</span>
                                                <!--[error name="mobilePhone"]-->
                                                    <div class="form-tip tip-error">$message</div>
                                                <!--[/error]-->
                                            </div>
                                        </div>
                                    <div class="formrow">
                                        <h3 class="label"><label for="">输入验证码</label></h3>
                                        <div class="form-enter">
                                            <input type="text" class="text" name="smscode" id="Text2" value="$_form.text('smscode')" />
                                        </div>
                                        <!--[error name="smscode"]-->
                                            <div class="form-tip tip-error">$message</div>
                                        <!--[/error]-->
                                    </div>
                                    <div class="formrow formrow-action">
                                        <input type="hidden" name="isunbound" value="1" />
                                        <span class="minbtn-wrap"><span class="btn"><input type="submit" name="unboundfinish" value="确认" class="button" /></span></span>
                                        <!--[if $IsOverrunLimitTryNum==false]-->
                                        <span class="minbtn-wrap minbtn-disable"><span class="btn"><input type="submit" id="resend"  name="unbound" value="(60) 重新发送验证码" class="button" /></span></span>
                                        <!--[/if]-->
                                    </div>
                                </div>
                        <div class="formhelper">
                            <h3>注意</h3>
                            <ul>
                                <li>如果1分钟后还未收到验证码, 请点击 "重新发送验证码" 按钮.</li>
                            </ul>
                        </div>
            <!--[else if $UnBoundMobileSecond ]-->
                        <div class="formgroup bindmobile-form">
                            <div class="bindmobile-tipdock">
                                <div class="bindmobile-tip bindmobile-tip-yes">您已经成功解除了绑定!</div>
                            </div>
                        </div>
            <!--[/if]-->
                        </form>
                        
                     </div>
                    <!--#include file="../_inc/_round_bottom.aspx"-->
                </div>
            </div>
            <!--#include file="_sidebar_setting.aspx"-->
        </div>
    </div>
</div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
<!--[if ($ChangeMobileSecond || $UnBoundMobileFirst ||$BindingMobileSecond) && $IsOverrunLimitTryNum==false]-->
<script type="text/javascript">
    var agreementTime = 60;
    var iagree = document.getElementById('resend');
    function closeAgreementTime() {
        if (agreementTime == 0) {
            iagree.disabled = '';
            iagree.value = '未收到短信点击此处重发';
            iagree.parentNode.parentNode.className = 'minbtn-wrap';
        } else {
        iagree.value = "未收到短信" + agreementTime + "秒后可重发";
            iagree.parentNode.parentNode.className = 'minbtn-wrap minbtn-disable';
            agreementTime--;
            iagree.disabled = 'disabled';
            window.setTimeout("closeAgreementTime()", 1000);
        }
    }
    closeAgreementTime();
</script>
<!--[/if]-->
</body>
</html>
