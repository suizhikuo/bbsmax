<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
<!--[if $IsSpace]-->
<link rel="stylesheet" type="text/css" href="$skin/styles/space.css" />
<!--/* 用户空间自定义样式 */-->

<!--[/if]-->
<script type="text/javascript">
function get_selected_photoids(){
  var photoids = document.getElementsByName("photoids");
  var result = [];
  for(var i=0; i<photoids.length; i++) {
    if(photoids[i].checked)
      result.push(photoids[i].value);
  }
  return result.join(',');
}

function goto_edit_mode(){
  var photoids = get_selected_photoids();
  if(photoids == '') {
      showAlert('请至少选择一张相片');
    return;
  }
  location.href = '$url(app/album/list)?id=$album.id&mode=manage&photoids=' + photoids;
}

function move_to_album(albumID) {
  var photoids = get_selected_photoids();
  if(photoids == '') {
      showAlert('请至少选择一张相片');
    return;
  }
    var form = document.forms[0];
    var button;
    button = addElement("input", form);
    button.style.display = 'none';
    button.name = 'Move';
    button.value = "1";

    var album;
    album = addElement("input", form);
    album.style.display = 'none';
    album.name = 'DesAlbumID';
    album.value = albumID;
    
    form.submit();
}

function delete_photo() {
  var photoids = get_selected_photoids();
  if(photoids == '') {
      showAlert('请至少选择一张相片');
    return;
  }
  openDialog('$dialog/album-deletephotos.aspx?photoids=' + photoids, function(){
    location.href = '$url(app/album/list)?id=$albumid&mode=manage';
  });
}

function delete_album() {
  openDialog('$dialog/album-delete.aspx?albumid=$albumid', function(){
    location.href = '$url(app/album/index)';
  });
}

function setAlbumCover(photoID) {
    var form = document.forms[0];
    var button;
    button = addElement("input", form);
    button.style.display = 'none';
    button.name = 'Cover';
    button.value = "1";

    var photo;
    photo = addElement("input", form);
    photo.style.display = 'none';
    photo.name = 'CoverPhotoID';
    photo.value = photoID;
    
    form.submit();
}

function selectAll() {
  var items = document.getElementsByName('photoids');
  for (var i = 0; i < items.length; i++) {
    if (items[i].checked) {
      items[i].checked = false;
      $('photoitem_' + i).className = 'photo-entry';
    } else {
      items[i].checked = true;
      $('photoitem_' + i).className = 'photo-entry selected';
    }
  }
}
</script>
</head>
<body>
<div class="container">
<!--#include file="../../_inc/_head.aspx"-->
    <div id="main" class="main <!--[if $IsSpace]--> section-space<!--[else]--> hasappsidebar section-app<!--[/if]-->">
        <!--[if $IsSpace]--><!--#include file="../../_inc/_round_top.aspx"--><!--[/if]-->
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/album.gif);"><!--[if $IsSpace]-->$AppOwner.Username 的<!--[/if]-->相册</span></h3>
                                <!--[if $IsSpace == false]-->
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a href="$url(app/album/index)?view=everyone"><span>大家的相册</span></a></li>
                                        <li><a href="$url(app/album/index)?view=friend"><span>好友的相册</span></a></li>
                                        <li><a class="current" href="$url(app/album/index)?view=my"><span>我的相册</span></a></li>
                                    </ul>
                                    <ul class="pagebutton">
                                        <li><a class="newalbum" href="$url(app/album/upload)?create=1">新建相册</a></li>
                                        <li><a class="uploadphoto" href="$url(app/album/upload)?id=$album.id">上传照片</a></li>
                                    </ul>
                                </div>
                                <!--[/if]-->
                            </div>
                        
                            <div class="clearfix workspace app-album">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <!--[if $album.UserID == $MyUserID && $TotalPhotoCount > 0]-->
                                        <div class="clearfix workspacehead">
                                           <div class="pagecrumbnav">
                                            </div>
                                           
                                            <div class="pageviewmode">
                                                    <a href="$dialog/album-update.aspx?albumid=$album.id" onclick="return openDialog(this.href, refresh);">编辑相册信息</a>
                                                <!--[if $_get.mode == "manage"]-->
                                                    <a href="$url(app/album/list)?id=$albumid">浏览相册图片</a>
                                                <!--[else]-->
                                                    <a href="$url(app/album/list)?id=$albumid&mode=manage">管理相册图片</a>
                                                <!--[/if]-->
                                            </div> 
                                        </div>
                                        <!--[/if]-->
                                        
                                <!--[if $IsShowPasswordBox]-->
                                        <form id="passwordform" method="post" action="$_form.action">
                                        <div class="formgroup passwordform">
                                            <div class="passwordform-tip">相册主人为该相册设置了密码.</div>
                                            <div class="formrow">
                                                <label class="label" for="password">输入密码</label>
                                                <div class="form-enter">
                                                    <input class="text" type="password" name="password" id="password" />
                                                </div>
                                                <!--[error name="password" form="passwordform"]-->
                                                <div class="form-tip tip-error">密码错误</div>
                                                <!--[/error]-->
                                            </div>
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="submitPassword" value="确认" class="button" /></span></span>
                                            </div>
                                        </div>
                                        </form>
                                <!--[else]-->
                                    <!--[if $TotalPhotoCount > 0]-->
                                        <form id="theform" method="post" id="albumform" action="$_form.action">
                                        <!--[if $_get.mode != "manage"]-->
                                        <div class="photolist photolist-viewmode">
                                            <ul class="clearfix photo-list">
                                                <!--[loop $photo in $PhotoList with $i]-->
                                                <li class="photoitem">
                                                    <div class="photo-entry">
                                                        <div class="photo-thumb">
                                                            <a href="$url(app/album/photo)?id=$photo.id">
                                                                <img src="$Photo.ThumbSrc" alt="" />&nbsp;
                                                            </a>
                                                        </div>
                                                        <div class="photo-title">
                                                            <a href="$url(app/album/photo)?id=$photo.id" title="$Photo.Name">
                                                                $Photo.Name
                                                            </a>
                                                        </div>
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[else if $_get.mode == "manage" && $_get.photoids == null]-->
                                        <div class="apptools">
                                            <div class="clearfix apptools-inner">
                                                <a class="select" href="#" onclick="selectAll(); return false;">全选</a>
                                                <a class="btn" href="#" onclick="goto_edit_mode(); return false;"><span><img src="$Root/max-assets/icon/pensil.gif" alt="" width="16" height="16" />编辑选中照片的信息</span></a>
                                                <!--[if $albumlist.count > 1]-->
                                                <div class="dropdowndock" onclick="return false;" onmouseover="$('menu0').style.display='';" onmouseout="$('menu0').style.display='none';">
                                                    <a class="btn btn-dropdown" href="#"><span><img src="$Root/max-assets/icon/right.gif" alt="" width="16" height="16" />将选中照片移动到</span></a>
                                                    <div class="dropdownmenu-wrap" id="menu0" style="display:none;">
                                                        <div class="dropdownmenu">
                                                            <ul class="dropdownmenu-list">
                                                                <!--[loop $album in $AlbumList]-->
                                                                <!--[if $album.id != AlbumID]-->
                                                                <li><a href="#" onclick="move_to_album($album.id); return false;">$album.Name</a></li>
                                                                <!--[/if]-->
                                                                <!--[/loop]-->
                                                            </ul>
                                                        </div>
                                                    </div>
                                                </div>
                                                <!--[/if]-->
                                                <span class="edge">|</span>
                                                <a class="btn" href="#" onclick="delete_photo(); return false;"><span><img src="$Root/max-assets/icon/del.gif" alt="" width="16" height="16" />删除所选照片</span></a>
                                                <a class="btn" href="#" onclick="delete_album(); return false;"><span><img src="$Root/max-assets/icon/del.gif" alt="" width="16" height="16" />删除该相册</span></a>
                                            </div>
                                        </div>
                                        <div class="photolist photolist-editmode">
                                            <ul class="clearfix photo-list">
                                                <!--[loop $photo in $PhotoList with $i]-->
                                                <li class="photoitem" >
                                                    <div class="photo-entry" id="photoitem_$i" onclick="$('photoid$i').checked=!$('photoid$i').checked; if($('photoid$i').checked ) $('photoitem_$i').className = 'photo-entry selected'; else $('photoitem_$i').className = 'photo-entry'; ">
                                                        <div class="photo-thumb">
                                                            <label for="photoid$i">
                                                                <img src="$Photo.ThumbSrc" alt="" onclick="$('photoid$i').checked=!$('photoid$i').checked; if($('photoid$i').checked ) $('photoitem_$i').className = 'photo-entry selected'; else $('photoitem_$i').className = 'photo-entry'; " />&nbsp;
                                                            </label>
                                                        </div>
                                                        <div class="photo-title">
                                                            <span class="photo-checkbox">
                                                                <input type="checkbox" id="photoid$i" name="photoids" value="$Photo.ID" onclick="if(this.checked ) $('photoitem_$i').className = 'photo-entry selected'; else $('photoitem_$i').className = 'photo-entry'; $('photoid$i').checked=!$('photoid$i').checked;" />
                                                            </span>
                                                            <label for="photoid$i" title="$Photo.Name">
                                                                $Photo.Name
                                                            </label>
                                                        </div>
                                                    </div>
                                                    <div class="entry-action">
                                                        <a class="action-edit" href="$url(app/album/list)?id=$albumid&mode=manage&photoids=$photo.id" title="编辑照片信息">编辑</a>
                                                        <a class="action-delete" href="$dialog/album-deletephotos.aspx?photoids=$photo.id" onclick="return openDialog(this.href, refresh);" title="删除该照片">删除</a>
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[else if $_get.mode == "manage" && $_get.photoids != null]-->
                                        <!--[unnamederror]-->
                                        <div class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <div class="photolisttable">
                                            <table class="photolist-table">
                                                <thead>
                                                    <tr>
                                                        <td class="photo-thumb">照片预览</td>
                                                        <td class="photo-info">照片信息</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <!--[loop $photo in $PhotoList with $i]-->
                                                    <tr>
                                                        <td class="photo-thumb">
                                                            <a href="#">
                                                                <img src="$Photo.ThumbSrc" alt="" />
                                                            </a>
                                                        </td>
                                                        <td class="photo-info">
                                                            <div class="formgroup photoinfo-form">
                                                                <div class="formrow">
                                                                    <label class="label" for="PhotoName_$Photo.ID">名称</label>
                                                                    <div class="form-enter">
                                                                      <input type="hidden" name="PhotoID" value="$Photo.ID" />
                                                                        <input type="text" class="text" name="PhotoName_$Photo.ID" id="PhotoName_$Photo.ID" value="$Photo.Name" />
                                                                    </div>
                                                                </div>
                                                                <div class="formrow">
                                                                    <label class="label" for="PhotoDesc_$Photo.ID">描述</label>
                                                                    <div class="form-enter">
                                                                        <textarea cols="30" rows="2" name="PhotoDesc_$Photo.ID" id="PhotoDesc_$Photo.ID">$Photo.Description</textarea>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <!--[/loop]-->
                                                </tbody>
                                            </table>
                                        </div>
                                        <div class="clearfix photolisttable-action">
                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="保存" name="Save" /></span></span>
                                            <!--[if $_get.upload == "1"]-->
                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="保存，并继续上传" name="SaveAndUpload" /></span></span>
                                            <!--[/if]-->
                                            <!--span class="minbtn-wrap"><span class="btn"><input class="button" type="button" value="取消" onclick="if(confirm('取认要放弃所有修改吗？')) location.href='$url(app/album/list)?id=$album.id'" /></span></span-->
                                        </div>
                                        <!--[/if]-->
                                        </form>
                                        <!--[pager name="pager2" skin="../../_inc/_pager_app.aspx"]-->
                                    <!--[else]-->
                                        <div class="nodata">
                                            该相册暂时没有任何照片.
                                        </div>
                                    <!--[/if]-->
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
            <!--[if $IsSpace]-->
                <!--#include file="../../space/_spacesidebar.aspx"-->
            <!--[else]-->
                <!--#include file="../../_inc/_sidebar_app.aspx"-->
            <!--[/if]-->
        </div>
        <!--[if $IsSpace]--><!--#include file="../../_inc/_round_bottom.aspx"--><!--[/if]-->
    </div>
<!--#include file="../../_inc/_foot.aspx"-->
</div>
</body>
</html>
