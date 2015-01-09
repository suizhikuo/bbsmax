<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>积分兑换与积分交易设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">
	<h3>转帐设置</h3>
	<div class="Help">
	    <p>转帐是指用户把自己的积分转给别人</p>
	    <p>以下列出的是已经启用的积分</p>
	    </div>
	<form name="transfer" action="$_form.action" method="post">
	<div class="DataTable">
        <table>
        <thead>
		<tr>
		<td>
	    <input type="checkbox" name="enable" id="enable" value="true"  $_form.checked('enable','true',$PointSettings.EnablePointTransfer) /><label for="enable">开启转帐功能</label> (只有开启转帐功能以下设置才有效)
	    </td>
		</tr>
		</thead>
		</table>
	</div>
	<div class="DataTable">
        <table>
        <thead>
		<tr>
			<td>积分</td>
			<td>允许转帐</td>
			<td>税率</td>
			<td>转帐后剩余最低余额</td>
		</tr>
		</thead>
		<tbody>
		<!--[EnabledUserPointList]-->
		<!--[error line="$i"]-->
	        <tr class="ErrorMessage">
            <td colspan="5" class="Message"><div class="Tip Tip-error">$Messages</div></td>
            </tr>
            <tr class="ErrorMessageArrow">
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td><!--[if $HasError("taxRate")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("minRemaining")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            </tr>
        <!--[/error]-->
		<tr>
		    <!--[PointTransferRule UserPointType="$type"]-->
		    <td>$parent($Name)</td>
			<td><input value="true" name="canTransfer.$parent($PointID)" type="checkbox" $_form.checked('canTransfer.$parent($PointID)','true',$rule.canTransfer) /></td>
			<td><input type="text" class="text number" name="taxRate.$parent($PointID)" value="$_form.text('taxRate.$parent($PointID)',$rule.taxRate)" /> %</td>
			<td><input type="text" class="text number" name="minRemaining.$parent($PointID)" value="$_form.text('minRemaining.$parent($PointID)',$rule.MinRemainingValue)" /></td>
			<!--[/PointTransferRule]-->
		</tr>
		<!--[/EnabledUserPointList]-->
		</tbody>
		</table>
		<div class="Actions">
            <input class="button" name="savepointTransferRules" type="submit" value="保存" />
        </div>
	</div>
	</form>
</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
