<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/other.css" />
</head>
<body>
<div class="container section-announcement">
<!--#include file="_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">
                        <!--[if $announcementList.count > 0]-->
                            <!--[loop $announcement in $announcementList]-->
                            <div class="panel announcemententry" id="$announcement.AnnouncementID">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>$announcement.Subject 
                                    <!--[if $announcement.EndDate < $DateNow]-->
                                    <em class="tip">(已过期)</em>
                                    <!--[/if]-->
                                    </span></h3>
                                    <p class="panel-toggle">
                                        <a id="collapse_0" class="collapse" onclick="Collapse(this, 1);return false;" title="收起/展开" href="#">收起/展开</a>
                                    </p>
                                </div></div></div>
                                <div class="panel-body">
                                    <ul class="announcement-info">
                                        <li><span class="label">发布人:</span> <span class="value"><a class="fn" href="$url(space/$announcement.PostUserID)">$announcement.User.Name</a></span></li>
                                        <li><span class="label">起始时间:</span> <span class="value">$outputdatetime($announcement.BeginDate)</span></li>
                                        <li><span class="label">结束时间:</span> <span class="value">
                                        <!--[if $announcement.EndDate == $DateTimeMaxValue]--> 
                                        无限期 
                                        <!--[else]-->
                                        $outputdatetime($announcement.EndDate)
                                        <!--[/if]-->
                                        </span></li>
                                    </ul>
                                    <div class="announcement-content">
                                        <!--[if $announcement.AnnouncementType == AnnouncementType.Text]-->
                                        $announcement.Content
                                        <!--[else]-->
                                        <a href="$formatLink($announcement.Content)" target="_blank">$announcement.Content</a>
                                        <!--[/if]-->
                                    </div>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            <!--[/loop]-->
                        <!--[else]-->
                            <div class="nodata">
                                当前没有任何公告.
                            </div>
                        <!--[/if]-->
                        </div>
                    </div>
                </div>
                <!--#include file="_inc/_round_bottom.aspx"-->
            </div>
        </div>
    </div>
<!--#include file="_inc/_foot.aspx"-->
</div>
</body>
</html>
