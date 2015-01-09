function clickProxy(targetid){
    $('proxy_'+targetid).style.display = 'none';
    $('comment_box_'+targetid).style.display = '';
    $('textbox_'+targetid).focus();
}
function CommentBoxLostFocus(targetid) {
        if($('hasVcode_'+targetid))
            return;
        if ($('textbox_' + targetid).value == '') {
            $('proxy_' + targetid).style.display = '';
            $('comment_box_' + targetid).style.display = 'none';
        }
}
function ReplyComment(userid,username,targetid,commentid){
    $('proxy_'+targetid).style.display = 'none';
    $('comment_box_'+targetid).style.display = '';
    var textbox = $('textbox_'+targetid);
    textbox.focus();
    textbox.value='回复'+username+'：';
    $('replyuserid').value = userid;
    $('replycommentid').value = commentid;
}
function expandReply(targetid) {
    var commentDiv = $('comment_div_'+targetid);
    var commentBox = $('comment_box_'+targetid);
    var button = $('comment_button_'+targetid);
    if(commentDiv.style.display==''){
        commentDiv.style.display = 'none';
        commentBox.style.display = 'none';
        button.innerHTML = '评论';
    }
    else{
        commentDiv.style.display = '';
        commentBox.style.display = '';
        $('proxy_'+targetid).style.display = 'none';
        button.innerHTML = '收起评论';
    }
}
function submitComment(targetid){
    ajaxSubmit('form_comment_'+targetid, 'addcomment', 'sp_comments_' + targetid + ',sp_vcode_' + targetid + ',sp_hiddeninput_' + targetid, function(result){ 
        if(result == null || result.iserror == null || result.iserror == false){
            $('textbox_'+targetid).value = '';
            $('maxlimit'+targetid).innerHTML = 140;
            CommentBoxLostFocus(targetid); 
            if(result != null && result.iswarning)
                showAlert(result.message);
        }
    }, null, true);
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