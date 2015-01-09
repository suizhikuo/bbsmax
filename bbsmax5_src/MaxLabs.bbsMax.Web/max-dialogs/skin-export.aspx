<!--[DialogMaster title="导出皮肤" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post">
<!--[pre-include src="_error_.ascx" /]-->

<!--[if $ExportDone]-->
<div class="clearfix dialogbody">
    <div class="dialogconfirm dialogconfirm-success">
        <h3>皮肤导出完成: <a href="$DownloadPath">点此下载</a></h3>
        <p>你可以点下面的删除按钮，删除服务器上的临时文件。</p>
    </div>
</div>
<div class="clearfix dialogfoot">
     <input type="hidden" name="FileName" value="$_form.htmlencode($FileName)" />
    <button class="button button-highlight" type="submit" name="delete" accesskey="d" title="删除"><span>删除(<u>D</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

<!--[else]-->

<div class="clearfix dialogbody">
    <div class="dialogconfirm">
        <h3>导出皮肤过程需要一些时间，请您耐心等待。</h3>
    </div>
</div>

<div class="clearfix dialogfoot">
    <input type="hidden" value="$_form.htmlencode($UrlReferrer)" name="urlReferrer" />
    <button class="button button-highlight" type="submit" name="sure" accesskey="b" title="开始"><span>开始(<u>B</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>

<!--[/if]-->
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
