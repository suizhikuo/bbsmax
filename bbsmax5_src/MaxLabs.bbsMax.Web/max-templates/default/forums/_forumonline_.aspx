<div class="panel online">
    <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
        <h3 class="panel-title">
            <span>
                在线会员
                <a class="online-pagelink" href="$url(members)?view=onlineuser">详细在线列表</a>
            </span>
        </h3>
        <!--[if $IsOnlyShowCount==false]-->
        <!--[if $ShowOnlineMemberNum == 0 || $OnlineMemberCount <= $ShowOnlineMemberNum]-->
        <!--[if $displayOnline == DisplayStatus.Yes]-->
        <p class="panel-toggle"><a class="collapse" href="$AttachQueryString('displayOnline=no')" title="关闭列表">关闭列表</a></p>
        <!--[else]-->
        <p class="panel-toggle"><a class="expand" href="$AttachQueryString('displayOnline=yes')" title="打开列表">打开列表</a></p>
        <!--[/if]-->
        <!--[/if]-->
        <!--[/if]-->
    </div></div></div>
    <!--[if $displayOnline == DisplayStatus.Yes && $IsShowOnlineDetail]-->
    <div class="panel-body">
        <div  class="clearfix online-sign">
            <ul class="online-signlist">
                $GetOnlineRoleImgs(@"<li><img src=""{0}"" alt="""" /> {1}</li>")
            </ul>
        </div>
        <div class="clearfix online-users">
            <!--[if $displayMember == DisplayStatus.Yes]-->
            <script type="text/javascript">
                var memberHTML = '<a href="$url(space/[userID])" onmouseout="closeOnlineTip()" onmouseover="onlineUserTip(this, \'[userID]\',1)" target="_blank"><img src="{logoUrl}" alt="" />{nickName}</a>';
                var memberHTML2 = '<a href="$url(space/[userID])" onmouseout="closeOnlineTip()" onmouseover="onlineUserTip(this, \'[userID]\',1)" target="_blank" class="invisible"><img src="{logoUrl}" alt="" />{nickName}</a>';
                var invisibleHTML ='<span>$GetEveryoneIcon(@"<img src=""{0}"" alt="""" />") (隐身用户)</span>';
                $OnlineMemberScriptData
            </script>
            <!--[/if]-->
            <!--[if $IsShowOnlineGuestData]-->
            <script type="text/javascript">
                var guestHTML = '<a href="#" onmouseout="closeOnlineTip()" onmouseover="onlineUserTip(this, \'{guestID}\',0)" onclick="return false;">$GetOnlineGuestIcon(@"<img src=""{0}"" alt="""" />{1}")</a>';
                $OnlineGuestScriptData
            </script>
            <!--[/if]-->
        </div>
        
        <div id="onlineTemplate" class="dropdownmenu-wrap online-usertrack" style="display:none;  ">
            <div class="dropdownmenu">
                <div class="dropdownmenu-inner">
                    <ul>
                        <li>(隐身用户)</li>
                        <li><span class="label">操作:</span> <span class="value">{action}</span></li>
                        <li><span class="label">所在版块:</span> <span class="value">{forum}</span></li>
                        <li><span class="label">所在主题:</span> <span class="value">{thread}</span></li>
                        <li><span class="label">来访时间:</span> <span class="value">{createtime}</span></li>
                        <li><span class="label">活动时间:</span> <span class="value">{updatetime}</span></li>
                        <li><span class="label">IP:</span> <span class="value">{ip} &nbsp;&nbsp;&nbsp;{area}</span></li>
                        <li><span class="label">浏览器:</span> <span class="value">{browser}</span></li>
                        <li><span class="label">操作系统:</span> <span class="value">{os}</span></li>
                    </ul>
                </div>
            </div>
        </div>
        <!--[if $HasMoreOnline]-->
        <p class="online-showall">
            $TotalOnline 人在线, 仅显示$ShowOnlineCount位, <a href="$url(members)?view=onlineuser">点击此处查看更多</a>
        </p>
        <!--[/if]-->
        <!--[if $displayOnline == DisplayStatus.Yes]-->
        <p class="online-stat">
        <!--[if $IsForumPage == false]-->
            在线 <em>$TotalOnline</em>
            <!--[if $OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow]-->
            - 会员 <em>$OnlineMemberCount</em>
            - 游客 <em>$OnlineGuestCount</em>
            <!--[/if]-->
            - 最高记录是 <em>$MaxOnlineCount</em> 于 <em>$outputdatetime($MaxOnlineDate)</em>
        <!--[else]-->
            <!--[if $OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow]-->
            - 本版在线 <em>$forumOnlineCount</em>
            - 在线会员 <em>$forumOnlineMemberCount</em>
            - 在线$GuestRole.RoleName <em>$forumOnlineGuestCount</em>
            <!--[else]-->
            - 在线 <em>$TotalOnline</em>
            - 会员<em>$OnlineMemberCount</em>
            - $GuestRole.RoleName <em>$OnlineGuestCount</em>
            <!--[/if]-->
        <!--[/if]-->
        </p>
        <!--[/if]-->
    </div>
    <!--[else]-->
    <div class="panel-body">
        <p class="online-stat">
        <!--[if $IsForumPage == false]-->
            在线 <em>$TotalOnline</em>
            <!--[if $OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow]-->
            - 会员 <em>$OnlineMemberCount</em>
            - 游客 <em>$OnlineGuestCount</em>
            <!--[/if]-->
            - 最高记录是 <em>$MaxOnlineCount</em> 于 <em>$outputdatetime($MaxOnlineDate)</em>
        <!--[else]-->
            <!--[if $OnlineSetting.OnlineMemberShow != OnlineShowType.NeverShow]-->
            - 本版在线 <em>$forumOnlineCount</em>
            - 在线会员 <em>$forumOnlineMemberCount</em>
            - 在线$GuestRole.RoleName <em>$forumOnlineGuestCount</em>
            <!--[else]-->
            - 在线 <em>$TotalOnline</em>
            - 会员<em>$OnlineMemberCount</em>
            - $GuestRole.RoleName <em>$OnlineGuestCount</em>
            <!--[/if]-->
        <!--[/if]-->
        </p>
    </div>
    <!--[/if]-->
    <div class="panel-foot"><div><div>-</div></div></div>
</div>
<script type="text/javascript">

    var oUrl = "$url(handler/onlineinfo)?forumid=$currentForumid";
    var onlineRequesting = false;
    var online_isGetGuest = false, online_isGetMember = false;
    function onlineUserTip(e1, e2, isMember) {
       
        var _o = window._onlineusers;
        function showOnlineTip() {
            var item = _o[e2.toString()];
            
            if (!item) return;
            var onlineTemplate = $("onlineTemplate");
            var newTipObject = window._onlineTipPanel;
            if (!newTipObject) {
                newTipObject = addElement("div", document.body);
                newTipObject.style.position = "absolute";
                newTipObject.className = onlineTemplate.className;
                window._onlineTipPanel = newTipObject;
                window.onlinePopup = new popup(newTipObject,[]);
            }
            var cnt = onlineTemplate.innerHTML;

            for (var k in item) {
                cnt = cnt.replace("{" + k + "}", item[k]);
            }

            if (item.isSpider) {
                cnt = cnt.replace("操作系统","蜘蛛");
            }
       
            newTipObject.innerHTML = cnt;
            var ilist = newTipObject.getElementsByTagName("ul")[0];
            var listItems = ilist.getElementsByTagName("li");
            if (item.isSpider) ilist.removeChild(listItems[7]);
            if (!item.threadID) ilist.removeChild(listItems[3]);
            if (!item.forumID) ilist.removeChild(listItems[2]);
            if (!item.hidden) ilist.removeChild(listItems[0]);

            var p = window.onlinePopup;
            p.show({ target: e1 });
        }

        
        if ( ((isMember && !online_isGetMember)||(!isMember && !online_isGetGuest))  && onlineRequesting == false) {
            onlineRequesting = true;
            var ajax = new ajaxWorker(oUrl +(isMember? "&member=true":"&guest=true") + "&r=" + new Date().getMilliseconds(), "get", '');
            ajax.addListener(200, function(r) {
                if (r) {

                    eval("var temp = " + r + ";");
                    if (!_o)
                        _o = temp;
                    else {
                        for (var k in temp) {
                            _o[k] = temp[k];
                        }
                    }

                    window._onlineusers = _o;
                    showOnlineTip();
                    online_isGetGuest = !isMember;
                    online_isGetMember = isMember;
                    onlineRequesting = false;
                }
            });
            ajax.send();
        }
        else {
            if (!onlineRequesting)
                showOnlineTip();
        }
    }

    function closeOnlineTip() { var o = window._onlineTipPanel; if (o) window.onlinePopup.hide(); }
</script>
