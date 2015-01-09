<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户空间相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置用户空间功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table style="margin-bottom:1px;">
		<tr>
			<th>
			    <h4>是否允许游客访问用户空间</h4>
				<p><input type="radio" name="AllowGuestAccess" id="AllowGuestAccess1" value="True" $_form.checked('AllowGuestAccess','True',$SpaceSettings.AllowGuestAccess == true) /> <label for="AllowGuestAccess1">允许</label></p>
				<p><input type="radio" name="AllowGuestAccess" id="AllowGuestAccess2" value="False" $_form.checked('AllowGuestAccess','False',$SpaceSettings.AllowGuestAccess == false) /> <label for="AllowGuestAccess2">禁止</label></p>
			</th>
			<td>禁止游客访问用户空间将导致未登录用户无法访问用户空间，也将导致搜索引擎无法抓取用户空间内容。</td>
		</tr>
        </table>
        <table>
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
