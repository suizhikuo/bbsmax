<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>收藏相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
    <h3>设置收藏功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table>
			<tr>
				<th>
					<h4>是否开启收藏功能</h4>
					<p><input type="radio" name="EnableFavoriteFunction" id="EnableFavoriteFunction1" value="True" $_form.checked('EnableFavoriteFunction','True',$FavoriteSettings.EnableFavoriteFunction == true) /> <label for="EnableFavoriteFunction1">开启</label></p>
					<p><input type="radio" name="EnableFavoriteFunction" id="EnableFavoriteFunction2" value="False" $_form.checked('EnableFavoriteFunction','False',$FavoriteSettings.EnableFavoriteFunction == false) /> <label for="EnableFavoriteFunction2">关闭</label></p>
				</th>
				<td>关闭收藏功能将使所有网站用户在网站上看不到任何关于收藏功能的链接，也不能使用任何关于收藏的功能。</td>
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
