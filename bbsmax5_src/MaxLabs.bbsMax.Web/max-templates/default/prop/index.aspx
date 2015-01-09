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
                                        <li><a class="current" href="$url(prop/index)"><span>道具中心</span></a></li>
                                        <li><a href="$url(prop/my)"><span>我的道具</span></a></li>
                                        <li><a href="$url(prop/selling)"><span>二手市场</span></a></li>
                                        <li><a href="$url(prop/log)"><span>道具记录</span></a></li>
                                    </ul>
                                </div>
                            </div>
                        
                            <div class="clearfix workspace app-prop">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">

                                        <!--[if $proplist.count > 0]-->
                                        <div class="proplist">
                                            <ul class="prop-list">
                                                <!--[loop $prop in $proplist]-->
                                                <li class="propitem">
                                                    <div class="clearfix propitem-inner">
                                                        <div class="prop-thumb">
                                                            <div class="prop-icon">
                                                                <img src="$prop.IconUrl" alt="" />
                                                            </div>
                                                        </div>
                                                        <div class="prop-entry">
                                                            <h3 class="prop-title">
                                                                $prop.Name
                                                            </h3>
                                                            <div class="prop-description">
                                                                $prop.DescriptionShort(40)
                                                            </div>
                                                        </div>
                                                        <div class="prop-price">
                                                            $GetPriceName($prop)
                                                            <span class="value">$prop.Price</span>$GetPriceUnit($prop)
                                                        </div>
                                                        <div class="prop-store">
                                                            库存 <strong class="numeric">$prop.TotalNumber</strong>
                                                        </div>
                                                        <div class="prop-action">
                                                            <span class="minbtn-wrap"><span class="btn"><a class="button" href="$Dialog/prop-buy.aspx?id=$prop.propid" onclick="return openDialog(this.href)">购买</a></span></span>
                                                        </div>
                                                    </div>
                                                </li>
                                                <!--[/loop]-->
                                                <li class="clear">&nbsp;</li>
                                            </ul>
                                        </div>
                                        <!--[pager name="pager" skin="../_inc/_pager_app.aspx" count="$TotalPropCount" pagesize="$PropListPageSize" ]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            暂时没有任何道具.
                                        </div>
                                        <!--[/if]-->
                                        
                                    </div>
                                </div>
                                <div class="workspace-sidebar">
                                    <div class="workspace-sidebar-inner">
                                        <!--#include file="_propsidebar_.aspx"-->
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
