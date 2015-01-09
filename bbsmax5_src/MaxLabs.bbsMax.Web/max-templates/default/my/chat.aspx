<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
<script type="text/javascript">
function deleteChatSession(result) {
    if (result) refresh();
}
//<!--[if $beginChatUserID > 0]-->
addPageEndEvent(function(){
  window.setTimeout("openDialog('$dialog/chat.aspx?to=$beginChatUserID');",50);
});
//<!--[/if]-->
</script>
</head>
<body>
<div class="container section-chat">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/chat.gif);">聊天</span></h3>
                            </div>
                            <!--[if $ChatSessionList.Count > 0]-->
                            <div class="clearfix workspace">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <!--[ajaxpanel id="ap_chatmessage" idonly="true"]-->
                                        <!--[if $SelectedChatSession != null]-->
                                        <div class="msgloglist">
                                            <div class="clearfix msgloglist-head">
                                                <strong class="title">与$SelectedChatSession.User.Name的对话</strong>
                                                <span class="counts">$SelectedChatSession.TotalMessages 条记录</span>
                                            </div>
                                            <!--[if $ChatMessageList.Count > 0]-->
                                            <div class="msglog-list">
                                                <!--[loop $Message in $ChatMessageList]-->
                                                <!--[if $ChatMessageList.DateChanged]-->
                                                <h3 class="msglog-date">$outputdate($Message.CreateDate)</h3>
                                                <!--[/if]-->
                                                <div class="msglogitem <!--[if !$Message.IsReceive]--> msglogitem-my<!--[/if]-->">
                                                    <div class="msglog-title">
                                                        <!--[if $Message.IsReceive]-->
                                                        <a class="fn" href="$url(space/$Message.TargetUser.id)">$Message.TargetUser.Name</a>
                                                        <!--[else]-->
                                                        <a class="fn" href="$url(space/$My.id)">$My.Name</a>
                                                        <!--[/if]-->
                                                        <span class="published">$outputtime( $Message.CreateDate)</span>
                                                    </div>
                                                    <div class="msglog-content">
                                                        $Message.Content
                                                    </div>
                                                </div>
                                                <!--[/loop]-->
                                            </div>
                                            <!--[pager name="messagelist" ajaxpanelID="ap_chatmessage,ap_hidden" skin="../_inc/_pager_chat_reverse.aspx" ]-->
                                            <!--[/if]-->
                                        </div>
                                        <!--[else]-->
                                        <div class="nodata">
                                            请从左边的列表选择你要查看的聊天记录.<br />
                                            或者发起一个新的对话.
                                        </div>
                                        <form action="$_form.action" id="beginchatform" method="post">
                                        <div class="panel round-tl startchat">
                                            <div class="panel-body round-tr">
                                                <div class="round-bl">
                                                <div class="clearfix round-br">
                                                    <!--[ajaxpanel id="ap_hidden" idonly="true"]-->
                                                    <input type="hidden" name="to" value="$_form.text('to',$TargetUserID)" />
                                                    <input type="hidden" name="page" value="$_form.text('page',$PageNumber)" />
                                                    <input type="hidden" name="msgpage" value="$_form.text('msgpage',$MessagePageNumber)" />
                                                    <!--[/ajaxpanel]-->
                                                    <!--[unnamederror]-->
                                                    <div class="errormsg">$message</div>
                                                    <!--[/unnamederror]-->
                                                    <div class="clearfix startchatform">
                                                        <div class="startchatform-enter">
                                                            <input type="text" class="text" name="usernameToChat" value="输入用户名." onfocus="this.value='';this.style.color='black';" />
                                                        </div>
                                                        <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" name="beginChat" value="发起对话" /></span></span>
                                                    </div>
                                                </div>
                                                </div>
                                            </div>
                                        </div>
                                        </form>
                                        <!--[/if]-->
                                        <!--[/ajaxpanel]-->
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                    <!--[ajaxpanel id="ap_chatlist" idonly="true"]-->
                                        <form id="FormMessages" action="$_form.action" method="post">
                                        <div class="msglist">
                                        <!--[loop $ChatSession in $ChatSessionList]-->
                                            <div class="$_if($SelectedChatSessionID==$ChatSession.ID,'clearfix msgitem current','clearfix msgitem')" id="msg_$ChatSession.ID">
                                                <div class="msg-avatar mediumavatar">
                                                    <a href="$url(space/$chatSession.User.id)"><img src="$chatSession.User.SmallAvatarPath" alt="" width="24" height="24" /></a>
                                                </div>
                                                <div class="msg-entry">
                                                    <a href="$url(my/chat)?to=$ChatSession.UserID&page=$PageNumber" onclick="ajaxRender(this.href,'ap_chatlist,ap_chatmessage,ap_hidden');return false;">
                                                        <span class="author">$ChatSession.User.name</span>
                                                        <!--[if $ChatSession.UnreadMessages != 0]-->
                                                        <span class="msg-status">$ChatSession.UnreadMessages条</span>
                                                        <!--[/if]-->
                                                        <span class="updated">$ChatSession.FriendlyUpdateDate</span>
                                                        <span class="msg-lastmsg">$ChatSession.LastMessage</span>
                                                    </a>
                                                </div>
                                                <div class="entry-action">
                                                    <a class="action-chat" title="发起对话" href="$dialog/chat.aspx?to=$ChatSession.UserID" onclick="return parent.openDialog(this.href)">对话</a>
                                                    <a class="action-delete" title="清空对话记录" href="$dialog/chat-session-delete.aspx?tuid=$ChatSession.UserID" onclick="return openDialog(this.href,this,deleteChatSession);" id="a_delete_$ChatSession.UserID">删除</a>
                                                </div>
                                                <!--[if $SelectedChatSessionID == $ChatSession.ID]-->
                                                <div class="current-arrow">&nbsp;</div>
                                                <!--[/if]-->
                                            </div>
                                        <!--[/loop]-->
                                        </div>
                                        <!--[pager name="chatlist" skin="../_inc/_pager_app.aspx"]-->
                                        </form> 
                                    <!--[/ajaxpanel]-->
                                    </div>
                                </div>
                            </div>
                            <!--[else]-->
                            <div class="nodata">
                                暂时没有任何对话记录. 你可以尝试发起一个新的对话.
                            </div>
                            <form action="$_form.action" id="beginchatform" method="post">
                            <div class="panel round-tl startchat">
                                <div class="panel-body round-tr">
                                    <div class="round-bl">
                                    <div class="clearfix round-br">
                                        <!--[ajaxpanel id="ap_hidden" idonly="true"]-->
                                        <input type="hidden" name="to" value="$_form.text('to',$TargetUserID)" />
                                        <input type="hidden" name="page" value="$_form.text('page',$PageNumber)" />
                                        <input type="hidden" name="msgpage" value="$_form.text('msgpage',$MessagePageNumber)" />
                                        <!--[/ajaxpanel]-->
                                        <!--[unnamederror]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <div class="clearfix startchatform">
                                            <div class="startchatform-enter">
                                                <input type="text" class="text" name="usernameToChat" value="输入用户名或ID." onfocus="this.value='';this.style.color='black';" />
                                            </div>
                                            <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" name="beginChat" value="发起对话" /></span></span>
                                        </div>
                                    </div>
                                    </div>
                                </div>
                            </div>
                            </form>
                            <!--[/if]-->
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
