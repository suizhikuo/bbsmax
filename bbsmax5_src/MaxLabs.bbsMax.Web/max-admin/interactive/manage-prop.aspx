<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>道具管理</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx" /]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
    <div class="PageHeading">
        <h3>道具列表</h3>
        <div class="ActionsBar">
            <script type="text/javascript">
                function JumpToDetail(type) {
                    window.location.href = window.location.href.replace('manage-prop.aspx', 'manage-prop-detail.aspx?proptype=' + type);
                }
            </script>
            <a href="$dialog/prop-create.aspx" onclick="return openDialog(this.href, function(result){ JumpToDetail(result); })"><span>创建道具</span></a>
        </div>
    </div>
    <div class="DataTable">
    <h4>道具<span class="counts">总数: $TotalPropCount</span></h4>
    <form action="$_form.action" method="post">
        <!--[if $TotalPropCount > 0]-->
        <table>
			<thead>
				<tr>
	                <th class="CheckBoxHold">&nbsp;</th>
					<th>道具名称</th>
					<th style="width:50px">状态</th>
					<td style="width:50px">价格</td>
					<td style="width:50px">重量</td>
					<td style="width:50px">交易</td>
					<th>描述</th>
					<td>类型</td>
					<th>总数</th>
					<th style="width:50px">售出</th>
					<th style="width:40px;">操作</th>
				</tr>
			</thead>
            <tbody>
        <!--[/if]-->
        <!--[loop $Prop in $PropList]-->
            <tr <!--[if !$Prop.Enable]-->style="color:#999;"<!--[/if]-->>
				<td>
					<input type="checkbox" class="CheckBoxHold" name="PropIDs" value="$Prop.PropID" />
				</td>
				<td><strong>$Prop.Name</strong></td>
                <td>
                    <!--[if $Prop.Enable]-->
                    <span style="color:#060;">启用</span>
                    <!--[else]-->
                    禁用
                    <!--[/if]-->
                </td>
                <td>$Prop.PriceName $Prop.Price $Prop.PriceUnit</td>
                <td>$Prop.PackageSize</td>
                <td>
                    <!--[if $Prop.AllowExchange]-->
                    允许
                    <!--[else]-->
                    不允许
                    <!--[/if]-->
                </td>
                
                <td>$Prop.Description</td>
                <td>$Prop.PropTypeName</td>
                <td>$Prop.TotalNumber</td>
                <td>$Prop.SaledNumber</td>
                <td>
                    <a href="manage-prop-detail.aspx?id=$Prop.PropID&page=$_get.page">编辑</a>
                </td>
             </tr>
        <!--[/loop]-->
        <!--[if $TotalPropCount > 0]-->
            </tbody>
        </table>
        <!--[AdminPager Count="$TotalPropCount" PageSize="10" /]-->
        <div class="Actions">
        <input type="submit" name="Disable" value="禁用所选道具" class="button" onclick="return confirm('确认要禁用所选道具吗?');" /> <input type="submit" name="Enable" value="启用所选道具" class="button" onclick="return confirm('确认要启用所选道具吗?');" /> <input type="submit" name="Delete" value="删除所选道具" class="button" onclick="return confirm('确认要删除所选道具吗?删除后不可恢复!');" />
        </div>
		<!--[else]-->
	    <div class="NoData">未设置任何插件</div>
		<!--[/if]-->
	</form>
    </div>
</div>

<!--[include src="../_foot_.aspx" /]-->
</body>
</html>
