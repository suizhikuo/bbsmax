<!--[ajaxpanel id="toolbar_message" idonly="true"]-->
<!--[if $SessionList.count > 0]-->
<ul class="messagelist" id="toolbar_topmessage">
    <!--[loop $s in $SessionList]-->
    <li class="clearfix" id="message_$s.id">
        <a class="clearfix msgitem" href="$dialog/chat.aspx?to=$s.userid" onclick="return openMessage(this.href,$s.id,$s.UnreadMessages)">
            <span class="avatar"><img src="$s.user.smallavatarpath" alt="" width="24" height="24" /></span>
            <span class="name">$s.user.name</span>
            <span class="unread" id="unread_$s.id">$s.UnreadMessages</span>
            <span class="date">$outputfriendlydate($s.updatedate)</span>
            <span class="summary"><span class="lastmsg">$s.LastMessage</span></span>
        </a>
        <a class="msgignore" href="javascript:;" onclick="ignoreSession($s.userid)" title="忽略">忽略</a>
    </li>
    <!--[/loop]-->
</ul>
<!--[if !$EnablePassportClient ]-->
<div class="clearfix messagemenu-link">
    <a class="link-ignore" href="$url(_part/toolbar_message)?ignore=1" onclick="return ignoreAllMessage(this.href)"><span>忽略全部</span></a>
    <a class="link-more" href="$HttpRoot$url(my/chat)"><span>查看全部</span></a>
    <!--[if $SessionList.TotalRecords > $SessionList.count]-->
    <a class="link-expand" href="$HttpRoot$url(my/chat)"><span>查看更多</span></a>
    <!--[/if]-->
</div>
<!--[else]-->
<div class="clearfix messagemenu-link">
    <a class="link-ignore" href="$url(_part/toolbar_message)?ignore=1" onclick="return ignoreAllMessage(this.href)" target="_blank"><span>忽略全部</span></a>
    <a class="link-more" href="$PassportClient.CenterNotifyUrl" target="_blank"><span>查看全部</span></a>
    <!--[if $SessionList.TotalRecords > $SessionList.count]-->
    <a class="link-expand" href="$PassportClient.CenterNotifyUrl" target="_blank"><span>查看更多</span></a>
    <!--[/if]-->
</div>
<!--[/if]-->
<script type="text/javascript">
    var messageCount = $("sp_message_count");
    if (!window.messagePanel) messagePanel = currentPanel;
function ignoreAllMessage(url)
{
    ajaxRequest(url);
    setVisible(messageCount,0);
    panel.close();
    return false;
}

function ignoreSession(userid) {
    var url = "$url(_part/toolbar_message)?ignoresession=1&userid=" + userid;
    ajaxRender(url,"toolbar_message");
}

function openMessage(url,id,c)
{
    var item = $("message_" + id);
    window.needUpdateChatSession = true; //这个是通知聊天对话框更新顶上未读消息
   openDialog(url); 
   return false;
}

var _mcount = $my.UnreadMessages;

messageCount.innerHTML =_mcount;
setVisible(messageCount,1);
</script>
<!--[else]-->
<script type="text/javascript">
var messageCount =$("sp_message_count");
setVisible(messageCount, 0);
$("top_link_message").onclick = null;
if (currentPanel) currentPanel.close();
if (window.messagePanel) { messagePanel.close();messagePanel = null; }
</script>
<!--[/if]-->
<!--[/ajaxpanel]-->