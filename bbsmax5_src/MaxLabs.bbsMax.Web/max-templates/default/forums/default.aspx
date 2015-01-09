<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>$PageTitle</title>
<!--#include file="_forumhtmlhead_.aspx"-->
<link rel="alternate" type="application/rss+xml" href="$root/rss.aspx?ticket=$ticket" title="$PageTitle" />
</head>
<body>
<div class="container section-forumindex">
<!--#include file="../_inc/_head.aspx"-->
    <div id="main" class="main $_if($IsShowSideBar, 'rightsidebar', 'nosidebar')">
        <div class="clearfix main-inner">
            <div class="content">
                <div class="clearfix content-inner">
                    <div class="content-main">
                        <!--#include file="../_inc/_round_top.aspx"-->
                        <div class="content-main-inner">
                            
                            <div class="clearfix forumtoptext">
                                <div id="forumnotice" class="forumnotice">
                                    <h3 class="forumnotice-label">公告</h3>
                                    <ul class="forumnotice-list" id="forumnotice_list">
                                        <!--[if $announcements.count == 0]-->
                                        <li>没有任何公告</li>
                                        <!--[else]-->
                                            <!--[loop $announcement in $announcements]-->
                                            <!--[if $announcement.AnnouncementType == AnnouncementType.Text]-->
                                        <li><a href="$url(announcements)?id=$announcement.announcementID#$announcement.announcementID">$announcement.Subject</a> <span class="date">$outputFriendlyDateTime($announcement.BeginDate)</span></li>
                                            <!--[else]-->
                                        <li><a href="$formatLink($announcement.Content)">$announcement.Subject</a> <span class="date">$outputFriendlyDateTime($announcement.BeginDate)</span></li>
                                            <!--[/if]-->
                                            <!--[/loop]-->
                                        <!--[/if]-->
                                    </ul>
                                </div>
                                <!--[if $announcements.count > 1]-->
                                <script type="text/javascript">
                                    announcement("forumnotice_list");
                                </script>
                                <!--[/if]-->
                                <!--[if $Islogin && $PageName=="default"]-->
                                <form id="sidebarToggle" method="post" action="$_form.action">
                                <div class="sidebar-toggle">
                                    <a class="$_if($IsShowSideBar, 'toggle-collapse', 'toggle-expand')" href="javascript:void(0);" title="$_if($IsShowSideBar, '关闭侧栏', '打开侧栏')" onclick="clickSidebarToggle();return false;"><span>$_if($IsShowSideBar, '关闭侧栏', '打开侧栏')</span></a>
                                </div>
                                </form>
                                <!--[/if]-->






                            </div>
                            <!--[if $ForumCatalogs.Count > 0]-->
                            <!--[loop $forumCatalog in $ForumCatalogs with $LoopIndex]-->
                                <!--[if $forumCatalog.ColumnSpan <= 1]-->
                            <div class="panel forumthumbs">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span><a href="$url($forumCatalog.CodeName/list-1)">$forumCatalog.ForumName</a></span></h3>
                                    <p class="panel-toggle"><a class="collapse" href="#" title="收起/展开" id="collapse_$LoopIndex" onclick="Collapse(this, $forumCatalog.ForumID);return false;">收起/展开</a></p>
                                    <!--[if $forumCatalog.Moderators.Count > 0]-->
                                    <p class="divisionmaster">版主: $GetModeratorLinks($forumCatalog,@"<a class=""fn"" href=""{0}"">{1}</a>",", ")</p>
                                    <!--[/if]-->
                                </div></div></div>
                                <div class="panel-body" id="block_$forumCatalog.ForumID">
                                    <!--[if $forumCatalog.SubForumsForList.count == 0]-->
                                    <div class="nodata">
                                        该版块没有任何子版块.
                                    </div>
                                    <!--[else]-->
                                    <table>
                                        <!--[loop $forum in $forumCatalog.SubForumsForList]-->
                                        <tr>
                                            <td class="forum-info">
                                                <dl class="forumunit">
                                                    <dt class="title">
                                                        <a href="$url($forum.CodeName/list-1)" <!--[if $forum.ForumType==ForumType.Link && $forum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$forum.ForumName</a>
                                                        <!--[if $forum.TodayPostsWithSubForums>0]-->
                                                        <span class="status">(今日:<em>$forum.TodayPostsWithSubForums</em>)</span>
                                                        <!--[/if]-->
                                                    </dt>
                                                    <dd class="image">
                                                        <a href="$url($forum.CodeName/list-1)" target="_blank">
                                                        <!--[if $forum.logoUrl!=""]-->
                                                            <img src="$forum.logoUrl" alt="" />
                                                        <!--[else]-->
                                                            <img src="$skin/images/icons/topicicon_default.png" alt="" />
                                                        <!--[/if]-->
                                                        <!--[if $forum.ForumType!=ForumType.Link && $forum.Password != ""]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_lock.png" alt="" title="加密版块" />
                                                        <!--[else if $forum.CanVisit($my)]-->
                                                            <!--[if $forum.ForumType == ForumType.Link]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_link.png" alt="" title="外部版块" />
                                                            <!--[else]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_accessible.png" alt="" title="有访问权" />
                                                            <!--[/if]-->
                                                        <!--[else]-->
                                                            <img class="overlayicon" src="$skin/images/icons/overlay_ban.png" alt="" title="无访问权" />
                                                        <!--[/if]-->
                                                        </a>
                                                    </dd>
                                                    <dd class="intro">
                                                        $forum.Description
                                                    </dd>
                                                    <!--[if $DisplaySubforumsInIndexpage && $forum.SubForumsForList.Count>0]-->
                                                    <dd class="childforum">
                                                        子版块:
                                                        <!--[loop $subForum in $forum.SubForumsForList with $i]-->
                                                        <a href="$url($subForum.CodeName/list-1)" <!--[if $subForum.ForumType==ForumType.Link && $subForum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$subForum.ForumName</a><!--[if $i < $forum.SubForumsForList.Count-1]-->, <!--[/if]-->
                                                        <!--[/loop]-->
                                                    </dd>
                                                    <!--[/if]-->
                                                    <dd class="boardmaster">
                                                        版主: $GetModeratorLinks($forum,@"<a class=""fn"" href=""{0}"">{1}</a>{2}",", ")
                                                    </dd>
                                                </dl>
                                            </td>
                                            <td class="forum-stats">
                                                <span class="stats-topic">$forum.TotalThreadsWithSubForums</span> /
                                                <span class="stats-post">$forum.TotalPostsWithSubForums</span>
                                            </td>
                                            <td class="forum-last">
                                                <!--[if $CanSeeLastUpdate($forum)]-->
                                                    <!--[if $forum.LastThread == null]-->
                                                <p class="lastpost-nopost">
                                                    暂时没有主题
                                                </p>
                                                    <!--[else]-->
                                                <p class="lastpost-title">
                                                    <a href="$url($forum.codename/$forum.LastThread.ThreadTypeString-$forum.LastThread.ThreadID-$forum.LastThread.TotalPages-1)#last">$CutString($forum.LastThread.SubjectText,30)</a>
                                                </p>
                                                <p class="lastpost-user">
                                                    <!--[if $forum.LastThread.LastReplyUserID != 0]-->
                                                    <a class="fn" href="$url(space/$forum.LastThread.LastReplyUserID)" target="_blank">$forum.LastThread.LastReplyUsername</a>
                                                    <!--[else]-->
                                                        <!--[if $forum.LastThread.LastReplyUsername != ""]-->
                                                    游客:$forum.LastThread.LastReplyUsername
                                                        <!--[else]-->
                                                    匿名游客
                                                        <!--[/if]-->
                                                    <!--[/if]-->
                                                    <span class="date">更新于 $outputFriendlyDateTime($forum.LastThread.UpdateDate)</span>
                                                </p>
                                                    <!--[/if]-->
                                                <!--[else]-->
                                                <p class="inaccessible">
                                                    未获得查看权限
                                                </p>
                                                <!--[/if]-->
                                            </td>
                                        </tr>
                                        <!--[/loop]-->
                                    </table>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                                <!--[else]-->
                            <div class="panel forumcates">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span><a href="$url($forumCatalog.CodeName/list-1)">$forumCatalog.ForumName</a></span></h3>
                                    <p class="panel-toggle"><a class="collapse" href="#" title="收起/展开" id="collapse_$LoopIndex" onclick="Collapse(this, $forumCatalog.ForumID);return false;">收起</a></p>
                                    <!--[if $forumCatalog.Moderators.Count > 0]-->
                                    <p class="divisionmaster">分区版主: $GetModeratorLinks($forumCatalog,@"<a class=""fn"" href=""{0}"">{1}</a>",", ")</p>
                                    <!--[/if]-->
                                </div></div></div>
                                <div class="panel-body" id="block_$forumCatalog.ForumID">
                                <!--[loop $forum in $forumCatalog.SubForumsForList with $LoopIndex]-->
                                    <!--[if ($LoopIndex+1) % ($forumCatalog.ColumnSpan) == 1]-->
                                    <ul class="clearfix multicolumns$forumCatalog.ColumnSpan">
                                    <!--[/if]-->
                                        <li>
                                            <dl class="forumunit">
                                                <dt class="title">
                                                    <a href="$url($forum.CodeName/list-1)" <!--[if $forum.ForumType==ForumType.Link && $forum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$forum.ForumName</a>
                                                    <!--[if $forum.TodayPostsWithSubForums>0]-->
                                                    <span class="status">(今日:<em>$forum.TodayPostsWithSubForums</em>)</span>
                                                    <!--[/if]-->
                                                </dt>
                                                <dd class="image">
                                                    <a href="$url($forum.CodeName/list-1)" target="_blank">
                                                        <!--[if $forum.logoUrl!=""]-->
                                                        <img src="$forum.logoUrl" alt="" />
                                                        <!--[else]-->
                                                        <img src="$skin/images/icons/topicicon_default.png" alt="" />
                                                        <!--[/if]-->
                                                        <!--[if $forum.ForumType!=ForumType.Link && $forum.Password != ""]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_lock.png" alt="" title="加密版块" />
                                                        <!--[else if $forum.CanVisit($my)]-->
                                                            <!--[if $forum.ForumType == ForumType.Link]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_link.png" alt="" title="外部版块" />
                                                            <!--[else]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_accessible.png" alt="" title="有访问权" />
                                                            <!--[/if]-->
                                                        <!--[else]-->
                                                        <img class="overlayicon" src="$skin/images/icons/overlay_ban.png" alt="" title="无访问权" />
                                                        <!--[/if]-->
                                                    </a>
                                                </dd>
                                                <dd class="stat">
                                                    主题: <span class="stat-topic">$forum.TotalThreadsWithSubForums</span>,
                                                    帖数: <span class="stat-topic">$forum.TotalPostsWithSubForums</span>
                                                </dd>
                                                <!--[if $DisplaySubforumsInIndexpage && $forum.SubForumsForList.Count > 0]-->
                                                <dd class="childforum">
                                                    子版块:
                                                    <!--[loop $subForum in $forum.SubForumsForList with $i]-->
                                                        <a href="$url($subForum.CodeName/list-1)" <!--[if $subForum.ForumType==ForumType.Link && $subForum.ExtendedAttribute.LinkOpenByNewWidow]-->target="_blank"<!--[/if]-->>$subForum.ForumName</a><!--[if $i < $forum.SubForumsForList.Count-1]-->, <!--[/if]-->
                                                    <!--[/loop]-->
                                                </dd>
                                                <!--[/if]-->
                                                <dd class="last">
                                                    最后发表:
                                                    <!--[if $forum.LastThread != null ]-->
                                                    <a href="$url($forum.codename/$forum.LastThread.ThreadTypeString-$forum.LastThread.ThreadID-$forum.LastThread.TotalPages-1)#last">$outputFriendlyDateTime($forum.LastThread.UpdateDate)</a>
                                                    <!--[else]-->
                                                    -
                                                    <!--[/if]-->
                                                </dd>
                                            </dl>
                                        </li>
                                    <!--[if ($LoopIndex+1) % ($forumCatalog.ColumnSpan)==0 || ($LoopIndex+1) == $forumCatalog.SubForumsForList.Count]-->
                                    </ul>
                                    <!--[/if]-->
                                <!--[/loop]-->
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                                <!--[/if]-->
                            
                            <!--[if $hasInForumAD]-->
                            <div class="ad-banner-forumdivide">$InForumAD</div>
                            <!--[/if]-->
                            
                            <!--[/loop]-->
                            <!--[else]-->
                            <div class="panel forumthumbs">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>没有版块</span></h3>
                                </div></div></div>
                                <div class="panel-body">
                                    <div class="nodata">
                                        论坛未建任何版块, 无法发布内容. 请使用管理员帐号登录, 并在后台管理面板中添加版块.
                                    </div>
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            <!--[/if]-->
                            
                            <!--[if $imgLinks.Count > 0 || $textLinks.Count > 0]-->
                            <div class="panel links">
                                <div class="panel-head"><div class="panel-head-inner"><div class="clearfix panel-head-inner">
                                    <h3 class="panel-title"><span>联盟论坛</span></h3>
                                    <p class="panel-toggle"><a class="collapse" href="#" title="收起/展开" id="collapse_link" onclick="Collapse(this, 'link');return false;">收起/展开</a></p>
                                </div></div></div>
                                <div class="panel-body" id="block_link">
                                    <!--[if $imgLinks.Count > 0]-->
                                    <div class="clearfix links-image">
                                        <!--[loop $link in $imgLinks]-->
                                        <a href="$FormatLink($link.Url)" title="$link.Description" target="_blank"><img src="$link.imageUrl" alt="$link.Name" width="88" height="31" /></a>
                                        <!--[/loop]-->
                                    </div>
                                    <!--[/if]-->
                                    <!--[if $textLinks.Count > 0]-->
                                    <div class="clearfix links-text">
                                        <!--[loop $link in $textLinks]-->
                                        <a href="$FormatLink($link.Url)" title="$link.Description" target="_blank">$link.Name</a>
                                        <!--[/loop]-->
                                    </div>
                                    <!--[/if]-->
                                </div>
                                <div class="panel-foot"><div><div>-</div></div></div>
                            </div>
                            <!--[/if]-->

                            <!--[if $IsShowOnline]-->
                            <!--#include file="_forumonline_.aspx"-->
                            <!--[/if]-->
                            
                            <div class="forumsign">
                                <div class="forumsign-inner">
                                    <img src="$skin/images/icons/overlay_accessible.png" alt="" /> 有权访问
                                    <img src="$skin/images/icons/overlay_ban.png" alt="" /> 无权访问
                                    <img src="$skin/images/icons/overlay_link.png" alt="" /> 外部版块
                                    <img src="$skin/images/icons/overlay_lock.png" alt="" /> 加密版块
                                </div>
                            </div>
                        </div>
                        <!--#include file="../_inc/_round_bottom.aspx"-->
                    </div>
                    <!--[if $IsShowSideBar]-->
                    <!--#include file="_forumsidebar_.aspx"-->
                    <!--[/if]-->
                </div>
            </div>
        </div>
    </div>
<!--#include file="../_inc/_foot.aspx"-->
</div>
<script type="text/javascript">
// 版块伸缩
function  Collapse (th, index) {
    var block = $('block_' + index);
    var display;
    if (block.style.display == 'none') {
        display = 'block';
    }
    else {
        display = 'none';
    }
    block.style.display = display;
}
function clickSidebarToggle(){
    if(sidebarToggle()){
        clickButton('Default_Open_Sidebar','sidebarToggle');
    }
    else{
        clickButton('Default_Close_Sidebar','sidebarToggle');
    }
}

function sidebarToggle() {
    var elem = $("main");
    if (elem) {
        var ec = elem.className;
        var nextyear = new Date( );
        nextyear.setFullYear(nextyear.getFullYear() + 1);
        document.cookie = "version=" + document.lastModified + "; expires=" + nextyear.toGMTString() + ";";

        if (ec.indexOf("nosidebar") != -1) {
            return true;
        } else {
            return false;
        }
    }
}
</script>
</body>
</html>
