<table class="FormTable">
<!--[error name="{=$Prefix}count"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	
<tr><th><h4>添加好友数量</h4>
      <input value="$_form.text('{=$Prefix}count',$out($Values['{=$Prefix}count']))"  class="text" name="{=$Prefix}count" type="text"/>
    </th>
<td>
请填写数字
</td>
</tr>
</table>