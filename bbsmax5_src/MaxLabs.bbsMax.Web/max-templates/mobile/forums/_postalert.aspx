<!--[if $post.PostType == PostType.ThreadContent && $thread.ThreadRecord!=null && $IsShowThreadContent]-->
<div class="poststatus">本主题由 $thread.ThreadRecord[0] 于 $thread.ThreadRecord[2] 执行 $GetActionTypeName($thread.ThreadRecord[1])</div>
<!--[/if]-->
<!--[if $post.PostType == PostType.ThreadContent && $thread.IsValued && $IsUnapprovedTopic==false && $ViewValuedThread == false]-->
<div class="postalert"><!--[if $IsLogin==false]-->(您是游客)<!--[/if]-->您没有查看精华帖子的权限</div>
<!--[else if $post.PostType == PostType.ThreadContent && $IsUnapprovedTopic==false && $ViewThread == false]-->
<div class="postalert"><!--[if $IsLogin == false]-->(您是游客)<!--[/if]-->您没有查看主题内容的权限</div>
<!--[else if $post.PostType != PostType.ThreadContent && $IsUnapprovedTopic==false && $ViewReply == false]-->
<div class="postalert"><!--[if $IsLogin == false]-->(您是游客)<!--[/if]-->您没有查看回复内容的权限</div>
<!--[else if $forum.IsShieldedUser($post.UserID)]-->
<div class="postalert">该用户发言被屏蔽<!--[if $IsShowContent($post)]-->, 但您仍有查看权限<!--[/if]--></div>
<!--[else if $post.IsShielded == true]-->
<div class="postalert">本帖有违规内容被屏蔽<!--[if $IsShowContent($post)]-->, 但您仍有查看权限<!--[/if]--></div>
<!--[else if $post.PostType == PostType.ThreadContent && $IsUnapprovedTopic==false && $Thread.Price > 0]-->
<div class="postalert">本帖需要购买才能查看, 价格: <span class="price">$SellThreadPoint.Name $thread.Price {=$SellThreadPoint.UnitName}</span>.
    <!--[if $IsLogin == false]-->
    您是游客(请先登录)。
    <!--[else if $AlwaysViewContents || $Thread.PostUserID == $MyUserID]-->
    但您拥有不购买即可查看的权限。
    <!--[else if $Thread.IsOverSellDays($forumSetting)]-->
    现已免费。
    <!--[else if $Thread.IsBuyed($My)]-->
    您已经购买,可以查看。
    <!--[else]-->
    <a href="$Dialog/thread-buy.aspx?threadid=$Thread.ThreadID">[立即购买]</a> (<span class="price">您还有 $SellThreadPoint.Name $My.GetPoint($SellThreadPoint.Type) $SellThreadPoint.UnitName</span>)
    <!--[/if]-->
</div>
<!--[/if]-->
