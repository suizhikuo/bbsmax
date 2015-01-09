<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="../_inc/_htmlhead.aspx"-->
<link rel="stylesheet" type="text/css" href="$skin/styles/my.css" />
</head>
<body>
<div class="container section-mythreads">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main hasappsidebar">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            <div class="clearfix pagecaption">
                                <h3 class="pagecaption-title"><span style="background-image:url($Root/max-assets/icon/topic.gif);">我的论坛主题</span></h3>
                                <div class="clearfix pagecaption-body">
                                    <ul class="pagetab">
                                        $GetMyThreadTypeLinks(@"<li><a href=""{0}""><span>{1}</span></a></li>",@"<li><a class=""current"" href=""{0}""><span>{1}</span></a></li>","")
                                    </ul>
                                </div>
                            </div>
                            <div class="clearfix workspace">
                                <div class="workspace-content">
                                    <div class="clearfix workspace-content-inner">
                                        <!--[if $threadList.count > 0]-->
                                        <div class="mythreadstable">
                                            <table>
                                                <thead>
                                                    <tr>
                                                        <td class="title">主题</td>
                                                        <td class="category">所属版块</td>
                                                        <td class="published">发布时间</td>
                                                        <td class="last">最后回复</td>
                                                        <td class="stats">回复/查看</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <!--[loop $thread in $threadList]-->
                                                    <tr>
                                                        <td class="title">$GetThreadCatalogName($thread.ThreadCatalogID,"[{0}]") $GetThreadLink($thread, 60,@"<a href=""{0}"" target=""_blank"">{1}</a>", false)</td>
                                                        <td class="category">$thread.Forum.ForumName</td>
                                                        <td class="published"><a class="fn" href="$url(space/$thread.PostUserID)" target="_blank">$thread.PostUsername</a> <span class="date">$outputFriendlyDateTime($thread.CreateDate)</span></td>
                                                        <td class="last"><a class="fn" href="$url(space/$thread.LastReplyUserID)" target="_blank">$thread.LastReplyUsername</a> <span class="date">$outputFriendlyDateTime($thread.UpdateDate)</span></td>
                                                        <td class="stats">$thread.TotalReplies / $thread.TotalViews</td>
                                                    </tr>
                                                    <!--[/loop]-->
                                                </tbody>
                                            </table>
                                        </div>
                                        <!--[pager name="list" skin="../_inc/_pager_app.aspx"]-->
                                        <!--[else]-->
                                        <div class="nodata">
                                            当前没有此类主题.
                                        </div>
                                        <!--[/if]-->
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
