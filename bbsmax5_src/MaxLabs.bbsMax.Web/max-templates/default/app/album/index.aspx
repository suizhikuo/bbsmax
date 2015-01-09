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
                                        <li><a $_if($selectedEveryone, 'class="current"') href="$url(app/album/index)?view=everyone"><span>大家的相册</span></a></li>
                                        <li><a $_if($selectedFriend, 'class="current"') href="$url(app/album/index)?view=friend"><span>好友的相册</span></a></li>
                                        <li><a $_if($selectedMy, 'class="current"') href="$url(app/album/index)?view=my"><span>我的相册</span></a></li>
                                    </ul>
                                    <ul class="pagebutton">
                                        <li><a class="newalbum" href="$url(app/album/upload)?create=1">新建相册</a></li>
                                        <li><a class="uploadphoto" href="$url(app/album/upload)">上传照片</a></li>
                                    </ul>
                                </div>
                                <!--[/if]-->
                            </div>
                        
                            <div class="clearfix workspace app-album">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <!--[if $albumlist.count > 0]-->
                                        <div class="albumlist">
                                            <ul class="clearfix album-list">
                                                <!--[loop $album in $albumlist]-->
                                                <li class="albumitem">
                                                    <div class="clearfix albumitem-inner">
                                                        <div class="album-coverwrapper">
                                                            <div class="album-cover">
                                                                <a href="$url(app/album/list)?id=$Album.ID">
                                                                    <!--[if $CanSeePhoto($album)]-->
                                                                    <img src="$Album.CoverSrc" alt="" />&nbsp;
                                                                    <!--[else]-->
                                                                    <img src="$root/max-assets/images/default_photo.gif" alt="" />&nbsp;
                                                                    <!--[/if]-->
                                                                </a>
                                                            </div>
                                                            <!--[if $Album.CanUploadPhoto]-->
                                                            <div class="album-uploadphoto">
                                                                <a href="$url(app/album/upload)?id=$Album.ID">上传照片</a>
                                                            </div>
                                                            <!--[/if]-->
                                                            <span class="album-counts">照片数: $album.TotalPhotos</span>
                                                        </div>
                                                        <div class="album-entry">
                                                            <div class="album-title">
                                                                <a class="album-name" href="$url(app/album/list)?id=$Album.ID">$CutString($Album.Name,16)</a>
                                                                
                                                                <!--[if $album.DisplayForPasswordHolderOnly]-->
                                                                <span class="privacy">凭密码可见</span>
                                                                <!--[else if $album.DisplayForFriendOnly]-->
                                                                <span class="privacy">仅好友可见</span>
                                                                <!--[else if $album.DisplayForOwnerOnly]-->
                                                                <span class="privacy">仅自己可见</span>
                                                                <!--[/if]-->
                                                            </div>
                                                            <div class="album-description">
                                                                $CutString($Album.Description,30)
                                                            </div>
                                                            <div class="album-info">
                                                                <!--[if $selectedMy == false]-->
                                                                <p class="album-owner">
                                                                    <a class="fn url" href="$url(space/$album.User.id)">$album.User.Name</a> 发布
                                                                </p>
                                                                <!--[/if]-->
                                                                <p class="album-update">
                                                                    $album.FriendlyUpdateDate更新
                                                                </p>
                                                            </div>
                                                            <!--[if $CanSeePhoto($album)]-->
                                                            <!--[if $photolist($album.id) != null && $photolist($album.id).count > 0 ]-->
                                                            <div class="album-recent">
                                                                <ul class="clearfix album-recent-list">
                                                                    <li>
                                                                        <a href="$url(app/album/photo)?id=$photolist($album.id)[0].id"><img src="$photolist($album.id)[0].thumbsrc" alt="" />&nbsp;</a>
                                                                    </li>
                                                                    <!--[if $photolist($album.id).count > 1 ]-->
                                                                    <li>
                                                                        <a href="$url(app/album/photo)?id=$photolist($album.id)[1].id"><img src="$photolist($album.id)[1].thumbsrc" alt="" />&nbsp;</a>
                                                                    </li>
                                                                    <!--[/if]-->
                                                                    <!--[if $photolist($album.id).count > 2 ]-->
                                                                    <li>
                                                                        <a href="$url(app/album/photo)?id=$photolist($album.id)[2].id"><img src="$photolist($album.id)[2].thumbsrc" alt="" />&nbsp;</a>
                                                                    </li>
                                                                    <!--[/if]-->
                                                                </ul>
                                                            </div>
                                                            <!--[/if]-->
                                                            <!--[/if]-->
                                                        </div>
                                                        <!--[if $album.canedit || $album.candelete]-->
                                                        <div class="entry-action">
                                                            <!--[if $album.canedit]-->
                                                            <a class="action-edit" href="$dialog/album-update.aspx?albumid=$album.id" title="编辑相册信息" onclick="return openDialog(this.href, refresh);">编辑</a>
                                                            <!--[/if]-->
                                                            <!--[if $album.candelete]-->
                                                            <a class="action-delete" href="$dialog/album-delete.aspx?albumid=$album.id" onclick="return openDialog(this.href,refresh);" title="删除该相册">删除</a>
                                                            <!--[/if]-->
                                                        </div>
                                                        <!--[/if]-->
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[pager name="pager1" skin="../../_inc/_pager_app.aspx" count="$AlbumTotalCount" pagesize="$AlbumListPageSize" ]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            暂时没有任何相册.
                                        </div>
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
