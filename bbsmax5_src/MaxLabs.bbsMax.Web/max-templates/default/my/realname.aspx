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
                            <h3 class="pagecaption-title"><span>实名认证</span></h3>
                        </div>
                        
                        <form  method="post" action="$_form.action" enctype="multipart/form-data">
                            <div class="formgroup realname-form">
                                <!--[unnamederror]-->
                                <div class="errormsg">$message</div>
                                <!--[/unnamederror]-->
                            <!--[if $HasAuthenticInfo]-->
                                <!--[if $AuthenticUser.Verified]-->
                                <div class="realname-tip realname-tip-yes">您的身份已通过验证</div>
                                <div class="formrow">
                                    <h3 class="label">真实姓名</h3>
                                    <div class="form-enter">
                                        $AuthenticUser.realname
                                    </div>
                                </div>
                                <div class="formrow">
                                    <h3 class="label">身份证号码</h3>
                                    <div class="form-enter">
                                        $IDCardNumber
                                    </div>
                                    <div class="form-note">以上信息只有您自己能看到，任何第三方人员都无法看到该信息。</div>
                                </div>
                                <!--[else if $AuthenticUser.processed==false]-->
                                <div class="realname-waiting">我们已经收到您的申请，工作人员将会尽快审核您的资料，请耐心等待。</div>
                                <div class="formrow">
                                    <h3 class="label">真实姓名</h3>
                                    <div class="form-enter">
                                        $AuthenticUser.realname
                                    </div>
                                </div>
                                <div class="formrow">
                                    <h3 class="label">身份证号码</h3>
                                    <div class="form-enter">
                                        $IDCardNumber
                                    </div>
                                    <div class="form-note">以上信息只有您自己能看到，任何第三方人员都无法看到该信息。</div>
                                </div>
                                <!--[else]-->
                                <div class="realname-tip realname-tip-no">您的实名认证被拒绝，原因：$AuthenticUser.remark </div>
                                <!--[/if]-->
                            <!--[else]-->
                                <div class="realname-tip realname-tip-no">您还未对帐号进行实名认证。认证后，您的账户将获得以下优势:</div>
                                <ul class="realname-note">
                                    <li>部分高级功能只对实名用户开放。</li>
                                    <li>将更容易找回您的帐号。</li>
                                    <li>提高信用级别，交易更受信任。</li>
                                </ul>
                            <!--[/if]-->
                            <!--[if $CanInputAuthenticInfo]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="realname">真实姓名</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="realname" id="realname" value="$_form.text("realname")"  maxlength="10" />
                                        <!--[error name="realname"]-->
                                        <span class="form-tip tip-error">$message</span>
                                        <!--[/error]-->
                                    </div>
                                </div>
                                <div class="formrow">
                                    <h3 class="label"><label for="idnumber">身份证号码</label></h3>
                                    <div class="form-enter">
                                        <input type="text" class="text" name="idnumber" id="idnumber" value="$_form.text("idnumber")" maxlength="18" />
                                        <!--[error name="idnumber"]-->
                                        <span class="form-tip tip-error">$message</span>
                                        <!--[/error]-->
                                    </div>
                                </div>
                                <!--[if $UploadCardImage]-->
                                <div class="formrow">
                                    <h3 class="label"><label for="idcardfileface">身份证图片(正面)</label></h3>
                                    <div class="form-enter">
                                        <input type="file" name="idcardfileface" id="idcardfileface" />
                                        <!--[error name="idcardfileface"]-->
                                        <span class="form-tip tip-error">$message</span>
                                        <!--[/error]-->
                                    </div>
                                    <div class="form-note">
                                        身份证扫描件文件最大不能超过2MB，格式为jpg, gif或png。如果用高像素数码相机拍摄，请用绘图软件裁剪到适合大小以后再上传。
                                    </div>
                                </div>
                                <div class="formrow">
                                    <h3 class="label"><label for="idcardfileback">身份证图片(背面)</label></h3>
                                    <div class="form-enter">
                                        <input type="file" name="idcardfileback" id="idcardfileback" />
                                        <!--[error name="idcardfileback"]-->
                                        <span class="form-tip tip-error">$message</span>
                                        <!--[/error]-->
                                    </div>
                                    <div class="form-note">
                                        身份证扫描件文件最大不能超过$AllowFileSize,格式为jpg, gif或png。如果用高像素数码相机拍摄，请用绘图软件裁剪到适合大小以后再上传。
                                    </div>
                                </div>
                                <!--[/if]-->
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap buttonicon-valid"><span class="btn"><input type="submit" name="submit" value="提交" class="button" /></span></span>
                                </div>
                            <!--[/if]-->
                            </div>
                        </form>
                        
                    </div>
                </div>
                <!--#include file="_sidebar_setting.aspx"-->
            </div>
            <!--#include file="../_inc/_round_bottom.aspx"-->
        </div>
    </div>
    </div>
</div>
<!--#include file="../_inc/_foot.aspx"-->
</body>
</html>
