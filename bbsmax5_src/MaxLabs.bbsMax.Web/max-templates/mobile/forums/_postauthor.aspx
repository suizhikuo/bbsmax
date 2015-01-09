<div class="postinfo">
    <!--[if $post.UserID == 0]-->
    <span class="author">
        <span class="avatar">
            <img class="photo" src="$root/max-assets/avatar/avatar_48.gif" alt="" width="48" height="48">
        </span>
        <span class="fn">游客:<!--[if $post.Username != ""]-->$post.Username<!--[else]-->匿名<!--[/if]--></span>
    </span>
    <!--[else]-->
    <span class="vcard author">
        <a class="url avatar" href="$url(space/$post.UserID)">
            <img class="photo" src="$post.User.AvatarPath" alt="" width="48" height="48">
        </a>
        <a class="fn nickname" href="$url(space/$post.UserID)">$post.Username</a>
    </span>
    <!--[/if]-->
    <!--[if $IsShowPostIndexAlias]-->
    <span class="number">$post.PostIndexAlias</span>
    <!--[/if]-->
    <span class="pulished">$outputFriendlyDateTime($post.CreateDate)</span>
</div>
