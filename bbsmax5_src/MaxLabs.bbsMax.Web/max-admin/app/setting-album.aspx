<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>相册相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置相册功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
    <table style="margin-bottom:1px;">
		<tr>
			<th>
			    <h4>是否开启相册功能</h4>
				<p><input type="radio" name="EnableAlbumFunction" id="EnableAlbumFunction1" value="True" $_form.checked('EnableAlbumFunction','True',$AlbumSettings.EnableAlbumFunction == true) /> <label for="EnableAlbumFunction1">开启</label></p>
				<p><input type="radio" name="EnableAlbumFunction" id="EnableAlbumFunction2" value="False" $_form.checked('EnableAlbumFunction','False',$AlbumSettings.EnableAlbumFunction == false) /> <label for="EnableAlbumFunction2">关闭</label></p>
			</th>
			<td>关闭相册功能将使所有网站用户在网站上看不到任何关于相册功能的链接，也不能使用任何关于相册的功能。</td>
		</tr>
    </table>
    <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $AlbumSettings.MaxAlbumCapacity with $index]-->
        <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$AlbumSettings.MaxAlbumCapacity.Count" name="MaxAlbumCapacity" type="long" title="相片存储空间上限" description="允许每个用户使用的相片存储空间上限" /]-->
	    <!--[/loop]-->
		<!--[loop $item in $AlbumSettings.MaxPhotoFileSize with $index]-->
        <!--[load src="../_exceptableitem_filesize_.ascx" index="$index" item="$item" itemCount="$AlbumSettings.MaxPhotoFileSize.Count" name="MaxPhotoFileSize" type="long" title="相片文件大小上限" description="允许每个用户存放的单张相片文件的存储空间上限" /]-->
	    <!--[/loop]-->
    </table>
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
