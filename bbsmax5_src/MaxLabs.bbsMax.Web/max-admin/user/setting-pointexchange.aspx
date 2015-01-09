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
 	<h3>积分兑换</h3>
 	<div class="Help">
    <p>兑换是指用户把自己的不同类型的扩展积分互相转换</p>
    </div>
	<form action="$_form.action" method="post">
	    <div class="DataTable">
        <h4>开启积分兑换功能</h4>
        <table>
        <thead>
		    <tr>
			    <td>
			    <input type="checkbox" name="enable" id="enable" value="true"  $_form.checked('enable','true',$PointSettings.EnablePointExchange) /><label for="enable">开启</label> (只有开启积分兑换功能以下设置才有效)
			    </td>
			</tr>
		</thead>
		</table>
		<div class="Actions">
            <input class="button" name="saveenable" type="submit" value="保存" />
        </div>
		</div>
	    <div class="DataTable">
        <h4>积分兑换比例设置</h4>
        <table>
        <thead>
		    <tr>
			    <td>
			    <table style="width:auto;"><tr>
			    <!--[EnabledUserPointList]-->
				<td style="width:8em;">$Name</td>
                <!--[/EnabledUserPointList]-->
                </tr></table>
                </td>
			</tr>
		</thead>
		<tbody>
		    <!--[error line="0"]-->
		    <tr class="nohover">
			    <td style="padding:0;border-bottom:0;">
		            <table style="width:auto;">
	                    <tr class="ErrorMessage">
                        <td colspan="$EnabledUserPointCount" class="Message"><div class="Tip Tip-error">$Messages</div></td>
                        </tr>
                        <tr class="ErrorMessageArrow">
                        <!--[EnabledUserPointList]-->
                        <td style="width:8em;"><!--[if $Parent($HasError($this($Type)))]--><div class="TipArrow">&nbsp;</div><!--[else]--><div class="TipArrow" style="visibility:hidden;">&nbsp;</div><!--[/if]--></td>
                        <!--[/EnabledUserPointList]-->
                        </tr>
                    </table>
                </td>
            </tr>
            <!--[/error]-->
			<tr>
			    <td>
			    <table style="width:auto;"><tr>
			    <!--[EnabledUserPointList]-->
				<td style="width:8em;"><input type="text" class="text" style="width:95%;" name="pointExchangeProportion.$pointID" value="$_form.text('pointExchangeProportion.$pointID',$PointExchangeProportions.GetProportion($type))" /></td>
                <!--[/EnabledUserPointList]-->
                </tr></table>
                </td>
			</tr>
		</tbody>
		</table>
		<div class="Actions">
            <input class="button" name="savepointExchangeProportions" type="submit" value="保存" />
        </div>
		</div>
	</form>
		
    <h3>兑换规则</h3>
    <form action="$_form.action" method="post">
    <div class="SearchTable">
        <table>
        <tr>
            <td style="80px;">允许从</td>
            <td>
                <select name="points">
                <option value="">请选择积分</option>
                <!--[EnabledUserPointList]-->
                <option value="$pointID" $_form.selected('points',$pointID)>$Name</option>
                <!--[/EnabledUserPointList]-->
                </select>
                兑换到
                <select name="targetpoints">
                <option value="">请选择积分</option>
                <!--[EnabledUserPointList]-->
                <option value="$pointID" $_form.selected('targetpoints',$pointID)>$Name</option>
                <!--[/EnabledUserPointList]-->
                </select>
            </td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td>
                <input class="button" name="addexchangerule" type="submit" value="添加" />
            </td>
        </tr>
        </table>
    </div>
    
    <!--[PointExchangeRuleList]-->
    <!--[head]-->
    <div class="DataTable">
    <h4>积分兑换规则列表</h4>
    <!--[if $hasItems]-->
    <table>
        <thead>
	        <tr>
		        <td>规则</td>
		        <td>兑换比例</td>
		        <td>兑换税率</td>
		        <td>兑换后剩余最低余额</td>
		        <td>操作</td>
	        </tr>
        </thead>
        <tbody>
    <!--[/if]-->
    <!--[/head]-->
    <!--[item]-->
        <!--[error line="$i"]-->
            <tr class="ErrorMessage">
                <td colspan="5" class="Message"><div class="Tip Tip-error">$Messages</div></td>
            </tr>
            <tr class="ErrorMessageArrow">
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td><!--[if $HasError("taxRate")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td><!--[if $HasError("minRemaining")]--><div class="TipArrow">&nbsp;</div><!--[else]-->&nbsp;<!--[/if]--></td>
                <td>&nbsp;</td>
            </tr>
       <!--[/error]-->
	        <tr>
		        <td>$PointName -&gt; $TargetPointName <input type="hidden" name="ids" value="$i" /><input type="hidden" name="key.$i" value="$Rule.Key" /></td>
		        <td>$Proportion：$TargetProportion</td>
		        <td><input type="text" class="text number" name="taxRate.$i" value="$_form.text('taxRate.$i',$Rule.TaxRate)" /> %</td>   
		        <td><input type="text" class="text number" name="minRemaining.$i" value="$_form.text('minRemaining.$i',$Rule.MinRemainingValue)" /></td> 
		        <td><a href="?action=delete&key=$Rule.Key">删除</a></td>
	        </tr>
    <!--[/item]-->
    <!--[foot]-->
    <!--[if $hasItems]-->
        </tbody>
    </table>
    <div class="Actions">
        <input class="button" name="savepointExchangeRules" type="submit" value="保存" />
    </div>
    <!--[else]-->
    <div class="NoData">当前没有积分兑换规则.</div>
    <!--[/if]-->
    </div>
    <!--[/foot]-->
    <!--[/PointExchangeRuleList]-->
    </form>

</div>
<!--[include src="../_foot_.aspx"/]-->
</body>
</html>
