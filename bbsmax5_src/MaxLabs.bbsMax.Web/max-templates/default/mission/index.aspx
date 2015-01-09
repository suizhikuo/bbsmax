<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
</head>
<body>
<div class="container section-app">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/event_time.gif);">任务</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a class="current" href="$url(mission/index)"><span>新任务</span></a></li>
                                        <li><a href="$url(mission/current)"><span>我的任务</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace app-mission">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        
                                        <div class="filtertab missionfilter">
                                            <ul class="clearfix tab-list">
                                                <li><a href="$url(mission/index)" $_if($_get.cid == null, ' class="current"')>全部</a></li>
                                             <!--[loop $category in $categoryList]-->
                                                <li><a href="$url(mission/index)?cid=$category.id" $_if($_get.cid == $category.id.tostring(), ' class="current"')>$category.name</a></li>
                                             <!--[/loop]-->
                                            </ul>
                                        </div>
                                        <!--[if $missioncount > 0]-->
                                        <div class="missionlist">
                                            <table>
                                                <thead>
                                                    <tr class="missionitem">
                                                        <td class="mission-icon">
                                                            &nbsp;
                                                        </td>
                                                        <td class="mission-entry">
                                                            任务
                                                        </td>
                                                        <td class="mission-award">
                                                            奖励
                                                        </td>
                                                        <td class="mission-action">
                                                            &nbsp;
                                                        </td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        <!--[loop $mission in $missionlist]-->
                                                    <tr class="missionitem">
                                                        <td class="mission-icon">
                                                            <img src="$mission.iconpath" alt="" />
                                                        </td>
                                                        <td class="mission-entry">
                                                            <h3 class="mission-title">
                                                                <a href="$url(mission/detail)?mid=$mission.id">$mission.Name</a>
                                                                <a title="该任务参与人数: $mission.TotalUsers." class="usercount" href="$url(mission/detail)?mid=$mission.ID">参与人次 $mission.TotalUsers</a>
                                                            </h3>
                                                            <div class="mission-summary">
                                                                $mission.Description
                                                            </div>
                                                            
                                                            <!--[if $mission.childmissions != null && $mission.childmissions.count > 0]-->
                                                            <div class="mission-steplist">
                                                                <ol>
                                                                    <!--[loop $child in $mission.childmissions with $i]-->
                                                                    <li>
                                                                        <span class="mission-step-number">{=$i + 1}</span>
                                                                        $child.name
                                                                    </li>
                                                                    <!--[/loop]-->
                                                                </ol>
                                                            </div>
                                                            <!--[/if]-->
                                                        </td>
                                                        <td class="mission-award">
                                                            $Mission.PrizeDescription(@"<p><span class=""label"">{0}</span> <span class=""value"">{1}{2}</span></p>","")
                                                        </td>
                                                        <td class="mission-action">
                                                            <!--[if $mission.CanApply]-->
                                                            <form action="$_form.action" method="post">
                                                            <input type="hidden" value="$mission.id" name="missionid" />
                                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="参与任务" name="apply" /></span></span>
                                                            </form>
                                                            <!--[else]-->
                                                            <span class="minbtn-wrap minbtn-disable"><span class="btn"><span class="button">参与任务</span></span></span>
                                                            <p class="mission-note">暂时不符合参与条件</p>
                                                            <!--[/if]-->
                                                        </td>
                                                    </tr>
                                        <!--[/loop]-->
                                                </tbody>
                                            </table>
                                        </div>
                                        <!--[pager name="pager1" skin="../_inc/_pager_app.aspx" count="$MissionCount" pagesize="5" ]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            暂时没有任何新任务.
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
