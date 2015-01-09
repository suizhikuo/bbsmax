<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>粘贴为无格式文本</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var KE = parent.KE;
var dialog = parent.openDialog.obj;
function ok(){
    var html = KE.$('textArea', document).value;
    html = KE.util.escape(html);
    var re = new RegExp("\r\n|\n|\r", "g");
    html = html.replace(re, "<br />$&");
    KE.util.insertContent("$_get.id", html);

    KE.$('textArea', document).value = "";
    closePanel();
}

function closePanel() {
    KE.panel.hide("$_get.id");
}
</script>
</head>
<body class="dialogsection-htmleditor">
    <div class="clearfix editordialoghead" id="dialogTitleBar">
        <h3 class="editordialogtitle"><span>粘贴为无格式文本</span></h3>
    </div>
    <div class="editordialogbody">
        <div class="formgroup dialogform editorpasteform">
            <div class="formrow">
                <div class="form-enter">
                    <textarea class="pasteinput" id="textArea" cols="30" rows="8"></textarea>
                </div>
                <div class="form-note">请使用快捷键(Ctrl+V)把内容粘贴到方框内.</div>
            </div>
            <div class="formrow formrow-action">
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="确定" onclick="ok();" /></span></span>
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="取消" onclick="closePanel();" /></span></span>
            </div>
        </div>
    </div>
</body>
</html>
