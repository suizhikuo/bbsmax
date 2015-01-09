<!--[if $post.PostType == PostType.ThreadContent && $thread.ThreadRecord!=null && $IsShowThreadContent]-->
<div class="post-status">
    本主题由 $thread.ThreadRecord[0] 于 $thread.ThreadRecord[2] 执行 $GetActionTypeName($thread.ThreadRecord[1]) <a href="$Dialog/threadlogs.aspx?threadid=$threadid" onclick="return openDialog(this.href, null)">[查看记录]</a>
</div>
<!--[/if]-->

<!--[if $post.PostType == PostType.ThreadContent && $thread.IsValued && $IsUnapprovedTopic==false && $ViewValuedThread == false]-->
<div class="post-alert alert-permission"><!--[if $IsLogin==false]-->(您是游客)<!--[/if]-->您没有查看精华帖子的权限</div>
<!--[else if $post.PostType == PostType.ThreadContent && $IsUnapprovedTopic==false && $ViewThread == false]-->
<div class="post-alert alert-permission"><!--[if $IsLogin == false]-->(您是游客)<!--[/if]-->您没有查看主题内容的权限</div>
<!--[else if $post.PostType != PostType.ThreadContent && $IsUnapprovedTopic==false && $ViewReply == false]-->
<div class="post-alert alert-permission"><!--[if $IsLogin == false]-->(您是游客)<!--[/if]-->您没有查看回复内容的权限</div>

<!--[else if $forum.IsShieldedUser($post.UserID)]-->
<div class="post-alert alert-ban">该用户发言被屏蔽<!--[if $IsShowContent($post)]-->,但您仍有查看权限<!--[/if]--></div>
<!--[else if $post.IsShielded == true]-->
<div class="post-alert alert-ban">本帖有违规内容被屏蔽<!--[if $IsShowContent($post)]-->,但您仍有查看权限<!--[/if]--></div>
<!--[else if $post.PostType == PostType.ThreadContent && $IsUnapprovedTopic==false && $Thread.Price > 0]-->
<div class="post-alert alert-buy">本帖需要购买才能查看，价格：<span style="color:Red">$SellThreadPoint.Name<strong>$thread.Price</strong>{=$SellThreadPoint.UnitName}</span>。
    <!--[if $IsLogin == false]-->
        您是游客(请先登录)。
    <!--[else if $AlwaysViewContents || $Thread.PostUserID == $MyUserID]-->
        但您拥有不购买即可查看的权限。<a href="$dialog/threadexchanges.aspx?threadid=$thread.threadID" onclick="return openDialog(this.href, null)">[ 购买记录 ]</a>
    <!--[else if $Thread.IsOverSellDays($forumSetting)]-->
        现已免费。<a href="$dialog/threadexchanges.aspx?threadid=$thread.threadID" onclick="return openDialog(this.href, null)">[ 购买记录 ]</a>
    <!--[else if $Thread.IsBuyed($My)]-->
        您已经购买,可以查看。<a href="$dialog/threadexchanges.aspx?threadid=$thread.threadID" onclick="return openDialog(this.href, null)">[ 购买记录 ]</a>
    <!--[else]-->
        [<a href="$Dialog/thread-buy.aspx?threadid=$Thread.ThreadID" onclick="return openDialog(this.href, refresh)" title="购买帖子"> 立即购买 </a>] （<span style="color:Red">您还有$SellThreadPoint.Name $My.GetPoint($SellThreadPoint.Type) $SellThreadPoint.UnitName</span> <!--[if $IsShowChargePointLink($SellThreadPoint)]-->[<a href="$url(my/pay)" target="_blank"> 充值 </a>]<!--[/if]-->）
    <!--[/if]-->
</div>
<!--[/if]-->