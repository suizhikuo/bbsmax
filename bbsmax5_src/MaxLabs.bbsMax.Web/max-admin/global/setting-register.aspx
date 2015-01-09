<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>注册功能设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->

<script type="text/javascript">
    function insertScope(s) {
        showTable(true);
        var l = $("scope_list");
        var id = l.rows.length;
        var r = l.insertRow(id);

        r.id = "item_" + s.ID;
        
        id = 0;
        var c = r.insertCell(id++);
        c.innerHTML = s.Message;
        c=r.insertCell(id++);
        c.innerHTML = s.OperetorName;
        c=r.insertCell(id++);
        c.innerHTML = s.OperetingDatetime;
        c = r.insertCell(id++);
        c.innerHTML = String.format('<a href="$dialog/timing-delete.aspx?type=1&scopeid={0}" onclick="return openDialog(this.href,function(){ removeElement($(\'item_{0}\'))})">删除</a>', s.ID); 
    }

    function showTable(b) {
        var s = $('DataTable');
        var e = $('emptydata');
        if (b == true) {
            s.style.display = "";
            e.style.display = "none";
        }
        else {
            s.style.display = "none";
            e.style.display = "";
        }
    }

</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置注册功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table style="margin-bottom:1px;">
        <!--[error name="EnableRegister"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>开启注册</h4>
                <p><input type="radio" name="EnableRegister" id="EnableRegister1" value="0" $_form.checked('EnableRegister','0',($RegisterSettings.EnableRegister==RegisterMode.Open)) /> <label for="EnableRegister1">开放注册</label></p>
                <p><input type="radio" name="EnableRegister" id="EnableRegister2" value="1" $_form.checked('EnableRegister','1',($RegisterSettings.EnableRegister==RegisterMode.Closed)) /> <label for="EnableRegister2">关闭注册</label></p>
                <p><input type="radio" name="EnableRegister" id="EnableRegister3" value="2" $_form.checked('EnableRegister','2',($RegisterSettings.EnableRegister==RegisterMode.TimingClosed)) /> <label for="EnableRegister3">定时关闭注册</label></p>
            </th>
            <td>当从其他模式切换到"定时关闭注册"模式时,进行添加删除操作,要点击"保存设置",才能生效.</td>
        </tr>
        <tr id="Timinglist">
            <td colspan="4">
                <h4>&nbsp;&nbsp;定时关闭注册时间列表 <a id="timing" href="$dialog/timing-add.aspx?type=1" onclick="return openDialog(this.href,function( s ){insertScope(s);})" > 添加时间范围</a> 	</h4>	        
            
            <div id="emptydata">当前没有指定定时范围</div>
                <table id="DataTable" class="DataTable"> 
                 <thead>
                     <tr>
                       <td>定时范围 </td>
                       <td>操作者  </td>
                       <td>添加时间  </td>
                       <td>操作</td>
                    </tr>
                </thead>
              <tbody id="scope_list">
                <!--[loop $item in $ScopeItemList]-->
                 <tr id="item_$item.id">
                   <td>$item.Message </td>
                   <td>$item.OperetorName </td>
                   <td>$item.OperetingDatetime </td>
                   <td><a href="$dialog/timing-delete.aspx?type=1&scopeid=$item.id" onclick="return openDialog(this.href,function(){ removeElement($('item_$item.id'))})" >删除</a></td>
                 </tr>
                <!--[/loop]-->
              </tbody>   
             </table>
             <!--[if $ListIsEmpty]-->
            <script type="text/javascript">		        showTable(false);</script>    
            <!--[else]-->
            <script type="text/javascript">                showTable(true);</script>
            <!--[/if]--> 
              
            </td>
               
            
        </tr>
        
        <!--[error name="ClosedMessage"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="mode_closed">
            <th>
                <h4>关闭系统原因</h4>
                <textarea name="ClosedMessage" cols="30" rows="6" style="height:200px;width:550px;">$RegisterSettings.ClosedMessage</textarea>
            </th>
            <td>&nbsp;</td>
        </tr>
    </table>
    <table id="mode_WelcomeMail">
    <tbody>
    <tr>
    <th>
    <h4>发送新用户欢迎邮件</h4>
    <p><input type="radio" name="EnableWelcomeMail" id="EnableWelcomeMail1" value="true" $_form.checked('EnableWelcomeMail','true',$RegisterSettings.EnableWelcomeMail) /> <label for="EnableWelcomeMail1">开启</label></p>
    <p><input type="radio" name="EnableWelcomeMail" id="EnableWelcomeMail2" value="false" $_form.checked('EnableWelcomeMail','false',!$RegisterSettings.EnableWelcomeMail) /> <label for="EnableWelcomeMail2">关闭</label></p>
    </th>
    <td>&nbsp;</td>
    </tr>
       <!--[error name="WelcomeMailTitle"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="tr_welcomeTitle">
            <th>
                <h4>新用户欢迎邮件标题</h4>
                <input type="text" class="text" name="WelcomeMailTitle" value="$_form.text('WelcomeMailTitle',$RegisterSettings.WelcomeMailTitle)" />
            </th>
            <td>&nbsp;</td>
        </tr>
       <!--[error name="WelcomeMailContent"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="tr_welcomeContent">
            <th>
                <h4>新用户欢迎邮件内容</h4>
                <div class="htmleditorwrap">
                    <textarea name="WelcomeMailContent" style="height:200px;width:550px;">$_form.text('WelcomeMailContent',$RegisterSettings.WelcomeMailContent)</textarea>
                </div>
            </th>
            <td>&nbsp;</td>
        </tr>
    </tbody>
    </table>

    <table id="canReg" style="margin-bottom:1px;">
        <tr>
            <th>
                <h4>注册限制</h4>
                <a href="$admin/global/setting-registerlimit.aspx" title="设置注册限制">前往设置</a>
            </th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <h4>邀请或推广模式</h4>
                <!--[if $InvitationSettings.InviteMode == InviteMode.Close]-->
                未开启
                <!--[else if $InvitationSettings.InviteMode == InviteMode.InviteSerialRequire]-->
                已开启，只能通过邀请码注册
                <!--[else if $InvitationSettings.InviteMode == InviteMode.InviteSerialOptional]-->
                已开启，可以通过邀请码注册，但不是必需的
                <!--[else if $InvitationSettings.InviteMode == InviteMode.InviteLinkRequire]-->
                已开启，只能通过推广链接注册
                <!--[else if $InvitationSettings.InviteMode == InviteMode.InviteLinkOptional]-->
                已开启，可以通过推广链接注册，但不是必需的
                <!--[/if]-->  <a href="$admin/global/setting-invitation.aspx" title="设置邀请功能">前往设置</a>
            </th>
            <td>&nbsp;</td>
        </tr>
       <!--[error name="NewUserPracticeTime"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>新注册用户见习时间(分钟)</h4>
                <input type="text" class="text number" name="NewUserPracticeTime" value="$_form.text('NewUserPracticeTime',$RegisterSettings.NewUserPracticeTime)" />
            </th>
            <td>&nbsp;</td>
        </tr>
       <!--[error name="EmailVerifyMode"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>验证用户Email的真实性</h4>
                <!--[if !$EmailSettings.EnableSendEmail]-->
                <p>当前系统邮件发送功能已经关闭. <a href="$admin/global/setting-email.aspx">转到邮件设置</a></p>
                <!--[/if]-->
                <!--[if $LoginSettings.LoginType == UserLoginType.Email]-->
                <p><input type="radio" name="EmailVerifyMode" id="EmailVerifyMode1" disabled="disabled" value="Disabled" $_form.checked('EmailVerifyMode','Disabled',$RegisterSettings.EmailVerifyMode== EmailVerifyMode.Disabled) /> <label for="EmailVerifyMode1">不验证</label></p>
                <p><input type="radio" name="EmailVerifyMode" id="EmailVerifyMode2" disabled="disabled" value="Required" $_form.checked('EmailVerifyMode','Required',$RegisterSettings.EmailVerifyMode== EmailVerifyMode.Required) /> <label for="EmailVerifyMode2">注册就要求验证, 通过验证Email激活账号</label></p>
                <p><input type="radio" name="EmailVerifyMode" id="EmailVerifyMode3" disabled="disabled" value="Optional" $_form.checked('EmailVerifyMode','Optional',$RegisterSettings.EmailVerifyMode== EmailVerifyMode.Optional) /> <label for="EmailVerifyMode3">注册时不验证, 用户可登录后再验证.</label></p>
                <br /><span style="color:Red">当登录设置为"邮箱登录"模式以后,该选项会自动选择"注册就要求验证, 通过验证Email激活账号",且不可更改;</span>
                <input type="hidden" name="EmailVerifyMode" value="Required" />
                <!--[else]-->
                <p><input type="radio" name="EmailVerifyMode" id="EmailVerifyMode1" value="Disabled" $_form.checked('EmailVerifyMode','Disabled',$RegisterSettings.EmailVerifyMode== EmailVerifyMode.Disabled) /> <label for="EmailVerifyMode1">不验证</label></p>
                <p><input type="radio" name="EmailVerifyMode" id="EmailVerifyMode2" value="Required" $_form.checked('EmailVerifyMode','Required',$RegisterSettings.EmailVerifyMode== EmailVerifyMode.Required) /> <label for="EmailVerifyMode2">注册就要求验证, 通过验证Email激活账号</label></p>
                <p><input type="radio" name="EmailVerifyMode" id="EmailVerifyMode3" value="Optional" $_form.checked('EmailVerifyMode','Optional',$RegisterSettings.EmailVerifyMode== EmailVerifyMode.Optional) /> <label for="EmailVerifyMode3">注册时不验证, 用户可登录后再验证.</label></p>
                <!--[/if]-->
            </th>
            <td>注意: 通过验证Email激活账号, 在未激活账号之前无法登陆, 而且账号会在24小时后自动清理.</td>
        </tr>
    <tbody id="mode_email">
       <!--[error name="ActivationExpiresDate"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>发送Email后过期时间(小时)</h4>
                <input class="text" type="text" name="ActivationExpiresDate" value="$_form.text('ActivationExpiresDate',$RegisterSettings.ActivationExpiresDate)" onkeyup="value=value.replace(/[^\d]/g,'')" />
            </th>
            <td>&nbsp;</td>
        </tr>
       <!--[error name="ActivationEmailTitle"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="tr_activationEmailTitle">
            <th>
                <h4>用户帐号激活邮件标题</h4>
                <input type="text" class="text" name="ActivationEmailTitle" value="$_form.text('ActivationEmailTitle',$RegisterSettings.ActivationEmailTitle)" />
            </th>
            <td>&nbsp;</td>
        </tr>
       <!--[error name="ActivationEmailContent"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="tr_activationEmailContent">
            <th>
                <h4>验证用户帐号激活邮件内容</h4>
                <div class="htmleditorwrap">
                <textarea name="ActivationEmailContent" style="height:200px;width:550px;">$_form.text('ActivationEmailContent',$RegisterSettings.ActivationEmailContent)</textarea>
                </div>
            </th>
            <td>&nbsp;</td>
        </tr>
    </tbody>
    <tbody id="mode_emailvalidate">
       <!--[error name="EmailValidationTitle"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>邮箱验证邮件标题</h4>
                <p><input type="text" name="EmailValidationTitle" class="text" value="$_form.text('EmailValidationTitle',$RegisterSettings.EmailValidationTitle)" />  </p>
            </th>
            <td>&nbsp;</td>
            </tr>
       <!--[error name="EmailValidationContent"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th><h4>邮箱验证邮件内容</h4>
            <textarea name="EmailValidationContent" style="height:200px;width:550px;">$_form.text("EmailValidationContent",$RegisterSettings.EmailValidationContent)</textarea>
            </th>
             <td>&nbsp;</td>
        </tr>
    </tbody>
        <!--[error name="DisplayLicenseMode"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>显示注册协议</h4>
                <p><input type="radio" name="DisplayLicenseMode" id="DisplayLicenseMode3" value="None" $_form.checked('DisplayLicenseMode','None', $RegisterSettings.DisplayLicenseMode==LicenseMode.None) /><label for="DisplayLicenseMode3">不显示</label></p>
                <p><input type="radio" name="DisplayLicenseMode" id="DisplayLicenseMode1" value="Independent" $_form.checked('DisplayLicenseMode','Independent', $RegisterSettings.DisplayLicenseMode==LicenseMode.Independent) /><label for="DisplayLicenseMode1">在单独页面显示</label></p>
                <p><input type="radio" name="DisplayLicenseMode" id="DisplayLicenseMode2" value="Embed" $_form.checked('DisplayLicenseMode','Embed', $RegisterSettings.DisplayLicenseMode==LicenseMode.Embed) /><label for="DisplayLicenseMode2">在注册页面显示</label></p>
            </th>
            <td>&nbsp;</td>
        </tr>
    <tbody id="RegisterLicense">
       <!--[error name="RegisterLicenseDisplayTime"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="RegisterLicenseDisplayTime">
            <th>
                <h4>注册协议显示时间（秒）</h4>
                <input type="text" class="text number" name="RegisterLicenseDisplayTime" value="$_form.text('RegisterLicenseDisplayTime',$RegisterSettings.RegisterLicenseDisplayTime)" onkeyup="value=value.replace(/[^\d]/g,'')" />
            </th>
            <td>&nbsp;</td>
        </tr>
        <!--[error name="RegisterLicenseContent"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr id="RegisterLicense">
            <th>
                <h4>注册协议内容</h4>
                <div class="htmleditorwrap">
                <textarea name="RegisterLicenseContent" cols="30" rows="6" style="height:200px;width:550px;">$_form.text('RegisterLicenseContent',$RegisterSettings.RegisterLicenseContent)</textarea>
                </div>
            </th>
            <td>&nbsp;</td>
        </tr>
    </tbody>
    </table>

    <table>
        <tr class="nohover">
            <th><input type="submit" value="保存设置" class="button" name="savesetting" /></th>
            <td>&nbsp;</td>
        </tr>
    </table>
    
    </div>
    </form>
</div>
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>
<script type="text/javascript">
    initDisplay("EmailVerifyMode", [
     { value: "Disabled", display: false, id: 'mode_email' }
    , { value: "Disabled", display: false, id: 'mode_emailvalidate' }
    , { value: "Disabled", display: true, id: "mode_WelcomeMail" }
    , { value: "Required", display: true, id: 'mode_email' }
    , { value: "Required", display: false, id: 'mode_emailvalidate' }
    , { value: "Required", display: false, id: "mode_WelcomeMail" }
    , { value: "Optional", display: false, id: 'mode_email' }
    , { value: "Optional", display: true, id: 'mode_emailvalidate' }
    , { value: "Optional", display: true, id: "mode_WelcomeMail" }
]);
     initDisplay('EnableRegister', [
     { value: "1", display: true, id: 'mode_closed' }
     , { value: "0", display: false, id: 'mode_closed' }
     , { value: "2", display: true, id: 'mode_closed' }
     , { value: "0", display: false, id: 'Timinglist' }
     , { value: "1", display: false, id: 'Timinglist' }
     , { value: "2", display: true, id: 'Timinglist' }
     , { value: "1", display: false, id: 'canReg' }
     , { value: "2", display: false, id: 'canReg' }
     , { value: "0", display: true, id: 'canReg'}]);
     
     initDisplay('EnableWelcomeMail', [
     { value: "false", display: false, id: 'tr_welcomeTitle' }
     , { value: "true", display: true, id: 'tr_welcomeTitle' }
     , { value: "false", display: false, id: 'tr_welcomeContent' }
     , { value: "true", display: true, id: 'tr_welcomeContent' }
     ]);

initDisplay('DisplayLicenseMode', [
 {value:"None", display:false,  id:'RegisterLicense'}
,{value:"Independent", display:true,   id:'RegisterLicense'}
,{value:"Embed", display:true,   id:'RegisterLicense'}
,{value:"None", display:false,  id:'RegisterLicenseDisplayTime'}
,{value:"Independent", display:true,   id:'RegisterLicenseDisplayTime'}
,{value:"Embed", display:false,  id:'RegisterLicenseDisplayTime'}
]);
//initDisplay('DisplayLicenseMode',[]);
addPageEndEvent( function(){initMiniEditor(editorToolBar.setting);} );
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
