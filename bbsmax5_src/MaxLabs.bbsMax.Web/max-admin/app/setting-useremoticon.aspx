<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>用户表情设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
<div class="PageHeading">
	    <h3> 用户表情设置 </h3>
</div>
<form action="$_form.action" method="post">
    	<div class="FormTable">
   <table style="margin-bottom:1px;">
<tr>
	<th>
	    <div class="itemtitle">
	        <strong>开启自定义表情</strong>
	        </div>

		    <input type="radio" name="EnableUserEmoticons" value="true" $_form.checked('EnableUserEmoticons','true',$EmoticonSettings.EnableUserEmoticons) id="enableEmot1" /><label for="enableEmot1">开启</label>
		    <input type="radio" name="EnableUserEmoticons" value="false" $_form.checked('EnableUserEmoticons','false',!$EmoticonSettings.EnableUserEmoticons) id="enableEmot2" /><label for="enableEmot2">关闭</label>
</th>
</tr>
</table>
        <!--[loop $item in $EmoticonSettings.EnableUserEmoticons with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$EmoticonSettings.EnableUserEmoticons.Count" name="EnableUserEmoticons"  title="开启自定义表情功能" description="是否允许使用自定义表情功能" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $EmoticonSettings.MaxEmoticonSpace with $index]-->
        <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$EmoticonSettings.MaxEmoticonSpace.Count" name="MaxEmoticonSpace" title="最大空间大小" description="允许用户表情所占服务器空间大小限制( 0表示不限制 )" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $EmoticonSettings.MaxEmoticonCount with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$EmoticonSettings.MaxEmoticonCount.Count" type="int" name="MaxEmoticonCount" title="最大表情数" description="允许用户最多有多少个自定义表情( 0表示不限制 )" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $EmoticonSettings.MaxEmoticonFileSize with $index]-->
        <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$EmoticonSettings.MaxEmoticonFileSize.Count" name="MaxEmoticonFileSize" title="最大表情文件大小" description="最大的表情文件大小限制。( 0表示不限制 )" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $EmoticonSettings.Import with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$EmoticonSettings.Import.Count" textboxwidth="4" name="Import" title="表情导入" description="是否允许通过 EIP或者CFC表情包导入表情文件" /]-->
	    <!--[/loop]-->
	    <!--[loop $item in $EmoticonSettings.Export with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$EmoticonSettings.Export.Count" name="Export" title="表情导出" description="是否允许用户导出自定义表情" /]-->
	    <!--[/loop]-->
	    <table>
	    <tr class="nohover">
		    <th><input type="submit" value="保存设置" class="button" name="saveEmoticonSettings" /></th>
		    <td>&nbsp;</td>
	    </tr>
	    </table>
    </div>
    </form>
 </div>   
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
