<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>AJAX功能设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置AJAX功能</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	    <table>
		    <tr>
			    <th>
			        <h4>注册和登陆时使用AJAX</h4>
				    <p><input type="radio" name="EnableAjaxOnRegisterAndLogin" id="EnableAjaxOnRegisterAndLogin1" checked="checked"/> <label for="EnableAjaxOnRegisterAndLogin1">使用</label></p>
				    <p><input type="radio" name="EnableAjaxOnRegisterAndLogin" id="EnableAjaxOnRegisterAndLogin2"/> <label for="EnableAjaxOnRegisterAndLogin2">禁用</label></p>
			    </th>
			    <td>&nbsp;</td>
		    </tr>
		    <tr>
			    <th>
			        <h4>发表主题和回复主题时使用AJAX</h4>
				    <p><input type="radio" name="PostUseAjax" value="true" id="PostUseAjax1" $_form.checked('PostUseAjax','true',$AjaxSettings.PostUseAjax)/> <label for="PostUseAjax1">使用</label></p>
				    <p><input type="radio" name="PostUseAjax" value="false" id="PostUseAjax2"  $_form.checked('PostUseAjax','false',!$AjaxSettings.PostUseAjax)/> <label for="PostUseAjax2">禁用</label></p>
			    </th>
			    <td>&nbsp;</td>
		    </tr>
		    <tr>
			    <th>
			        <h4>快速发表主题和快速回复主题时使用AJAX</h4>
				    <p><input type="radio" name="QuicklyPostUseAjax" value="true" id="QuicklyPostUseAjax1" $_form.checked('QuicklyPostUseAjax','true',$AjaxSettings.QuicklyPostUseAjax)/> <label for="QuicklyPostUseAjax1">使用</label></p>
				    <p><input type="radio" name="QuicklyPostUseAjax" value="false" id="QuicklyPostUseAjax2" $_form.checked('QuicklyPostUseAjax','false',!$AjaxSettings.QuicklyPostUseAjax)/> <label for="QuicklyPostUseAjax2">禁用</label></p>
			    </th>
			    <td>&nbsp;</td>
		    </tr>
		    <tr>
			    <th>
			        <input type="submit" value="保存设置" class="button" name="savesetting" />
			    </th>
			    <td>&nbsp;</td>
		    </tr>
	    </table>
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
