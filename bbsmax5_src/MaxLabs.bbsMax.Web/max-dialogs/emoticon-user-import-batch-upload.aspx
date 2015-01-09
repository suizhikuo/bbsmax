<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<link rel="Stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="Stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<title>表情上传</title>
<script type="text/javascript">
    var root = "$root";
</script>
<script src="$root/max-assets/javascript/max-lib.js" type="text/javascript"></script>
</head>
<body style="background:transparent none;">
<div class="clearfix dialogbody">
<!--[if !$IsPostBack]-->
<form action='$_form.action' method="post" enctype="multipart/form-data">
<div class="datatablewrap" id="container">
    <table class="datatable" id="table1" style=" border-collapse:collapse;">
        <thead>
            <tr>
                <td>快捷方式</td>
                <td>文件</td>
                <td>&nbsp;</td>
            </tr>
        </thead>
        <tbody>
            <tr id="row-0">
                <td>
                    <input type="hidden" value="1" name="key" />
                    <input type="text" class="text" value="" name="shortcut1" />
                </td>
                <td>
                    <input onchange="createFile(this,0);$('cancelrow-0').style.display='';" type="file" name="emotfile" />
                </td>
                <td>
                    <a href="javascript:void(removeElement($('row-0')))" id="cancelrow-0" style="display:none;">取消</a>
                </td>
            </tr>
            <tr id="newrow">
                <td>
                    <input type="hidden" value="{0}" name="key" />
                    <input type="text" class="text" value="" name="shortcut{0}" />
                </td>
                <td>
                    <input onchange="createFile(this,'{0}');$('deleteRow{0}').style.display='';" type="file" name="emotfile" />
                </td>
                <td>
                    <a href="javascript:void(0)" id="deleteRow{0}" style="display:none;">取消</a>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<input type="hidden" name="upload" value="1" />
</form>
<!--[/if]-->
</div>
<script type="text/javascript">
    var fcount = 0;

function upload()
{
    if (fcount == 0) { showAlert('请选择文件'); return false; }
    document.forms[0].submit();
}

function createFile(o) {
    if (fcount < 20) {
        fcount++;
        table.insertRow();
        var extName = o.value.match(/\.[a-z]{3,4}$/ig);
        extName = (extName + "").toLowerCase();
        if (extName != ".jpg" && extName != ".gif" && extName != ".png" && extName != ".jpge") {
            setTimeout("removeElement($('" + o.parentNode.parentNode.id + "'));", 50);
            alert("请勿上传非 *.jpg; *.gif; *.png 的文件。");
        }
    }
    else {
        showAlert("最多一次20个文件");
    }
}

//<!--[if $IsPostBack]-->
if (parent.onFileUpload) {
    parent.onFileUpload($ResultJson);
}
//<!--[else]-->
var table = new DynamicTable("table1", "key");   
//<!--[/if]-->
</script>
</body>
</html>
