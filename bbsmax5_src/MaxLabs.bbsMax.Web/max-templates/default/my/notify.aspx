<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
<script type="text/javascript">
function removeNotify(ids) {
        refresh();
//    if(ids)
//    {
//        for(var i=0;i<ids.length;i++)
//        {
//            delElement($('notify'+ids[i]));
//        }
//    }
    }
    function processNotify(id,action) {
        ajaxRequest('$url(_part/toolbar_notify)?ac=' + action + '&notifyid=' + id, function (r) {
            ajaxRender("", ['notifyList', 'notifyTypeList'],null,null);
            try {
              //  execInnerJavascript(r);
            }
            catch (e) { }
        });
    }

    function ignoreAll() {
        var url = '$dialog/notify-ignorebytype.aspx?type=0';
        ajaxPostForm("ignoreAllForm", url, "ignorenotify", function () {
            ajaxRender("", ['notifyList', 'notifyTypeList'], null, null);
         });
    }
</script>
</head>
<body>
<div class="container section-notify">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/megaphone.gif);">通知</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a class="current" href="$url(my/notify)"><span>通知</span></a></li>
                                        <li><a href="$url(my/notify-setting)"><span>通知接收设置</span></a></li>
                                    </ul>
                                </div>
                            </div>
                            <div class="clearfix workspace">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <!--[ajaxPanel id="notifyList"]-->
                                        <!--[if $TotalCount>0]-->
                                        <form action="$_form.action" id="form1" method="post">
                                        <div class="notifylist">
                                            <ul class="notify-list">
                                            <!--[if $notifyType==0 && $my.Systemnotifys.Count>0]-->
                                                <!--[loop $Notify in $my.Systemnotifys]-->
                                                <li class="notifyitem" id="sysnotify$Notify.NotifyID">
                                                    <div class="notify-entry">
                                                        <div class="notify-content">
                                                            <span><span class="unread">未处理</span>$Notify.content</span>
                                                            <span class="notify-date">$outputdatetime( $Notify.createdate)</span>
                                                        </div>
                                                    </div>
                                                    <div class="entry-action">
                                                        <input type="hidden" value="$Notify.notifyId" name="notifyid" />
                                                        <a class="action-banned" href="javascript:;" onclick="processNotify(-$Notify.notifyId,'ignore')" title="不再提醒">不再提醒</a>
                                                    </div>
                                                    <div class="clear">-</div>
                                                </li>
                                                <!--[/loop]-->
                                            <!--[/if]-->
                                            <!--[if $notifyType!=-1]-->  <!--非-1代表是用户的通知-->
                                                <!--[loop $Notify in $NotifyList]-->
                                                <li class="notifyitem" id="notify$Notify.NotifyID">
                                                    <div class="notify-entry">
                                                        <div class="notify-content">
                                                            <!--[if !$Notify.isRead]--><strong><span class="unread">未处理</span> <!--[/if]-->$Notify.content<!--[if !$Notify.isRead]--></strong><!--[/if]-->
                                                            <span class="notify-date">$outputfriendlydatetime($Notify.updateDate)</span>
                                                        </div>
                                                        <!--[if $Notify.hasaction]-->
                                                        <div class="notify-action">
                                                        <!--[loop $ac in $notify.actions]-->
                                                            <a href="$ac.url"<!--[if $ac.isdialog]--> onclick="return openDialog('$ac.url',refresh)"<!--[/if]-->><span>$ac.title</span></a>
                                                        <!--[/loop]-->
                                                        </div>
                                                        <!--[/if]-->
                                                    </div>
                                                    <div class="entry-action">
                                                        <input type="hidden" value="$Notify.notifyId" name="notifyid" />
                                                        <a class="action-delete" href="javascript:;" onclick="processNotify($Notify.notifyId,'delete')"  title="删除">删除</a>
                                                        <!--[if !$Notify.isRead  && $Notify.Keep]-->
                                                        <a class="action-banned" href="javascript:;" onclick="processNotify($Notify.notifyId,'ignore')" title="不再提醒">不再提醒</a>
                                                        <!--[/if]-->
                                                    </div>
                                                    <div class="clear">-</div>
                                                </li>
                                                <!--[/loop]-->
                                            <!--[else]-->
                                                <!--[loop $Notify in $MyAllSystemNotifies]-->
                                                <li class="notifyitem" id="sysnotify$Notify.ID">
                                                    <div class="notify-entry">
                                                        <div class="notify-content">
                                                            <!--[if $SysIsRead($Notify.NotifyId)]--><strong><span class="unread">未处理</span> <!--[/if]-->$Notify.content<!--[if $SysIsRead($Notify.NotifyId)]--></strong><!--[/if]-->
                                                            <span class="notify-date">$outputdatetime( $Notify.createdate)</span>
                                                        </div>
                                                    </div>
                                                    <div class="entry-action">
                                                        <input type="hidden" value="$Notify.notifyId" name="notifyid" />
                                                        <!--[if $SysIsRead($Notify.NotifyId)]-->
                                                        <a class="action-banned" href="javascript:;" onclick="processNotify(-$Notify.notifyId,'ignore')" title="不再提醒">不再提醒</a>
                                                        <!--[/if]-->
                                                    </div>
                                                    <div class="clear">-</div>
                                                </li>
                                                <!--[/loop]-->
                                            <!--[/if]-->
                                            </ul>
                                        </div>
                                        </form>
                                        
                                        <div class="clearfix notifycontrol">
                                            <!--[if $ShowIgnoreAllButton]-->
                                            <div class="notify-ignoreall">
                                            <form id="ignoreAllForm" action="$dialog/notify-ignorebytype.aspx?type=0" method="post">
                                                <a href="#" onclick="ignoreAll();return false;"><span>全部忽略</span></a>
                                            </form>
                                            </div>
                                            <!--[/if]-->
                                            <!--[pager name="pager1" skin="../_inc/_pager_app.aspx"]-->
                                        </div>
                                        
                                        <!--[else]-->
                                        <div class="nodata">
                                            没有相关的通知.
                                        </div>
                                        <!--[/if]-->
                                        <!--[/ajaxPanel]-->
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        <!--[ajaxPanel id="notifyTypeList"]-->
                                        <div class="notifycategory">
                                            <ul class="notifycategory-list">
                                                <li><a <!--[if $IsSelected('All')]-->class="current"<!--[/if]--> href="$url(my/notify)">全部
                                                </a>
                                                </li>
                                                <li><a <!--[if $notifyType==-1]-->class="current"<!--[/if]--> href="$url(my/notify)?type=-1">系统通知
                                                <!--[if $my.Systemnotifys.Count>0]-->
                                                <span class="counts">({=$my.Systemnotifys.Count})</span>
                                                <!--[/if]-->
                                                </a></li>
                                                <!--[loop $n in $NotifyTypeList]-->
                                                <li><a <!--[if $n.typeid==$notifyType]-->class="current"<!--[/if]--> href="$url(my/notify)?type=$n.typeid">$n.TypeName 
                                                <!--[if $my.unreadnotify[$n.typeid]>0]-->
                                                <span class="counts">({=$my.unreadnotify[$n.typeid]})</span>
                                                <!--[/if]-->
                                                </a></li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <script type="text/javascript">
                                            var notifyCount = $("sp_notify_count");
                                            if (notifyCount) {
                                                notifyCount.innerHTML = $my.totalunreadnotifies;
                                                setVisible(notifyCount, $my.totalunreadnotifies);
                                            }
                                        </script>
                                        <!--[/ajaxPanel]-->
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
            <!--#include file="../_inc/_sidebar_app.aspx"-->
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>

</body>
</html>
