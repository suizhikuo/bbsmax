<table class="FormTable">
<tr>
	<th><h4>任务动作类型</h4>
      <input id="{=$Prefix}Radio1" value="1" name="{=$Prefix}action" onclick="displayAction();"  $_form.checked("{=$Prefix}action","1",$out($Values["{=$Prefix}action"])) type="radio" /><label for="{=$Prefix}Radio1">发表日志</label><br />
      <input id="{=$Prefix}Radio2" value="2" name="{=$Prefix}action" onclick="displayAction();"  $_form.checked("{=$Prefix}action","2",$out($Values["{=$Prefix}action"])) type="radio" /><label for="{=$Prefix}Radio2">评论日志</label><br />
    </th>
	<td></td>
  
</tr>

<!--[error name="{=$Prefix}articleID"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	
<tr id="{=$Prefix}action2-1"><th><h4>指定被评论的日志ID</h4>

  <input type="text" class="text" name="{=$Prefix}articleID" value="$_form.text('{=$Prefix}articleID',$out($Values['{=$Prefix}articleID']))" />
  </th><td>填日志ID，用于限制用户发表回复必须是针对该篇日志的，如果留空则不限制</td>
  
</tr>

<!--[error name="{=$Prefix}userID"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	

<tr id="{=$Prefix}action2-2"><th><h4>指定被评论的日志的作者ID</h4>
  
  <input type="text" class="text" name="{=$Prefix}userID" value="$_form.text('{=$Prefix}userID',$out($Values['{=$Prefix}userID']))" />
  </th><td>填写日志作者的用户ID（如果填写了日志ID就不必再指定作者ID），用以限制用户必发表回复必须是针对该作者所发表的日志的，如果留空则不限制，</td>
  
</tr>

<!--[error name="{=$Prefix}count"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	

<tr><th><h4>需要达到的数量</h4>

  <input type="text" class="text" name="{=$Prefix}count" value="$_form.text('{=$Prefix}count',$out($Values['{=$Prefix}count']))" />
  </th><td>值必须大于0 </td>
  
</tr>

<!--[error name="{=$Prefix}timeout"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	

<tr><th><h4>完成任务的时间限制(小时)</h4>
  
  <input type="text" class="text" name="{=$Prefix}timeout"  value="$_form.text('{=$Prefix}timeout',$out($Values['{=$Prefix}timeout']))" />
  </th><td>设置会员从申领任务到完成任务的时间限制，如果超出这个设置，则认为会员任务失败。如果不限制会员执行任务的时间，请设置为0或者留空</td>
  
</tr>
</table>
<script type="text/javascript">
initDisplay('{=$Prefix}action',[
 { value : '1' , display  : false, id : '{=$Prefix}action2-1'}
,{ value : '1' , display  : false, id : '{=$Prefix}action2-2'}
,{ value : '2' ,  display  : true,  id : '{=$Prefix}action2-1'}
,{ value : '2' ,  display  : true,  id : '{=$Prefix}action2-2'}
]); 
</script>