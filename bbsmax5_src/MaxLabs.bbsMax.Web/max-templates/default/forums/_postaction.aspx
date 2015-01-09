<div class="post-actions">
    <ul class="post-actions-list">
        $GetReplyLink($post,@"<li><a class=""action-reply"" href=""{0}""><span>回复</span></a></li>")
        $GetQuoteLink($post,@"<li><a class=""action-quote"" href=""{0}""><span>引用</span></a></li>")
        $GetMarkLink($post, @"<li><a class=""action-mark"" href=""{0}"" onclick=""return openDialog(this.href, refresh)""><span>评分</span></a></li>")
        $GetCancelMarkLink($post, @"<li><a class=""action-undomark"" href=""{0}"" onclick=""return openDialog(this.href, refresh)""><span>撤销评分</span></a></li>")
        $GetUsePropLink($post,@"<li><a class=""action-magic"" href=""{0}"" onclick=""return openDialog(this.href, refresh)""><span>使用道具</span></a></li>")
    </ul>
</div>

<div class="clearfix post-lovehate">
    <!--[ajaxpanel id="ap_postlovehate_$post.PostID" idonly="true"]-->
    <div class="post-lovehate-count post-lovehate-love">
        <a class="post-lovehate-action" href="javascript:void();" onclick="return ajaxRender('$AttachQueryString("OtherAction=setlovehate&postid=" + $post.PostID+"&islove=true")','ap_postlovehate_$post.PostID',ajaxCallback);">
            <span class="post-lovehate-text">支持</span>
            <span class="post-lovehate-number">$post.LoveCount</span>
        </a>
    </div>
    <div class="post-lovehate-count post-lovehate-hate">
        <a class="post-lovehate-action" href="javascript:void();" onclick="return ajaxRender('$AttachQueryString("OtherAction=setlovehate&postid=" + $post.PostID+"&islove=false")','ap_postlovehate_$post.PostID',ajaxCallback);">
            <span class="post-lovehate-text">反对</span>
            <span class="post-lovehate-number">$post.HateCount</span>
        </a>
    </div>
    $GetReportLink($post,@"<div class=""post-report""><a href=""{0}"" onclick=""return openDialog(this.href);""><span>举报</span></a></div>")
    <a class="post-top" href="#top">TOP</a>
    <!--[/ajaxpanel]-->
</div>
