<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入音频</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var root = "$root";
var KE = parent.KE;
var dialog = parent.openDialog.obj;
var mediaType='auto';

function check(url) {
//    if (!url.match(/^\w+:\/\/.{3,}\.(mp3|wav|wma|ra|mid)(\?|$)/i)) {
//        alert(KE.lang['invalidMedia']);
//        window.focus();
//        return false;
//    }
    return true;
}

function preview() {
    var url = KE.$('url', document).value;
    if (!check(url)) return false;
    var embed = KE.$$('embed', document);
    embed.src = url;
    if (url.match(/\.(rm|rmvb)$/i) == null) {
        embed.type = "video/x-ms-asf-plugin";
    } else {
        embed.type = "audio/x-pn-realaudio-plugin";
    }
    embed.loop = "true";
    embed.autostart = "true";
    embed.width = 260;
    embed.height = 190;
    KE.$('previewDiv', document).innerHTML = "";
    KE.$('previewDiv', document).appendChild(embed);
}

function ok() {

    var url = KE.$('url', document).value;
    if (mediaType == 'auto') {
        var profix;
        var reg = /.+\.(\w{2,4})\s*$/ig;
        profix = reg.exec(url);

        if (!profix) {
            alert("无法识别多媒体类型,可能文件后缀不正确，或者请手动选择媒体类型！");
            return;
        }
        mediaType = profix[1].toLowerCase();
        closePanel();
    }
    var isAudio = false, isVedio = true;

    var imgFile = "editor_media.gif";

    if (mediaType == "mp3" || mediaType == "wma" || mediaType == "wma" || mediaType == "wav" || mediaType == "mid") {
        mediaType = "mp3";
        imgFile = "editor_audio.gif";
        isAudio = true;
    }
    else if (mediaType == "ra" || mediaType == "rm" || mediaType == "rmvb") {
        isVedio = true;
        if (mediaType == 'ra') {
            isAudio = true;
        }
        else if (mediaType == 'rm' || mediaType == 'rmvb') {
            isVedio = true;
        }
        mediaType = "rm";
        imgFile = "editor_real.gif";

    }
    else if (mediaType == "wmv" || mediaType == "avi") {
        mediaType = "wmv";
        imgFile = "editor_video.gif";
        isVedio = true;
    }
    else if (mediaType == "flv" || mediaType == "swf") {
        mediaType = "flash";
        imgFile = "editor_flash.gif";

        isVedio = true;
    }


    isVedio = !isAudio;

    var w, h;
    var auto = document.getElementById('autoplay').checked;
    w = isVedio ? 550 : 400;
    h = isAudio ? 64 : 400;

    if (!check(url)) return false;
    var html;
    var ubb;
    ubb = '[' + mediaType + '=' + w + ',' + h + ',' + (auto ? 1 : 0) + ']' + url + '[/' + mediaType + ']'
    html = '<img src="' + parent.root + '/max-assets/images/blank.gif?max_autoplay=' + (auto ? 'true' : 'false') + '" width="' + w + '" height="' + h + '" class="max_editor_' + mediaType + '" title="max_editor_' + mediaType + '"  style="border:solid 1px #aaaaaa; background-position:center center; background-repeat:no-repeat;background-image:url(' + parent.root + '/max-assets/images/' + imgFile + ') " alt="' + url + '"/>';
    KE.util.insertContent("$_get.id", html, ubb);
    KE.$('url', document).value="http://";
    closePanel();

}
</script>
<script type="text/javascript" src="$root/max-assets/javascript/max-lib.js"></script>
</head>
<body class="dialogsection-htmleditor">
    <div class="clearfix editordialoghead">
        <div class="editordialogtab">
            <ul class="clearfix">
                <!--[if $ForumId>0]--><li><a href="audio.aspx?id=$_get.id&forumid=$ForumID" $_if($_get.tab == null, 'class="current"')>本地上传</a></li><!--[/if]-->
                                      <li><a href="audio.aspx?tab=1&id=$_get.id&forumid=$ForumID" $_if($_get.tab == "1"|| $ForumID==0, 'class="current"')>远程文件</a></li>
                <!--[if $EnableNetDiskFunction && $ForumId>0]--><li><a href="audio.aspx?tab=3&id=$_get.id&forumid=$ForumID" $_if($_get.tab == "3", 'class="current"')>网络硬盘</a></li><!--[/if]-->
                <!--[if $ForumId>0]--><li><a href="audio.aspx?tab=2&id=$_get.id&forumid=$ForumID" $_if($_get.tab == "2", 'class="current"')>历史附件</a></li><!--[/if]-->
            </ul>
        </div>
        <div class="editordialogaction">
            <a href="javascript:void(closePanel());">X<span>.</span></a>
        </div>
    </div>
    <div class="editordialogbody">
        <!--#include file="_editorattachtool.aspx"-->
        <!--[if $_get.tab == "1" || $ForumID==0]-->
        <div class="formgroup dialogform remoteinsert dialogform-horizontal">
            <div class="formrow">
                <h3 class="label"><label for="url">音频地址</label></h3>
                <div class="form-enter">
                    <input type="text" class="text url" id="url" name="url" value="http://" maxlength="255" />
                </div>
            </div>
            <div class="formrow">
                <h3 class="label"><label>类型</label></h3>
                <div class="form-enter">
                    <ul class="optionlist">
                        <li>
                            <input type="radio" value="auto" name="media_type" onclick="if(this.checked)mediaType='auto'" checked="checked" id="media_type_auto" />
                            <label for="media_type_auto">自动识别</label>
                        </li>
                        <li>
                            <input type="radio" value="mp3" name="media_type" onclick="if(this.checked)mediaType='mp3'" id="media_type_3" />
                            <label for="media_type_3">MP3</label>
                        </li>
                        <li>
                            <input type="radio" value="wma" name="media_type"  onclick="if(this.checked)mediaType='wma'"id="media_type_2" />
                            <label for="media_type_2">WMA</label>
                        </li>
                        <li>
                            <input type="radio" value="ra" name="media_type"  onclick="if(this.checked)mediaType='ra'"id="media_type_1" />
                            <label for="media_type_1">RA</label>
                        </li>
                        <li>
                            <input type="radio" value="mid" name="media_type"  onclick="if(this.checked)mediaType='mid'"id="media_type_7" />
                            <label for="media_type_7">MID</label>
                        </li>
                        <%--
                        <li>
                            <input type="radio" value="mov" name="media_type" id="media_type_6" />
                            <label for="media_type_5">MOV</label>
                        </li>
                        --%>
                    </ul>
                </div>
            </div>
            <div class="formrow">
                <div class="form-enter">
                    <input type="checkbox" name="auto_remote" id="autoplay" value="1" />
                    <label for="autoplay">自动播放</label>
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