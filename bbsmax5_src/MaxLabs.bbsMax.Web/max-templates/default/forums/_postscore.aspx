<!--[if $post.PostMarks.Count!=0]-->
<div class="post-score">
    <h3 class="post-score-title">
        <span>帖子评分 <span class="count">共$post.MarkCount条</span></span>
        <span class="post-score-link">
            <a class="post-score-action" href="$Dialog/post-rate.aspx?postid=$post.postid&postAlias=$UrlEncode($post.PostIndexAlias)" onclick="return openDialog(this.href, null)">我要评分</a>
        </span>
    </h3>
    <div class="clearfix post-score-content">
        <ul class="post-score-list">
            <!--[loop $postMark in $post.PostMarks]-->
            <li class="clearfix">
                <a class="fn" href="$url(space/$postMark.UserID)" target="_blank">$postMark.Username</a>:
                <span class="post-score-score">$postMark.GetPostMarkPoints(@"<span class=""label"">{0}:</span> <span class=""value"">+{1}</span> ",@"<span class=""label"">{0}:</span> <span class=""value negative"">{1}</span> ")</span>
                <span class="post-score-reason">$postMark.Reason</span>
                <span class="date">$outputFriendlyDateTime($postMark.CreateDate)</span>
            </li>
            <!--[/loop]-->
        </ul>
        <!--[if $post.MarkCount > $post.PostMarks.Count]-->
        <div class="post-score-count">
            当前显示最近$post.PostMarks.Count条
            <a class="post-score-loglink" href="$Dialog/post-rateusers.aspx?postid=$post.postid" onclick="return openDialog(this.href, null)">查看评分记录</a>
        </div>
        <!--[/if]-->
    </div>
</div>
 <!--[/if]-->
