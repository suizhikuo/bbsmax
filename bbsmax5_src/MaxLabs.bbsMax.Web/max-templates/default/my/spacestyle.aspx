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
                                <h3 class="pagecaption-title"><span>个人空间风格设置</span></h3>
                            </div>
                            <div class="formcaption">
                                <p>你可以预览或者使用这里的任何个人空间风格.</p>
                            </div>

                            <div class="spacethemes">
                                <ul class="clearfix themelist">
                                    <!--[loop $Theme in $ThemeList]-->
                                    <li>
                                        <p class="theme-thumb">
                                            <img src="$Root/max-spacestyles/$theme.Dir/preview.jpg" alt="" width="120" height="120" />
                                            <!--[if $CurrentSpaceTheme.Name == $theme.Name]-->
                                            <span class="theme-current">当前使用风格</span>
                                            <!--[else]-->
                                            <a class="theme-active" href="$url(space/$MyUserID)?theme=$Theme.Dir&op=apply">启用</a>
                                            <a class="theme-preview" href="$url(space/$MyUserID)?theme=$Theme.Dir" target="_blank">预览</a>
                                            <!--[/if]-->
                                        </p>
                                        <p class="theme-title">$theme.Name</p>
                                    </li>
                                    <!--[/loop]-->
                                </ul>
                            </div>
                            
                        </div>
                    </div>
                    <!--#include file="_sidebar_setting.aspx"-->
                </div>
                <!--#include file="../_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>
