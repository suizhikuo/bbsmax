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
    function submitPhoto(id) {
        return ajaxRender('$url(app/album/photo)?id=' + id, 'ap_photoname,ap_photoimg,ap_photolist', function(result) {
        if (result != null) {
            if (result.message != null)
                showAlert(result.message);
        }
    });
}

var imgwidth = 0;
var imgheight = 0;
var imgRect = null;
function imageScale2(e) {
    var w = containerWidth;
    imageScale(e, w, w);
    imgRect = getRect(e);

    var setCursor = function (ev) {
        ev = ev || window.event;
        var x = ev.clientX - imgRect.left;

        if (x > e.width / 2) {
            // if (e.style.cursor != 'url($skin/images/cursor_right.cur), auto')
            e.style.cursor = 'url($skin/images/cursor_right.cur), auto';
        } else {
            //  if (e.style.cursor != 'url($skin/images/cursor_left.cur), auto')
            e.style.cursor = 'url($skin/images/cursor_left.cur), auto';
        }
    };
    e.onmousemove = setCursor;
    e.onmouseover = setCursor;

    e.onclick = function (ev) {
        ev = ev || window.event;

        var x = ev.clientX - imgRect.left; // -e.offsetLeft;

        if (x > e.width / 2)
            submitPhoto($('NextPhotoid').value);
        else
            submitPhoto($('PreviousPhotoid').value);
    };
}

//function move_left(){
//  var marginLeft = parseInt($('photolist').style.marginLeft);
//  if(!marginLeft){
//    marginLeft = 0;
//    if(parseInt($('photolist').style.width) < 630)
//      return;
//  }
//  
//  if(marginLeft + parseInt($('photolist').style.width) < 630)
//    return;
//  var a = 0;
//  var fun = function(){
//    a += 10;
//    $('photolist').style.marginLeft = (marginLeft - Math.sin(a * Math.PI / 180) * 630).toString() + 'px';
//    if(a < 90)
//      setTimeout(fun, 13);
//  }
//  
//  fun();
//}
//function move_right(){
//  var marginLeft = parseInt($('photolist').style.marginLeft);
//  if(!marginLeft) {
//    marginLeft = 0;
//    if(parseInt($('photolist').style.width) < 630)
//      return;
//  }
//  
//  if(marginLeft > -630)
//    return;
//  var a = 0;
//  var fun = function(){
//    a += 10;
//    $('photolist').style.marginLeft = (marginLeft + Math.sin(a * Math.PI / 180) * 630).toString() + 'px';
//    if(a < 90)
//      setTimeout(fun, 13);
//  }
//  fun();
//}

var slide_on = false;
var slide_count = 0;
var slide_timer = 5;
var containerWidth = 0;
var slide_func = function() {
    if (slide_on) {
        slide_count++;

        if (slide_count >= slide_timer) {
            submitPhoto($('NextPhotoid').value);
            slide_count = 0;
        }
        else {
            $('slide_count').innerHTML = (slide_timer - slide_count) + 'S';
        }
        setTimeout(slide_func, 1000);
    }
}

function slideShow() {
    if (slide_on) {
        submitPhoto($('NextPhotoid').value);
    }
    else {
        $('slide_count').innerHTML = '';
    }
    slide_count = 0;
    setTimeout(slide_func, 0);
}

function updatePhotoSize(img) {
    if ($('photo_size'))
        $('photo_size').innerHTML =img.width + ' x ' + img.height + ' px';
}

var listHeight = 0;
function vCenter(img) {
    if (!listHeight) listHeight = img.parentNode.offsetHeight;
    imageScale(img, listHeight, listHeight);
    var t = (listHeight - img.height) / 2 - 2;
    if(t>0) img.style.paddingTop = t + 'px';
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
                                        <li><a class="current" href="$url(app/album/index)"><span>我的相册</span></a></li>
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
                                        
                                        <!--[if $IsShowPasswordBox]-->
                                        <div class="clearfix workspacehead">
                                            <div class="pagecrumbnav">
                                                <a href="$url(app/album/index)?id=$album.id&uid=$album.userid">$appowner.name的相册</a>&raquo;
                                                <a href="$url(app/album/list)?id=$album.id&uid=$album.userid">$album.name</a>&raquo;浏览相片
                                            </div>
                                        </div>
                                        <form id="passwordform" method="post" action="$_form.action">
                                        <div class="formgroup passwordform">
                                            <div class="passwordform-tip">相册主人为该相册设置了密码.</div>
                                            <div class="formrow">
                                                <label class="label" for="password">输入密码</label>
                                                <div class="form-enter">
                                                    <input class="text" type="password" name="password" id="password" />
                                                </div>
                                                <!--[error name="password" form="passwordform"]-->
                                                <div class="form-tip tip-error">
                                                    密码错误
                                                </div>
                                                <!--[/error]-->
                                            </div>
                                            <div class="formrow formrow-action">
                                                <span class="minbtn-wrap"><span class="btn"><input type="submit" name="submitPassword" value="确认" class="button" /></span></span>
                                            </div>
                                        </div>
                                        </form>
                                        <!--[else]-->

                                        <!--[ajaxpanel id="ap_photolist" idonly="true"]-->
                                        <div class="clearfix photonav">
                                            <div class="photonav-previous">
                                                <a href="$url(app/album/photo)?id=$PreListPhoto.PhotoID" onclick="ajaxRender(this.href,'ap_photoname,ap_photoimg,ap_photolist');return false;">&laquo;</a>
                                            </div>
                                            <div class="photonavlist">
                                                <ul class="clearfix photonav-list" id="photolist" style="width:{=$photolist.count * 63}px">
                                                    <!--[loop $photo in $photolist]-->
                                                    <li><a class="$_if($photoid==$photo.id,'current','')" href="javascript:;" onclick="submitPhoto($photo.id);return false;"><img src="$photo.thumbsrc" alt="" onload="vCenter(this)" /></a></li>
                                                    <!--[/loop]-->
                                                </ul>
                                            </div>
                                            <div class="photonav-next">
                                                <a href="$url(app/album/photo)?id=$NextListPhoto.PhotoID" onclick="ajaxRender(this.href,'ap_photoname,ap_photoimg,ap_photolist');return false;">&raquo;</a>
                                            </div>
                                        </div>
                                        <!--[/ajaxpanel]-->
                                        
                                        <!--[ajaxpanel id="ap_photoimg" idonly="true"]-->
                                        <div class="clearfix photoviewer">
                                            <div class="clearfix photo-viewertool">
                                                <div class="page-status">
                                                    $CurrentPhotoNumber / $TotalPhotos
                                                </div>
                                                <div class="photo-page">
                                                    <a class="page-previous" href="javascript:;" onclick="submitPhoto($PreviousPhoto.id);">&laquo;上一张</a>
                                                    <a class="page-next" href="javascript:;" onclick="submitPhoto($NextPhoto.id);">下一张&raquo;</a>
                                                </div>
                                            </div>
                                            <input type="hidden" id="NextPhotoid" value="$NextPhoto.id" />
                                            <input type="hidden" id="PreviousPhotoid" value="$PreviousPhoto.id" />
                                            <div class="clearfix photo-content">
                                                <div class="photo-entry">
                                                    <div class="photo-entry-inner" id="photo_container" style=" overflow:hidden;">
                                                      <img id="photoshow" src="$Photo.Src" alt="$Photo.Name.ToHtml" onload="updatePhotoSize(this); imageScale2(this);" style=" cursor:url($skin/images/cursor_right.cur), auto;"/>
                                                    </div>
                                                    <script type="text/javascript">
                                                        if (!containerWidth) containerWidth = $('photo_container').offsetWidth;
                                                        $("photo_container").style.width = containerWidth + "px";
                                                    </script>
                                                </div>
                                                <div class="photo-action">
                                                    <ul class="photo-action-list">
                                                        <li><a class="action-slide" href="#" onclick="slide_on = !slide_on;slideShow();return false;"><em>.</em>幻灯<span id="slide_count"></span></a></li>
                                                        <!--[if $CanUseCollection]-->
                                                        <li><a class="action-share" href="$dialog/share-create.aspx?type=collection&sharetype=Picture&targetID=$Photo.ID" onclick="return openDialog(this.href);"><em>.</em>收藏</a></li>
                                                        <!--[/if]-->
                                                        <!--[if $CanUseShare]-->
                                                        <li><a class="action-share" href="$dialog/share-create.aspx?type=share&sharetype=Picture&targetID=$Photo.ID" onclick="return openDialog(this.href);"><em>.</em>分享</a></li>
                                                        <!--[/if]-->
                                                        <li><a class="action-report"  href="$dialog/report-add.aspx?type=photo&id=$photo.id&uid=$album.userid" onclick="return openDialog(this.href)"><em>.</em>举报</a></li>
                                                        <!--[if $CanManagePhoto]-->
                                                        <li><a class="action-edit" href="$url(app/album/list)?id=$photo.albumid&photoids=$photo.id&mode=manage" target="_blank"><em>.</em>编辑</a></li>
                                                        <li><a class="action-delete" href="$dialog/album-deletephotos.aspx?photoids=$photo.id" onclick="return openDialog(this.href, function(){location.href='$url(app/album/photo)?id=$NextPhoto.ID';})"><em>.</em>删除</a></li>
                                                        <!--[/if]-->
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="clearfix photolayout">
                                            <div class="photoreview">
                                                <div class="clearfix photoreview-inner">
                                                    <div class="avatar photo-owner">
                                                        <a href="$url(space/$appowner.Userid)"><img src="$appowner.Avatarpath" alt="" width="48" height="48" /></a>
                                                    </div>
                                                    <div class="review-content">
                                                        <div class="clearfix photoexplain">
                                                            <div class="bubble-nw"><div class="bubble-ne"><div class="bubble-n"><div class="bubble-pointer">&nbsp;</div></div></div></div>
                                                            <div class="bubble-e"><div class="clearfix bubble-w">
                                                                <h3 class="photo-title">$photo.name</h3>
                                                                <div class="photo-description">
                                                                    $_if(string.IsNullOrEmpty($photo.description), "暂时没有相片描述", $photo.Description)
                                                                </div>
                                                            </div></div>
                                                            <div class="bubble-sw"><div class="bubble-se"><div class="bubble-s">&nbsp;</div></div></div>
                                                        </div>
                                                        <div class="entry-comment photocomment" id="comments">
                                                        <!-- #include file="../../_inc/_commentlist2.aspx"  commentList="$commentList"  commentType="photo" -->
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="photodetail">
                                                <div class="photodetail-inner">
                                                    <h3 class="photodetail-cate"><span>照片信息</span></h3>
                                                    <ul class="photodetail-group">
                                                        <li><span class="label">上传时间</span> <span class="value">$photo.createdate</span></li>
                                                        <li><span class="label">图片尺寸</span> <span class="value" id="photo_size">未知</span></li>
                                                        <li><span class="label">文件大小</span> <span class="value">$filesize($photo.filesize)</span></li>
                                                        <li><span class="label">文件格式</span> <span class="value">$photo.filetype</span></li>
                                                    </ul>
                                                    <%-- 
                                                    <h3 class="photodetail-cate"><span>EXIF信息</span></h3>
                                                    <ul class="photodetail-group">
                                                        <li><span class="label">相机</span> <span class="value">-</span></li>
                                                        <li><span class="label">光圈</span> <span class="value">-</span></li>
                                                        <li><span class="label">快门</span> <span class="value">-</span></li>
                                                        <li><span class="label">焦距</span> <span class="value">-</span></li>
                                                        <li><span class="label">ISO</span> <span class="value">-</span></li>
                                                        <li><span class="label">时间</span> <span class="value">-</span></li>
                                                    </ul>
                                                    --%>
                                                    <h3 class="photodetail-cate"><span>查看照片</span></h3>
                                                    <ul class="photodetail-group">
                                                        <li><a href="$photo.src" target="_blank">新窗口打开照片</a></li>
                                                        <li><a href="$photo.DownloadUrl" target="_blank">下载照片</a></li>
                                                    </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <!--[/ajaxpanel]-->
                                        
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
