<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_spacehtmlhead.aspx"-->
</head>
<body>
<div class="container section-space section-space-activity">
<!--#include file="_spacethemetip.aspx"-->
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main">
        <!--#include file="../_inc/_round_top.aspx"-->
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <div class="content-main-inner">

                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/leavemsg.gif);">$AppOwner.Username 的动态</span></h3>
                            </div>
                            
                            <div class="panel activities">
                                <div class="panel-body">
                                <!--[ajaxpanel id="ap_feeds" idonly="true"]-->
                                <!--[if $feedList.Count > 0]-->
                                    <!--#include file="_feedbody.aspx"-->
                                    <!--[if $haveMoreFeeds]-->
                                    <div class="ajaxloading" style="display:none" id="ajaxsending">
                                        <span>加载中...</span>
                                    </div>
                                    <div class="showallactivities" id="getmorediv">
                                        <a href="javascript:;" onclick="javascript:$('ajaxsending').style.display='';$('getmorediv').style.display='none';ajaxRender('$AttachQueryString("getcount="+$NextGetCount)','ap_feeds',ajaxCallback);return false;">更多动态</a>
                                    </div>
                                    <!--[/if]-->
                                <!--[else if $FeedCanDisplay == false]-->
                                    <div class="nodata">
                                        由于空间主人对动态设置了隐私您无法查看他/她的动态.
                                    </div>
                                <!--[else]-->
                                    <div class="nodata">
                                        当前没有任何动态.
                                    </div>
                                <!--[/if]-->
                                <!--[/ajaxpanel]-->
                                </div>
                            </div>
                                
                        </div>
                    </div>
                    <!--include file="_spacecontentsub.aspx"-->
                </div>
            </div>
            <!--#include file="_spacesidebar.aspx"-->
        </div>
        <!--#include file="../_inc/_round_bottom.aspx"-->
    </div>
    <!--#include file="../_inc/_foot.aspx"-->
</div>
</body>
</html>

