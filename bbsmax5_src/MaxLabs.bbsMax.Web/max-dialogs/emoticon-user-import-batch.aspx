<!--[DialogMaster title="上传表情文件到分组 - $Group.groupname" width="500"]-->
<!--[place id="body"]-->
<form action='$_form.action' method="post" enctype="multipart/form-data">
<!--[include src="_error_.ascx" /]-->

<iframe frameborder="0" scrolling="auto" id="frame_emoticon_upload" src="$dialog/emoticon-user-import-batch-upload.aspx?groupid=$group.groupid" style="width:500px;height:300px;" allowtransparency="true"></iframe>

<div class="clearfix dialogfoot">
    <input type="hidden" name="state" id="import_state" />
    <input type="hidden" name="message" id="import_message" />
    <button class="button button-highlight" type="button" accesskey="u" title="上传" onclick="emoticon_batch_upload()" name="upload" ><span>上传(<u>U</u>)</span></button>
    <button class="button" type="reset" accesskey="c" title="取消" onclick="panel.close();"><span>取消(<u>C</u>)</span></button>
</div>
</form>
<script type="text/javascript">
    var importPanel = window.panel;
    var emoticon_batch_upload = function() {
        importPanel = window.panel;
        $("frame_emoticon_upload").contentWindow.upload();
    }
    var onFileUpload = function(r) {
        $("import_state").value = r.state;
        $("import_message").value = r.msg;
        
        
        var datas = getFormData(importPanel.panel.getElementsByTagName("form")[0], null);
        importPanel.postToPage(null, datas);
    }
</script>
<!--[/place]-->
<!--[/dialogmaster]-->