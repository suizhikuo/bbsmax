<div class="activity-content">
    <div class="clearfix entry activity-entry">
        $FormatFeedTitle($feed)
        <!--[if $FormatFeedDescription($feed) != ""]-->
        <div class="summary">
             $FormatFeedDescription($feed)
        </div>
        <!--[/if]-->
        <!--[if $feed.DisplayComments]-->
        <!--[if $islogin || $GetComments($feed).count > 0]--> 
        <div class="comment-trigger">
            <a href="#" onclick="expandReply($feed.id);return false;"><span id="comment_button_$feed.id">收起回复</span></a>
        </div>
        <!--[/if]-->
        <!--[/if]-->
    </div>
    
    <!--[if $feed.DisplayComments]-->
    <div class="entry-comment activitycomment" id="comment_div_$feed.id">
        <h3 class="comment-title"><span>回复</span></h3>
        
        <!--[ajaxpanel id="sp_comments_$feed.id" idonly="true"]-->
        <input type="hidden" name="getcommentcount" value="$GetCommentCount" />
        <!--[if $GetComments($feed).count > 0]-->
        <div class="entry-commentlist" id="commentlist_$feed.id">
            <ul class="entry-comment-list">
            <!--[loop $comment in $GetComments($feed) with $i]-->
                <!--[if $i == 1 && $IsShowGetMoreCommentLink($feed)]-->
                <li class="commentitem-expand">
                    <a href="javascript:;" onclick="ajaxRender('$AttachQueryString("commentfeedid=$feed.id&getcommentcount=$GetCommentCount")', 'sp_comments_$feed.id,sp_getcommentcount_$feed.id'); return false;">
                    <!--[if $feed.CommentCount<=$DefaultGetCommentCount]-->
                    显示全部$feed.CommentCount条评论
                    <!--[else]-->
                    显示更早的评论
                    <!--[/if]-->
                    </a>
                </li>
                <!--[/if]-->
                <li class="commentitem" id="commentid_$comment.id">
                    <div class="mediumavatar comment-avatar">
                        <a href="$url(space/$comment.User.id)"><img src="$comment.User.SmallAvatarpath" alt="" width="24" height="24" /></a>
                    </div>
                    <div class="hentry comment-entry">
                        <div class="entry-meta">
                            <span class="vcard author"><a class="url fn" href="$url(space/$comment.User.id)">$comment.User.name</a></span>
                            <span class="published" title="$comment.CreateDate">$comment.FriendlyCreateDate</span>
                            <!--[if $comment.userid != $MyUserID]-->
                            <a href="javascript:;" onclick="ReplyComment($comment.userid,'$GetSafeJs($comment.User.name)',$feed.id,$comment.id)" title="回复此评论">回复</a>
                            <!--[/if]-->
                        </div>
                        <div class="entry-content">
                            $comment.Content
                        </div>
                    </div>
                    <div class="clear">&nbsp;</div>
                </li>
            <!--[/loop]-->
            </ul>
        </div>
        <!--[/if]-->
        <!--[/ajaxpanel]-->
        
        <!--[if $islogin]-->
        <div class="commentformlite" id="proxy_$feed.id">
            <p class="commentformlite-text" onclick="clickProxy($feed.id);"><span>我要评论...</span></p>
        </div>
        
        <div class="clearfix commentform" id="comment_box_$feed.id" style="display:none;">
        <form action="$_form.action" method="post" id="form_$feed.id">
            <div class="comment-avatar mediumavatar">
                <a href="$url(space/$my.id)"><img src="$my.SmallAvatarpath" alt="" width="24" height="24" /></a>
            </div>
            
            <!--[unnamederror]-->
            <div class="errormsg">$message</div>
            <!--[/unnamederror]-->
            <!--[ValidateCode actionType="CreateComment" id="$feed.id.ToString()"]-->
            <!--[error name="$inputName"]-->
            <div class="errormsg">$message</div>
            <!--[/error]-->
            <!--[/ValidateCode]-->
            
            <div class="clearfix commentform-enter">
                <!--[ajaxpanel id="sp_getcommentcount_$feed.id" idonly="true"]-->
                <input type="hidden" name="getcommentcount" value="$GetCommentCount" />
                <!--[/ajaxpanel]-->
                <input type="hidden" name="replyuserid" id="replyuserid" value="0" />
                <input type="hidden" name="replycommentid" id="replycommentid" value="0" />
                <input type="hidden" name="commentfeedid" value="$feed.id" />
                <input type="hidden" name="feedid" value="$feed.id" />
                <input type="hidden" name="targetid" value="$feed.CommentTargetID" />
                <input type="hidden" name="actiontype" value="$feed.ActionType" />
                <input type="hidden" name="appid" value="$feed.AppID" />
                <textarea cols="10" rows="2" name="comment_content" id="textbox_$feed.id" onblur="CommentBoxLostFocus($feed.id);" onkeyup="textCounter(this, 'maxlimit$feed.id', 140)"></textarea>
            </div>
            
            <div class="clearfix commentform-action">
                <!--[ajaxpanel id="sp_vcode_$feed.id" idonly="true"]-->
                <!--[ValidateCode actionType="CreateComment" id="$feed.id.ToString()"]-->
                <input type="hidden" id="hasvcode_$feed.id" value="1" />
                <div class="commentform-captcha" id="vcode_$feed.id">
                    <h3 class="label"><label for="$inputName">验证码</label> <span class="form-note">$tip</span></h3>
                    <div class="captcha-enter">
                        <input type="text" class="text validcode" name="$inputName" id="$inputName" value="" onfocus="showVCode(this,'$imageurl');" autocomplete="off" $_if($disableIme,'style="ime-mode:disabled;"') />
                    </div>
                </div>
                <!--[/ValidateCode]-->
                <!--[/ajaxpanel]-->
                
                <div class="commentform-submit">
                    <span class="commentform-textlimit" id="maxlimit$feed.id">140</span>
                    <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" value="评论" name="addfeedcomment" onclick="submitComment($feed.id);return false;" /></span></span>
                </div>
            </div>
        </form>
        </div>
        <!--[/if]-->
    </div>
    <!--[/if]-->
</div>