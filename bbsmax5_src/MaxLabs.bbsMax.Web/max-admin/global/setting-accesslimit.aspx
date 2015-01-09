<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>设置访问限制</title>
<!--[include src="../_htmlhead_.aspx"/]-->
<script type="text/javascript">
initDisplay("AccessIPLimitMode", [
              { value : "Free",   display : false, id : 'AccessIPLimitList' }
             ,{ value : "Reject" ,display : true , id : 'AccessIPLimitList' }
             ,{ value : "Allow"  ,display : true , id : 'AccessIPLimitList' }]
             );
initDisplay("AdminIPLimitMode", [
              { value : "Free",   display : false, id : 'AdminIPLimitList' }
             ,{ value : "Reject" ,display : true , id : 'AdminIPLimitList' }
             ,{ value : "Allow"  ,display : true , id : 'AdminIPLimitList' }]
             );
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置访问限制</h3>
	<form action="$_form.Action" method="post">
	<div class="FormTable">
		<table>
            <!--[error name="AccessIPLimitList"]-->
            <!--[include src="../_error_.aspx"/]-->
            <!--[/error]-->
			<tr id="AccessIPLimitList">
				<th>
				    <h4>论坛访问受限IP列表</h4>
					<textarea cols="40" rows="10" name="AccessIPLimitList">$_form.text("AccessIPLimitList",$AccessLimitSettings.AccessIPLimitList)</textarea>
				</th>
				<td>每行填写一个IP地址.</td>
			</tr>
            <!--[error name="AdminIPLimitMode"]-->
            <!--[include src="../_error_.aspx"/]-->
            <!--[/error]-->
			<tr>
				<th>
				    <h4>管理员控制台访问IP限制方式</h4>
					<p><input type="radio" name="AdminIPLimitMode" id="AdminIPLimitMode1" value="Free"   $_form.checked("AdminIPLimitMode","Free",(int)$AccessLimitSettings.AdminIPLimitMode==0) /> <label for="AdminIPLimitMode1">不进行限制</label></p>
					<p><input type="radio" name="AdminIPLimitMode" id="AdminIPLimitMode2" value="Reject" $_form.checked("AdminIPLimitMode","Reject",(int)$AccessLimitSettings.AdminIPLimitMode==2) /> <label for="AdminIPLimitMode2">禁止列表中的IP访问</label></p>
					<p><input type="radio" name="AdminIPLimitMode" id="AdminIPLimitMode3" value="Allow"  $_form.checked("AdminIPLimitMode","Allow",(int)$AccessLimitSettings.AdminIPLimitMode==1) /> <label for="AdminIPLimitMode3">只允许列表中IP访问</label></p>
				</th>
				<td>&nbsp;</td>
			</tr>
            <!--[error name="AdminIPLimitList"]-->
            <!--[include src="../_error_.aspx"/]-->
            <!--[/error]-->
			<tr id="AdminIPLimitList">
				<th>
				    <h4>管理员控制台访问受限IP列表</h4>
					<textarea cols="40" rows="5" name="AdminIPLimitList">$_form.text("AdminIPLimitList",$AccessLimitSettings.AdminIPLimitList)</textarea>
				</th>
				<td>每行填写一个IP地址.</td>
			</tr>
			<tr>
				<th>
				    <input type="submit" value="保存设置" name="savesetting" class="button" />
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
