<div class="clearfix pagination">
    <div class="pagination-inner">
        <!--[if $_Pager.Start > 1]-->
        <a href="$_Pager.GetUrl(1)">1 ... </a>
        <!--[/if]-->
        <!--[loop $_Pager.Start to $_Pager.End with $i]-->
            <!--[if $i == $_Pager.Page]-->
        <a class="current" href="$_Pager.GetUrl($i)">$i</a>
            <!--[else]-->
        <a href="$_Pager.GetUrl($i)">$i</a>
            <!--[/if]-->
        <!--[/loop]-->
        <!--[if $_Pager.End < $_Pager.PageCount ]-->
        <a href="$_Pager.GetUrl($_Pager.PageCount)"> ... $_Pager.PageCount</a>
        <!--[/if]-->
    </div>
</div>
