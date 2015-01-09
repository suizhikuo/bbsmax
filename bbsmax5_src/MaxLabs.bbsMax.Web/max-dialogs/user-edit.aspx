<!--[DialogMaster title="修改用户资料" width="700" ]-->
<!--[place id="body"]-->
<!--[UserView userid="$_getint.id"]-->

<!--#include file="_error_.ascx" -->
<!--[success]-->
<div class="dialogmsg dialogmsg-success">用户资料修改成功</div>
<!--[/success]-->

<!--#include file="_tab_userinfo_.aspx" tab="profile" -->

<form id="form1" method="post" action="$_form.action">
<div class="clearfix dialogbody">
    <div class="scroller" style="height:300px;">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">用户名</h3>
            <div class="form-enter">
                <input type="text" class="text" value="$user.username" disabled="disabled" />
                <!--[error name="username"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">头像</h3>
            <div class="form-enter">
                $user.Avatar
                <!--[if !$user.IsDefaultAvatar]-->
                <a onclick="return confirm('确定删除该用户的头像吗?');form1.submit();">清除头像</a>
                <!--[/if]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="isactive">账号已激活</label></h3>
            <div class="form-enter">
                <input id="isactive" type="checkbox" value="true" name="isActive" $_form.checked("isActive","true",$user.isActive) />
                <label for="isactive">账号已激活</label>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="realname">真实姓名</label></h3>
            <div class="form-enter">
                <input id="realname" name="realname" type="text" class="text" value="$_form.text('realname',$user.realname)" />
                <!--[error name="realname"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <div class="formrow">
            <h3 class="label"><label for="email">Email</label></h3>
            <div class="form-enter">
                <input id="email" name="email" type="text" class="text" value="$_form.text('email',$user.Email)" />
                <!--[error name="email"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
        <!--[if $ShowEmailCheck]-->
        <div class="formrow">
            <h3 class="label"><label for="emailvalidated">Email通过验证</label></h3>
            <div class="form-enter">
                <input id="emailvalidated" $_form.checked('emailvalidated','true',$user.emailvalidated) type="checkbox" value="true" name="emailvalidated" />
                <label for="emailvalidated">通过验证</label>
            </div>
        </div>
        <!--[/if]-->
        <div class="formrow">
            <h3 class="label">出生日期</h3>
            <div class="form-enter">
                <select id="Select1" name="birthyear">
                    <option value="0">年</option>
                    <!--[loop $varYear in $years]-->
                    <option value="$varYear" $_form.selected("birthyear","$varYear",$varyear==$user.birthday.Year)>$varYear</option>
                    <!--[/loop]-->
                </select>
                <select id="Select2" name="birthmonth">
                    <option value="0">月</option>
                    <!--[loop $varMonth in $months]-->
                    <option value="$varMonth" $_form.selected("birthmonth","$varMonth",$varmonth==$user.birthday.month)>$varMonth</option>
                    <!--[/loop]-->
                </select>
                <select id="Select3" name="birthday">
                    <option value="0">日</option>
                    <!--[loop $varDay in $days]-->
                    <option value="$varDay" $_form.selected("birthday","$varday",$varday==$user.birthday.day)>$varDay</option>
                    <!--[/loop]-->
                </select>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">性别</h3>
            <div class="form-enter">
                <input type="radio" $_form.checked("gender","0",$user.gender==MaxLabs.bbsMax.Enums.Gender.NotSet) id="noset" name="gender" value="0" /> <label for="noset">保密</label>
                <input type="radio" $_form.checked("gender","1",$user.gender==MaxLabs.bbsMax.Enums.Gender.Male) id="male" value="1" name="gender" /> <label for="male">男</label>
                <input type="radio" $_form.checked("gender","2",$user.gender==MaxLabs.bbsMax.Enums.Gender.Female) id="female" value="2" name="gender" /> <label for="female">女</label>
            </div>
        </div>
        <!--[UserExtendedFieldList userID="$user.userID"]-->
        <div class="formrow">
            <h3 class="label">$Name</h3>
            <div class="form-enter">
                <!--[load src="$fieldType.FrontendControlSrc" value="$userValue" field="$_this" /]-->
                <!--[if $DisplayType == ExtendedFieldDisplayType.AllVisible]-->
                (所有人可见)
                <!--[else if $DisplayType == ExtendedFieldDisplayType.FriendVisible]-->
                (仅好友可见)
                <!--[else if $DisplayType == ExtendedFieldDisplayType.SelfVisible]-->
                (仅自己可见)
                <!--[else]-->
                <select name="{=$Key}_displaytype">
                <option value="0" $_form.selected("{=$Key}_displaytype","0",$PrivacyType)>所有人可见</option>
                <option value="1" $_form.selected("{=$Key}_displaytype","1",$PrivacyType)>仅好友可见</option>
                <option value="2" $_form.selected("{=$Key}_displaytype","2",$PrivacyType)>仅自己可见</option>
                </select>
                <!--[/if]-->
                <!--[error name="$Key"]--><span class="form-tip tip-error">$message</span><!--[/error]-->
            </div>
        </div>
        <!--[/UserExtendedFieldList]-->
        <div class="formrow">
            <h3 class="label"><label for="signature">签名</label></h3>
            <div class="form-enter">
                <textarea name="signature" id="signature" cols="30" rows="4">$_form.text('signature',$ParsedSignature)</textarea>
                <!--[error name="signature"]-->
                <span class="form-tip tip-error">$message</span>
                <!--[/error]-->
            </div>
        </div>
    </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" accesskey="s" name="save"><span>保存(<u>S</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[if $ImportEditor]-->
<!--script type="text/javascript" src="$root/max-assets/kindeditor/kindeditor.js"></script-->
<!--[/if]-->
<!--[if $ImportEmoticonLib]-->
<!--script type="text/javascript" src="$url(handler/emotelib)?userid=$user.userid&action=userinfo"></script-->
<!--[/if]-->
<!--[if $ImportEditor]-->
<script type="text/javascript">
//function initEditor(config) {
//    KE.init(config);
//    KE.create( config.id);
//}

//addPageEndEvent(function(){
//<!--[if SignatureAllowHtml]-->
//KE.util.setCurrentMode('signature','html');
//initEditor({
//id: 'signature',
//action: "user",
//targetID: $user.userid,
//skinType: 'max_editor',
//resizeMode:1,
//cssPath: '$Root/max-assets/kindeditor/editor.css',
//items: [
//    'fontname', 'fontsize', 'textcolor', 'bgcolor', 'bold',
//    'italic', 'underline', 'strikethrough',
//    'justifyleft', 'justifycenter', 'justifyright',
//    'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent',
//    'removeformat', 'fullscreen', '-',
//    'undo', 'redo', 'plainpaste', 'wordpaste',
//    'link', 'unlink', 'table', 'emoticons',
//    'image', 'flash', 'media',
//    'code', 'hide', 'quote', 'free'
//] 
//});
//<!--[else]-->
//KE.util.setCurrentMode('signature','ubb');
//initEditor({
//id: 'signature',
//action:"user",
//targetID:$user.userid,
//skinType: 'max_editor',
//resizeMode:1,
//cssPath: '$Root/max-assets/kindeditor/editor.css',
//items: [
//    'fontname', 'fontsize', 'textcolor', 'bgcolor', 'bold',
//    'italic', 'underline', 'strikethrough',
//    'justifyleft', 'justifycenter', 'justifyright',
//    'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent',
//    'removeformat', '-',
//     $_if($TagSettings.AllowUrl,"'link', 'unlink',") $_if($TagSettings.AllowTable,"'table',") $_if($ImportEmoticonLib,"'emoticons',")
//    $_if($TagSettings.AllowImage,"'image',") $_if($TagSettings.AllowFlash,"'flash',") $_if($TagSettings.AllowVideo||$TagSettings.AllowAudio,"'media',")'-'
//],
//onCtrlEnter: function(){}
//});
//<!--[/if]-->
    //});
</script>
<!--[/if]-->
<!--[/UserView]-->
<!--[/place]-->
<!--[/dialogmaster]-->