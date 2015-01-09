<!--[DialogMaster title="上传表情文件" width="400"]-->
<!--[place id="body"]-->
<form action='$_form.action' method="post" enctype="multipart/form-data">
<input type="hidden" name="upload" value="upload" />
<div class="dialogmsg dialogmsg-error" id="lblerror" style=" display:none"></div>
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label"><label for="emotfile">文件</label></h3>
            <div class="form-enter">
                <!--input type="file" name="emotfile" id="emotfile" /-->
                 <iframe frameborder="0" id="uploadFrame" scrolling="no" style="width:250px;height:30px" src="$dialog/emoticon-upload-inner.aspx?groupid=$group.groupid" allowtransparency="true"></iframe>
            </div>
            <div class="form-note">支持的表情包格式: EIP和CFC ， 或者您如果有FTP上传的权限， 可以把直接上传GIF文件到表情目录。</div>    
        </div>
    </div>
</div>
<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="button"  onclick="emotupload()" accesskey="u" title="上传"><span>上传(<u>U</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">

    var importPanel = window.panel;
    var emotupload = function () {
        importPanel = window.panel;
        $("uploadFrame").contentWindow.upload();
    }

    var importPanel = window.panel;
    var emotupload = function () {
        importPanel = window.panel;
        $("uploadFrame").contentWindow.upload();
    }

    var onFileUpload = function (r) {
        if (r) {
            if (r.state == 0) {
                var lbl = $("lblerror");
                lbl.innerHTML = r.message;
                setVisible(lbl,1);

                window.setTimeout(function () { setVisible(lbl, 0); }, 3000);
            }
            else if (r.state == 1) {
                var datas = getFormData(importPanel.panel.getElementsByTagName("form")[0], null);
                importPanel.postToPage("$dialog/emoticon-upload.aspx?groupid=$group.groupid&isdialog=1", datas);
            }
        }
    }

</script>
<!--[/place]-->
<!--[/dialogmaster]-->