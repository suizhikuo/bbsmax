<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<link rel="Stylesheet" type="text/css" href="$root/max-assets/style/max-admin.css" />
</head>
<body style="background:transparent none";>
<form action='$_form.action' method="post" enctype="multipart/form-data">
<div id="upload">
    <input type="file" name="file1" />
    <input type="submit" name="upload" onclick="onUploading()" value="上传" /><!--[pre-include src="_error_.ascx" /]-->
</div>
<div id="uploading" style="display:none;">
    <span class="ajaxloading" id="uploading-tip"><span>正在上传，请稍候...</span></span>
</div>
</form>
<script type="text/javascript">
    var onUploading = function() {
        document.getElementById("upload").style.display = "none";
        document.getElementById("uploading").style.display = "";
    }
</script>
<!--[if $UploadSuccess]-->
<script type="text/javascript">
    if (parent.onFileUpload) {
        parent.onFileUpload($fileJson);
    }
</script>
<!--[/if]-->
</body>
</html>
