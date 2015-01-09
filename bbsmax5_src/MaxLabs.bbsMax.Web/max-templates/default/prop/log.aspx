<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
</head>
<body>
<div class="container">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar section-app">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/magic.gif);">道具</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a href="$url(prop/index)"><span>道具中心</span></a></li>
                                        <li><a href="$url(prop/my)"><span>我的道具</span></a></li>
                                        <li><a href="$url(prop/selling)"><span>二手市场</span></a></li>
                                        <li><a class="current" href="$url(prop/log)"><span>道具记录</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace app-proplog">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <div class="filtertab">
                                            <ul class="clearfix tab-list">
                                                <li><a href="$AttachQueryString("page=1&type=")" class="$IsSelected(PropLogType.All,"current")"><span>全部</span></a></li>
                                                <li><a href="$AttachQueryString("page=1&type="+PropLogType.Buy)" class="$IsSelected(PropLogType.Buy,"current")"><span>购买</span></a></li>
                                                <li><a href="$AttachQueryString("page=1&type="+PropLogType.Gift)" class="$IsSelected(PropLogType.Gift,"current")"><span>赠送</span></a></li>
                                                <li><a href="$AttachQueryString("page=1&type="+PropLogType.Given)" class="$IsSelected(PropLogType.Given,"current")"><span>获赠</span></a></li>
                                                <li><a href="$AttachQueryString("page=1&type="+PropLogType.Use)" class="$IsSelected(PropLogType.Use,"current")"><span>使用</span></a></li>
                                                <li><a href="$AttachQueryString("page=1&type="+PropLogType.BeUsed)" class="$IsSelected(PropLogType.BeUsed,"current")"><span>被使用</span></a></li>
                                            </ul>
                                        </div>
                                        <!--[if $loglist.count > 0]-->
                                        <div class="proploglist">
                                            <ul class="proplog-list">
                                                <!--[loop $log in $LogList]-->
                                                <li>
                                                    <span class="entry">$log.log</span>
                                                    <span class="date">$OutputFriendlyDate($log.CreateDate)</span>
                                                </li>
                                                <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[pager skin="../_inc/_pager_app.aspx" count="$TotalLogCount" pagesize="$LogListPageSize" ]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            暂时没有道具记录.
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
