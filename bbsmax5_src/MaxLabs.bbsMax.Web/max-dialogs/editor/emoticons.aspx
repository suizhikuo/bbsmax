<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入表情</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
    //<!--[if !$IsPanel]-->
    window.dialog = parent.openDialog.obj;
//    if (dialog) {
//        dialog.width = 400;
//        dialog.height = 310;
//    }
    //<!--[/if]-->


    var result = null;
    var tabLeft = 0;
    var $ = function(id) { return document.getElementById(id); }
    function addFace() {
        var keid = '$EditorID'; result = { "ubb": arguments[0], "html": '<img src="' + arguments[1] + '" alt="" />' };
        var ke = parent.KE;
        if (ke && keid) {
            ke.layout.hide(keid);
            ke.util.insertContent(keid, result.html, result.ubb);
            parent.KE.panel.hide(keid, "emoticons");
        }
        else {
            if (parent.facePanel) {
                var id = '$EditorID';
                if (id)
                    parent.window["mx_editor2_" + id].insertContent(arguments[0]);
                else if ("$Callback")
                    eval("parent.{=$Callback}(result)");
                parent.facePanel.close();
                parent.facePanel = null;
            }
            else if (window.dialog) {
                dialog.close();
            }
        }
    }
    function switchTo(id) {
        var h = location.href;
        if (h.indexOf("groupid=", 0) > 0) {
            h = h.replace(/groupid=-?\d+/ig, "groupid=" + id);
        }
        else {
            if (h.indexOf("?") > 0)
                h += "&groupid=" + id;
            else
                h += "?groupid=" + id;
        }
        h = h.replace(/&left=[^&$]+/i, '');
        h += "&left=" + tabLeft;
        location.replace(h);
    }

    function we(t, x, s, i) { document.write('<div onclick="addFace(\'' + s + '\',\'$EncodeHttpRoot' + i + '\')" onmouseover="showPreview(this,\'$EncodeHttpRoot' + i + '\')"><p style="background:url(\'$EncodeHttpRoot' + t + '\') no-repeat -' + x + 'px 0;"></p></div>'); }
    var preview = $('face_preview');
    var container = $('preview_container');
    //显示表情预览
    function showPreview(th, src) {
        preview = $('face_preview');
        container = $('preview_container');
        var img;
        img = document.createElement("img");
        container.appendChild(img);
        img.onload = function() { AvatarLoaded(this); preview.style.visibility = 'visible'; };

        img.src = src;
        var l = th.offsetLeft;
        var s = preview.style;
        if (l > 200)
        { s.left = '0'; s.right = ''; }
        else
        { s.left = ''; s.right = '0'; }
        //s.top = "25px";
        s.display = '';
        th.onmouseout || (th.onmouseout = hidePreview);
    }
    
    function refresh() { var href = location.href; if (href.indexOf('#', 0) > -1) { href = href.substring(0, href.indexOf('#', 0)) } location.replace(href); }
    function AvatarLoaded(th, nonMargin) {
        th.onload = null; th.onerror = null; imageScale(th, 90, 90);
        var s = preview.style;
        //var pt=0,pl=0;
        //pt= (90-th.height)/2;
        //s.width = "90px";
        //s.height = th.height;
        //s.paddingTop=pt+"px";
        //s.paddingBottom = pt+"px";
    }
    
    function hidePreview() { $('preview_container').innerHTML = ''; $('face_preview').style.visibility = 'hidden'; }
    function imageScale(e, w, h) { var width = e.width; var height = e.height; var scale = width / height; if (width > w) { e.width = w; e.height = w / scale; } if (height > h) { e.height = h; e.width = h * scale; } }

    var ti;
    var tc;
    function moveTab(lr) {
        if (mflag == false) return;
        if (!ti) ti = document.getElementById("tabitems");
        if (!tc) tc = document.getElementById("face_title_list");
        var s = ti.style;
        var l = parseInt(s.left);
        var wc = tc.offsetWidth;  //parseInt(tc.style.width);
        var w = parseInt(s.width);
        if (lr == "l") {
            if (w + l > wc) {
                s.left = (l - 3) + "px";
            }
            else {
                mflag = false; return;
            }
        }
        else {
            if (l < 0) {
                s.left = (l + 3) + "px";
            }
            else {
                mflag = false; return;
            }
        }
        tabLeft = l;
        window.setTimeout("moveTab('" + lr + "')", 10);
    }
    var timeFlag = false;
    var mflag = false;
</script>
</head>
<body class="dialogsection-htmleditor dialogsection-emoticon">
    <div class="clearfix editordialoghead dialoghead-emoticongroup" id="face_title">
        <div class="editordialogtab-wrap">
            <div id="face_title_list" class="editordialogtab">
                <ul class="clearfix" id="tabitems">
                    <!--[loop $group in $FaceGroupList]-->
                    <li><a $_if($group==$currentgroup,'class="current"') href="javascript:void(switchTo($group.groupid))">$group.groupname</a></li>
                    <!--[/loop]-->
                </ul>
            </div>
        </div>
        <div class="emoticongroup-control" id="control">
            <a href="javascript:;" class="emoticongroup-prev" id="face_prev" onmouseout="mflag=false;" onmouseover="mflag=true;moveTab('r');return false;">&lt;</a>
            <a href="javascript:;" class="emoticongroup-next" id="face_next" onmouseout="mflag=false;" onmouseover="mflag=true;moveTab('l');return false;">&gt;</a>
        </div>
    </div>
    <div class="editordialogbody">
        <div id="face_preview" class="emoticon-preview"><div id="preview_container"></div></div>
        <!--[if $EmoticonList.count != 0]-->
        <div class="clearfix emoticons">
            <script type="text/javascript">
            <!--[loop $emote in $EmoticonList with $i]-->we('{=$GetUrl($emote.ThunmbnailFilePath)}',{=$currentGroup.IsDefault?(($PageNumber-1)*$pagesize+$i)*24:0},'$GetJsString($emote.shortcut)','$GetUrl($emote.imageurl)');<!--[/loop]-->
            </script>
        </div>
        <!--[else]-->
        <div class="emoticons-nodata"><p>该分组没有任何表情.</p></div>
        <!--[/if]-->
        <div class="emoticon-bottom">
            <div class="emoticon-action">
                <!--[if $CanUseUserEmotion && $MyDefaultGroup!=null]-->
                <a style="background: url($root/max-assets/icon/emoticons.gif) no-repeat 0 50%;" href="$url(app/emoticon/index)" target="_blank">管理</a>
                <a style="background:url($root/max-assets/icon/emoticons_import.gif) no-repeat 0 50%;" href="$dialog/emoticon-user-import.aspx?groupid=$MyDefaultGroup.groupid" onclick="return parent.openDialog(this.href, refresh)">导入</a>
                <a style="background:url($root/max-assets/icon/emoticons_upload.gif) no-repeat 0 50%;" href="$dialog/emoticon-user-import-batch.aspx?groupid=$MyDefaultGroup.groupid" onclick="return parent.openDialog(this.href, refresh)">上传</a>
                <!--[/if]-->
                <a style="background:url($root/max-assets/icon/refresh.gif) no-repeat 0 50%;" href="javascript:void(location.reload());">刷新</a>
            </div>
            <!--[pager name="list" skin="../_pager.aspx"]-->
        </div>
        <script type="text/javascript">
            window.setTimeout(function() {
                ti = document.getElementById("tabitems"); var w = 0; var t; for (var i = 0; i < ti.childNodes.length; i++) { if (ti.childNodes[i].offsetWidth) w += ti.childNodes[i].offsetWidth + 2; } ti.style.width = w + 50 + "px";
                var s = ti.style;
                s.left = "$Left" + "px";
            }, 30);
        </script>
    </div>
</body>
</html>