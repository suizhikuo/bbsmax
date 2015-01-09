<div class="postgapwrap">
    <table class="postgaptable">
        <tr>
            <td class="col-left">
                &nbsp;
            </td>
            <td class="col-right">
                <div class="clearfix postoperate">
                    <div class="topic-action">
                        <div class="clearfix action-button">
                            <a href="$url($codename/post)?threadid=$threadID&action=reply&page=$PageNumber"><span><img src="$skin/images/theme/button_reply.png" alt="" /></span></a>
                            <a id="action-newtopic2" href="$url($codename/post)?action=thread"><span><img src="$skin/images/theme/button_post.png" alt="" /></span></a>
                            <!--[if $IsShowModeratorManageLink]-->
                            <a class="action-manage" id="action-manage2" href="#"><span><img src="$skin/images/theme/button_manage.png" alt="" /></span></a>
                            <!--[/if]-->
                        </div>
                        
                        <div id="action-newtopic-list" class="dropdownmenu-wrap newtoipc-dropdownmenu" style="display:none;">
                            <div class="dropdownmenu">
                                <ul class="dropdownmenu-list">
                                    <li><a class="icon1" href="$url($codename/post)?action=thread">主题</a></li>
                                    <!--[if $CanCreatePoll]-->
                                    <li><a class="icon2" href="$url($codename/post)?action=poll">投票</a></li>
                                    <!--[/if]-->
                                    <!--[if $CanCreateQuestion]-->
                                    <li><a class="icon3" href="$url($codename/post)?action=question">提问</a></li>
                                    <!--[/if]-->
                                    <!--[if $CanCreatePolemize]-->
                                    <li><a class="icon4" href="$url($codename/post)?action=polemize">辩论</a></li>
                                    <!--[/if]-->
                                </ul>
                            </div>
                        </div>
                    </div>
                    <div class="pagination">
                        <div class="pagination-inner">
                            <!--[ajaxpanel id="ap_pager_down" idonly="true"]-->
                            <a class="back" href="$ForumUrl">&laquo; 返回列表</a>
                            <!--[pager name="PostListPager" skin="../_inc/_pager_bbs.aspx"]-->
                            <!--[/ajaxpanel]-->
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>

<!--[if $isShowReply]-->
<form id="quicklypost" method="post" enctype="multipart/form-data" action="$_form.action">
<div class="panel quickpost quickpost-reply">
    <div class="panel-head">
        <h3 class="panel-title">快速回复</h3>
    </div>
    <div class="panel-body">
        <div class="postwrap">
            <table class="posttable">
                <tr>
                    <td class="postauthor">
                        <div class="authorinfo">
                            <div class="authorinfo-inner">
                                <div class="authorinfo-wrap">
                                    <div class="useridentity">
                                        <!--[if $isLogin==false]-->
                                        <p class="user-name">
                                            <strong class="fn">游客</strong>
                                        </p>
                                        <!--[else]-->
                                        <p class="user-name">
                                            <a class="url" href="$url(space/$my.UserID)">
                                                <strong class="fn">$my.Username</strong>
                                            </a>
                                        </p>
                                        <!--[/if]-->
                                    </div>
                                    <!--[if $DisplayAvatar]-->
                                    <div class="user-maindata">
                                        <div class="user-avatar">
                                            <!--[if $isLogin==false]-->
                                            <img class="photo" src="$root/max-assets/avatar/avatar_120.gif" alt="" width="120" height="120" />
                                            <!--[else]-->
                                            <img class="photo" src="$my.BigAvatarPath" alt="$my.Username" width="120" height="120" />
                                            <!--[/if]-->
                                        </div>
                                    </div>
                                    <!--[/if]-->
                                </div>
                            </div>
                        </div>
                    </td>
                    <td class="postmain">
                        <div class="postmain-inner">
                            <div class="quickpost-enter">
                                <div class="quickpost-enter-inner">
                                    <!--[ajaxpanel id="ap_error" idonly="true"]-->
                                    <!--[unnamederror]-->
                                    <div class="errormsg">$Message</div>
                                    <!--[/unnamederror]-->
                                    <!--[if $IsOverUpdateSortOrderTime]-->
                                    <div class="alertmsg">由于该主题太长时间未被回复，您的回复不会将该主题顶上去</div>
                                    <!--[/if]-->
                                    <!--[/ajaxpanel]-->
                                    <div class="formgroup quickpost-form">
                                        <!--[if $IsLogin==false && $EnableGuestNickName]-->
                                        <div class="formrow">
                                            <h3 class="label"><label for="guestNickName">昵称</label></h3>
                                            <div class="form-enter">
                                                <input name="guestNickName" id="guestNickName" type="text" class="text" value="$_form.text("guestNickName")" />
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                        <!--[if $Thread.ThreadType == ThreadType.Polemize]-->
                                        <div class="formrow">
                                            <div class="form-enter">
                                                <strong>选择您的立场:</strong>
                                                <input type="hidden" name="isReplyPolemize" value="true" />
                                                <input type="radio" name="viewPointType" id="viewPointType_right" value="2" $_form.selected("viewPointType","2") />
                                                <label for="viewPointType_right">正方</label>
                                                <input type="radio" name="viewPointType" id="viewPointType_left" value="3" $_form.selected("viewPointType","3") />
                                                <label for="viewPointType_left">反方</label>
                                                <input type="radio" name="viewPointType" id="viewPointType_neutral" value="4" $_form.selected("viewPointType","4") />
                                                <label for="viewPointType_neutral">中立</label>
                                            </div>
                                        </div>
                                        <!--[/if]-->
                                        <div class="formrow">
                                            <div class="form-enter">
                                            <!--[if $AllowMaxcode]-->
                                            <script src="$root/max-assets/javascript/max-smalleditor.js"></script>
                                            <script type="text/javascript">
                                            createQuicklyEditor("editor");
                                            addPageEndEvent(function () { removeElement($("editor_area_text")); });
                                            </script>
                                            <!--[/if]-->
                                            <textarea name="editor" id="editor_area_text" cols="65" rows="5"></textarea>
                                            </div>
                                        </div>
                                        
                                        <!--[ajaxpanel id="ap_vcode" idonly="true"]-->
                                        <!--[ValidateCode actionType="ReplyTopic"]-->
                                        <div class="formrow">
                                            <div class="form-enter">
                                                <input name="$inputName" id="$inputName" type="text" class="text validcode" value="输入验证码" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                                                <span class="form-note">$tip</span>
                                            </div>
                                        </div>
                                        <!--[/ValidateCode]-->
                                        <!--[/ajaxpanel]-->
                                        <div class="formrow formrow-action">
                                            <div class="quickpost-submit">
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" name="postButton" id="postButton" onclick="return clickPostButton();" type="submit" value="确认发布" /></span></span>
                                                <span class="quickpost-submit-tip">[直接按 Ctrl+Enter 可完成发布]</span>
                                            </div>
                                            <div class="quickpost-extraoption">
                                                <!--[if $IsShowHtmlAndMaxCode]-->
                                                <input type="radio" checked="checked" name="eritoSellect" value="0" id="eritoSellect1" onclick="editorSellect()" /><label for="eritoSellect1">使用UBB</label> 
                                                <input type="radio" name="eritoSellect" value="1" id="eritoSellect2" onclick="editorSellect()" /><label for="eritoSellect2">使用HTML</label>
                                                <script type="text/javascript">
                                                    function editorSellect() {
                                                        if ($('eritoSellect1').checked)
                                                            $('editortool').style.display = '';
                                                        else
                                                            $('editortool').style.display = 'none';
                                                    }
                                                </script>
                                                <!--[else if $AllowHtml]-->
                                                <span>允许使用HTML</span>
                                                <!--[else if $AllowMaxcode]-->
                                                <span>允许使用UBB</span>
                                                <!--[/if]-->
                                                <input type="checkbox" name="tolastpage" id="tolastpage" value="1" $_form.checked('tolastpage','1',$ReplyReturnThreadLastPage) />
                                                <label for="tolastpage">回帖后跳转到最后一页</label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
</form>
<!--[/if]-->

<script type="text/javascript">
    function openPage(action) {
        var u = "$dialog/forum/" + action + ".aspx?codename=$Forum.CodeName";
        return openDialog(u);
    }
    //<!--[if $isShowReply]-->
    var isSending = 0;
function clickPostButton(){
    var form = $('quicklypost');
    if (form.editor.value == '') {
        showAlert('内容不能为空');
        window.setTimeout(" setButtonDisable('postButton', false)", 300);
        return false;
    }
    if (isSending) return;
    isSending = 1;
    var _pb= $('postButton');
    _pb.disabled = true;
    _pb.value = "正在发布";
    if (window.threadImageCollection) threadImageCollection=[];
    //<!--[if $QuicklyPostUseAjax]-->
    ajaxSubmit('quicklypost', 'postButton', 'ap_pager_up,ap_pager_down,ap_threadinfo,ap_postlist,ap_quickreply,ap_quickreplyoption,ap_vcode,ap_error', function (result) {
        if (result != null) {
            document.getElementsByName('editor')[0].value = '';
            if ($('viewPointType_neutral')) $('viewPointType_neutral').checked = false;
            if ($('viewPointType_left')) $('viewPointType_left').checked = false;
            if ($('viewPointType_right')) $('viewPointType_right').checked = false;
            if (result.iswarning)
                showAlert(result.message);
            if (result.issuccess)
                location.href = '#last';
        }
        _pb.disabled = false;
        _pb.value = "确认发布";
        isSending = 0;
    }, null, true);
    return false;
    //<!--[else]-->
    form.submit();
    return true;
    //<!--[/if]-->
}
onCtrlEnter($("quicklypost"), function () { setButtonDisable("postButton", true); clickPostButton(); });
//<!--[/if]-->
</script>