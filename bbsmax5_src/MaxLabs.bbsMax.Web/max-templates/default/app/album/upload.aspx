<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
<!--[if $_get.id != null]-->
<script type="text/javascript" src="$root/max-assets/swfupload/swfupload.js"></script>
<script type="text/javascript">
function getFileSize(size) {
    if (size == 0)
        return '0K';
    if (size < 1024) return '1K';
    if (size < 1024 * 1024) return (size / 1024).toFixed(2) + 'K';
    if (size < 1024 * 1024 * 1024) return (size / 1024 / 1024).toFixed(2) + 'M';
    return (size / 1024 / 1024 / 1024).toFixed(2) + 'G';

}

var swfu;
var totalFileCount = 0;
var totalFileSize = 0;
var uploadcount = 0;
var photoIDs = [];
function initUploader(authCookie, url, fileSizeLimit) {
    var photolist = $('photolist');
    var photolist_item_template = $('photolist_item_template').innerHTML;

    swfu = new SWFUpload({

    // File Upload Settings
    file_size_limit: fileSizeLimit,
    file_types: "*.jpg;*.jpeg;*.png;*.gif",
    file_types_description: '图片文件',
    file_upload_limit: 0,

    post_params: {
      'UserAuthCookie': authCookie
    },

    file_queued_handler: function(file) {
      photoIDs = [];
      $('photolistbox').style.display = '';
      $('uploadbar').style.display = '';
      totalFileCount += 1;
      totalFileSize += file.size;
      $('totalfilecount').innerHTML = totalFileCount;
      $('totalfilesize').innerHTML = getFileSize(totalFileSize);
      photolist.innerHTML += '<ul class="clearfix photouploaditem">' + photolist_item_template.replace(/{id}/g, file.id).replace(/{name}/g, file.name).replace(/{size}/g, getFileSize(file.size)).replace(/{file_size}/g, file.size) + "</ul>";
      uploadcount += 1;
    },
    file_queue_error_handler: function(file) {
        showAlert(file.name + '超过了文件大小限制，您当前的文件大小权限为：每张不大于' + fileSizeLimit);
    },
    file_dialog_complete_handler: function(selected, queued, totalqueued) {

    },
    upload_start_handler: function(file) {
      if ($('uploadbar').style.display != 'none') {
        $('uploadbar').style.display = 'none';
        $('cancelupload').style.display = '';
        swfu.setButtonDisabled(true);
      }
      $('del_btn_' + file.id).style.display = 'none';
      swfu.setUploadURL(url + '&filename=' + encodeURIComponent(file.name));
    },
    upload_progress_handler: function(file, complete, total) {

      $('photo_pgbar_' + file.id).style.width = Math.ceil(complete * 100 / total) + '%';
      //$('photo_stat_' + file.id).innerHTML = '已上传：' + Math.ceil(complete * 100 / total) + '%';
    },
    upload_error_handler: function(file, errorcode, message) {

      //$('photo_stat_' + file.id).innerHTML = message;
    },
    upload_success_handler: function(file, data, response) {

      $('cancelupload').style.display = 'none';

      var datas = data.split('|');

      if (datas[0] == 'error') {
      }
      else {
        photoIDs.push(datas[7]);
      }
    },
    upload_complete_handler: function(file) {
      uploadcount -= 1;
      if (uploadcount == 0) {
        location.href = '$url(app/album/list)?upload=1&mode=manage&id=$album.id&photoids=' + photoIDs.join(',');
      } else {
        swfu.startUpload();
      }
    },

    // Button settings
    button_image_url: '$skin/images/upload_button.png',
    button_placeholder_id: 'UploadFile',
    button_width: 86,
    button_height: 30,
    button_action: SWFUpload.BUTTON_ACTION.SELECT_FILES,
    button_disabled: false,
    button_cursor: SWFUpload.CURSOR.HAND,
    button_window_mode: SWFUpload.WINDOW_MODE.OPAQUE,

    // Flash Settings
    flash_url: root + '/max-assets/swfupload/Flash/swfupload.swf', // Relative to this file

    // Debug Settings
    debug: false
  });
}
</script>
<!--[/if]-->
</head>
<body>
<div class="container">
<!--#include file="../../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar section-app">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/album.gif);">相册</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a href="$url(app/album/index)?view=everyone"><span>大家的相册</span></a></li>
                                        <li><a href="$url(app/album/index)?view=friend"><span>好友的相册</span></a></li>
                                        <li><a class="current" href="$url(app/album/index)"><span>我的相册</span></a></li>
                                    </ul>
                                    <ul class="pagebutton">
                                        <li><a class="newalbum" href="$url(app/album/upload)?create=1">新建相册</a></li>
                                        <li><a class="uploadphoto" href="$url(app/album/upload)">上传照片</a></li>
                                    </ul>
                                </div>
                            </div>
                            
                            <div class="clearfix workspace app-album">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <!--[if $_get.id == null || $_get.id == ""]-->
                                        <form method="post" action="$_form.action">
                                        <div class="clearfix workspacehead">
                                            <div class="pagecrumbnav">
                                                $My.UserName的相册: 新相册 &raquo; 上传照片
                                            </div>
                                        </div>
                                        
                                        
                                        <!--[if $_get.create != "1" && $albumlist.count == 0]-->
                                        <div class="page-message">您现在还没有相册，您需要新建一个相册才能上传照片。</div>
                                        <!--[/if]-->
                                        <div class="formgroup photoupload-form">
                                            <!--[unnamederror]-->
                                            <div class="errormsg">$message</div>
                                            <!--[/unnamederror]-->
                                            <div id="select_form" $_if($_get.create == "1" || $albumlist.count == 0, 'style="display:none"', '')>
                                                <div class="formrow">
                                                    <label class="label" for="albumid">将照片添加到</label>
                                                    <div class="form-enter">
                                                        <select name="id" id="album_id">
                                                            <option value="0">选择相册</option>
                                                            <!--[loop $album in $albumlist]-->
                                                            <option value="$album.id">$album.name</option>
                                                            <!--[/loop]-->
                                                        </select>
                                                    </div>
                                                    <a class="photoupload-newalbum" href="#" onclick="$('select_form').style.display='none';$('create_form').style.display=''; return false;">新相册</a>
                                                </div>
                                                <div class="formrow formaction">
                                                    <span class="minbtn-wrap"><span class="btn"><input class="button" type="button" value="下一步" onclick="location.href='$url(app/album/upload)?id=' + $('album_id').value;" /></span></span>
                                                </div>
                                            </div>
                                            <div id="create_form" $_if($_get.create == "1" || $albumlist.count == 0, '', 'style="display:none"')>
                                                <div class="formrow">
                                                    <label class="label" for="albumname">新相册名</label>
                                                    <div class="form-enter">
                                                        <input type="text" class="text" size="20" id="albumname" name="albumname" value="" />
                                                    </div>
                                                    <!--[if $albumlist.count > 0]-->
                                                    <a class="photoupload-newalbum" href="#" onclick="$('create_form').style.display='none'; $('select_form').style.display='';return false;">选择相册</a>
                                                    <!--[/if]-->
                                                </div>
                                                <div class="formrow">
                                                    <label class="label" for="albumprivacy">隐私设置</label>
                                                    <div class="form-enter">
                                                        <select name="albumprivacy" id="albumprivacy" >
                                                            <option value="AllVisible">全站用户可见</option>
                                                            <option value="FriendVisible">全好友可见</option>
                                                            <option value="SelfVisible">仅自己可见</option>
                                                            <option value="NeedPassword">凭密码查看</option>
                                                        </select>
                                                        <script type="text/javascript">
                                                            initDisplay("albumprivacy", [
                                                            { value: 'AllVisible', display: false, id: 'span_password' },
                                                            { value: 'FriendVisible', display: false, id: 'span_password' },
                                                            { value: 'SelfVisible', display: false, id: 'span_password' },
                                                            { value: 'NeedPassword', display: true, id: 'span_password' }
                                                        ]);
                                                        </script>
                                                    </div>
                                                </div>
                                                <div class="formrow" id="span_password">
                                                    <label class="label" for="albumpassword">密码</label>
                                                    <div class="formr-enter">
                                                        <input type="text" class="text" id="albumpassword" name="albumpassword" value="" size="10" />
                                                    </div>
                                                </div>
                                                <div class="formrow formaction">
                                                    <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" name="create_album" value="下一步" /></span></span>
                                                </div>
                                            </div>
                                        </div>
                                        </form>
                                        <!--[else]-->
                                        <form method="post" id="albumform" action="$_form.action" enctype="multipart/form-data">
                                        <div class="clearfix workspacehead">
                                            <div class="pagecrumbnav">
                                                $My.UserName的相册:
                                                <a href="$url(app/album/list)?id=$Album.ID">$album.name</a>
                                                &raquo; 上传照片
                                            </div>
                                        </div>
                                        <div class="photoupload">
                                            <div class="photoupload-button">
                                                <span id="UploadFile">上传照片</span>
                                            </div>
                                            <div class="photoupload-tip">
                                                提示: 单张照片大小限制在$FileSizeLimit.
                                                一次可以选择多张照片上传! 
                                            </div>
                                            <div class="photouploadlist" style="display:none" id="photolistbox">
                                                <div class="clearfix photoupload-head">
                                                    <span class="title"><span>照片</span></span>
                                                    <span class="filesize"><span>大小</span></span>
                                                    <span class="action"><span>删除</span></span>
                                                </div>
                                                <div class="photouploaditem-wrap" id="photolist">
                                                    <ul class="clearfix photouploaditem" style="display:none;" id="photolist_item_template">
                                                        <li class="status"><span style="width:0%;" id="photo_pgbar_{id}">100%</span></li>
                                                        <li class="title"><span>{name}</span></li>
                                                        <li class="filesize"><span>{size}</span></li>
                                                        <li class="action"><span><a href="#" title="删除" id="del_btn_{id}" onclick="swfu.cancelUpload('{id}'); this.parentNode.parentNode.parentNode.style.display = 'none'; totalFileCount--; totalFileSize -= parseInt('{file_size}'); $('totalfilecount').innerHTML = totalFileCount; $('totalfilesize').innerHTML = getFileSize(totalFileSize); if(totalFileCount == 0) $('uploadbar').style.display = 'none'; return false;">删除</a></li>
                                                    </ul>
                                                </div>
                                                <div class="clearfix photoupload-stat">
                                                    <span class="title"><span>共<span id="totalfilecount">0</span>张照片</span></span>
                                                    <span class="filesize"><span>总计:<span id="totalfilesize">0 K</span></span></span>
                                                    <span class="action"><span><a href="#" id="clarn_all" onclick="$('photolist').innerHTML = ''; $('uploadbar').style.display = 'none'; for(var i=0; i<totalFileCount; i++){ swfu.cancelUpload(swfu.getFile(i).id); } totalFileCount = 0; totalFileSize = 0; $('totalfilecount').innerHTML = totalFileCount; $('totalfilesize').innerHTML = getFileSize(totalFileSize); return false;">清空列表</a></span></span>
                                                </div>
                                            </div>
                                            
                                            <div class="clearfix photoupload-action" id="uploadbar" style="display:none;">
                                                <span class="minbtn-wrap"><span class="btn"><input id="uploadmultiphoto" class="button" type="button" value="开始上传" onclick="swfu.startUpload(); return false;" /></span></span>
                                                <a id="cancelupload" style="display:none" href="#" onclick="swfu.stopUpload(); return false;">取消上传</a>
                                            </div>
                                        </div>
                                        </form>
                                        <script type="text/javascript">
                                            initUploader('$UserAuthCookie', '$root/default.aspx?uploadtempfile.aspx?action=album&UserAuthCookie=$UserAuthCookie&albumid=$album.id', '$FileSizeLimit');
                                        </script>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        &nbsp;
                                    </div>
                                </div>
                            </div>

                        </div>
                        <!--#include file="../../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
            <!--#include file="../../_inc/_sidebar_app.aspx"-->
        </div>
    </div>
<!--#include file="../../_inc/_foot.aspx"-->
</div>
</body>
</html>
