<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户登录设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置登录功能</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
        <table>
             <tr>
                 <th>
                    <h4>用户登录模式</h4>
                    <p><input type="radio" name="LoginType" id="Logintype1" value="Username" $_form.checked('LoginType','Username',($LoginSettings.LoginType == UserLoginType.Username)) /><label for="Logintype1">账号登录</label> </p>
                    <p><input type="radio" name="LoginType" id="Logintype2" value="Email" $_form.checked('LoginType','Email', ($LoginSettings.LoginType == UserLoginType.Email) ) /><label for="Logintype2">邮箱登录</label>
                       <span style="color:Red">当设置邮箱登录以后,注册模式会自动选择"注册就要求验证, 通过验证Email激活账号",且不可更改;</span>
                     </p>
                    <p><input type="radio" name="LoginType" id="Logintype3" value="All" $_form.checked('LoginType','All', ($LoginSettings.LoginType == UserLoginType.All) ) /><label for="Logintype3">两者皆提供,供用户选择</label> </p>
                 </th>
        
            </tr>
            <tr>
                 <th>
                    <h4>登录方式</h4>
                    <p><input type="radio" name="UseDialog" id="UseDialog1" value="false" $_form.checked('UseDialog','false',($LoginSettings.usedialog==false)) /><label for="UseDialog1">跳转登录</label> </p>
                    <p><input type="radio" name="UseDialog" id="UseDialog2" value="true" $_form.checked('UseDialog','true', ($LoginSettings.usedialog) ) /><label for="UseDialog2">对话框登录</label> </p>
            </th>
            </tr>
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
<!--[include src="../_foot_.aspx"/]-->    
</body>
</html>
