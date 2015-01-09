<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>插件管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <div class="DataTable">
    <h4>插件<span class="counts">总数: $PluginList.Count</span></h4>
    <form action="$_form.action" method="post">
        <!--[if $PluginList.Count > 0]-->
        <table>
			<thead>
				<tr>
	                <th class="CheckBoxHold">&nbsp;</th>
					<th>插件名称</th>
					<th>描述</th>
					<th>状态</th>
				</tr>
			</thead>
            <tbody>
        <!--[/if]-->
    <!--[loop $Plugin in $PluginList]-->
            <tr>
				<td>
					<input type="checkbox" class="CheckBoxHold" name="PluginNames" value="$Plugin.Name" />
				</td>
                <td>$Plugin.DisplayName</td>
                <td>$Plugin.Description</td>
                <td>
                    <!--[if $Plugin.Enable]-->
                    已启用
                    <!--[else]-->
                    已禁用
                    <!--[/if]-->
                </td>
             </tr>
    <!--[/loop]-->
        <!--[if $PluginList.Count > 0]-->
            </tbody>
        </table>
        <div class="Actions">
        <input type="submit" name="Disable" value="禁用所选插件" class="button" onclick="return confirm('确认要禁用所选插件吗?');" /> <input type="submit" name="Enable" value="启用所选插件" class="button" onclick="return confirm('确认要启用所选插件吗?');" />
        </div>
		<!--[else]-->
	    <div class="NoData">未安装任何插件</div>
		<!--[/if]-->
	</form>
    </div>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
