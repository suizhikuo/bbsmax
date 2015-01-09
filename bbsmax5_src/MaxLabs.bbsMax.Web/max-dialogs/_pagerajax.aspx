<!--[if $_Pager.Start > 1]-->
    <a class="pages-first" href="$_Pager.GetUrl(1)" onclick="ajaxRender(this.href,'$_Pager.AjaxPanelID');return false;" title="第一页">第一页</a>
<!--[/if]-->
<!--[loop $_Pager.Start to $_Pager.End with $i]-->
    <!--[if $i == $_Pager.Page]-->
        <a class="current" href="$_Pager.GetUrl($i)" onclick="ajaxRender(this.href,'$_Pager.AjaxPanelID');return false;" title="当前第 $i 页">$i</a>
    <!--[else]-->
        <a href="$_Pager.GetUrl($i)" onclick="ajaxRender(this.href,'$_Pager.AjaxPanelID');return false;" title="前往第 $i 页">$i</a>
    <!--[/if]-->
<!--[/loop]-->
<!--[if $_Pager.End < $_Pager.PageCount]-->
    <a class="pages-last" href="$_Pager.GetUrl($_Pager.PageCount)" onclick="ajaxRender(this.href,'$_Pager.AjaxPanelID');return false;" title="最后一页">最后页</a>
<!--[/if]-->
