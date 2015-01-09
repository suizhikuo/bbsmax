<table class="FormTable">
<tr><th><h4>动作</h4>
      <input id="{=$Prefix}Radio1" value="0" name="{=$Prefix}action" onclick="displayAction();"  $_form.checked("{=$Prefix}action","0",$out($Values["{=$Prefix}action"])) type="radio" /><label for="{=$Prefix}Radio1">发主题</label><br />
      <input id="{=$Prefix}Radio2" value="1" name="{=$Prefix}action" onclick="displayAction();"  $_form.checked("{=$Prefix}action","1",$out($Values["{=$Prefix}action"])) type="radio" /><label for="{=$Prefix}Radio2">发回复</label><br />
      <input id="{=$Prefix}Radio3" value="2" name="{=$Prefix}action" onclick="displayAction();"  $_form.checked("{=$Prefix}action","2",$out($Values["{=$Prefix}action"])) type="radio" /><label for="{=$Prefix}Radio3">发主题/回复</label>
    </th>
<td></td>
  
</tr>

<tr><th><h4>指定版块</h4>
<!--[loop $forum in $Forums with $i]-->
  $ForumSeparators[$i] <input type="checkbox" value="$forum.ForumID" $_form.checked("{=$Prefix}forumIDs","$forum.ForumID",$out($Values["{=$Prefix}forumIDs"])) id="forum_$forum.ForumID" name="{=$Prefix}forumIDs" /><label for="forum_$forum.ForumID">$forum.ForumName</label><br />
<!--[/loop]-->
  </th><td>用户只能在选中版块发帖才能完成任务</td>
  
</tr>

<!--[error name="{=$Prefix}replytopic"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	
<tr id="{=$Prefix}action2-1"><th><h4>回复指定主题</h4>

  <input type="text" class="text" name="{=$Prefix}replytopic" value="$_form.text('{=$Prefix}replytopic',$out($Values['{=$Prefix}replytopic']))" />
  </th><td>回复指定主题，请填写该主题ID。如果留空则不限制</td>
  
</tr>

<!--[error name="{=$Prefix}replyuser"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	

<tr id="{=$Prefix}action2-2"><th><h4>回复指定作者</h4>
  
  <input type="text" class="text" name="{=$Prefix}replyuser" value="$_form.text('{=$Prefix}replyuser',$out($Values['{=$Prefix}replyuser']))" />
  </th><td>回复指定作者，只有回复该作者发表的主题才可以，请填写作者的用户ID。留空则不限制</td>
  
</tr>

<!--[error name="{=$Prefix}topiccount"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	

<tr><th><h4>发帖数量</h4>

  <input type="text" class="text" name="{=$Prefix}topiccount" value="$_form.text('{=$Prefix}topiccount',$out($Values['{=$Prefix}topiccount']))" />
  </th><td>必须大于0 </td>
  
</tr>

<!--[error name="{=$Prefix}timeout"]-->
<tr>
    <td colspan="2" class="Message">
        <div class="Tip Tip-error">$Message</div>
        <div class="TipArrow">&nbsp;</div>
    </td>
</tr>
<!--[/error]-->	

<tr><th><h4>时间限制(小时)</h4>
  
  <input type="text" class="text" name="{=$Prefix}timeout"  value="$_form.text('{=$Prefix}timeout',$out($Values['{=$Prefix}timeout']))" />
  </th><td>设置会员从申领任务到完成任务的时间限制，如果超出这个设置，则认为会员任务失败。如果不限制会员执行任务的时间，请设置为0或者留空</td>
  
</tr>
</table>
<script>
displayAction();
function displayAction()
{
    var obj = document.getElementsByName('{=$Prefix}action');
    var action2_1 = document.getElementById('{=$Prefix}action2-1');
    var action2_2 = document.getElementById('{=$Prefix}action2-2');
    
    var len=obj.length;
    var selectInt;
    for(var i=0;i<len;i++){
     if(obj[i].checked==true){
      selectInt=obj[i].value;
     }
    }
    
    if(selectInt == '1')
    {
        action2_1.style.display='';
        action2_2.style.display='';
    }
    else
    {
        action2_1.style.display='none';
        action2_2.style.display='none';
    }
}
</script>