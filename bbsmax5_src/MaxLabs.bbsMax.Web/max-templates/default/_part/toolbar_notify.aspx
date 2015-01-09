<!--[ajaxpanel id="toolbar_notify"]-->
<!--[if $my.totalunreadnotifies > 0]-->
<ul class="noticelist" id="notifies_list">
    <!--[loop $notify in $My.SystemNotifys]-->
    <li id="sysnotify_$notify.notifyid">
        <div class="entry">$notify.content <span class="date">$outputdate($notify.createdate)</span></div>
        <div class="action">
            <a href="$Dialog/notify-ignore.aspx?notifyid=-$notify.notifyid" onclick="return openDialog(this.href,this,function(r){removenotify('$notify.notifyid',true);});" title="不再提醒">不再提醒</a> 
        </div>
    </li>
    <!--[/loop]-->
    <!--[loop $notify in $TopNotifyList]-->
    <li id="notify_$notify.notifyid">
        <div class="entry">$notify.content <span class="date">$outputfriendlydatetime($notify.createdate)</span></div>
        <div class="action">
            <!--[loop $action in $notify.actions]-->
                <!--[if !$action.isdialog]-->
                <a href="$action.url">$action.title</a> |
                <!--[else]-->
                <a href="$action.url" onclick="openDialog(this.href,function(r){removenotify('$notify.notifyid',false);}); return false;">$action.title</a> |
                <!--[/if]-->
            <!--[/loop]-->
            <!--[if $notify.keep]-->
            <a href="$url(_part/toolbar_notify)?ac=ignore&notifyid=$notify.notifyid" onclick="return refreshNotify(this.href);">不再提醒</a> |
            <!--[/if]-->
            <a href="$url(_part/toolbar_notify)?ac=delete&notifyid=$notify.notifyid" onclick="return refreshNotify(this.href);">删除</a>
        </div>
    </li>
    <!--[/loop]-->
</ul>
<div class="clearfix messagemenu-link" id="allnotifyaction">
<!--[if !$EnablePassportClient ]-->
    <a class="link-ignore" href="$dialog/notify-ignorebytype.aspx?type=0" onclick=" return ignoreNotify(this.href);"><span>忽略全部</span></a>
    <a class="link-more" href="$HttpRoot$url(my/notify)"><span>查看全部</span></a>
    <!--[if $IsShowMore]-->
    <a class="link-expand" href="$HttpRoot$url(my/notify)" title="查看更多"><span>查看更多</span></a>
    <!--[/if]-->
<!--[else]-->
    <a class="link-ignore" href="$dialog/notify-ignorebytype.aspx?type=0" target="_blank" onclick=" return ignoreNotify(this.href);"><span>忽略全部</span></a>
    <a class="link-more" href="$PassportClient.CenterNotifyUrl" target="_blank"><span>查看全部</span></a>
    <!--[if $IsShowMore]-->
    <a class="link-expand" href="$PassportClient.CenterNotifyUrl" title="查看更多" target="_blank"><span>查看更多</span></a>
    <!--[/if]-->
<!--[/if]-->
</div>
<script type="text/javascript">
if(currentPanel) window.notifyLayer = currentPanel;
var notifyCount =$("sp_notify_count");

    function removenotify(id, system) {
      refreshNotify("$url(_part/toolbar_notify)");
      
    }
    function refreshNotify(url) {
        window.setTimeout('ajaxRender("'+url+'", "toolbar_notify");',40);
        return false;
    }

notifyCount.innerHTML=$my.totalunreadnotifies;
setVisible(notifyCount, 1);

function ignoreNotify(href) {
    var currentLayer = panel;
    openDialog(href, function (r) { setVisible($('sp_notify_count'), 0); currentLayer.close(); }); 
    return false;
}
</script>
<!--[else]-->
<script type="text/javascript">
var notifyCount =$("sp_notify_count");
setVisible(notifyCount, 0);
$("top_link_notify").onclick=null;
notifyLayer.close();
</script>
<!--[/if]-->
<!--[/ajaxpanel]-->