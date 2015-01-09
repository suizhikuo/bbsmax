<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>群发的系统通知管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<div class="Content">
<!--[include src="../_setting_msg_.aspx"/]-->
    <div class="PageHeading">
    <h3>创建新的群发</h3>
    <div class="ActionsBar">
        <a href="manage-systemnotify.aspx"  class="back"><span>返回群发的系统通知管理</span></a>
    </div>
    </div>
    <form id="form2" action="$_form.action" method="post">
<div class="FormTable">
	<table>
		<tr>
			<th>
			    <h4>群发的通知标题</h4>
			    <input name="subject" id="subject" type="text" class="text" value="$_form.text('subject',$subject)" />
			</th>
			<td>标题并无实际含义， 只是方便管理员辨别通知用， 前台用户不会看到通知标题</td>
		</tr>
		<tr>
		<th>
		<h4>发送时间</h4>
		<input name="begindate" id="begindate" type="text" style=" width:10em" class="text" value="$_form.text('begindate',$begindate)" /><a title="选择日期" id="A0" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a> - 
		<input name="enddate" id="enddate" type="text" style=" width:10em" class="text" value="$_form.text('enddate',$enddate)" /><a title="选择日期" id="A1" class="selector-date" href="javascript:void(0)"><img src="$Root/max-assets/images/calendar.gif" alt="选择日期" /></a>
		</th>
        <td>发送时间</td>
		</tr>
        <tr>
            <th>
                <h4>内容</h4>
                <textarea name="content" id="content" style="height:200px; width:400px;">$_form.text('content',$content)</textarea>
            </th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <h4>针对用户发送</h4>
                <textarea name="usernames" style="height:80px;">$_form.text("usernames",$Usernames)</textarea>
            </th>
            <td>请在输入框中输入接收通知的用户名，每行一个，不正确的用户名将被忽略</td>
        </tr>
		<tr>
            <th>
                <h4>用户组——管理员组</h4>
                <!--[loop $role in $ManagerRoles]-->
                <div style=" float:left; width:140px;">
                <input type="checkbox" name="roles" value="$role.roleid" $_form.checked('roles',$role.roleid,$IsChecked($role.roleid)) id="role_$role.roleid" /><label for="role_$role.roleid">$role.name</label>
                </div>
                <!--[/loop]-->
            </th>
            <td>请勾上接收通知的管理员用户组</td>
        </tr>
        <tr>
            <th>
                <h4>用户组——自定义组</h4>
                <div style="float:left; width:140px;">
                <input type="checkbox" name="roles" value="$UsersRole.roleid" $_form.checked('roles',$UsersRole.roleid,$IsChecked($UsersRole.roleid)) id="role_$UsersRole.roleid" /><label for="role_$UsersRole.roleid">$UsersRole.name</label>
                </div>
                <!--[loop $role in $basicRoles]-->
                <div style=" float:left; width:140px;">
                <input type="checkbox" name="roles" value="$role.roleid" $_form.checked('roles',$role.roleid,$IsChecked($role.roleid)) id="role_$role.roleid" /><label for="role_$role.roleid">$role.name</label>
                </div>
                <!--[/loop]-->
            </th>
            <td>请勾上接收通知的特殊用户组</td>
    <tr>
            <th>
                <h4>用户组——等级用户组</h4>
                <!--[loop $role in $levelroles]-->
                <div style=" float:left; width:140px;">
                <input type="checkbox" name="roles" value="$role.roleid" $_form.checked('roles',$role.roleid,$IsChecked($role.roleid)) id="role_$role.roleid" /><label for="role_$role.roleid">$role.name</label>
                </div>
                <!--[/loop]-->
            </th>
            <td>请勾上接收通知的等级用户组</td>
        </tr>
		<tr class="nohover">
		    <th>
		        <input type="submit" name="savenotify" value="立即群发" class="button" />
		    </th>
		    <td>&nbsp;</td>
	    </tr>
	</table>
    </div>
    </form>
</div>

<!--[include src="../_foot_.aspx" /]-->
<script type="text/javascript" src="$root/max-assets/nicedit/nicEdit.js"></script>
<script type="text/javascript">
        initMiniEditor("content");
        initDatePicker('begindate', 'A0');
        initDatePicker('enddate', 'A1');
</script>
</body>
</html>
