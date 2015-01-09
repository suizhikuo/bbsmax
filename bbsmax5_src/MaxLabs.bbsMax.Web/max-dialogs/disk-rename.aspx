<!--[DialogMaster title="重命名" width="400"]-->
<!--[place id="body"]-->
<form action='$_form.action' method="post">
<!--[include src="_error_.ascx" /]-->
<input type="hidden" name="diskDirectoryID" value="$_form.text('diskDirectoryID')" />
<input type="hidden" name="diskFileID" value="$_form.text('diskFileID')" />
<div class="clearfix dialogbody">
    <div class="datatablewrap" style="height:200px;" id="forumContainer">
        <table class="datatable">
        <thead>
            <tr>
                <td>文件类型</td>
                <td>文件名</td>
            </tr>
        </thead>
        <tbody>
        <!--[loop $file in $Filelist]-->
            <tr>
                <td><!--[if $file.isFile]--><img alt="" src="$file.smallicon" /><!--[else]--><img alt="" src="$Root/max-assets/images/folder_close.gif" /><!--[/if]--></td>
                <td><input type="text" class="text" name="$_if($file.isfile,'newFileName','newDirectoryName')" value="$_form.text('',$file.name)" /></td>
            </tr>
        <!--[/loop]-->
        </tbody>
        </table>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="rename" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->