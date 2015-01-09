<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>用户设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>签名设置</h3>
	<form action="$_form.action" method="post">
	<div class="FormTable">
	
	<!--[loop $item in $UserSettings.SignatureFormat with $index]-->
    <!--[load src="../_exceptableitem_enum_.ascx" index="$index" item="$item" itemCount="$UserSettings.SignatureFormat.Count" name="SignatureFormat" type="SignatureFormat" title="用户签名格式" description="允许用户在签名中使用的内容格式，<span class=\"red\">请注意：非管理员的用户组，请勿设置成HTML， 否则可能会带来安全隐患。</span>" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.SignatureHeight with $index]-->
    <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$UserSettings.SignatureHeight.Count" name="SignatureHeight" type="int" title="签名区高度" description="帖子页的用户签名区高度限制" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.SignatureLength with $index]-->
    <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$UserSettings.SignatureLength.Count" name="SignatureLength" type="int" title="签名长度" description="用户签名的长度限制" /]-->
    <!--[/loop]-->

    <!--[loop $item in $UserSettings.AllowUserEmoticon with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowUserEmoticon.Count" name="AllowUserEmoticon"  title="使用自定义表情" description="当用户的自定义表情可用时， 是否允许在签名中使用自定义表情" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.AllowDefaultEmoticon with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowDefaultEmoticon.Count" name="AllowDefaultEmoticon"  title="使用默认表情" description=" 是否允许在签名中使用系统默认表情" /]-->
    <!--[/loop]-->


	<h3>用户签名特殊UBB标签设置</h3>

    <!--[loop $item in $UserSettings.AllowImageTag with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowImageTag.Count" name="AllowImageTag"  title="图片" description="是否允许在签名中显示图片" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.AllowFlashTag with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowFlashTag.Count" name="AllowFlashTag"  title="Flash" description="是否允许在签名中显示Flash" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.AllowAudioTag with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowAudioTag.Count" name="AllowAudioTag"  title="音频" description="是否允许在签名中显示音频" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.AllowVideoTag with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowVideoTag.Count" name="AllowVideoTag"  title="视频" description="是否允许在签名中显示视频" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.AllowTableTag with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowTableTag.Count" name="AllowTableTag"  title="表格" description="是否允许在签名中插入表格" /]-->
    <!--[/loop]-->
    
    <!--[loop $item in $UserSettings.AllowUrlTag with $index]-->
    <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$UserSettings.AllowUrlTag.Count" name="AllowUrlTag"  title="允许URL" description="是否允许在签名中插入链接" /]-->
    <!--[/loop]-->

    <table>
	    <tr class="nohover">
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