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
                                <h3 class="pagecaption-title"><span>我的积分</span></h3>
                            </div>
                            <div class="formcaption">
                                <h3>积分概况</h3>
                                <p>显示个人积分情况, 以及各种类型积分在网站内的排名.</p>
                            </div>
                            
                            <div class="formgroup mypoint">
                                <div class="formrow">
                                    <h3 class="label">总积分</h3>
                                    <div class="form-enter">
                                        <span class="numeric">$my.points</span>
                                        <a class="viewrank" href="$url(members)?view=point">查看排名</a>
                                    </div>
                                </div>
                                <!--[loop $point in $PointList]-->
                                <div class="formrow">
                                    <h3 class="label">$point.name</h3>
                                    <div class="form-enter">
                                        <span class="numeric">$point.value $point.icon</span>
                                    </div>
                                </div>
                                <!--[/loop]-->
                            </div>
                            
                            <div class="formcaption">
                                <h3>积分图标升级说明</h3>
                                <p>积分图标升级公式:</p>
                            </div>
                            <div class="formgroup pointiconintro">
                                <div class="formrow">
                                    <h3 class="label">$GeneralPointName</h3>
                                    <div class="form-enter">
                                        $GeneralPointPointIconUpdateDescription
                                    </div>
                                </div>
                                <!--[EnabledUserPointList]-->
                                <div class="formrow">
                                    <h3 class="label">$Name</h3>
                                    <div class="form-enter">
                                        $GetPointIconUpdateDescription($type)
                                    </div>
                                </div>
                                <!--[/EnabledUserPointList]-->
                            </div>

                            <div class="formcaption" id="pointactionlist">
                                <h3>积分规则</h3>
                                <p>以下为每个应用, 论坛版块等不同操作对各种类型积分的影响情况.</p>
                            </div>
                            <div class="panel pointrules">
                                <div class="panel-head">
                                    <div class="panel-tab pointrules-cate">
                                        <ul class="clearfix pointrules-catelist">
                                        <!--[PointActionTypeList]-->
                                        <!--[item]-->
                                            <li>
                                                <a $IsSeclected($_get.type,'class="current"') href="$url(my/point)?type=$pointActionType.type#pointactionlist"><span>$pointActionType.Name</span></a>
                                            </li>
                                        <!--[/item]-->
                                        <!--[/PointActionTypeList]-->
                                        </ul>
                                    </div>
                                </div>
                                <div class="panel-body <!--[if $pointActionType.HasNodeList == true]-->pointrules-layout-col2<!--[/if]-->">
                                    <!--[if $pointActionType.HasNodeList == true]-->
                                    <div class="pointrules-subcate">
                                        $pointActionType.NodeItemList.GetTreeHtml("<ul>{0}</ul>","<li><a href=\"{4}?type=$pointActionType.type&nodeID={0}#pointactionlist\" class=\"{3}\"><span>{1}</span>{2}</a></li>","","current",$NodeID,"my/point")
                                    </div>
                                    <!--[/if]-->
                                    <div class="pointrules-content">
                                        <div class="pointrules-content-inner">
                                    <!--[if $PointActionType.ActionAttributes.Values.count > 0]-->
                                    <table class="pointrules-list">
                                        <thead>
                                            <tr>
                                                <td>操作</td>
                                                <!--[EnabledUserPointList]-->
                                                <td class="item">$Name</td>
                                                <!--[/EnabledUserPointList]-->
                                            </tr>
                                        </thead>
                                        <tbody>
                                        <!--[loop $actionItem in $PointActionType.ActionAttributes.Values]-->
                                            <!--[if $IsShow($actionItem) == true]-->
                                            <tr>
                                                <td><strong>$actionItem.ActionName</strong></td>
                                                <!--[EnabledUserPointList]-->
                                                <td class="item">
                                                    $GetPointValue($actionItem.Action,$pointID,@"<span class=""add"">{0}</span>",@"<span class=""del"">{0}</span>",@"<span>{0}</span>")
                                                </td>
                                                <!--[/EnabledUserPointList]-->
                                            </tr>
                                            <!--[/if]-->
                                        <!--[/loop]-->
                                        </tbody>
                                    </table>
                                    <!--[else]-->
                                    <div class="setting-message">
                                        <h3>未制定积分规则</h3>
                                        <p>该应用或版块没有对各种不同操作制定任何积分规则.</p>
                                    </div>
                                    <!--[/if]-->
                                        </div>
                                    </div>
                                </div>
                                
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
