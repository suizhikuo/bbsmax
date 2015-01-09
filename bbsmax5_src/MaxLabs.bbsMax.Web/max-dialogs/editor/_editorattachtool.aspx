<form method="post" action="$_form.action">
<!--[if $_get.tab=="3"]-->
<div class="clearfix diskinsert-operate">
    <div class="diskinsert-dir">
        <!--[if $CurrentDirectory.ParentID>0]-->
        <a class="flodericon" style="background-image:url($root/max-assets/icon/folder_up.gif);" href="$GetUrl($CurrentDirectory.ParentID)" title="返回上级">返回上级</a>
        <!--[else]-->
        <a class="flodericon">&nbsp;</a>
        <!--[/if]-->
        <select onchange="changeDirectory(this.value)">
            $GetDiskDirectoryDropDowm("&nbsp;&nbsp;",@"<option value=""{1}"" {3}>{0}{2}</option>", @"selected=""{3}""")
        </select>
    </div>
    <div class="diskinsert-action">
        <a style="padding-left:18px;background:url($root/max-assets/icon/file_up.gif) no-repeat 0 50%;" href="$dialog/disk-upload.aspx?directoryid=$CurrentDirectory.DirectoryID" onclick="return parent.openDialog(this.href,refresh)">上传到网络硬盘</a>
        <a style="padding-left:18px;background:url($root/max-assets/icon/folder_add.gif) no-repeat 0 50%;" href="$dialog/disk-createdirectory.aspx?directoryid=$CurrentDirectory.DirectoryID" onclick="return parent.openDialog(this.href,refresh)">创建目录</a>
    </div>
</div>
<!--[else if  $_get.tab=="2"]-->
<div class="clearfix historyinsert">
    <div class="historyinsert-time">
        时间
        <select id="year" name="year" onchange="submit()">
            <option value="" $_form.selected("year","","")>年</option>
            <!--[loop $y in $Years]-->
            <option value="$y" $_form.selected("year","$y","$year") >$y</option>
            <!--[/loop]-->
        </select>
        <select id="month" name="month" onchange="submit()">
            <option value="" $_form.selected("month","","")>月</option>
            <!--[loop 1 to 12 with $m]-->
            <option value="$m" $_form.selected("month","$m","$month")>$m</option>
            <!--[/loop]-->
        </select>
        <select id="day" name="day" onchange="submit()">
            <option value="" $_form.selected("day","","")>日</option>
            <!--[loop 1 to 31 with $d]-->
            <option value="$d" $_form.selected("day","$d","$day")>$d</option>
            <!--[/loop]-->
        </select>
    </div>
    <div class="historyinsert-search">
        <input type="text" class="text" id="keyword" name="keyword" value="$_form.text('keyword','$keyword')" />
        <input type="button" class="button" name="search" value="查找" onclick="submit()" />
    </div>
</div>

<!--[else if  $_get.tab==null && $ForumId>0]-->
<div class="clearfix fileupload-operate">
    <div class="fileupload-button"><div id="uploadContainet">上传$TypeName附件</div></div>
    <div class="fileupload-tip">
        提示: 单个文件大小限制在 $outputfilesize($MaxFileSize), 一次可以选择多个文件上传.
    </div>
</div>
<!--[/if]-->
<script type="text/javascript">
//function submit() {
//document.forms[0].submit();
//}
function closePanel() {
    KE.panel.hide("$_get.id");
}
</script>
</form>