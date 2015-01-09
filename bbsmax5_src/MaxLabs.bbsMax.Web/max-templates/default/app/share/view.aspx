<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app_share_frame.css" />
</head>
<body class="externalframe">
<table class="framelayout">
    <tr>
        <td class="topframe">
            <div class="clearfix topframe-inner">
                <div class="logo">
                    <a href="$url(default)" title="$BbsName" rel="home">
                        <img src="$skin/images/logo_small.gif" alt="$BbsName" />
                    </a>
                </div>
                
                <form action="$_form.action" method="post" id="form_share">
                <!--[ajaxpanel id="ap_lovehate"]-->
                <div class="sharelovehate">
                    <a class="sharelovehate-love $_if($CheckedLove == true, 'sharelovehate-love-checked', '')" href="#" title="顶" onclick="ajaxSubmit('form_share', 'agree', 'ap_lovehate', null, null, true); return false;"><span>$share.AgreeCount</span></a>
                    <a class="sharelovehate-hate $_if($CheckedHate == true, 'sharelovehate-hate-checked', '')" href="#" title="踩" onclick="ajaxSubmit('form_share', 'oppose', 'ap_lovehate', null, null, true); return false;"><span>$share.OpposeCount</span></a>
                </div>
                <!--[/ajaxpanel]-->
                </form>
                
                <div class="sharecount">
                    <!--[if $CanUseShare]-->
                    <a class="sharecount-button" href="$root/max-dialogs/share-create.aspx?refshareid=$share.shareid" onclick="return openDialog(this.href);">
                        <span class="sharecount-inner">
                            <span class="sharecount-text">我也分享 <span class="sharecount-stat">$share.ShareCount</span></span>
                        </span>
                    </a>
                     <!--[/if]-->
                    <!--[if $CanUseCollection]-->
                    <a class="sharecount-button" href="$root/max-dialogs/share-create.aspx?type=collection&refshareid=$share.shareid" onclick="return openDialog(this.href);">
                        <span class="sharecount-inner">
                            <span class="sharecount-text">我要收藏 </span>
                        </span>
                    </a>
                    <!--[/if]-->
                </div>
                
                <div class="closeframe">
                    <a href="$share.url"><span>X</span>关闭顶栏</a>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td class="mainframe">
            <iframe width="100%" height="100%" scrolling="auto" frameborder="0" src="$share.url" allowtransparency="true"></iframe>
        </td>
    </tr>
</table>
</body>
</html>