<!--[if $IsLogin]-->
<div class="postaction">
    $GetReplyLink($post,@"<a href=""{0}"">回复</a>")
    $GetQuoteLink($post,@"<a href=""{0}"">引用</a>")

    $GetEditPostLink($post,@"<a class=""action-edit"" href=""{0}"">编辑</a>")
    $GetDeletePostLinkForMobile($post,@"<a class=""action-delete"" href=""{0}"">删除</a>")

    <a href="$AttachQueryString("OtherAction=setlovehate&postid=" + $post.PostID+"&islove=true")">支持<span class="count">$post.LoveCount</span></a>
    <a href="$AttachQueryString("OtherAction=setlovehate&postid=" + $post.PostID+"&islove=false")">反对<span class="count">$post.HateCount</span></a>
</div>
<!--[/if]-->
