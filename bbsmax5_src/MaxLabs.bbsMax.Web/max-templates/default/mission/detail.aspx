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
                                        <li><a $_if($usermission == null || $usermission.FinishPercent == 1, 'class="current"') href="$url(mission/index)"><span>新任务</span></a></li>
                                        <li><a $_if($usermission != null && $usermission.FinishPercent != 1, 'class="current"') href="$url(mission/current)"><span>我的任务</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace app-mission">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <div class="clearfix workspacehead">
                                            <div class="pagecrumbnav">
                                                <!--[if $usermission == null]-->
                                                <a href="$url(mission/index)">新任务</a>
                                                <!--[else]-->
                                                <a href="$url(mission/current)">
                                                我的任务
                                                </a>
                                                <!--[/if]-->
                                                &raquo;
                                                $Mission.Name
                                            </div>
                                        </div>
                                        
                                        <div class="missiondetial">
                                            <h3 class="mission-title">
                                                $Mission.Name
                                            </h3>
                                            <div class="mission-icon">
                                                <img src="$mission.iconpath" alt="" />
                                            </div>
                                            
                                    <!--[if $UserMission == null || $UserMission.FinishPercent != 1]-->
                                        <!--[if $UserMission != null && $HasAnyError == false]-->
                                            <!--[if $UserMission.Status == MissionStatus.Abandon]-->
                                            <div class="mission-tip misssion-abandon">您已经放弃了这个任务。</div>
                                            <!--[else if $UserMission.Status == MissionStatus.OverTime]-->
                                            <div class="mission-tip misssion-fail">您在 $UserMission.CreateDate.Friendly 申请了参与这个任务，但是没有在限定时间内完成而导致任务失败了。</div>
                                            <!--[else]-->
                                            <div class="mission-tip misssion-working">
                                                <!--[if $IsNowAction]-->
                                                您成功申请了这个任务。
                                                <!--[else]-->
                                                您在 $UserMission.CreateDate.Friendly 申请了这个任务。
                                                <!--[/if]-->
                                            </div>
                                          <!--[/if]-->
                                        <!--[/if]-->
                                    <!--[else]-->
                                            <div class="mission-tip misssion-complete">
                                            <!--[if $mission.havePrize == true]-->
                                                <!--[if $UserMission.IsPrized == false]-->
                                                您已经成功完成了任务，快去领取任务奖励吧！
                                                <!--[else if $IsNowAction]-->
                                                恭喜，您已经成功完成任务并领取到了奖励！
                                                <!--[else]-->
                                                上一次完成时间：$UserMission.CreateDate.Friendly （已领取奖励）
                                                <!--[/if]-->
                                            <!--[/if]-->
                                            </div>
                                            <!--[if $mission.havePrize == true]-->
                                            <!--[if $UserMission.IsPrized && $IsNowAction]-->
                                             <div class="mission-award">
                                                <h4 class="title">奖励</h4>
                                                <div class="mission-award-content">
                                                    $Mission.PrizeDescription(@"<p><span class=""label"">{0}</span> <span class=""value"">{1}{2}</span></p>","")
                                                </div>
                                            </div>
                                            <!--[/if]-->
                                            <!--[/if]-->
                                    <!--[/if]-->
                                           
                                            <!--[if $IsShowApplyMissionButton == false && $UserMission == null]-->
                                            <div class="mission-tip misssion-notconform">您目前不符合参与该任务的条件.</div>
                                            <!--[/if]-->
                                            
                                            <!-- IsNowAction 是否是现在操作 -->
                                            <!-- IsFinished 是否已经完成 -->
                                            <!-- HasGetPrize 是否已经领完奖励 -->
                                            <!--[if $IsNowAction == false]-->
                                            
                                            <div class="mission-content">
                                                $Mission.Description
                                            </div>
                                            
                                            <div class="mission-award">
                                                <h4 class="title">奖励</h4>
                                                <div class="mission-award-content">
                                                    $Mission.PrizeDescription(@"<p><span class=""label"">{0}</span> <span class=""value"">{1}{2}</span></p>","")
                                                </div>
                                            </div>
                                            
                                            <!--[if $Mission.IsDisplayApplyCondition || $Mission.IsDisplayDeductPointDescription]-->
                                            <div class="mission-info">
                                                <h4 class="title">参与任务所需条件</h4>
                                                <div class="mission-info-content">
                                                    $Mission.ApplyConditionDescription
                                                </div>
                                                <div class="mission-info-content">
                                                    扣除积分
                                                    $Mission.DeductPointDescription
                                                </div>
                                            </div>
                                            <!--[/if]-->

                                            <!--[if $Mission.IsDisplayFinishCondition]-->
                                            <div class="mission-info">
                                                <h4 class="title">完成任务所需条件</h4>
                                                <div class="mission-info-content">
                                                    $Mission.FinishConditionDescription
                                                </div>
                                            </div>
                                            <!--[/if]-->
                                            
                                            <!--[if $UserMission != null]-->
                                            <div class="mission-info">
                                                <h4 class="title">任务进度</h4>
                                                <div class="mission-info-content">
                                                    <div class="mission-progressbar">
                                                        <div class="progressbar-chart"><div class="progressbar-completed" style="width:{=$UserMission.FinishPercent * 100}%"><div>&nbsp;</div></div></div>
                                                        <div class="progressbar-tip">已完成{=$UserMission.FinishPercent * 100}%</div>
                                                    </div>
                                                </div>
                                            </div>
                                            <!--[/if]-->
                                            
                                            <!--[if $mission.childmissions != null && $mission.childmissions.count > 0]-->
                                            <div class="clearfix mission-steplayout">
                                                <div class="mission-steptab">
                                                    <div class="mission-step">
                                                        <ul class="clearfix">
                                                            <!--[loop $mission in $mission.childmissions with $i]-->
                                                            <li $_if($_get.step == $i.tostring() || $_get.step == null && $stepusermission != null && $mission.id == $stepusermission.mission.id, 'class="current"')>
                                                                <!--[if $stepusermission != null]-->
                                                                  <!--[if $mission.id == $stepusermission.mission.id]-->
                                                                    <!--[if $IsShowGetStepMissionPrizeButton]-->
                                                                      <a class="mission-finish" title="已完成" href="$AttachQueryString('step=$i')">{=$i + 1}</a>
                                                                    <!--[else]-->
                                                                      <a class="mission-current" title="正在进行中" href="$AttachQueryString('step=$i')">{=$i + 1}</a>
                                                                    <!--[/if]-->
                                                                  <!--[else]-->
                                                                    <!--[if $mission.sortorder < $stepusermission.mission.sortorder]-->
                                                                      <a class="mission-finish" title="已完成" href="$AttachQueryString('step=$i')">{=$i + 1}</a>
                                                                    <!--[else]-->
                                                                      <a class="mission-disable" title="未进行" href="$AttachQueryString('step=$i')">{=$i + 1}</a>
                                                                    <!--[/if]-->
                                                                  <!--[/if]-->
                                                                <!--[/if]-->
                                                                <a class="mission-disable" title="未进行" href="$AttachQueryString('step=$i')">{=$i + 1}</a>
                                                                <!--[/if]-->
                                                            </li>
                                                            <!--[/loop]-->
                                                        </ul>
                                                    </div>
                                                </div>
                                            
                                                <div class="mission-stepinfo">
                                                    <h3 class="mission-title">
                                                        $StepMission.Name
                                                    </h3>
                                                    
                                                    <div class="mission-content">
                                                        $StepMission.Description
                                                    </div>
                                                    
                                                    <div class="mission-award">
                                                        <h4 class="title">奖励</h4>
                                                        <div class="mission-award-content">
                                                            $StepMission.PrizeDescription(@"<p><span class=""label"">{0}</span> <span class=""value"">{1}{2}</span></p>","")
                                                        </div>
                                                    </div>
                                                
                                                    <!--[if $StepMission.IsDisplayApplyCondition || $StepMission.IsDisplayDeductPointDescription]-->
                                                    <div class="mission-info">
                                                        <h4 class="title">参与任务所需条件</h4>
                                                        <div class="mission-info-content">
                                                            $StepMission.ApplyConditionDescription
                                                        </div>
                                                        <div class="mission-info-content">
                                                            扣除积分
                                                            $StepMission.DeductPointDescription
                                                        </div>
                                                    </div>
                                                    <!--[/if]-->

                                                    <!--[if $StepMission.IsDisplayFinishCondition]-->
                                                    <div class="mission-info">
                                                        <h4 class="title">完成任务所需条件</h4>
                                                        <div class="mission-info-content">
                                                            $StepMission.FinishConditionDescription
                                                        </div>
                                                    </div>
                                                    <!--[/if]-->
                                                    
                                                    <!--[if $StepUserMission != null]-->
                                                    <div class="mission-info">
                                                        <h4 class="title">任务进度</h4>
                                                        <div class="mission-info-content">
                                                            <div class="mission-progressbar">
                                                                <div class="progressbar-chart"><div class="progressbar-completed" style="width:{=StepUserMission.FinishPercent * 100}%"><div>&nbsp;</div></div></div>
                                                                <div class="progressbar-tip">已完成 {=StepUserMission.FinishPercent * 100}%</div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <!--[/if]-->
                                                </div>
                                            </div>
                                            <!--[/if]-->
                                            
                                            <!--[/if]-->
                                            
                                            <div class="clearfix mission-action">
                                                <form action="$_form.action" method="post">
                                                <!--[if $IsShowApplyMissionButton]-->
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="参与任务" name="applymission" /></span></span>
                                                <!--[else if $IsShowApplyMissionAgainButton]-->
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="再次参与" name="applymissionagain" /></span></span>
                                                <!--[else if $IsShowReApplyMissionButton]-->
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="重新参与" name="reapplymission" /></span></span>
                                                <!--[else if $IsShowGetMissionPrizeButton]-->
                                                <span class="minbtn-wrap minbtn-highlight"><span class="btn"><input class="button" type="submit" value="领取奖励" name="getmissionprize" /></span></span>
                                                <!--[else if $IsShowGetStepMissionPrizeButton]-->
                                                <span class="minbtn-wrap minbtn-highlight"><span class="btn"><input class="button" type="submit" value="领取奖励，并进行下个任务" name="getmissionprize" /></span></span>
                                                <!--[else if $UserMission == null]-->
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="参与任务" disabled="disabled" /></span></span>
                                                <p class="mission-note">您暂时不符合参与该任务的条件.</p>
                                                <!--[/if]-->
                                                <!--[if $IsShowAbandonMissionButton]-->
                                                <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" value="放弃任务" name="abandonmission" /></span></span>
                                                <!--[/if]-->
                                                </form>
                                            </div>
                                            
                                        </div>
                                        
                                        <!--[if $usercount > 0]-->
                                        <div class="mission-participant">
                                            <h3 class="title">参与该任务的用户 ($usercount)</h3>
                                            <div class="clearfix participantlist">
                                                <ul class="participant-list">
                                                <!--[loop $user in $UserList]-->
                                                    <li>
                                                        <div class="avatar">
                                                            $user.MissionUser.AvatarLink
                                                        </div>
                                                        <div class="name">$user.MissionUser.PopupNameLink</div>
                                                        <!--<div class="date"></div> -->
                                                    </li>
                                                 <!--[/loop]-->
                                                </ul>
                                            </div>
                                            
                                            <!--[pager name="pager1" skin="../_inc/_pager_app.aspx" count="$UserCount" pagesize="20" ]-->
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
