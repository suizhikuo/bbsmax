<!--[if $IsShowShareLink]-->
<div class="clearfix post-share">
    <!--[if $CanUseCollection]-->
    <a class="post-fav-button" onclick="return openDialog(this.href,refresh);" href="$dialog/share-create.aspx?type=collection&sharetype=topic&targetID=$thread.ThreadID">
        <span class="post-fav-inner">
            <span class="text">收藏 <span class="stat">$thread.CollectionCount</span></span>
        </span>
    </a>
    <!--[/if]-->
    <!--[if $CanUseShare]-->
    <a class="post-share-button" onclick="return openDialog(this.href,refresh);" href="$dialog/share-create.aspx?type=share&sharetype=topic&targetID=$thread.ThreadID">
        <span class="post-share-inner">
            <span class="text">分享 <span class="stat">$thread.ShareCount</span></span>
        </span>
    </a>
    <!--[/if]-->
</div>
<!--[/if]-->