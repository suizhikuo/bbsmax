<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
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
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/.gif);">充值</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a class="current" href="$url(my/pay)"><span>充值</span></a></li>
                                        <li><a href="$url(my/paylog)"><span>充值记录</span></a></li>
                                    </ul>
                                </div>
                            </div>
                           
                            <!--[if $userpay.State==false]-->
                            <div class="topuptip topuptip-error">
                                <h3 class="topuptip-title">充值过程出现错误.</h3>
                                <p>流水号: <span class="tradenumber">$userpay.orderno</span></p>
                            </div>
                            <!--[else]-->
                            <div class="topuptip topuptip-success">
                                <h3 class="topuptip-title">成功充值 <strong>$userpay.payvalue</strong> 个$userpay.paytypename. 此次交易共花费实际金额 <strong>$userpay.orderamount</strong> 元</h3>
                                <p>流水号: <span class="tradenumber">$userpay.orderno</span></p>
                            </div>
                            <!--[/if]-->
                            <ul class="topuptip-link">
                                <li><a href="$url(my/pay)">继续为账号充值</a></li>
                                <li><a href="$url(my/paylog)">查看账号充值记录</a></li>
                                 <!--[if $userpay.State==true]-->
                                <li><a href="$url(my/default)">充值成功, 返回中心首页</a></li>
                                <!--[/if]-->
                            </ul>
                            
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
