<!--[if $_Pager.Page > 1]-->
<a href="$_Pager.GetUrl($_Pager.Page-1)">&laquo; 上一页</a>
<!--[/if]-->
<!--[if $_Pager.Start > 1]-->
<a href="$_Pager.GetUrl(1)">1...</a>
<!--[/if]-->
<!--[loop $_Pager.Start to $_Pager.End with $i]-->
    <!--[if $i == $_Pager.Page]-->
<span class="current">$i</span>
    <!--[else]-->
<a href="$_Pager.GetUrl($i)">$i</a>
    <!--[/if]-->
<!--[/loop]-->
<!--[if $_Pager.End < $_Pager.PageCount ]-->
<a href="$_Pager.GetUrl($_Pager.PageCount)">...$_Pager.PageCount</a>
<!--[/if]-->
<!--[if $_Pager.Page < $_Pager.PageCount]-->
<a href="$_Pager.GetUrl($_Pager.Page+1)">下一页 &raquo;</a>
<!--[/if]-->
