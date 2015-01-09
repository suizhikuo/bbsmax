<script type="text/javascript">
function textCounter(target, display, maxcount) {
    var a = maxcount - target.value.length;
    if (a < 0) {
        target.value = target.value.substr(0, maxcount);
        a = 0;
    }
    document.getElementById(display).innerHTML = a;
}

function clickProxy(targetid){
    $('proxy_'+targetid).style.display = 'none';
    $('comment_box_'+targetid).style.display = '';
    $('textbox_'+targetid).focus();
}
function CommentBoxLostFocus(targetid) {
        if($('hasvcode_'+targetid))
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
function expandReply(targetid){
    var commentDiv = $('comment_div_'+targetid);
    var commentBox = $('comment_box_'+targetid);
    var button = $('comment_button_'+targetid);
    if(commentDiv.style.display==''){
        commentDiv.style.display = 'none';
        commentBox.style.display = 'none';
        button.innerHTML = '回复';
    }
    else{
        commentDiv.style.display = '';
        commentBox.style.display = '';
        $('proxy_' + targetid).style.display = 'none';
        button.innerHTML = '收起回复';
    }
}
function submitComment(targetid){
    ajaxSubmit('form_'+targetid, 'addfeedcomment', 'sp_comments_' + targetid + ',sp_vcode_' + targetid, function(result){ 
        if(result == null || result.iserror == null || result.iserror == false){
            $('textbox_'+targetid).value = '';
            CommentBoxLostFocus(targetid); 
            if(result != null && result.iswarning)
                showAlert(result.message);
        }
    }, null, true);
}
</script>
<div class="activitieslist">
    <ul class="activities-list">
        <!--[loop $feed in $feedList]-->
        <li class="clearfix activityitem" id="feedid_{=$feed.ID}">
            <div class="activity-type">
                <a href="$url(space/feed-list)?uid=$SpaceOwnerID&appid=$Feed.AppID.ToString()&actiontype=$Feed.ActionType" title="只看此类动态">
                    <img alt="" src="$GetAppActionIconUrl($Feed.AppID,$Feed.ActionType)" />
                </a>
            </div>
            <!--#include file="../_inc/_feedcontent.aspx"-->
            <!--[if $CanDeleteFeed]-->
            <div class="activity-actions">
                <a class="action-delete" href="$dialog/feed-delete.aspx?feedid=$Feed.ID&uid=$SpaceOwnerID" id="a_feed_$feed.ID" onclick="return openDialog(this.href,function(result){ delElement($('feedid_' + result.id)); });" title="删除">删除</a>
            </div>
            <!--[/if]-->
        </li>
        <!--[/loop]-->
    </ul>
</div>