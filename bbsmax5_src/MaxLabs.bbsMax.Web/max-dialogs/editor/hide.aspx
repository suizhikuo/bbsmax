<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入隐藏内容</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var KE = parent.KE;
var dialog = parent.openDialog.obj;
function ok() {
    var html = document.getElementById('textArea').value;
    var ubb = html;
    html = html.replace(/&/g, '&amp;');
    html = html.replace(/</g, '&lt;');
    html = html.replace(/>/g, '&gt;');
    html = html.replace(/\n/g, '<br/>');
    html = html.replace(/\s/g, '&nbsp;');
    html = '[hide]<br />' + html + '<br />[/hide]';
    KE.util.insertContent("$_get.id", html);
    document.getElementById('textArea').value = "";
    closePanel();
}

function closePanel() {
    KE.panel.hide("$_get.id");
}
</script>
</head>
<body class="dialogsection-htmleditor">
    <div class="clearfix editordialoghead" id="dialogTitleBar">
        <h3 class="editordialogtitle"><span>插入隐藏内容</span></h3>
    </div>
    <div class="editordialogbody">
        <div class="formgroup dialogform editorhideform">
            <div class="formrow">
                <div class="form-enter">
                    <textarea class="hideipunt" id="textArea" cols="30" rows="8"></textarea>
                </div>
                <div class="form-note">隐藏内容在用户回复帖子后, 可以查看隐藏的内容.</div>
            </div>
            <div class="formrow formrow-action">
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="确定" onclick="ok();" /></span></span>
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="取消" onclick="closePanel();" /></span></span>
            </div>
        </div>
    </div>
</body>
</html>