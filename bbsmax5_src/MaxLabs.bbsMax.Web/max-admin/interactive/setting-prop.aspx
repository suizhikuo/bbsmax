<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>道具系统相关设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<!--[if $_get.success == "1"]-->
<div class="Tip Tip-success">操作成功</div>
<!--[/if]-->
<div class="Content">
    <h3>设置道具系统</h3>
    <form action="$_form.action" method="post">
    <div class="FormTable">
        <table>
		<tr>
			<th>
			    <h4>是否开启道具系统</h4>
				<p><input type="radio" name="EnablePropFunction" id="EnablePropFunction1" value="True" $_form.checked('EnablePropFunction','True',$PropSettings.EnablePropFunction == true) /> <label for="EnablePropFunction1">开启</label></p>
				<p><input type="radio" name="EnablePropFunction" id="EnablePropFunction2" value="False" $_form.checked('EnablePropFunction','False',$PropSettings.EnablePropFunction == false) /> <label for="EnablePropFunction2">关闭</label></p>
			</th>
			<td>关闭道具系统将使所有网站用户在网站上看不到任何关于道具系统的链接，也不能使用任何关于道具的功能。</td>
		</tr>
		<tr>
		    <th>
		        <h4>道具使用记录清理</h4>
                <!--[load src="../_dataclearoption_.ascx" SaveRows="$PropSettings.SaveLogRows" DataClearMode="$PropSettings.DataClearMode" SaveDays="$PropSettings.SaveLogDays" /]-->
		    </th>
		    <td>系统可以定期清除过期的道具系统使用记录，请根据网站的道具使用情况，和数据增长情况设置</td>
		</tr>
        </table>
        <table class="multiColumns" style="margin-bottom:1px;">
		<!--[loop $item in $PropSettings.MaxPackageSize with $index]-->
        <!--[load src="../_exceptableitem_string_.ascx" index="$index" item="$item" itemCount="$PropSettings.MaxPackageSize.Count" name="MaxPackageSize" type="int" textboxwidth="4" title="用户道具负重" description="每个道具都占用一定的道具负重，道具负重决定了用户能持有的道具数量，在您新建道具的时候可以根据道具功能来权衡道具的重量。" /]-->
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
