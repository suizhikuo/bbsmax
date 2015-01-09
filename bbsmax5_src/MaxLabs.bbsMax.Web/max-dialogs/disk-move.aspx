<!--[DialogMaster title="移动目录或文件" width="400"]-->
<!--[place id="body"]-->
<form method="post" action="$_form.action">
<!--[include src="_error_.ascx" /]-->
<input type="hidden" name="diskDirectoryID" value="$directoryIdString" />
<input type="hidden" name="diskFileID" value="$fileIDString" />
<div class="clearfix dialogbody">
    <div class="dialogform">
        <div class="formrow">
            <h3 class="label">选择目标文件夹</h3>
            <div class="form-enter">
                <div class="scroller" style="height:200px;" id="forumContainer">
                    <ul class="diskfloder-menutree">
                    $GetDiskMenu("<em></em>",@"<li><a class=""{0}"" href=""{1}"">{2}</a></li>","","","current","",@"<span>{4}</span>","<span>根目录</span>")
                    </ul>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="submit" name="move" accesskey="y" title="确定"><span>确定(<u>Y</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->