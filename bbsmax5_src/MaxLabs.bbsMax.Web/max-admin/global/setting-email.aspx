<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Email功能设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
initDisplay('EnableSendEmail',[{ value :'true' ,display : true , id : 'emailSettings'}
,{ value :'false' ,display : false , id : 'emailSettings'}
,{ value :'true' ,display : true , id : 'append_SmtpServer'}
,{ value :'false' ,display : false , id : 'append_SmtpServer'}
]);

function openTest(id,isnew)
{
    var t = isnew?".new."+id:"."+id;
    var url='$dialog/testemail.aspx';
    var email=$$('senderemail'+t)[0].value;
    var smtp=$$('smtpserver'+t)[0].value;
    var pwd=$$('smtpserverpassword'+t)[0].value;
    var username = $$('smtpserveraccount' + t)[0].value;
    var enablessl = $$('enablessl' + t)[0].checked;


    var reg=new RegExp("^(\\w*[-_.]?[a-zA-Z0-9]+)+@([\\w-]+\\.)+[a-zA-Z]{2,}$","ig");
    var port = $$("port"+t)[0].value;
    if(email.trim()=="")
    {
        alert("请输入邮箱地址!"); 
        return false;
    }
    else
    {
        if(reg.test(email.trim())==false)
        {
            alert("您输入的邮箱地址不符合规范！");
            return false;
        }
    }
    if(pwd.trim()=="")
    {
        alert("请输入邮箱的密码");
        return false;
    }
    if(port.trim()=="")
    {
        alert("请输入SMTP服务器端口");
        return false;
    }
    if(username.trim()=="")
    {
    
        alert("请输入的邮箱用户名!");
        return false;
    }
    
    if(smtp.trim()=="")
    {
        alert("请输入SMTP服务器的地址！");
        return false;
    }
    
    url+="?email="+encodeURI(email);
    url+="&smtp="+encodeURI(smtp);
    url+="&username="+encodeURI(username);
    url+="&pwd="+encodeURI(pwd);
    url += "&port=" + port;
    url += "&ssl=" + enablessl;
    openDialog(url, function(r){});
}

function checkPort(chk,txtPortId) {
    var txt = $$(txtPortId)[0];
    txt.value = chk.checked ? 465 : 25;
}


function validateForm()
{
    var email=$$('sendersmail')[0].value;
    var smtp=$$('SmtpServer')[0].value;
    var pwd=$$('SmtpServerPassword')[0].value;
    var username=$$('SmtpServerAccount')[0].value;
    var port = $$("port")[0].value;
    var reg=new RegExp("^(\\w*[-_.]?[a-zA-Z0-9]+)+@([\\w-]+\\.)+[a-zA-Z]{2,}$","ig");
    
    if(email.trim()=="")
    {
        alert("请输入您的邮箱地址!");
        return false;
    }
    else
    {
        if(reg.test(email.trim())==false)
        {
            alert("您输入的邮箱地址不符合规范！");
            return false;
        }
    }
    if(pwd.trim()=="")
    {
        alert("请输入您邮箱的密码");
        return false;
    }
    
    if(username.trim()=="")
    {
    
        alert("请输入您的邮箱用户名!");
        return false;
    }
    
    if(smtp.trim()=="")
    {
        alert("请输入SMTP服务器的地址！");
        return false;
    }
}
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置Email功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table style="margin-bottom:1px;">
        <!--[error name="EnableSendEmail"]-->
        <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>允许系统发送Email</h4>
                <p><input type="radio" name="EnableSendEmail" id="EnableSendEmail1" value="true" $_form.checked('EnableSendEmail','true',$EmailSettings.EnableSendEmail) /> <label for="EnableSendEmail1">允许</label></p>
                <p><input type="radio" name="EnableSendEmail" id="EnableSendEmail2" value="false" $_form.checked('EnableSendEmail','false',$EmailSettings.EnableSendEmail == false) /> <label for="EnableSendEmail2">禁止</label></p>
            </th>
            <td>
            <!--[if !$EmailSettings.EnableSendEmail]-->
                如果禁止系统发送邮件。那么，用户帐号激活、邮箱验证、取回密码、好友邀请等功能将不能正常使用。
            <!--[/if]--> &nbsp;
            </td>
        </tr>
    </table>

    <table class="multicolumn-datatable" id="emailSettings" style="margin-bottom:1px;">
        <thead>
        <tr>
            <td>SMTP服务器地址<span class="request" title="必填项">*</span></td>
            <td>Email<span class="request" title="必填项">*</span></td>
            <td>用户名<span class="request" title="必填项">*</span></td>
            <td>密码<span class="request" title="必填项">*</span></td>
            <td>使用SSL<span class="request" title="必填项">*</span></td>
            <td>端口</td>
            <td>可用操作</td>
        </tr>
        </thead>
        <tbody>
        <!--[loop $e in $Emails with $i]-->
        <!--[error line="$i"]-->
        <tr class="ErrorMessage" id="error$e.emailid">
            <td colspan="7" class="Message"><div class="Tip Tip-error">$message</div></td>
        </tr>
        <tr class="ErrorMessageArrow" id="errorarray$e.emailid">
            <td><!--[if $HasError("smtpserver")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("senderemail")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("smtpserveraccount")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("smtpserverpassword")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("port")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <!--[/error]-->
        <tr id="row-$e.emailid">
            <td><input type="hidden" name="emailids" value="$e.emailid" />
            <input type="text" class="text" maxlength="150" name="smtpserver.$e.emailid" value="$_form.text('SmtpServer.$e.emailid', $e.SmtpServer)" /></td>
            <td><input type="text" class="text" maxlength="150" name="senderemail.$e.emailid" value="$_form.text('SenderEmail.$e.emailid', $e.SenderEmail)" /></td>
            <td><input type="text" class="text" maxlength="50" name="smtpserveraccount.$e.emailid" value="$_form.text('SmtpServerAccount.$e.emailid', $e.SmtpServerAccount)" /></td>
            <td><input type="password" class="text" maxlength="50" name="smtpserverpassword.$e.emailid" value="$_form.text('SmtpServerPassword.$e.emailid', $e.SmtpServerPassword)" /></td>
            <td><input type="checkbox" value="true" $_form.checked("enablessl.$e.emailid","true",$e.enablessl) onclick="checkPort(this,'port.$e.emailid')" name="enablessl.$e.emailid" id="enablessl.$e.emailid" /></td>
            <td><input type="text" class="text number" maxlength="5" name="port.$e.emailid" value="$_form.text('port.$e.emailid', $e.port)" /></td>
            <td>
                <a href="javascript:void(openTest($e.emailid,0))">测试</a> | 
                <!--[if $e.isnew]-->
                <a href="javascript:void(cancelNewrow('$e.emailid'))">删除</a>
                <!--[else]-->
                <a href="javascript:;" onclick="removeElement(this.parentNode.parentNode)">删除</a>
                <!--[/if]-->
            </td>
        </tr>
        <!--[/loop]-->
        <tr id="newrow">
            <td>
            <input type="hidden" name="emailids" value="{0}" />
            <input type="text" class="text" maxlength="150" name="smtpserver.new.{0}" value="" /></td>
            <td><input type="text" class="text" maxlength="150" name="senderemail.new.{0}" value="" /></td>
            <td><input type="text" class="text" maxlength="50" name="smtpserveraccount.new.{0}" value="" /></td>
            <td><input type="password" class="text" maxlength="50" name="smtpserverPassword.new.{0}" value="" /></td>
            <td><input type="checkbox"  onclick="checkPort(this,'port.new.{0}')" value="true" name="enablessl.new.{0}" id="enablessl_{0}" /></td>
            <td><input type="text" class="text number" maxlength="5" name="port.new.{0}" value="25" /></td>
            <td>
                <a href="javascript:void(openTest({0},1))">测试</a> |
                <a href="javascript:;" id="deleteRow{0}">取消</a>
                 <input type="hidden" value="true" name="isnew.{0}" />
            </td>
        </tr>
        </tbody>
    </table>

    <table>
        <tr>
            <th>
            <input type="submit" value="保存设置" class="button" name="savesetting" />
            <input type="button" id="append_SmtpServer" class="button" value="添加邮件服务器" onclick="dt.insertRow();" />
            </th>
            <td>&nbsp;</td>
        </tr>
    </table>
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
<script type="text/javascript">
var dt=new DynamicTable("emailSettings","emailids");

//<!--[if $Emails.Count==0]-->
window.onload=function(){
dt.insertRow();
}
//<!--[/if]-->
</script>
</body>
</html>
