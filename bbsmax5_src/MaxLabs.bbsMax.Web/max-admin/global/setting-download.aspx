<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>防盗链设置</title>
<!--[include src="../_htmlhead_.aspx" /]-->
<script type="text/javascript">
    initDisplay('UseDownloadFilter', [
        { value: 'True', display: true, id: 'AllowReferrerHost' },
        { value: 'False', display: false, id: 'AllowReferrerHost' }
    ]);
</script>
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>设置防盗链功能</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	<table>
        <!--[error name="UseDownloadFilter"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr>
			<th>
			    <h4>是否启用防盗链功能</h4>
				<p><input type="radio" name="UseDownloadFilter" id="UseDownloadFilter1" value="True" $_form.checked('UseDownloadFilter','True',$DownloadSettings.UseDownloadFilter==true) /> <label for="UseDownloadFilter1">启用</label></p>
				<p><input type="radio" name="UseDownloadFilter" id="UseDownloadFilter2" value="False" $_form.checked('UseDownloadFilter','False',$DownloadSettings.UseDownloadFilter==false) /> <label for="UseDownloadFilter2">不启用</label></p>
			</th>
			<td>此防盗链功能只对用户上传的文件有效，例如：附件、网络硬盘、照片等。</td>
		</tr>
        <!--[error name="RegisterIPLimitList"]--> 
            <!--[include src="../_error_.aspx"/]-->
        <!--[/error]-->
		<tr id="AllowReferrerHost">
			<th>
			    <h4>防盗链白名单</h4>
				<textarea name="AllowReferrerHost" cols="30" rows="6">$_form.text('AllowReferrerHost',$DownloadSettings.AllowReferrerHost)</textarea>
			</th>
			<td>
			    白名单中的域名不受防盗链系统限制<br />
			    域名允许通配符（*号表示任意字符，例如：<br/>
			    *.bbsmax.*<br/>
                *.google.com<br/>
                220.101.*<br/>
                *.128.*.23
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
