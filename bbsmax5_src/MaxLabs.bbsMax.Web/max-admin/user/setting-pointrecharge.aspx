<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>积分充值设置</title>
<!--[include src="../_htmlhead_.aspx"/]-->
</head>
<body>
<!--[include src="../_head_.aspx"/]-->
<!--[include src="../_setting_msg_.aspx"/]-->

<div class="Content">

	<h3>积分充值设置</h3>
	<div class="Help">
	    <p>积分充值是指用户可以用人民币来购买积分，<span style="color:Red">一旦开启了积分充值则该积分的上限将变为无限大</span></p>
	    <p>人民币 和 可充积分 是指人民币和积分的充值比例。比如人民币填“11”，可充积分填“10”表示 11块钱可以买到对应的积分数量为10</p>
	    <p>以下列出的是已经启用的积分</p>
	    
	    </div>
	<form name="transfer" action="$_form.action" method="post">
	<div class="FormTable">
	 <table>
	    <tr>
	        <th>
	        <h4>设置积分充值(只有开启积分充值以下设置才有效)</h4>
	        <p><input type="radio" name="enable" id="enable1" value="true"  $_form.checked('enable','true',$PaySettings.EnablePointRecharge) /><label for="enable1">开启</label></p>
	        <p><input type="radio" name="enable" id="enable2" value="false"  $_form.checked('enable','false',$PaySettings.EnablePointRecharge==false) /><label for="enable2">关闭</label></p>
	        </th>
	    </tr>
	  </table>
	</div>  
	 <div id="paymethod" class="FormTable">   
	 <table>
	    <tr>
	        <th>
	        <h4>积分充值的商品名称</h4>
	        <input type="text" class="text" name="ProductName" value="$_form.text('ProductName',$PaySettings.ProductName)" />
	        </th>
	        <td></td>
	    </tr>
	    <tr>
	        <th>
	        <h4>支付宝设置</h4>
	        <p><input type="radio" name="EnableAlipay" id="EnableAlipay1" value="True" $_form.checked('EnableAlipay','True',$PaySettings.EnableAlipay) /> <label for="EnableAlipay1">开启支付宝支付方式</label></p>
	        <p><input type="radio" name="EnableAlipay" id="EnableAlipay2" value="False" $_form.checked('EnableAlipay','False',$PaySettings.EnableAlipay==false) /> <label for="EnableAlipay2">关闭支付宝支付方式</label></p>
	       </th>
	       <td>&nbsp;</td>
	    </tr>
	    <tr id="AlipaySetting">
	        <th>
	        <h4>支付宝账号设置:</h4>
	        <input type="text" class="text" name="Alipay_SellerEmail" value="$_form.text('Alipay_SellerEmail',$PaySettings.Alipay_SellerEmail)" />
	        <h4>支付宝用户ID:</h4>
	        <input type="text" class="text" name="Alipay_PartnerID" value="$_form.text('Alipay_PartnerID',$PaySettings.Alipay_PartnerID)" />
	        <h4>支付宝验证码:</h4>
	        <input type="text" class="text" name="Alipay_Key" value="$_form.text('Alipay_Key',$PaySettings.Alipay_Key)" />
	        </th>
	        <td></td>
	    </tr>
	    <tr>
	        <th>
	        <h4>财付通设置</h4>
	        <p><input type="radio" name="EnableTenpay" id="EnableTenpay1" value="True" $_form.checked('EnableTenpay','True',$PaySettings.EnableTenpay) /> <label for="EnableTenpay1">开启财付通支付方式</label></p>
	        <p><input type="radio" name="EnableTenpay" id="EnableTenpay2" value="False" $_form.checked('EnableTenpay','False',$PaySettings.EnableTenpay==false) /> <label for="EnableTenpay2">关闭财付通支付方式</label></p>
	       </th>
	       <td>&nbsp;</td>
	    </tr>
	    <tr id="TenpaySetting">
	        <th>
	        <h4>财付通商户号:</h4>
	        <input type="text" class="text" name="Tenpay_BargainorID" value="$_form.text('Tenpay_BargainorID',$PaySettings.Tenpay_BargainorID)" />
	        <h4>财付通商户Key:</h4>
	        <input type="text" class="text" name="Tenpay_Key" value="$_form.text('Tenpay_Key',$PaySettings.Tenpay_Key)" />
	        </th>
	        <td></td>
	    </tr>
	    <tr>
	        <th>
	        <h4>快钱设置</h4>
	        <p><input type="radio" name="Enable99Bill" id="Enable99Bill1" value="True" $_form.checked('Enable99Bill','True',$PaySettings.Enable99Bill) /> <label for="Enable99Bill1">开启财付通支付方式</label></p>
	        <p><input type="radio" name="Enable99Bill" id="Enable99Bill2" value="False" $_form.checked('Enable99Bill','False',$PaySettings.Enable99Bill==false) /> <label for="Enable99Bill2">关闭财付通支付方式</label></p>
	       </th>
	       <td>&nbsp;</td>
	    </tr>
	    <tr id="_99BillSettings">
	        <th>
	        <h4>快钱人民币网关账户号:</h4>
	        <input type="text" class="text" name="_99Bill_MerchantAcctID" value="$_form.text('_99Bill_MerchantAcctID',$PaySettings._99Bill_MerchantAcctID)" />
	        <h4>快钱人民币网关密钥:</h4>
	        <input type="text" class="text" name="_99Bill_Key" value="$_form.text('_99Bill_Key',$PaySettings._99Bill_Key)" />
	        </th>
	        <td></td>
	    </tr>
	 </table>    
	<div class="DataTable">
        <table>
        <thead>
		<tr>
			<td>积分</td>
			<td>允许充值</td>
			<td>人民币(单位：元)</td>
			<td>可充积分</td>
			<td>一次至少需要充值积分数量</td>
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
            <td><!--[if $HasError("money")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("point")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            <td><!--[if $HasError("minvalue")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
            </tr>
        <!--[/error]-->
        <!--[if $GetRule($type) != null]-->
		<tr>
		    <td>$Name</td>
			<td><input value="true" name="canRecharge.$PointID" type="checkbox" $_form.checked('canRecharge.$PointID','true',$GetRule($type).enable) /></td>
			<td><input type="text" class="text number" name="money.$PointID" value="$_form.text('money.$PointID',$GetRule($type).money)" /> </td>
			<td><input type="text" class="text number" name="point.$PointID" value="$_form.text('point.$PointID',$GetRule($type).point)" /> </td>
			<td><input type="text" class="text number" name="minvalue.$PointID" value="$_form.text('minvalue.$PointID',$GetRule($type).MinValue)" /></td>
		</tr>
		<!--[else]-->
		<tr>
		    <td>$Name</td>
			<td><input value="true" name="canRecharge.$PointID" type="checkbox" $_form.checked('canRecharge.$PointID','true') /></td>
			<td><input type="text" class="text number" name="money.$PointID" value="$_form.text('money.$PointID')" /> </td>
			<td><input type="text" class="text number" name="point.$PointID" value="$_form.text('point.$PointID')" /> </td>
			<td><input type="text" class="text number" name="minvalue.$PointID" value="$_form.text('minvalue.$PointID')" /></td>
		</tr>
		<!--[/if]-->
		<!--[/EnabledUserPointList]-->
		</tbody>
		</table>
		</div>
	</div>
		<div class="Actions">
            <input class="button" name="save" type="submit" value="保存" />
        </div>
	
	</form>
</div>
<script type="text/javascript">
    initDisplay('enable', [
     { value: "true", display: true, id: 'paymethod' }
    , { value: "false", display: false, id: 'paymethod'}
]);
     initDisplay('EnableAlipay', [
     { value: "true", display: true, id: 'AlipaySetting' }
    , { value: "false", display: false, id: 'AlipaySetting' }
]);
     initDisplay('EnableTenpay', [
     { value: "true", display: true, id: 'TenpaySetting' }
    , { value: "false", display: false, id: 'TenpaySetting' }
]);
     initDisplay('Enable99Bill', [
     { value: "true", display: true, id: '_99BillSettings' }
    , { value: "false", display: false, id: '_99BillSettings' }
]);
_99BillSettings
</script>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
