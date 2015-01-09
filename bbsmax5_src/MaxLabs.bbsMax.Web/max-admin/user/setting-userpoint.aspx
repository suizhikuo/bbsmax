<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>扩展积分设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->
<div class="Content">
	<h3>扩展积分设置</h3>
	<div class="Help">
    <p>扩展积分至少要有一个是启用的；上限和下限不填则默认分别为整型的最大值“2147483647”和“0”；</p>
    <p>更新用户积分：是指如果用户的积分超出积分的上限或者下限则将其更新到上限或者下限，通常如果修改了积分的上限或者下限则需要更新用户积分；</p>
    <p><span style="color:Red">如果积分的上限不能更改是因为该积分类型已被设置为可以充值，则该积分的上限始终为整型的最大值</span></p>
    </div>
	<form action="$_form.action" method="post">
	<div class="DataTable">
        <table>
		<thead>
			<tr>
				<td style="width:30px;">编号</td>
				<td style="width:30px;">启用</td>
				<td style="width:60px;">公开显示</td>
				<td style="width:60px;">名称</td>
				<td style="width:60px;">单位</td>
				<td style="width:60px;">注册初始积分</td>
				<td style="width:60px;">上限</td>
				<td style="width:60px;">下限</td>
				<td style="">等级图标</td>
				<td style="width:70px;">操作</td>
			</tr>
		</thead>
		<tbody>
		<!--[AllUserPointList]-->
            <!--[error line="$i"]-->
                <tr class="ErrorMessage">
                <td colspan="10" class="Message"><div class="Tip Tip-error">$Messages</div></td>
                </tr>
                <tr class="ErrorMessageArrow">
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("name")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("initialValue")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("maxValue")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("minValue")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                </tr>
            <!--[/error]-->
			<tr>
			    <td>{=$PointID+1}</td>
			    <td><input value="true" name="enable.$PointID" type="checkbox" $_form.checked('enable.$PointID','true',$enable) /></td>
			    <td><input value="true" name="display.$PointID" type="checkbox" $_form.checked('display.$PointID','true',$display) /></td>
			    <td><input type="text" class="text" style="width:95%;" name="name.$PointID" value="$_form.text('name.$PointID',$Name)" /></td>
			    <td><input type="text" class="text" style="width:95%;" name="unitName.$PointID" value="$_form.text('unitName.$PointID',$UnitName)" /></td>
			    <td><input type="text" class="text" style="width:95%;" name="initialValue.$PointID" value="$_form.text('initialValue.$PointID',$InitialValue)" /></td>
			    <td><input type="text" class="text" style="width:95%;" name="maxValue.$PointID"   $_if($CanRecharge($type),'disabled="disabled"','') value="$_form.text('maxValue.$PointID',$maxValueString)" /></td>
			    <td><input type="text" class="text" style="width:95%;" name="minValue.$PointID" value="$_form.text('minValue.$PointID',$minValueString)" /></td>
			    <td>$GetPointIconUpdateDescription($Type)</td>
			    <td><a href="$dialog/point-icon.aspx?pointtype=$Type" onclick="return openDialog(this.href, refresh)">编辑图标</a></td>
			</tr>
        <!--[/AllUserPointList]-->
		</tbody>
	    </table>
        <div class="Actions">
            <input class="button" name="saveuserpoints" type="submit" value="保存设置" />
            <input class="button" name="updateuserpoints" type="submit" value="更新用户积分" />
        </div>
	</div>
	</form>
	
	<form action="$_form.action" method="post">
	<h3>其它设置</h3>
	<div class="FormTable">
    <table>
        <!--[error name="generalPointName"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>总积分名称</h4>
                <input type="text" class="text" name="generalPointName" value="$_form.text('generalPointName',$PointSettings.GeneralPointName)" />
            </th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <h4>公开显示总积分</h4>
                <input type="radio" id="displaypoint" name="displayGeneralPoint" value="true" $_form.checked('displayGeneralPoint','true',$PointSettings.DisplayGeneralPoint) />
                <label for="displaypoint">是</label>
                <input type="radio" id="nodisplaypoint" name="displayGeneralPoint" value="false" $_form.checked('displayGeneralPoint','false',$PointSettings.DisplayGeneralPoint==false) />
                <label for="nodisplaypoint">否</label>
            </th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <h4>总积分等级图标</h4>
                $GeneralPointPointIconUpdateDescription
                (<a href="$dialog/point-icon.aspx?pointtype=GeneralPoint" onclick="return openDialog(this.href, refresh)">编辑</a>)
            </th>
            <td>&nbsp;</td>
        </tr>
        <!--[error name="generalPointExpression"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>总积分公式</h4>
                <textarea name="generalPointExpression" cols="50" rows="8">$_form.text('generalPointExpression',$PointSettings.GeneralPointExpression)</textarea>
            </th>
            <td>
                <p>公式可用的字段:<br /> $PointExpressionColumsDescription </p>
                <p>可以用这些字段进行加(+)减(-)乘(*)除(/)运算, <span class="red">为防止除数为0,字段不能做除数,只能用常量做除数</span></p>
            </td>
        </tr>
        <!--[error name="pointTradeRate"]-->
            <!--[include src="../_error_.aspx" /]-->
        <!--[/error]-->
        <tr>
            <th>
                <h4>积分交易税率</h4>
                <input type="text" name="pointTradeRate" value="$_form.text('pointTradeRate',$PointSettings.TradeRate)" class="text number" /> %
            </th>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <th>
                <input type="submit" name="savePointOtherSetting" value="保存设置" class="button" />
                <input type="submit" name="updatePoints" value="重新计算用户积分" class="button" />
            </th>
            <td>&nbsp;</td>
        </tr>
        </table>
		</div>
	</form>
</div>

<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
