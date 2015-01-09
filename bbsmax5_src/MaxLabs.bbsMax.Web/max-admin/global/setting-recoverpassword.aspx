<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>找回密码功能设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>
<script type="text/javascript">
addPageEndEvent( function(){initMiniEditor(editorToolBar.setting);} );
initDisplay('Enable',[{value : 'true' , display : true , id : 'passwordSetting'},{value : 'false' , display : false , id : 'passwordSetting'}]);
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置找回密码功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table>
        <!--[error name="Enable"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>开启注册</h4>
                <p><input type="radio" name="Enable" id="Enable1" value="true" $_form.checked('Enable','true',$RecoverPasswordSettings.Enable) /> <label for="Enable1">开放找回密码功能</label></p>
                <p><input type="radio" name="Enable" id="Enable2" value="false" $_form.checked('Enable','false',!$RecoverPasswordSettings.Enable) /> <label for="Enable2">关闭找回密码功能</label></p>
            </th>
            <td>&nbsp;</td>
        </tr>
        <tbody id="passwordSetting">
        <!--[error name="EmailTitle"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>找回密码邮件标题</h4>
                <input type="text" class="text" name="EmailTitle" value="$_form.text('EmailTitle',$RecoverPasswordSettings.EmailTitle)" />
            </th>
            <td></td>
        </tr>
        <!--[error name="EmailContent"]-->
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>找回密码邮件内容</h4>
                <div class="htmleditorwrap">
                <textarea name="EmailContent" style="width:550px;height:200px;">$_form.text('EmailContent',$RecoverPasswordSettings.EmailContent)</textarea>
                </div>
            </th>
            <td></td>
        </tr>
        </tbody>
        <tr>
            <th class="nohover">
            <input type="submit" value="保存设置" class="button" name="savesetting" />
            </th>
            <td>&nbsp;</td>
        </tr>
    </table>
    </div>
    </form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>