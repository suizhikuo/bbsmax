<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>记录相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.success == "1"]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
    <h3>设置记录功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table style="margin-bottom:1px;">
			<tr>
				<th>
					<h4>是否开启记录功能</h4>
					<p><input type="radio" name="EnableDoingFunction" id="EnableDoingFunction1" value="True" $_form.checked('EnableDoingFunction','True',$DoingSettings.EnableDoingFunction == true) /> <label for="EnableDoingFunction1">开启</label></p>
					<p><input type="radio" name="EnableDoingFunction" id="EnableDoingFunction2" value="False" $_form.checked('EnableDoingFunction','False',$DoingSettings.EnableDoingFunction == false) /> <label for="EnableDoingFunction2">关闭</label></p>
				</th>
				<td>关闭记录功能将使所有网站用户在网站上看不到任何关于记录功能的链接，也不能使用任何关于记录的功能。</td>
			</tr>
        </table>
        <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $DoingSettings.EveryDayPostLimit with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$DoingSettings.EveryDayPostLimit.Count" name="EveryDayPostLimit" type="int" textboxwidth="4" title="每个用户每天允许发表的记录条数" description="此功能用于防止用户贫乏发布记录刷积分，您可以针对不同用户组做例外设置，0表示不限制。" /]-->
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
