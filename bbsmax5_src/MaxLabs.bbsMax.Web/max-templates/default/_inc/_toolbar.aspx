<div class="toolbar">
    <div class="clearfix toolbar-inner">
        <!--[if $ParentTopLinks.Count>0]-->
        <script type="text/javascript">
            var topBarMenu = {
                create: function (target, menucontents) {
                    if (!document.getElementById(menucontents)) {
                        return;
                    }
                    var contents_wrap = document.getElementById(menucontents);
                    var contents = contents_wrap.innerHTML;
                    target.className += " hover";
                    var dropdownmenu_wrap = document.createElement("div");
                    dropdownmenu_wrap.className = "dropdownmenu-wrap";
                    var dropdownmenu = document.createElement("div");
                    dropdownmenu.className = "dropdownmenu";
                    dropdownmenu.style.width = "auto";
                    var dropdownmenu_inner = document.createElement("div");
                    dropdownmenu_inner.className = "dropdownmenu-inner";
                    dropdownmenu_wrap.appendChild(dropdownmenu);
                    dropdownmenu.appendChild(dropdownmenu_inner);
                    dropdownmenu_inner.innerHTML = contents;
                    if (target.getElementsByTagName("div").length == 0) {
                        target.appendChild(dropdownmenu_wrap);
                    }
                },
                clear: function (target) {
                    target.className = target.className.replace("hover", "");
                }
            }
        </script>
        <ul id="topLinks" class="accesslink">
            <!--[loop $item in $ParentTopLinks]-->
            <!--[if $GetChildItems($item).Count>0]-->
            <li onmouseover="topBarMenu.create(this,'topLinks_menu_$item.ID');" onmouseout="topBarMenu.clear(this);">
                <a class="item-expand" href="$item.url" $_if($item.NewWindow,'target="_blank"','')><span>$item.name</span></a>
            </li>
            <!--[else]-->
            <li><a href="$item.url" $_if($item.NewWindow,'target="_blank"','')><span>$item.name</span></a></li>
            <!--[/if]-->
            <!--[/loop]-->
        </ul>
        <!--[loop $item in $ParentTopLinks]-->
        <!--[if $GetChildItems($item).Count>0]-->
        <div id="topLinks_menu_$item.ID" class="topbar-hiddencontents">
            <!--[loop $childItem in $GetChildItems($item)]-->
            <a href="$childItem.url" $_if($childItem.NewWindow,'target="_blank"','')><span>$childItem.name</span></a>
            <!--[/loop]-->
        </div>
        <!--[/if]-->
        <!--[/loop]-->
        <!--[/if]-->

        <div class="userbar">
    <!--[if $IsLogin]-->
        <!--[if !$EnablePassportClient ]-->
            <a class="username item-expand" href="$url(my/default)" id="top_link_center"><span>$My.username</span></a>
        <!--[else]-->
            <a class="username item-expand" href="$PassportClient.CenterUrl" id="top_link_center" target="_blank"><span>$My.username</span></a>
        <!--[/if]-->
            <!--[if $my.NeedInputInviteSerial]-->
            <a href="$url(my/friends-invite)">激活帐号</a>
            <!--[else if $EnableInvitation]-->
            <a href="$url(my/friends-invite)">邀请</a>
            <!--[/if]-->
            <!--[if  $CanManageDenouncing && $DenouncingCount > 0]-->
            <a href="javascript:;" id="reportmenu_dropdown" onclick="return openAjaxLayer('$url(_part/toolbar_report)',this,'managereportmenu',0,0,'center')">举报<span class="counts">$DenouncingCount</span></a>
            <!--[/if]-->
        <!--[if !$EnablePassportClient ]-->
            <!--[if $EnableChatFunction]-->
            <a href="$url(my/chat)" id="top_link_message" $_if($my.UnreadMessages == 0,'','onclick="return openTopbarLayer(\'$url(_part/toolbar_message)\',this,\'popupbubble menu-message\',0,0,\'right\')"')>消息<!--[if $my.UnreadMessages > 0]--><span class="counts" id="sp_message_count">$my.UnreadMessages</span><!--[/if]--></a>
            <!--[/if]-->
            <a href="$url(my/notify)" id="top_link_notify" $_if($my.TotalUnreadNotifies == 0,'','onclick="return openTopbarLayer(\'$url(_part/toolbar_notify)\',this,\'popupbubble menu-notice\',0,0,\'right\')"')>通知<!--[if $my.TotalUnreadNotifies > 0]--><span class="counts" id="sp_notify_count">$my.TotalUnreadNotifies</span><!--[/if]--></a>
        <!--[else]-->
            <!--[if $EnableChatFunction]-->
            <a href="$PassportClient.CenterChatUrl" target="_blank" id="top_link_message" $_if($my.UnreadMessages == 0,'','onclick="return openTopbarLayer(\'$url(_part/toolbar_message)\',this,\'popupbubble menu-message\',0,0,\'right\')"')>消息<!--[if $my.UnreadMessages > 0]--><span class="counts" id="sp_message_count">$my.UnreadMessages</span><!--[/if]--></a>
            <!--[/if]-->
            <a href="$PassportClient.CenterNotifyUrl" target="_blank" id="top_link_notify" $_if($my.TotalUnreadNotifies == 0,'','onclick="return openTopbarLayer(\'$url(_part/toolbar_notify)\',this,\'popupbubble menu-notice\',0,0,\'right\')"')>通知<!--[if $my.TotalUnreadNotifies > 0]--><span class="counts" id="sp_notify_count">$my.TotalUnreadNotifies</span><!--[/if]--></a>
        <!--[/if]-->
        <!--[if $my.UnreadMessages > 0 || $my.TotalUnreadNotifies > 0]-->
            <div style="display:none">
            $MessageSound
            </div>
        <!--[/if]-->

            <a href="javascript:;" id="friend_dropdown" onclick="return openFriendList('$url(_part/toolbar_friend)?default=true',this,'menu-friend',0,0)"><span>好友</span></a>
            <!--[if $My.CanLoginConsole]-->
            <a href="$root/max-admin/default.aspx" target="_blank">管理</a>
            <!--[/if]-->
            <a href="$url(logout/$My.ID)">退出</a>
    <!--[else]-->
            <a href="$url(login)" $_if($UseDialogLogin,'onclick="return openDialog(\'$dialog/login.aspx\',refresh);"')>立即登录</a>
            <a href="$url(register)">注册新帐号</a>
    <!--[/if]-->
        </div>
        
    </div>
</div>

<!--[if $IsLogin]-->
<div class="dropdownmenu-wrap menu-setting" id="settingpopup" style="display:none;">
    <div class="dropdownmenu">
        <div class="clearfix dropdownmenu-inner">
            <div class="clearfix menu-setting-account">
                <!--[if !$EnablePassportClient ]-->
                <a class="avatar" href="$url(my/avatar)"><img src="$my.smallAvatarPath" alt="" /></a>
                <!--[else]-->
                <a class="avatar" href="$PassportClient.SettingAvatarUrl" target="_blank"><img src="$my.smallAvatarPath" alt="" /></a>
                <!--[/if]-->
                <!--[if $My.IsOnline]-->
                <span class="visible">在线</span> <a href="$SetInvisibleUrl" title="当前在线状态, 点击切换为隐身.">[切换]</a>
                <!--[else]-->
                <span class="invisible">隐身</span> <a href="$SetOnlineUrl" title="当前隐身状态, 点击切换为在线.">[切换]</a>
                <!--[/if]-->
            </div>
            <ul class="clearfix menu-setting-list">
            <!--[if !$EnablePassportClient ]-->
                <li><a href="$url(my/setting)">修改资料</a></li>
                <li><a href="$url(my/avatar)">修改头像</a></li>
                <li><a href="$url(my/changepassword)">修改密码</a></li>
                <li><a href="$url(my/changeemail)">修改邮箱</a></li>
                <!--[if $EnableMobileBind]-->
                <li><a href="$url(my/bindmobile)">手机绑定</a></li>
                <!--[/if]-->
                <!--[if $EnableRealnameCheck]-->
                <li><a href="$url(my/realname)">实名认证</a></li>
                <!--[/if]-->
                <li><a href="$url(my/notify-setting)">通知设置</a></li>
            <!--[else]-->
                <li><a href="$PassportClient.SettingProfileUrl" target="_blank">修改资料</a></li>
                <li><a href="$PassportClient.SettingAvatarUrl" target="_blank">修改头像</a></li>
                <li><a href="$PassportClient.SettingPasswordUrl" target="_blank">修改密码</a></li>
                <li><a href="$PassportClient.SettingChangeEmailUrl" target="_blank">修改邮箱</a></li>
                <li><a href="$PassportClient.SettingNotifyUrl" target="_blank">通知设置</a></li>
            <!--[/if]-->
                <li><a href="$url(my/privacy)">隐私设置</a></li>
                <li><a href="$url(my/feedfilter)">动态过滤</a></li>
                <li><a href="$url(my/spacestyle)">空间风格</a></li>
                <li><a href="$url(my/point)">我的积分</a></li>
                <!--[if $EnablePointExchange]-->
                <li><a href="$url(my/point-exchange)">积分兑换</a></li>
                <!--[/if]-->
                <!--[if $EnablePointTransfer]-->
                <li><a href="$url(my/point-transfer)">积分转账</a></li>
                <!--[/if]-->
                <!--[if $EnablePointRecharge]-->
                <li><a href="$url(my/pay)">积分充值</a></li>
                <!--[/if]-->
            </ul>
        </div>
    </div>
</div>
<script type="text/javascript">
    var setPopup = new popup("settingpopup", "top_link_center", true, "hover");
    setPopup.offsetLeft = -1;
    setPopup.offsetTop = -2;
</script>
<!--[/if]-->

