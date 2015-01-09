<div class="FormTable">
<table>
<tr>
	<th>
	    <h4>任务动作类型</h4>
        <input id="{=$Prefix}Radio1" value="1" name="{=$Prefix}action" onclick="displayAction();"  $_form.checked("{=$Prefix}action","1",$out($Values["{=$Prefix}action"])) type="radio" /><label for="{=$Prefix}Radio1">上传图片</label><br />
    </th>
	<td>&nbsp;</td>
</tr>
<!--[error name="{=$Prefix}count"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	
<tr>
    <th>
        <h4>需要达到的数量</h4>
        <input type="text" class="text number" name="{=$Prefix}count" value="$_form.text('{=$Prefix}count',$out($Values['{=$Prefix}count']))" />
    </th>
    <td>值必须大于0 </td>
</tr>
<!--[error name="{=$Prefix}timeout"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	
<tr>
    <th>
        <h4>完成任务的时间限制(小时)</h4>
        <input type="text" class="text" name="{=$Prefix}timeout"  value="$_form.text('{=$Prefix}timeout',$out($Values['{=$Prefix}timeout']))" />
    </th>
    <td>设置会员从申领任务到完成任务的时间限制，如果超出这个设置，则认为会员任务失败。如果不限制会员执行任务的时间，请设置为0或者留空</td>
</tr>
</table>
</div>