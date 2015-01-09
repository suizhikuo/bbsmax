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
                                        <li><a href="$url(mission/index)"><span>新任务</span></a></li>
                                        <li><a href="$url(mission/current)" class="current"><span>我的任务</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace app-mission">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
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
                                        <!--[loop $usermission in $missionlist]-->
                                                    <tr class="missionitem">
                                                        <td class="mission-icon">
                                                            <img src="$usermission.mission.iconpath" alt="" />
                                                        </td>
                                                        <td class="mission-entry">
                                                            <h3 class="mission-title">
                                                                <a href="$url(mission/detail)?mid=$usermission.mission.ID">$usermission.mission.Name</a>
                                                                <a title="该任务参与人数: $usermission.mission.TotalUsers" class="usercount" href="$url(mission/detail)?mid=$usermission.mission.ID">参与人数 $usermission.mission.TotalUsers</a>
                                                            </h3>
                                                            <div class="mission-summary">
                                                                $usermission.mission.Description
                                                            </div>
                                                            <!--[if $usermission.mission.childmissions != null && $usermission.mission.childmissions.count > 0]-->
                                                            <div class="mission-steplist">
                                                                <ul>
                                                                    <!--[loop $child in $usermission.mission.childmissions with $i]-->
                                                                    <!--[if $usermission.mission.stepusermission.mission.id == $child.id]-->
                                                                    <li>
                                                                        <span class="mission-step-number">{=$i + 1}</span>
                                                                        <a href="$url(mission/detail)?mid=$usermission.mission.ID&step=$i">$child.name</a>
                                                                        <div class="mission-progressbar">
                                                                            <div class="progressbar-chart"><div class="progressbar-completed" style="width:{=$usermission.mission.stepusermission.FinishPercent * 100}%"><div>&nbsp;</div></div></div>
                                                                            <div class="progressbar-tip">已完成{=$usermission.mission.stepusermission.FinishPercent * 100}%</div>
                                                                        </div>
                                                                    </li>
                                                                    <!--[else]-->
                                                                    <li>
                                                                        <span class="mission-step-number">{=$i + 1}</span>
                                                                        <a href="$url(mission/detail)?mid=$usermission.mission.ID&step=$i">$child.name</a>
                                                                    </li>
                                                                    <!--[/if]-->
                                                                    <!--[/loop]-->
                                                                </ul>
                                                            </div>
                                                            <!--[else]-->
                                                            <div class="mission-steplist">
                                                                <ul>
                                                                    <li>
                                                                        <div class="mission-progressbar">
                                                                            <div class="progressbar-chart"><div class="progressbar-completed" style="width:{=$usermission.FinishPercent * 100}%"><div>&nbsp;</div></div></div>
                                                                            <div class="progressbar-tip">已完成{=$usermission.FinishPercent * 100}%</div>
                                                                        </div>
                                                                    </li>
                                                                </ul>
                                                            </div>
                                                            
                                                            <!--[/if]-->
                                                        </td>
                                                        <td class="mission-award">
                                                            $usermission.Mission.PrizeDescription(@"<p><span class=""label"">{0}</span> <span class=""value"">{1}{2}</span></p>","")
                                                        </td>
                                                        <td class="mission-action">
                                                        <!--[if $usermission.FinishPercent == 1]-->
                                                            <form action="$_form.action" method="post">
                                                            <input type="hidden" value="$usermission.Mission.id" name="missionid" />
                                                            <span class="minbtn-wrap btn-highlight"><span class="btn"><input class="button" type="submit" value="领取奖励" name="getprize" /></span></span>
                                                            <p class="mission-note">任务完成了</p>
                                                            </form>
                                                        <!--[else]-->
                                                            <form action="$_form.action" method="post">
                                                            <input type="hidden" value="$usermission.Mission.id" name="missionid" />
                                                            <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="放弃任务" name="abandon" /></span></span>
                                                            <!--[if $usermission.status == MissionStatus.OverTime]-->
                                                            <p class="mission-note">任务超时失败</p>
                                                            <!--[else]-->
                                                            <p class="mission-note">正在进行中</p>
                                                            <!--[/if]-->
                                                            </form>
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
                                            暂时没有参与任何任务.
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
