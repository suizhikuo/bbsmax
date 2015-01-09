<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>好友印象相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.success == "1"]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
    <h3>设置好友印象功能</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table style="margin-bottom:1px;">
		<tr>
			<th>
			    <h4>是否开启好友印象功能</h4>
				<p><input type="radio" name="EnableImpressionFunction" id="EnableImpressionFunction1" value="True" $_form.checked('EnableImpressionFunction','True',$ImpressionSettings.EnableImpressionFunction == true) /> <label for="EnableImpressionFunction1">开启</label></p>
				<p><input type="radio" name="EnableImpressionFunction" id="EnableImpressionFunction2" value="False" $_form.checked('EnableImpressionFunction','False',$ImpressionSettings.EnableImpressionFunction == false) /> <label for="EnableImpressionFunction2">关闭</label></p>
			</th>
			<td>关闭好友印象功能将使所有网站用户在网站上看不到任何关于好友印象功能的链接，也不能使用任何关于好友印象的功能。</td>
		</tr>
		<tr>
			<th>
			    <h4>两次描述间时间间隔限制(小时为单位)</h4>
				<p><input type="text" name="TimeLimit" class="text number" value="$_form.text('TimeLimit', $ImpressionSettings.TimeLimit)" /></p>
			</th>
			<td>设置每个用户对某人做印象描述的最短时间间隔。</td>
		</tr>
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
