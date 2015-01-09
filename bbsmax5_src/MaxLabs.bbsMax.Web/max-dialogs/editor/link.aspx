<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入链接</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var dialog = parent.openDialog.obj;
var KE = parent.KE;
var dialog = parent.openDialog.obj;
function ok() {
var obj = document.getElementById('hyperLink');
var url = obj.value;
var result ={};
result.html='<a href="'+ url +'" target="_blank">'+url+'</a>';
result.ubb='[url='+url+']'+url+'[/url]';
result.url=url;
result.id="$_get.id";
result.ok=true;
//dialog.result= result;
KE.util.createLink(result.id,result.url);
//dialog.ok(result);
obj.value = "http://";
closePanel();

}

function onEnter(e){ 
    if( (window.event && window.event.keyCode==13)||(e&&13 == e.which))
        ok();
}
document.documentElement.onkeyup = onEnter;

function closePanel() {
    KE.panel.hide("$_get.id");
}
</script>
</head>
<body class="dialogsection-htmleditor">
    <div class="clearfix editordialoghead" id="dialogTitleBar">
        <h3 class="editordialogtitle"><span>插入链接</span></h3>
    </div>
    <div class="editordialogbody">
        <div class="formgroup dialogform editorlinkform">
            <div class="formrow">
                <label class="label">链接地址</label>
                <div class="form-enter">
                    <input type="text" class="text url" id="hyperLink" name="hyperLink" value="http://" />
                </div>
            </div>
            <%--
            <div class="formrow">
                <h3 class="label"><label for="linkType">打开链接方式</label></h3>
                <div class="form-enter">
                    <select id="linkType" name="linkType">
                    <option value="_blank">在新标签页/窗口打开</option>
                    <option value="_self">在当前页面打开</option>
                    </select>
                </div>
            </div>
            --%>
            <div class="formrow formrow-action">
                <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" value="确定" onclick="ok();" /></span></span>
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="取消" onclick="closePanel();" /></span></span>
            </div>
        </div>
    </div>
</body>
</html>
