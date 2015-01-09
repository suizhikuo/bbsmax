<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<link rel="Stylesheet" type="text/css" href="$root/max-assets/style/max-admin.css" />
<title>表情上传</title>
</head>
<body style="background:transparent none;">
<form id="form1" action="$_form.action" method="post" enctype="multipart/form-data">
<div id="upload">
    <input type="file" name="file1" />
    <input type="hidden" name="upload" value="上传" />
    <input type="hidden" name="groupmode" value="0" />
    <!--[pre-include src="_error_.ascx" /]-->
</div>

<div id="uploading" style="display:none;">
    <span class="ajaxloading" id="uploading-tip"><span>正在上传，请稍候...</span></span>
</div>
</form>
<script type="text/javascript">
    var onUploading = function () {
        document.getElementById("upload").style.display = "none";
        document.getElementById("uploading").style.display = "";
    }

    function upload() {
        document.forms[0].submit();
        onUploading();
    }
</script>
<!--[if $ClickUpload]-->
<script type="text/javascript">
    if (parent.onFileUpload) {
        parent.onFileUpload($ReturnJson);
    }
</script>
<!--[/if]-->
</body>
</html>
