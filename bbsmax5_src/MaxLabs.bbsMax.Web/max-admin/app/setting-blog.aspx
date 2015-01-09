<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>日志相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.success == "1"]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
    <h3>设置日志功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table style="margin-bottom:1px;">
		<tr>
			<th>
			    <h4>是否开启博客功能</h4>
				<p><input type="radio" name="EnableBlogFunction" id="EnableBlogFunction1" value="True" $_form.checked('EnableBlogFunction','True',$BlogSettings.EnableBlogFunction == true) /> <label for="EnableBlogFunction1">开启</label></p>
				<p><input type="radio" name="EnableBlogFunction" id="EnableBlogFunction2" value="False" $_form.checked('EnableBlogFunction','False',$BlogSettings.EnableBlogFunction == false) /> <label for="EnableBlogFunction2">关闭</label></p>
			</th>
			<td>关闭博客功能将使所有网站用户在网站上看不到任何关于博客功能的链接，也不能使用任何关于博客的功能。</td>
		</tr>
        </table>
        <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $BlogSettings.AllowHtml with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$BlogSettings.AllowHtml.Count" name="AllowHtml" type="bool" title="是否允许在日志内容中使用HTML" description="请谨慎启用此功能，允许在日志内容中输入HTML将使黑客有机会破坏您的网站或损害您网站的用户，通常此功能只针对于您本人或可信任的管理员做例外设置，如果您不清楚启用此设置将带来什么影响建议您不要对任何用户组启用此功能。" /]-->
	    <!--[/loop]-->
		<!--[loop $item in $BlogSettings.AllowUbb with $index]-->
        <!--[load src="../_exceptableitem_bool_.ascx" index="$index" item="$item" itemCount="$BlogSettings.AllowUbb.Count" name="AllowUbb" type="bool" title="是否允许用户在日志内容中使用UBB" description="启用此功能后用户将可以在发布日志时使用UBB标签格式化内容和插入图片、视频、链接等。" /]-->
	    <!--[/loop]-->
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
