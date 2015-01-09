<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$pagetitle</title>
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
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/.gif);">充值</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a href="$url(my/pay)"><span>充值</span></a></li>
                                        <li><a class="current" href="$url(my/paylog)"><span>充值记录</span></a></li>
                                    </ul>
                                </div>
                            </div>
                            <!--[if $userpaylist.TotalRecords > 0]-->
                            <form action="$_form.action" method="post">
                            <div class="clearfix topuplogcaption">
                                <div class="filtertab">
                                    <ul class="clearfix tab-list">
                                        <li><a href="$url(my/paylog)?state=1" $_if($State=="1",'class="current"')><span>成功记录</span></a></li>
                                        <li><a href="$url(my/paylog)?state=2" $_if($State=="2",'class="current"')><span>全部记录</span></a></li>
                                        <li><a href="$url(my/paylog)?state=0" $_if($State=="0",'class="current"')><span>失败记录</span></a></li>
                                    </ul>
                                </div>
                                <div class="topuplog-inquire">
                                    <div class="topuplog-inquire-form">
                                        起始日期
                                        <input name="begindate" id="begindate" type="text" class="text" value="$_form.text('begindate',$filter.begindate)" />
                                        截止日期
                                        <input name="enddate" id="enddate" type="text" class="text" value="$_form.text('enddate',$filter.enddate)" />
                                    </div>
                                    <span class="minbtn-wrap"><span class="btn"><input class="button" type="submit" name="search" value=" 查询 " /></span></span>
                                </div>
                            </div>
                            <div class="topuplogtable">
                                <table class="topuplog-table">
                                    <thead>
                                        <tr>
                                            <td class="number">流水号</td>
                                            <td class="date">交易时间</td>
                                            <td class="payment">充值类型</td>
                                            <td class="payment">充值数量</td>
                                            <td class="payment">支付方式</td>
                                            <td class="value">实际支付金额</td>
                                            <td class="value">充值状态</td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                     <!--[loop $usertype in $userpaylist]-->
                                        <tr>
                                        
                                            <td class="number">$usertype.orderNo</td>
                                            <td class="date">$_if($usertype.paydate.year<1900,"--",$outputdatetime($usertype.paydate))</td>
                                            <td class="date">$usertype.paytypename</td>
                                            <td class="date">$usertype.payvalue</td>
                                            <td>$usertype.paymentname</td>
                                            <td class="value">$usertype.orderamount</td>
                                             <td class="value">$usertype.paystate</td>
                                        </tr>
                                    <!--[/loop]-->
                                    </tbody>
                                </table>
                            </div>
                            <!--[pager name="pager1" skin="../_inc/_pager_app.aspx" Count="$userpaylist.totalRecords" PageSize="$PageSize" ]-->
                            </form>
                            <!--[else]-->
                            <div class="nodata">没有获取到相应交易记录.</div>
                            <!--[/if]-->
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
