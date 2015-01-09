<!--[if $IsEnableThreadRank]-->
<!--[ajaxpanel id="ap_rank" idonly="true"]-->
<div class="post-rank">
    <div class="post-ranksign post-rank-$thread.Rank" title="等级: $thread.Rank">
        <p class="post-rank-star">$thread.Rank</p>
        <p class="post-rank-toggle">
        <a href="javascript:;" onclick="return ajaxSubmit('threadform','rankButton1','ap_rank', ajaxCallback, null, true);" class="star1" title="1星">1</a>
        <a href="javascript:;" onclick="return ajaxSubmit('threadform','rankButton2','ap_rank', ajaxCallback, null, true);" class="star2" title="2星">2</a>
        <a href="javascript:;" onclick="return ajaxSubmit('threadform','rankButton3','ap_rank', ajaxCallback, null, true);" class="star3" title="3星">3</a>
        <a href="javascript:;" onclick="return ajaxSubmit('threadform','rankButton4','ap_rank', ajaxCallback, null, true);" class="star4" title="4星">4</a>
        <a href="javascript:;" onclick="return ajaxSubmit('threadform','rankButton5','ap_rank', ajaxCallback, null, true);" class="star5" title="5星">5</a>
        </p>
    </div>
    <a class="post-rank-showuser" href="$Dialog/rankusers.aspx?threadid=$threadid" onclick="return openDialog(this.href, null)">[查看评级用户]</a>
</div>
<!--[/ajaxpanel]-->
<!--[/if]-->