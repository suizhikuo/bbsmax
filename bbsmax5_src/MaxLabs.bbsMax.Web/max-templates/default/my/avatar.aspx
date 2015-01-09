<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/setting.css" />
</head>
<body>
<div class="container section-setting">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="../_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                            
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span>设置头像</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a class="$_if($IsMakingMode,'current')" href="$url(my/avatar)?mode=making"><span>制作头像</span></a></li>
                                        <li><a class="$_if($IsShotMode,'current')" href="$url(my/avatar)?mode=shot"><span>在线拍摄</span></a></li>
                                    </ul>
                                </div>
                            </div>
                            <form action="$_form.action" method="post" enctype="multipart/form-data">
                            <div class="clearfix avatarlayout">
                                <div class="avatarcreator">
                                    <div class="avatarcreator-inner">
                                        <!--[if $EnableAvatarCheck]-->
                                        <div class="alertmsg">
                                            注意：您设置的头像需要等待管理员审核
                                            <!--[if $CanCheckAvatar]-->
                                            （您自己就有审核头像的权限,因此无须审核)
                                            <!--[/if]-->
                                        </div>
                                        <!--[/if]-->
                                        <!--[unnamederror]-->
                                        <div id="errorMessage" class="errormsg">$message</div>
                                        <!--[/unnamederror]-->
                                        <div class="successmsg" style="display:none;" id="successmsg">头像制作完成, 您需要刷新页面才能看到新头像! </div>
                                        <div class="avatarflash" id="avatarShow">
                                            <object classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000"
                                                id="Object1" width="600" height="500"
                                                codebase="http://fpdownload.macromedia.com/get/flashplayer/current/swflash.cab">
                                                <param name="movie" value="$root/max-assets/flash/KaChaMax.swf" />
                                                <param name="quality" value="high" />
                                                <param name="bgcolor" value="#FFFFFF" />
                                                <param name="wmode" value="opaque" />
                                                <param name="allowScriptAccess" value="sameDomain" />
                                                <param name="flashVars" value="$AvatarBuilderVars" />
                                                <embed src="$root/max-assets/flash/KaChaMax.swf" quality="high" bgcolor="#FFFFFF"
                                                    width="600" height="500" name="KaChaMax" align="middle"
                                                    play="true"
                                                    loop="false"
                                                    quality="high"
                                                    wmode="opaque"
                                                    allowScriptAccess="sameDomain"
                                                    type="application/x-shockwave-flash"
                                                    pluginspage="http://www.adobe.com/go/getflashplayer"
                                                    flashVars="$AvatarBuilderVars">
                                                </embed>
                                            </object>
                                        </div>
                                    </div>
                                </div>
                                <div class="avatarpreview">
                                    <div class="avatarpreview-inner">
                                        <h3 class="avatar-title">当前使用头像</h3>
                                        <div class="avatar-image">
                                            <img id="mybigavatar" src="{=$my.bigavatarpath}?a=1" width="120" height="120" alt="" />
                                        </div>
                                        <div class="avatar-image">
                                            <img id="myavatar" src="{=$my.avatarpath}?a=1" width="48" height="48" alt="" />
                                        </div>
                                        <div class="avatar-image">
                                            <img id="mysmallAvatar" src="{=$my.smallavatarpath}?a=1" width="24" height="24" alt="" />
                                        </div>
                                        <!--[if !$my.IsDefaultAvatar]-->
                                        <div class="clearfix avatar-action">
                                            <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" value="删除头像" name="clearavatar" onclick="return confirm('确定要恢复系统默认头像吗?');"/></span></span>
                                        </div>
                                        <!--[/if]-->
                                        <!--[if $HasUncheckAvatar]-->
                                        <h3 class="avatar-title">待审核头像</h3>
                                        <div class="avatar-image">
                                            <img src="$UncheckedAvatarUrl" width="120" height="120" alt="" />
                                        </div>
                                        <div class="avatar-action">
                                            <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" value="取消审核" name="cancelavatarcheck" onclick="return confirm('确定要取消头像审核吗?')"/></span></span>
                                        </div>
                                        <!--[/if]-->
                                        <div class="avatarnote">
                                            <p>允许上传不超过 <strong>1mb</strong> 的 jpg, gif, png 等格式的图片.</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            </form>
                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
<script type="text/javascript">
    var userID = $my.UserID;
    var isDefault = $_if($My.IsDefaultAvatar,'true','false');
    function updateAvatar() {
    if (isDefault) { alert("头像保存成功!"); refresh(); return; } 
    $('successmsg').style.display = ''; 
    var a = $('mybigavatar');
    var a1 = $("myavatar");
    var a2 = $("mysmallAvatar");
    a1.src = a1.src + "&r=" + Math.random();
    a.src = a.src + "&r=" + Math.random();
    a2.src = a2.src + "?r=" + Math.random();
    //refresh();
}  
</script>
</div>
</body>
</html>
