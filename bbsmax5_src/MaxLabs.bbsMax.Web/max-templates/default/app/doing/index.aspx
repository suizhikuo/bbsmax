<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/app.css" />
<!--[if $IsSpace]-->
<link rel="stylesheet" type="text/css" href="$skin/styles/space.css" />
<!--/* 用户空间自定义样式 */-->

<!--[/if]-->
<script type="text/javascript" src="$root/max-assets/javascript/max-commentlist.js"></script>
</head>
<body>
<div class="container" id="container">
<!--#include file="../../_inc/_head.aspx"-->
    <div id="main" class="main <!--[if $IsSpace]--> section-space<!--[else]--> hasappsidebar section-app<!--[/if]-->">
        <!--[if $IsSpace]--><!--#include file="../../_inc/_round_top.aspx"--><!--[/if]-->
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/doing.gif);"><!--[if $IsSpace]-->$AppOwner.Username 的<!--[/if]-->$FunctionName</span></h3>
                                <!--[if $IsSpace == false]-->
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        <li><a $_if(SelectedEveryone, 'class="current" ' ) href="$url(app/doing/index)?view=everyone"><span>大家的$FunctionName</span></a></li>
                                        <li><a $_if(SelectedFriend, 'class="current" ' ) href="$url(app/doing/index)?view=friend"><span>好友的$FunctionName</span></a></li>
                                        <li><a $_if(SelectedMy, 'class="current" ' ) href="$url(app/doing/index)?view=my"><span>我的$FunctionName</span></a></li>
                                        <li><a $_if(SelectedCommented, 'class="current" ' ) href="$url(app/doing/index)?view=commented"><span>我评论的$FunctionName</span></a></li>
                                    </ul>
                                </div>
                                <!--[/if]-->
                            </div>
                            
                            <div class="clearfix workspace app-status">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <!--[if $IsShowDoingForm]-->
                                        <form name="doingform" method="post" action="$_form.action">
                                        <div class="clearfix statusform">
                                            <div class="statusform-avatar avatar">
                                                <a href="$url(space/$my.id)"><img src="$my.avatarpath" alt="" width="48" height="48" /></a>
                                            </div>
                                            <div class="statusform-content">
                                                <div class="bubble-nw"><div class="bubble-ne"><div class="bubble-n"><div class="bubble-pointer">&nbsp;</div></div></div></div>
                                                <div class="bubble-e"><div class="clearfix bubble-w">
                                                <!--[unnamederror form="doingform"]-->
                                                <div class="errormsg">$message</div>
                                                <!--[/unnamederror]-->
                                                <div class="statusform-textarea">
                                                    <textarea cols="70" rows="2" id="message" name="content" onkeyup="textCounter(this, 'maxlimit', $MaxDoingLength)"></textarea>
                                                </div>
                                                <div class="clearfix statusform-action">
                                                    <div class="statusform-submit">
                                                        <span class="statusform-tip">
                                                            还可输入<strong class="numeric" id="maxlimit">$MaxDoingLength</strong>个字符
                                                        </span>
                                                        <span class="btn-wrap"><span class="btn"><input type="submit" id="adddoing" name="adddoing" class="button" value="发布" /></span></span>
                                                    </div>
                                                </div>
                                                
                                                </div></div>
                                                <div class="bubble-sw"><div class="bubble-se"><div class="bubble-s">&nbsp;</div></div></div>
                                            </div>
                                        </div>
                                        </form>
                                        <!--[/if]-->
                                        <!--[if $DoingList.count > 0]-->
                                        <div class="hfeed statuslist">
                                        <ul class="status-list">
                                            <!--[loop $doing in $DoingList]-->
                                            <li class="statusitem" id="doingid_$doing.id">
                                                <div class="avatar status-avatar">
                                                    <a href="$url(space/$doing.User.id)"><img src="$doing.User.Avatarpath" alt="" width="48" height="48" /></a>
                                                </div>
                                                <div class="status-content">
                                                    <div class="status-content-inner">
                                                        <div class="clearfix hentry status-entry">
                                                            <div class="entry-content">
                                                                <div class="bubble-nw"><div class="bubble-ne"><div class="bubble-n"><div class="bubble-pointer">&nbsp;</div></div></div></div>
                                                                <div class="bubble-e"><div class="clearfix bubble-w">
                                                                    <span class="vcard author"><a class="url fn" href="$url(space/$doing.User.id)">$doing.User.name</a></span>
                                                                    $doing.Content
                                                                </div></div>
                                                                <div class="bubble-sw"><div class="bubble-se"><div class="bubble-s">&nbsp;</div></div></div>
                                                            </div>
                                                            <div class="entry-meta">
                                                                <span class="published" title="$doing.CreateDate">$doing.FriendlyCreateDate</span> -
                                                                <a href="javascript:void(0)" onclick="expandReply($doing.id)"><span id="comment_button_$doing.id">收起评论</span></a>
                                                            </div>
                                                        </div>
                                                        <div id="comment_div_$doing.id" class="entry-comment statuscomment">
                                                        <!-- #include file="../../_inc/_commentlist.aspx" targetid="$doing.id" targetCommentCount="$doing.commentlist.count" commentList="$GetComments($doing)" IsShowGetAll="$IsShowGetAll($doing)" TotalComments="$doing.TotalComments" commentType="doing" IsShowCommentPager="$IsShowCommentPager($doing)" -->
                                                        </div>
                                                    </div>
                                                </div>
                                                <!--[if $Doing.CanDelete]-->
                                                <div class="entry-action status-action">
                                                    <a class="action-delete" title="删除" href="$dialog/doing-delete.aspx?id=$doing.ID" onclick="return openDialog(this.href,this, function(r){removeElement($('doingid_$doing.id'));})">删除</a>
                                                </div>
                                                <!--[/if]-->
                                                <div class="clear">&nbsp;</div>
                                            </li>
                                            <!--[/loop]-->
                                        </ul>
                                        </div>
                                        <!--[pager name="doinglist" skin="../../_inc/_pager_app.aspx"]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            当前没有发布任何记录.
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
                        <!--#include file="../../_inc/_round_bottom.aspx"-->
                    </div>
                </div>
            </div>
            <!--[if $IsSpace]-->
                <!--#include file="../../space/_spacesidebar.aspx"-->
            <!--[else]-->
                <!--#include file="../../_inc/_sidebar_app.aspx"-->
            <!--[/if]-->
        </div>
        <!--[if $IsSpace]--><!--#include file="../../_inc/_round_bottom.aspx"--><!--[/if]-->
    </div>
<!--#include file="../../_inc/_foot.aspx"-->
</div>
</body>
</html>
