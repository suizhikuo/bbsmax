<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>从Word粘贴</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var KE = parent.KE;
function ok(){
    var wordIframe = KE.$('wordIframe', document);
    var wordDoc = KE.util.getIframeDoc(wordIframe);
    var html = KE.util.outputHtml("$_get.id", wordDoc.body);
    KE.util.insertContent("$_get.id", html);
    closePanel();
//    dialog.ok({
//        ok : true,
//        html : html
//    });
}

function setIframe() {
    var iframe = parent.KE.$('wordIframe', document);
    var iframeDoc = parent.KE.util.getIframeDoc(iframe);
    iframeDoc.designMode = "On";
    iframeDoc.open();
    iframeDoc.write('<html><head><title>从Word粘贴</title></head>');
    iframeDoc.write('<body style="margin:5px;padding:0;color:#000;background:#fff;font:12px/1.5 "Lucida Sans Unicode",Helvetica,Arial,sans-serif;">');
    if (parent.KE.browser != 'IE') iframeDoc.write('<br />');
    iframeDoc.write('</body></html>');
    iframeDoc.close();
}

function closePanel() {
    KE.panel.hide("$_get.id");
}
</script>
</head>
<body class="dialogsection-htmleditor" onload="javascript:setIframe();">
    <div class="clearfix editordialoghead" id="dialogTitleBar">
        <h3 class="editordialogtitle"><span>从Word粘贴</span></h3>
    </div>
    <div class="editordialogbody">
        <div class="formgroup dialogform editorpasteform">
            <div class="formrow">
                <div class="form-enter">
                    <iframe class="pasteinput" id="wordIframe" name="wordIframe" frameborder="0" allowtransparency="true"></iframe>
                </div>
                <div class="form-note">请使用快捷键(Ctrl+V)把内容粘贴到下面的方框里.</div>
            </div>
            <div class="formrow formrow-action">
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="确定" onclick="ok();" /></span></span>
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="取消" onclick="closePanel();" /></span></span>
            </div>
        </div>
    </div>
</body>
</html>
