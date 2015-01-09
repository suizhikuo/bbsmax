<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
</head>
<body>
<div class="container section-notify">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/megaphone.gif);">通知</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a href="$url(my/notify)"><span>通知</span></a></li>
                                        <li><a class="current" href="$url(my/notify-setting)"><span>通知接收设置</span></a></li>
                                    </ul>
                                </div>
                            </div>
                            <div class="formcaption">
                                <p>如果您觉得这些通知打扰到您，您可以自定义是否接收某类型的通知.</p>
                            </div>
                            <!--[success]-->
                            <div class="successmsg" id="successmsg">设置保存成功.</div>
                            <!--[/success]-->
                            <form action="$_form.action" method="post">
                            <div class="formgrounp">
                                
                                <div class="formrow">
                                    <h3 class="label">通知类型</h3>
                                    <ul class="clearfix form-optionlist">
                                        <!--[loop $item in $NotifyList with $i]-->
                                        <li>
                                            <!--[if $item.OpenState == NotifyState.AlwaysClose]-->
                                            <input type="checkbox" disabled="disabled" name="$item.NotifyType" id="n_$i" />
                                            <label for="n_$i"> $GetNotifyName( $item.NotifyType)</label>
                                            <!--[else if $item.OpenState == NotifyState.AlwaysOpen]-->
                                            <input type="checkbox" disabled="disabled" checked="checked" id="n_$i"name="$item.NotifyType" />
                                            <label for="n_$i">$GetNotifyName( $item.NotifyType)</label>
                                            <!--[else if $item.OpenState == NotifyState.DefaultOpen]-->
                                            <input type="checkbox" checked="checked"  name="$item.NotifyType" value="DefaultOpen" id="n_$i" />
                                            <label for="n_$i">$GetNotifyName( $item.NotifyType)</label>
                                            <!--[else if $item.OpenState == NotifyState.DefaultClose]-->
                                            <input type="checkbox"  name="$item.NotifyType" id="n_$i" value="DefaultOpen" />
                                            <label for="n_$i">$GetNotifyName( $item.NotifyType)</label>
                                            <!--[/if]-->
                                        </li>
                                        <!--[/loop]-->
                                    </ul>
                                </div>
                                
                                <div class="formrow formrow-action">
                                    <span class="minbtn-wrap"><span class="btn"><input type="submit" class="button" name="savesetting" value="保存设置" /></span></span>
                                </div>
                            </div>
                            </form>
                            
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
            <!--#include file="../_inc/_sidebar_app.aspx"-->
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
