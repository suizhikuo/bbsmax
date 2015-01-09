<!--[page inherits="MaxLabs.bbsMax.Web.Pager" /]-->
<!--[if $Count > 1 ]-->
<div class="pagination">
<!--[if $showFirst]--><a href="$GetUrl(1)" title="第一页">1 ... </a><!--[/if]-->
<!--[loop $start  to $end with $i]-->
    <!--[if $index != $i]-->
    <a href="$GetUrl($i)">$i</a>
    <!--[else]-->
    <span class="current">$i</span>
    <!--[/if]-->
<!--[/loop]-->
<!--[if $showLast]--><a href="$GetUrl($Count)" title="最后一页"> ...$Count</a><!--[/if]-->
</div>
<!--[/if]-->