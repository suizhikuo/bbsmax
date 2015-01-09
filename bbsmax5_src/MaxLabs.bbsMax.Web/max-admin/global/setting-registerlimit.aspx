<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>注册限制设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript">
initDisplay('RegisterIPLimitMode',[
 { value : 'Free' ,   display : false , id : 'RegisterIPLimitList'}
,{ value : 'Allow' ,  display : true  , id : 'RegisterIPLimitList'}
,{ value : 'Reject' , display : true  , id : 'RegisterIPLimitList'}]);

initDisplay('RegisterEmailLimitMode',[
 {value : 'Reject', display : true   ,  id : 'RegisterEmailLimitList'}
,{value : 'Free',   display : false  ,  id : 'RegisterEmailLimitList'}
,{value : 'Allow',  display : true   ,  id : 'RegisterEmailLimitList'}]);
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置注册限制</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table>
        <!--[error name="UserNameLengthScope"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>用户名长度范围</h4>
				<input type="text" class="text" name="UserNameLengthScope" value="$_form.text('UserNameLengthScope',$RegisterLimitSettings.UserNameLengthScope)" />
			</th>
			<td>格式: 2~20</td>
		</tr>
        <!--[error name="AllowUsernames"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>允许以下字符作为用户名</h4>
				<p><input type="checkbox" name="AllowUsernames" id="AllowUsernames1" $_form.checked('AllowUsernames','Chinese',$RegisterLimitSettings.AllowUsernames.CanUseChinese) value="Chinese" /> <label for="AllowUsernames1">中文字符</label></p>
			    <p><input type="checkbox" name="AllowUsernames" id="AllowUsernames2" $_form.checked('AllowUsernames','English',$RegisterLimitSettings.AllowUsernames.CanUseEnglish) value="English"/> <label for="AllowUsernames2">英文字符</label></p>
				<p><input type="checkbox" name="AllowUsernames" id="AllowUsernames3" $_form.checked('AllowUsernames','Number',$RegisterLimitSettings.AllowUsernames.CanUseNumber) value="Number"/> <label for="AllowUsernames3">数字</label></p>
				<p><input type="checkbox" name="AllowUsernames" id="AllowUsernames4" $_form.checked('AllowUsernames','Blank',$RegisterLimitSettings.AllowUsernames.CanUseBlank) value="Blank"/> <label for="AllowUsernames4">特殊符号 (点，下划线，@)</label></p>
				<p><input type="checkbox" name="AllowUsernames" id="AllowUsernames5" $_form.checked('AllowUsernames','OtherChar',$RegisterLimitSettings.AllowUsernames.CanUseOtherChar) value="OtherChar"/> <label for="AllowUsernames5">空格 (只能在用户名中间，不能连续空格)</label></p>
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="UserNameForbiddenWords"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>用户名中禁止出现的关键字</h4>
				<textarea name="UserNameForbiddenWords" cols="30" rows="6">$_form.text('UserNameForbiddenWords',$RegisterLimitSettings.UserNameForbiddenWords)</textarea>
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="TimeSpanForContinuousRegister"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>同一IP连续注册的时间间隔 (分钟)</h4>
				<input type="text" class="text number" name="TimeSpanForContinuousRegister" value="$_form.text('TimeSpanForContinuousRegister',$RegisterLimitSettings.TimeSpanForContinuousRegister)" />
			</th>
			<td>&nbsp;0代表不限制</td>
		</tr>
        <!--[error name="RegisterIPLimitMode"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>注册IP限制方式</h4>
				<p><input type="radio" name="RegisterIPLimitMode" id="RegisterIPLimitMode1" value="Free" $_form.checked('RegisterIPLimitMode','Free',(int)$RegisterLimitSettings.RegisterIPLimitMode==0) /> <label for="RegisterIPLimitMode1">不进行限制</label></p>
				<p><input type="radio" name="RegisterIPLimitMode" id="RegisterIPLimitMode2" value="Reject" $_form.checked('RegisterIPLimitMode','Reject',(int)$RegisterLimitSettings.RegisterIPLimitMode==2) /> <label for="RegisterIPLimitMode2">禁止列表中的IP注册</label></p>
				<p><input type="radio" name="RegisterIPLimitMode" id="RegisterIPLimitMode3" value="Allow" $_form.checked('RegisterIPLimitMode','Allow',(int)$RegisterLimitSettings.RegisterIPLimitMode==1) /> <label for="RegisterIPLimitMode3">只允许列表中IP注册</label></p>
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="RegisterIPLimitList"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr id="RegisterIPLimitList">
			<th>
			    <h4>IP列表</h4>
				<textarea name="RegisterIPLimitList" cols="30" rows="6">$_form.text('RegisterIPLimitList',$RegisterLimitSettings.RegisterIPLimitList)</textarea>
			</th>
			<td>
			    格式:<br/>
			    192.168.*<br/>
                202.101.23<br/>
                220.*<br/>
                *.128.*.23
			</td>
		</tr>
<%--		
		<tr>
			<th>
			    <h4>注册时段限制方式</h4>
				<p><input type="radio" name="RegisterTimeLimitMode" id="RegisterTimeLimitMode1" checked="" /> <label for="RegisterTimeLimitMode1">不进行限制</label></p>
				<p><input type="radio" name="RegisterTimeLimitMode" id="RegisterTimeLimitMode2" checked="" /> <label for="RegisterTimeLimitMode2">禁止在列表中的时段注册</label></p>
				<p><input type="radio" name="RegisterTimeLimitMode" id="RegisterTimeLimitMode3" checked="" /> <label for="RegisterTimeLimitMode3">只允许在列表中的时段注册</label></p>
			</th>
			<td>&nbsp;</td>
		</tr>
		<tr id="RegisterTimeLimitList">
			<th>
			    <h4>时段列表</h4>
				<textarea name="RegisterTimeLimitList" cols="30" rows="6">$_form.text('RegisterTimeLimitList',$RegisterLimitSettings.RegisterTimeLimitList)</textarea>
			</th>
			<td>
			    格式: <br/>
			    00:00-01:00
			</td>
		</tr>
		--%>
        <!--[error name="RegisterEmailLimitMode"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>注册Email限制方式</h4>
				<p><input type="radio" name="RegisterEmailLimitMode" id="RegisterEmailLimitMode1" value="Free" $_form.checked('RegisterEmailLimitMode','Free',(int)$RegisterLimitSettings.RegisterEmailLimitMode==0) /> <label for="RegisterEmailLimitMode1">不进行限制</label></p>
				<p><input type="radio" name="RegisterEmailLimitMode" id="RegisterEmailLimitMode2" value="Reject" $_form.checked('RegisterEmailLimitMode','Reject', (int)$RegisterLimitSettings.RegisterEmailLimitMode==2) /> <label for="RegisterEmailLimitMode2">禁止列表中的Email用于注册</label></p>
				<p><input type="radio" name="RegisterEmailLimitMode" id="RegisterEmailLimitMode3" value="Allow"  $_form.checked('RegisterEmailLimitMode','Allow',(int)$RegisterLimitSettings.RegisterEmailLimitMode==1) /> <label for="RegisterEmailLimitMode3">只允许列表中Email用于注册</label></p>
			</th>
			<td>&nbsp;</td>
		</tr>
        <!--[error name="RegisterEmailLimitList"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr id="RegisterEmailLimitList">
			<th>
			    <h4>Email列表</h4>
				<textarea name="RegisterEmailLimitList" cols="30" rows="6">$_form.text('RegisterEmailLimitList',$RegisterLimitSettings.RegisterEmailLimitList)</textarea>
			</th>
			<td>
			    格式: <br/>
			    *@bbsmax.com<br/>
			    admin@bbsmax.com
			</td>
		</tr>
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
