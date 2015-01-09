<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入Flash</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var root = "$root";
var KE = parent.KE;
var dialog = parent.openDialog.obj;

function check(url) {
    if (url.match(/^\w+:\/\/.{3,}(swf)(\?|$)/i) == null) {
        alert(KE.lang['invalidSwf']);
        window.focus();
        return false;
    }
    return true;
};
/*
function preview() {
    var url = KE.$('url', document).value;
    if (!check(url)) return false;
    var embed = KE.$$('embed', document);
    embed.src = url;
    embed.type = "application/x-shockwave-flash";
    embed.quality = "high";
    embed.width = 190;
    embed.height = 190;
    KE.$('previewDiv', document).innerHTML = "";
    KE.$('previewDiv', document).appendChild(embed);
};
*/
function ok(){
    var url = KE.$('url', document).value;
    //if (!check(url)) return false;
    var h = document.getElementById("flashheight").value;
    var w = document.getElementById("flashwidth").value;
    var ubb='[flash';
    var html='';
h=h||400;
var serial=new Date().getMilliseconds();
w=w||550;
html+='<img src="'+parent.root+'/max-assets/images/blank.gif" width="'+ w +'" height="'+h+'" class="max_editor_flash" title="max_editor_flash"  style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url('+parent.root+'/max-assets/images/editor_flash.gif)" alt="'+url+'"/>'

    if(w&&!isNaN(w)&&h&&!isNaN(h))
    {
        ubb+='='+w+','+h+',1';
    }
    ubb+=']'+url+'[/flash]';

    KE.util.insertContent("$_get.id", html, ubb);
    KE.$('url', document).value = "http://";
    closePanel();
}

function onEnter(e){ 
    if( (window.event && window.event.keyCode==13)||(e&&13 == e.which))
        ok();
} 
document.documentElement.onkeyup=onEnter;

</script>
<script type="text/javascript" src="$root/max-assets/javascript/max-lib.js"></script>
</head>
<body class="dialogsection-htmleditor">
    <div class="clearfix editordialoghead">
        <div class="editordialogtab">
            <ul class="clearfix">
                <!--[if $ForumId>0]--><li><a href="flash.aspx?id=$_get.id&forumid=$Forumid" $_if($_get.tab == null, 'class="current"')>本地上传</a></li><!--[/if]-->
                                      <li><a href="flash.aspx?tab=1&id=$_get.id&forumid=$Forumid" $_if($_get.tab == "1"||$ForumID==0, 'class="current"')>远程文件</a></li>
                <!--[if $EnableNetDiskFunction && $ForumId>0]--><li><a href="flash.aspx?tab=3&id=$_get.id&forumid=$Forumid" $_if($_get.tab == "3", 'class="current"')>网络硬盘</a></li><!--[/if]-->
                <!--[if $ForumId>0]--><li><a href="flash.aspx?tab=2&id=$_get.id&forumid=$Forumid" $_if($_get.tab == "2", 'class="current"')>历史附件</a></li><!--[/if]-->
            </ul>
        </div>
        <div class="editordialogaction">
            <a href="javascript:void(closePanel());">X<span>.</span></a>
        </div>
    </div>
    <div class="editordialogbody">
        <!--#include file="_editorattachtool.aspx"-->
        <!--[if $_get.tab == "1" || $ForumID==0]-->
        <div class="formgroup dialogform dialogform-horizontal remoteinsert">
            <div class="formrow">
                <h3 class="label"><label for="url">Flash地址</label></h3>
                <div class="form-enter">
                    <input type="text" class="text url" id="url" name="url" value="http://" maxlength="255" />
                </div>
            </div>
            <div class="formrow">
                <h3 class="label"><label for="flashwidth">宽度</label> </h3>
                <div class="form-enter">
                    <input type="text" class="text number" name="flashwidth" id="flashwidth" value="550" /> px
                    <span class="form-note">(可选填项)</span>
                </div>
             </div>
            <div class="formrow">
                <h3 class="label"><label for="flashheight">高度</label> </h3>
                <div class="form-enter">
                    <input type="text" class="text number" name="flashheight" id="flashheight" value="400" /> px
                    <span class="form-note">(可选填项)</span>
                </div>
            </div>
            <div class="formrow formrow-action">
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="确定" onclick="ok();" /></span></span>
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="取消" onclick="closePanel();" /></span></span>
            </div>
        </div>
        <!--[/if]-->
        <!--#include file="_editorattach.aspx"-->
    </div>
</body>
</html>