<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入代码片段</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var KE = parent.KE;
var dialog = parent.openDialog.obj;
function ok() {
    var html = KE.$('textArea', document).value;
    var ubb = html;
    html = html.replace(/</g, '&lt;');
    html = html.replace(/>/g, '&gt;');
    html = html.replace(/\n/g, '<br/>');
    html = html.replace(/\s/g, '&nbsp;');
    html = '[code]<br />' + html + '<br />[/code]';
    var ubb = html;
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
        <h3 class="editordialogtitle"><span>插入代码片段</span></h3>
    </div>
    <div class="editordialogbody">
        <div class="formgroup dialogform editorcodeform">
            <div class="formrow">
                <div class="form-enter">
                    <textarea class="codeipunt" id="textArea" cols="50" rows="8"></textarea>
                </div>
            </div>
        </div>
        <div class="formrow formrow-action">
            <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="确定" onclick="ok();" /></span></span>
            <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="取消" onclick="closePanel();" /></span></span>
        </div>
    </div>
</body>
</html>