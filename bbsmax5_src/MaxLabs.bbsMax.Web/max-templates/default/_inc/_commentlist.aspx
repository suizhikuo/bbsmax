<h3 class="comment-title"><span>回复</span></h3>
<!--[ajaxpanel id="sp_comments_$targetid" idonly="true"]-->
<ul class="entry-comment-list" id="commentlist_$targetid">
<!--[loop $comment in $commentList with $i]-->
    <!--[if $i==1 && $IsShowGetAll]-->
    <li class="commentitem-expand">
        <a href="javascript:;" onclick="ajaxRender('$AttachQueryString('cdid=$targetid')', 'sp_comments_$targetid,sp_hiddeninput_$targetid'); return false;">显示全部 $TotalComments条评论</a>
    </li>
    <!--[/if]-->
    <li class="commentitem" id="commentid_$targetid">
        <div class="mediumavatar comment-avatar">
            <a href="$url(space/$comment.User.id)"><img src="$comment.User.SmallAvatarpath" alt="" width="24" height="24" /></a>
        </div>
        <div class="hentry comment-entry">
            <div class="entry-meta">
                <span class="vcard author"><a class="url fn" href="$url(space/$comment.User.id)">$comment.User.name</a></span>
                <span class="published" title="$comment.CreateDate">$comment.FriendlyCreateDate</span>
                <!--[if $comment.userid!=$MyUserID]-->
                <a class="" href="javascript:;" onclick="ReplyComment($comment.userid,'$GetSafeJs($comment.User.name)',$targetid,$comment.id)" title="回复">回复</a>
                <!--[/if]-->
            </div>
            <div class="entry-content">
                $comment.Content
            </div>
        </div>
        <!--[if $Comment.CanEdit || $comment.candelete]-->
        <div class="entry-action">
            <!--[if $Comment.CanEdit]-->
                <a class="action-edit" href="$dialog/comment-edit.aspx?type=$commentType&commentid=$comment.ID" onclick="return openDialog(this.href,this, function(r){ajaxRender('$AttachQueryString('cdid=$CommentListTargetID&cp=$CommentListPageNumber')', 'sp_comments_$targetid,sp_hiddeninput_$targetid');if(r){if(r.iswarning){showAlert(r.message);}}})" title="编辑">编辑</a>
            <!--[/if]-->
            <!--[if $comment.candelete]-->
                <a class="action-delete" href="$dialog/comment-delete.aspx?commentid=$comment.ID&type=$commentType" onclick="return openDialog(this.href,this, function(r){ajaxRender('$AttachQueryString('cdid=$CommentListTargetID&cp=$CommentListPageNumber')', 'sp_comments_$targetid,sp_hiddeninput_$targetid');})" title="删除此回复">删除</a>
            <!--[/if]-->
        </div>
        <!--[/if]-->
        <div class="clear">&nbsp;</div>
    </li>
<!--[/loop]-->
</ul>
<!--[if $IsShowCommentPager]-->
<!--[pager name="commentlist" ajaxpanelID="sp_comments_$targetid,sp_hiddeninput_$targetid" skin="_pager_app_ajax.aspx"]-->
<!--[/if]-->
<!--[unnamederror form="form_$targetid"]-->
<div class="errormsg">$message</div>
<!--[/unnamederror]-->
<!--[error form="form_$targetid" name="content"]-->
<div class="errormsg">$message</div>
<!--[/error]-->
<!--[ValidateCode actionType="CreateComment" id="$targetid.ToString()"]-->
<!--[error name="$inputName" form="form_$targetid"]-->
<div class="errormsg">$message</div>
<!--[/error]-->
<!--[/ValidateCode]-->
<!--[/ajaxpanel]-->
<div class="commentformlite" id="proxy_$targetid">
    <p class="commentformlite-text" onclick="clickProxy($targetid)"><span>添加评论...</span></p>
</div>
<form action="$_form.action" method="post" id="form_comment_$targetid">
<div class="clearfix commentform" id="comment_box_$targetid" style="display:none;">
    <div class="comment-avatar mediumavatar">
        <!--[ajaxpanel id="sp_hiddeninput_$targetid" idonly="true"]-->
        <input name="clpn" id="clpn" type="hidden" value="$CommentListPageNumber" />
        <input name="cld" id="cld" type="hidden" value="$CommentListTargetID" />
        <!--[/ajaxpanel]-->
        <a href="$url(space/$my.id)"><img src="$my.SmallAvatarpath" alt="" width="24" height="24" /></a>
    </div>
    <div class="clearfix commentform-enter">
        <input type="hidden" name="targetid" value="$targetid" />
        <input type="hidden" name="replyuserid" id="replyuserid" value="0" />
        <input type="hidden" name="replycommentid" id="replycommentid" value="0" />
        <textarea cols="10" rows="2" name="content" id="textbox_$targetid" onblur="CommentBoxLostFocus($targetid);" onkeyup="textCounter(this, 'maxlimit$targetid', 140)"></textarea>
    </div>
    <div class="clearfix commentform-action">
        <!--[ajaxpanel id="sp_vcode_$targetid" idonly="true"]-->
        <!--[ValidateCode actionType="CreateComment" id="$targetid.ToString()"]-->
        <div class="commentform-captcha" id="vcode_$targetid">
            <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
            <div class="captcha-enter">
                <input type="hidden" id="hasvcode_$targetid" value="1" />
                <input type="text" class="text validcode" name="$inputName" id="$inputName" value="" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
            </div>
        </div>
        <!--[/ValidateCode]-->
        <!--[/ajaxpanel]-->
        <div class="commentform-submit">
            <span class="commentform-textlimit" id="maxlimit$targetid">140</span>
            <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" value="评论" name="addcomment" onclick="submitComment($targetid);return false;" /></span></span>
        </div>
    </div>
</div>
</form>
