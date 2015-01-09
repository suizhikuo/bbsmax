<!--[if $post.PostMarks.Count != 0]-->
<div class="postscore">
    <h3 class="title"><span>帖子评分</span></h3>
    <ul class="list">
        <!--[loop $postMark in $post.PostMarks]-->
        <li>
            <a class="user" href="$url(space/$postMark.UserID)">$postMark.Username</a>
            <span class="score">$postMark.GetPostMarkPoints(@"{0}: +{1} ",@"{0}: {1} ")</span>
            <span class="reason">$postMark.Reason</span>
            <span class="date">$outputFriendlyDateTime($postMark.CreateDate)</span>
        </li>
        <!--[/loop]-->
    </ul>
    <!--[if $post.MarkCount > $post.PostMarks.Count]-->
    <div class="count">
        共 $post.MarkCount 位用户评分 / 当前显示最近 $post.PostMarks.Count 条
    </div>
    <!--[/if]-->
</div>
<!--[/if]-->
