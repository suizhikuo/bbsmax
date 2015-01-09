<!--[DialogMaster width="550"]-->
<!--[place id="body"]-->
<div class="clearfix dialoghead" id="dialogTitleBar_$PanelID">
    <h3 class="dialogchat-user"><a href="$dialog/user-profiles.aspx?uid=$ChatUser.id" onclick="return openDialog(this.href);">$ChatUser.Name</a></h3>
    <!--[if $ChatUser.doing != ""]-->
    <p class="dialogchat-userstatus">$ChatUser.doing</p>
    <!--[/if]-->
    <div class="dialogclose"><a href="javascript:;" id="max_chatclosebutton" accesskey="Q" title="关闭">关闭(<u>Q</u>)</a></div>
    <script type="text/javascript">
        maxDragObject(currentPanel.panel, $('dialogTitleBar_$PanelID'));
        $("max_chatclosebutton").onclick = function (e) {
            panel.close(); 
           //endEvent(e);
        }
    </script>
</div>
<form action="/" method="post" id="max_chatform">
<input type="hidden" name="tuid" value="$ToUserID" />
<input type="hidden" name="issend" value="true" />
<div class="clearfix dialogchatlayout">
    <div class="dialogchatcontent">
        <div class="dialogchatcontent-inner">
            <div class="clearfix dialogchatmsg-wrap">
                <div class="clearfix dialogchatmsg">
                    <div class="dialogchatmsg-inner">
                        <div class="dialogchatmsg-scroll" id="max_chat_scrollwrapper">
                            <!--[loop $Message in $ChatMessageList]-->
                            <!--[if $NewDay($Message.CreateDate)]--><h3 class="chatmsgdate">$outputdate($Message.CreateDate)</h3><!--[/if]-->
                            <div class="chatmsgitem $_if($Message.IsReceive==false,'chatmsgitem-my')">
                                <div class="bubble-top"><div>&nbsp;</div></div>
                                <div class="clearfix bubble-middle">
                                    <div class="chatmsgentry">
                                        $Message.Content
                                        <span class="date">$outputtime($Message.CreateDate)</span>
                                    </div>
                                </div>
                                <div class="bubble-bottom"><div>&nbsp;</div></div>
                            </div>
                            <!--[/loop]-->
                        </div>
                    </div>
                </div>
                <div class="dialogchaterror" id="max_chat_errorlabel" style="display:none;"><div>message</div></div>
            </div>
            <div class="clearfix dialogchattool">
                <ul>
                    <!--[if $CanUseDefaultEmotcion|| $CanUseUserEmoticon]-->
                    <li><a class="emoticon" href="javascript:void(0);" id="max_chat_face" onclick="return openFace();">表情</a></li>
                    <!--[/if]-->
                    <li><a class="chatlog" href="$url(my/chat)?to=$ChatUser.ID" target="_blank">聊天记录</a></li>
                </ul>
            </div>
            <div class="clearfix dialogchatform">
                <div class="textarea"><div class="inner"><div>
                <textarea id="max_chat_message" name="content" cols="30" rows="2"></textarea>
                </div></div></div>
                <div class="sendmsgbutton"><input type="button" name="sendmessages" id="buttonsend" onclick="return onsend();" value="发送" /></div>
            </div>
            <!--[validateCode actionType="$validateActionName"]-->
            <div class="clearfix dialogchatcaptcha" id="max_chat_vcode">
                <input type="text" class="text validcode" name="$inputName" onkeypress="return onvcodeEnter();" id="validatecode" $_if($disableIme,'style="ime-mode:disabled;"') autocomplete="off" />
                <span class="captcha">
                    <img alt="" src="$imageurl" title="看不清,点击刷新" onclick="this.src=this.src+'&rnd=' + Math.random();" />
                </span>
                <div class="note">$tip</div>
            </div>
            <!--[/validateCode]-->
        </div>
    </div>
    <div class="dialogchatuser">
        <div class="dialogchatuser-inner">
            <div class="dialogchat-avatar">
                <a href="$url(space/$ChatUser.ID)" target="_blank">$ChatUser.bigAvatar</a>
            </div>
            <ul class="dialogchat-useraction">
                <li><a class="addfriend" href="$dialog/friend-tryadd.aspx?uid=$ChatUser.ID" onclick="return openDialog(this.href)">加为好友</a></li>
                <li><a class="space" href="$url(space/$ChatUser.ID)" target="_blank">访问他的空间</a></li>
                <li><a class="backlist" href="$dialog/blacklist-add.aspx?uid=$ChatUser.ID" onclick="return openDialog(this.href)">拉入黑名单</a></li>
            </ul>
        </div>
    </div>
    <div class="imgperloader" style="background-image:url($root/max-templates/default/images/dialogchat_bubble.png)"></div>
    <div class="imgperloader" style="background-image:url($root/max-templates/default/images/dialogchat_bubble_alt.png)"></div>
</div>
<!--[if $HasSound]-->
<object style="display:none;" id="soundplayer" classid="clsid:6BF52A52-394A-11D3-B153-00C04F79FAA6" type="application/x-oleobject" codebase="http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,7,1112" width="0" height="0"><param name="url" value="#" /><param name="autoStart" value="0" /></object>
<!--[/if]-->
</form>
<script type="text/javascript">
//<!--[if $HasSound]-->
    function playSound() {//只支持IE的提示音
        var s = "$Bgsound";
        try {
            var p = $("soundplayer");
            var m;
            if (s) {
                p.controls.stop();
                p.url = s;
                p.controls.play();
            }
        }
        catch (e) {
        }
    }
    //<!--[/if]-->

    function onvcodeEnter(ev) {
        var e = ev || window.event;
        if (e && (e.keyCode == 13 || e.which == 13) && !e.ctrlKey) {
            onsend();
            return false;
        }
        return true;
    }

    var isSend = false;
    var isRecive = false;
    function onsend() {
        if (isRecive) {
            window.setTimeout("onsend()", 50);
            return;
        }
    
        isSend = true;
        if (btnSend.disabled)
            return;
        var u = String.format(chatHandlerUrl, maxMessageID);
        $("max_chatform").action = u;
        var re = ajaxSubmit('max_chatform', 'sendmessages', '*', function(r) {
            processResult(r); btnSend.disabled = false;
            removeCssClass(btnSend, "disable");
            var m = $('max_chat_message');
            m.focus();
        }, false);

        btnSend.disabled = true;
        addCssClass(btnSend, "disable");
        return re;
    }

    function autoget() {
        if (window.chatThread) {
            clearInterval(window.chatThread);
        }
        window.chatThread = setInterval("getNewMessage()", 5000);
    }
    var chatHandlerUrl = "$url(handler/message)?tuid=$ToUserID&maxid={0}&count=200";
    var messageTemplate = '<div class="bubble-top"><div>&nbsp;</div></div>\
                                <div class="clearfix bubble-middle">\
                                    <div class="chatmsgentry">\
                                       {0}\
                                        <span class="date">{1}</span>\
                                    </div>\
                                </div>\
                                <div class="bubble-bottom"><div>&nbsp;</div></div>';

    function appendMessage(m) {

        if (m.length && mc) {
            while (mc.childNodes.lenght >= (200 - m.length)) {
                mc.removeChild(mc.childNodes[0]);
            }
        }

        for (var i = 0; i < m.length; i++) {
            if (m[i].MessageID > maxMessageID && m[i].MessageID > 0)
                maxMessageID = m[i].MessageID;
            var item = addElement("div", mc);
            var cssClass = "chatmsgitem";
            cssClass += m[i].IsReceive ? "" : ' chatmsgitem-my';
            addCssClass(item, cssClass);

             var tm = /\d+:\d+(?::\d+)?$/.exec(m[i].CreateDate);
            item.innerHTML = String.format(messageTemplate, m[i].Content,tm);
        }
        scrollToBottom('max_chat_scrollwrapper');

        if (window.playSound && !isSend)
            playSound();
    }

    var ajax;

    function processResult(r) {
        if (r) {
        try{
             eval("var result =" + r);
            }
            catch(e){
                //alert(e.message +" r=" + r);
            }
            if (result) {
                if (result.state == 0) {//成功
                    appendMessage(result.data);
                    var m = $('max_chat_message');
                    if (isSend) m.value = '';                    
                    var codeDiv = $("max_chat_vcode");
                    if (codeDiv) {
                        setVisible(codeDiv, false);
                    }
                }
                else if (result.state == 1)  //错误
                {
                    showError(result.data);

                }
                else if (result.state == 2) {//验证码错误
                    showError(result.data);
                    var codeDiv = $("max_chat_vcode");
                    if(codeDiv)
                    {
                        setVisible(codeDiv, true);
                    }
                }
            }
        }

        isSend = false;
        isRecive = false;
    }

    function getNewMessage() {
        if (btnSend.disabled) //正在发送消息、暂停接收
            return;

        ajax = null;
        var u = String.format(chatHandlerUrl, maxMessageID, new Date().getMilliseconds());
        ajax = new ajaxWorker(u, "get", '');
        ajax.addListener(200, processResult);
        ajax.send(u);
        isRecive = true;
    }
    function max_chat_addFace(r) {
        $('max_chat_message').value += r.ubb;
        isOpenFace = false;
    }
    var maxMessageID = $MaxMessageID;
    var isOpenFace = false;
    addHandler(document.documentElement, 'click', function() { if (isOpenFace) { window.facePanel.close(); isOpenFace = false; } });
    function openFace() {
        if (!isOpenFace) {
            var url = root + '/max-dialogs/editor/emoticons.aspx?defalut=$CanUseDefaultEmotcion&user=$CanUseUserEmoticon&ispanel=true&callback=max_chat_addFace';
            window.facePanel = openPanel(url, $('max_chat_face'), null, 390, 310, "top");
            window.setTimeout('isOpenFace=true;', 10);
        }
        return false;
    }

    function showError(msg) {
        var label = $("max_chat_errorlabel");
        label.style.display = "";
        label.childNodes[0].innerHTML = msg;
        setTimeout('$("max_chat_errorlabel").style.display="none";', 2000);        
    }

    var mc = $('max_chat_scrollwrapper'); var btnSend = $('buttonsend');
    var result = 1;

    onCtrlEnter($("max_chatform"), onsend);
    
  //  ctrlEnterEvent(onsend, 'max_chat_message');
    autoget();
   // $('max_chat_message').focus();
    mc.scrollTop = 65536;
    currentPanel.result = 1;
    currentPanel.addCloseHandler(function () { window.clearInterval(window.chatThread); });

    if (window.needUpdateChatSession)  //由顶上的未读消息列表传递过来的参数
    {
        ajaxRender("$url(_part/toolbar_message)", "toolbar_message");
        window.needUpdateChatSession = false;
    }
</script>
<!--[/place]-->
<!--[/dialogmaster]-->
