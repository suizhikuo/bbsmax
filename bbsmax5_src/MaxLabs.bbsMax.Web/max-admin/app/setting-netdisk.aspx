<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>网络硬盘功能设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $Success]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
	<h3>设置网络硬盘功能</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
<table style="margin-bottom:1px;">
<tr>
	<th>
	    <div class="itemtitle">
	        <strong>开启网络硬盘功能</strong>
	    </div>

    <input type="radio" name="EnableDisk" value="true" $_form.checked('EnableDisk','true',$DiskSettings.EnableDisk) id="EnableDisk1" /><label for="EnableDisk1">开启</label>
    <input type="radio" name="EnableDisk" value="false" $_form.checked('EnableDisk','false',!$DiskSettings.EnableDisk) id="EnableDisk2" /><label for="EnableDisk2">关闭</label>
    </th>
    </tr>
    </table>
    <!--[loop $item in $DiskSettings.EnableDisk with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$DiskSettings.EnableDisk.Count" name="EnableDisk"  title="开启网络硬盘" description="是否允许使用网络硬盘功能" /]-->
    <!--[/loop]-->
	<!--[loop $item in $DiskSettings.DefaultViewMode with $index]-->
    <!--[load src="../_exceptableitem_enum_.ascx" index="$index" item="$item" itemCount="$DiskSettings.DefaultViewMode.Count" name="DefaultViewMode" type="FileViewMode" title="默认网络硬盘视图" description="默认网络硬盘视图" /]-->
    <!--[/loop]-->
    <!--[loop $item in $DiskSettings.DiskSpaceSize with $index]-->
    <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$DiskSettings.DiskSpaceSize.Count" name="DiskSpaceSize" title="最大空间大小" description="允许用户网络硬盘所占服务器空间大小限制( 0表示不限制 )" /]-->
    <!--[/loop]-->
    <!--[loop $item in $DiskSettings.MaxFileCount with $index]-->
    <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$DiskSettings.MaxFileCount.Count" type="int" name="MaxFileCount" title="最大文件数" description="允许用户最多有多少个文件( 0表示不限制 )" /]-->
    <!--[/loop]-->
    <!--[loop $item in $DiskSettings.MaxFileSize with $index]-->
    <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$DiskSettings.MaxFileSize.Count" name="MaxFileSize" title="最大文件大小" description="最大的文件大小限制。( 0表示不限制 )" /]-->
    <!--[/loop]-->
    <!--[loop $item in $DiskSettings.AllowFileExtensions with $index]-->
    <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$DiskSettings.AllowFileExtensions.Count" type="ExtensionList" name="AllowFileExtensions" title="允许的文件扩展名" description="多个扩展名之间用逗号(,)分隔。如果不需要限制。请输入  *" /]-->
    <!--[/loop]-->

    <table>
	    <tr class="nohover">
			<th>
			<input type="submit" value="保存设置" class="button" name="savedisksetting" />
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