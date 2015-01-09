<div class="clearfix pagination">
    <div class="pagination-inner">
    <!--[if $_Pager.Start>1]-->
        <a href="$_Pager.GetUrl($_Pager.PageCount)" onclick="return ajaxRender(this.href,'$_Pager.AjaxPanelID');">1...<</a>
    <!--[/if]-->
    <!--[loop $_Pager.Start to $_Pager.End with $i]-->
        <!--[if $i==$_Pager.Page]-->
        <a class="current" href="$_Pager.GetUrl({=$_Pager.PageCount+1-$i})" onclick="return ajaxRender(this.href,'$_Pager.AjaxPanelID');">$i</a>
        <!--[else]-->
        <a href="$_Pager.GetUrl({=$_Pager.PageCount+1-$i})" onclick="return ajaxRender(this.href,'$_Pager.AjaxPanelID');">$i</a>
        <!--[/if]-->
    <!--[/loop]-->
    <!--[if $_Pager.End<$_Pager.PageCount]-->
        <a href="$_Pager.GetUrl(1)" onclick="return ajaxRender(this.href,'$_Pager.AjaxPanelID');">...$_Pager.PageCount</a>
    <!--[/if]-->
    </div>
</div>