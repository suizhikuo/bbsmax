<script type="text/javascript">
    function rcomment(uid, username, commentid) {
        var content = $('comment_content');
        content.focus();
        content.value = '回复' + username + ':';
        $('replyuserid').value = uid;
        $('replycommentid').value = commentid;
    }
</script>
<h3 class="comment-title"><span>回复</span></h3>
<!--[ajaxpanel id="ap_commentlist" idnoly="true"]-->
<ul class="entry-comment-list">
<!--[loop $comment in $commentlist]-->
    <li class="commentitem" id="commentid_$comment.id">
        <div class="mediumavatar comment-avatar">
            <a href="$url(space/$comment.User.id)"><img src="$comment.User.SmallAvatarpath" alt="" width="24" height="24" /></a>
        </div>
        <div class="hentry comment-entry">
            <div class="entry-content">
                <span class="vcard author"><a class="url fn" href="$url(space/$comment.User.id)">$comment.User.name</a></span>
                <span class="published" title="$comment.CreateDate">$comment.FriendlyCreateDate</span>
                <!--[if $comment.userid!=$MyUserID]-->
                <a class="" href="javascript:;" onclick="rcomment($comment.userid,'$GetSafeJs($comment.User.name)',$comment.id)">回复</a>
                <!--[/if]-->
            </div>
            <div class="entry-meta">
                $comment.Content
            </div>
        </div>
        <!--[if $Comment.CanEdit || $comment.candelete]-->
        <div class="entry-action">
            <!--[if $Comment.CanEdit]-->
            <a class="action-edit" href="$dialog/comment-edit.aspx?type=$commentType&commentid=$comment.ID" onclick="return openDialog(this.href,this, function(r){ajaxRender('$_form.action', 'ap_commentlist,ap_commentCount');if(r){if(r.iswarning){showAlert(r.message);}}})" title="编辑">编辑</a>
            <!--[/if]-->
            <!--[if $comment.candelete]-->
            <a class="action-delete" href="$dialog/comment-delete.aspx?commentid=$comment.ID&type=$commentType" onclick="return openDialog(this.href,this, function(r){ajaxRender('$_form.action', 'ap_commentlist,ap_commentCount');})" title="删除此回复">删除</a>
            <!--[/if]-->
        </div>
        <!--[/if]-->
        <div class="clear">&nbsp;</div>
    </li>
<!--[/loop]-->
</ul>
<!--[if $commentlist.count > 0]-->
<!--[pager name="commentlist" ajaxpanelID="ap_commentlist" skin="_pager_app_ajax.aspx"]-->
<!--[/if]-->
<!--[unnamederror]-->
<div class="errormsg">$message</div>
<!--[/unnamederror]-->
<!--[ValidateCode actionType="CreateComment"]-->
<!--[error name="$inputName"]-->
<div class="errormsg">$message</div>
<!--[/error]-->
<!--[/ValidateCode]-->
<!--[/ajaxpanel]-->
<form action="$_form.action" method="post" id="commentform">
<div class="clearfix commentform">
    <div class="comment-avatar mediumavatar">
        <a href="$url(space/$my.id)"><img src="$my.SmallAvatarpath" alt="" width="24" height="24" /></a>
    </div>
    <div class="commentform-enter">
        <input type="hidden" name="replyuserid" id="replyuserid" value="0" />
        <input type="hidden" name="replycommentid" id="replycommentid" value="0" />
        <textarea cols="10" rows="2" name="content" id="comment_content"></textarea>
    </div>
    <div class="clearfix commentform-action">
        <!--[ajaxpanel id="ap_vcode" idOnly="true"]-->
        <!--[ValidateCode actionType="CreateComment"]-->
        <div class="commentform-captcha">
            <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
            <div class="form-enter">
                <input type="text" class="text" name="$inputName" id="$inputName" value="" onfocus="showVCode(this,'$imageurl');" $_if($disableIme,'style="ime-mode:disabled;"') />
            </div>
        </div>
        <!--[/ValidateCode]-->
        <!--[/ajaxpanel]-->
        <div class="commentform-submit">
            <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="回复" name="addcomment" onclick="ajaxSubmit('commentform','addcomment','ap_commentlist,ap_vcode,ap_commentCount', function(r){ if(r){if(r.iswarning){showAlert(r.message);}}$('comment_content').value = '';}, null, true)" /></span></span>
        </div>
    </div>
</div>
</form>