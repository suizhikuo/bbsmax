<div class="menufriend-head">
    <div class="clearfix menufriend-head-inner">
        <div class="closer"><a href="javascript:;" id="max_friendpanel" onclick="panel.close();" title="关闭">关闭</a></div>
        <div class="avatar"><a href="$url(space/$my.id)">$my.avatar</a></div>
        <div class="username" id="friend_title">$my.username</div>
        <div class="usertagline-wrap" onmouseover="addCssClass(this,'usertagline-hover')" onmouseout="removeCssClass(this,'usertagline-hover')" id="center_doing">
            <div class="usertagline">
                <div class="usertagline-inner">
                    <div class="usertagline-text" id="doingcontent" onclick="switchDoingForm(event)">
                        <!--[if $my.doing==null || $my.doing==""]-->
                        点击此处输入您现在的心情
                        <!--[else]-->
                        $my.doing
                        <!--[/if]-->
                    </div>
                    <div class="clearfix usertagline-form" id="mydoingform" onmouseover="doingFormFocus=true;" onmouseout="doingFormFocus=false;" style="display:none;">
                        <form action="$_form.action" method="post" onsubmit=" ajaxPostForm(this.id,this.action, 'adddoing',doingCallback); return false;" id="im_doingform">
                            <div class="tagline-input">
                                <input class="text" type="text" onkeypress="this.changed=true" name="content" maxlength="140" onblur="ajaxPostForm($('im_doingform'),this.action, 'adddoing',doingCallback); " id="CenterDoingContent" value="$_form.text('content',$my.doing)"/>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
        
    </div>
</div>
<!--[ajaxpanel id="toolbar_friend"]-->
<div class="menufriend-body">
    <div class="clearfix menufriend-body-inner">
    <!--[loop $g in $GroupList]-->
        <!--[if $g.IsSelect]-->
        <div class="menufriend-group-expand">
            <h3 class="menufriend-title"><a href="javascript:;" onclick="showfriends('-1',false);"><span>$g.name <em class="count">[$g.onlinecount/{=$g.TotalFriends}]</em></span></a></h3>
            <ul class="menufriend-list" id="friend_list">
                <!--[loop $f in $FriendList]-->
                <li class="clearfix frienditem <!--[if $f.IsOnline]-->online<!--[/if]-->" id="friend_templet">
                    <a class="avatar" href="$root/max-dialogs/chat.aspx?to=$f.userid" onclick="return openDialog(this.href);"><img src="$f.user.smallavatarpath" alt="" width="24" height="24" /></a>
                    <a class="name" href="$root/max-dialogs/chat.aspx?to=$f.userid" onclick="return openDialog(this.href);">$f.user.name</a>
                    <span class="status"><a class="space" target="_top" href="$url(space/$f.userid)">&nbsp;</a>$f.user.doing</span>
                </li>
                <!--[/loop]-->
            </ul>
        </div>
        <!--[else]-->
        <div class="menufriend-group-collapse" onclick="showfriends('$g.groupid',false);">
            <h3 class="menufriend-title"><a href="javascript:;"><span>$g.name <em class="count">[{=$g.onlinecount}/{=$g.TotalFriends}]</em></span></a></h3>
        </div>
        <!--[/if]-->
    <!--[/loop]-->
    </div>
</div>
<div class="clearfix menufriend-foot">
    <a class="friend-manage" target="_top" href="$url(my/friends)"><span>管理好友</span></a>
    <a class="friend-search" target="_top" href="$url(members)?view=search"><span>寻找好友</span></a>
</div>
<script type="text/javascript">
    if (currentPanel) {
        maxDragObject(currentPanel.panel, $('friend_title'));
    }

    function showfriends(id, b) {
        var url = "$url(_part/toolbar_friend)?groupid=" + id;
        ajaxRender(url, "toolbar_friend");
    }

    var sDoingForm = false;
    var doingFormFocus = false;
    function switchDoingForm(e) {
        e = e || window.event;

        setVisible($("doingcontent"), sDoingForm);
        setVisible($('mydoingform'), !sDoingForm);
        if (sDoingForm)
            removeCssClass($('center_doing'), 'usertagline-active');
        else {
            addCssClass($('center_doing'), 'usertagline-active');
            doingFormFocus = true;
        }
        sDoingForm = !sDoingForm;
        ajaxLayer._clickPanel = 1;

        if (e) endEvent(e);
    }

    addHandler(document.documentElement, "click", caonima);

    function caonima(e) {
        var xxx = $("CenterDoingContent");
        if (sDoingForm && !doingFormFocus && xxx && !xxx.changed)
            switchDoingForm(e);
    }

    if (currentPanel) currentPanel.addCloseHandler(function () { removeHandler(document.documentElement, "click", caonima); });

    function doingCallback(a) {
        $("doingcontent").innerHTML = a;
        $("CenterDoingContent").value = a;
        setVisible($("doingcontent"), true);
        setVisible($('mydoingform'), false);
        removeCssClass($('center_doing'), 'usertagline-active');
        sDoingForm = !sDoingForm;
    }
</script>
<!--[/ajaxpanel]-->
