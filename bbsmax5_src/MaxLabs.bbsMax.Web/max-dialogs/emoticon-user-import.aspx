<!--[DialogMaster title="导入表情包文件" width="400"]-->
<!--[place id="body"]-->
<form action="$_form.action" method="post" enctype="multipart/form-data">
<!--[include src="_error_.ascx" /]-->
<div class="clearfix dialogbody">
    <div class="dialogform dialogform-horizontal">
        <div class="formrow">
            <h3 class="label">当表情包内带分组信息时</h3>
            <div class="form-enter">
                <p>
                <input type="radio" name="groupmode" value="0" id="mode1" onclick="setMode(0)" checked="checked" />
                <label for="mode1">始终导入到当前分组</label>
                </p>
                <p>
                <input type="radio" name="groupmode" value="1" id="mode2" onclick="setMode(1)" />
                <label for="mode2">使用表情包内置分组</label>
                </p>
            </div>
        </div>
        <div class="formrow">
            <h3 class="label">文件</h3>
            <div class="form-enter">
                <iframe frameborder="0" id="uploadFrame" scrolling="no" style="width:250px;height:30px" src="$dialog/emoticon-user-import-upload.aspx?groupid=$group.groupid" allowtransparency="true"></iframe>
            </div>
            <div class="form-note">由于服务器限制，单个表情包文件最大不得超过<span class="red">{=$maxrequestlength/1024}MB</span></div>
            <div class="form-note">
                表情包文件支持.EIP和.CFC格式(与腾讯QQ的表情包格式兼容), 您可以导出QQ的表情文件, 上传到这里来用!
            </div>
        </div>
    </div>
</div>

<div class="clearfix dialogfoot">
    <button class="button button-highlight" type="button"  onclick="emotupload()" accesskey="u" title="上传"><span>上传(<u>U</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
<input name="message" id="import_message" type="hidden" value="" />
<input name="state" id="import_state" type="hidden" value="0" />
<script type="text/javascript">
    var importPanel = window.panel;
    var emotupload = function() {
    importPanel = window.panel;
        $("uploadFrame").contentWindow.upload();
    }

    var setMode = function (m) {

        var iframe = $("uploadFrame");
        var doc = null;
        try {
            if (iframe.contentDocument) {
                doc = iframe.contentDocument;
            } else {
                var win = iframe.contentWindow;
                doc = win.document;
            }
            doc.getElementsByName("groupmode")[0].value = m;
        }
        catch (ex) {
            window.setTimeout(setMode(m));
        }
    }

    var onFileUpload = function(r) {
        $("import_state").value = r.state;
        $("import_message").value = r.msg;

        var datas = getFormData(importPanel.panel.getElementsByTagName("form")[0], null);
        importPanel.postToPage(null, datas);
    }

</script>
</form>
<!--[/place]-->
<!--[/dialogmaster]-->
