<!--[if $CanAddComment]-->
<form id="boardform" action="$_form.action" method="post">
<div class="clearfix spacecommentform">
    <div class="avatar spacecommentform-avatar">
        $My.AvatarLink
    </div>
    <div class="formgroup spacecommentform-field">
        <!--[ajaxpanel id="sp_board_error" idonly="true"]-->
            <!--[unnamederror form="boardform"]-->
        <div class="errormsg">$message</div>
            <!--[/unnamederror]-->
            <!--[error name="content" form="boardform"]-->
        <div class="errormsg">$message</div>
            <!--[/error]-->
        <!--[/ajaxpanel]-->
        <div class="formrow spacecommentform-enter">
            <div class="form-enter">
                <input type="hidden" name="targetid" value="$SpaceOwnerID"/>
                <input type="hidden" name="type" value="Board"/>
                <input type="hidden" name="commentid" id="targetBoardCommentID" value="0"/>
                <textarea name="content" id="boardcontent" rows="4" cols="60"></textarea>
            </div>
        </div>
        <!--[ajaxpanel id="sp_board_vcode" idonly="true"]-->
        <!--[ValidateCode actionType="CreateComment"]-->
        <div class="formrow spacecommentform-captcha">
            <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
            <div class="form-enter">
                <input type="text" class="text validcode" name="$inputName" id="$inputName" value="" onfocus="showVCode(this,'$imageurl');" autocomplete="off" />
            </div>
            <!--[error name="$inputName" form="boardform"]-->
            <div class="form-tip tip-error">$message</div>
            <!--[/error]-->
        </div>
        <!--[/ValidateCode]-->
        <!--[/ajaxpanel]-->
        <div class="formrow formrow-action">
            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" name="addcomment" value="留言" onclick="submitSpaceComment();return false;" /></span></span>
        </div>
    </div>
</div>
<script type="text/javascript">
function submitSpaceComment(){
    ajaxSubmit('boardform', 'addcomment', 'sp_space_comments,sp_board_vcode,sp_board_error', function(result){ 
        if(result != null){
            if(result.iswarning)
                showAlert(result.message);
        }else{
            $('boardcontent').value = '';
        }
    }, null, true);
}
</script>
</form>
<!--[/if]-->
<!--[ajaxpanel id="sp_space_comments" idonly="true"]-->
<!--[if $CommentList.Count > 0]-->
<script type="text/javascript">
    function replyComment(targetid,username){
        var contentBox = $('boardcontent'); 
        contentBox.focus();
        contentBox.value='回复'+username+'：';
        $('targetBoardCommentID').value = targetid;
    }
    function editComment(targetid, url){
        return openDialog(url, function(result){
            if(result){
                if(result.iswarning == true){
                    delElement($('commentid_'+targetid));
                    showAlert(result.message);
                }else{
                    $('commentContent_'+targetid).innerHTML=result.content;
                }
            }
        });
    }
</script>
<div class="spacecommentlist">
    <ul class="spacecomment-list" id="comment_ul">
        <!--[loop $Comment in $CommentList]-->
        <li class="clearfix spacecomment-entry" id="commentid_{=$comment.id}">
            <div class="avatar">
                $comment.User.AvatarLink
            </div>
            <div class="content">
                <div class="title">
                    <a class="fn name" href="$url(space/$comment.User.id)">$comment.User.Name</a>
                    <span class="date">$comment.FriendlyCreateDate</span>
                </div>
                <div class="detail" id="comment_$Comment.ID">
                    <span id="commentContent_$Comment.ID">
                    $comment.Content
                    </span>
                    <!--[if $comment.userid!=$myuserid]-->
                    <a onclick="replyComment($comment.id,'$GetSafeJs($comment.User.Name)');" href="javascript:;">回复</a>
                    <!--[/if]-->
                </div>
            </div>
            <div class="actions">
                <!--[if $comment.userid!=$myuserid]-->
                <a class="action-reply" onclick="replyComment($comment.id,'$GetSafeJs($comment.User.Name)');" href="javascript:;" title="回复">回复</a>
                <!--[/if]-->
                <!--[if $Comment.CanEdit]-->
                <a class="action-edit" href="$dialog/comment-edit.aspx?type=board&commentid=$comment.ID" onclick="return editComment($comment.id,this.href)" title="编辑">编辑</a>
                <!--[/if]-->
                <!--[if $Comment.CanDelete]-->
                <a class="action-delete" href="$dialog/comment-delete.aspx?type=board&commentid=$comment.ID" onclick="return openDialog(this.href,function(r){delElement($('commentid_$comment.id'))})" title="删除">删除</a>
                <!--[/if]-->
            </div>
        </li>
        <!--[/loop]-->
    </ul>
</div>
<!--[if $IsSpacePage == false]-->
<!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
<!--[/if]-->
<!--[else if $BoardCanDisplay == false]-->
<div class="nodata">
    由于空间主人对留言设置了隐私您无法查看他的留言.
</div>
<!--[else]-->
<div class="nodata">
    当前没有任何用户留言.
</div>
<!--[/if]-->
<!--[/ajaxpanel]-->
