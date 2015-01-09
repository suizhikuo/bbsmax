<article class="hentry">
    <!-- #include file="_postauthor.aspx" -->
    <!--[if $IsShowContent($post) && $CanSeeContent($post) && $post.SubjectText != ""]-->
    <h1 class="entry-title">$post.SubjectText</h1>
    <!--[/if]-->
    <!--[if $Thread.ThreadType == ThreadType.Polemize]-->
    <div class="dabatesign <!--[if $post.PostType == PostType.Polemize_Agree]--> dabatesign-right<!--[else if $post.PostType == PostType.Polemize_Against]--> dabatesign-left<!--[else if $post.PostType == PostType.Polemize_Neutral]--> dabatesign-neutral<!--[/if]-->">
        <span><!--[if $post.PostType == PostType.Polemize_Agree]-->正方<!--[else if $post.PostType == PostType.Polemize_Against]-->反方<!--[else if $post.PostType == PostType.Polemize_Neutral]-->中立<!--[/if]--></span>
    </div>
    <!--[/if]-->
    <!-- #include file="_postalert.aspx" -->
<!--[if $IsShowContent($post) && $CanSeeContent($post)]-->
    <div class="entry-content">
        $Highlight($post.ContentText)
    </div>
    <!-- #include file="_postlastmodify.aspx" -->
    <!--[if $IsShowPostGetReward($post.PostID)]-->
    <div class="postextra">
        <p>该帖得分: $GetReward($post.PostID) $QuestionPoint.UnitName</p>
    </div>
    <!--[/if]-->
    <!-- #include file="_postattachment.aspx" -->
    <!-- #include file="_postscore.aspx" -->
<!--[/if]-->
    <!-- #include file="_postsignature.aspx" -->
    <!-- #include file="_postaction.aspx" -->
</article>
