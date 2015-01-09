<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>插入图片</title>
<link rel="stylesheet" type="text/css" href="$skin/styles/common.css" />
<link rel="stylesheet" type="text/css" href="$skin/styles/dialog.css" />
<script type="text/javascript">
var root = "$root";
function onEnter(e){ 
    if( (window.event && window.event.keyCode==13)||(e&&13 == e.which))
        ok();
} 
document.documentElement.onkeyup=onEnter;
var KE = parent.KE;
var dialog = parent.openDialog.obj;

function check() {
    var url =  KE.$('url', document).value;
    var width = '0';  //KE.$('imgWidth', document).value;
    var height = '0';  //KE.$('imgHeight', document).value;
    //var border = KE.$('imgBorder', document).value;
    if (url.match(/\.(jpg|jpeg|gif|bmp|png)(\?|$)/i) == null) {
        alert(KE.lang['invalidImg']);
        window.focus();
        return false;
    }
    if (width.match(/^\d+$/) == null) {
        alert(KE.lang['invalidWidth']);
        window.focus();
        return false;
    }
    if (height.match(/^\d+$/) == null) {
        alert(KE.lang['invalidHeight']);
        window.focus();
        return false;
    }
//    if (border.match(/^\d+$/) == null) {
//        alert(KE.lang['invalidBorder']);
//        window.focus();
//        return false;
//    }
    return true;

}

function ok(){

    if (!check()) return false;
    var url = KE.$('url', document).value;
    var title = ''; // KE.$('imgTitle', document).value;
    var width = KE.$('imagewidth', document).value;
    var height = KE.$('imageheight', document).value;
    //var border = KE.$('imgBorder', document).value;
    
    var html = '<img src="' + url + '" ';
    if (width) html += 'width="' + width + '" ';
    if (height) html += 'height="' + height + '" ';
    if (title) {
        html += 'title="' + title + '" ';
        html += 'alt="' + title + '" ';
    }
    //html += 'border="' + border + '"';
    html += ' />';
    
    var ubb = '[img';

    if (width > 0 && height > 0) 
        ubb += width + ',' + height;
    else if (width > 0) 
        ubb += width;
    else if (height > 0)
        ubb += ',' + height;

    ubb += ']' + url + '[/img]' + title;

    KE.util.insertContent("$_get.id", html, ubb);
    closePanel();
}

function changeType(obj) {
    if (obj.value == 1) {
        document.getElementById('url').style.display = 'none';
        document.getElementById('imgFile').style.display = 'block';
    } else {
        document.getElementById('url').style.display = 'block';
        document.getElementById('imgFile').style.display = 'none';
    }
}
function swithDisplay(a,b){
    if(a == 1) {
        document.getElementById('albumlist').style.display = 'none'; 
        document.getElementById('imageform').style.display = 'block';
        document.getElementById('albumlist_a').className = 'current';
        document.getElementById('imageform_a').className = '';
        
        dialog.resize(dialog.width, 320);
    } else {
        document.getElementById('albumlist').style.display = 'block'; 
        document.getElementById('imageform').style.display = 'none';
        document.getElementById('albumlist_a').className = '';
        document.getElementById('imageform_a').className = 'current';
        
        dialog.resize(dialog.width, 450);
    }
    return false;
}
</script>
<script type="text/javascript" src="$root/max-assets/javascript/max-lib.js"></script>
</head>
<body class="dialogsection-htmleditor">
    <div class="clearfix editordialoghead">
        <div class="editordialogtab">
            <ul class="clearfix">
                <!--[if $ForumId>0]--><li><a href="photo.aspx?id=$_get.id&forumid=$Forumid" $_if($TabState == 0, 'class="current"')>本地上传</a></li><!--[/if]-->
                <li><a href="photo.aspx?tab=1&id=$_get.id&forumid=$Forumid" $_if($TabState == 1|| ($ForumID==0 && $TabState==0), 'class="current"')>远程文件</a></li>
                <!--[if $EnableAlbumFunction]-->
                <!--[if $ForumId>0]--><li><a href="photo.aspx?tab=4&id=$_get.id&forumid=$Forumid" $_if($TabState == 4, 'class="current"')>相册</a></li><!--[/if]-->
                <!--[/if]-->
                <!--[if $EnableNetDiskFunction && $ForumId>0]-->
                <li><a href="photo.aspx?tab=3&id=$_get.id&forumid=$Forumid" $_if($TabState == 3, 'class="current"')>网络硬盘</a> </li>
                <!--[/if]-->
                <!--[if $ForumId>0]--><li><a href="photo.aspx?tab=2&id=$_get.id&forumid=$Forumid" $_if($TabState == 2, 'class="current"')>历史附件</a></li><!--[/if]-->
            </ul>
        </div>
        <div class="editordialogaction">
            <a href="javascript:void(closePanel());">X<span>.</span></a>
        </div>
    </div>
    <div class="editordialogbody">
        <!--#include file="_editorattachtool.aspx"-->
        <!--[if $TabState == 1 || $ForumID==0]-->
        <form name="uploadForm" method="post" enctype="multipart/form-data" action="">
        <div class="formgroup dialogform remoteinsert  dialogform-horizontal" id="imageform" $_if($_get.album != null, 'style="display:none"')>
            <div class="formrow">
                <h3 class="label"><label for="url">图片地址</label></h3>
                <div class="form-enter">
                    <input type="hidden" id="editorId" name="id" value="" />
                    <input type="text" class="text url" id="url" name="url" value="http://" maxlength="255" />
                </div>
            </div>
            <div class="formrow">
                <h3 class="label"><label for="imagewidth">宽度</label> </h3>
                <div class="form-enter">
                    <input type="text" class="text number" name="imagewidth" id="imagewidth" value="" /> px
                    <span class="form-note">(可选填项)</span>
                </div>
            </div>
            <div class="formrow">
                <h3 class="label"><label for="imageheight">高度</label> </h3>
                <div class="form-enter">
                    <input type="text" class="text number" name="imageheight" id="imageheight" value="" /> px
                    <span class="form-note">(可选填项)</span>
                </div>
            </div>
            <div class="formrow formrow-action">
                <span class="minbtn-wrap"><span class="btn"><input type="button" class="button" value="确定" onclick="ok();" /></span></span>
            </div>
        </div>
        </form>
        <!--[else if $TabState == 4]-->
        <form action="$_form.action" method="post">
        <div class="clearfix albuminsert">
            <ul class="albuminsert-mode">
                <li>
                    <input type="radio" id="UseOriginalPhoto" checked="checked" name="UseOriginalPhoto" />
                    <label for="UseOriginalPhoto">插入原图</label>
                </li>
                <li>
                    <input name="UseOriginalPhoto" type="radio" id="UseThumbPhoto"/>
                    <label for="UseThumbPhoto">插入缩略图</label>
                </li>
            </ul>
        </div>
        <div id="albumlist" class="clearfix albuminsert-layout">
            <!--[if $AlbumList.Count == 0]-->
            <div class="filelist-nodata">
                <p>当前没有任何相册.</p>
            </div>
            <!--[else]-->
            <table class="filelist-table">
                <tr>
                    <td class="album">
                        <div class="albuminsert-dir">
                            <div class="albuminsert-dir-inner">
                                <ul>
                                    <!--[loop $Album in $AlbumList]-->
                                    <li><a $_if($AlbumID == $Album.ID, 'class="current"') href="?album=$Album.ID&isdialog=$_get.isdialog&tab=4&id=$_get.id&forumid=$Forumid">$Album.Name</a></li>
                                    <!--[/loop]-->
                                </ul>
                            </div>
                        </div>
                    </td>
                    <td class="photo">
                        <!--[if $Album.DisplayForOwnerOnly]-->
                        <div class="filelist-nodata">
                            <p>此相册只有您自己可见, 所以相片不能插入到内容中.</p>
                        </div>
                        <!--[else if $PhotoList.Count == 0]-->
                        <div class="filelist-nodata">
                            <p>此相册没有图片.</p>
                        </div>
                        <!--[else]-->
                        <script type="text/javascript">
                            function insertImage(src, thumbStr, url) {
                                var html, ubb;
                                var ke = parent.KE;
                                if (document.getElementById('UseOriginalPhoto').checked) {
                                    html = '<img src="' + src + '" />',
                                    ubb = '[img]' + src + '[/img]'
                                } else {
                                    html = '<a href="' + url + '"><img src="' + thumbStr + '" /></a> 点击图片查看原图',
                                    ubb = '[url="' + url + '"][img]' + thumbStr + '[/img][/url] 点击图片查看原图'
                                }
                                if (ke) {
                                    ke.util.insertContent("$_get.id", html, ubb);   
                                }
                            }
                        </script>
                        <div class="filethumbview">
                            <ul class="clearfix filethumb-list">
                                <!--[loop $Photo in $PhotoList]-->
                                <li class="thumbitem">
                                    <a class="thumb-entry" onclick="insertImage('$Photo.Src','$Photo.ThumbSrc','$url(app/album/photo)?id=$Photo.ID');return false;">
                                        <span class="thumb">
                                            <img src="$Photo.ThumbSrc" alt="" onclick="" />&nbsp;
                                        </span>
                                        <span class="title">
                                            $Photo.Name
                                        </span>
                                    </a>
                                </li>
                                <!--[/loop]-->
                            </ul>
                        </div>
                        <!--[/if]-->
                    </td>
                </tr>
            </table>
            <!--[/if]-->
        </div>
        <!--[pager name="list" skin="../_pager.aspx"]-->
        </form>
        <!--[/if]-->
        <!--#include file="_editorattach.aspx"-->
    </div>
</body>
</html>