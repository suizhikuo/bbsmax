<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/other.css" />
</head>
<body>
<div class="container section-members">
<!--#include file="_inc/_head.aspx"-->
    <div id="main" class="main">
        <div class="clearfix main-inner">
            <div class="content">
                <!--#include file="_inc/_round_top.aspx"-->
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">

                        <!--[if $view=="show"]-->
                            <!--#include file="_member_inc/_pointshow.aspx"-->
                        <!--[else if $View == "onlineuser" || $View == "onlineguest"]-->
                            <!--#include file="_member_inc/_online.aspx"-->
                        <!--[else if $view == "search"]-->
                            <!--#include file="_member_inc/_registerusers.aspx"-->
                        <!--[else if $view=="viewnumber"||$view=="female"||$view=="male"]-->
                            <!--#include file="_member_inc/_popularranking.aspx"-->
                        <!--[else if $view == "point"]-->
                            <!--#include file="_member_inc/_pointranking.aspx"-->
                        <!--[/if]-->
                            
                        </div>
                    </div>
                    <div class="content-sub">
                        <div class="content-sub-inner">
                            <div class="sidebar">
                                <div class="panel rankingcategory">
                                    <div class="panel-head">
                                        <h3 class="panel-title"><span>会员排行</span></h3>
                                    </div>
                                    <div class="panel-body">
                                        <ul class="category-list">
                                            <li><a class="$GetCurrentClass("show","current")" href="$url(members)?view=show">竞价排行</a></li>
                                            <li><a class="$_if($view=='viewnumber'||$view=='female'||$view=='male','current')" href="$url(members)?view=viewnumber">人气排行</a></li>
                                            <li><a class="$GetCurrentClass("point","current")" href="$url(members)?view=point">活跃度排行</a></li>
                                            <%--
                                            <li><a class="$GetCurrentClass("female","current")" href="$url(members)?view=female">美女排行</a></li>
                                            <li><a class="$GetCurrentClass("male","current")" href="$url(members)?view=male">帅哥排行</a></li>
                                            <li><a class="$GetCurrentClass("friendnumber","current")" href="$url(members)?view=friendnumber">好友数排行</a></li>
                                            <li><a class="$GetCurrentClass("onlinetimes","current")" href="$url(members)?view=onlinetimes">在线时间排行</a></li>
                                            <li><a class="$GetCurrentClass("postcount","current")" href="$url(members)?view=postcount">论坛发贴排行</a></li>
                                            --%>
                                        </ul>
                                    </div>
                                </div>
                                <!--[if $IsShowOnline]-->
                                <div class="panel onlinecategory">
                                    <div class="panel-head">
                                        <h3 class="panel-title"><span>在线列表</span></h3>
                                    </div>
                                    <div class="panel-body">
                                        <ul class="category-list">
                                            <!--[if $IsShowOnlineMember]-->
                                            <li><a class="$GetCurrentClass("onlineuser","current")" href="$url(members)?view=onlineuser">在线会员</a></li>
                                            <!--[/if]-->
                                            <!--[if $IsShowOnlineGuest]-->
                                            <li><a class="$GetCurrentClass("onlineguest","current")" href="$url(members)?view=onlineguest">在线游客</a></li>
                                            <!--[/if]-->
                                        </ul>
                                    </div>
                                </div>
                                <!--[/if]-->
                                <div class="panel membercategory">
                                    <div class="panel-head">
                                        <h3 class="panel-title"><span>会员列表</span></h3>
                                    </div>
                                    <div class="panel-body">
                                        <ul class="category-list">
                                            <li><a class="$_if($view=='search','current')" href="$url(members)?view=search">注册会员</a></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
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
